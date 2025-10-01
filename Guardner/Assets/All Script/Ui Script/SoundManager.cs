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
        // SFX AudioSource�� ���ٸ� ����
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // ���� BGM �ҽ� ���� (�⺻������ mainBGM ���)
        currentBGMSource = mainBGM;

        // ��ųʸ� �ʱ�ȭ
        InitializeAudioDictionaries();

        // UI �����̴� �ʱ�ȭ
        InitializeUI();

        // ����� ���� �ҷ�����
        LoadSoundSettings();

        // ���� ����
        UpdateVolumes();
    }

    private void InitializeAudioDictionaries()
    {
        bgmDictionary.Clear();
        sfxDictionary.Clear();

        // BGM ��ųʸ� ����
        foreach (var clip in bgmClip)
        {
            if (clip != null && !bgmDictionary.ContainsKey(clip.name))
            {
                bgmDictionary.Add(clip.name, clip);
            }
        }

        // SFX ��ųʸ� ����
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
        // ������ ���� �����̴� ����
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.value = masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        // BGM ���� �����̴� ����
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.value = bgmVolume;
            bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        // SFX ���� �����̴� ����
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // ���Ұ� ��ư ����
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
        Debug.Log($"PlayMainBGM ȣ���: {clipName}, ȣ�� ��ġ: {System.Environment.StackTrace}");

        if (mainBGM != null && mainBGM.clip != null && mainBGM.clip.name == clipName && mainBGM.isPlaying)
            return;

        if (battleBGM != null && battleBGM.isPlaying)
            battleBGM.Stop();

        currentBGMSource = mainBGM;
        PlayBGM(clipName, mainBGM);
    }

    public void PlayBattleBGM(string clipName)
    {
        Debug.Log($"PlayBattleBGM ȣ���: {clipName}, ȣ�� ��ġ: {System.Environment.StackTrace}");

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
            Debug.LogWarning($"BGM Ŭ���� ã�� �� �����ϴ�: {clipName}");
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
            Debug.LogWarning($"SFX Ŭ���� ã�� �� �����ϴ�: {clipName}");
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