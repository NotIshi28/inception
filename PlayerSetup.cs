using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;

    public GameObject camera;
    public GameObject remoteCamera;

    public TextMeshPro nameText;

    public string playerName;

    public Transform TPWeaponHolder;

    private float setupDelay = 0.5f;
    public void isLocalPlayer()
    {
        TPWeaponHolder.gameObject.SetActive(false);
        movement.enabled = true;
        camera.SetActive(false);

        StartCoroutine(DelayedRemoteCameraSetup());
    }

    private IEnumerator DelayedRemoteCameraSetup()
    {
        yield return new WaitForSeconds(setupDelay);
        SetupRemoteCameras();
        
        if (!FindAndActivateRemoteCamera())
        {
            StartCoroutine(RetryFindRemoteCamera());
        }
    }
    
    private IEnumerator RetryFindRemoteCamera()
    {
        int maxAttempts = 10;
        int attempts = 0;
        
        while (attempts < maxAttempts)
        {
            yield return new WaitForSeconds(1f);
            attempts++;
            
            Debug.Log("Retrying to find remote camera, attempt " + attempts);
            if (FindAndActivateRemoteCamera())
            {
                Debug.Log("Successfully found remote camera on retry " + attempts);
                break;
            }
            
            if (attempts == maxAttempts)
            {
                Debug.LogError("Failed to find any remote cameras after " + maxAttempts + " attempts");
            }
        }
    }

    private bool FindAndActivateRemoteCamera()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        bool foundRemoteCamera = false;

        foreach (GameObject otherPlayer in allPlayers)
        {
            PhotonView pv = otherPlayer.GetComponent<PhotonView>();
            
            if (pv != null && pv.IsMine) continue;

            PlayerSetup otherSetup = otherPlayer.GetComponent<PlayerSetup>();
            if (otherSetup != null)
            {
                if (otherSetup.remoteCamera == null)
                {
                    Debug.LogError("Remote camera is null on player: " + otherSetup.playerName);
                    continue;
                }
                
                otherSetup.remoteCamera.SetActive(true);
                Debug.Log("Remote camera activated for: " + otherSetup.playerName);
                foundRemoteCamera = true;
                break;
            }
        }
        
        return foundRemoteCamera;
    }

    public void isRemotePlayer()
    {
        Debug.Log("Remote player active: " + playerName);
        TPWeaponHolder.gameObject.SetActive(false);
        movement.enabled = false;
        camera.SetActive(false);
        
        if (remoteCamera != null)
        {
            remoteCamera.SetActive(false); 
            Debug.Log("Remote camera ready for: " + playerName);
        }
        else
        {
            Debug.LogError("Remote camera reference is missing for: " + playerName);
        }
    }

    private void SetupRemoteCameras()
    {
        if (!FindAndActivateRemoteCamera())
        {
            Debug.LogWarning("No remote cameras found, activating own remote camera as fallback");
            if (remoteCamera != null)
            {
                remoteCamera.SetActive(true);
            }
        }
    }

    [PunRPC]
    public void setTPWeapon(int _weaponIndex)
    {
        foreach(Transform weapon in TPWeaponHolder)
        {
            weapon.gameObject.SetActive(false);
        }

        TPWeaponHolder.GetChild(_weaponIndex).gameObject.SetActive(true);
    }

    [PunRPC]
    public void SetName(string _name)
    {
        playerName = _name;
        nameText.text=_name;
    }

}
