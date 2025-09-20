using System.Collections.Generic;
using UnityEngine;
public enum Rairity
{
    Normal = 1,
    Rair,
    Unique,
}

public class PlayerSkillData
{
    public string Name { get; set; }
    public int Id { get; set; }
    public Rairity SkillRarity { get; set; }
    public int Stun { get; set; }
    public int Bind { get; set; }
    public float AttackSpeed { get; set; }
    public float AttackPowerBoost { get; set; }
    public float EnemyAttackSpeed { get; set; }
    public float EnemyMovementSpeed { get; set; }
    public float CriticalChance { get; set; }
    public float HPReduction { get; set; }
    public float SkillCoolTimeReduction { get; set; }
    public float AccuracyReduction { get; set; }
    public int Range { get; set; }
    public int Duration { get; set; }
    public int CoolTime { get; set; }
    public string SkillDescription { get; set; }
    public int GardenerSkillDrawId { get; set; }
    public SkillTargetType TargetType { get; set; }
}

public class PlayerSkillTable : DataTable
{
    private readonly Dictionary<int, PlayerSkillData> table = new Dictionary<int, PlayerSkillData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<PlayerSkillData>(textAsset.text);

        foreach(var skill in list)
        {
            if(!table.ContainsKey(skill.Id))
            {
                //Debug.Log($"로드된 스킬 : {skill.Id}, {skill.Name}");
                table.Add(skill.Id, skill);
            }
            else
            {
                Debug.Log("아이디 중복");
            }
        }      
    }
    public PlayerSkillData Get(int id)
    {
        if(!table.ContainsKey(id))
        {
            Debug.LogError($"SkillTable에 skillId {id}가 없습니다.");
            return null;
        }
        return table[id];
    }

    public IEnumerable<PlayerSkillData> GetAll()
    {
        return table.Values;
    }
}
