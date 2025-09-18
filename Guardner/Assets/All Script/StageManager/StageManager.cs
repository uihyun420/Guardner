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
    private int stage; //�������� �ܰ�
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
            Debug.Log("�������� �����Ͱ� �ε���� �ʾҽ��ϴ�.");
            return;
        }

        ResetStageProgress();
        Debug.Log($"�������� {stage} ����! �� {totalWaveCount}���� ���̺�");
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
            Debug.Log($"�������� : {stage} �ε� �Ϸ�");
        }
        else
        {
            Debug.Log($"�������� ID : {stageId}�� ã�� �� �����ϴ�");
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

        if(currentWave > totalWaveCount) // ���� ��ȯ ����
        {
            isStageCompleted = true;
            Debug.Log("�������� Ŭ����!");
            return;
        }
        Debug.Log($"���̺� : {currentWave}, {totalWaveCount} ����!");
        SpawnWaveMonster();
    }

    public void SpawnWaveMonster()
    {
        if(string.IsNullOrEmpty(waveMonsterList))
        {
            Debug.Log("���̺� ���� ����Ʈ�� ������ϴ�.");
            return;
        }
        //waveMonsterList�� �Ľ��Ͽ� ���� ID �迭�� ��ȯ
        string[] monsterIds = waveMonsterList.Split(',');

        foreach(string monsterIdStr in monsterIds)
        {
            if(int.TryParse(monsterIdStr.Trim(), out int monsterId))
            {
                monsterSpawner.SpawnMonster(monsterId);
                enemiesRemaining++;
            }
        }
        Debug.Log($"���̺� {currentWave}: {enemiesRemaining}���� ���� ��ȯ �Ϸ�");
    }
    
}
