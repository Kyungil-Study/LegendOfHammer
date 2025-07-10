using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BackEnd;
using Unity.VisualScripting;

public class Augment
{
    public string CommonAugmentData;
    public string WarriorAugmentData;
    public string WizardAugmentData;
    public string ArcherAugmentData;
    
    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"commonAugmentData : {CommonAugmentData}");
        result.AppendLine($"warriorAugmentData : {WarriorAugmentData}");
        result.AppendLine($"wizardAugmentData : {WizardAugmentData}");
        result.AppendLine($"archerAugmentData : {ArcherAugmentData}");
        return result.ToString();
    }
}
public class BackendAugmentData
{
    private static BackendAugmentData _instance = null;

    public static BackendAugmentData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendAugmentData();
            }
            return _instance;
        }
    }

    public static Augment augment;

    // 증강 데이터 초기화
    public void InitalizeAugmentData()
    {
        if (augment == null)
        {
            augment = new Augment();
        }
        Debug.Log("증강 초기화");
        augment.CommonAugmentData = "";
        augment.WarriorAugmentData = "";
        augment.WizardAugmentData = "";
        augment.ArcherAugmentData = "";
        
        Param param = new Param();
        param.Add("commonAugmentData", augment.CommonAugmentData);
        param.Add("warriorAugmentData", augment.WarriorAugmentData);
        param.Add("wizardAugmentData", augment.WizardAugmentData);
        param.Add("archerAugmentData", augment.ArcherAugmentData);

        Debug.Log("AUGMENT_DATA 테이블에 새로운 데이터 행 추가");

        var bro = Backend.GameData.Insert("AUGMENT_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log("증강 데이터를 추가하는데 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("증강 데이터 추가 실패 : " + bro);
        }
    }

    // 증강 데이터 조회
    public void GetAugmentData()
    {
        Debug.Log("증강 테이블 데이터 조회 함수 호출");
        var bro = Backend.GameData.GetMyData("AUGMENT_DATA", new Where());
        if (bro.IsSuccess())
        {
            Debug.Log("데이터 조회에 성공했습니다. : " + bro);
            LitJson.JsonData gameDataJson = bro.FlattenRows(); 
            if(gameDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
            }
            else
            {
                augment = new Augment();
                augment.CommonAugmentData  = gameDataJson[0]["commonAugmentData"].ToString();
                augment.WarriorAugmentData = gameDataJson[0]["warriorAugmentData"].ToString();
                augment.WizardAugmentData  = gameDataJson[0]["wizardAugmentData"].ToString();
                augment.ArcherAugmentData  = gameDataJson[0]["archerAugmentData"].ToString();
            }
        }
    }
    
    // 증강 초기화 => 스테이지 클리어 실패했을때 호출하세요
    public void ResetAugmentData()
    {
        if (augment == null)
            augment = new Augment();
        
        augment.CommonAugmentData  = "";
        augment.WarriorAugmentData = "";
        augment.WizardAugmentData  = "";
        augment.ArcherAugmentData  = "";

    }

    // 증강 정보를 로컬에서 뒤끝으로 쏴주는 메서드
    public void UpdateAugmentData()
    {
        if (augment == null)
        {
            return;
        }
        Param param = new Param();
        param.Add("commonAugmentData", augment.CommonAugmentData);
        param.Add("warriorAugmentData", augment.WarriorAugmentData);
        param.Add("wizardAugmentData", augment.WizardAugmentData);
        param.Add("archerAugmentData", augment.ArcherAugmentData);

        BackendReturnObject bro = null;
        
        bro = Backend.GameData.Update("AUGMENT_DATA",new Where(), param);

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 수정에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("데이터 수정에 실패했습니다. : " + bro);
        }
    }
}