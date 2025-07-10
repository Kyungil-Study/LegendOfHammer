using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using BackEnd;

public class RankUIManager : MonoBehaviour
{
    public Transform contentParent; // Content에 할당
    public GameObject rankSlotPrefab; // RankSlot 프리팹

    // private void OnEnable()
    // {
    //     StartCoroutine(LoadRankList());
    // }
    
    private void Start() //테스트용
    {
        StartCoroutine(LoadRankList());
    }

    IEnumerator LoadRankList()
    {
        var bro = Backend.URank.User.GetRankList("0197f331-b9e4-7254-93f5-b70549c6ed31");

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("랭킹 불러오기 실패: " + bro);
            yield break;
        }

        // 기존 슬롯 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (JsonData data in bro.FlattenRows())
        {
            GameObject slot = Instantiate(rankSlotPrefab, contentParent);
            RankSlot rankSlot = slot.GetComponent<RankSlot>();

            string rank = data["rank"].ToString();
            string nickname = data["nickname"].ToString();
            string score = data["score"].ToString();

            rankSlot.SetSlot(rank, nickname, score);
        }

        Debug.Log("랭킹 UI 업데이트 완료");
    }
}