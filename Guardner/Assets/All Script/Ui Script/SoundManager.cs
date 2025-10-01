using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource mainBGM;
    [SerializeField] private AudioSource battleBGM;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> bgmClip = new List<AudioClip>();
    [SerializeField] private List<AudioClip> sfxClip = new List<AudioClip>();

    [Header("Volume Settings")]
    [SerializeField] private float masterVolume = 1.0f;
    [SerializeField] private float bgmVolume = 0.7f;
    [SerializeField] private float sfxVolume = 1.0f;

    [Header("Mute Settings")]
    [SerializeField] private bool isMuted = false;
    [SerializeField] private bool isBgmMuted = false;
    [SerializeField] private bool isSfxMuted = false;

    [Header("UI Sliders (Optional)")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("UI Toggle Buttons (Optional)")]
    [SerializeField] private Button masterMuteButton;
    [SerializeField] private Button bgmMuteButton;
    [SerializeField] private Button sfxMuteButton;

    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    private AudioSource currentBGMSource;

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSoundManager()
    {
        // SFX AudioSource가 없다면 생성
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // 현재 BGM 소스 설정 (기본적으로 mainBGM 사용)
        currentBGMSource = mainBGM;

        // 딕셔너리 초기화
        InitializeAudioDictionaries();

        // UI 슬라이더 초기화
        InitializeUI();

        // 저장된 설정 불러오기
        LoadSoundSettings();

        // 볼륨 적용
        UpdateVolumes();
    }

    private void InitializeAudioDictionaries()
    {
        bgmDictionary.Clear();
        sfxDictionary.Clear();

        // BGM 딕셔너리 생성
        foreach (var clip in bgmClip)
        {
            if (clip != null && !bgmDictionary.ContainsKey(clip.name))
            {
                bgmDictionary.Add(clip.name, clip);
            }
        }

        // SFX 딕셔너리 생성
        foreach (var clip in sfxClip)
        {
            if (clip != null && !sfxDictionary.ContainsKey(clip.name))
            {
                sfxDictionary.Add(clip.name, clip);
            }
        }
    }

    private void InitializeUI()
    {
        // 마스터 볼륨 슬라이더 연결
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.value = masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        // BGM 볼륨 슬라이더 연결
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.value = bgmVolume;
            bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        // SFX 볼륨 슬라이더 연결
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // 음소거 버튼 연결
        if (masterMuteButton != null)
        {
            masterMuteButton.onClick.AddListener(ToggleMasterMute);
        }

        if (bgmMuteButton != null)
        {
            bgmMuteButton.onClick.AddListener(ToggleBGMMute);
        }

        if (sfxMuteButton != null)
        {
            sfxMuteButton.onClick.AddListener(ToggleSFXMute);
        }
    }

    public void PlayMainBGM(string clipName)
    {
        Debug.Log($"PlayMainBGM 호출됨: {clipName}, 호출 위치: {System.Environment.StackTrace}");

        if (mainBGM != null && mainBGM.clip != null && mainBGM.clip.name == clipName && mainBGM.isPlaying)
            return;

        if (battleBGM != null && battleBGM.isPlaying)
            battleBGM.Stop();

        currentBGMSource = mainBGM;
        PlayBGM(clipName, mainBGM);
    }

    public void PlayBattleBGM(string clipName)
    {
        Debug.Log($"PlayBattleBGM 호출됨: {clipName}, 호출 위치: {System.Environment.StackTrace}");

        if (battleBGM != null && battleBGM.clip != null && battleBGM.clip.name == clipName && battleBGM.isPlaying)
            return;

        if (mainBGM != null && mainBGM.isPlaying)
            mainBGM.Stop();

        currentBGMSource = battleBGM;
        PlayBGM(clipName, battleBGM);
    }

    private void PlayBGM(string clipName, AudioSource audioSource)
    {
        if (isMuted || isBgmMuted || audioSource == null) return;

        if (bgmDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM 클립을 찾을 수 없습니다: {clipName}");
        }
    }

    public void StopBGM()
    {
        if (mainBGM != null) mainBGM.Stop();
        if (battleBGM != null) battleBGM.Stop();
    }

    public void StopMainBGM()
    {
        if (mainBGM != null) mainBGM.Stop();
    }

    public void StopBattleBGM()
    {
        if (battleBGM != null) battleBGM.Stop();
    }

    public void PauseBGM()
    {
        if (currentBGMSource != null) currentBGMSource.Pause();
    }

    public void ResumeBGM()
    {
        if (!isMuted && !isBgmMuted && currentBGMSource != null)
        {
            currentBGMSource.UnPause();
        }
    }

    public void PlaySFX(string clipName)
    {
        if (isMuted || isSfxMuted || sfxSource == null) return;

        if (sfxDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX 클립을 찾을 수 없습니다: {clipName}");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (isMuted || isSfxMuted || clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        UpdateSliderValues();
        SaveSoundSettings();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        UpdateSliderValues();
        SaveSoundSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        UpdateSliderValues();
        SaveSoundSettings();
    }

    private void UpdateVolumes()
    {
        float finalMasterVolume = isMuted ? 0 : masterVolume;
        float finalBGMVolume = (isMuted || isBgmMuted) ? 0 : bgmVolume;
        float finalSFXVolume = (isMuted || isSfxMuted) ? 0 : sfxVolume;

        if (mainBGM != null)
        {
            mainBGM.volume = finalMasterVolume * finalBGMVolume;
        }

        if (battleBGM != null)
        {
            battleBGM.volume = finalMasterVolume * finalBGMVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = finalMasterVolume * finalSFXVolume;
        }
    }

    private void UpdateSliderValues()
    {
        if (masterVolumeSlider != null && masterVolumeSlider.value != masterVolume)
        {
            masterVolumeSlider.value = masterVolume;
        }

        if (bgmVolumeSlider != null && bgmVolumeSlider.value != bgmVolume)
        {
            bgmVolumeSlider.value = bgmVolume;
        }

        if (sfxVolumeSlider != null && sfxVolumeSlider.value != sfxVolume)
        {
            sfxVolumeSlider.value = sfxVolume;
        }
    }
    public void ToggleMasterMute()
    {
        isMuted = !isMuted;
        UpdateVolumes();
        SaveSoundSettings();
    }

    public void ToggleBGMMute()
    {
        isBgmMuted = !isBgmMuted;
        UpdateVolumes();
        SaveSoundSettings();
    }

    public void ToggleSFXMute()
    {
        isSfxMuted = !isSfxMuted;
        UpdateVolumes();
        SaveSoundSettings();
    }

    public void SetMasterMute(bool mute)
    {
        isMuted = mute;
        UpdateVolumes();
        SaveSoundSettings();
    }

    public void SetBGMMute(bool mute)
    {
        isBgmMuted = mute;
        UpdateVolumes();
        SaveSoundSettings();
    }

    public void SetSFXMute(bool mute)
    {
        isSfxMuted = mute;
        UpdateVolumes();
        SaveSoundSettings();
    }

    private void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.SetInt("IsBGMMuted", isBgmMuted ? 1 : 0);
        PlayerPrefs.SetInt("IsSFXMuted", isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSoundSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        isBgmMuted = PlayerPrefs.GetInt("IsBGMMuted", 0) == 1;
        isSfxMuted = PlayerPrefs.GetInt("IsSFXMuted", 0) == 1;
    }

    public float GetMasterVolume() => masterVolume;
    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;
    public bool IsMuted() => isMuted;
    public bool IsBGMMuted() => isBgmMuted;
    public bool IsSFXMuted() => isSfxMuted;
    public bool IsPlayingBGM() => (mainBGM != null && mainBGM.isPlaying) || (battleBGM != null && battleBGM.isPlaying);

}