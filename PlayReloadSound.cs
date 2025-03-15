using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayReloadSound : MonoBehaviour
{
    public AudioSource reloadSource;
    public AudioClip reloadClip;

    public void PlayReload()
    {
        reloadSource.clip = reloadClip;
        reloadSource.volume = 0.1f;
        reloadSource.Play();
    }
}
