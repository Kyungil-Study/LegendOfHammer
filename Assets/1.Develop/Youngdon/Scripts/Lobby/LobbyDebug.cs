using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"[로비] UserID: {UserData.Instance.UserID}");
        Debug.Log($"[로비] Nickname: {UserData.Instance.Nickname}");
    }
}
