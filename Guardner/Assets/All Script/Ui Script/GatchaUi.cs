using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GatchaUi : GenericWindow
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI guardnerDicriptText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI levelText;
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



}
