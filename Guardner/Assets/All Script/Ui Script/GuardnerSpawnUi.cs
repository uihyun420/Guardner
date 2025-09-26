using UnityEngine;
using UnityEngine.UI;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // 가드너 아이템 ui 프리팹 
    public Transform contentParent; // 스크롤랙트의 content

    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector에서 연결
    [SerializeField] private BattleUi battleUi;

    private int selectedGuardnerId; // 선택된 가드너 ID 저장
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
                Debug.Log("이미 가드너가 소환된 위치입니다.");
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

        // 콘텐츠 크기와 뷰포트 크기 비교해서 스크롤 활성/비활성
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
                Debug.Log("가드너가 이미 존재합니다.");
                return;
            }
            guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos);
            battleUi.UpdateGuardnerCount();
        }
        Close();
    }
}
