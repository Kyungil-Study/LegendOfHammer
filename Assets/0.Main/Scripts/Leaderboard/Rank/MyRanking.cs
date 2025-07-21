// using System;
// using System.Collections.Generic;
// using BackEnd;
// using LitJson;
// using UnityEngine;
// using TMPro;
// using System.Linq;
// using UnityEngine.Serialization;
//
// public class MyRanking : MonoBehaviour
// {
//     [FormerlySerializedAs("RankText")] public TMP_Text rankText;
//     public TMP_Text[] nameText;
//     public class RankItem
//     {
//         public string gamerInDate;
//         public string nickname;
//         public string score;
//         public string index;
//         public string rank;
//         public string extraData = string.Empty;
//         public string extraName = string.Empty;
//         public string totalCount;
//
//         public override string ToString()
//         {
//             string str = $"유저인데이트:{gamerInDate}\n닉네임:{nickname}\n점수:{score}\n정렬:{index}\n순위:{rank}\n총합:{totalCount}\n";
//             if(extraName != string.Empty)
//             {
//                 str += $"{extraName}:{extraData}\n";
//             }
//             return str;
//         }
//     }
//
//     private void Start()
//     {
//         GetMyRankTest();
//     }
//
//     public void GetMyRankTest()
//     {
//         string userUuid = "0197f332-475d-76e0-ba8d-7855d3691f71";
//
//         List<RankItem> rankItemList = new List<RankItem>();
//
//         BackendReturnObject bro = Backend.URank.User.GetMyRank(userUuid);
//
//         if(bro.IsSuccess())
//         {
//             LitJson.JsonData rankListJson = bro.GetFlattenJSON();
//             string extraName = string.Empty;
//             for(int i = 0; i < rankListJson["rows"].Count; i++)
//             {
//                 RankItem rankItem = new RankItem();
//
//                 rankItem.gamerInDate = rankListJson["rows"][i]["gamerInDate"].ToString();
//                 rankItem.nickname = rankListJson["rows"][i]["nickname"].ToString();
//                 rankItem.score = rankListJson["rows"][i]["score"].ToString();
//                 rankItem.index = rankListJson["rows"][i]["index"].ToString();
//                 rankItem.rank = rankListJson["rows"][i]["rank"].ToString();
//                 rankItem.totalCount = rankListJson["totalCount"].ToString();
//                 rankText.text = rankItem.rank;
//
//                 if(rankListJson["rows"][i].ContainsKey(rankItem.extraName))
//                 {
//                     rankItem.extraData = rankListJson["rows"][i][rankItem.extraName].ToString();
//                 }
//
//                 rankItemList.Add(rankItem);
//                 Debug.Log(rankItem.ToString());
//             }
//         }
//     }
// }

using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.Serialization;

public class MyRanking : MonoBehaviour
{
    [FormerlySerializedAs("RankText")] public TMP_Text rankText;
    public TMP_Text[] nameText;    // 길이가 최소 3 이상이어야 합니다

    private string rankUUID = "0197f332-475d-76e0-ba8d-7855d3691f71";

    private void Start()
    {
        GetMyRankTest();
        GetTop3Names();
    }

    public void GetMyRankTest()
    {
        var bro = Backend.URank.User.GetMyRank(rankUUID);
        if (!bro.IsSuccess()) return;

        JsonData json = bro.GetFlattenJSON();
        // bro.GetFlattenJSON() 안에는 rows 배열과 totalCount 만 있습니다.
        var rows = json["rows"];
        if (rows.Count > 0)
        {
            // 내 랭크는 rows[0]
            rankText.text = rows[0]["rank"].ToString();
        }
    }

    public void GetTop3Names()
    {
        // offset=0, limit=2 → rows.Count 최대 3
        var bro = Backend.URank.User.GetRankList(rankUUID, 3, 0);
        if (!bro.IsSuccess())
        {
            Debug.LogError("Top3 불러오기 실패: " + bro.GetMessage());
            return;
        }

        JsonData rows = bro.GetFlattenJSON()["rows"];
        int count = Mathf.Min(rows.Count, nameText.Length);
        for (int i = 0; i < count; i++)
        {
            // nickname은 바로 ToString() 해도 S/N 구분 필요 없이 문자열로 나옵니다
            nameText[i].text = rows[i]["nickname"].ToString();
        }
        // 만약 3 미만인 경우 비워두고 싶으면:
        // for (int i = count; i < nameText.Length && i < 3; i++)
        // {
        //     nameText[i].text = "-";
        // }
    }
}
