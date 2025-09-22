using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillManager : SkillManager
{
    public PlayerSkillTable playerSkillTable => DataTableManager.PlayerSkillTable; 

    public void UsePlayerSkill(int skillId)
    {
        var skillData = playerSkillTable.Get(skillId);
        if (skillData == null) return; 

        if (!CanUseSkill(skillId, skillData.CoolTime))
        {
            return;
        }
        ApplyPlayerSkillEffect(skillData);

        lastUsedTime[skillId] = Time.time;
    }

    private void ApplyPlayerSkillEffect(PlayerSkillData skillData)
    {
        switch (skillData.Id)
        {
            case 51140: // ���� �̱�
                PullingWeedsEffect(skillData);
                Debug.Log($"��뽺ų : {skillData.Name}, ��ų ���̵� {skillData.Id}, ��ų ȿ�� : {skillData.SkillDescription}");
                break;
            case 52830:
                MorningWateringEffect(skillData);
                Debug.Log($"��뽺ų : {skillData.Name}, ��ų ���̵� {skillData.Id}, ��ų ȿ�� : {skillData.SkillDescription}");
                break;
            case 521035:
                PestRepellentEffect(skillData);
                Debug.Log($"��뽺ų : {skillData.Name}, ��ų ���̵� {skillData.Id}, ��ų ȿ�� : {skillData.SkillDescription}");
                break;
            case 52650:
                PruningEffect(skillData);
                Debug.Log($"��뽺ų : {skillData.Name}, ��ų ���̵� {skillData.Id}, ��ų ȿ�� : {skillData.SkillDescription}");
                break;
            case 521260:
                SunlightControlEffect(skillData);
                Debug.Log($"��뽺ų : {skillData.Name}, ��ų ���̵� {skillData.Id}, ��ų ȿ�� : {skillData.SkillDescription}");
                break;
            case 52170:
                SoilLevelingEffect(skillData);
                Debug.Log($"��뽺ų : {skillData.Name}, ��ų ���̵� {skillData.Id}, ��ų ȿ�� : {skillData.SkillDescription}");
                break;
            case 521590:
                FragranceBloomEffect(skillData);
                Debug.Log($"��뽺ų : {skillData.Name}, ��ų ���̵� {skillData.Id}, ��ų ȿ�� : {skillData.SkillDescription}");
                break;



                // �ٸ� ��ų �߰�
        }
    }


    private List<MonsterBehavior> GetAliveMonsters()
    {
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters == null || monsterSpawner.spawnedMonsters.Count == 0)
        {
            Debug.Log("������ ���Ͱ� �����ϴ�.");
            return new List<MonsterBehavior>();
        }

        var aliveMonsters = monsterSpawner.spawnedMonsters.Where(m => m != null && !m.IsDead).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("����ִ� ���Ͱ� �����ϴ�.");
        }
        return aliveMonsters;
    }

    private List<GuardnerBehavior> GetAliveGuardner()
    {
        if (guardnerSpawner == null || guardnerSpawner.spawnedGuardners == null || guardnerSpawner.spawnedGuardners.Count == 0)
        {
            Debug.Log("������ �����簡 �����ϴ�.");
            return new List<GuardnerBehavior>();
        }
        var aliveGuardner = guardnerSpawner.spawnedGuardners.Where(g => g != null).ToList();
        if(aliveGuardner.Count == 0)
        {
            Debug.Log("��ȯ�� �����簡 �����ϴ�.");
        }
        return aliveGuardner;
    }


    private void PullingWeedsEffect(PlayerSkillData skillData)
    {
        var aliveMonsters = GetAliveMonsters();
        if (aliveMonsters.Count == 0) return;

        int targetCount = Mathf.Min(5, aliveMonsters.Count);
        var randomMonsters = new List<MonsterBehavior>();

        for (int i = 0; i < targetCount; i++)
        {
            int randomIndex = Random.Range(0, aliveMonsters.Count);
            randomMonsters.Add(aliveMonsters[randomIndex]);
            aliveMonsters.RemoveAt(randomIndex); // �ߺ�����
        }

        foreach (var monster in randomMonsters)
        {
            if (monster != null && !monster.IsDead)
            {
                float reductionPercent = Mathf.Abs(skillData.HPReduction); // ���� ���
                int damage = Mathf.RoundToInt(monster.monsterData.HP * reductionPercent);

                monster.Ondamage(damage);
                Debug.Log($"���� �̱�: {monster.monsterData.Name}���� {damage} ������ (HP {reductionPercent * 100}% ����)");
            }
        }
    }


    private void MorningWateringEffect(PlayerSkillData skillData)
    {
        var aliveGuardner = GetAliveGuardner();
        if(aliveGuardner.Count == 0) return;

        float boostPercent = 0.2f;
        float duration = skillData.Duration;

        foreach(var guardner in aliveGuardner)
        {
            if(guardner != null)
            {
                float boostAmount = guardner.aps * boostPercent;
                guardner.AttackSpeedBoost(boostAmount, duration);
                Debug.Log($"�Ʊ� ����� {guardner.name} ���ݼӵ� {boostPercent * 100}% ���� ({duration}��)");
            }
        }
    }
    
    private void PestRepellentEffect(PlayerSkillData skillData) // ������ġ��
    {
        var aliveMonster = GetAliveMonsters();
        if (aliveMonster.Count == 0) return;

        float reducePercent = 0.15f;
        float duration = skillData.Duration;    
        
        foreach(var monster in aliveMonster)
        {
            if (monster != null && !monster.IsDead)
            {
                monster.StopCoroutine("CoSpeedDebuff");
                monster.StartCoroutine(monster.CoSpeedDebuff(reducePercent, reducePercent, duration));
            }
        }
    }

    private void PruningEffect(PlayerSkillData skillData)
    {
        var aliveMonster = GetAliveMonsters();        

        foreach(var monster in aliveMonster)
        {
            if(monster != null && !monster.IsDead)
            {
                monster.Stun(skillData.Bind);
                float monsterHpReducePercent = Mathf.Abs(skillData.HPReduction);
                int damage = Mathf.RoundToInt(monster.monsterData.HP * monsterHpReducePercent);
                monster.Ondamage(damage);
            }
        }
    }

    private void SunlightControlEffect(PlayerSkillData skillData)
    {
        var aliveGuardner = GetAliveGuardner();
        if (aliveGuardner.Count == 0) return;

        float attackSpeedBoost = skillData.AttackSpeed;
        float criticalValue = skillData.CriticalChance; // �ϳ��� ������ Ȯ���� ������ ��� ó�� �����Behavior�� Attack ���� int�� ��ȯ�ɶ� �ݿø� �ϸ� 0���� ���ع���
        float duration = skillData.Duration;

        foreach (var guardner in aliveGuardner)
        {
            if(guardner != null)
            {
                float apsBoostAmount = guardner.aps * attackSpeedBoost;
                guardner.AttackSpeedBoost(apsBoostAmount, duration);

                // ũ��Ƽ�� ���� ���� - Ȯ���� ������ ��� ���� ��
                guardner.hasCriticalBuff = true;
                guardner.buffCriticalChance = criticalValue;  // Ȯ��
                guardner.buffCriticalDamage = criticalValue;  // �������� ���� ��

                StartCoroutine(RemoveCriticalBuff(guardner, duration));
            }
        }
    }
    private IEnumerator RemoveCriticalBuff(GuardnerBehavior guardner, float duration)
    {
        yield return new WaitForSeconds(duration);
        guardner.hasCriticalBuff = false;
        guardner.buffCriticalChance = 0f;
        guardner.buffCriticalDamage = 0f;
    }

    private void SoilLevelingEffect(PlayerSkillData skillData)
    {
        var aliveMonster = GetAliveMonsters();        
        foreach (var monster in aliveMonster)
        {
            if (monster != null && !monster.IsDead)
            {
                float reductionPercent = Mathf.Abs(skillData.HPReduction); // ���� ���
                int damage = Mathf.RoundToInt(monster.monsterData.HP * reductionPercent);
                monster.Stun(skillData.Stun);
                monster.Ondamage(damage);
            }
        }
    }

    private void FragranceBloomEffect(PlayerSkillData skillData)
    {
        var aliveGuardner = GetAliveGuardner();
        float duration = skillData.Duration;
        float attackPowerPercent = skillData.AttackPowerBoost;
        float coolTimeReducePercent = skillData.SkillCoolTimeReduction;


        foreach (var guardner in aliveGuardner)
        {
            int attackPowerAmount = Mathf.RoundToInt(guardner.attackPower * attackPowerPercent);
            guardner.AttackPowerBoost(attackPowerAmount, duration);

            int skillId = guardner.guardnerData.SkillID;
            GuardnerSkillData guardnerSkillData = guardnerSkillTable.Get(skillId);
            if (guardnerSkillData != null)
            {
                // ���� ��Ÿ�� ����
                float originalCoolTime = guardnerSkillData.CoolTime;
                float reducedCoolTime = originalCoolTime * (1f - coolTimeReducePercent);

                guardnerSkillData.CoolTime = reducedCoolTime;


                float coolTimeDiff = originalCoolTime - reducedCoolTime; // ����� �׽�Ʈ��
                Debug.Log($"����� {guardner.name}�� ��ų ��Ÿ�� {originalCoolTime:F2}�� �� {reducedCoolTime:F2}�� ({coolTimeDiff:F2}�� ����)");


                // ���� �ð� �� ���� ��Ÿ������ ����
                StartCoroutine(RestoreGuardnerSkillCoolTime(guardnerSkillData, originalCoolTime, duration));
            }
        }
    }
    private IEnumerator RestoreGuardnerSkillCoolTime(GuardnerSkillData skillData, float originalCoolTime, float duration)
    {
        yield return new WaitForSeconds(duration);
        skillData.CoolTime = originalCoolTime;
    }





    public float RemainCoolTime(int skillId, float coolTime) // ���� ��Ÿ�� ���
    {
        if(!lastUsedTime.ContainsKey(skillId))
        {
            return 0f;  
        }
        float lastTime = lastUsedTime[skillId];
        float remain = coolTime - (Time.time - lastTime);
        return Mathf.Max(0f, remain);
    }











}
