using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // ����� ������ ui ������ 
    public Transform contentParent; // ��ũ�ѷ�Ʈ�� content

    [SerializeField] private GameObject blockScreenPanel;
    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector���� ����
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private Button ExitButton;
    [SerializeField] private ScreenTouch screenTouch; // ScreenTouch ���� �߰�
    [SerializeField] private ReCellUi reCellUi;

    private int selectedGuardnerId; // ���õ� ����� ID ����
    private int selectedAreaIndex;


    private void Update()
    {
        if (contentParent == null || battleUi == null) return;

        foreach (Transform child in contentParent)
        {
            var itemUi = child.GetComponent<GuardnerItemUi>();
            if (itemUi != null && itemUi.Data != null) 
            {
                if (battleUi.gold < itemUi.Data.SummonGold)
                {
                    itemUi.SetTextColor(Color.red);
                    itemUi.SetNameTextColor(Color.red);
                }
                else
                {
                    itemUi.SetTextColor(Color.white);
                    itemUi.SetNameTextColor(Color.white);
                }
            }
        }
    }

    private void Awake()
    {
        isOverlayWindow = true;
        ExitButton.onClick.AddListener(OnClickExitButton);
    }

    public override void Open()
    {

        reCellUi.Close(); // Close() �޼��� ȣ��� ������ �ݱ�
        reCellUi.gameObject.SetActive(false);
        reCellUi.enabled = false;

        selectedAreaIndex = guardnerSpawner.screenTouch.GetSelectedAreaIndex();
        if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
        {
            Vector2 expectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;
            if (guardnerSpawner.IsGuardnerAtPosition(expectedSpawnPos))
            {
                return;
            }
        }


        // guardnerSpawner�� null�� ��� (Ʃ�丮�� ���) ó��
        if (guardnerSpawner != null && guardnerSpawner.screenTouch != null)
        {
            selectedAreaIndex = guardnerSpawner.screenTouch.GetSelectedAreaIndex();
            if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
            {
                Vector2 expectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;
                if (guardnerSpawner.IsGuardnerAtPosition(expectedSpawnPos))
                {
                    return;
                }
            }
        }
        else
        {
            // Ʃ�丮�� ���: �⺻�� ����
            selectedAreaIndex = 0;
        }

        base.Open();

        screenTouch.enabled = false;

        DisplayAvailableGuardner();

        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.enabled = true;
        scrollRect.gameObject.SetActive(true);
    }

    public override void Close()
    {
        screenTouch.enabled = true;
        base.Close();
    }

    private void DisplayAvailableGuardner()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ������ ����ʸ� ǥ��
        if (guardnerSpawner != null)
        {
            // �Ϲ� ���
            foreach (var guardnerId in guardnerSpawner.ownedGuardnerIds)
            {
                var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
                if (guardnerData != null)
                {
                    CreateGuardnerItem(guardnerData, guardnerId);
                }
            }
        }
        else
        {
            // Ʃ�丮�� ���: �⺻ ����ʵ� ǥ��
            var tutorialGuardnerIds = new int[] { 11120, 11235 }; // ���ϴ� ����� ID�� ����
            foreach (var guardnerId in tutorialGuardnerIds)
            {
                var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
                if (guardnerData != null)
                {
                    CreateGuardnerItem(guardnerData, guardnerId);
                }
            }
        }

        if (scrollRect != null && scrollRect.content != null && scrollRect.viewport != null)
        {
            var contentHeight = scrollRect.content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            scrollRect.vertical = contentHeight > viewportHeight;
        }
    }

    private void CreateGuardnerItem(GuardnerData data, int guardnerId)
    {
        var item = Instantiate(guardnerItemPrefab, contentParent);
        var itemUi = item.GetComponent<GuardnerItemUi>();

        if (itemUi != null)
        {
            itemUi.SetData(data, OnSelectGuardner);

            if (battleUi.gold < data.SummonGold)
            {
                itemUi.SetTextColor(Color.red);
                itemUi.SetNameTextColor(Color.red);
            }
            else
            {
                itemUi.SetTextColor(Color.white);
                itemUi.SetNameTextColor(Color.white);
            }
        }
    }

    private void OnSelectGuardner(int guardnerId)
    {
        selectedGuardnerId = guardnerId;

        if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
        {
            Vector2 selectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;

            if (guardnerSpawner.IsGuardnerAtPosition(selectedSpawnPos))
            {
                return;
            }
            if (guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos))
            {
                battleUi.UpdateGuardnerCount();
            }
        }
        Close();
    }

    private void OnClickExitButton()
    {
        Close();
    }
}
