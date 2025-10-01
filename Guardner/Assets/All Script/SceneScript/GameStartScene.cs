using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStartScene : MonoBehaviour
{
    [SerializeField] private Button gameStart;
    [SerializeField] private Button gameExit;


    private void Awake()
    {
        gameStart.onClick.AddListener(onClickGameStart);
        gameExit.onClick.AddListener(onClickGameExit);
    }

    public void onClickGameStart()
    {
        SceneManager.LoadScene("GameScene");
        SoundManager.soundManager.PlaySFX("UiClick2Sfx");
    }
    public void onClickGameExit()
    {
        Application.Quit();
    }

}
