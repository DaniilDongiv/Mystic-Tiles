using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundWaveConductor : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioBlender;
    [SerializeField] private Slider _soundscapeSlider;
    [SerializeField] private Slider _melodySlider;
    
    private const string SoundscapeVolumeKey = "SoundscapeVolume";
    private const string MelodyVolumeKey = "MelodyVolume";

    private void Start()
    {
        var savedSoundscapeVolume = PlayerPrefs.GetFloat(SoundscapeVolumeKey, 0.8f);
        var savedMelodyVolume = PlayerPrefs.GetFloat(MelodyVolumeKey, 0.8f);

        _audioBlender.SetFloat(AvatarPhaseSetup.REVERB_TRACE, savedSoundscapeVolume);
        _audioBlender.SetFloat(AvatarPhaseSetup.AUDIO_SIGNAL_POINT, savedMelodyVolume);
        
        _soundscapeSlider.value = savedSoundscapeVolume;
        _melodySlider.value = savedMelodyVolume;
    }

    public void AmbientCurator(float volume)
    {
        _audioBlender.SetFloat(AvatarPhaseSetup.REVERB_TRACE, volume);
        PlayerPrefs.SetFloat(SoundscapeVolumeKey, volume);
    }

    public void HarmonySynth(float volume)
    {
        _audioBlender.SetFloat(AvatarPhaseSetup.AUDIO_SIGNAL_POINT, volume);
        PlayerPrefs.SetFloat(MelodyVolumeKey, volume);
    }
    
    public void PreserveConfigurations()
    {
        PlayerPrefs.Save();
    }
}