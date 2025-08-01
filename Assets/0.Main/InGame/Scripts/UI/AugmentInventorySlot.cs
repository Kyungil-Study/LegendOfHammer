using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmentInventorySlot : MonoBehaviour
{
    [SerializeField] private Image augmentIcon;
    [SerializeField] private TMP_Text augmentText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(CommonAugmentUserData userData)
    {
        if (userData == null)
        {
            Debug.LogError("CommonAugmentUserData is null");
            return;
        }

        var data =userData.GetData();
        augmentIcon.sprite = data.GetIcon();
        augmentText.text = $"{data.GetGrade()}:{userData.Count}";
    }
    
    public void Initialize(ClassAugmentUserData userData)
    {
        if (userData == null)
        {
            Debug.LogError("WarriorAugmentUserData is null");
            return;
        }

        var data = userData.GetData();
        augmentIcon.sprite = data.GetIcon();
        augmentText.text = $"Level.{userData.Level}";
    }
    
}
