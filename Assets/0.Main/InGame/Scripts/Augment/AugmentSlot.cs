using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class AugmentSlot : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] TMP_Text augmentNameText;
    [SerializeField] TMP_Text augmentGradeText;
    [SerializeField] TMP_Text augmentDescriptionText;

    public Augment augment;
    
    public void SetAugment(Augment augment)
    {
        this.augment = augment;
        augmentNameText.text = augment.GetName();
        augmentGradeText.text = augment.GetGrade(); // Assuming ID is used as level for simplicity
        augmentDescriptionText.text = augment.GetDescription(); // Assuming GetAugmentType() returns a string representation
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BattleEventManager.Instance.CallEvent(new SelectAugmentEventArgs(augment));
    }
}
