
using UnityEngine;

[System.Serializable]
public class ES3UserStageData
{
    public int CurrentStage = 1;
    public int MaxStage = 1;
    public int StageAttemptCount = 0;
}

[System.Serializable]
public class ES3UserAugmentData
{
    public string WarriorAugment;
    public string WizardAugment;
    public string ArcherAugment;
    public string CommonAugment;
}

public class ES3Manager : SingletonBase<ES3Manager>
{
    private ES3Settings UserDataSettings { get; set; }
    private ES3File UserDataES3File { get; set; }
    
    public ES3UserStageData StageData = new ES3UserStageData();
    public ES3UserAugmentData AugmentData = new ES3UserAugmentData();
    
    
    private ES3File UserDataES3FileLoaded { get; set; }
    public bool bSetEasySaveUser { get; set; } = false;

    public void SaveBuffered<T>(string key, T value)
    {
        UserDataES3File.Save<T>(key, value);
    }

    public void CommitBuffered()
    {
        UserDataES3File.Sync();
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        UserDataSettings = new ES3Settings("UserData.es3");
        UserDataES3File = new ES3File(UserDataSettings);

        if (TryLoad("stage", out StageData) == false)
        {
            StageData = new ES3UserStageData(); // StageData가 없으면 새로 생성
        }
        
        if (TryLoad("augment", out AugmentData) == false)
        {
            AugmentData = new ES3UserAugmentData(); // AugmentData가 없으면 새로 생성
        }
    }

   
    public bool TryLoad<T>(string key, out T tValue)
    {
        UserDataES3FileLoaded = new ES3File(UserDataSettings);
        
        if (UserDataES3FileLoaded.KeyExists(key))
        {
            tValue = UserDataES3FileLoaded.Load<T>(key);
            return true; 
        }
        
        tValue = default; 
        return false;
    }

    public void Delete(string key)
    {
        if (ES3.KeyExists(key, UserDataSettings.path))
            ES3.DeleteKey(key, UserDataSettings);
    }

    public void DeleteAll()
    {
        ES3.DeleteFile(UserDataSettings.path);
    }

    public void SetStage(int stage)
    {
        StageData.CurrentStage = stage;
        StageData.MaxStage = Mathf.Max(StageData.MaxStage, StageData.CurrentStage);
        ES3Manager.Instance.SaveBuffered("stage", StageData);
        CommitBuffered();
    }
    
    public void SaveAumgents()
    {
        ES3Manager.Instance.SaveBuffered("augment", AugmentData);
        CommitBuffered();
    }

    public void NextStage()
    {
        StageData.CurrentStage += 1;
        StageData.MaxStage = Mathf.Max(StageData.MaxStage, StageData.CurrentStage);

        ES3Manager.Instance.SaveBuffered("stage", StageData);
        CommitBuffered();
    }
    
    public void ResetCurrentStage()
    {
        StageData.CurrentStage = 1;
        ES3Manager.Instance.SaveBuffered("stage", StageData);
        CommitBuffered();
    }
}