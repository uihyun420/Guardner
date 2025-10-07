using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DictionarySetUi : GenericWindow
{
    [Header("Button")]
    [SerializeField] private Button exitButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject guardnerItemPrefab; // GuardnerItemUi ������
    [SerializeField] private GameObject notFoundPrefab; // NotFound ������

    [Header("Layout")]
    [SerializeField] private Transform contentParent; // ���Ե��� �� �θ� ������Ʈ
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
        Debug.Log("[DictionarySetUi] CreateGuardnerSlots ����");
        ClearSlots();

        // null üũ��
        if (contentParent == null)
        {
            Debug.LogError("[DictionarySetUi] contentParent�� null�Դϴ�!");
            return;
        }

        if (guardnerItemPrefab == null)
        {
            Debug.LogError("[DictionarySetUi] guardnerItemPrefab�� null�Դϴ�!");
            return;
        }

        if (notFoundPrefab == null)
        {
            Debug.LogError("[DictionarySetUi] notFoundPrefab�� null�Դϴ�!");
            return;
        }

        if (guardnerSpawner == null)
        {
            Debug.LogError("[DictionarySetUi] guardnerSpawner�� null�Դϴ�!");
            return;
        }

        // ��� ����� ID �������� (1~12������)
        var allGuardnerIds = GetAllGuardnerIds();
        Debug.Log($"[DictionarySetUi] ������ ����� ID ����: {allGuardnerIds.Count}");

        for (int i = 0; i < MAX_GUARDNER_COUNT; i++)
        {
            if (i < allGuardnerIds.Count)
            {
                int guardnerId = allGuardnerIds[i];
                Debug.Log($"[DictionarySetUi] {i}��° ����: ����� ID {guardnerId}");
                CreateSlotForGuardner(guardnerId);
            }
            else
            {
                Debug.Log($"[DictionarySetUi] {i}��° ����: NotFound");
                CreateNotFoundSlot();
            }
        }

        Debug.Log($"[DictionarySetUi] �� ������ ���� ����: {currentSlots.Count}");

        // ��ũ�� ����
        SetupScrollRect();
    }

    private List<int> GetAllGuardnerIds()
    {
        var allIds = new List<int>();

        // GuardnerTable null üũ
        if (DataTableManager.GuardnerTable == null)
        {
            Debug.LogError("[DictionarySetUi] GuardnerTable�� null�Դϴ�!");
            return allIds;
        }

        // DataTable���� ��� ����� ID�� �����ͼ� ����
        foreach (var data in DataTableManager.GuardnerTable.GetAll())
        {
            allIds.Add(data.Id);
        }

        Debug.Log($"[DictionarySetUi] ��ü ����� ID��: {string.Join(", ", allIds)}");
        var result = allIds.OrderBy(id => id).Take(MAX_GUARDNER_COUNT).ToList();
        Debug.Log($"[DictionarySetUi] ���� �� 12��: {string.Join(", ", result)}");

        return result;
    }

    private void CreateSlotForGuardner(int guardnerId)
    {
        bool isOwned = guardnerSpawner != null && guardnerSpawner.HasGuardner(guardnerId);
        Debug.Log($"[DictionarySetUi] ����� {guardnerId} ���� ����: {isOwned}");

        if (guardnerSpawner != null)
        {
            Debug.Log($"[DictionarySetUi] ������ ����ʵ�: {string.Join(", ", guardnerSpawner.ownedGuardnerIds)}");
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
        Debug.Log($"[DictionarySetUi] ������ ����� ���� ����: {guardnerId}");

        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
        if (guardnerData == null)
        {
            Debug.LogError($"[DictionarySetUi] ����� �����͸� ã�� �� ����: {guardnerId}");
            return;
        }

        var slot = Instantiate(guardnerItemPrefab, contentParent);
        Debug.Log($"[DictionarySetUi] ����� ���� ������: {slot.name}");

        var itemUi = slot.GetComponent<GuardnerItemUi>();

        if (itemUi != null)
        {
            // Ŭ�� �� ���常 ��� (DictionarySetDiscriptUi�� �̹� �������� ǥ�õ�)
            itemUi.DictionarySetData(guardnerData, (id) =>
            {
                SoundManager.soundManager.PlaySFX("UiClick2Sfx");
                // �ʿ��ϴٸ� ���⼭ �ش� ������� ��Ʈ�� ���̶���Ʈ�� �� �ֽ��ϴ�
            });
        }
        else
        {
            Debug.LogError($"[DictionarySetUi] GuardnerItemUi ������Ʈ�� ã�� �� ����");
        }

        currentSlots.Add(slot);
    }

    private void CreateNotFoundSlot()
    {
        Debug.Log("[DictionarySetUi] NotFound ���� ����");
        var slot = Instantiate(notFoundPrefab, contentParent);
        Debug.Log($"[DictionarySetUi] NotFound ���� ������: {slot.name}");
        currentSlots.Add(slot);
    }

    private void ClearSlots()
    {
        Debug.Log($"[DictionarySetUi] ���� ���� {currentSlots.Count}�� ����");
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
        // �ʿ��� �ʱ�ȭ �۾�
    }
}