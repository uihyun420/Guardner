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
    public MonsterSpawner monsterSpawner; // Inspector���� ����
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
}
