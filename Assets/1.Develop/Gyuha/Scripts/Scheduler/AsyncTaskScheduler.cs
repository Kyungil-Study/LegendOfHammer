using System;
using System.Collections.Generic;

public class AsyncTaskScheduler
{
    private readonly ITimeProvider timeProvider;
    private readonly List<TimedTask> tasks = new();

    public AsyncTaskScheduler(ITimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }

    public TimedTask Schedule(float delay)
    {
        var task = new TimedTask(timeProvider.Time, delay);
        tasks.Add(task);
        return task;
    }

    public void Tick()
    {
        float now = timeProvider.Time;
        for (int i = tasks.Count - 1; i >= 0; i--)
        {
            var task = tasks[i];
            task.TryComplete(now);
            if (task.IsCompleted)
                tasks.RemoveAt(i);
        }
    }
}