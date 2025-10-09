using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public enum PlayerSkillEffectType
{
    PullingWeeds,      // 잡초뽑기
    MorningWatering,   // 아침물주기
    PestRepellent,     // 해충퇴치제
    Pruning,           // 가지치기
    SunlightControl,   // 햇빛조절
    SoilLeveling,      // 토양정리
    FragranceBloom,    // 향기블룸
    PestExtermination, // 해충박멸
    SolarFocus,        // 태양광집중
    GardenFestival     // 정원축제
}

public class PlayerSkillManager : SkillManager
{
    public PlayerSkillTable playerSkillTable => DataTableManager.PlayerSkillTable;

    [SerializeField] private TextMeshProUGUI buffText;
    [SerializeField] private TextMeshProUGUI deBuffText;

    private StringBuilder sb = new StringBuilder();
    private List<SkillEffect> activeSkillEffects = new List<SkillEffect>();
    private bool isBattleStarted = false;

    [SerializeField] private GameObject pullingWeedsEffectPrefab;      // 잡초뽑기 이펙트
    [SerializeField] private GameObject morningWateringEffectPrefab;   // 아침물주기 이펙트
    [SerializeField] private GameObject pestRepellentEffectPrefab;     // 해충퇴치제 이펙트
    [SerializeField] private GameObject pruningEffectPrefab;           // 가지치기 이펙트
    [SerializeField] private GameObject sunlightControlEffectPrefab;   // 햇빛조절 이펙트
    [SerializeField] private GameObject soilLevelingEffectPrefab;      // 토양정리 이펙트
    [SerializeField] private GameObject fragranceBloomEffectPrefab;    // 향기블룸 이펙트
    [SerializeField] private GameObject pestExterminationEffectPrefab; // 해충박멸 이펙트
    [SerializeField] private GameObject solarFocusEffectPrefab;        // 태양광집중 이펙트
    [SerializeField] private GameObject gardenFestivalEffectPrefab;    // 정원축제 이펙트

    public void SetBattleState(bool isStarted)
    {
        isBattleStarted = isStarted;
    }

    // 스킬 효과 정보를 저장하는 클래스
    public class SkillEffect
    {
        public string effectText;
        public float remainingTime;
        public bool isBuff; // true면 버프, false면 디버프

        public SkillEffect(string effect, float duration, bool buff)
        {
            effectText = effect;
            remainingTime = duration;
            isBuff = buff;
        }
    }

    private void UpdateActiveEffects()
    {
        // 남은 시간 감소 및 만료된 효과 제거
        for (int i = activeSkillEffects.Count - 1; i >= 0; i--)
        {
            activeSkillEffects[i].remainingTime -= Time.deltaTime;
            if (activeSkillEffects[i].remainingTime <= 0)
            {
                activeSkillEffects.RemoveAt(i);
            }
        }
    }
    private void UpdateEffectTexts()
    {
        // 버프 텍스트 업데이트
        if (buffText != null)
        {
            sb.Clear();
            var buffs = activeSkillEffects.Where(e => e.isBuff).ToList();
            if (buffs.Count > 0)
            {
                foreach (var buff in buffs)
                {
                    sb.AppendLine($"{buff.effectText} ({Mathf.CeilToInt(buff.remainingTime)}초)");
                }
            }
            buffText.text = sb.ToString();
        }

        // 디버프 텍스트 업데이트
        if (deBuffText != null)
        {
            sb.Clear();
            var debuffs = activeSkillEffects.Where(e => !e.isBuff).ToList();
            if (debuffs.Count > 0)
            {
                foreach (var debuff in debuffs)
                {
                    sb.AppendLine($"{debuff.effectText} ({Mathf.CeilToInt(debuff.remainingTime)}초)");
                }
            }
            deBuffText.text = sb.ToString();
        }
    }

    private void AddSkillEffect(string effectText, float duration, bool isBuff)
    {
        // 같은 스킬이 이미 활성화되어 있으면 시간 갱신
        var existingEffect = activeSkillEffects.FirstOrDefault(e => e.effectText == effectText);
        if (existingEffect != null)
        {
            existingEffect.remainingTime = duration;
        }
        else
        {
            activeSkillEffects.Add(new SkillEffect(effectText, duration, isBuff));
        }
    }

