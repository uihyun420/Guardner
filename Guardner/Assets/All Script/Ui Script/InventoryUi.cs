using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUi : GenericWindow
{
    private Dictionary<string, int> itemCounts = new();

    // 아이템 UI 오브젝트 참조 (프리팹에서 할당)
    [SerializeField] private InventoryItemUi enhanceTicketUi;
    [SerializeField] private InventoryItemUi lotteryTicketUi;
    [SerializeField] private Button exitButton;


    private void Awake()
    {
        exitButton.onClick.AddListener(OnClickExitButton);
        LoadInventoryData();
    }

    public override void Open()
    {
        base.Open();
        Debug.Log("[InventoryUi] Open 호출됨");


        LoadInventoryData();
        RefreshUi();
    }

    public override void Close()
    {
        base.Close();
    }


    // 저장된 인벤토리 데이터 로드
    private void LoadInventoryData()
    {
        itemCounts.Clear();

        if (SaveLoadManager.Data != null)
        {
            var saveData = SaveLoadManager.Data as SaveDataV1;
            if (saveData != null && saveData.inventoryItems != null)
            {
                foreach (var item in saveData.inventoryItems)
                {
                    itemCounts[item.Key] = item.Value;
                }
            }
            if (SaveLoadManager.Data != null)
            {
                if (saveData != null && saveData.inventoryItems != null)
                {
                    Debug.Log("[InventoryUi] 세이브 데이터: " +
                        string.Join(", ", saveData.inventoryItems.Select(kv => $"{kv.Key}:{kv.Value}")));
                }
            }

        }
    }

    // 아이템 추가/갱신 메서드
    public void AddItem(string itemType, int count)
    {
        if (itemCounts.ContainsKey(itemType))
            itemCounts[itemType] += count;
        else
            itemCounts[itemType] = count;

        Debug.Log($"[InventoryUi] 저장된 값: {itemType} = {itemCounts[itemType]}");

        // 세이브 데이터에도 저장
        SaveInventoryItem(itemType, itemCounts[itemType]);
        RefreshUi();
    }

    // 아이템 사용 메서드
    public bool UseItem(string itemType, int count)
    {
        Debug.Log($"[InventoryUi] UseItem 호출: {itemType}, 요청 수량: {count}, 현재 수량: {GetItemCount(itemType)}");

        if (itemCounts.ContainsKey(itemType) && itemCounts[itemType] >= count)
        {
            itemCounts[itemType] -= count;
            if (itemCounts[itemType] <= 0)
                itemCounts.Remove(itemType);

            // 세이브 데이터에도 반영
            SaveInventoryItem(itemType, itemCounts.ContainsKey(itemType) ? itemCounts[itemType] : 0);
            RefreshUi();
            return true;
        }
        return false;
    }

    // 아이템 개수 확인 메서드
    public int GetItemCount(string itemType)
    {
        return itemCounts.ContainsKey(itemType) ? itemCounts[itemType] : 0;
    }

    // 세이브 데이터에 인벤토리 아이템 저장
    private void SaveInventoryItem(string itemType, int count)
    {
        if (SaveLoadManager.Data != null)
        {
            var saveData = SaveLoadManager.Data as SaveDataV1;
            if (saveData != null)
            {
                if (count > 0)
                    saveData.inventoryItems[itemType] = count;
                else if (saveData.inventoryItems.ContainsKey(itemType))
                    saveData.inventoryItems.Remove(itemType);

                SaveLoadManager.Save();
                Debug.Log($"[InventoryUi] {itemType} 저장됨: {count}개");
            }
        }
    }

    // UI 갱신
    private void RefreshUi()
    {
        enhanceTicketUi.SetCount(itemCounts.TryGetValue("EnhanceTicket", out var enhanceCount) ? enhanceCount : 0);
        lotteryTicketUi.SetCount(itemCounts.TryGetValue("LotteryTicket", out var lotteryCount) ? lotteryCount : 0);
    }

    private void OnClickExitButton()
    {
        Close();
    }
}