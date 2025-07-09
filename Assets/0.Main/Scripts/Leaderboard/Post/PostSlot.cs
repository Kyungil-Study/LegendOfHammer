using System.Text;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostSlot : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text contentText;
    public TMP_Text rewardText;
    public Button receiveButton;

    private int postIndex;
    private PostType postType;
    public System.Action onReceived;
    

    public void SetData(Post post, int index, PostType type)
    {
        titleText.text = post.title;
        contentText.text = post.content;

        StringBuilder sb = new StringBuilder();
        foreach (var item in post.postReward)
        {
            sb.AppendLine($"{item.Key} x {item.Value}");
        }

        rewardText.text = sb.ToString();
        postIndex = index;
        postType = type;

        receiveButton.onClick.RemoveAllListeners();
        receiveButton.onClick.AddListener(() =>
        {
            BackendPost.Instance.PostReceive(postType, postIndex);
            onReceived?.Invoke();
        });
    }
}