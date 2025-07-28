using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClassAugmentIConData
{
    [SerializeField]
    public int AugmentID; 
    [SerializeField]
    public Sprite Icon;
}

[CreateAssetMenu(fileName = "ClassAugmentIconTableSAO", menuName = "Augment/ClassAugmentIconTableSAO", order = 1)]
public class ClassAugmentIconTableSAO : ScriptableObject
{
    [SerializeField] private List<ClassAugmentIConData> augmentIcons = new List<ClassAugmentIConData>();

    public Sprite GetIconByAugmentID(int augmentID)
    {
        foreach (var data in augmentIcons)
        {
            if (data.AugmentID == augmentID)
            {
                return data.Icon;
            }
        }
        return null; // 아이콘을 찾지 못한 경우 null 반환
    }
}
