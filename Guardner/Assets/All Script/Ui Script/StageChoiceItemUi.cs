using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StageState
{
    Cleared,
    Current,
    Locked
}

public class StageChoiceItemUi : MonoBehaviour
{
    [SerializeField] private Button stageChoiceButton;
    [SerializeField] private TextMeshProUGUI stageText; 

    public void SetStage(int stageNumber, StageState state)
    {
        stageText.text = stageNumber.ToString();
        switch (state)
        {
            case StageState.Cleared:
                break;
            case StageState.Current:
                break;
            case StageState.Locked:
                break;
        }

    }
}
