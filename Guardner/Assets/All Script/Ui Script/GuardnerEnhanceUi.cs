using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerEnhanceUi : GenericWindow
{
    public ScrollRect scrollRect;
    public Transform contentParent;
    public GameObject guardnerEnhanceItemPrefab; // �� ����� �׸� ������

    [SerializeField] private Button BackButton;
    [SerializeField] private Button guardnerButton;

    [SerializeField] private GuardnerEnhanceResultUi guardnerEnhanceResultUi;

    [SerializeField] private Button guardnerGatchButton;
    [SerializeField] private Button skillGatchButton;
    [SerializeField] private GatchaUi gatchaUi;

    [SerializeField] private InventoryUi inventoryUi;
    [SerializeField] private GuardnerSpawner guardnerSpawner;

    // ���� ��ȭ ���� ���� (����: �����δ� ���̺� ������ ��� �ҷ��;� ��)
    private Dictionary<int, int> guardnerLevelDict = new Dictionary<int, int>();

    private void OnEnable()
    {
        ResetList();
    }

    private void Awake()
    {
        BackButton.onClick.AddListener(OnClickBackButton);
        guardnerGatchButton.onClick.AddListener(OnClickGuardnerGatchButton);
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }

    public void ResetList()
    {
        // ���� �׸� ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ��� ��ȭ ������ �������� (Id+Level �������, Id���� ���� ������ ǥ��)
        foreach (var guardnerId in GetAllGuardnerIds())
        {
            int level = GetCurrentLevel(guardnerId);
            var enhanceData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, level);
            if (enhanceData != null)
            {
                CreateEnhanceItem(enhanceData, guardnerId, level);
            }
        }
    }


    private int GetCurrentLevel(int guardnerId)
    {
        // �����δ� �÷��̾� �����Ϳ��� ���� ��ȭ ������ �����;� ��
        if (guardnerLevelDict.TryGetValue(guardnerId, out int level))
            return level;
        return 1; // �⺻ 1����
    }

    private void CreateEnhanceItem(GuardnerEnhanceData data, int guardnerId, int level)
    {
        var go = Instantiate(guardnerEnhanceItemPrefab, contentParent);
        var itemUi = go.GetComponent<GuardnerEnhanceItemUi>();
        if (itemUi != null)
        {
            var sprite = GetGuardnerSprite(guardnerId);
            // �� �� ���� ȣ���ϰ�, ���� guardnerId ����
            itemUi.SetData(data, sprite, () => OnEnhanceButton(guardnerId));
        }
    }
    private Sprite GetGuardnerSprite(int guardnerId)
    {
        var sprite = Resources.Load<Sprite>($"GuardnerIcons/{guardnerId}");
        if (sprite == null)
            Debug.LogWarning($"�̹��� ����: GuardnerIcons/{guardnerId}");

        Debug.Log($"[GetGuardnerSprite] guardnerId: {guardnerId}");
        return sprite;
    }

    private void OnEnhanceButton(int guardnerId)
    {
        int level = GetCurrentLevel(guardnerId);

        guardnerEnhanceResultUi.SetEnhanceData(guardnerId, level);
        guardnerEnhanceResultUi.Open();
    }

    private void OnClickBackButton()
    {
        Close();
    }

    private void OnClickGuardnerGatchButton()
    {
        // �̱�� ���� Ȯ��
        Debug.Log("[Gatcha] UseItem ȣ�� �� LotteryTicket ����: " + inventoryUi.GetItemCount("LotteryTicket"));

        if (inventoryUi != null && inventoryUi.UseItem("LotteryTicket", 1))
        {
            var allIds = new List<int>(GetAllGuardnerIds());
            int randomIdx = Random.Range(0, allIds.Count);
            int guardnerId = allIds[randomIdx];
            int level = 1;

            // GuardnerSpawner�� ���� ����� ȹ��
            if (guardnerSpawner != null)
            {
                guardnerSpawner.AcquireGuardner(guardnerId);
            }

            var enhanceData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, level);
            if (gatchaUi != null && enhanceData != null)
            {
                gatchaUi.SetGuardnerInfo(enhanceData, GetGuardnerSprite(guardnerId));
                gatchaUi.Open();
            }
        }
        else
        {
            Debug.Log("�̱���� �����մϴ�.");
        }
    }

    private IEnumerable<int> GetAllGuardnerIds()
    {
        // ������ ����� ��Ͽ��� ���������� ����
        if (guardnerSpawner != null)
            return guardnerSpawner.ownedGuardnerIds;

        // ���: ��ȭ���̺��� ��� ID
        var ids = new HashSet<int>();
        foreach (var data in DataTableManager.GuardnerEnhanceTable.GetAll())
            ids.Add(data.Id);
        return ids;
    }
}
