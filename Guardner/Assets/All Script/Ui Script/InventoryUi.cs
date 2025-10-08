using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : GenericWindow
{
    private Dictionary<string, int> itemCounts = new();

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
        LoadInventoryData();
        RefreshUi();
    }

    public override void Close()
    {
        base.Close();
    }

    // ����� �κ��丮 ������ �ε�
    public void LoadInventoryData()
    {
        itemCounts.Clear();

        var saveData = (SaveDataV1)SaveLoadManager.Data;
        foreach (var item in saveData.inventoryItems)
        {
            itemCounts[item.Key] = item.Value;
        }
    }

    // ������ �߰�/���� �޼���
    public void AddItem(string itemType, int count)
    {
        if (itemCounts.ContainsKey(itemType))
            itemCounts[itemType] += count;
        else
            itemCounts[itemType] = count;

        SaveInventoryItem(itemType, itemCounts[itemType]);
        RefreshUi();
    }

    // ������ ��� �޼���
    public bool UseItem(string itemType, int count)
    {
        if (itemCounts.ContainsKey(itemType) && itemCounts[itemType] >= count)
        {
            itemCounts[itemType] -= count;
            if (itemCounts[itemType] <= 0)
                itemCounts.Remove(itemType);

            SaveInventoryItem(itemType, itemCounts.ContainsKey(itemType) ? itemCounts[itemType] : 0);
            RefreshUi();
            return true;
        }
        return false;
    }

    // ������ ���� Ȯ�� �޼���
    public int GetItemCount(string itemType)
    {
        return itemCounts.ContainsKey(itemType) ? itemCounts[itemType] : 0;
    }

    // ���̺� �����Ϳ� �κ��丮 ������ ����
    private void SaveInventoryItem(string itemType, int count)
    {
        var saveData = (SaveDataV1)SaveLoadManager.Data;
        if (count > 0)
            saveData.inventoryItems[itemType] = count;
        else
            saveData.inventoryItems.Remove(itemType);

        SaveLoadManager.Save();
    }

    // UI ����
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