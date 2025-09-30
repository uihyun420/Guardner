using UnityEngine;
using UnityEngine.UI;

public class SettingUi : GenericWindow
{
    [SerializeField] private Button ExitButton;

    private void Awake()
    {
        ExitButton.onClick.AddListener(OnClickExitButton);
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
}
