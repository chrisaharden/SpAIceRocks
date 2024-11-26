using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    private AudioSource backgroundAudio;
    private AudioSource sfxAudio;

    [Header("Background Music")]
    public AudioClip[] backgroundMusics;
    public AudioClip shopBackgroundMusic;
    public int currentMusicIndex = 0;
    private int previousMusicIndex = 0;

    [Header("Game SFX")]
    public AudioClip cashRegisterSound;
    public AudioClip boardClearedSound;
    public AudioClip swapSound;
    public AudioClip matchSound;
    public AudioClip outOfMovesSound;
    public AudioClip[] toolSounds;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            
            // Setup background music audio source
            backgroundAudio = gameObject.AddComponent<AudioSource>();
            backgroundAudio.loop = true;
            backgroundAudio.volume = 0.015f;
            
            // Setup SFX audio source
            sfxAudio = gameObject.AddComponent<AudioSource>();
            sfxAudio.loop = false;
            sfxAudio.volume = 0.079f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusics != null && backgroundMusics.Length > 0)
        {
            AudioClip nextTrack = backgroundMusics[currentMusicIndex];
            if (backgroundAudio.clip != nextTrack)
            {
                backgroundAudio.clip = nextTrack;
                backgroundAudio.Play();
            }
        }
    }

    public void PlayShopBackgroundMusic()
    {
        if (shopBackgroundMusic != null && backgroundAudio != null)
        {
            previousMusicIndex = currentMusicIndex;
            backgroundAudio.clip = shopBackgroundMusic;
            backgroundAudio.Play();
        }
    }

    public void RestorePreviousBackgroundMusic()
    {
        currentMusicIndex = previousMusicIndex;
        PlayBackgroundMusic();
    }

    public void UpdateMusicIndex(int newIndex)
    {
        if (backgroundMusics != null && backgroundMusics.Length > 0)
        {
            currentMusicIndex = newIndex % backgroundMusics.Length;
        }
    }

    public void PlayCashRegisterSound()
    {
        if (sfxAudio != null && cashRegisterSound != null)
        {
            sfxAudio.PlayOneShot(cashRegisterSound);
        }
    }

    public void PlayBoardClearedSound()
    {
        if (sfxAudio != null && boardClearedSound != null)
        {
            sfxAudio.PlayOneShot(boardClearedSound);
        }
    }

    public void PlaySwapSound()
    {
        if (sfxAudio != null && swapSound != null)
        {
            sfxAudio.PlayOneShot(swapSound);
        }
    }

    public void PlayMatchSound()
    {
        if (sfxAudio != null && matchSound != null)
        {
            sfxAudio.PlayOneShot(matchSound);
        }
    }

    public void PlayOutOfMovesSound()
    {
        if (sfxAudio != null && outOfMovesSound != null)
        {
            sfxAudio.PlayOneShot(outOfMovesSound);
        }
    }

    public void PlayToolSound(int toolIndex)
    {
        if (sfxAudio != null && toolSounds != null && toolIndex >= 0 && toolIndex < toolSounds.Length && toolSounds[toolIndex] != null)
        {
            sfxAudio.PlayOneShot(toolSounds[toolIndex]);
        }
    }
}
