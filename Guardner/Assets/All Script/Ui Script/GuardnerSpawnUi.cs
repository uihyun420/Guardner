using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // 가드너 아이템 ui 프리팹 
    public Transform contentParent; // 스크롤랙트의 content

    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector에서 연결

    private int selectedGuardnerId; // 선택된 가드너 ID 저장

    public override void Open()
    {
        base.Open();
        DisplayAvailableGuardner();
    }

    public override void Close()
    {
        base.Close();
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
            itemUi.SetData(data, OnSelectGuardner);
    }

    private void OnSelectGuardner(int guardnerId)
    {
        selectedGuardnerId = guardnerId;

        if (guardnerSpawner.spawnPos.Length > 0)
        {
            Vector2 selectedSpawnPos = guardnerSpawner.spawnPos[0].transform.position;
            guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos);
        }
        Close();
    }
}
