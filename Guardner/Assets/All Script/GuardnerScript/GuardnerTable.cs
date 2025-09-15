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
    public string Name { get;  set; }
    public string EGName { get;  set; }
    public int Id { get;  set; }
    public int IdDivide { get;  set; } // ID 구분
    public int Level { get;  set; }
    public GuardnerTypes Role { get;  set; } // 타입
    public int AttackPower { get;  set; }
    public float APS { get;  set; }
    public float DPS { get;  set; }
    public int GateHP { get;  set; } // 문 추가 내구도 
    public int AttackRange { get;  set; }
    public int SummonGold { get;  set; } // 소환 골드 
    public int SellingGold { get;  set; } // 판매 골드
    public GuardnerGrade Rarity { get;  set; } // 등급
    public int MaxUPLevel { get;  set; } // 강화 최대 레벨 
    public int SkillID { get;  set; }
    public int LvStatId { get;  set; } // 레벨 추가 능력치 ID
    public string Reference { get;  set; } // 설명

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

