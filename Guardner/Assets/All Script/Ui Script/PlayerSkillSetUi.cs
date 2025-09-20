using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillSetUi : GenericWindow
{
    public ScrollRect scrollRect;
    public GameObject playerSkillItemPrefab; // 플레이어 스킬 아이템 UI 프리팹
    public Transform contentParent; // 스크롤렉트의 content

    [SerializeField] private BattleUi battleUi;
    [SerializeField] private PlayerSkillManager playerSkillManager;

    private int selectedSkillSlot; // 현재 선택된 스킬 슬롯 (1, 2, 3)
    private int selectedSkillId; // 선택된 스킬 ID

    public override void Open()
    {
        base.Open();

        // 배경 터치 막기
        if (battleUi != null && battleUi.guardnerSpawner != null && battleUi.guardnerSpawner.screenTouch != null)
            battleUi.guardnerSpawner.screenTouch.SetUiBlocking(true);

        if (scrollRect != null)
            scrollRect.horizontal = false;

        DisplayAvailableSkills();
    }

    public override void Close()
    {
        base.Close();

        // 배경 터치 다시 허용
        if (battleUi != null && battleUi.guardnerSpawner != null && battleUi.guardnerSpawner.screenTouch != null)
            battleUi.guardnerSpawner.screenTouch.SetUiBlocking(false);
    }

    // BattleUi에서 호출될 메서드 - 스킬 슬롯 설정
    public void OpenForSkillSlot(int skillSlot)
    {
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

        if (itemUi != null)
        {
            // 이미 할당된 스킬인지 체크
            bool isAlreadyAssigned = battleUi.IsSkillAlreadyAssigned(skillData.Id);
            itemUi.SetData(skillData, OnSelectSkill, isAlreadyAssigned);
        }
    }

    private void OnSelectSkill(int skillId)
    {
        selectedSkillId = skillId;

        // BattleUi에 선택된 스킬 할당
        battleUi.AssignSkillToSlot(selectedSkillSlot, skillId);

        Close();
    }
}