using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AugmentDisplayData
{
    public CommonAugment commonData;
    public ClassAugment classData;
}

public class AugmentInventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image augmentIcon;
    [SerializeField] private TMP_Text augmentText;
    
    private AugumentDisplayPanel augmentDisplayPanel;
    private AugmentDisplayData augmentDisplayData = new();
    
    public void Initialize(CommonAugmentUserData userData, AugumentDisplayPanel panel)
    {
        if (userData == null)
        {
            Debug.LogError("CommonAugmentUserData is null");
            return;
        }
        
        CommonAugment data = userData.GetData();
        augmentDisplayData.commonData = data;
        
        augmentIcon.sprite = data.GetIcon();
        augmentText.text = $"{data.GetGrade()}:{userData.Count}";
        
        augmentDisplayPanel = panel;
    }
    
    public void Initialize(ClassAugmentUserData userData, AugumentDisplayPanel panel)
    {
        if (userData == null)
        {
            Debug.LogError("WarriorAugmentUserData is null");
            return;
        }

        ClassAugment data = userData.GetData();
        augmentDisplayData.classData = data;
        
        augmentIcon.sprite = data.GetIcon();
        augmentText.text = $"Level.{userData.Level}";

        augmentDisplayPanel = panel;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        augmentDisplayPanel.SetAugmentDisplayData(augmentDisplayData);
        augmentDisplayPanel.gameObject.SetActive(true);
    }
}
