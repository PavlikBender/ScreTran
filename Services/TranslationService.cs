using GTranslate.Translators;

namespace ScreTran;

public class TranslationService : ITranslationService
{
    private readonly YandexTranslator _yandexTranslator;
    private readonly BingTranslator _bingTranslator;
    private readonly GoogleTranslator _googleTranslator;

    public TranslationService()
    {
        _yandexTranslator = new YandexTranslator();
        _bingTranslator = new BingTranslator();
        _googleTranslator = new GoogleTranslator();
    }

    /// <summary>
    /// Translate input from english to russian.
    /// </summary>
    /// <param name="input">Input text.</param>
    /// <param name="translator">Translator type.</param>
    /// <returns>Translated text.</returns>
    public string Translate(string input, Enumerations.Translator translator)
    {
        return Task.Run(async () => await TranslateAsync(input, translator)).Result;
    }

    /// <summary>
    /// Translate input from english to russian asynchroniously.
    /// </summary>
    /// <param name="input">Input text.</param>
    /// <param name="translator">Translator type.</param>
    /// <returns>Translated text.</returns>
    private async Task<string> TranslateAsync(string input, Enumerations.Translator translator)
    {
        if (translator == Enumerations.Translator.Google)
            return (await _googleTranslator.TranslateAsync(input, "ru")).Translation;
        if (translator == Enumerations.Translator.Yandex)
            return (await _yandexTranslator.TranslateAsync(input, "ru")).Translation;
        if (translator == Enumerations.Translator.Bing)
            return (await _bingTranslator.TranslateAsync(input, "ru")).Translation;

        return input;
    }
}


