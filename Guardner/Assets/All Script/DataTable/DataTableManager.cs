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

        var stageTable = new StageTable();
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
  
    

    public static T Get<T>(string id) where T : DataTable
    {
        if(!tables.ContainsKey(id))
        {
            Debug.Log("id ����");
            return null;
        }
        return tables[id] as T;
    }
}
