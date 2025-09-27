using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUi : GenericWindow
{
    [SerializeField] private Button battleStartButton;
    [SerializeField] private Button DictionaryButton;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private DictionaryUi dictionaryUi;
    [SerializeField] private Button EnhanceButton;
    [SerializeField] private GuardnerEnhanceUi guardnerEnhanceUi;
    [SerializeField] private TextMeshProUGUI goldText;

    public int mainUiGold = 0;

    private WindowManager windowManager;

    private void Start()
    {
        battleStartButton.onClick.AddListener(OnBattleStartButton);
        DictionaryButton.onClick.AddListener(OnClickDictionaryButton);
        EnhanceButton.onClick.AddListener(OnClickEnhanceButton);
    }
    private void Update()
    {
        SetMainUiGoldText();
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

    private void OnClickDictionaryButton()
    {
        dictionaryUi.Open();
    }

    public void AddMainUiGold(int amount)
    {
        mainUiGold += amount;
    }
    public void SetMainUiGoldText()
    {
        var sb = new StringBuilder();
        sb.Append(mainUiGold).Append(" G");
        goldText.text = sb.ToString();
    }
    public void ResetMainUiGold()
    {
        mainUiGold = 0;
    }

    public void OnClickEnhanceButton()
    {
        guardnerEnhanceUi.Open();
    }

}
