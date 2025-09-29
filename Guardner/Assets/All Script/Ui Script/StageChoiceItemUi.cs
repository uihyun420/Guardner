using System;
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

    private int stageNumber;
    private Action<int> onStageSelected;

    private void Awake()
    {
        stageChoiceButton.onClick.AddListener(OnClickStageButton);
    }

    public void SetStage(int stageNumber, StageState state, Action<int> onStageSelected = null)
    {
        this.stageNumber = stageNumber;
        this.onStageSelected = onStageSelected;

        stageText.text = stageNumber.ToString();
        switch (state)
        {
            case StageState.Cleared:
                stageChoiceButton.interactable = true;
                break;
            case StageState.Current:
                stageChoiceButton.interactable = true;
                break;
            case StageState.Locked:
                stageText.color = Color.gray;
                stageChoiceButton.interactable = false;
                break;
        }
    }


    private void OnClickStageButton()
    {
        onStageSelected?.Invoke(stageNumber);
    }
}


