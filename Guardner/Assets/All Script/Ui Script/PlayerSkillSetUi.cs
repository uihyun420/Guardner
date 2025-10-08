using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkillSetUi : GenericWindow
{
    public ScrollRect scrollRect;
    public GameObject playerSkillItemPrefab; // 플레이어 스킬 아이템 UI 프리팹
    public Transform contentParent; // 스크롤렉트의 content

    [SerializeField] private BattleUi battleUi;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private ScreenTouch screenTouch; // ScreenTouch 참조 추가


    private int selectedSkillSlot; // 현재 선택된 스킬 슬롯 (1, 2, 3)
    private int selectedSkillId; // 선택된 스킬 ID
    private bool isStartedBattle = false;


    public void IsBattleState(bool startedBattle)
    {
        isStartedBattle = startedBattle;
    }

    private void Awake()
    {
        isOverlayWindow = true;
    }


    public override void Open()
    {
        base.Open();

        screenTouch.enabled = false; // SetUiBlocking 대신 컴포넌트 자체를 비활성화        

        DisplayAvailableSkills();

        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.enabled = true;
        scrollRect.gameObject.SetActive(true);

        battleUi.blockTouchScreenPanel.SetActive(true);
    }

    public override void Close()
    {
        screenTouch.enabled = true;

        base.Close();
        battleUi.blockTouchScreenPanel.SetActive(false);
    }

    // BattleUi에서 호출될 메서드 - 스킬 슬롯 설정
    public void OpenForSkillSlot(int skillSlot)
    {
        if (isStartedBattle)
        {
            return;
        }

        selectedSkillSlot = skillSlot;
        Open();
    }

    private void DisplayAvailableSkills()
    {
        // 기존 아이템 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        var playerSkillTable = DataTableManager.PlayerSkillTable;

        foreach (var skillData in playerSkillTable.GetAll())
        {
            if (skillData != null)
            {
                CreatePlayerSkillItem(skillData);
            }
        }

        // 스크롤 설정
        if (scrollRect != null)
        {
            var contentHeight = scrollRect.content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            scrollRect.vertical = contentHeight > viewportHeight;
        }
    }

    private void CreatePlayerSkillItem(PlayerSkillData skillData)
    {
        var item = Instantiate(playerSkillItemPrefab, contentParent);
        var itemUi = item.GetComponent<PlayerSkillItemUi>();


        // 이미 할당된 스킬인지 체크
        bool isAlreadyAssigned = battleUi.IsSkillAlreadyAssigned(skillData.Id);
        itemUi.SetData(skillData, OnSelectSkill, isAlreadyAssigned);

    }

    private void OnSelectSkill(int skillId)
    {
        selectedSkillId = skillId;

        // BattleUi에 선택된 스킬 할당
        battleUi.AssignSkillToSlot(selectedSkillSlot, skillId);

        Close();
    }
}