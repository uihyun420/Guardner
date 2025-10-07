using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DictionarySetUi : GenericWindow
{
    [Header("Button")]
    [SerializeField] private Button exitButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject guardnerItemPrefab; // GuardnerItemUi 프리팹
    [SerializeField] private GameObject notFoundPrefab; // NotFound 프리팹

    [Header("Layout")]
    [SerializeField] private Transform contentParent; // 슬롯들이 들어갈 부모 오브젝트
    [SerializeField] private ScrollRect scrollRect;

    [Header("References")]
    [SerializeField] private GuardnerSpawner guardnerSpawner;
    [SerializeField] private DictionarySetDiscriptUi dictionarySetDiscriptUi;

    private const int MAX_GUARDNER_COUNT = 12;
    private List<GameObject> currentSlots = new List<GameObject>();

    private void Awake()
    {
        isOverlayWindow = true;

        exitButton.onClick.AddListener(() =>
        {
            SoundManager.soundManager.PlaySfxButton1();
            Close();
        });
    }

    public override void Open()
    {
        base.Open();
        CreateGuardnerSlots();
    }

    public override void Close()
    {
        base.Close();
        ClearSlots();
    }

    private void CreateGuardnerSlots()
    {
        Debug.Log("[DictionarySetUi] CreateGuardnerSlots 시작");
        ClearSlots();

        // null 체크들
        if (contentParent == null)
        {
            Debug.LogError("[DictionarySetUi] contentParent가 null입니다!");
            return;
        }

        if (guardnerItemPrefab == null)
        {
            Debug.LogError("[DictionarySetUi] guardnerItemPrefab이 null입니다!");
            return;
        }

        if (notFoundPrefab == null)
        {
            Debug.LogError("[DictionarySetUi] notFoundPrefab이 null입니다!");
            return;
        }

        if (guardnerSpawner == null)
        {
            Debug.LogError("[DictionarySetUi] guardnerSpawner가 null입니다!");
            return;
        }

        // 모든 가드너 ID 가져오기 (1~12번까지)
        var allGuardnerIds = GetAllGuardnerIds();
        Debug.Log($"[DictionarySetUi] 가져온 가드너 ID 개수: {allGuardnerIds.Count}");

        for (int i = 0; i < MAX_GUARDNER_COUNT; i++)
        {
            if (i < allGuardnerIds.Count)
            {
                int guardnerId = allGuardnerIds[i];
                Debug.Log($"[DictionarySetUi] {i}번째 슬롯: 가드너 ID {guardnerId}");
                CreateSlotForGuardner(guardnerId);
            }
            else
            {
                Debug.Log($"[DictionarySetUi] {i}번째 슬롯: NotFound");
                CreateNotFoundSlot();
            }
        }

        Debug.Log($"[DictionarySetUi] 총 생성된 슬롯 개수: {currentSlots.Count}");

        // 스크롤 설정
        SetupScrollRect();
    }

    private List<int> GetAllGuardnerIds()
    {
        var allIds = new List<int>();

        // GuardnerTable null 체크
        if (DataTableManager.GuardnerTable == null)
        {
            Debug.LogError("[DictionarySetUi] GuardnerTable이 null입니다!");
            return allIds;
        }

        // DataTable에서 모든 가드너 ID를 가져와서 정렬
        foreach (var data in DataTableManager.GuardnerTable.GetAll())
        {
            allIds.Add(data.Id);
        }

        Debug.Log($"[DictionarySetUi] 전체 가드너 ID들: {string.Join(", ", allIds)}");
        var result = allIds.OrderBy(id => id).Take(MAX_GUARDNER_COUNT).ToList();
        Debug.Log($"[DictionarySetUi] 정렬 후 12개: {string.Join(", ", result)}");

        return result;
    }

    private void CreateSlotForGuardner(int guardnerId)
    {
        bool isOwned = guardnerSpawner != null && guardnerSpawner.HasGuardner(guardnerId);
        Debug.Log($"[DictionarySetUi] 가드너 {guardnerId} 소유 여부: {isOwned}");

        if (guardnerSpawner != null)
        {
            Debug.Log($"[DictionarySetUi] 소유한 가드너들: {string.Join(", ", guardnerSpawner.ownedGuardnerIds)}");
        }

        if (isOwned)
        {
            CreateOwnedGuardnerSlot(guardnerId);
        }
        else
        {
            CreateNotFoundSlot();
        }
    }

    private void CreateOwnedGuardnerSlot(int guardnerId)
    {
        Debug.Log($"[DictionarySetUi] 소유한 가드너 슬롯 생성: {guardnerId}");

        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
        if (guardnerData == null)
        {
            Debug.LogError($"[DictionarySetUi] 가드너 데이터를 찾을 수 없음: {guardnerId}");
            return;
        }

        var slot = Instantiate(guardnerItemPrefab, contentParent);
        Debug.Log($"[DictionarySetUi] 가드너 슬롯 생성됨: {slot.name}");

        var itemUi = slot.GetComponent<GuardnerItemUi>();

        if (itemUi != null)
        {
            // 클릭 시 사운드만 재생 (DictionarySetDiscriptUi는 이미 고정으로 표시됨)
            itemUi.DictionarySetData(guardnerData, (id) =>
            {
                SoundManager.soundManager.PlaySFX("UiClick2Sfx");
                // 필요하다면 여기서 해당 가드너의 세트를 하이라이트할 수 있습니다
            });
        }
        else
        {
            Debug.LogError($"[DictionarySetUi] GuardnerItemUi 컴포넌트를 찾을 수 없음");
        }

        currentSlots.Add(slot);
    }

    private void CreateNotFoundSlot()
    {
        Debug.Log("[DictionarySetUi] NotFound 슬롯 생성");
        var slot = Instantiate(notFoundPrefab, contentParent);
        Debug.Log($"[DictionarySetUi] NotFound 슬롯 생성됨: {slot.name}");
        currentSlots.Add(slot);
    }

    private void ClearSlots()
    {
        Debug.Log($"[DictionarySetUi] 기존 슬롯 {currentSlots.Count}개 정리");
        foreach (var slot in currentSlots)
        {
            if (slot != null)
                Destroy(slot);
        }
        currentSlots.Clear();
    }

    private void SetupScrollRect()
    {
        if (scrollRect != null)
        {
            var contentHeight = scrollRect.content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            scrollRect.vertical = contentHeight > viewportHeight;
        }
    }

    private void Start()
    {
        // 필요한 초기화 작업
    }
}