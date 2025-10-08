using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private MainMenuUi mainMenu;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private TextMeshProUGUI hasGachaItemCountText;
    private int needCount = 1;
    private int lastGachaItemCount = -1;


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
    private void Update()
    {
        SetGoldText();        
    }
    public override void Open()
    {
        UpdateGachaItemCountText();
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

        // 보유 가드너만 표시
        foreach (var guardnerId in GetAllGuardnerIdsOwned())
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
            itemUi.SetData(data, sprite, () => OnEnhanceButton(guardnerId));
        }

    }
    private Sprite GetGuardnerSprite(int guardnerId)
    {
        var sprite = Resources.Load<Sprite>($"GuardnerIcons/{guardnerId}");
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
        if (inventoryUi != null && inventoryUi.UseItem("LotteryTicket", 1))
        {
            var allIds = new List<int>(GetAllGuardnerIdsAll());
            int randomIdx = Random.Range(0, allIds.Count);
            int guardnerId = allIds[randomIdx];
            int level = 1;

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
            
            UpdateGachaItemCountText();
            ResetList();
            SoundManager.soundManager.PlaySFX("GachaResultSfx");
        }
    }

    // 전체 가드너 ID 반환 (뽑기 전용)
    private IEnumerable<int> GetAllGuardnerIdsAll()
    {
        var ids = new HashSet<int>();
        foreach (var data in DataTableManager.GuardnerEnhanceTable.GetAll())
            ids.Add(data.Id);
        return ids;
    }

    // 보유 가드너만 반환 (UI 리스트 등)
    private IEnumerable<int> GetAllGuardnerIdsOwned()
    {
        if (guardnerSpawner != null)
            return guardnerSpawner.ownedGuardnerIds;

        // 예외 상황: 보유 가드너가 없을 때 전체 반환
        return GetAllGuardnerIdsAll();
    }

    private void SetGoldText()
    {
        goldText.text = $"{mainMenu.mainUiGold}";
    }

    private void UpdateGachaItemCountText()
    {
        if (inventoryUi == null) return;

        int currentCount = inventoryUi.GetItemCount("LotteryTicket");
        if (lastGachaItemCount != currentCount)
        {
            hasGachaItemCountText.text = $"가드너 뽑기({currentCount} / {needCount})";
            lastGachaItemCount = currentCount;
        }
    }
}
