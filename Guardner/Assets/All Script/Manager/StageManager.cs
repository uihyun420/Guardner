using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public StageData stageData;
    private StageTable stageTable;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MonsterSpawner monsterSpawner;

    public int currentWave;
    public int enemiesRemaining;
    public bool isBossStage;
    public bool isStageCompleted;

    private int id;
    private int stage; //스테이지 단계
    private int baseSpawnMonster;
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
        monsterSpawner.ClearMonster();
        StopAllCoroutines();
        
        ResetStageProgress();
        Debug.Log($"스테이지 {stage} 시작! 총 {totalWaveCount}개의 웨이브");
        StartCoroutine(CoSpawnWaveMonster());
    }


    public void LoadStage(int stageId)
    {
        stageTable = DataTableManager.StageTable;

        StageData data = stageTable.Get(stageId);
        if (data != null)
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

    private IEnumerator CoSpawnMonsterWithDelay(int baseSpawnMonster, int monsterId, Vector2 spawnPos, int sortingOrder, float initialDelay, float interval)
    {
        yield return new WaitForSeconds(initialDelay);
        while (!isStageCompleted)
        {
            monsterSpawner.SpawnMonster(monsterId, spawnPos, sortingOrder);
            enemiesRemaining++;
            Debug.Log($"몬스터 {monsterId}");
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator CoSpawnWaveMonster()
    {
        int[] monsterIds = new int[] { baseSpawnMonster, monsterAId, monsterBId, monsterCId, monsterDId, monsterEId, monsterFId, monsterHId };
        Vector2 spawnPos = new Vector2(-3, 4);        

        for (int i = 0; i < monsterIds.Length; i++)
        {
            int monsterId = monsterIds[i];
            if (monsterId == 0) continue;


            float initialDelay = (i == 0) ? 0f : waveMonsterRespawnInterval + 1f; 
            float interval = waveMonsterRespawnInterval;

            StartCoroutine(CoSpawnMonsterWithDelay(baseSpawnMonster, monsterId, spawnPos, i, initialDelay, interval));
        }

        yield return null;
    }

    public void OnBattleTimerEnd()
    {
        if (!isStageCompleted)
        {
            isStageCompleted = true;
            gameManager.OnStageClear(); // 싱글톤 또는 참조 방식
        }
    }
}
