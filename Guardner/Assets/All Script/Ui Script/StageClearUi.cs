
using System.Net.NetworkInformation;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageClearUi : GenericWindow
{
    [SerializeField] private Button mainMenu;
    [SerializeField] private Button nextStage;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private GuardnerSpawner guardnerSpawner;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private MainMenuUi mainMenuUi;
    [SerializeField] private TextMeshProUGUI stageRewardText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject spawnRect;


    private bool rewardGiven = false;
    private int lastRewardedStage = -1; // ������ ������ ������ �������� 

    private void Start()
    {
        mainMenu.onClick.AddListener(OnClickMainMenuButton);
        nextStage.onClick.AddListener(OnNextStageButton);
    }

    public override void Open()
    {
        base.Open();

        spawnRect.SetActive(false);
        // UI�� ���� �� ���� ��������
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (stageManager == null)
            stageManager = FindObjectOfType<StageManager>();

        int currentStage = stageManager.stageData.Stage;
        if (!rewardGiven && lastRewardedStage != currentStage)
        {
            GiveStageReward();
            rewardGiven = true;
            lastRewardedStage = currentStage;
        }
        SetGameClearStageText();
        Time.timeScale = 0;
    }
    public override void Close()
    {
        base.Close();
        spawnRect.SetActive(true);

    }

    public void OnNextStageButton()
    {
        if (stageManager == null || stageManager.stageData == null)
        {
            return;
        }

        // �������� ���� ��������
        int currentStage = stageManager.stageData.Stage;
        int nextStage = currentStage + 1;
        int nextStageId = FindNextStageId(nextStage);

        Debug.Log($"���� ��������: {currentStage}, ���� ��������: {nextStage}, ���� �������� ID: {nextStageId}");

        if (nextStageId <= 0)
        {
            if (gameManager != null)
                gameManager.GameExit();
            return;
        }
        rewardGiven = false;

        Time.timeScale = 1;

        battleUi.ResetBattleTimer();

        Close();

        stageManager.LoadStage(nextStageId);
        monsterSpawner.ClearMonster();
        guardnerSpawner.ClearGuardner();

        if (manager != null)
        {
            manager.Open(WindowType.Battle);
        }
    }


    public void OnClickMainMenuButton()
    {
        Time.timeScale = 1;
        rewardGiven = false;
        stageManager.StageStop();
        monsterSpawner.ClearMonster();
        guardnerSpawner.ClearGuardner();

        // BattleUI �ʱ�ȭ (�ʿ��� ���)
        if (battleUi != null)
        {
            battleUi.ResetBattleTimer();
            battleUi.gameObject.SetActive(false);  // BattleUI ��Ȱ��ȭ

            // BattleUI �ڽ� ������Ʈ�� ��Ȱ��ȭ (�ʿ��� ���)
            if (battleUi.battleUi != null)
                battleUi.battleUi.SetActive(false);
        }

        Close();

        if (manager != null)
        {
            manager.Open(WindowType.MainMenuUi);
        }
    }


    // Stage ��ȣ�� �ش��ϴ� �������� ID ã��
    private int FindNextStageId(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1: return 1630;
            case 2: return 2640;
            case 3: return 3650;
            case 4: return 4660;
            case 5: return 56100;
            case 6: return 65110;
            case 7: return 75120;
            case 8: return 85130;
            case 9: return 95140;
            case 10: return 105150;
            case 11: return 114160;
            case 12: return 124170;
            case 13: return 134180;
            case 14: return 144190;
            case 15: return 153200;
            default: return -1; // �������� �ʴ� ��������
        }
    }

    public void GiveStageReward()
    {
        int currentStage = stageManager.stageData.Stage;
        var stageRewardTable = DataTableManager.StageRewardTable;
        var rewardData = stageRewardTable.GetStage(currentStage);

        if(rewardData == null)
        {
            Debug.Log($"�������� {currentStage}�� ���� ���� �����Ͱ� �����ϴ�.");
            return;
        }

        int totalReward = 0;
        int bonusReward = 0;
        int gardenerTickets = 0;  // ����� �̱��
        int playerSkillTickets = 0;  // �÷��̾� ��ų �̱��

        // �⺻����
        if (rewardData.BaseRewardId == 11002)
        {
            totalReward += rewardData.BaseRewardIdQty;
        }

        // �߰����� (�̱�� ó�� �߰�)
        if (rewardData.BaseReward2Id == 11002)
        {
            totalReward += rewardData.BaseReward2IdQty;
        }
        else if (rewardData.BaseReward2Id == 131991)  // ����� �̱��
        {
            gardenerTickets += rewardData.BaseReward2IdQty;
        }
        else if (rewardData.BaseReward2Id == 231991)  // �÷��̾� ��ų �̱��
        {
            playerSkillTickets += rewardData.BaseReward2IdQty;
        }

        if (rewardData.Bonus1RewardId == 11002)
        {
            totalReward += rewardData.Bonus1RewardIdRewardQty;
        }

        // ���ʽ� ���� 2 (Bonus2RewardId�� 11003�� �߰� ���)
        if (rewardData.Bonus2RewardId == 11003)
        {
            int killUnit = 30;
            int stage = stageManager.stageData.Stage;
            if (stage >= 6 && stage <= 10)
            {
                killUnit = 100;
            }
            else if(stage >= 11)
            {
                killUnit = 130;
            }

            int bonusCount = stageManager.monsterKillCount / killUnit;
            if(bonusCount > 0)
            {
                bonusReward += rewardData.Bonus2RewardIdRewardQty * bonusCount;
                stageManager.monsterKillCount = 0;
            }            
        }

        // ��� ����
        if (mainMenuUi != null && totalReward > 0)
        {
            mainMenuUi.AddMainUiGold(totalReward + bonusReward);
            Debug.Log($"�������� {currentStage} Ŭ���� ����: {totalReward} ��� ����");
        }

        // �̱�� ���� (���߿� ������ �޼����)
        if (gardenerTickets > 0)
        {
            // TODO: ����� �̱�� ���� ���� �߰�
            Debug.Log($"�������� {currentStage} Ŭ���� ����: {gardenerTickets} ����� �̱�� ����");
        }

        if (playerSkillTickets > 0)
        {
            // TODO: �÷��̾� ��ų �̱�� ���� ���� �߰�
            Debug.Log($"�������� {currentStage} Ŭ���� ����: {playerSkillTickets} �÷��̾� ��ų �̱�� ����");
        }

        // UI �ؽ�Ʈ ������Ʈ
        var sb = new StringBuilder();
        sb.Append(totalReward).Append(" G");
        if(bonusReward > 0)
        {
            sb.Append("\n���ʽ� ���� : ").Append(bonusReward).Append(" G");
        }
        if (gardenerTickets > 0)
            sb.Append("\n������ �̱�� : ").Append(gardenerTickets).Append("��");

        if (playerSkillTickets > 0)
            sb.Append("����� ��ų �̱�� : ").Append(playerSkillTickets).Append("��");

        stageRewardText.text = sb.ToString();
    }
    private void SetGameClearStageText()
    {
        var sb = new StringBuilder();
        sb.Clear();
        sb.Append("STAGE LEVEL ").Append(stageManager.stageData.Stage);
        stageText.text = sb.ToString();
    }
}
