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
    public MonsterSpawner monsterSpawner; // Inspector에서 연결
    public GuardnerSpawner guardnerSpawner;
    StringBuilder sb = new StringBuilder();


    [SerializeField] private StageManager stageManager;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private PlayerSkillSetUi playerSkillSetUi; // Inspector에서 연결

    // 각 스킬 슬롯에 할당된 스킬 ID 저장
    private int assignedSkill1 = -1;
    private int assignedSkill2 = -1;
    private int assignedSkill3 = -1;

    private void Start()
    {
        // 스킬 버튼 이벤트 연결
        if (skill1 != null)
            skill1.onClick.AddListener(() => OnSkillSlotClicked(1));
        if (skill2 != null)
            skill2.onClick.AddListener(() => OnSkillSlotClicked(2));
        if (skill3 != null)
            skill3.onClick.AddListener(() => OnSkillSlotClicked(3));
    }

    // 스킬 슬롯 버튼 클릭 시 호출
    private void OnSkillSlotClicked(int slotNumber)
    {
        int assignedSkillId = GetAssignedSkillId(slotNumber);

        if (assignedSkillId == -1)
        {
            // 스킬이 할당되지 않은 경우 - PlayerSkillSetUi 열기
            playerSkillSetUi.OpenForSkillSlot(slotNumber);
        }
        else
        {
            // 스킬이 할당된 경우 - 스킬 사용
            playerSkillManager.UsePlayerSkill(assignedSkillId);
        }
    }

    // 스킬을 슬롯에 할당
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

    // 스킬이 이미 할당되었는지 체크
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
            // 버튼 텍스트나 이미지 업데이트
            var buttonText = skillButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = skillData.Name;
            }

            // 스킬 아이콘 설정
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
            Debug.Log($"사용된 스킬ID: {skillId}");
        }
        else
        {
            Debug.Log("쿨타임");
            // 버튼 비활성화 등 추가 UI 처리
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
        sb.Append("남은시간 : ").Append(Mathf.FloorToInt(battleTimer)).Append("초");
        battleTimeText.text = sb.ToString();
    }

    public void SetGoldText()
    {
        sb.Clear();
        sb.Append(gold).Append("골드");
        goldText.text = sb.ToString();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"골드 획득 : {amount}");
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
        sb.Append("배치 가능한 정원사 : ").Append(canSpawnGuardnerCount).Append("/").Append(maxGuardnerCount);
        guardnerSpawnCount.text = sb.ToString();
    }
    public void UpdateGuardnerCount()
    {
        canSpawnGuardnerCount--;
        SetGuardnerSpawnCount();
    }
}
