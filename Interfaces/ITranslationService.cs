namespace ScreTran;

public interface ITranslationService
{
    /// <summary>
    /// Translate input from english to russian.
    /// </summary>
    /// <param name="input">input text.</param>
    /// <returns>Translated text.</returns>
    string Translate(string input);
}
