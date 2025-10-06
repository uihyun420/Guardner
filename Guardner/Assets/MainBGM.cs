using UnityEngine;

public class MainBGM : MonoBehaviour
{
    private void Start()
    {
        SoundManager.soundManager.PlayMainBGM("GameStartBGM");
    }
}
