using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SkillManager : MonoBehaviour
{
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

    public void UseSkill()
    {
        var guardner = GetComponent<GuardnerBehavior>();
        var monster = GetComponent<MonsterBehavior>();

        // KnockBack (Monster)
        if (selectSkill.KnockBack > 0 && monster != null)
        {
            var rb = monster.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = Vector2.right;
                rb.AddForce(direction * selectSkill.KnockBack, ForceMode2D.Impulse);
                Debug.Log($"KnockBack ����: {selectSkill.KnockBack} (SkillID: {selectSkill.SkillID})");
            }
        }

        // Stun (Monster)
        if (selectSkill.Stun > 0 && monster != null)
        {
            monster.Stun(selectSkill.Stun);
            Debug.Log($"Stun ����: {selectSkill.Stun}�� (SkillID: {selectSkill.SkillID})");
        }

        // GateDamageReflection (Monster)
        if (selectSkill.GateDamageReflection > 0 && monster != null)
        {
            float reflectedDamage = monster.attackPower * selectSkill.GateDamageReflection;
            monster.ReflectDamage(reflectedDamage);
            Debug.Log($"GateDamageReflection ����: {reflectedDamage} (SkillID: {selectSkill.SkillID})");
        }

        // AttackPowerBoost (Guardner)
        if (selectSkill.AttackPowerBoost > 0 && guardner != null)
        {
            float attackPowerBoost = guardner.attackPower * selectSkill.AttackPowerBoost;
            float duration = selectSkill.Duration;
            guardner.AttackPowerBoost(attackPowerBoost, duration);
            Debug.Log($"AttackPowerBoost ����: {attackPowerBoost} ({duration}��, SkillID: {selectSkill.SkillID})");
        }

        // AttackSpeedBoost (Guardner)
        if (selectSkill.AttackSpeedBoost > 0 && guardner != null)
        {
            float attackSpeedBoost = guardner.aps * selectSkill.AttackSpeedBoost;
            float duration = selectSkill.Duration;
            guardner.AttackSpeedBoost(attackSpeedBoost, duration);
            Debug.Log($"AttackSpeedBoost ����: {attackSpeedBoost} ({duration}��, SkillID: {selectSkill.SkillID})");
        }

        lastUsedTime[selectSkill.SkillID] = Time.time;
    }

}
