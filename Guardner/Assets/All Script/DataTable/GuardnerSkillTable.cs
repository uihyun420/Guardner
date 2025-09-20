using System.Collections.Generic;
using UnityEngine;

public enum SkillTargetType
{
    Monster = 1,
    Guardner,
}

public class GuardnerSkillData
{
    public string Name { get; set; }
    public int SkillID { get; set; }
    public int KnockBack { get; set; }
    public int Stun { get; set; }
    public float GateDamageReflection { get; set; }
    public float AttackSpeedBoost { get; set; }
    public float AttackPowerBoost { get; set; }
    public float Duration { get; set; }
    public float CoolTime { get; set; }
    public int DebuffClean { get; set; } // 상태이상 해제
    public int DebuffCleanCoolTime { get; set; } //  상태 이상 해제 스킬 쿨타임
    public int IDDivide { get; set; } // ID구별
    public SkillTargetType TargetType { get; set; }
}

public class GuardnerSkillTable : DataTable
{
    private readonly Dictionary<int , GuardnerSkillData> table = new Dictionary<int, GuardnerSkillData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<GuardnerSkillData>(textAsset.text);

        foreach(var skill in list)
        {
            if (!table.ContainsKey(skill.SkillID))
            {
                //Debug.Log($"로드된 스킬 : {skill.SkillID}, {skill.Name}");
                table.Add(skill.SkillID, skill);               
            }
            else
            {
                Debug.Log("아이디 중복오류");
            }
        }
    }

    public GuardnerSkillData Get(int id)
    {
        if(!table.ContainsKey(id))
        {
            Debug.LogError($"SkillTable에 skillId {id}가 없습니다.");
            return null;
        }
        return table[id];
    }

    public IEnumerable<GuardnerSkillData> GetAll()
    {
        return table.Values;
    }
}
