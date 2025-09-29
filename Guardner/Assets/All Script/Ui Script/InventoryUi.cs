using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUi : GenericWindow
{
    private Dictionary<string, int> itemCounts = new();

    // ������ UI ������Ʈ ���� (�����տ��� �Ҵ�)
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
        Debug.Log("[InventoryUi] Open ȣ���");


        LoadInventoryData();
        RefreshUi();
    }

    public override void Close()
    {
        base.Close();
    }


    // ����� �κ��丮 ������ �ε�
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
                    Debug.Log("[InventoryUi] ���̺� ������: " +
                        string.Join(", ", saveData.inventoryItems.Select(kv => $"{kv.Key}:{kv.Value}")));
                }
            }

        }
    }

    // ������ �߰�/���� �޼���
    public void AddItem(string itemType, int count)
    {
        if (itemCounts.ContainsKey(itemType))
            itemCounts[itemType] += count;
        else
            itemCounts[itemType] = count;

        Debug.Log($"[InventoryUi] ����� ��: {itemType} = {itemCounts[itemType]}");

        // ���̺� �����Ϳ��� ����
        SaveInventoryItem(itemType, itemCounts[itemType]);
        RefreshUi();
    }

    // ������ ��� �޼���
    public bool UseItem(string itemType, int count)
    {
        Debug.Log($"[InventoryUi] UseItem ȣ��: {itemType}, ��û ����: {count}, ���� ����: {GetItemCount(itemType)}");

        if (itemCounts.ContainsKey(itemType) && itemCounts[itemType] >= count)
        {
            itemCounts[itemType] -= count;
            if (itemCounts[itemType] <= 0)
                itemCounts.Remove(itemType);

            // ���̺� �����Ϳ��� �ݿ�
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
                Debug.Log($"[InventoryUi] {itemType} �����: {count}��");
            }
        }
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