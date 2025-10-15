using System.Collections.Generic;
using UnityEngine;


public class DictionarySetData
{
    public int SetId { get; set; }
    public string SetName { get; set; }
    public string ConditionDescription { get; set; }
    public int RequiredGardener1Id { get; set; }
    public int RequiredGardener2Id { get; set; }
    public int RequiredGardener3Id { get; set; }
    public int RequiredGardener4Id { get; set; }
    public int RewardId { get; set; }
    public int RewardAmount { get; set; }
}

public class DictionarySetTable : DataTable
{
    private readonly Dictionary<int, DictionarySetData> table = new Dictionary<int, DictionarySetData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<DictionarySetData>(textAsset.text);

        foreach (var skill in list)
        {
            if (!table.ContainsKey(skill.SetId))
            {
                table.Add(skill.SetId, skill);
            }
            else
            {
                Debug.Log("아이디 중복오류");
            }
        }
    }

    public DictionarySetData Get(int id)
    {
        if (!table.ContainsKey(id))
        {
            return null;
        }
        return table[id];
    }

    public IEnumerable<DictionarySetData> GetAll()
    {
        return table.Values;
    }
}
