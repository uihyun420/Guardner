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

    public GameObject blockTouchScreenPanel;
    public SkillManager skillManager;
    public TextMeshProUGUI battleTimeText;
    public TextMeshProUGUI goldText;

    [SerializeField] private Button readyTextButton;
    [SerializeField] private Button BattleTimerButton;
    [SerializeField] private GameObject door;
    [SerializeField] private SettingUi settingUi;
    [SerializeField] private Button settingUiButton;
    public void ReadyTimeSetZero()
    {
        readyTimer = 0f;
    }
    public void BattleTimerSetZero()
    {
        battleTimer = 0f;
    }

    public void ResetBattleTimer()
    {
        battleTimer = 180f;
        readyTimer = 60f;
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
    public MonsterSpawner monsterSpawner;
    public GuardnerSpawner guardnerSpawner;
    StringBuilder sb = new StringBuilder();

    [SerializeField] private GameObject readyTimeObject;
    [SerializeField] private GameObject battleStartObject;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private PlayerSkillSetUi playerSkillSetUi;

    // 각 스킬 슬롯에 할당된 스킬 ID 저장
    private int assignedSkill1 = -1;
    private int assignedSkill2 = -1;
    private int assignedSkill3 = -1;

    private void Start()
    {
        // 스킬 버튼 이벤트 연결
        if (skill1 != null)
            skill1.onClick.AddListener(() => 
            {
                SoundManager.soundManager.PlaySFX("UiClickSfx");
                OnSkillSlotClicked(1);
            });
        if (skill2 != null)
            skill2.onClick.AddListener(() => 
            {
                SoundManager.soundManager.PlaySFX("UiClickSfx");
                OnSkillSlotClicked(2);
            });
        if (skill3 != null)
            skill3.onClick.AddListener(() => 
            {
                SoundManager.soundManager.PlaySFX("UiClickSfx");
                OnSkillSlotClicked(3);
            });

        BattleTimerButton.onClick.AddListener(() => 
        {
            SoundManager.soundManager.PlaySFX("UiClickSfx");
            BattleTimerSetZero();
        });
        settingUiButton.onClick.AddListener(() => 
        {
            SoundManager.soundManager.PlaySFX("UiClickSfx");
            settingUi.Open();
        });

        StartCoroutine(CoSetReadyTimeUi());
    }

    // 스킬 슬롯 버튼 클릭 시 호출
    private void OnSkillSlotClicked(int slotNumber)
    {
        int assignedSkillId = GetAssignedSkillId(slotNumber);

        if (isReadyTime)
        {
            playerSkillSetUi.OpenForSkillSlot(slotNumber);
        }
        else
        {
            if (assignedSkillId != -1)
            {
                playerSkillManager.UsePlayerSkill(assignedSkillId);
                SetSkillButtonInteractable(slotNumber, false); // 버튼 비활성화
            }
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
        SoundManager.soundManager.PlaySFX("UiClick2Sfx");

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
        if (skillId == -1)
        {
            var buttonImage = skillButton.GetComponent<Image>();
            if (buttonImage != null)
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

                if (sprite != null)
                {
                    buttonImage.sprite = sprite;
                }
            }
        }
    }
    private void Awake()
    {
        battleTimer = 180f;
        readyTimer = 60f;
        gold = 150;
        isReadyTime = true;
        canSpawnGuardnerCount = maxGuardnerCount;
    }

    private void Update()
    {
        if (isReadyTime)
        {
            SetReadyText();
            if (readyTimer <= 0)
            {
                readyTextButton.gameObject.SetActive(false);
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
        SoundManager.soundManager.PlaySFX("UiClick2Sfx");

    }

    public void OnSkillButtonClicked(int skillId)
    {
        var skillData = skillManager.guardnerSkillTable.Get(skillId);
        var sb = new StringBuilder();
        SoundManager.soundManager.PlaySFX("UiClick2Sfx");

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
        if (readyTimer <= 0)
        {
            readyTimer = 0;

        }
        sb.Clear();
        sb.Append(Mathf.FloorToInt(readyTimer)).Append("초");
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
        readyTextButton.gameObject.SetActive(true);
        base.Open();

        var doorBehavior = door.GetComponent<DoorBehavior>();
        if (doorBehavior != null)
        {
            doorBehavior.Init();
        }
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
        gold = 150;
        if (playerSkillManager != null)
        {
            playerSkillManager.lastUsedTime.Clear();
        }
        playerSkillManager.SetBattleState(false);
        playerSkillSetUi.IsBattleState(false);
        StartCoroutine(CoSetReadyTimeUi());
    }
    public override void Close()
    {
        base.Close();
    }
    public void TimeSetZero() // 테스트용
    {
        battleTimer = 0;
    }

    public void SetGuardnerSpawnCount()
    {
        sb.Clear();
        sb.Append("정원사 ").Append(canSpawnGuardnerCount).Append("/").Append(maxGuardnerCount);
        guardnerSpawnCount.text = sb.ToString();
    }
    public void UpdateGuardnerCount()
    {
        canSpawnGuardnerCount--;
        if (canSpawnGuardnerCount < 0)
        {
            canSpawnGuardnerCount = 0;
        }
        SetGuardnerSpawnCount();
    }

    public void UpdateGuardnerPlusCount()
    {
        canSpawnGuardnerCount++;
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
                if (buttonImage != null)
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
        if (SoundManager.soundManager != null)
        {
            SoundManager.soundManager.StopBGM();
            SoundManager.soundManager.PlayBattleBGM("BattleBGM");
        }
        yield return new WaitForSeconds(1.5f);
        readyTimeObject.gameObject.SetActive(false);
    }

    private IEnumerator CoSetBattleStart()
    {
        battleStartObject.gameObject.SetActive(true);

        playerSkillManager.SetBattleState(true);
        playerSkillSetUi.IsBattleState(true);
        yield return new WaitForSeconds(1.5f);
        battleStartObject.gameObject.SetActive(false);
    }


    // 초기화
    public void CompleteReset()
    {
        // 모든 코루틴 정지
        StopAllCoroutines();

        // 타이머 및 상태 초기화
        battleTimer = 180f;
        readyTimer = 60f;
        isReadyTime = true;

        // 골드 초기화
        gold = 150;

        // 정원사 수 초기화
        canSpawnGuardnerCount = maxGuardnerCount;

        // 스킬 슬롯 초기화
        assignedSkill1 = -1;
        assignedSkill2 = -1;
        assignedSkill3 = -1;

        // UI 초기화
        UpdateSkillButtonUI(skill1, -1);
        UpdateSkillButtonUI(skill2, -1);
        UpdateSkillButtonUI(skill3, -1);
        text1.text = "+";
        text2.text = "+";
        text3.text = "+";

        // 스킬 버튼 상태 초기화
        SetSkillButtonInteractable(1, true);
        SetSkillButtonInteractable(2, true);
        SetSkillButtonInteractable(3, true);

        // 스킬 버튼 fillAmount 초기화
        var skill1Image = skill1.GetComponent<Image>();
        var skill2Image = skill2.GetComponent<Image>();
        var skill3Image = skill3.GetComponent<Image>();
        if (skill1Image != null) skill1Image.fillAmount = 1f;
        if (skill2Image != null) skill2Image.fillAmount = 1f;
        if (skill3Image != null) skill3Image.fillAmount = 1f;

        // 쿨타임 텍스트 초기화
        if (coolTimeText1 != null) coolTimeText1.text = "";
        if (coolTimeText2 != null) coolTimeText2.text = "";
        if (coolTimeText3 != null) coolTimeText3.text = "";

        // PlayerSkillManager 초기화
        if (playerSkillManager != null)
        {
            playerSkillManager.lastUsedTime.Clear();
            playerSkillManager.SetBattleState(false);
        }

        // PlayerSkillSetUi 초기화
        if (playerSkillSetUi != null)
        {
            playerSkillSetUi.IsBattleState(false);
        }

        // 몬스터와 정원사 클리어
        if (monsterSpawner != null)
        {
            monsterSpawner.ClearMonster();
        }
        if (guardnerSpawner != null)
        {
            guardnerSpawner.ClearGuardner();
        }

        // 문 초기화
        var doorBehavior = door.GetComponent<DoorBehavior>();
        if (doorBehavior != null)
        {
            doorBehavior.Init();
        }

        // Ready 버튼 활성화
        readyTextButton.gameObject.SetActive(true);

        // 게임 오브젝트 상태 초기화
        if (readyTimeObject != null) readyTimeObject.SetActive(false);
        if (battleStartObject != null) battleStartObject.SetActive(false);

        Debug.Log("BattleUi 완전 초기화 완료");
    }
}
