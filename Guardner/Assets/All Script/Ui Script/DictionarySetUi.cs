using UnityEngine;
using UnityEngine.UI;

public class DictionarySetUi : GenericWindow
{
    [Header("Button")]
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        isOverlayWindow = true;

        exitButton.onClick.AddListener(() =>
        {
            SoundManager.soundManager.PlaySfxButton1();
            Close();
        });
    }


    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }
    private void Start()
    {
        
        //SoundManager.soundManager.StopBattleBGM();
    }
}
