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

    public void UseSkill(MonsterBehavior monstertarget, GuardnerBehavior guardnertarget)
    {
        //var monster = monstertarget;
        //var guardner = guardnertarget;
        var rb = monstertarget.GetComponent<Rigidbody2D>();

        // KnockBack (Monster)
        if (selectSkill.KnockBack > 0)
        {
            Vector2 direction = Vector2.right; // ���������� �б�
            rb.AddForce(direction * selectSkill.KnockBack, ForceMode2D.Impulse);
            Debug.Log($"KnockBack ����: {selectSkill.KnockBack} (SkillID: {selectSkill.SkillID})");
        }

        // Stun (Monster)
        if (selectSkill.Stun > 0)
        {
            monstertarget.Stun(selectSkill.Stun);
            Debug.Log($"Stun ����: {selectSkill.Stun}�� (SkillID: {selectSkill.SkillID})");
        }

        // GateDamageReflection (Monster)
        if (selectSkill.GateDamageReflection > 0)
        {
            float reflectedDamage = monstertarget.attackPower * selectSkill.GateDamageReflection;
            monstertarget.ReflectDamage(reflectedDamage);
            Debug.Log($"GateDamageReflection ����: {reflectedDamage} (SkillID: {selectSkill.SkillID})");
        }

        // AttackPowerBoost (Guardner)
        if (selectSkill.AttackPowerBoost > 0)
        {
            float attackPowerBoost = guardnertarget.attackPower * selectSkill.AttackPowerBoost;
            float duration = selectSkill.Duration;
            guardnertarget.AttackPowerBoost(attackPowerBoost, duration);
            Debug.Log($"AttackPowerBoost ����: {attackPowerBoost} ({duration}��, SkillID: {selectSkill.SkillID})");
        }

        // AttackSpeedBoost (Guardner)
        if (selectSkill.AttackSpeedBoost > 0)
        {
            float attackSpeedBoost = guardnertarget.aps * selectSkill.AttackSpeedBoost;
            float duration = selectSkill.Duration;
            guardnertarget.AttackSpeedBoost(attackSpeedBoost, duration);
            Debug.Log($"AttackSpeedBoost ����: {attackSpeedBoost} ({duration}��, SkillID: {selectSkill.SkillID})");
        }

        lastUsedTime[selectSkill.SkillID] = Time.time; // ������ ��� �ð� ����
    }

}
