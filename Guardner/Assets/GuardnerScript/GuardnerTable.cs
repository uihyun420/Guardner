using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GuardnerTypes
{
    Melee = 1,
    Ranged,
    Support,
}

public enum GuardnerGrade
{
    General = 1,
    Unique,
    Legendery,
}
public class GuardnerData
{
    public string Name { get; private set; }
    public string EGName { get; private set; }
    public int Id { get; private set; }
    public int IdDivide { get; private set; } // ID 구분
    public int Level { get; private set; }
    public GuardnerTypes Role { get; private set; } // 타입
    public int AttackPower { get; private set; }
    public float APS { get; private set; }
    public float DPS { get; private set; }
    public int GateHP { get; private set; } // 문 추가 내구도 
    public int AttackRange { get; private set; }
    public int SummonGold { get; private set; } // 소환 골드 
    public int SellingGold { get; private set; } // 판매 골드
    public GuardnerGrade Rarity { get; private set; } // 등급
    public int MaxUPLevel { get; private set; } // 강화 최대 레벨 
    public int SkillID { get; private set; }
    public int LvStatId { get; private set; } // 레벨 추가 능력치 ID
    public string Reference { get; private set; } // 설명

    public override string ToString()
    {
        return $"이름: {Name}\n 도감 설명: {Reference}";
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

