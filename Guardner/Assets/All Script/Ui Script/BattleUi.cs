using System.Text;
using TMPro;
using Unity.VisualScripting;
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
    private float battleTimer;

    private int gold;
    public MonsterSpawner monsterSpawner; // Inspector에서 연결
    public GuardnerSpawner guardnerSpawner;
    StringBuilder sb = new StringBuilder();


    [SerializeField] private StageManager stageManager;


    private void Awake()
    {
        battleTimer = 30;
        gold = 0;
    }

    private void Update()
    {
        SetBattleTimer();
        SetGoldText();
    }

    public void OnSkillButtonClicked(int skillId)
    {
        var skillData = skillManager.guardnerSkillTable.Get(skillId);

        if (skillManager.CanUseSkill(skillId))
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
}
