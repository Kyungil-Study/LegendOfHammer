using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public TextMeshProUGUI messageText;

    public void Show(string message)
    {
        popupPanel.SetActive(true);
        messageText.text = message;
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
    }
}