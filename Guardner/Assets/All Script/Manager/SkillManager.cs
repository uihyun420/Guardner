using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public enum SkillEffectType
{
    Stun,
    Damage,
    Buff
}




public class SkillManager : MonoBehaviour
{
    public GuardnerSkillTable guardnerSkillTable => DataTableManager.GuardnerSkillTable;
    protected GuardnerSkillData selectSkill;

    public Dictionary<int, float> lastUsedTime = new Dictionary<int, float>(); // ��ų ID�� ������ ��� �ð� ���� 

    [SerializeField] protected GuardnerSpawner guardnerSpawner;
    [SerializeField] protected MonsterSpawner monsterSpawner;


    [SerializeField] private GameObject stunEffectPrefab;
    //[SerializeField] private GameObject reflectionEffectPrefab;
    [SerializeField] private GameObject damageEffectPrefab;




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

            // ������� SkillID�� ��ġ�ϴ� ��ų�� ����
            var skillData = guardnerSkillTable.Get(guardner.skillId);
            if (skillData != null && CanUseSkill(skillData.SkillID, skillData.CoolTime))
            {
                SelectSkill(skillData.SkillID);
                UseSkill();
               // Debug.Log($"����� {guardner.name}�� ��ų {skillData.Name} ���");
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
        // ���� ��� ��ų
        if (selectSkill.TargetType == SkillTargetType.Monster)
        {
            ApplySkillToMonsters();
        }
        // ����� ��� ��ų
        else if (selectSkill.TargetType == SkillTargetType.Guardner)
        {
            ApplySkillToAllGuardners();
        }

        lastUsedTime[selectSkill.SkillID] = Time.time;
    }

    // ���� ��� ��ų ����
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

    // ����� ��� ��ų ����
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
        // �������� spawnedMonsters ����Ʈ ���� ���
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters.Count == 0)
            return null;

        // ���� ������� Ÿ�����ϰ� �ִ� ���� �켱 Ȯ��
        if (guardner != null && guardner.Monster != null && !guardner.Monster.IsDead)
        {
            return guardner.Monster;
        }

        // ����� ���� ���� ���� ���� ã��
        MonsterBehavior nearestMonster = null;
        float nearestDistance = float.MaxValue;

        // spawnedMonsters ����Ʈ ���� ��ȸ - ���� ����ȭ
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
            Debug.Log($"Stun ����: {selectSkill.Stun}�� (SkillID: {selectSkill.SkillID})");
        }

        // GateDamageReflection
        if (selectSkill.GateDamageReflection > 0)
        {
            float reflectedDamage = targetMonster.attackPower * selectSkill.GateDamageReflection;
            targetMonster.ReflectDamage(reflectedDamage);
            Debug.Log($"GateDamageReflection ����: {reflectedDamage} (SkillID: {selectSkill.SkillID})");
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
            //Debug.Log($"AttackSpeedBoost ����: {attackSpeedBoost} ({duration}��, SkillID: {selectSkill.SkillID})");
        }
    }

    public void PlaySkillEffect(Vector3 position, SkillEffectType effectType)
    {
        GameObject effectPrefab = null;

        switch (effectType)
        {
            case SkillEffectType.Stun:
                effectPrefab = stunEffectPrefab;
                break;
            case SkillEffectType.Damage:
                effectPrefab = damageEffectPrefab;
                break;
            case SkillEffectType.Buff:
                // ���� ����Ʈ�� ����𿡰� ����ǹǷ� ���⼭�� ó������ ����
                return;
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);

            // ��ƼŬ �ý����� �ִٸ� �ڵ����� �����ǵ��� ����
            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                var main = particles.main;
                Destroy(effect, main.duration + main.startLifetime.constantMax);
            }
            else
            {
                // ��ƼŬ�� ���ٸ� 2�� �� ����
                Destroy(effect, 2f);
            }
        }
    }

    // ���Ͱ� �������� ���� �� ȣ���� �޼���
    public void PlayDamageEffect(Vector3 position)
    {
        PlaySkillEffect(position, SkillEffectType.Damage);
    }
}


