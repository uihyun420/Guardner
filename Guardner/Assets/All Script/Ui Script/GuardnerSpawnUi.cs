using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // 가드너 아이템 ui 프리팹 
    public Transform contentParent; // 스크롤랙트 content

    [SerializeField] private GameObject blockScreenPanel;
    [SerializeField] private GuardnerSpawner guardnerSpawner; 
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private Button ExitButton;
    [SerializeField] private ScreenTouch screenTouch; 
    [SerializeField] private ReCellUi reCellUi;


    private int selectedGuardnerId; // 선택된 가드너 ID 저장
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
        if (reCellUi != null)
        {
            reCellUi.Close(); 
            reCellUi.gameObject.SetActive(false);
            reCellUi.enabled = false;
        }

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
            selectedAreaIndex = 0;
        }

        base.Open();

        if (screenTouch != null)
        {
            screenTouch.enabled = false;
        }

        DisplayAvailableGuardner();

        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.enabled = true;
            scrollRect.gameObject.SetActive(true);
        }
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
        if (guardnerSpawner != null)
        {
            // 일반 모드
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
            // 튜토리얼 모드: 기본 가드너들 표시
            var tutorialGuardnerIds = new int[] { 11120, 11235 }; // 원하는 가드너 ID로 변경
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

    //private void OnSelectGuardner(int guardnerId)
    //{
    //    selectedGuardnerId = guardnerId;

    //    if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
    //    {
    //        Vector2 selectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;

    //        if (guardnerSpawner.IsGuardnerAtPosition(selectedSpawnPos))
    //        {
    //            return;
    //        }
    //        if (guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos))
    //        {
    //            battleUi.UpdateGuardnerCount();
    //        }
    //    }
    //    Close();
    //}

    private void OnClickExitButton()
    {
        Close();
    }
    private void OnSelectGuardner(int guardnerId)
    {
        selectedGuardnerId = guardnerId;

        if (guardnerSpawner != null)
        {
            // 일반 모드
            if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
            {
                Vector2 selectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;

                if (guardnerSpawner.IsGuardnerAtPosition(selectedSpawnPos))
                {
                    return;
                }

                // 튜토리얼 모드인지 확인
                var tutorialSpawner = guardnerSpawner as TutorialGuardnerSpawner;
                if (tutorialSpawner != null)
                {
                    if (tutorialSpawner.SpawnGuardnerForTutorial(guardnerId, selectedSpawnPos))
                    {
                        if (battleUi != null)
                        {
                            battleUi.UpdateGuardnerCount();
                        }
                    }
                }
                else
                {
                    // 일반 모드
                    if (guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos))
                    {
                        if (battleUi != null)
                        {
                            battleUi.UpdateGuardnerCount();
                        }
                    }
                }
            }
        }
        Close();
    }
}
