using UnityEngine;
using System.Collections.Generic;
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
        Debug.Log("[InventoryUi] Awake - ���� ���� ����:");
        Debug.Log($"enhanceTicketUi: {(enhanceTicketUi != null ? "�����" : "NULL")}");
        Debug.Log($"lotteryTicketUi: {(lotteryTicketUi != null ? "�����" : "NULL")}");
    }

    public override void Open()
    {
        base.Open();
        Debug.Log("[InventoryUi] Open ȣ���");

        RefreshUi();
    }

    public override void Close()
    {
        base.Close();
    }

    // ������ �߰�/���� �޼���
    public void AddItem(string itemType, int count)
    {
        if (itemCounts.ContainsKey(itemType))
            itemCounts[itemType] += count;
        else
            itemCounts[itemType] = count;

        Debug.Log($"[InventoryUi] ����� ��: {itemType} = {itemCounts[itemType]}");
        RefreshUi();
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