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
    public int IdDivide { get;  set; } // ID ����
    public int Level { get;  set; }
    public GuardnerTypes Role { get;  set; } // Ÿ��
    public int AttackPower { get;  set; }
    public float APS { get;  set; }
    public float DPS { get;  set; }
    public int GateHP { get;  set; } // �� �߰� ������ 
    public int AttackRange { get;  set; }
    public int SummonGold { get;  set; } // ��ȯ ��� 
    public int SellingGold { get;  set; } // �Ǹ� ���
    public GuardnerGrade Rarity { get;  set; } // ���
    public int MaxUPLevel { get;  set; } // ��ȭ �ִ� ���� 
    public int SkillID { get;  set; }
    public int LvStatId { get;  set; } // ���� �߰� �ɷ�ġ ID
    public string Reference { get;  set; } // ����

    public override string ToString()
    {
        return $"�̸�: {Name}\n ���� ����: {Reference}";
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
                Debug.Log("���̵� �ߺ�����");
            }
        }
    }

    public GuardnerData Get(int id)
    {
        if (!table.ContainsKey(id))
        {
            Debug.Log("Id ����");
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

