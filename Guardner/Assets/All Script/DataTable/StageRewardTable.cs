using System.Collections.Generic;
using UnityEngine;


public class StageRewardData
{
    public int Stage { get; set; }
    public int RewardID { get; set; }
    public int BaseRewardId { get; set; }
    public int BaseRewardIdQty { get; set; }
    public int BaseReward2Id { get; set; }
    public int BaseReward2IdQty { get; set; }
    public int Bonus1RewardId { get; set; }
    public int Bonus1RewardIdRewardQty { get; set; }
    public string Bonus1RewardIdCondition { get; set; }
    public int Bonus2RewardId { get; set; }
    public int Bonus2RewardIdRewardQty { get; set; }
    public string Bonus2RewardIdCondition { get; set; }
}




public class StageRewardTable : DataTable
{
    private readonly Dictionary<int, StageRewardData> table = new Dictionary<int, StageRewardData>();
    public override void Load(string filename)
    {
        table.Clear();
        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<StageRewardData>(textAsset.text);

        foreach (var stageReward in list)
        {
            if (!table.ContainsKey(stageReward.RewardID))
            {
                Debug.Log($"로드된 보상 : {stageReward.RewardID}");
                table.Add(stageReward.RewardID, stageReward);
            }
            else
            {
                Debug.Log("아이디 중복오류");
            }
        }
    }

    public StageRewardData Get(int id)
    {
        if (!table.ContainsKey(id))
        {
            Debug.LogError($"StageRewardTable에 RewardId {id}가 없습니다.");
            return null;
        }
        return table[id];
    }

    public IEnumerable<StageRewardData> GetAll()
    {
        return table.Values;
    }
}
