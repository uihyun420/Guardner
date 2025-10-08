using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public enum SkillEffectType
{
    Stun,
    DeBuff,
    Buff,
    Damage
}

public class SkillManager : MonoBehaviour
{
    public GuardnerSkillTable guardnerSkillTable => DataTableManager.GuardnerSkillTable;
    protected GuardnerSkillData selectSkill;

    public Dictionary<int, float> lastUsedTime = new Dictionary<int, float>(); // 스킬 ID별 마지막 사용 시간 저장 

    [SerializeField] protected GuardnerSpawner guardnerSpawner;
    [SerializeField] protected MonsterSpawner monsterSpawner;


    [SerializeField] private GameObject guardnerStunEffectPrefab;
    [SerializeField] private GameObject guardnerBuffEffectPrefab;
    [SerializeField] private GameObject guardnerDeBuffEffectPrefab;
    [SerializeField] private GameObject guardnerDamageEffectPrefab;



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


    protected virtual void ApplySkillToMonsters()
    {
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters.Count == 0)
            return;

        if (guardnerSpawner == null || guardnerSpawner.spawnedGuardners.Count == 0)
            return;

        // 스킬을 사용하는 가드너 찾기
        GuardnerBehavior skillUser = null;
        foreach (var guardner in guardnerSpawner.spawnedGuardners)
        {
            if (guardner != null && guardner.skillId == selectSkill.SkillID)
            {
                skillUser = guardner;
                break;
            }
        }

        if (skillUser == null) return;

        // 해당 가드너의 범위 내 몬스터에게만 스킬 적용
        foreach (var monster in monsterSpawner.spawnedMonsters)
        {
            if (monster != null && !monster.IsDead)
            {
                float distance = Vector2.Distance(skillUser.transform.position, monster.transform.position);

                // 가드너의 공격 범위 내에 있는 몬스터에게만 적용
                if (distance <= skillUser.attackRange)
                {
                    ApplyMonsterSkills(monster);
                }
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
                PlaySkillEffect(guardner.transform.position, SkillEffectType.Buff);
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

        // spawnedMonsters 리스트 직접 순회 - 성능 최적화
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
            PlaySkillEffect(targetMonster.transform.position, SkillEffectType.Stun);
        }

        // GateDamageReflection
        if (selectSkill.GateDamageReflection > 0)
        {
            float reflectedDamage = targetMonster.attackPower * selectSkill.GateDamageReflection;
            targetMonster.ReflectDamage(reflectedDamage);
            PlaySkillEffect(targetMonster.transform.position, SkillEffectType.Damage);
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
            PlaySkillEffect(guardner.transform.position, SkillEffectType.Buff);

        }

        // AttackSpeedBoost
        if (selectSkill.AttackSpeedBoost > 0)
        {
            float attackSpeedBoost = guardner.aps * selectSkill.AttackSpeedBoost;
            float duration = selectSkill.Duration;
            guardner.AttackSpeedBoost(attackSpeedBoost, duration);
            PlaySkillEffect(guardner.transform.position, SkillEffectType.Buff);
        }
    }

    public void PlaySkillEffect(Vector3 position, SkillEffectType effectType)
    {
        GameObject effectPrefab = null;

        switch (effectType)
        {
            case SkillEffectType.Stun:
                effectPrefab = guardnerStunEffectPrefab;
                break;
            case SkillEffectType.Damage:
                effectPrefab = guardnerDamageEffectPrefab;
                break;
            case SkillEffectType.Buff:
                effectPrefab = guardnerBuffEffectPrefab;
                break;
            case SkillEffectType.DeBuff:
                effectPrefab = guardnerDeBuffEffectPrefab;
                break;
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);

            // 파티클 시스템이 있다면 자동으로 삭제되도록 설정
            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                var main = particles.main;
                Destroy(effect, main.duration + main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effect, 0.5f);
            }
        }
    }

    // 몬스터가 데미지를 받을 때 호출할 메서드
    public void PlayDamageEffect(Vector3 position)
    {
        PlaySkillEffect(position, SkillEffectType.Damage);
    }
}


