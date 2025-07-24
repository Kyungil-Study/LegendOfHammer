
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

public class BattleUserManager : MonoSingleton<BattleUserManager>
{
    public UserStageData StageData { get; private set; } = new UserStageData();
    public UserAugmentData AugmentData { get; private set; } = new UserAugmentData();

}