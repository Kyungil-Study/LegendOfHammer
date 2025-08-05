using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugumentDisplayPanel : MonoBehaviour
{
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    
    private AugmentDisplayData displayData;
    private Button exitButton; 
    
    public void SetAugmentDisplayData(AugmentDisplayData data)
    {
        displayData = data;
    }

    private void Awake()
    {
        exitButton = GetComponentInChildren<Button>();
        exitButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        if (displayData.classData != null)
        {
            displayImage.sprite = displayData.classData.GetIcon();
            descriptionText.text = displayData.classData.GetDescription();
        }
        
        if (displayData.commonData != null)
        {
            displayImage.sprite = displayData.commonData.GetIcon();
            descriptionText.text = displayData.commonData.GetDescription();
        }
    }
    
    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}