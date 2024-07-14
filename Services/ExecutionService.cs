using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tesseract;

namespace ScreTran;

public class ExecutionService : IExecutionService
{
    private readonly IParametersService _parameters;
    private readonly ITranslationService _translationService;
    private readonly SettingsModel _settings;
    private readonly Timer _timer;
    private readonly TesseractEngine _engine;
    private readonly ImageAttributes _imageAttributes;

    private Task _lastTask;
    private string _lastLine;
    private float _currentBrightness;

    /// <summary>
    /// Return true if service started, otherwise false.
    /// </summary>
    private bool _isStarted;
    public bool IsStarted => _isStarted;

    public ExecutionService(ISettingsService settingsService, IParametersService parametersService, ITranslationService translationService)
    {
        _parameters = parametersService;
        _settings = settingsService.Settings;

        _isStarted = false;
        _timer = new Timer(ProccessByTimerCommands, null, 0, 700);
        _engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        _imageAttributes = new();

        _lastLine = string.Empty;
        _currentBrightness = 0;
        _translationService = translationService;
    }

    /// <summary>
    /// Starts service.
    /// </summary>
    public void Start()
    {
        _isStarted = true;
    }

    /// <summary>
    /// Stops service.
    /// </summary>
    public void Stop()
    {
        _isStarted = false;
    }

    /// <summary>
    /// Timer execution.
    /// </summary>
    private void ProccessByTimerCommands(object? state)
    {
        if (!IsStarted)
            return;

        // If previous task still running, skip creating new task.
        if (_lastTask?.Status == TaskStatus.Running)
            return;

        _lastTask = Task.Run(RecognizeTextAndTranslate);
    }

    /// <summary>
    /// Updates color matrix of image. In current realization it's only change brightness and makes image black and white.
    /// </summary>
    private void UpdateColorMatrix()
    {
        var colorMatrix = new ColorMatrix(
            new float[][] {
                       new float[] {1, 1, 1, 0, 0},        // red
                       new float[] {1, 1, 1, 0, 0},        // green
                       new float[] {1, 1, 1, 0, 0},        // blue
                       new float[] {0, 0, 0, 1, 0},        // alpha
                       new float[] { _currentBrightness, _currentBrightness, _currentBrightness, 1, 1}         // https://stackoverflow.com/questions/55660749/how-to-use-a-slider-control-to-adjust-the-brightness-of-a-bitmap
            }
        );

        // Set the new color matrix
        _imageAttributes.SetColorMatrix(colorMatrix,
           ColorMatrixFlag.Default,
           ColorAdjustType.Bitmap);
    }

    /// <summary>
    /// Gets shot of selection area, and sets color matrix.
    /// </summary>
    /// <returns>image byte array of selection area.</returns>
    private byte[] GetImage()
    {
        var selectionWindowPosition = _settings.SelectionWindowPosition;
        using var stream = new MemoryStream();
        var bitmap = new Bitmap((int)selectionWindowPosition.Width - (_parameters.SelectionBorderThickness * 2),
                                (int)selectionWindowPosition.Height - (_parameters.SelectionBorderThickness * 2));
        using (var g = Graphics.FromImage(bitmap))
        {
            // Сделать скриншот в области выбора.
            g.CopyFromScreen((int)selectionWindowPosition.Left + _parameters.SelectionBorderThickness,
                             (int)selectionWindowPosition.Top + _parameters.SelectionBorderThickness,
                             0,
                             0,
            bitmap.Size,
            CopyPixelOperation.SourceCopy);

            var bmpRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            // Отрисовать применяя матрицу.
            g.DrawImage(bitmap, bmpRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, _imageAttributes);
        }
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

        return stream.ToArray();
    }

    /// <summary>
    /// Gets image of selection area, trying recognize text from image and translates it to russian.
    /// </summary>
    private void RecognizeTextAndTranslate()
    {
        if (_currentBrightness != _settings.Brightness)
        {
            _currentBrightness = _settings.Brightness;
            UpdateColorMatrix();
        }

        try
        {
            using var img = Pix.LoadFromMemory(GetImage());
            using var page = _engine.Process(img);
            // Получение текста и удаление стандартных артефактов.
            var line = page.GetText().Replace("|", "I")
                                     .Replace(" '", "")
                                     .Replace(" \"", "")
                                     .Replace(".,.", "...")
                                     .Replace("., ", ".. ")
                                     .Trim();

            // Замена запятой на конце на точку.
            if (line.EndsWith(','))
                line = $"{line.TrimEnd(',')}.";

            // Удаление пустых строк или сторк с артефактами.
            line = RemoveShortLines(line, _settings.ShortLineThreshold);

            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line == _lastLine)
                return;

            var currentConfidence = page.GetMeanConfidence();

            if (currentConfidence < _settings.ConfidenceThreshold)
                return;

            // Установить значения.
            _parameters.Confidence = currentConfidence;
            // Перевести текст.
            _parameters.TranslatedLine = _translationService.Translate(line);

            _lastLine = line;
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.ToString());
            _parameters.Confidence = 0;
            _parameters.TranslatedLine = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Removes lines that shorter than threshold.
    /// </summary>
    private string RemoveShortLines(string text, int threshold)
    {
        return string.Join("\n", text.Split("\n").Select(l => l.Trim()).Where(l => l.Length > threshold));
    }
}