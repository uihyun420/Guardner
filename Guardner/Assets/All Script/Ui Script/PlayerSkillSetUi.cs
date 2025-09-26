using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkillSetUi : GenericWindow
{
    public ScrollRect scrollRect;
    public GameObject playerSkillItemPrefab; // �÷��̾� ��ų ������ UI ������
    public Transform contentParent; // ��ũ�ѷ�Ʈ�� content

    [SerializeField] private BattleUi battleUi;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private ScreenTouch screenTouch; // ScreenTouch ���� �߰�


    private int selectedSkillSlot; // ���� ���õ� ��ų ���� (1, 2, 3)
    private int selectedSkillId; // ���õ� ��ų ID
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

        // ScreenTouch�� ������ ��Ȱ��ȭ
        if (screenTouch != null)
        {
            screenTouch.enabled = false; // SetUiBlocking ��� ������Ʈ ��ü�� ��Ȱ��ȭ
        }

        DisplayAvailableSkills();

        // ScrollRect ������ DisplayAvailableSkills �Ŀ� ����
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.enabled = true;
            scrollRect.gameObject.SetActive(true);
        }

        // ���� �� ScrollRect �ʱ�ȭ
        StartCoroutine(InitializeScrollRect());
    }

    public override void Close()
    {
        // ScreenTouch �ٽ� Ȱ��ȭ
        if (screenTouch != null)
        {
            screenTouch.enabled = true;
        }
        base.Close();
    }

    private IEnumerator InitializeScrollRect()
    {
        // 2������ ��� (UI�� ������ ������ ������)
        yield return null;
        yield return null;

        if (scrollRect != null)
        {
            // Canvas ���� ������Ʈ
            Canvas.ForceUpdateCanvases();

            // ScrollRect �缳��
            scrollRect.enabled = false;
            yield return null;
            scrollRect.enabled = true;

            // ��ũ�� ��ġ�� �� ���� �ʱ�ȭ
            scrollRect.verticalNormalizedPosition = 1f;

            // EventSystem Ŭ���� �� �ٽ� ����
            EventSystem.current.SetSelectedGameObject(null);
            yield return null;
            EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);
        }
    }

    // BattleUi���� ȣ��� �޼��� - ��ų ���� ����
    public void OpenForSkillSlot(int skillSlot)
    {
        if(isStartedBattle)
        {
            return;
        }

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