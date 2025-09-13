using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public enum Type
{
    Common = 1,
    Normal,
    Boss,
}

public class MonsterData
{
    public int Id { get;  set; }
    public string Name { get;  set; }
    public string EGName { get;  set; }
    public int IdDivide { get;  set; }
    public Type Type { get;  set; }
    public int SkillId { get;  set; }
    public int OverLapping { get;  set; } // 스킬 중첩 여부
    public float BaseMoveSpeed { get;  set; }
    public int HP { get;  set; } 
    public int BaseAttackPower { get;  set; }
    public string Reference { get;  set; }
}

public class MonsterTable : DataTable
{
    private readonly Dictionary<int, MonsterData> table = new Dictionary<int, MonsterData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<MonsterData>(textAsset.text);

        foreach (var monster in list)
        {
            if (!table.ContainsKey(monster.Id))
            {
                table.Add(monster.Id, monster);
            }
            else
            {
                Debug.Log("아이디 중복오류");
            }
        }
    }

    public MonsterData Get(int id)
    {
        if (!table.ContainsKey(id))
        {
            return null;
        }
        return table[id];
    }

    public MonsterData GetRandom()
    {
        var monsterList = table.Values.ToList();
        return monsterList[Random.Range(0, monsterList.Count)];
    }
}
