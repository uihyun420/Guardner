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
        // 현재 최신 버전이므로 자기 자신 반환
        return this;
    }

    // 가드너 강화 정보 추가/업데이트 메서드
    public void SetGuardnerEnhance(string guardnerId, GuardnerSaveData enhanceData)
    {
        GuardnerEnhances[guardnerId] = enhanceData;
    }

    // 가드너 강화 정보 조회 메서드
    public GuardnerSaveData GetGuardnerEnhance(string guardnerId)
    {
        if (GuardnerEnhances.ContainsKey(guardnerId))
        {
            return GuardnerEnhances[guardnerId];
        }
        return null;
    }

    // 가드너 언락 메서드
    public void UnlockGuardner(string guardnerId)
    {
        if (!UnlockedGuardners.Contains(guardnerId))
        {
            UnlockedGuardners.Add(guardnerId);
        }
    }

    // 가드너 언락 여부 확인 메서드
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