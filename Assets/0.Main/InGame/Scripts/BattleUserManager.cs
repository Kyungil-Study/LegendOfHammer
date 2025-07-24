
public class UserStageData
{
    public int CurrentStage { get; set; }
    public int MaxStage { get; set; }
    public int StageAttemptCount { get; set; }
}

public class UserAugmentData
{
    public string WarriorAugment { get; set; }
    public string WizardAugment { get; set; }
    public string ArcherAugment { get; set; }
    public string CommonAugment { get; set; }
}

public class BattleUserManager : SingletonBase<BattleUserManager>
{
    public UserStageData StageData { get; private set; } = new UserStageData();
    public UserAugmentData AugmentData { get; private set; } = new UserAugmentData();

    public override void OnInitialize()
    {
        base.OnInitialize();
        // 초기화 로직을 여기에 추가할 수 있습니다.
    }
}