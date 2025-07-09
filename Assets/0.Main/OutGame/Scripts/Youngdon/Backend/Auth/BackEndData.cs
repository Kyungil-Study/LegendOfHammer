using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BackEnd;
using Unity.VisualScripting;

public class Stage 
{
    public int Currentstage = 1;
    public int Maxstage = 1;

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"currentstage : {Currentstage}");
        result.AppendLine($"maxstage : {Maxstage}");
        return result.ToString();
    }
}

public class BackendGameData
{
    private static BackendGameData _instance = null;

    public static BackendGameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendGameData();
            }
            return _instance;
        }
    }

    public static Stage stage;

    public void InitalizeStage()
    {
        if (stage == null)
        {
            stage = new Stage();
        }
        Debug.Log("스테이지 초기화");
        stage.Currentstage = 1;
        stage.Maxstage = 1;
        
        Param param = new Param();
        param.Add("currentStage", stage.Currentstage);
        param.Add("maxStage", stage.Maxstage);
        
        Debug.Log("STAGE_DATA 테이블에 새로운 데이터 행 추가");
        
        var bro = Backend.GameData.Insert("STAGE_DATA", param);

        if(bro.IsSuccess()) {
            Debug.Log("데이터를 추가하는데 성공했습니다. : " + bro);
        } else {
            Debug.LogError("데이터를 추가하는데 실패했습니다. : " + bro);
        }
    }

    public void GetStage()
    {
        Debug.Log("'STAGE_DATA' 테이블의 데이터를 조회하는 함수를 호출합니다.");
        var bro = Backend.GameData.GetMyData("STAGE_DATA",new Where());
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
                stage = new Stage();
                stage.Currentstage = int.Parse(gameDataJson[0]["currentStage"].ToString());
                stage.Maxstage = int.Parse(gameDataJson[0]["maxStage"].ToString());
            }
        }
    }

    public void ClearStage()
    {
        stage.Currentstage += 1;
    }

    public void UpdateStage()
    {
        if(stage == null) {
            Debug.LogError("데이터가 존재하지 않습니다. Initialize 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }
        Param param = new Param();
        param.Add("currentStage", stage.Currentstage);
        param.Add("maxStage", stage.Maxstage);

        BackendReturnObject bro = null;
        bro = Backend.GameData.Update("STAGE_DATA", new Where(), param);
        if(bro.IsSuccess()) 
        {
            Debug.Log("데이터 수정에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("데이터 수정에 실패했습니다. : " + bro);
        }
    }
}