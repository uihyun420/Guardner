using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public StageData stageData;
    private StageTable stageTable;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private BattleUi battleUi;

    public int currentWave;
    public int enemiesRemaining;
    public bool isBossStage;
    public bool isStageCompleted;
    public int monsterKillCount = 0;

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
        monsterFId = stageData.MonsterFId;
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

        int bossMonsterId = 0;
        if (stage == 5) bossMonsterId = 4222300;
        else if (stage == 10) bossMonsterId = 4222500;
        else if (stage == 15) bossMonsterId = 4222400;

        if (bossMonsterId != 0)
        {
            Vector2 bossSpawnPos = new Vector2(-3, 4); // 원하는 위치로 조정
            StartCoroutine(CoSpawnBossMonster(bossMonsterId, bossSpawnPos, 10));
        }
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

        int spawnCount = 0;

        while (!isStageCompleted && spawnCount < totalWaveCount)
        {
            monsterSpawner.SpawnMonster(monsterId, spawnPos, sortingOrder);
            enemiesRemaining++;
            spawnCount++;
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
            if (monsterId == 0)
            {
                continue;
            }

            float initialDelay = (i == 0) ? 0f : waveMonsterRespawnInterval + 1f;
            float interval = waveMonsterRespawnInterval;

            StartCoroutine(CoSpawnMonsterWithDelay(baseSpawnMonster, monsterId, spawnPos, i, initialDelay, interval));
        }
        yield return null;
    }

    public void OnBattleTimerEnd()
    {
        isStageCompleted = true;
        gameManager.OnStageClear();
    }

    public void StageStop()
    {
        if(monsterSpawner != null)
        {
            monsterSpawner.StopAllCoroutines();
        }
        isStageCompleted = true;
    }

    private IEnumerator CoSpawnBossMonster(int bossMonsterId, Vector2 spawnPos, int sortingOrder)
    {
        yield return new WaitForSeconds(1f); // 1분 대기

        if (!isStageCompleted)
        {
            monsterSpawner.SpawnMonster(bossMonsterId, spawnPos, sortingOrder);
            enemiesRemaining++;
            Debug.Log($"보스 몬스터 {bossMonsterId} 스폰됨");
        }
    }
}
