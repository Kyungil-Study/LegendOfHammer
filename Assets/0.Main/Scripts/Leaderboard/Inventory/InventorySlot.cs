using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemCountText;
    
    // // InventoryUIManager나 GameManager에서
    // public TextMeshProUGUI pointText;
    //
    // void UpdatePointUI()
    // {
    //     if (BackendGameData.userData.inventory.TryGetValue("던전런포인트", out int point))
    //     {
    //         pointText.text = $"보유 포인트: {point}P";
    //     }
    //     else
    //     {
    //         pointText.text = "보유 포인트: 0P";
    //     }
    // }

    public void SetData(string itemName, int itemCount)
    {
        itemNameText.text = itemName;
        itemCountText.text = $"{itemCount}개";
    }
}