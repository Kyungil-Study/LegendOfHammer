using UnityEngine;

/// [기능] 로그인한 유저의 스테이지 정보를 관리하는 클래스
public class StageData : MonoBehaviour, IStageData
{
    public static StageData Instance;

    // 실제 저장될 스테이지 값
    private int currentStage = 1;
    private int maxStage = 1;

    public int CurrentStage => currentStage;
    public int MaxStage => maxStage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// [기능] 스테이지 정보 설정
    public void SetStageData(int current, int max)
    {
        currentStage = current;
        maxStage = max;
    }

    /// [기능] 현재 스테이지 갱신
    public void UpdateCurrentStage(int stage)
    {
        currentStage = stage;
        if (stage > maxStage)
            maxStage = stage;
    }
    
}

