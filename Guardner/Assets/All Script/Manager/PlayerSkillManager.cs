using System.Collections.Generic;
using System.Linq;
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
            Debug.Log("스킬 쿨타임입니다.");
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
                // 다른 스킬 추가
        }
    }

    private void PullingWeedsEffect(PlayerSkillData skillData)
    {
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters == null || monsterSpawner.spawnedMonsters.Count == 0)
        {
            Debug.Log("적용할 몬스터가 없습니다.");
            return;
        }

        var aliveMonsters = monsterSpawner.spawnedMonsters.Where(m => m != null && !m.IsDead).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("살아있는 몬스터가 없습니다.");
            return;
        }

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
                float reductionPercent = skillData.HPReduction > 0 ? skillData.HPReduction : 0.5f;
                int damage = Mathf.RoundToInt(monster.monsterData.HP * reductionPercent);

                monster.Ondamage(damage);
                Debug.Log($"잡초 뽑기: {monster.monsterData.Name}에게 {damage} 데미지 (HP {reductionPercent * 100}% 감소)");
            }
        }
        Debug.Log($"잡초 뽑기 스킬 사용: {randomMonsters.Count}마리 몬스터 타겟");
    }
}
