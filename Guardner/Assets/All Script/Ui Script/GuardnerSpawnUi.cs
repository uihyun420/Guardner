using UnityEngine;
using UnityEngine.UI;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // 가드너 아이템 ui 프리팹 
    public Transform contentParent; // 스크롤랙트의 content

    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector에서 연결

    private int selectedGuardnerId; // 선택된 가드너 ID 저장
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
                Debug.Log("가드너가 이미 존재합니다.");
                return;
            }
            guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos);
        }
        Close();
    }
}
