using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class PostUIManager : MonoBehaviour
{
    public Transform contentParent;
    public GameObject postSlotPrefab;

    public Button rankTabBtn;
    public Button userTabBtn;
    public Button adminTabBtn;
    public Button couponTabBtn;
    public Button receiveAllBtn;

    private PostType currentPostType;

    private void Start()
    {
        rankTabBtn.onClick.AddListener(() => LoadPostTab(PostType.Rank));
        userTabBtn.onClick.AddListener(() => LoadPostTab(PostType.User));
        adminTabBtn.onClick.AddListener(() => LoadPostTab(PostType.Admin));
        couponTabBtn.onClick.AddListener(() => LoadPostTab(PostType.Coupon));

        receiveAllBtn.onClick.AddListener(() =>
        {
            BackendPost.Instance.PostReceiveAll(currentPostType);
            Invoke(nameof(ReloadCurrentTab), 0.1f); // 약간의 딜레이 후 UI 갱신
        });

        LoadPostTab(PostType.Rank); // 기본은 랭크 우편
    }

    private void LoadPostTab(PostType type)
    {
        currentPostType = type;
        ClearList();
        BackendPost.Instance.PostListGet(type);
        Invoke(nameof(ReloadCurrentTab), 0.1f); // 데이터 로딩 후 UI 갱신 (간단한 대기 방식)
    }

    private void ReloadCurrentTab()
    {
        ClearList();

        var posts = BackendPost.Instance.GetPostList(); // 아래 설명 참고

        for (int i = 0; i < posts.Count; i++)
        {
            GameObject go = Instantiate(postSlotPrefab, contentParent);
            go.GetComponent<PostSlot>().SetData(posts[i], i, currentPostType);
            go.GetComponent<PostSlot>().onReceived = ReloadCurrentTab;
        }
    }

    private void ClearList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}