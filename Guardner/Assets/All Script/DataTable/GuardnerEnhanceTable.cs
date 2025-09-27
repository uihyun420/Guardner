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
}


public class GuardnerEnhanceTable : DataTable
{
    private readonly Dictionary<int, GuardnerEnhanceData> table = new Dictionary<int, GuardnerEnhanceData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<GuardnerEnhanceData>(textAsset.text);

        foreach (var guardner in list)
        {
            if (!table.ContainsKey(guardner.Id))
            {                
                table.Add(guardner.Id, guardner);
            }
            else
            {
                Debug.Log("아이디 중복오류");
            }
        }
    }

    public GuardnerEnhanceData Get(int id)
    {
        if (!table.ContainsKey(id))
        {
            Debug.LogError($"SkillTable에 skillId {id}가 없습니다.");
            return null;
        }
        return table[id];
    }

    public IEnumerable<GuardnerEnhanceData> GetAll()
    {
        return table.Values;
    }
}
