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
            Debug.Log("쿨타임이 남아있어 스킬을 사용할 수 없습니다.");
            // 필요하다면 버튼 비활성화 등 추가 UI 처리
        }
    }

    public override void Open()
    {
        base.Open();
    }
}
