using UnityEngine;
using UnityEngine.UI;

public class MainMenuUi : GenericWindow
{
    [SerializeField] private Button battleStartButton;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;

    private WindowManager windowManager;

    private void Start()
    {
        battleStartButton.onClick.AddListener(OnBattleStartButton);
    }


    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    private void OnBattleStartButton()
    {
        Close();
        gameManager.StartGameStage(1630);

        if (windowManager != null)
        {
            windowManager.Open(WindowType.Battle);
        }
        else if (battleUi != null)
        {
            battleUi.Open();
        }
    }
}
