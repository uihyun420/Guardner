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

    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text bgmVolumeText;
    [SerializeField] private Text sfxVolumeText;

    [SerializeField] private Button bgmMuteButton;
    [SerializeField] private Button sfxMuteButton;

    private void Awake()
    {
        ExitButton.onClick.AddListener(OnClickExitButton);
        GameExitButton.onClick.AddListener(Application.Quit);
        MainMenuButton.onClick.AddListener(OnClickMainMenuButton);

        // 볼륨 컨트롤 초기화 추가
        InitializeVolumeControls();

        if (bgmMuteButton != null)
            bgmMuteButton.onClick.AddListener(OnClickBgmMuteButton);
        if (sfxMuteButton != null)
            sfxMuteButton.onClick.AddListener(OnClickSfxMuteButton); // 필요시 SFX도 동일하게 구현
    }

    private void InitializeVolumeControls()
    {
        // BGM 볼륨 슬라이더 설정
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        // SFX 볼륨 슬라이더 설정
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    public override void Open()
    {
        Time.timeScale = 0;
        base.Open();

        // 창이 열릴 때마다 현재 볼륨 값으로 슬라이더 업데이트
        UpdateSliderValues();
    }

    public override void Close()
    {
        Time.timeScale = 1;
        base.Close();
    }

    private void OnClickExitButton()
    {
        SoundManager.soundManager.PlaySfxButton1();
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
        SoundManager.soundManager.PlaySfxButton1();
        Close();
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (SoundManager.soundManager != null)
        {
            SoundManager.soundManager.SetBGMVolume(value);
            UpdateVolumeTexts();
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.soundManager != null)
        {
            SoundManager.soundManager.SetSFXVolume(value);
            UpdateVolumeTexts();
        }
    }

    private void UpdateSliderValues()
    {
        if (SoundManager.soundManager == null) return;

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = SoundManager.soundManager.GetBGMVolume();
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = SoundManager.soundManager.GetSFXVolume();
        }

        UpdateVolumeTexts();
    }

    private void UpdateVolumeTexts()
    {
        if (bgmVolumeText != null && bgmVolumeSlider != null)
        {
            bgmVolumeText.text = $"BGM: {Mathf.RoundToInt(bgmVolumeSlider.value * 100)}%";
        }

        if (sfxVolumeText != null && sfxVolumeSlider != null)
        {
            sfxVolumeText.text = $"SFX: {Mathf.RoundToInt(sfxVolumeSlider.value * 100)}%";
        }
    }

    private void OnClickBgmMuteButton()
    {
        if (SoundManager.soundManager != null)
        {
            // 현재 상태를 반전시켜서 적용
            bool isMuted = SoundManager.soundManager.IsBGMMuted();
            SoundManager.soundManager.SetBGMMute(!isMuted);
        }
    }

    private void OnClickSfxMuteButton()
    {
        if (SoundManager.soundManager != null)
        {
            // 현재 상태를 반전시켜서 적용
            bool isMuted = SoundManager.soundManager.IsSFXMuted();
            SoundManager.soundManager.IsSFXMuted();
        }
    }

}