using System.Net.Http;
using System.Web;
using GTranslate.Translators;
using Newtonsoft.Json.Linq;

namespace ScreTran;

public class TranslationService : ITranslationService
{
    private readonly YandexTranslator _yandexTranslator;
    private readonly BingTranslator _bingTranslator;

    public TranslationService()
    {
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
            return await TranslateGoogleAsync(input); 
            // Не использовать!!! Ошибка TOO MANY REQUESTS
            //return (await _googleTranslator.TranslateAsync(input, "ru")).Translation;
        if (translator == Enumerations.Translator.Yandex)
            return (await _yandexTranslator.TranslateAsync(input, "ru")).Translation;
        if (translator == Enumerations.Translator.Bing)
            return (await _bingTranslator.TranslateAsync(input, "ru")).Translation;

        return input;
    }

    /// <summary>
    /// Translate input from english to russian GoogleTranslate asynchroniously.
    /// </summary>
    /// <param name="input">input text.</param>
    /// <returns>Translated text.</returns>
    public async Task<string> TranslateGoogleAsync(string input)
    {
        var to = "ru";
        var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={to}&dt=t&q={HttpUtility.UrlEncode(input)}";

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url).ConfigureAwait(false);
        return string.Join(string.Empty, JArray.Parse(response)[0].Select(x => x[0]));
    }
}


