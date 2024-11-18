using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;

    private void Start()
    {
        
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
       
        UpdateMasterVolume();
        UpdateSFXVolume();
        UpdateMusicVolume();

        masterVolumeSlider.onValueChanged.AddListener(delegate { UpdateMasterVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });
    }

    public void UpdateMasterVolume()
    {
        if (masterVolumeSlider != null)
        {
            float volume = masterVolumeSlider.value;

            Debug.Log(SoundManager.Instance);

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.SetMasterVolume(volume);
                PlayerPrefs.SetFloat("MasterVolume", volume);
            }
            else
            {
                Debug.LogError("SoundManager Instance is not available!");
            }
        }
        else
        {
            Debug.LogError("Master Volume Slider is not assigned in the Inspector!");
        }
    }

    public void UpdateSFXVolume()
    {
        float volume = sfxVolumeSlider.value;
        SoundManager.Instance.SetSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void UpdateMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        SoundManager.Instance.SetMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void CloseOptions()
    {
        gameObject.SetActive(false);
    }
}
