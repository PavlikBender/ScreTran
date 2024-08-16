namespace ScreTran;

public interface ITranslationService
{
    /// <summary>
    /// Translate input from english to russian.
    /// </summary>
    /// <param name="input">Input text.</param>
    /// <param name="translator">Translator type.</param>
    /// <returns>Translated text.</returns>
    string Translate(string input, Enumerations.Translator translator);
}
