
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

    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject spawnRect;
    [SerializeField] private InventoryUi inventoryUi;

    [SerializeField] private TextMeshProUGUI stageRewardText;
    [SerializeField] private TextMeshProUGUI bonusGoldRewardText;
    [SerializeField] private TextMeshProUGUI guardnerItemRewardText;
    [SerializeField] private TextMeshProUGUI skillItemRewardText;

    [SerializeField] private GameObject bonusGold;
    [SerializeField] private GameObject guardnerItemReward;
    [SerializeField] private GameObject skillItemReward;

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
            SaveStageCompletion(currentStage);
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

        if (rewardData == null)
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
            else if (stage >= 11)
            {
                killUnit = 130;
            }

            int bonusCount = stageManager.monsterKillCount / killUnit;
            if (bonusCount > 0)
            {
                bonusReward += rewardData.Bonus2RewardIdRewardQty * bonusCount;
                stageManager.monsterKillCount = 0;
            }
        }

        // ��� ����
        if (mainMenuUi != null && totalReward > 0)
        {
            // mainMenuUi.AddMainUiGold(totalReward + bonusReward);
            SaveLoadManager.AddGold(totalReward + bonusReward);
            Debug.Log($"�������� {currentStage} Ŭ���� ����: {totalReward} ��� ����");
        }


        if (gardenerTickets > 0)
        {
            inventoryUi?.AddItem("LotteryTicket", gardenerTickets); // ����� �̱�� �κ��丮�� ����
        }

        if (playerSkillTickets > 0)
        {
            inventoryUi?.AddItem("EnhanceTicket", playerSkillTickets); // �÷��̾� ��ų �̱�� �κ��丮�� ����
        }

        // UI ǥ��/���� ó��
        if (totalReward > 0)
        {
            stageRewardText.text = totalReward.ToString();
            stageRewardText.gameObject.SetActive(true);
        }
        else
        {
            stageRewardText.gameObject.SetActive(false);
        }

        if (bonusReward > 0)
        {
            bonusGoldRewardText.text = bonusReward.ToString();
            bonusGold.gameObject.SetActive(true);
        }
        else
        {
            bonusGold.SetActive(false);
        }

        if (gardenerTickets > 0)
        {
            guardnerItemRewardText.text = gardenerTickets.ToString();
            guardnerItemReward.gameObject.SetActive(true);
        }
        else
        {
            guardnerItemReward.gameObject.SetActive(false);
        }

        if (playerSkillTickets > 0)
        {
            skillItemRewardText.text = playerSkillTickets.ToString();
            skillItemReward.gameObject.SetActive(true);
        }
        else
        {
            skillItemReward.gameObject.SetActive(false);
        }
    }
    private void SetGameClearStageText()
    {
        var sb = new StringBuilder();
        sb.Clear();
        sb.Append("STAGE LEVEL ").Append(stageManager.stageData.Stage);
        stageText.text = sb.ToString();
    }

    private void SaveStageCompletion(int clearStage)
    {
        SaveLoadManager.Data.CurrentStage = clearStage;
        SaveLoadManager.Data.IsStageCleared = true;
        SaveLoadManager.UpdateStageProgress(clearStage);
        Debug.Log($"�������� {clearStage} �Ϸ� ������ ����Ǿ����ϴ�. ���� �������� {clearStage}�� ����Ǿ����ϴ�.");
    }


}

