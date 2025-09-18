using UnityEngine;
using UnityEngine.UI;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // ����� ������ ui ������ 
    public Transform contentParent; // ��ũ�ѷ�Ʈ�� content

    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector���� ����

    private int selectedGuardnerId; // ���õ� ����� ID ����
    private int selectedAreaIndex;

    public override void Open()
    {
        base.Open();
        guardnerSpawner.screenTouch.SetUiBlocking(true);

        selectedAreaIndex = guardnerSpawner.screenTouch.GetSelectedAreaIndex();
        if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
        {
            Vector2 expectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;
        }
        DisplayAvailableGuardner();
        
    }

    public override void Close()
    {
        base.Close();
        guardnerSpawner.screenTouch.SetUiBlocking(false);
    }

    private void DisplayAvailableGuardner()
    {
        foreach(Transform child in contentParent)
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
    }

    private void CreateGuardnerItem(GuardnerData data, int guardnerId)
    {
        var item = Instantiate(guardnerItemPrefab, contentParent);
        var itemUi = item.GetComponent<GuardnerItemUi>();

        if (itemUi != null)
        {
            itemUi.SetData(data, OnSelectGuardner);
        }
    }

    private void OnSelectGuardner(int guardnerId)
    {
        selectedGuardnerId = guardnerId;

        if(selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
        {
            Vector2 selectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;

            if(guardnerSpawner.IsGuardnerAtPosition(selectedSpawnPos))
            {
                Debug.Log("����ʰ� �̹� �����մϴ�.");
                return;
            }
            guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos);
        }
        Close();
    }
}
