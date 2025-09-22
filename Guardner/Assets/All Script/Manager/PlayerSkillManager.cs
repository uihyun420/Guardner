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
            case 51140: // 잡초 뽑기
                PullingWeedsEffect(skillData);
                Debug.Log($"사용스킬 : {skillData.Name}, 스킬 아이디 {skillData.Id}, 스킬 효과 : {skillData.SkillDescription}");
                break;
            case 52830:
                MorningWateringEffect(skillData);
                Debug.Log($"사용스킬 : {skillData.Name}, 스킬 아이디 {skillData.Id}, 스킬 효과 : {skillData.SkillDescription}");
                break;
            case 521035:
                PestRepellentEffect(skillData);
                Debug.Log($"사용스킬 : {skillData.Name}, 스킬 아이디 {skillData.Id}, 스킬 효과 : {skillData.SkillDescription}");
                break;
            case 52650:
                PruningEffect(skillData);
                Debug.Log($"사용스킬 : {skillData.Name}, 스킬 아이디 {skillData.Id}, 스킬 효과 : {skillData.SkillDescription}");
                break;
            case 521260:
                SunlightControlEffect(skillData);
                Debug.Log($"사용스킬 : {skillData.Name}, 스킬 아이디 {skillData.Id}, 스킬 효과 : {skillData.SkillDescription}");
                break;
            case 52170:
                SoilLevelingEffect(skillData);
                Debug.Log($"사용스킬 : {skillData.Name}, 스킬 아이디 {skillData.Id}, 스킬 효과 : {skillData.SkillDescription}");
                break;
            case 521590:
                FragranceBloomEffect(skillData);
                Debug.Log($"사용스킬 : {skillData.Name}, 스킬 아이디 {skillData.Id}, 스킬 효과 : {skillData.SkillDescription}");
                break;



                // 다른 스킬 추가
        }
    }


    private List<MonsterBehavior> GetAliveMonsters()
    {
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters == null || monsterSpawner.spawnedMonsters.Count == 0)
        {
            Debug.Log("적용할 몬스터가 없습니다.");
            return new List<MonsterBehavior>();
        }

        var aliveMonsters = monsterSpawner.spawnedMonsters.Where(m => m != null && !m.IsDead).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("살아있는 몬스터가 없습니다.");
        }
        return aliveMonsters;
    }

    private List<GuardnerBehavior> GetAliveGuardner()
    {
        if (guardnerSpawner == null || guardnerSpawner.spawnedGuardners == null || guardnerSpawner.spawnedGuardners.Count == 0)
        {
            Debug.Log("적용할 정원사가 없습니다.");
            return new List<GuardnerBehavior>();
        }
        var aliveGuardner = guardnerSpawner.spawnedGuardners.Where(g => g != null).ToList();
        if(aliveGuardner.Count == 0)
        {
            Debug.Log("소환된 정원사가 없습니다.");
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
            aliveMonsters.RemoveAt(randomIndex); // 중복방지
        }

        foreach (var monster in randomMonsters)
        {
            if (monster != null && !monster.IsDead)
            {
                float reductionPercent = Mathf.Abs(skillData.HPReduction); // 절댓값 사용
                int damage = Mathf.RoundToInt(monster.monsterData.HP * reductionPercent);

                monster.Ondamage(damage);
                Debug.Log($"잡초 뽑기: {monster.monsterData.Name}에게 {damage} 데미지 (HP {reductionPercent * 100}% 감소)");
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
                Debug.Log($"아군 가드너 {guardner.name} 공격속도 {boostPercent * 100}% 증가 ({duration}초)");
            }
        }
    }
    
    private void PestRepellentEffect(PlayerSkillData skillData) // 해충퇴치제
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
        float criticalValue = skillData.CriticalChance; // 하나의 값으로 확률과 데미지 모두 처리 가드너Behavior의 Attack 에서 int로 변환될때 반올림 하면 0으로 변해버림
        float duration = skillData.Duration;

        foreach (var guardner in aliveGuardner)
        {
            if(guardner != null)
            {
                float apsBoostAmount = guardner.aps * attackSpeedBoost;
                guardner.AttackSpeedBoost(apsBoostAmount, duration);

                // 크리티컬 버프 적용 - 확률과 데미지 모두 같은 값
                guardner.hasCriticalBuff = true;
                guardner.buffCriticalChance = criticalValue;  // 확률
                guardner.buffCriticalDamage = criticalValue;  // 데미지도 같은 값

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
                float reductionPercent = Mathf.Abs(skillData.HPReduction); // 절댓값 사용
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
                // 기존 쿨타임 저장
                float originalCoolTime = guardnerSkillData.CoolTime;
                float reducedCoolTime = originalCoolTime * (1f - coolTimeReducePercent);

                guardnerSkillData.CoolTime = reducedCoolTime;


                float coolTimeDiff = originalCoolTime - reducedCoolTime; // 디버그 테스트용
                Debug.Log($"가드너 {guardner.name}의 스킬 쿨타임 {originalCoolTime:F2}초 → {reducedCoolTime:F2}초 ({coolTimeDiff:F2}초 감소)");


                // 일정 시간 후 원래 쿨타임으로 복구
                StartCoroutine(RestoreGuardnerSkillCoolTime(guardnerSkillData, originalCoolTime, duration));
            }
        }
    }
    private IEnumerator RestoreGuardnerSkillCoolTime(GuardnerSkillData skillData, float originalCoolTime, float duration)
    {
        yield return new WaitForSeconds(duration);
        skillData.CoolTime = originalCoolTime;
    }





    public float RemainCoolTime(int skillId, float coolTime) // 남은 쿨타임 계산
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
