using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsManager : MonoBehaviour
{
    public static float musicVolume { get; private set; }
    public static float soundEffectsVolume { get; private set; }

    [SerializeField] private Text musicSliderText;
    [SerializeField] private Text soundEffectsSliderText;

    public void OnMusicSliderValuerChange(float value)
    {
        musicVolume = value;
        musicSliderText.text = ((int)(value*100)).ToString();
        AudioManager.Instance.UpdateMixerVolume();
    }
    public void OnSoundEffectsSliderValuerChange(float value)
    {
        soundEffectsVolume = value;
        soundEffectsSliderText.text = ((int)(value*100)).ToString();
        AudioManager.Instance.UpdateMixerVolume();
    }
}
