using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class SendAnimToSoundManager : MonoBehaviour
{
    public PlayerPhotonSoundManager playerPhotonSoundManager;

    public void TriggerFootstepSound()
    {
        playerPhotonSoundManager.PlayFootstepSound();
    }
}
