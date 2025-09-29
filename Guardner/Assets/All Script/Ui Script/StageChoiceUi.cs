using UnityEngine;
using UnityEngine.UI;

public class StageChoiceUi : GenericWindow
{
    [SerializeField] private GameObject clearedStagePrefab;
    [SerializeField] private GameObject currentStagePrefab;
    [SerializeField] private GameObject lockedStagePrefab;
    [SerializeField] private Transform contentParent; // 슬롯들이 들어갈 부모 오브젝트

    [SerializeField] private Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(OnClickExitButton);
    }


    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
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

            if (i < currentStageIndex)
            {
                prefabToUse = clearedStagePrefab;
                state = StageState.Cleared;
            }

            else if (i == currentStageIndex)
            {
                prefabToUse = currentStagePrefab;
                state = StageState.Current;
            }
            else
            {
                prefabToUse = lockedStagePrefab;
                state = StageState.Locked;
            }

            var slot = Instantiate(prefabToUse, contentParent);
            var itemUi = slot.GetComponent<StageChoiceItemUi>();
            itemUi.SetStage(i, state);
        }
    }
}
