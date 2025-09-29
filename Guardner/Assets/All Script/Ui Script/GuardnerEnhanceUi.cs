using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerEnhanceUi : GenericWindow
{
    public ScrollRect scrollRect;
    public Transform contentParent;
    public GameObject guardnerEnhanceItemPrefab; // 각 가드너 항목 프리팹

    [SerializeField] private Button BackButton;
    [SerializeField] private Button guardnerButton;

    [SerializeField] private GuardnerEnhanceResultUi guardnerEnhanceResultUi;

    [SerializeField] private Button guardnerGatchButton;
    [SerializeField] private Button skillGatchButton;
    [SerializeField] private GatchaUi gatchaUi;

    [SerializeField] private InventoryUi inventoryUi;
    [SerializeField] private GuardnerSpawner guardnerSpawner;

    // 현재 강화 레벨 정보 (예시: 실제로는 세이브 데이터 등에서 불러와야 함)
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
        // 기존 항목 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 모든 강화 데이터 가져오기 (Id+Level 구조라면, Id별로 현재 레벨만 표시)
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
        // 실제로는 플레이어 데이터에서 현재 강화 레벨을 가져와야 함
        if (guardnerLevelDict.TryGetValue(guardnerId, out int level))
            return level;
        return 1; // 기본 1레벨
    }

    private void CreateEnhanceItem(GuardnerEnhanceData data, int guardnerId, int level)
    {
        var go = Instantiate(guardnerEnhanceItemPrefab, contentParent);
        var itemUi = go.GetComponent<GuardnerEnhanceItemUi>();
        if (itemUi != null)
        {
            var sprite = GetGuardnerSprite(guardnerId);
            // ← 한 번만 호출하고, 실제 guardnerId 전달
            itemUi.SetData(data, sprite, () => OnEnhanceButton(guardnerId));
        }
    }
    private Sprite GetGuardnerSprite(int guardnerId)
    {
        var sprite = Resources.Load<Sprite>($"GuardnerIcons/{guardnerId}");
        if (sprite == null)
            Debug.LogWarning($"이미지 없음: GuardnerIcons/{guardnerId}");

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
        // 뽑기권 개수 확인
        Debug.Log("[Gatcha] UseItem 호출 전 LotteryTicket 개수: " + inventoryUi.GetItemCount("LotteryTicket"));

        if (inventoryUi != null && inventoryUi.UseItem("LotteryTicket", 1))
        {
            var allIds = new List<int>(GetAllGuardnerIds());
            int randomIdx = Random.Range(0, allIds.Count);
            int guardnerId = allIds[randomIdx];
            int level = 1;

            // GuardnerSpawner를 통해 가드너 획득
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
            Debug.Log("뽑기권이 부족합니다.");
        }
    }

    private IEnumerable<int> GetAllGuardnerIds()
    {
        // 보유한 가드너 목록에서 가져오도록 변경
        if (guardnerSpawner != null)
            return guardnerSpawner.ownedGuardnerIds;

        // 백업: 강화테이블의 모든 ID
        var ids = new HashSet<int>();
        foreach (var data in DataTableManager.GuardnerEnhanceTable.GetAll())
            ids.Add(data.Id);
        return ids;
    }
}
