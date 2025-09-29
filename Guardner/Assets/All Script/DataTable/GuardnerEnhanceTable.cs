using System.Collections.Generic;
using UnityEngine;


public class GuardnerEnhanceData
{
    public string Name { get; set; }
    public string EGName { get; set; }
    public int Id { get; set; }
    public int Level { get; set; }
    public int NeedGold { get; set; }
    public int NeedItemId { get; set; }
    public int NeedItemQty { get; set; }
    public int AttackPower { get; set; }
    public float APS { get; set; }
    public float DPS { get; set; }
    public int GateHP { get; set; }
    public int AttackRange { get; set; }
    public int LvStatId { get; set; }
    public string Reference { get; set; }
}


public class GuardnerEnhanceTable : DataTable
{
    private readonly Dictionary<(int, int), GuardnerEnhanceData> table = new Dictionary<(int, int), GuardnerEnhanceData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<GuardnerEnhanceData>(textAsset.text);

        foreach (var guardner in list)
        {
            var key = (guardner.Id, guardner.Level);
            if (!table.ContainsKey(key))
            {
                table.Add(key, guardner);
            }
            else
            {
                Debug.Log("아이디+레벨 중복오류");
            }
        }
    }

    public GuardnerEnhanceData Get(int id, int level)
    {
        var key = (id, level);
        if (!table.ContainsKey(key))
        {
            Debug.LogError($"강화테이블에 id {id}, level {level}이 없습니다.");
            return null;
        }
        return table[key];
    }

    public IEnumerable<GuardnerEnhanceData> GetAll()
    {
        return table.Values;
    }
}
