using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public bool lightVersion;
    public Animator canvas;

    [Header("Дефолтные значения")]
    [SerializeField] private float musicDefault;
    [SerializeField] private float effectsDefault;
    [SerializeField] private float uiDefault;

    [Header("Микшеры звуков")]
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup effectsMixer;
    public AudioMixerGroup uiMixer;

    [Header("Слайдеры настройки")]
    public Slider musicSlider;
    public Slider effectsSlider;
    public Slider uiSlider;

    private float musicValue;
    private float effectsValue;
    private float uiValue;

    [Header("Музыка")]
    private float originallMusic;
    private bool isMusicChange;

    [Header("Эффекты")]
    private float originallEffects;
    private bool isEffectsChange;

    [Header("Интерфейс")]
    private float originallUi;
    private bool isUiChange;

    private void Start()
    {
        ChangeMusic(musicValue);
        ChangeEffect(effectsValue);
        ChangeUI(uiValue);
        if (!lightVersion) {
            SetNewOriginall();
            SetChangesFalse();
        }
    }

    public void CheckChanges()
    {
        var buttonFunc = canvas.GetComponent<ButtonFunctional>();
        if (isEffectsChange || isMusicChange || isUiChange) {
            buttonFunc.SetConfirmPanel("AudioSettings");
        } else {
            buttonFunc.AudioSettings();
        }
    }

    public void ConfirmCancel(bool value)
    {
        if (value)
            ReturnToNormal();
        canvas.SetBool("isConfirm", false);
    }

    public void ChangeMusic(float volume)
    {
        if (!lightVersion)
            musicSlider.value = musicValue;
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, volume));
        isMusicChange = volume != originallMusic;
    }

    public void ChangeEffect(float volume)
    {
        if (!lightVersion)
            effectsSlider.value = effectsValue;
        effectsMixer.audioMixer.SetFloat("EffectsVolume", Mathf.Lerp(-80, 0, volume));
        isEffectsChange = volume != originallEffects;
    }

    public void ChangeUI(float volume)
    {
        if (!lightVersion)
            uiSlider.value = uiValue;
        uiMixer.audioMixer.SetFloat("UIVolume", Mathf.Lerp(-80, 0, volume));
        isUiChange = volume != originallUi;
    }

    public void SetNewOriginall()
    {
        originallMusic = musicSlider.value;
        originallEffects = effectsSlider.value;
        originallUi = uiSlider.value;
    }

    public void SetChangesFalse()
    {
        isEffectsChange = false;
        isMusicChange = false;
        isUiChange = false;
    }

    public void SetDefaults()
    {
        musicSlider.value = musicDefault;
        ChangeMusic(musicDefault);

        effectsSlider.value = effectsDefault;
        ChangeEffect(effectsDefault);

        uiSlider.value = uiDefault;
        ChangeUI(uiDefault);
    }

    public void ReturnToNormal()
    {
        musicSlider.value = originallMusic;
        ChangeMusic(musicDefault);

        effectsSlider.value = originallEffects;
        ChangeEffect(effectsDefault);

        uiSlider.value = originallUi;
        ChangeUI(uiDefault);
    }

    public void SetValues(float volumeMusic, float volumeEffects, float volumeUI)
    {
        musicValue = volumeMusic;
        effectsValue = volumeEffects;
        uiValue = volumeUI;
    }
}
