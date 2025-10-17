using System.Collections.Generic;
using UnityEngine;

public class StageData
{    
    public int ID { get; set; }
    public int Stage { get; set; } // 스테이지 단계
    public int BaseSpawnMonster { get; set; }
    public int BaseMonsterRespawnInterval { get; set; }
    public float WaveMonsterRespawnInterval { get; set; }
    public int MonsterAId { get; set; }
    public int MonsterBId { get; set; }
    public int MonsterCId { get; set; }
    public int MonsterDId { get; set; }
    public int MonsterEId { get; set; }
    public int MonsterFId { get; set; }
    public int MonsterGId { get; set; }
    public int MonsterHId { get; set; }
    public string WaveMonsterList { get; set; }
    public int TotalWaveCount { get; set; }
    public string BossMonster { get; set; }
    public float HPBoost { get; set; }
    public float AttackPowerBoost {get; set;}
    public float MovementSpeed { get; set; }
    public int RewardID { get; set; }
}
public class StageTable : DataTable
{
    private readonly Dictionary<int, StageData> table = new Dictionary<int, StageData>();
    
    public override void Load(string filename)
    {
        table.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<StageData>(textAsset.text);

        foreach(var stage in list)
        {
            if(!table.ContainsKey(stage.ID))
            {
                table.Add(stage.ID, stage);
            }
        }
    }

    public StageData Get(int id)
    {
        if(!table.ContainsKey(id))
        {
            return null;
        }
        return table[id];
    }
}
