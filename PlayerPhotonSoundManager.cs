using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerPhotonSoundManager : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip footstepClip1;
    public AudioClip footstepClip2;


    public AudioSource gunSource;
    public AudioClip[] allGunClips; 
    
    
    public void PlaySound()
    {
        GetComponent<PhotonView>().RPC("PlayFootstepSound", RpcTarget.All);
    }


    [PunRPC]
    public void PlayFootstepSound()
    {
        int random = Random.Range(1, 3);
        
        footstepSource.clip = random == 1 ? footstepClip1 : footstepClip2;

        footstepSource.pitch = Random.Range(0.8f, 1f);
        footstepSource.volume = Random.Range(0.2f, 0.4f);

        footstepSource.Play();
    }

    public void PlayShootSound(int index)
    {
        GetComponent<PhotonView>().RPC("PlayShootSoundRPC", RpcTarget.All, index);
    }

    [PunRPC]
    public void PlayShootSoundRPC(int index)
    {
        gunSource.clip = allGunClips[index];

        gunSource.volume = 0.1f;

        gunSource.Play();
    }

}
