using UnityEngine;
using TMPro;

public class RankSlot : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nicknameText;
    public TMP_Text scoreText;

    public void SetSlot(string rank, string nickname, string score)
    {
        rankText.text = rank;
        nicknameText.text = nickname;
        scoreText.text = score;
    }
}