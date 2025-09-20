using System.Text;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class BattleUi : GenericWindow
{
    public GameObject battleUi;

    public Button skill1;
    public Button skill2;
    public Button skill3;

    //public Button spawnPos1;

    public SkillManager skillManager;
   
    public TextMeshProUGUI battleTimeText;
    public TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI guardnerSpawnCount;
    private int maxGuardnerCount = 16;
    public int canSpawnGuardnerCount = 0;
    private float battleTimer;

    private int gold;
    public MonsterSpawner monsterSpawner; // Inspector���� ����
    public GuardnerSpawner guardnerSpawner;
    StringBuilder sb = new StringBuilder();


    [SerializeField] private StageManager stageManager;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private PlayerSkillSetUi playerSkillSetUi; // Inspector���� ����

    // �� ��ų ���Կ� �Ҵ�� ��ų ID ����
    private int assignedSkill1 = -1;
    private int assignedSkill2 = -1;
    private int assignedSkill3 = -1;

    private void Start()
    {
        // ��ų ��ư �̺�Ʈ ����
        if (skill1 != null)
            skill1.onClick.AddListener(() => OnSkillSlotClicked(1));
        if (skill2 != null)
            skill2.onClick.AddListener(() => OnSkillSlotClicked(2));
        if (skill3 != null)
            skill3.onClick.AddListener(() => OnSkillSlotClicked(3));
    }

    // ��ų ���� ��ư Ŭ�� �� ȣ��
    private void OnSkillSlotClicked(int slotNumber)
    {
        int assignedSkillId = GetAssignedSkillId(slotNumber);

        if (assignedSkillId == -1)
        {
            // ��ų�� �Ҵ���� ���� ��� - PlayerSkillSetUi ����
            playerSkillSetUi.OpenForSkillSlot(slotNumber);
        }
        else
        {
            // ��ų�� �Ҵ�� ��� - ��ų ���
            playerSkillManager.UsePlayerSkill(assignedSkillId);
        }
    }

    // ��ų�� ���Կ� �Ҵ�
    public void AssignSkillToSlot(int slotNumber, int skillId)
    {
        switch (slotNumber)
        {
            case 1:
                assignedSkill1 = skillId;
                UpdateSkillButtonUI(skill1, skillId);
                break;
            case 2:
                assignedSkill2 = skillId;
                UpdateSkillButtonUI(skill2, skillId);
                break;
            case 3:
                assignedSkill3 = skillId;
                UpdateSkillButtonUI(skill3, skillId);
                break;
        }
    }

    // ��ų�� �̹� �Ҵ�Ǿ����� üũ
    public bool IsSkillAlreadyAssigned(int skillId)
    {
        return assignedSkill1 == skillId || assignedSkill2 == skillId || assignedSkill3 == skillId;
    }

    private int GetAssignedSkillId(int slotNumber)
    {
        switch (slotNumber)
        {
            case 1: return assignedSkill1;
            case 2: return assignedSkill2;
            case 3: return assignedSkill3;
            default: return -1;
        }
    }

    private void UpdateSkillButtonUI(Button skillButton, int skillId)
    {
        var skillData = DataTableManager.PlayerSkillTable.Get(skillId);
        if (skillData != null)
        {
            // ��ư �ؽ�Ʈ�� �̹��� ������Ʈ
            var buttonText = skillButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = skillData.Name;
            }

            // ��ų ������ ����
            var buttonImage = skillButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                // buttonImage.sprite = Resources.Load<Sprite>($"SkillIcons/{skillData.GardenerSkillDrawId}");
            }
        }
    }
    private void Awake()
    {
        battleTimer = 60;
        gold = 150;
        canSpawnGuardnerCount = maxGuardnerCount;
    }

    private void Update()
    {
        SetBattleTimer();
        SetGoldText();
        SetGuardnerSpawnCount();
    }

    public void OnPlayerSkillButtonClicked(int skillId)
    {
        playerSkillManager.UsePlayerSkill(skillId);
    }

    public void OnSkillButtonClicked(int skillId)
    {
        var skillData = skillManager.guardnerSkillTable.Get(skillId);

        if (skillManager.CanUseSkill(skillId, skillData.CoolTime))
        {
            foreach (var monster in monsterSpawner.spawnedMonsters)
            {
                if (monster != null)
                    monster.Stun(skillData.Stun);
            }
            skillManager.SelectSkill(skillId);
            skillManager.UseSkill();
            Debug.Log($"���� ��ųID: {skillId}");
        }
        else
        {
            Debug.Log("��Ÿ��");
            // ��ư ��Ȱ��ȭ �� �߰� UI ó��
        }
    }

    public void SetBattleTimer()
    {
        battleTimer -= Time.deltaTime;
        if (battleTimer <= 0)
        {
            stageManager.OnBattleTimerEnd();
            battleTimer = 0;
        }
        sb.Clear();
        sb.Append("�����ð� : ").Append(Mathf.FloorToInt(battleTimer)).Append("��");
        battleTimeText.text = sb.ToString();
    }

    public void SetGoldText()
    {
        sb.Clear();
        sb.Append(gold).Append("���");
        goldText.text = sb.ToString();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"��� ȹ�� : {amount}");
    }
    public override void Open()
    {
        base.Open();
    }


    public void OnGuardnerSpawnButtonClicked()
    {
        int[] guardnerId = { 11135, 11255, 11235 };

        Vector2[] spawnPos = new Vector2[]
        {
             new Vector2(0.5f, 0f),
             new Vector2(1f, 0f),
             new Vector2(0.5f, 0.5f)
        };

        for (int i = 0; i < guardnerId.Length; i++)
        {
            guardnerSpawner.SpawnGuardner(guardnerId[i], spawnPos[i]);
        }
    }
    public void TimeSetZero()
    {
        battleTimer = 0;       
    }
    
    public void SetGuardnerSpawnCount()
    {
        sb.Clear();
        sb.Append("��ġ ������ ������ : ").Append(canSpawnGuardnerCount).Append("/").Append(maxGuardnerCount);
        guardnerSpawnCount.text = sb.ToString();
    }
    public void UpdateGuardnerCount()
    {
        canSpawnGuardnerCount--;
        SetGuardnerSpawnCount();
    }
}
