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
            Debug.Log("��ų ��Ÿ���Դϴ�.");
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
                // �ٸ� ��ų �߰�
        }
    }

    private void PullingWeedsEffect(PlayerSkillData skillData)
    {
        if (monsterSpawner == null || monsterSpawner.spawnedMonsters == null || monsterSpawner.spawnedMonsters.Count == 0)
        {
            Debug.Log("������ ���Ͱ� �����ϴ�.");
            return;
        }

        var aliveMonsters = monsterSpawner.spawnedMonsters.Where(m => m != null && !m.IsDead).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("����ִ� ���Ͱ� �����ϴ�.");
            return;
        }

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
                float reductionPercent = skillData.HPReduction > 0 ? skillData.HPReduction : 0.5f;
                int damage = Mathf.RoundToInt(monster.monsterData.HP * reductionPercent);

                monster.Ondamage(damage);
                Debug.Log($"���� �̱�: {monster.monsterData.Name}���� {damage} ������ (HP {reductionPercent * 100}% ����)");
            }
        }
        Debug.Log($"���� �̱� ��ų ���: {randomMonsters.Count}���� ���� Ÿ��");
    }
}
