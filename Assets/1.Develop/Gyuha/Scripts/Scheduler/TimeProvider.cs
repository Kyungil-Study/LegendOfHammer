using UnityEngine;

public interface ITimeProvider
{
    float Time { get; }
    void Update(float deltaTime);
}

public class ControllableTimeProvider : ITimeProvider
{
    public float Time => time;
    public float TimeScale { get; private set; } = 1f;
    public bool IsPaused => TimeScale == 0f;

    private float time;

    public void Update(float deltaTime)
    {
        time += deltaTime * TimeScale;
    }

    public void SetTimeScale(float scale)
    {
        TimeScale = Mathf.Clamp(scale, 0f, 10f);
    }

    public void Pause() => TimeScale = 0f;
    public void Resume() => TimeScale = 1f;
}