using System.Collections.Generic;
using UnityEngine;



public class GuardnerSkillEnhanceData
{
    public string Name {  get; set; }
    public int SkillId {  get; set; } // 가드너 스킬 강화 ID
    public int Id {  get; set; } // 가드너 스킬 ID
    public int Level { get; set; }
    public float HPReduction {  get; set; }
    public float AttackPowerBoost { get; set; }
    public float AttackSpeed {  get; set; }
    public float EnemyMovementSpeed { get; set; }
    public float EnemyAttackSpeed { get; set; }
    public float CriticalChance { get; set; }
    public float AccuracyReduction { get; set; }
    public float SkillCoolTimeReduction {  get; set; }
    public int Bind {  get; set; }
    public int Stun {  get; set; }
    public int Duration { get; set; }
    public int CoolTime {  get; set; }
    public int NeedGold { get; set; }
}


public class GuardnerSkillEnhanceTable : DataTable
{
    private readonly Dictionary<(int, int), GuardnerSkillEnhanceData> table = new Dictionary<(int, int), GuardnerSkillEnhanceData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<GuardnerSkillEnhanceData>(textAsset.text);

        foreach(var guardnerSkill in list)
        {
            var key = (guardnerSkill.Id, guardnerSkill.Level);
            if (!table.ContainsKey(key))
            {
                table.Add(key, guardnerSkill);
            }
            else
            {
                Debug.Log("아이디+레벨 중복오류");
            }
        }
    }

    public GuardnerSkillEnhanceData Get(int id, int level)
    {
        var key = (id, level);
        if (!table.ContainsKey(key))
        {
            Debug.LogError($"강화테이블에 id {id}, level {level}이 없습니다.");
            return null;
        }
        return table[key];
    }

    public IEnumerable<GuardnerSkillEnhanceData> GetAll()
    {
        return table.Values;
    }
}
