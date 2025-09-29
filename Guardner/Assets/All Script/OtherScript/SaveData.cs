using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SaveData
{
    public int Version { get; protected set; }
    public int CurrentStage { get; set; }
    public bool IsStageCleared { get; set; }
    public abstract SaveData VersionUp();
}

[Serializable]
public class SaveDataV1 : SaveData
{
    public string PlayerName { get; set; } = string.Empty;
    public int Gold { get; set; } = 0;
    public int StageProgress { get; set; } = 1;
    public Dictionary<string, GuardnerSaveData> GuardnerEnhances { get; set; } = new Dictionary<string, GuardnerSaveData>();
    public List<string> UnlockedGuardners { get; set; } = new List<string>();

    public SaveDataV1()
    {
        Version = 1;
    }

    public override SaveData VersionUp()
    {
        // ���� �ֽ� �����̹Ƿ� �ڱ� �ڽ� ��ȯ
        return this;
    }

    // ����� ��ȭ ���� �߰�/������Ʈ �޼���
    public void SetGuardnerEnhance(string guardnerId, GuardnerSaveData enhanceData)
    {
        GuardnerEnhances[guardnerId] = enhanceData;
    }

    // ����� ��ȭ ���� ��ȸ �޼���
    public GuardnerSaveData GetGuardnerEnhance(string guardnerId)
    {
        if (GuardnerEnhances.ContainsKey(guardnerId))
        {
            return GuardnerEnhances[guardnerId];
        }
        return null;
    }

    // ����� ��� �޼���
    public void UnlockGuardner(string guardnerId)
    {
        if (!UnlockedGuardners.Contains(guardnerId))
        {
            UnlockedGuardners.Add(guardnerId);
        }
    }

    // ����� ��� ���� Ȯ�� �޼���
    public bool IsGuardnerUnlocked(string guardnerId)
    {
        return UnlockedGuardners.Contains(guardnerId);
    }
}

[Serializable]
public class GuardnerSaveData
{
    public int Level { get; set; } = 1;
    public int AttackPower { get; set; } = 0;
    public int Health { get; set; } = 0;
    public float AttackSpeed { get; set; } = 1.0f;
    public float MovementSpeed { get; set; } = 1.0f;
    public List<string> Abilities { get; set; } = new List<string>();

    public GuardnerSaveData() { }

    public GuardnerSaveData(int level, int attack, int health, float attackSpeed, float moveSpeed)
    {
        Level = level;
        AttackPower = attack;
        Health = health;
        AttackSpeed = attackSpeed;
        MovementSpeed = moveSpeed;
    }
}