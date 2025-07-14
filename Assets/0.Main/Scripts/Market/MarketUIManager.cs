using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;

// public class MarketUIManager : MonoBehaviour
// {
//     public GameObject itemSlotPrefab;
//     public Transform contentParent;
//     
//     // InventoryUIManager나 GameManager에서
//     public TextMeshProUGUI pointText;
//     public void UpdatePointUI()
//     {
//         if (BackendGameData.userData.inventory.TryGetValue("던전런 포인트", out int point))
//         {
//             pointText.text = $"보유 포인트: {point}P";
//         }
//         else
//         {
//             pointText.text = "보유 포인트: 0P";
//         }
//     }
//
//     private async void Start()
//     {
//         List<MarketData> marketItems = await TSVLoader.LoadTableAsync<MarketData>("MarketTable");
//
//         if (marketItems == null || marketItems.Count == 0)
//         {
//             Debug.LogWarning("[MarketUIManager] 마켓 데이터를 불러오지 못했습니다.");
//             return;
//         }
//
//         foreach (var item in marketItems)
//         {
//             GameObject slot = Instantiate(itemSlotPrefab, contentParent);
//             var ui = slot.GetComponent<ItemSlotUI>();
//             ui.SetData(item);
//         }
//         
//         UpdatePointUI();
//     }
// }

public class MarketUIManager : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public Transform contentParent;
    public TextMeshProUGUI pointText;
    public string resourcePath;

    private GenericResourceManager<MarketData, DummyContainer> marketManager;

    private async void Start()
    {
        marketManager = new InlineMarketManager(resourcePath);
        var result = await marketManager.LoadAsync();

        if (result.Success == false || marketManager.Records == null || marketManager.Records.Count == 0)
        {
            Debug.LogWarning("[MarketUIManager] 마켓 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var item in marketManager.Records)
        {
            GameObject slot = Instantiate(itemSlotPrefab, contentParent);
            var ui = slot.GetComponent<ItemSlotUI>();
            ui.SetData(item);
        }

        UpdatePointUI();
    }

    public void UpdatePointUI()
    {
        if (BackendGameData.userData.inventory.TryGetValue("던전런 포인트", out int point))
        {
            pointText.text = $"보유 포인트: {point}P";
        }
        else
        {
            pointText.text = "보유 포인트: 0P";
        }
    }

    // 내부 전용 리소스 매니저 구현체
    private class InlineMarketManager : GenericResourceManager<MarketData, DummyContainer>
    {
        public InlineMarketManager(string path)
        {
            this.resourcePath = path;
        }
    }

    // 더미 타입 (싱글톤 미사용용도)
    private class DummyContainer : GenericResourceManager<MarketData, DummyContainer> { }
}
