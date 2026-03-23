namespace TaskProcessing.Worker.Interfaces;

public interface ITaskProcessor
{
    Task ProcessNextQueuedTaskAsync(CancellationToken cancellationToken);
}