    private void Update()
    {
        UpdateActiveEffects();
        UpdateEffectTexts();
    }

    public void UsePlayerSkill(int skillId)
    {
        if (!isBattleStarted) return;

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
                SoundManager.soundManager.PlaySFX("WeedRemovalSfx");
                break;
            case 52830: // 아침 물주기
                MorningWateringEffect(skillData);
                SoundManager.soundManager.PlaySFX("MorningWateringSfx");
                AddSkillEffect(skillData.SkillDescription, skillData.Duration, true); // 버프 효과
                break;
            case 521035: // 해충 퇴치제
                PestRepellentEffect(skillData);
                SoundManager.soundManager.PlaySFX("WeedRemovalSfx");
                AddSkillEffect(skillData.SkillDescription, skillData.Duration, false); // 디버프 효과
                break;
            case 52650: // 가지치기
                PruningEffect(skillData);
                SoundManager.soundManager.PlaySFX("WeedRemovalSfx");
                break;
            case 521260: // 햇빛 조절
                SunlightControlEffect(skillData);
                SoundManager.soundManager.PlaySFX("MorningWateringSfx");
                AddSkillEffect(skillData.SkillDescription, skillData.Duration, true); // 버프 효과
                break;
            case 52170: // 땅고르기
                SoilLevelingEffect(skillData);
                SoundManager.soundManager.PlaySFX("WeedRemovalSfx");
                break;
            case 521590: // 향기 블룸
                FragranceBloomEffect(skillData);
                SoundManager.soundManager.PlaySFX("MorningWateringSfx");
                AddSkillEffect(skillData.SkillDescription, skillData.Duration, true); // 버프 효과
                break;
            case 5215100: // 해충 박멸
                PestExtermination(skillData);
                SoundManager.soundManager.PlaySFX("WeedRemovalSfx");
                AddSkillEffect(skillData.SkillDescription, skillData.Duration, true); // 버프 효과
                break ;
            case 521120: // 태양광 집중
                SolarFocus(skillData);
                SoundManager.soundManager.PlaySFX("WeedRemovalSfx");
                AddSkillEffect(skillData.SkillDescription, skillData.Duration, true); // 버프 효과
                break;
            case 5220150: // 태양광 집중
                GardenFestival(skillData);
                SoundManager.soundManager.PlaySFX("MorningWateringSfx");
                AddSkillEffect(skillData.SkillDescription, skillData.Duration, true); // 버프 효과
                break;
        }
    }

    public void PlayPlayerSkillEffect(Vector3 position, PlayerSkillEffectType effectType)
    {
        GameObject effectPrefab = null;

        switch (effectType)
        {
            case PlayerSkillEffectType.PullingWeeds:
                effectPrefab = pullingWeedsEffectPrefab;
                break;
            case PlayerSkillEffectType.MorningWatering:
                effectPrefab = morningWateringEffectPrefab;
                break;
            case PlayerSkillEffectType.PestRepellent:
                effectPrefab = pestRepellentEffectPrefab;
                break;
            case PlayerSkillEffectType.Pruning:
                effectPrefab = pruningEffectPrefab;
                break;
            case PlayerSkillEffectType.SunlightControl:
                effectPrefab = sunlightControlEffectPrefab;
                break;
            case PlayerSkillEffectType.SoilLeveling:
                effectPrefab = soilLevelingEffectPrefab;
                break;
            case PlayerSkillEffectType.FragranceBloom:
                effectPrefab = fragranceBloomEffectPrefab;
                break;
            case PlayerSkillEffectType.PestExtermination:
                effectPrefab = pestExterminationEffectPrefab;
                break;
            case PlayerSkillEffectType.SolarFocus:
                effectPrefab = solarFocusEffectPrefab;
                break;
            case PlayerSkillEffectType.GardenFestival:
                effectPrefab = gardenFestivalEffectPrefab;
                break;
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);

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


    private List<MonsterBehavior> GetAliveMonsters()
    {
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters == null || monsterSpawner.spawnedMonsters.Count == 0)
        {
            return new List<MonsterBehavior>();
        }
        var aliveMonsters = monsterSpawner.spawnedMonsters.Where(m => m != null && !m.IsDead).ToList();
        return aliveMonsters;
    }

    private List<GuardnerBehavior> GetAliveGuardner()
    {
        if (guardnerSpawner == null || guardnerSpawner.spawnedGuardners == null || guardnerSpawner.spawnedGuardners.Count == 0)
        {
            return new List<GuardnerBehavior>();
        }
        var aliveGuardner = guardnerSpawner.spawnedGuardners.Where(g => g != null).ToList();
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
                PlayPlayerSkillEffect(monster.transform.position, PlayerSkillEffectType.PullingWeeds);
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
                PlayPlayerSkillEffect(guardner.transform.position, PlayerSkillEffectType.MorningWatering);

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
                PlayPlayerSkillEffect(monster.transform.position, PlayerSkillEffectType.PestRepellent);
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
                PlayPlayerSkillEffect(monster.transform.position, PlayerSkillEffectType.Pruning);
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
                PlayPlayerSkillEffect(guardner.transform.position, PlayerSkillEffectType.SunlightControl);

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
                PlayPlayerSkillEffect(monster.transform.position, PlayerSkillEffectType.SoilLeveling);
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

                // 일정 시간 후 원래 쿨타임으로 복구
                StartCoroutine(RestoreGuardnerSkillCoolTime(guardnerSkillData, originalCoolTime, duration));
                PlayPlayerSkillEffect(guardner.transform.position, PlayerSkillEffectType.FragranceBloom);

            }
        }
    }


    private void PestExtermination(PlayerSkillData skillData)
    {
        var aliveMonsters = GetAliveMonsters();
        if (aliveMonsters.Count == 0) return;

        float reducePercent = 0.25f; // 25% 감소
        float duration = skillData.Duration;

        foreach (var monster in aliveMonsters)
        {
            if (monster != null && !monster.IsDead)
            {
                monster.StopCoroutine("CoSpeedDebuff");
                monster.StartCoroutine(monster.CoSpeedDebuff(reducePercent, reducePercent, duration));
                PlayPlayerSkillEffect(monster.transform.position, PlayerSkillEffectType.PestExtermination);
            }
        }
        AddSkillEffect(skillData.SkillDescription, duration, false); // 디버프 효과 텍스트


    }

    private void SolarFocus(PlayerSkillData skillData)
    {
        var aliveMonsters = GetAliveMonsters();
        if (aliveMonsters.Count == 0) return;

        foreach (var monster in aliveMonsters)
        {
            if (monster != null && !monster.IsDead)
            {
                int currentHp = monster.monsterData.HP; // 몬스터의 최대 HP
                monster.Ondamage(currentHp);
                PlayPlayerSkillEffect(monster.transform.position, PlayerSkillEffectType.SolarFocus);
            }
        }
        AddSkillEffect(skillData.SkillDescription, 0.5f, false);
    }

    private void GardenFestival(PlayerSkillData skillData)
    {
        var aliveMonsters = GetAliveMonsters();
        if (aliveMonsters.Count > 0)
        {
            foreach (var monster in aliveMonsters)
            {
                if (monster != null && !monster.IsDead)
                {
                    int damage = Mathf.RoundToInt(monster.monsterData.HP * 0.8f);
                    monster.Ondamage(damage);
                    PlayPlayerSkillEffect(monster.transform.position, PlayerSkillEffectType.PullingWeeds);

                }
            }
            AddSkillEffect("몬스터 HP -80%", 0.5f, false); // 디버프 효과 텍스트
        }

        var aliveGuardners = GetAliveGuardner();
        float duration = skillData.Duration;
        float attackPowerPercent = 0.3f; 
        if (aliveGuardners.Count > 0)
        {
            foreach (var guardner in aliveGuardners)
            {
                int attackPowerAmount = Mathf.RoundToInt(guardner.attackPower * attackPowerPercent);
                guardner.AttackPowerBoost(attackPowerAmount, duration);
                PlayPlayerSkillEffect(guardner.transform.position, PlayerSkillEffectType.GardenFestival);

            }
            AddSkillEffect("정원사 전체 공격력 +30%", duration, true); 
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
