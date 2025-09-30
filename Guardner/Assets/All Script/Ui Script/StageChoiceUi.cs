using UnityEngine;
using UnityEngine.UI;

public class StageChoiceUi : GenericWindow
{
    [SerializeField] private GameObject clearedStagePrefab;
    [SerializeField] private GameObject currentStagePrefab;
    [SerializeField] private GameObject lockedStagePrefab;
    [SerializeField] private Transform contentParent; // ���Ե��� �� �θ� ������Ʈ
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private MainMenuUi mainMenuUi;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(OnClickExitButton);
    }

    public override void Open()
    {
        base.Open();

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        int totalStages = 15;
        int maxClearedStage = SaveLoadManager.GetMaxStage();
        int currentStageIndex = maxClearedStage + 1;

        // �������� ���൵ �����
        Debug.Log($"���� �������� ���൵: {maxClearedStage}, ���� ��������: {currentStageIndex}");


        if (maxClearedStage >= totalStages)
        {
            currentStageIndex = totalStages;
        }

        CreateStageSlots(totalStages, maxClearedStage, currentStageIndex);

        if (mainMenuUi != null)
        {
            mainMenuUi.gameObject.SetActive(false);
        }

    }

    public override void Close()
    {
        base.Close();
        if (mainMenuUi != null)
        {
            mainMenuUi.Open();
        }
    }

    private void OnClickExitButton()
    {
        Close();
    }

    public void CreateStageSlots(int totalStages, int clearedStageIndex, int currentStageIndex)
    {
        for (int i = 1; i <= totalStages; i++)
        {
            GameObject prefabToUse;
            StageState state;

            if (i <= clearedStageIndex) // i�� Ŭ������ �������� ���ϸ�
            {
                prefabToUse = clearedStagePrefab;
                state = StageState.Cleared;
            }
            else if (i == currentStageIndex) // ���� ������ ��������
            {
                prefabToUse = currentStagePrefab;
                state = StageState.Current;
            }
            else // ���� ��� ��������
            {
                prefabToUse = lockedStagePrefab;
                state = StageState.Locked;
            }

            var slot = Instantiate(prefabToUse, contentParent);
            var itemUi = slot.GetComponent<StageChoiceItemUi>();
            // ���⼭ OnStageSelected �ݹ��� �����ؾ� ��
            itemUi.SetStage(i, state, OnStageSelected);
        }
    }

    private void OnStageSelected(int stageNumber)
    {
        int stageId = GetStageId(stageNumber);

        if(stageId <= 0)
        {
            Debug.Log($"�������� {stageNumber}�� �ش��ϴ� ID�� ã�� �� �����ϴ�.");
        }

        Close();

        stageManager.LoadStage(stageId);
        //stageManager.StartStage(); // �������� ���� �߰�

        if (windowManager != null)
        {
            windowManager.Open(WindowType.Battle);
        }
        else if(battleUi != null)
        {
            battleUi.Open();
        }
        else
        {
            Debug.LogError("WindowManager�� BattleUi ��� null�Դϴ�!");
        }
    }

    private int GetStageId(int stageNumber)
    {
        switch(stageNumber)
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
