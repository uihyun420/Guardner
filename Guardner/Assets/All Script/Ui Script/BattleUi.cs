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
            Debug.Log($"���� ��ųID: {skillId}");
        }
        else
        {
            Debug.Log("��Ÿ���� �����־� ��ų�� ����� �� �����ϴ�.");
            // �ʿ��ϴٸ� ��ư ��Ȱ��ȭ �� �߰� UI ó��
        }
    }

    public override void Open()
    {
        base.Open();
    }
}
