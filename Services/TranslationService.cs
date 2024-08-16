using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Web;
using GTranslate.Translators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScreTran;

public class TranslationService : ITranslationService
{
    private readonly GoogleTranslator _googleTranslator;
    private readonly YandexTranslator _yandexTranslator;
    private readonly BingTranslator _bingTranslator;

    public TranslationService()
    {
        _googleTranslator = new GoogleTranslator();
        _yandexTranslator = new YandexTranslator();
        _bingTranslator = new BingTranslator();
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


