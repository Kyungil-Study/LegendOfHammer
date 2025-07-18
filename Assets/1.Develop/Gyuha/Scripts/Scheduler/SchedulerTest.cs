using System.Threading.Tasks;
using UnityEngine;

public class SchedulerTest : MonoBehaviour
{
    private AsyncTaskScheduler scheduler;
    private ControllableTimeProvider timeProvider;
    
    private void Start()
    {
        scheduler = new AsyncTaskScheduler(timeProvider);

        // 3초 후에 완료되는 Task 등록
        var timedTask = scheduler.Schedule(3f);

        Debug.Log("3초 후 완료 예정");

        _ = WaitForTask(timedTask);
    }

    private async Task WaitForTask(TimedTask task)
    {
        while (task.IsCompleted == false)
        {
            float remaining = task.GetRemainingTime(Time.time);
            float percent = task.GetProgress(Time.time) * 100f;
            Debug.Log($"남은 시간: {remaining:F2}s / 진행도: {percent:F0}%");
            await Task.Delay(500);
        }

        await task.Task;
        Debug.Log("작업 완료");
    }

    private void Update()
    {
        if(timeProvider.IsPaused == false) timeProvider.Update(Time.deltaTime);
        scheduler.Tick();
    }
}