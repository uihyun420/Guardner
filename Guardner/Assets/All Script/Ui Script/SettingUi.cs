using UnityEngine;
using UnityEngine.UI;

public class SettingUi : GenericWindow
{
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button GameExitButton;
    [SerializeField] private MainMenuUi mainMenuUi;
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private GuardnerSpawner guardnerSpawner;
    [SerializeField] private StageManager stageManager;

    private void Awake()
    {
        ExitButton.onClick.AddListener(OnClickExitButton);
        GameExitButton.onClick.AddListener(Application.Quit);
        MainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    public override void Open()
    {
        Time.timeScale = 0;
        base.Open();
    }

    public override void Close()
    {
        Time.timeScale = 1;        
        base.Close();
    }

    private void OnClickExitButton()
    {
        Close();
    }

    private void OnClickMainMenuButton()
    {
        Time.timeScale = 1;

        if (stageManager != null)
        {
            stageManager.StageStop();
        }

        if (monsterSpawner != null)
        {
            monsterSpawner.StopAllCoroutines();
            monsterSpawner.gameObject.SetActive(false);
        }

        if (battleUi != null)
        {
            battleUi.CompleteReset();
        }

        if (windowManager != null)
        {
            windowManager.Open(WindowType.MainMenuUi);
        }

        if (mainMenuUi != null)
        {
            mainMenuUi.Open();
        }

        Close();
    }

}
