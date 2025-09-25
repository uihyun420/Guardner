using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUi : GenericWindow
{
    public GameObject battleUi;

    public Button skill1;
    public Button skill2;
    public Button skill3;

    public SkillManager skillManager;   
    public TextMeshProUGUI battleTimeText;    
    public TextMeshProUGUI goldText;

    [SerializeField] private Button readyTextButton;
    [SerializeField] private Button BattleTimerButton;
    public void ReadyTimeSetZero() // 테스트 
    {
        readyTimer = 0f;
    }
    public void BattleTimerSetZero()
    {
        battleTimer = 0f;
    }

    public void ResetBattleTimer()
    {
        battleTimer = 60f;
        readyTimer = 30f; 
        isReadyTime = true; 
    }

    [SerializeField] private TextMeshProUGUI coolTimeText1;
    [SerializeField] private TextMeshProUGUI coolTimeText2;
    [SerializeField] private TextMeshProUGUI coolTimeText3;

    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text3;

    [SerializeField] private TextMeshProUGUI guardnerSpawnCount;

    private int maxGuardnerCount = 16;
    public int canSpawnGuardnerCount = 0;
    public float battleTimer;
    private float readyTimer = 30f;
    private bool isReadyTime = true;

    public int gold;
    public MonsterSpawner monsterSpawner; // Inspector에서 연결
    public GuardnerSpawner guardnerSpawner;
    StringBuilder sb = new StringBuilder();

    [SerializeField] private GameObject readyTimeObject;
    [SerializeField] private GameObject battleStartObject;
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


        BattleTimerButton.onClick.AddListener(BattleTimerSetZero);


        StartCoroutine(CoSetReadyTimeUi());
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
            SetSkillButtonInteractable(slotNumber, false); // 버튼 비활성화
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
                text1.text = string.Empty;
                break;
            case 2:
                assignedSkill2 = skillId;
                UpdateSkillButtonUI(skill2, skillId);
                text2.text = string.Empty;
                break;
            case 3:
                assignedSkill3 = skillId;
                UpdateSkillButtonUI(skill3, skillId);
                text3.text = string.Empty;
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
        if(skillId == -1)
        {
            var buttonImage = skillButton.GetComponent<Image>();
            if(buttonImage != null)
            {
                var defaultSprite = Resources.Load<Sprite>("SkillIcons/Skill_Default");
                buttonImage.sprite = defaultSprite;
            }
            return;
        }

        var skillData = DataTableManager.PlayerSkillTable.Get(skillId);
        if (skillData != null)
        {
            // 스킬 아이콘 설정
            var buttonImage = skillButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                string imagePath = $"SkillIcons/Skill_{skillData.Id}";
                var sprite = Resources.Load<Sprite>(imagePath);

                if(sprite != null)
                {
                    buttonImage.sprite = sprite;
                }
            }
        }
    }
    private void Awake()
    {
        battleTimer = 60f;
        readyTimer = 30f;
        gold = 150;
        isReadyTime = true;
        canSpawnGuardnerCount = maxGuardnerCount;
    }

    private void Update()
    {
        if(isReadyTime)
        {
            SetReadyText();
            if(readyTimer <= 0)
            {
                isReadyTime = false;
                StartCoroutine(CoSetBattleStart());
                stageManager.StartStage();
            }
        }
        else
        {
            SetBattleTimer();
        }
        SetGoldText();
        SetGuardnerSpawnCount();
        SetCoolTimeText();
    }

    public void OnPlayerSkillButtonClicked(int skillId)
    {
        playerSkillManager.UsePlayerSkill(skillId);
    }

    public void OnSkillButtonClicked(int skillId)
    {
        var skillData = skillManager.guardnerSkillTable.Get(skillId);
        var sb = new StringBuilder();

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

    public void SetReadyText()
    {
        readyTimer -= Time.deltaTime;
        if(readyTimer <= 0)
        {
            readyTimer = 0;
        }
        sb.Clear();
        sb.Append("준비 시간 : ").Append(Mathf.FloorToInt(readyTimer)).Append("초");
        battleTimeText.text = sb.ToString(); // 준비시간도 battleTimeText에 표시
    }

    public void SetGoldText()
    {
        sb.Clear();
        sb.Append(gold).Append(" G");
        goldText.text = sb.ToString();
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }
    public override void Open()
    {
        base.Open();
        canSpawnGuardnerCount = maxGuardnerCount;
        assignedSkill1 = -1;
        assignedSkill2 = -1;
        assignedSkill3 = -1;
        UpdateSkillButtonUI(skill1, -1);
        UpdateSkillButtonUI(skill2, -1);
        UpdateSkillButtonUI(skill3, -1);
        text1.text = "+";
        text2.text = "+";
        text3.text = "+";

        if(playerSkillManager != null)
        {
            playerSkillManager.lastUsedTime.Clear();
        }

        StartCoroutine(CoSetReadyTimeUi());
    }

    public void TimeSetZero() // 테스트용
    {
        battleTimer = 0;       
    }
    
    public void SetGuardnerSpawnCount()
    {
        sb.Clear();
        sb.Append("배치 가능한 정원사 ").Append(canSpawnGuardnerCount).Append("/").Append(maxGuardnerCount);
        guardnerSpawnCount.text = sb.ToString();
    }
    public void UpdateGuardnerCount()
    {
        canSpawnGuardnerCount--;
        SetGuardnerSpawnCount();
    }

    public void SetCoolTimeText()
    {
        for (int i = 1; i <= 3; i++)
        {
            int skillId = GetAssignedSkillId(i);
            Image buttonImage = null;
            TextMeshProUGUI targetText = null;
            switch (i)
            {
                case 1:
                    targetText = coolTimeText1;
                    if (skill1 != null) buttonImage = skill1.GetComponent<Image>();
                    break;
                case 2:
                    targetText = coolTimeText2;
                    if (skill2 != null) buttonImage = skill2.GetComponent<Image>();
                    break;
                case 3:
                    targetText = coolTimeText3;
                    if (skill3 != null) buttonImage = skill3.GetComponent<Image>();
                    break;
            }

            StringBuilder sb = new StringBuilder();            
            if (skillId != -1 && playerSkillManager.lastUsedTime.ContainsKey(skillId))
            {
                var skillData = DataTableManager.PlayerSkillTable.Get(skillId);
                float remainSec = playerSkillManager.RemainCoolTime(skillId, skillData.CoolTime);
                if (remainSec > 0)
                {
                    sb.Append(Mathf.CeilToInt(remainSec)).Append("초");
                    // 쿨타임 진행률 계산
                    float fillAmount = 1f - (remainSec / skillData.CoolTime);
                    if (buttonImage != null)
                        buttonImage.fillAmount = fillAmount; // 버튼 이미지 fillAmount로 진행률 표시
                    SetSkillButtonInteractable(i, false); // 쿨타임 중엔 비활성화
                }
                else
                {
                    if (buttonImage != null)
                        buttonImage.fillAmount = 1f; // 쿨타임 끝나면 완전히 채움
                    SetSkillButtonInteractable(i, true); // 쿨타임 끝나면 버튼 활성화
                }
            }
            else // 스킬이 할당되어 있지만 사용한 적이 없거나 쿨타임이 리셋된 경우
            {
                if(buttonImage !=null)
                {
                    buttonImage.fillAmount = 1f;
                }
                SetSkillButtonInteractable(i, true);
            }
            if (targetText != null)
                targetText.text = sb.ToString();
        }
    }

    private void SetSkillButtonInteractable(int slotNumber, bool interactable)
    {
        switch (slotNumber)
        {
            case 1: if (skill1 != null) skill1.interactable = interactable; break;
            case 2: if (skill2 != null) skill2.interactable = interactable; break;
            case 3: if (skill3 != null) skill3.interactable = interactable; break;
        }
    }

    private IEnumerator CoSetReadyTimeUi()
    {
        readyTimeObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        readyTimeObject.gameObject.SetActive(false);
    }

    private IEnumerator CoSetBattleStart()
    {
        battleStartObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        battleStartObject.gameObject.SetActive(false);
    }
}
