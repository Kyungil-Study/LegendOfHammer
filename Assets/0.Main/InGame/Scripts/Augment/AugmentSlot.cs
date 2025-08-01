using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AugmentSlot : MonoBehaviour 
{
    [SerializeField] TMP_Text augmentNameText;
    [SerializeField] Image augmentIcon; // todo : icon 연동되면 사용
    [SerializeField] TMP_Text augmentGradeText;
    [SerializeField] TMP_Text augmentDescriptionText;
    [SerializeField] Button augmentButton;
    Material augmentMaterial;

    public Augment augment;
    
    Action<Augment> onClickAction;
    
    private void Awake()
    {
        if (augmentNameText == null || augmentIcon == null || augmentGradeText == null || augmentDescriptionText == null || augmentButton == null)
        {
            Debug.LogError("AugmentSlot: Missing UI components. Please assign all required components in the inspector.");
        }
        
        // Initialize the button click action
        augmentButton.onClick.AddListener(() => onClickAction?.Invoke(augment));
        augmentMaterial = augmentButton.GetComponent<Image>().material;

    }

    private void OnEnable()
    {
    }
    
    private void OnDisable()
    {
        // Clear the augment data when the slot is disabled
        augment = null;
        augmentNameText.text = string.Empty;
        augmentGradeText.text = string.Empty;
        augmentDescriptionText.text = string.Empty;
        augmentIcon.sprite = null; // Assuming GetIcon() returns a Sprite
    }

    public void SetAugment(Augment augment, Action<Augment> onClickAction)
    {
        this.augment = augment;
        augmentNameText.text = augment.GetName();
        augmentGradeText.text = augment.GetGrade(); // Assuming ID is used as level for simplicity
        augmentDescriptionText.text = augment.GetDescription(); // Assuming GetAugmentType() returns a string representation
        augmentIcon.sprite = augment.GetIcon(); // Assuming GetIcon() returns a Sprite
        
        // Set the augment icon if available
        this.onClickAction = onClickAction; 
    }

}
