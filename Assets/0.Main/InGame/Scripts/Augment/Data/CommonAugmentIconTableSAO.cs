using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommonAugmentIconData
{
    [SerializeField]
    public int OptionID; // Augment Option ID
    [SerializeField]
    public Sprite Icon;
}

[CreateAssetMenu(fileName = "CommonAugmentIconTableSAO", menuName = "Augment/CommonAugmentIconTableSAO", order = 1)]
public class CommonAugmentIconTableSAO : ScriptableObject
{
    [SerializeField] private List<CommonAugmentIconData> augmentIcons = new List<CommonAugmentIconData>();

    public Sprite GetIconByOptionID(int optionID)
    {
        foreach (var data in augmentIcons)
        {
            if (data.OptionID == optionID)
            {
                return data.Icon;
            }
        }
        return null; // 아이콘을 찾지 못한 경우 null 반환
    }

}
