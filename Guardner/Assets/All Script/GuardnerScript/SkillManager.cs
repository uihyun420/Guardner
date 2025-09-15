using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    //public GuardnerSkillTable guardnerSkillTable = new GuardnerSkillTable();
    public GuardnerSkillTable guardnerSkillTable => DataTableManager.GuardnerSkillTable;
    private GuardnerSkillData selectSkill;

    private Dictionary<int, float> lastUsedTime = new Dictionary<int, float>(); // ��ų ID�� ������ ��� �ð� ���� 

    public void Init(GuardnerSkillData data)
    {
        selectSkill = data;
    }

    public void SelectSkill(int skillId)
    {
        selectSkill = guardnerSkillTable.Get(skillId);
    }

    public bool CanUseSkill(int skillId)
    {
        if (!lastUsedTime.ContainsKey(skillId))
        {
            return true;
        } 
        float lastTime = lastUsedTime[skillId];
        float coolTime = selectSkill.CoolTime;
        return Time.time - lastTime >= coolTime;
    }

    public void UseSkill(MonsterBehavior monstertarget, GuardnerBehavior guardnertarget )
    {
        var monster = monstertarget.GetComponent<MonsterBehavior>();
        var guardner = guardnertarget.GetComponent<GuardnerBehavior>();
        var rb = monstertarget.GetComponent<Rigidbody2D>();

        switch (selectSkill.TargetType)
        {
            case SkillTargetType.Monster:
                if (selectSkill.KnockBack > 0)
                {
                    Vector2 direction = (monstertarget.transform.position - transform.position).normalized;
                    rb.AddForce(direction * selectSkill.KnockBack, ForceMode2D.Impulse);
                    Debug.Log($"[Skill] KnockBack ����: {selectSkill.KnockBack} (SkillID: {selectSkill.SkillID})");
                }
                if (selectSkill.Stun > 0)
                {
                    monster.Stun(selectSkill.Stun);
                    Debug.Log($"[Skill] Stun ����: {selectSkill.Stun}�� (SkillID: {selectSkill.SkillID})");
                }
                if (selectSkill.GateDamageReflection > 0)
                {
                    float reflectedDamage = monster.attackPower * selectSkill.GateDamageReflection;
                    monster.ReflectDamage(reflectedDamage);
                    Debug.Log($"[Skill] GateDamageReflection ����: {reflectedDamage} (SkillID: {selectSkill.SkillID})");
                }
                break;
            case SkillTargetType.Guardner:
                if (selectSkill.AttackPowerBoost > 0)
                {
                    float attackPowerBoost = guardner.attackPower * selectSkill.AttackPowerBoost;
                    float duration = selectSkill.Duration;
                    guardner.AttackPowerBoost(attackPowerBoost, duration);
                    Debug.Log($"[Skill] AttackPowerBoost ����: {attackPowerBoost} ({duration}��, SkillID: {selectSkill.SkillID})");

                }
                if (selectSkill.AttackSpeedBoost > 0)
                {
                    float attackSpeedBoost = guardner.aps * selectSkill.AttackSpeedBoost;
                    float duration = selectSkill.Duration;
                    guardner.AttackSpeedBoost(attackSpeedBoost, duration);
                    Debug.Log($"[Skill] AttackSpeedBoost ����: {attackSpeedBoost} ({duration}��, SkillID: {selectSkill.SkillID})");

                }
                break;
        }

        //if (selectSkill.DebuffClean > 0)
        //{

        //}
        //if (selectSkill.DebuffCleanCoolTime > 0)
        //{

        //}

        lastUsedTime[selectSkill.SkillID] = Time.time; // ������ ��� �ð� ����
    }

}
