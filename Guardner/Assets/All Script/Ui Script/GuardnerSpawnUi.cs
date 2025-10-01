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

    private void Awake()
    {
        isOverlayWindow = true;
        ExitButton.onClick.AddListener(OnClickExitButton);
    }

    public override void Open()
    {
        reCellUi.gameObject.SetActive(false);
        reCellUi.enabled = false;

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

        // ScreenTouch를 완전히 비활성화
        if (screenTouch != null)
        {
            screenTouch.enabled = false; // 컴포넌트 자체를 비활성화
        }

        // 강화 정보가 갱신될 수 있으므로 매번 새로 표시
        DisplayAvailableGuardner();

        // ScrollRect 설정을 DisplayAvailableGuardner 후에 실행 (PlayerSkillSetUi와 동일하게)
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.enabled = true;
            scrollRect.gameObject.SetActive(true);
        }

        // 지연 후 ScrollRect 초기화 (PlayerSkillSetUi와 동일한 코루틴 사용)
        StartCoroutine(InitializeScrollRect());
    }


    private IEnumerator InitializeScrollRect()
    {
        // 2프레임 대기 (UI가 완전히 생성될 때까지)
        yield return null;
        yield return null;

        if (scrollRect != null)
        {
            // Canvas 강제 업데이트
            Canvas.ForceUpdateCanvases();

            // ScrollRect 재설정
            scrollRect.enabled = false;
            yield return null;
            scrollRect.enabled = true;

            // 스크롤 위치를 맨 위로 초기화
            scrollRect.verticalNormalizedPosition = 1f;

            // EventSystem 클리어 후 다시 설정
            EventSystem.current.SetSelectedGameObject(null);
            yield return null;
            EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);
        }
    }

    public override void Close()
    {
        // ScreenTouch 다시 활성화
        if (screenTouch != null)
        {
            screenTouch.enabled = true;
        }

        base.Close();

    }

    private void DisplayAvailableGuardner()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 보유한 가드너만 표시하도록 변경
        if (guardnerSpawner != null)
        {
            foreach (var guardnerId in guardnerSpawner.ownedGuardnerIds)
            {
                var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
                if (guardnerData != null)
                {
                    CreateGuardnerItem(guardnerData, guardnerId);
                }
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
                Debug.Log("가드너가 이미 존재합니다.");
                return;
            }
           if(guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos))
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
