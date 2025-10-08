using System.Collections.Generic;
using UnityEngine;



public class GuardnerSkillEnhanceData
{
    public string Name {  get; set; }
    public int SkillId {  get; set; } // ����� ��ų ��ȭ ID
    public int Id {  get; set; } // ����� ��ų ID
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
                Debug.Log("���̵�+���� �ߺ�����");
            }
        }
    }

    public GuardnerSkillEnhanceData Get(int id, int level)
    {
        var key = (id, level);
        if (!table.ContainsKey(key))
        {
            Debug.LogError($"��ȭ���̺� id {id}, level {level}�� �����ϴ�.");
            return null;
        }
        return table[key];
    }

    public IEnumerable<GuardnerSkillEnhanceData> GetAll()
    {
        return table.Values;
    }
}
