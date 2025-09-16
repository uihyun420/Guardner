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

    public Button spawnPos1;

    public SkillManager skillManager;
    public MonsterBehavior monsterTarget;
    public GuardnerBehavior guardnerTarget;

    private float battleTimer;
    public TextMeshProUGUI battleTimeText;
    public TextMeshProUGUI goldText;

    private int gold;

    StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        battleTimer = 300;
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
        skillManager.SelectSkill(skillId);

        if (skillManager.CanUseSkill(skillId))
        {
            skillManager.UseSkill(monsterTarget, guardnerTarget);
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
            battleTimer = 0f;
            Time.timeScale = 0f;
        }
        sb.Clear();
        sb.Append("Time : ").Append(Mathf.FloorToInt(battleTimer));
        battleTimeText.text = sb.ToString();
    }

    public void SetGoldText()
    {
        sb.Clear();
        sb.Append(gold);
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


    public GuardnerSpawner guardnerSpawner;

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
}
