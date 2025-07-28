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

    public void Initialize(Augment augment)
    {
        augmentIcon.sprite = augment.GetIcon();
    }
}
