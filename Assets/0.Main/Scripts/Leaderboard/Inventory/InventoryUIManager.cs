using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [Header("슬롯 프리팹")]
    [SerializeField] private GameObject slotPrefab;

    [Header("부모 컨테이너 (LayoutGroup이 붙어야 함)")]
    [SerializeField] private Transform slotParent;

    private List<GameObject> slotInstances = new List<GameObject>();

    // private void OnEnable()
    // {
    //     RefreshInventory();
    // }
    
    private void Start()
    {
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        // 기존 슬롯 제거
        foreach (var slot in slotInstances)
        {
            Destroy(slot);
        }
        slotInstances.Clear();

        // 유저 인벤토리 가져오기
        Dictionary<string, int> inventory = BackendGameData.userData.inventory;

        foreach (var kvp in inventory)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            InventorySlot slotUI = slot.GetComponent<InventorySlot>();

            if (slotUI != null)
            {
                slotUI.SetData(kvp.Key, kvp.Value);
            }

            slotInstances.Add(slot);
        }
    }
}