using UnityEngine;
using System.Collections.Generic;

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
    }


    public static GuardnerTable GuardnerTable
    {
        get
        {
            return Get<GuardnerTable>(DataTableIds.Guardner);
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
