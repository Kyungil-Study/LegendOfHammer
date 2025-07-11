using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ItemSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    [FormerlySerializedAs("salePeriodText")] public TextMeshProUGUI MaxCountText;
    public Image productImage;
    public Button buyButton;
    public GameObject soldOutOverlay;

    private MarketData data;

    


    public void SetData(MarketData marketData)
    {
        data = marketData;

        nameText.text = data.Name;
        priceText.text = $"{data.Piece}원";
        MaxCountText.text = $"{data.Number}";

        var sprite = Resources.Load<Sprite>($"MarketIcons/{data.Product_Image_ID}");
        if (sprite != null) productImage.sprite = sprite;

        soldOutOverlay.SetActive(!data.Check_Product_Availability);
        buyButton.interactable = data.Check_Product_Availability;

        buyButton.onClick.AddListener(OnBuy);
    }

    private void OnBuy()
    {
        int point = 0;
        string currencyKey = "던전런 포인트";

        if (!BackendGameData.userData.inventory.TryGetValue(currencyKey, out point))
        {
            Debug.LogWarning($"{currencyKey} 정보가 존재하지 않습니다.");
            return;
        }

        if (point < data.Piece)
        {
            Debug.LogWarning($"[구매 실패] {currencyKey} 부족: {point} < {data.Piece}");
            return;
        }

        // 서버 저장
        BackendGameData.Instance.GameDataUpdate();

        Debug.Log($"[구매 성공] {data.Name} - {data.Piece} {currencyKey} 차감");
        
        if (data.Currency >= data.Number)
        {
            ShowPopup("구매 제한 횟수를 초과했습니다.");
            // 품절 처리 예시
            data.Check_Product_Availability = false;
            soldOutOverlay.SetActive(true);
            buyButton.interactable = false;
            return;
        }
        
        string itemName = data.Name;
        int itemCount = 1;

        if (BackendGameData.userData.inventory.ContainsKey(itemName))
        {
            BackendGameData.userData.inventory[itemName] += itemCount;
        }
        else
        {
            BackendGameData.userData.inventory.Add(itemName, itemCount);
        }
        
        // 재화 차감
        BackendGameData.userData.inventory[currencyKey] -= data.Piece;
        
        // 서버 저장
        BackendGameData.Instance.GameDataUpdate();

        data.Currency++;

        // UI 갱신
        var inventoryUI = FindObjectOfType<InventoryUIManager>();
        if (inventoryUI != null)
        {
            inventoryUI.RefreshInventory();
        }
        
        var marketUI = FindObjectOfType<MarketUIManager>();
        if (marketUI != null)
        {
            marketUI.UpdatePointUI();
        }
    }
    
    private void ShowPopup(string message)
    {
        var popup = FindObjectOfType<PopupManager>();
        if (popup != null)
        {
            popup.Show(message);
        }
    }


}