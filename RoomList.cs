using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public class RoomList : MonoBehaviourPunCallbacks
{

    public static RoomList instance;

    public GameObject roomManagerGameObject;
    public RoomManager roomManager;

    //UI
    public Transform roomListParent;
    public GameObject roomListItemPrefab;
    // public GameObject joinButton;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void Awake()
    {
        instance = this;   
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }


    public void ChangeRoomToCreateName(string _roomName)
    {
        roomManager.roomNameTojoin = _roomName;
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach(var room in roomList)
            {
                for(int i = 0; i < cachedRoomList.Count; i++)
                {
                    if(room.Name == cachedRoomList[i].Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;
                        

                        if(room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }

                        cachedRoomList = newList;
                    }
                }
            }
        }
        
        UpdateUI();

    }

    void UpdateUI()
    {
        foreach(Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach(var room in cachedRoomList)
        {
            // Instantiate(roomListItemPrefab, roomListParent).GetComponent<RoomList>().SetUp(room);

            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            roomItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = room.Name;

            if(room.PlayerCount == 5)
            {
                roomItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Full!";
            }
            else
            {
                roomItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = room.PlayerCount + " / 5";
            }
     
            roomItem.GetComponent<RoomItemButton>().RoomName = room.Name;
        }
    }

    public void JoinRoomByName(string _roomName)
    {
        // PhotonNetwork.JoinRoom(_roomName);
        roomManager.roomNameTojoin = _roomName;
        roomManagerGameObject.SetActive(true);

        gameObject.SetActive(false);
    }

}
