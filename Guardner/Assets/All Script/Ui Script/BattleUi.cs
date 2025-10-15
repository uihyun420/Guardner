using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUi : GenericWindow
{
    [Header("Button")]
    public Button skill1;
    public Button skill2;
    public Button skill3;
    [SerializeField] private Button readyTextButton;
    [SerializeField] private Button BattleTimerButton;
    [SerializeField] private Button settingUiButton;

    [Header("GameObject")]
    public GameObject battleUi;
    public GameObject blockTouchScreenPanel;
    [SerializeField] private GameObject readyTimeObject;
    [SerializeField] private GameObject battleStartObject;
    [SerializeField] private GameObject door;

    [Header("Reference")]
    public SkillManager skillManager;
    public MonsterSpawner monsterSpawner;
    public GuardnerSpawner guardnerSpawner;
    [SerializeField] private SettingUi settingUi;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private PlayerSkillSetUi playerSkillSetUi;

    [Header("Text")]
    public TextMeshProUGUI battleTimeText;
    public TextMeshProUGUI goldText;
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
    StringBuilder sb = new StringBuilder();
    private int assignedSkill1 = -1;
    private int assignedSkill2 = -1;
    private int assignedSkill3 = -1;

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



    private void Start()
    {
        skill1.onClick.AddListener(() =>
        {
            SoundManager.soundManager.PlaySFX("UiClickSfx");
            OnSkillSlotClicked(1);
        });
        skill2.onClick.AddListener(() =>
        {
            SoundManager.soundManager.PlaySFX("UiClickSfx");
            OnSkillSlotClicked(2);
        });
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
    }

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
                SetSkillButtonInteractable(slotNumber, false);
            }
        }
    }

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
        var buttonImage = skillButton.GetComponent<Image>();

        if (skillId == -1)
        {
            var defaultSprite = Resources.Load<Sprite>("SkillIcons/Skill_Default");
            buttonImage.sprite = defaultSprite;
            return;
        }

        var skillData = DataTableManager.PlayerSkillTable.Get(skillId);
        string imagePath = $"SkillIcons/Skill_{skillData.Id}";
        var sprite = Resources.Load<Sprite>(imagePath);
        buttonImage.sprite = sprite;
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
        SoundManager.soundManager.PlaySFX("UiClick2Sfx");

        if (skillManager.CanUseSkill(skillId, skillData.CoolTime))
        {
            foreach (var monster in monsterSpawner.spawnedMonsters)
            {
                monster.Stun(skillData.Stun);
            }
            skillManager.SelectSkill(skillId);
            skillManager.UseSkill();
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
        battleTimeText.text = sb.ToString();
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
        StartCoroutine(CoSetReadyTimeUi());
        readyTextButton.gameObject.SetActive(true);
        base.Open();

        var doorBehavior = door.GetComponent<DoorBehavior>();
        doorBehavior.Init();

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
        playerSkillManager.lastUsedTime.Clear();
        playerSkillManager.SetBattleState(false);
        playerSkillSetUi.IsBattleState(false);
    }

    public override void Close()
    {
        base.Close();
        SoundManager.soundManager.ResumeToPreviousBGM();
    }

    public void TimeSetZero()
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
                    buttonImage = skill1.GetComponent<Image>();
                    break;
                case 2:
                    targetText = coolTimeText2;
                    buttonImage = skill2.GetComponent<Image>();
                    break;
                case 3:
                    targetText = coolTimeText3;
                    buttonImage = skill3.GetComponent<Image>();
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
                    float fillAmount = 1f - (remainSec / skillData.CoolTime);
                    buttonImage.fillAmount = fillAmount;
                    SetSkillButtonInteractable(i, false);
                }
                else
                {
                    buttonImage.fillAmount = 1f;
                    SetSkillButtonInteractable(i, true);
                }
            }
            else
            {
                buttonImage.fillAmount = 1f;
                SetSkillButtonInteractable(i, true);
            }
            targetText.text = sb.ToString();
        }
    }

    private void SetSkillButtonInteractable(int slotNumber, bool interactable)
    {
        switch (slotNumber)
        {
            case 1: skill1.interactable = interactable; break;
            case 2: skill2.interactable = interactable; break;
            case 3: skill3.interactable = interactable; break;
        }
    }

    private IEnumerator CoSetReadyTimeUi()
    {
        readyTimeObject.gameObject.SetActive(true);
        SoundManager.soundManager.PlayBattleBGM("BattleBGM");
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

    public void CompleteReset()
    {
        StopAllCoroutines();

        battleTimer = 180f;
        readyTimer = 60f;
        isReadyTime = true;
        gold = 150;
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

        SetSkillButtonInteractable(1, true);
        SetSkillButtonInteractable(2, true);
        SetSkillButtonInteractable(3, true);

        skill1.GetComponent<Image>().fillAmount = 1f;
        skill2.GetComponent<Image>().fillAmount = 1f;
        skill3.GetComponent<Image>().fillAmount = 1f;

        coolTimeText1.text = "";
        coolTimeText2.text = "";
        coolTimeText3.text = "";

        playerSkillManager.lastUsedTime.Clear();
        playerSkillManager.SetBattleState(false);
        playerSkillSetUi.IsBattleState(false);

        monsterSpawner.ClearMonster();
        guardnerSpawner.ClearGuardner();

        var doorBehavior = door.GetComponent<DoorBehavior>();
        doorBehavior.Init();

        readyTextButton.gameObject.SetActive(true);
        readyTimeObject.SetActive(false);
        battleStartObject.SetActive(false);
    }
}