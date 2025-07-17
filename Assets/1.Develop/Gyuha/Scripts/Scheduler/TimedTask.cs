using System;
using UnityEngine;
using System.Threading.Tasks;

public class TimedTask
{
    public float StartTime { get; }
    public float Duration { get; }
    public float EndTime => StartTime + Duration;

    public Task Task => tcs.Task;
    public bool IsCompleted => tcs.Task.IsCompleted;

    private readonly TaskCompletionSource<bool> tcs;

    public TimedTask(float startTime, float duration)
    {
        StartTime = startTime;
        Duration = duration;
        tcs = new TaskCompletionSource<bool>();
    }

    public void TryComplete(float currentTime)
    {
        if (IsCompleted == false && currentTime >= EndTime)
        {
            tcs.TrySetResult(true);
        }
    }

    public float GetRemainingTime(float currentTime)
    {
        return Mathf.Max(0f, EndTime - currentTime);
    }

    public float GetProgress(float currentTime)
    {
        if (Duration <= 0f) return 1f;
        return Mathf.Clamp01((currentTime - StartTime) / Duration);
    }
}