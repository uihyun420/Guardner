
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class StageClearUi : GenericWindow
{
    [SerializeField] private Button retry;
    [SerializeField] private Button nextStage;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;

    private void Start()
    {
        retry.onClick.AddListener(OnRetryButton);
        nextStage.onClick.AddListener(OnNextStageButton);
    }


    public override void Open()
    {
        base.Open();

        // UI�� ���� �� ���� ��������
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (stageManager == null)
            stageManager = FindObjectOfType<StageManager>();

        Time.timeScale = 0;
    }
    public override void Close()
    {
        base.Close();
    }

    public void OnNextStageButton()
    {
        Debug.Log("=== OnNextStageButton ���� ===");

        if (stageManager == null || stageManager.stageData == null)
        {
            Debug.Log("StageManager �Ǵ� StageData�� null�Դϴ�.");
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

        Time.timeScale = 1;

        battleUi.ResetBattleTimer();

        Close();

        // �������� �ε�
        stageManager.LoadStage(nextStageId);

        // WindowManager�� ���� Battle UI�� ��ȯ
        if (manager != null)
        {
            manager.Open(WindowType.Battle);
        }
        stageManager.StartStage();
    }


    public void OnRetryButton()
    {
        if (stageManager == null || stageManager.stageData == null)
            return;

        int currentStageId = stageManager.stageData.ID;

        // Ÿ�ӽ����� ����
        Time.timeScale = 1;

        // �������� �ε�
        stageManager.LoadStage(currentStageId);

        // WindowManager�� ���� Battle UI�� ��ȯ
        if (manager != null)
        {
            manager.Open(WindowType.Battle);
        }

        // �������� ����
        stageManager.StartStage();
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
}
