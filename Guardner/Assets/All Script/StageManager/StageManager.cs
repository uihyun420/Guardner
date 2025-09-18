using System.Collections;
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
        StartCoroutine(CoSpawnWaveMonster());
    }


    public void LoadStage(int stageId)
    {
        if (stageTable == null)
        {
            stageTable = new StageTable();
            stageTable.Load(DataTableIds.Stage);
        }
        StageData data = stageTable.Get(stageId);
        if (data != null)
        {
            Init(data);
           // Debug.Log($"스테이지 : {stage} 로드 완료");
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

    public void SpawnWaveMonster()
    {
        int[] monsterIds = new int[] { monsterAId, monsterBId, monsterCId, monsterDId, monsterEId, monsterFId, monsterHId};
        int spawnCount = 0;
        foreach(var monsterId in monsterIds)
        {
            monsterSpawner.SpawnMonster(monsterId);
            enemiesRemaining++;
            spawnCount++;
            Debug.Log($"소환된 몬스터 {monsterId}");
        }
        Debug.Log($"웨이브 : {currentWave}, {spawnCount}마리 몬스터 소환 완료");
       
    }
    
    private IEnumerator CoSpawnWaveMonster()
    {
        for(int i = 0; i < totalWaveCount; i++)
        {
            currentWave = i + 1;
            SpawnWaveMonster();
            yield return new WaitForSeconds(waveMonsterRespawnInterval);
        }
        isStageCompleted = true;
        Debug.Log("스테이지 클리어");
    }

}
