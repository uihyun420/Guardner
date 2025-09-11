using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GuardnerTypes
{

}

public enum GuardnerGrade
{
    One = 1,
    Two,
    Three,
}
public class GuardnerData
{
    public string Name { get; private set; }
    public int Id { get; private set; }
    public int Level { get; private set; }
    public int MaxLevel { get; private set; }
    public GuardnerTypes Type { get; private set; }
    public GuardnerGrade Grade { get; private set; }
    public int Att { get; private set; }
    public int AttackRange { get; private set; }
    public float DPS { get; private set; }
    public float APS { get; private set; }
    public float HP { get; private set; }
    public int Door { get; private set; }
    public int Cost { get; private set; }
    public int Price { get; private set; }
    public string GuardnerDescript { get; private set; }

    public override string ToString()
    {
        return $"이름: {Name}\n 도감 설명: {GuardnerDescript}";
    }

}

public class GuardnerTable : DataTable
{
    private readonly Dictionary<int, GuardnerData> table = new Dictionary<int, GuardnerData>();

    public override void Load(string filename)
    {
        table.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<GuardnerData>(textAsset.text);
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

    public GuardnerData Get(int id)
    {
        if (!table.ContainsKey(id))
        {
            Debug.Log("Id 없음");
            return null;
        }
        return table[id];
    }

    public GuardnerData GetRandom()
    {
        var guardnerList = table.Values.ToList();
        return guardnerList[Random.Range(0, guardnerList.Count)];
    }


}

