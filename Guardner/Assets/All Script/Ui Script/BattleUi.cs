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
    public MonsterBehavior monsterTarget;
    public GuardnerBehavior guardnerTarget;

    private float battleTimer;
    public TextMeshProUGUI battleTimeText;

    private void Awake()
    {
        battleTimer = 300;
    }

    private void Update()
    {
        SetBattleTimer();
    }

    public void OnSkillButtonClicked(int skillId)
    {
        var skillData = skillManager.guardnerSkillTable.Get(skillId);
        skillManager.SelectSkill(skillId);
        
        if(skillManager.CanUseSkill(skillId))
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
        if(battleTimer <= 0)
        {
            battleTimer = 0f;
            Time.timeScale = 0f; 
        }
        var sb = new StringBuilder();
        sb.Append("Time : ").Append(Mathf.FloorToInt(battleTimer));
        battleTimeText.text = sb.ToString();
    }


    public override void Open()
    {
        base.Open();
    }
}
