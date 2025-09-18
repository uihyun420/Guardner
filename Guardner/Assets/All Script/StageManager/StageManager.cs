using UnityEngine;

public class StageManager : MonoBehaviour
{
    public StageData stageData;
    private StageTable stageTable;
    [SerializeField] private MonsterSpawner monsterSpawner;


    public int currentWave;
    public int enemiesRemaining;
    public bool isBossStage;
    public bool isStageCompleted;

    private int id;
    private int stage; //스테이지 단계
    private string baseSpawnMonster;
    private float waveMonsterRespawnInterval;
    private int monsterAId;
    private int monsterBId;
    private int monsterCId;
    private int monsterDId;
    private int monsterEId;
    private int monsterFId;
    private int monsterGId;
    private int monsterHId;
    private string waveMonsterList;
    private int totalWaveCount;
    private string bossMonster;
    private float hpBoost;
    private float attackPowerBoost;
    private float movementSpeed;
    private int rewardId;

    private void Init(StageData data)
    {
        stageData = data;
        id = stageData.ID;
        stage = stageData.Stage;
        baseSpawnMonster = stageData.BaseSpawnMonster;
        waveMonsterRespawnInterval = stageData.WaveMonsterRespawnInterval;
        monsterAId = stageData.MonsterAId;
        monsterBId = stageData.MonsterBId;
        monsterCId = stageData.MonsterCId;
        monsterDId = stageData.MonsterDId;
        monsterEId = stageData.MonsterEId;
        monsterGId = stageData.MonsterGId;
        monsterHId = stageData.MonsterHId;
        waveMonsterList = stageData.WaveMonsterList;
        totalWaveCount = stageData.TotalWaveCount;
        bossMonster = stageData.BossMonster;
        hpBoost = stageData.HPBoost;
        attackPowerBoost = stageData.AttackPowerBoost;
        movementSpeed = stageData.MovementSpeed;
        rewardId = stageData.RewardID;
    }

    public void StartStage()
    {
        if (stageData == null)
        {
            Debug.Log("스테이지 데이터가 로드되지 않았습니다.");
            return;
        }

        ResetStageProgress();
        Debug.Log($"스테이지 {stage} 시작! 총 {totalWaveCount}개의 웨이브");
        StartWave();
    }


    public void LoadStage(int stageId)
    {
        if(stageTable == null)
        {
            stageTable = new StageTable();
            stageTable.Load(DataTableIds.Stage);
        }
        StageData data = stageTable.Get(stageId);
        if(data !=null)
        {
            Init(data);
            Debug.Log($"스테이지 : {stage} 로드 완료");
        }
        else
        {
            Debug.Log($"스테이지 ID : {stageId}를 찾을 수 없습니다");
        }
    }

    private void ResetStageProgress()
    {
        currentWave = 0;
        enemiesRemaining = 0;
        isBossStage = false;
        isStageCompleted = false;
    }

    public void StartWave()
    {
        currentWave++;

        if(currentWave > totalWaveCount) // 보스 소환 조건
        {
            isStageCompleted = true;
            Debug.Log("스테이지 클리어!");
            return;
        }
        Debug.Log($"웨이브 : {currentWave}, {totalWaveCount} 시작!");
        SpawnWaveMonster();
    }

    public void SpawnWaveMonster()
    {
        if(string.IsNullOrEmpty(waveMonsterList))
        {
            Debug.Log("웨이브 몬스터 리스트가 비었습니다.");
            return;
        }
        //waveMonsterList를 파싱하여 몬스터 ID 배열로 변환
        string[] monsterIds = waveMonsterList.Split(',');

        foreach(string monsterIdStr in monsterIds)
        {
            if(int.TryParse(monsterIdStr.Trim(), out int monsterId))
            {
                monsterSpawner.SpawnMonster(monsterId);
                enemiesRemaining++;
            }
        }
        Debug.Log($"웨이브 {currentWave}: {enemiesRemaining}마리 몬스터 소환 완료");
    }
    
}
