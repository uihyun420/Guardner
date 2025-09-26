using UnityEngine;
using UnityEngine.UI;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // ����� ������ ui ������ 
    public Transform contentParent; // ��ũ�ѷ�Ʈ�� content

    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector���� ����
    [SerializeField] private BattleUi battleUi;

    private int selectedGuardnerId; // ���õ� ����� ID ����
    private int selectedAreaIndex;


    private void Awake()
    {
        isOverlayWindow = true;
    }

    public override void Open()
    {
        selectedAreaIndex = guardnerSpawner.screenTouch.GetSelectedAreaIndex();
        if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
        {
            Vector2 expectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;
            if (guardnerSpawner.IsGuardnerAtPosition(expectedSpawnPos))
            {
                Debug.Log("�̹� ����ʰ� ��ȯ�� ��ġ�Դϴ�.");
                return;
            }
        }

        base.Open();

        if (scrollRect != null)
            scrollRect.horizontal = false;

        DisplayAvailableGuardner();
    }

    public override void Close()
    {
        base.Close();
    }

    private void DisplayAvailableGuardner()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var prefabInfo in guardnerSpawner.guardnerPrefabs)
        {
            var guardnerData = DataTableManager.GuardnerTable.Get(prefabInfo.guardnerId);
            if (guardnerData != null)
            {
                CreateGuardnerItem(guardnerData, prefabInfo.guardnerId);
            }
        }

        // ������ ũ��� ����Ʈ ũ�� ���ؼ� ��ũ�� Ȱ��/��Ȱ��
        if (scrollRect != null)
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
            }
            else
            {
                itemUi.SetTextColor(Color.black);
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
                Debug.Log("����ʰ� �̹� �����մϴ�.");
                return;
            }
            guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos);
            battleUi.UpdateGuardnerCount();
        }
        Close();
    }
}
