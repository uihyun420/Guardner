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
