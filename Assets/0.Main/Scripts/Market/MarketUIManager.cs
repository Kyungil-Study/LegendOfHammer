using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MarketUIManager : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public Transform contentParent;

    private async void Start()
    {
        List<MarketData> marketItems = await TSVLoader.LoadTableAsync<MarketData>("MarketTable");

        if (marketItems == null || marketItems.Count == 0)
        {
            Debug.LogWarning("[MarketUIManager] 마켓 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var item in marketItems)
        {
            GameObject slot = Instantiate(itemSlotPrefab, contentParent);
            var ui = slot.GetComponent<ItemSlotUI>();
            ui.SetData(item);
        }
    }
}