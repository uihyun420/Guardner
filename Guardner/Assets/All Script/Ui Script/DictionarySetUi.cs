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

    private const int maxGuardnerCount = 12;
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
        ClearSlots();

        // 모든 가드너 ID 가져오기 (1~12번까지)
        var allGuardnerIds = GetAllGuardnerIds();

        for (int i = 0; i < maxGuardnerCount; i++)
        {
            if (i < allGuardnerIds.Count)
            {
                int guardnerId = allGuardnerIds[i];
                CreateSlotForGuardner(guardnerId);
            }
            else
            {
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

        if (DataTableManager.GuardnerTable == null)
        {
            return allIds;
        }

        // DataTable에서 모든 가드너 ID를 가져와서 정렬
        foreach (var data in DataTableManager.GuardnerTable.GetAll())
        {
            allIds.Add(data.Id);
        }
        var result = allIds.OrderBy(id => id).Take(maxGuardnerCount).ToList();
        return result;
    }

    private void CreateSlotForGuardner(int guardnerId)
    {
        bool isOwned = guardnerSpawner != null && guardnerSpawner.HasGuardner(guardnerId);
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

        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
        if (guardnerData == null)
        {
            return;
        }

        var slot = Instantiate(guardnerItemPrefab, contentParent);

        var itemUi = slot.GetComponent<GuardnerItemUi>();

        itemUi.DictionarySetData(guardnerData, (id) =>
        {
            SoundManager.soundManager.PlaySFX("UiClick2Sfx");
        });


        currentSlots.Add(slot);
    }

    private void CreateNotFoundSlot()
    {
        var slot = Instantiate(notFoundPrefab, contentParent);
        currentSlots.Add(slot);
    }

    private void ClearSlots()
    {
        foreach (var slot in currentSlots)
        {
            if (slot != null)
                Destroy(slot);
        }
        currentSlots.Clear();
    }

    private void SetupScrollRect()
    {
        var contentHeight = scrollRect.content.rect.height;
        var viewportHeight = scrollRect.viewport.rect.height;

        scrollRect.vertical = true;
        scrollRect.horizontal = false;
    }
}