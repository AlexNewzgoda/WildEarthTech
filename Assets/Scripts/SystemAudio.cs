using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemAudio : MonoBehaviour
{
    private static SystemAudio _instance;
    public static SystemAudio Instance => _instance;
    public bool Singletone = true;

    private void Awake()
    {
        if (Singletone)
        {
            if (_instance != null)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public AudioSource Source;

    public static void PlayOneShot(AudioClip audioClip, float volume)
    {
        Instance.Source.PlayOneShot(audioClip, volume);
    }
}
