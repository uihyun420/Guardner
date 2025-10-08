using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // 가드너 아이템 ui 프리팹 
    public Transform contentParent; // 스크롤랙트의 content

    [SerializeField] private GameObject blockScreenPanel;
    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector에서 연결
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private Button ExitButton;
    [SerializeField] private ScreenTouch screenTouch; // ScreenTouch 참조 추가
    [SerializeField] private ReCellUi reCellUi;

    private int selectedGuardnerId; // 선택된 가드너 ID 저장
    private int selectedAreaIndex;


    private void Update()
    {
        foreach (Transform child in contentParent)
        {
            var itemUi = child.GetComponent<GuardnerItemUi>();
            if (itemUi != null && itemUi.Data != null) // Data 프로퍼티가 있다고 가정
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

        reCellUi.Close(); // Close() 메서드 호출로 완전히 닫기
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

        // 보유한 가드너만 표시
        foreach (var guardnerId in guardnerSpawner.ownedGuardnerIds)
        {
            var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
            if (guardnerData != null)
            {
                CreateGuardnerItem(guardnerData, guardnerId);
            }
        }

        var contentHeight = scrollRect.content.rect.height;
        var viewportHeight = scrollRect.viewport.rect.height;
        scrollRect.vertical = contentHeight > viewportHeight;
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
