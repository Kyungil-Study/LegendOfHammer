using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI salePeriodText;
    public Image productImage;
    public Button buyButton;
    public GameObject soldOutOverlay;

    private MarketData data;

    public void SetData(MarketData marketData)
    {
        data = marketData;

        nameText.text = data.Name;
        priceText.text = $"{data.Piece}원";
        salePeriodText.text = data.Sale_Period;

        var sprite = Resources.Load<Sprite>($"MarketIcons/{data.Product_Image_ID}");
        if (sprite != null) productImage.sprite = sprite;

        soldOutOverlay.SetActive(!data.Check_Product_Availability);
        buyButton.interactable = data.Check_Product_Availability;

        buyButton.onClick.AddListener(OnBuy);
    }

    private void OnBuy()
    {
        Debug.Log($"[구매] {data.Name} - {data.Piece}원");
        // 구매 처리 로직은 여기 추가
    }
}