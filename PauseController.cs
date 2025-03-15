using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PauseController : MonoBehaviour
{
    public GameObject canvas;
    // public Camera roomCamera;

    public void Start()
    {
        canvas.SetActive(false);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        canvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject localPlayerObject = null;

        PhotonView[] photonViews = FindObjectsByType<PhotonView>(FindObjectsSortMode.None);
        foreach (PhotonView view in photonViews)
        {
            if (view.IsMine && view.Owner.IsLocal)
            {
                localPlayerObject = view.gameObject;
                break;
            }
        }

        if (localPlayerObject != null)
        {
            Movement movement = localPlayerObject.GetComponent<Movement>();
            if (movement != null)
            {   
                Debug.Log(movement.cameraCanMove);
                movement.cameraCanMove = false;
                Debug.Log(movement.cameraCanMove);
            }
        }
    }


    public void Resume()
    {
        canvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameObject localPlayerObject = null;
        
        PhotonView[] photonViews = FindObjectsByType<PhotonView>(FindObjectsSortMode.None);
        foreach (PhotonView view in photonViews)
        {
            if (view.IsMine && view.Owner.IsLocal)
            {
                localPlayerObject = view.gameObject;
                break;
            }
        }

        if (localPlayerObject != null)
        {
            Movement movement = localPlayerObject.GetComponent<Movement>();
            if (movement != null)
            {
                Debug.Log(movement.cameraCanMove);
                movement.cameraCanMove = true;
                Debug.Log(movement.cameraCanMove);
            }
        }
        
    }

    public void Exit()
    {
        Application.Quit();
    }
}
