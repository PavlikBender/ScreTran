using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;

namespace ScreTran;

public class ExecutionService : IExecutionService
{
    private readonly IParametersService _parameters;
    private readonly ITranslationService _translationService;
    private readonly SettingsModel _settings;
    private  Timer _timer;
    private readonly string _regExPattern;

    private readonly PaddleOcrAll _paddleOcrAll;

    private Task _lastTask;
    private string _lastLine;

    public ExecutionService(ISettingsService settingsService, IParametersService parametersService, ITranslationService translationService)
    {
        _parameters = parametersService;
        _settings = settingsService.Settings;

        _lastLine = string.Empty;
        _translationService = translationService;

        _regExPattern = @"\W";

        _paddleOcrAll = new PaddleOcrAll(LocalFullModels.EnglishV4, PaddleDevice.Onnx())
        {
            AllowRotateDetection = false,
            Enable180Classification = false,
        };

    }

    /// <summary>
    /// Starts service.
    /// </summary>
    public void Start()
    {
        var period = (int)(1000 * _settings.Period);
        _timer = new Timer(ProccessByTimerCommands, null, 0, period);
    }

    /// <summary>
    /// Stops service.
    /// </summary>
    public void Stop()
    {
        _timer.Dispose();
    }

    /// <summary>
    /// Timer execution.
    /// </summary>
    private void ProccessByTimerCommands(object? state)
    {
        // If previous task still running, skip creating new task.
        if (_lastTask?.Status == TaskStatus.Running)
            return;

        _lastTask = Task.Run(RecognizeTextAndTranslate);
    }

    /// <summary>
    /// Gets shot of selection area, and sets color matrix.
    /// </summary>
    /// <returns>image byte array of selection area.</returns>
    private byte[] GetImage()
    {
        var selectionWindowPosition = _settings.SelectionWindowPosition;

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
            g.DrawImage(bitmap, bmpRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        return stream.ToArray();
    }

    /// <summary>
    /// Распознает текст на изображении с использованием PaddleOCR.
    /// </summary>
    /// <param name="sampleImageData">Массив байтов, представляющий изображение.</param>
    /// <returns>Распознанный текст.</returns>
    private string PaddleOCRRecognize(byte[] sampleImageData)
    {
        using var src = Cv2.ImDecode(sampleImageData, ImreadModes.Color);
        // Уменьшаем размер картинки для экономии ресурсов.
        Cv2.Resize(src, src, new OpenCvSharp.Size(src.Width * 0.6, src.Height * 0.6));
        return _paddleOcrAll.Run(src).Text;
    }

    /// <summary>
    /// Gets image of selection area, trying recognize text from image and translates it to russian.
    /// </summary>
    private void RecognizeTextAndTranslate()
    {
        try
        {
            var line = PaddleOCRRecognize(GetImage());

            if (string.IsNullOrWhiteSpace(line))
                return;

            line = line.Trim()
                 // Для корректной обработки переводчиком нужно убрать переводы строк 
                 .Replace("\n", " ")
                 .Replace(",.", ",")
                 .Replace(".,", ".")
                 .Replace("?.", "?")
                 .Replace("!.", "!")
                 // Remove double dot.
                 .Replace("..", ".")
                 // Restore three dot.
                 .Replace("..", "...");

            var nakedLine = Regex.Replace(line.ToLower(), _regExPattern, string.Empty);

            // Сравнение строк с удаленными знаками препинания и пробелами.
            if (nakedLine == _lastLine)
                return;

            _lastLine = nakedLine;

            _parameters.TranslatedLine = _translationService.Translate(line, _settings.Translator);
        }
        catch (Exception ex)
        {
            _parameters.TranslatedLine = $"Error: {ex.Message}";
        }
    }
}