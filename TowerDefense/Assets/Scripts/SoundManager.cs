using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Tower Sounds")]
    public AudioClip machineGunShot;
    public AudioClip rocketLaunch;
    public AudioClip laserShoot;
    public AudioClip explosionSound;
    public AudioClip enemyDeathSound;
    public AudioClip upgradeSound;
    public AudioClip buildSound;
    public AudioClip mainMenuMusic;

    private AudioSource musicAudioSource;
    private AudioSource sfxAudioSource;
    public AudioSource laserAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeAudioSources()
    {
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        laserAudioSource = gameObject.AddComponent<AudioSource>();

        musicAudioSource.loop = true;
        laserAudioSource.loop = true;
    }

    public void PlayMainMenuMusic()
    {
        if (!musicAudioSource.isPlaying && mainMenuMusic != null)
        {
            musicAudioSource.clip = mainMenuMusic;
            musicAudioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        sfxAudioSource.volume = value;
        laserAudioSource.volume = value;
    }

    public void SetMusicVolume(float value)
    {
        musicAudioSource.volume = value;
    }

    public void PlayMachineGunShot() => PlaySound(machineGunShot);
    public void PlayRocketLaunch() => PlaySound(rocketLaunch);

    public void PlayLaserShoot()
    {
        if (laserShoot != null && !laserAudioSource.isPlaying)
        {
            laserAudioSource.clip = laserShoot;
            laserAudioSource.Play();
        }
    }

    public void StopLaserShoot()
    {
        if (laserAudioSource.isPlaying)
        {
            laserAudioSource.Stop();
        }
    }

    public void PlayExplosionSound() => PlaySound(explosionSound);
    public void PlayEnemyDeathSound() => PlaySound(enemyDeathSound);
    public void PlayUpgradeSound() => PlaySound(upgradeSound);
    public void PlayBuildSound() => PlaySound(buildSound);

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
}
