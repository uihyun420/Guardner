using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableManager()
    {
        Init();
    }

    private static void Init()
    {
        var guardnerTable = new GuardnerTable();
        guardnerTable.Load(DataTableIds.Guardner);
        tables.Add(DataTableIds.Guardner, guardnerTable);

        var monsterTable = new MonsterTable();
        monsterTable.Load(DataTableIds.Monster);
        tables.Add(DataTableIds.Monster, monsterTable);

        var guardnerSkillTable = new GuardnerSkillTable();
        guardnerSkillTable.Load(DataTableIds.GuardnerSkill);
        tables.Add(DataTableIds.GuardnerSkill, guardnerSkillTable);

        var playerSkillTable = new PlayerSkillTable();
        playerSkillTable.Load(DataTableIds.PlayerSkill);
        tables.Add(DataTableIds.PlayerSkill, playerSkillTable);

        var stageTable = new StageTable();
        stageTable.Load(DataTableIds.Stage);
        tables.Add(DataTableIds.Stage, stageTable);

        var stageRewardTable = new StageRewardTable();
        stageRewardTable.Load(DataTableIds.StageReward);
        tables.Add(DataTableIds.StageReward, stageRewardTable);

        var guardnerEnhanceTable = new GuardnerEnhanceTable();
        guardnerEnhanceTable.Load(DataTableIds.GuardnerEnhance);
        tables.Add(DataTableIds.GuardnerEnhance, guardnerEnhanceTable);

        var dictionarySetTable = new DictionarySetTable();
        dictionarySetTable.Load(DataTableIds.DictionarySet);
        tables.Add(DataTableIds.DictionarySet, dictionarySetTable);
    }
    public static GuardnerEnhanceTable GuardnerEnhanceTable
    {
        get
        {
            return Get<GuardnerEnhanceTable>(DataTableIds.GuardnerEnhance);
        }
    }

    public static GuardnerTable GuardnerTable
    {
        get
        {
            return Get<GuardnerTable>(DataTableIds.Guardner);
        }
    }

    public static MonsterTable MonsterTable
    {
        get
        {
            return Get<MonsterTable>(DataTableIds.Monster);
        }
    }
    public static GuardnerSkillTable GuardnerSkillTable
    {
        get
        {
            return Get<GuardnerSkillTable>(DataTableIds.GuardnerSkill);
        }
    }

    public static PlayerSkillTable PlayerSkillTable
    {
        get
        {
            return Get<PlayerSkillTable>(DataTableIds.PlayerSkill);
        }
    }
    public static StageTable StageTable
    {
        get
        {
            return Get<StageTable>(DataTableIds.Stage);
        }
    }
    public static StageRewardTable StageRewardTable
    {
        get
        {
            return Get<StageRewardTable>(DataTableIds.StageReward);
        }
    }

    public static DictionarySetTable DictionarySetTable
    {
        get
        {
            return Get<DictionarySetTable>(DataTableIds.DictionarySet);
        }
    }


    public static T Get<T>(string id) where T : DataTable
    {
        if(!tables.ContainsKey(id))
        {
            Debug.Log("id ¿À·ù");
            return null;
        }
        return tables[id] as T;
    }
}
