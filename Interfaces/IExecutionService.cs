namespace ScreTran;

public interface IExecutionService
{
    /// <summary>
    /// Starts service.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops service.
    /// </summary>
    void Stop();

    /// <summary>
    /// Return true if service started, otherwise false.
    /// </summary>
    bool IsStarted
    {
        get;
    }
}
