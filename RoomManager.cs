using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;

public class RoomManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{

    public static RoomManager instance;
    public GameObject player;
    public Transform[] spawnPoints;

    public GameObject roomCam;

    public GameObject nameUI;
    public GameObject connectingUI;

    [HideInInspector]
    public int kills = 0;
    public int deaths = 0;
    
    public string roomNameTojoin = "room";
    private string playerName="WigglySquid123";

    private float playerUpdateTimer = 0f;
    private const float PLAYER_UPDATE_INTERVAL = 1.0f;

    private void Update()
    {
        if(!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            return;
        }

        playerUpdateTimer += Time.deltaTime;

        if (playerUpdateTimer >= PLAYER_UPDATE_INTERVAL)
        {
            playerUpdateTimer = 0f;
            UpdatePlayerObjects();
        }
    }

    private void UpdatePlayerObjects()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Updating player objects: " + playerObjects.Length);
        
        foreach(GameObject playerObj in playerObjects)
        {
            PhotonView photonView = playerObj.GetComponent<PhotonView>();
            
            // Set up the current player
            if (photonView != null && photonView.IsMine)
            {
                playerObj.GetComponent<PlayerSetup>().isLocalPlayer();
            }
            else if (photonView != null)
            {
                playerObj.GetComponent<PlayerSetup>().isRemotePlayer();
            }
        }
    }

    public void ChangeName(string _name)
    {
        playerName = _name;
    }

    public void JoinButtonPressed()
    {
        Debug.Log("Connecting to server...");
        PhotonNetwork.JoinOrCreateRoom(roomNameTojoin, new Photon.Realtime.RoomOptions { MaxPlayers = 5 }, null);

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    void Awake()
    {
        instance = this;   
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("Joined room");

        roomCam.SetActive(false);

        RespawnPlayer();

        SetupExistingPlayer();
    }

    public void RespawnPlayer()
    {

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().isLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;
        _player.GetComponent<PhotonView>().RPC("SetName", RpcTarget.AllBuffered, playerName);

        PhotonNetwork.LocalPlayer.NickName = playerName;
    }

    private void SetupExistingPlayer()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObj in playerObjects)
        {
            PhotonView photonView = playerObj.GetComponent<PhotonView>();

            if(photonView != null && !photonView.IsMine)
            {
                playerObj.GetComponent<PlayerSetup>().isRemotePlayer();
            }

            if(photonView != null && photonView.IsMine)
            {
                playerObj.GetComponent<PlayerSetup>().isLocalPlayer();
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        GameObject playerObj = info.photonView.gameObject;

        if(!info.photonView.IsMine)
        {
            playerObj.GetComponent<PlayerSetup>().isRemotePlayer();
        }

        SetupExistingPlayer();
    }
    
    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        catch
        {

        }
    }

}
