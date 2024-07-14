using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScreTran;

public class TranslationService : ITranslationService
{
    public TranslationService()
    {
    }

    /// <summary>
    /// Translate input from english to russian.
    /// </summary>
    /// <param name="input">input text.</param>
    /// <returns>Translated text.</returns>
    public string Translate(string input)
    {
        var from = "en";
        var to = "ru";
        var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={HttpUtility.UrlEncode(input)}";

        using var client = new HttpClient();
        var response = Task.Run(async () => await client.GetStringAsync(url).ConfigureAwait(false)).Result;
        var translatedArray = JsonConvert.DeserializeObject<JArray>(response)!.First().Value<JArray>();
        return string.Join("", translatedArray!.Select(t => t.First().Value<string>()));
    }
}
