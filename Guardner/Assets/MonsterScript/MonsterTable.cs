using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public enum Type
{
    One,
    Two,
    Three,
}

public class MonsterData
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int SkillId { get; private set; }
    public int SkillStackable { get; private set; }
    public float MoveSpeed { get; private set; }
    public int HP { get; private set; }
    public int Att { get; private set; }
    public string Ref { get; private set; }
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

        foreach(var monster in list)
        {
            if(!table.ContainsKey(monster.Id))
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
        if(!table.ContainsKey(id))
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
