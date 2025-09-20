using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    public GuardnerSkillTable guardnerSkillTable => DataTableManager.GuardnerSkillTable;
    protected GuardnerSkillData selectSkill;

    protected Dictionary<int, float> lastUsedTime = new Dictionary<int, float>(); // 스킬 ID별 마지막 사용 시간 저장 

    [SerializeField] protected GuardnerSpawner guardnerSpawner;
    [SerializeField] protected MonsterSpawner monsterSpawner;

    public void Init(GuardnerSkillData data)
    {
        selectSkill = data;
    }

    private void Update()
    {
        if (guardnerSpawner == null || guardnerSpawner.spawnedGuardners.Count == 0)
            return;
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters.Count == 0)
            return;

        foreach (var guardner in guardnerSpawner.spawnedGuardners)
        {
            if (guardner == null)
                continue;

            // 가드너의 SkillID와 일치하는 스킬만 적용
            var skillData = guardnerSkillTable.Get(guardner.skillId);
            if (skillData != null && CanUseSkill(skillData.SkillID, skillData.CoolTime))
            {
                SelectSkill(skillData.SkillID);
                UseSkill();
                Debug.Log($"가드너 {guardner.name}가 스킬 {skillData.Name} 사용");
            }
        }
    }

    public virtual void SelectSkill(int skillId)
    {
        selectSkill = guardnerSkillTable.Get(skillId);
    }

    public virtual bool CanUseSkill(int skillId, float coolTime)
    {
        if (!lastUsedTime.ContainsKey(skillId))
        {
            return true;
        }
        float lastTime = lastUsedTime[skillId];
        return Time.time - lastTime >= coolTime;
    }

    public virtual void UseSkill()
    {
        // 몬스터 대상 스킬
        if (selectSkill.TargetType == SkillTargetType.Monster)
        {
            ApplySkillToMonsters();
        }
        // 가디언 대상 스킬
        else if (selectSkill.TargetType == SkillTargetType.Guardner)
        {
            ApplySkillToAllGuardners();
        }

        lastUsedTime[selectSkill.SkillID] = Time.time;
    }

    // 몬스터 대상 스킬 적용
    protected virtual void ApplySkillToMonsters()
    {
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters.Count == 0)
            return;

        foreach (var monster in monsterSpawner.spawnedMonsters)
        {
            if (monster != null && !monster.IsDead)
            {
                ApplyMonsterSkills(monster);
            }
        }
    }

    // 가디언 대상 스킬 적용
    protected virtual void ApplySkillToAllGuardners()
    {
        if (guardnerSpawner == null || guardnerSpawner.spawnedGuardners.Count == 0)
            return;

        foreach (var guardner in guardnerSpawner.spawnedGuardners)
        {
            if (guardner != null)
            {
                ApplyGuardnerSkills(guardner);
            }
        }
    }

    protected virtual MonsterBehavior FindTargetMonster(GuardnerBehavior guardner)
    {
        // 스포너의 spawnedMonsters 리스트 직접 사용
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters.Count == 0)
            return null;

        // 현재 가디언이 타겟팅하고 있는 몬스터 우선 확인
        if (guardner != null && guardner.Monster != null && !guardner.Monster.IsDead)
        {
            return guardner.Monster;
        }

        // 가디언 공격 범위 내의 몬스터 찾기
        MonsterBehavior nearestMonster = null;
        float nearestDistance = float.MaxValue;

        // spawnedMonsters 리스트 직접 순회 - 성능 최적화!
        foreach (var monster in monsterSpawner.spawnedMonsters)
        {
            if (monster != null && !monster.IsDead)
            {
                float distance = Vector2.Distance(transform.position, monster.transform.position);

                if (guardner != null && distance <= guardner.attackRange && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestMonster = monster;
                }
            }
        }

        return nearestMonster;
    }

    protected virtual void ApplyMonsterSkills(MonsterBehavior targetMonster)
    {
        if (targetMonster == null) return;

        // Stun
        if (selectSkill.Stun > 0)
        {
            targetMonster.Stun(selectSkill.Stun);
            Debug.Log($"Stun 적용: {selectSkill.Stun}초 (SkillID: {selectSkill.SkillID})");
        }

        // GateDamageReflection
        if (selectSkill.GateDamageReflection > 0)
        {
            float reflectedDamage = targetMonster.attackPower * selectSkill.GateDamageReflection;
            targetMonster.ReflectDamage(reflectedDamage);
            Debug.Log($"GateDamageReflection 적용: {reflectedDamage} (SkillID: {selectSkill.SkillID})");
        }
    }

    protected virtual void ApplyGuardnerSkills(GuardnerBehavior guardner)
    {
        if (guardner == null) return;

        // AttackPowerBoost
        if (selectSkill.AttackPowerBoost > 0)
        {
            float attackPowerBoost = guardner.attackPower * selectSkill.AttackPowerBoost;
            float duration = selectSkill.Duration;
            guardner.AttackPowerBoost(attackPowerBoost, duration);
            Debug.Log($"AttackPowerBoost 적용: {attackPowerBoost} ({duration}초, SkillID: {selectSkill.SkillID})");
        }

        // AttackSpeedBoost
        if (selectSkill.AttackSpeedBoost > 0)
        {
            float attackSpeedBoost = guardner.aps * selectSkill.AttackSpeedBoost;
            float duration = selectSkill.Duration;
            guardner.AttackSpeedBoost(attackSpeedBoost, duration);
            Debug.Log($"AttackSpeedBoost 적용: {attackSpeedBoost} ({duration}초, SkillID: {selectSkill.SkillID})");
        }
    }
}