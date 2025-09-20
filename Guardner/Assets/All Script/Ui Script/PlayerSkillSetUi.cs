using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillSetUi : GenericWindow
{
    public ScrollRect scrollRect;
    public GameObject playerSkillItemPrefab; // �÷��̾� ��ų ������ UI ������
    public Transform contentParent; // ��ũ�ѷ�Ʈ�� content

    [SerializeField] private BattleUi battleUi;
    [SerializeField] private PlayerSkillManager playerSkillManager;

    private int selectedSkillSlot; // ���� ���õ� ��ų ���� (1, 2, 3)
    private int selectedSkillId; // ���õ� ��ų ID

    public override void Open()
    {
        base.Open();

        // ��� ��ġ ����
        if (battleUi != null && battleUi.guardnerSpawner != null && battleUi.guardnerSpawner.screenTouch != null)
            battleUi.guardnerSpawner.screenTouch.SetUiBlocking(true);

        if (scrollRect != null)
            scrollRect.horizontal = false;

        DisplayAvailableSkills();
    }

    public override void Close()
    {
        base.Close();

        // ��� ��ġ �ٽ� ���
        if (battleUi != null && battleUi.guardnerSpawner != null && battleUi.guardnerSpawner.screenTouch != null)
            battleUi.guardnerSpawner.screenTouch.SetUiBlocking(false);
    }

    // BattleUi���� ȣ��� �޼��� - ��ų ���� ����
    public void OpenForSkillSlot(int skillSlot)
    {
        selectedSkillSlot = skillSlot;
        Open();
    }

    private void DisplayAvailableSkills()
    {
        // ���� ������ ����
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

        // ��ũ�� ����
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
            // �̹� �Ҵ�� ��ų���� üũ
            bool isAlreadyAssigned = battleUi.IsSkillAlreadyAssigned(skillData.Id);
            itemUi.SetData(skillData, OnSelectSkill, isAlreadyAssigned);
        }
    }

    private void OnSelectSkill(int skillId)
    {
        selectedSkillId = skillId;

        // BattleUi�� ���õ� ��ų �Ҵ�
        battleUi.AssignSkillToSlot(selectedSkillSlot, skillId);

        Close();
    }
}