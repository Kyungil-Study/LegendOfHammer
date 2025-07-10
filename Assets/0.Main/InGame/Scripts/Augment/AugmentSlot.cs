using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AugmentSlot : MonoBehaviour , I
{
    [SerializeField] TMP_Text augmentNameText;
    [SerializeField] TMP_Text augmentLevelText;
    [SerializeField] TMP_Text augmentDescriptionText;

    public Augment augment;
    
    public void SetAugment(Augment augment)
    {
        this.augment = augment;
        augmentNameText.text = augment.GetName();
        augmentLevelText.text = $"Lv. {augment.GetID()}"; // Assuming ID is used as level for simplicity
    }
}
