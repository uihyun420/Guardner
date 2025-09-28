using UnityEngine;
using System.Collections.Generic;
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
        Debug.Log("[InventoryUi] Awake - 슬롯 연결 상태:");
        Debug.Log($"enhanceTicketUi: {(enhanceTicketUi != null ? "연결됨" : "NULL")}");
        Debug.Log($"lotteryTicketUi: {(lotteryTicketUi != null ? "연결됨" : "NULL")}");
    }

    public override void Open()
    {
        base.Open();
        Debug.Log("[InventoryUi] Open 호출됨");

        RefreshUi();
    }

    public override void Close()
    {
        base.Close();
    }

    // 아이템 추가/갱신 메서드
    public void AddItem(string itemType, int count)
    {
        if (itemCounts.ContainsKey(itemType))
            itemCounts[itemType] += count;
        else
            itemCounts[itemType] = count;

        Debug.Log($"[InventoryUi] 저장된 값: {itemType} = {itemCounts[itemType]}");
        RefreshUi();
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