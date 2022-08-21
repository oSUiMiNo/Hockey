using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
//using System;

public class RoomDoorWay : MonoBehaviourPunCallbacks
{
    public static RoomDoorWay instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private GameManager gameManager = null;
    public GameData gameData;
    public bool tryingConectingToMasterServer = false;
    public bool isConectedToMasterServer = false;
    public bool ready = false;

    public GameObject avatar0 = null;
    public GameObject avatar1 = null;

    private void Start()
    {
        gameManager = GameManager.instance;
        Random.InitState(System.DateTime.Now.Millisecond);
        //gameData = Resources.Load<GameData>("GameData_Instance");

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        ConnectToMasterServer();
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }

    public void ConnectToMasterServer()
    {
        tryingConectingToMasterServer = true;
        isConectedToMasterServer = false;
        Debug.Log("マスターサーバーへの接続を試みます");
        PhotonNetwork.ConnectUsingSettings();
    }
    public void Join()
    {
        ready = false;
      
        //後で実装*****************************************************
        //PhotonNetwork.NickName = gameManager.playerName;
        //後で実装*****************************************************

        Debug.Log("ランダムなルームへの参加を試みます");
        PhotonNetwork.JoinRandomRoom();  // ランダムなルームに参加する
    }

    public void Leave()
    {
        Debug.Log("ルームからの退出を試みます");
        PhotonNetwork.LeaveRoom();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターサーバーに接続しました");
        tryingConectingToMasterServer = false;
        isConectedToMasterServer = true;

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        Join();
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Photonサーバーから切断しました");
        Debug.Log(cause);
        tryingConectingToMasterServer = true;
        isConectedToMasterServer = false;
        Debug.Log("再度マスターサーバーへの接続を試みます");
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ルームが無いので作成します");
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;  // ルームの参加人数を2人に設定する
        PhotonNetwork.CreateRoom(null, roomOptions);  // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
    }


    private bool masterIsWhite = false;
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加しました");

        //後で実装*****************************************************
        //GivePlayers();
        //後で実装*****************************************************

        int randomNumber = Random.Range(0, 2);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ランダムな数値" + randomNumber);
            PhotonNetwork.InstantiateRoomObject("ChessSet", Vector3.zero, Quaternion.identity);

            if (randomNumber == 0) SetMasterColor(true);
            else SetMasterColor(false);

            GetMasterColor(); if (!gotMasterColor) return;
            if (GetMasterColor())
            {
                Debug.Log("マスターの色は白 OnJoinedRoom");
                avatar0 = PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 1f, -80f), Quaternion.identity);
             
                Debug.Log(avatar0);
                avatar0.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("マスターの色は黒 OnJoinedRoom");
                avatar1 = PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 1f, 80f), Quaternion.identity);
               
                Debug.Log(avatar1);
                avatar1.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
        }
        else
        {
            GetMasterColor(); if (!gotMasterColor) return;
            if (GetMasterColor())
            {
                Debug.Log("自分の色は黒 OnJoinedRoom");
                avatar1 = PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 1f, 80f), Quaternion.identity);
            }
            else
            {
                Debug.Log("自分の色は白 OnJoinedRoom");
                avatar0 = PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 1f, -80f), Quaternion.identity);
            }
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("ルームから退出しました");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer + " が参加しました");

        //後で実装*****************************************************
        //GivePlayers();
        //後で実装*****************************************************

        if (GetMasterColor()) avatar0.GetComponent<Avatar>().photonView.RPC(nameof(AllowAction), RpcTarget.All);
        else avatar1.GetComponent<Avatar>().photonView.RPC(nameof(AllowAction), RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer + " が退出しました");

        //後で実装*****************************************************
        //GivePlayers();
        //後で実装*****************************************************

        Debug.Log("退出します");
        Leave();
        //後で実装*****************************************************
        //gameManager.Loard_Menu();
        //後で実装*****************************************************
    }



    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //Debug.Log("マスターが白かどうか " + GetMasterColor());
        if (PhotonNetwork.IsMasterClient)
        {
            if (GetMasterColor())
            {
                Debug.Log("マスターの色は白 OnRoomPropertiesUpdate");
                avatar0 = PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 1f, -80f), Quaternion.identity);
                
                Debug.Log(avatar0);
                avatar0.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("マスターの色は黒 OnRoomPropertiesUpdate");
                avatar1 = PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 1f, 80f), Quaternion.identity);
                
                Debug.Log(avatar1);
                avatar1.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
        }
        else
        {
            if (GetMasterColor())
            {
                Debug.Log("自分の色は黒 OnRoomPropertiesUpdate");
                avatar1 = PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 1f, 80f), Quaternion.identity);
            }
            else
            {
                Debug.Log("自分の色は白 OnRoomPropertiesUpdate");
                avatar0 = PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 1f, -80f), Quaternion.identity);
            }
        }

        ready = true;
        Debug.Log("Ready = true");
    }


    public Player[] GetPlayers() { return PhotonNetwork.PlayerList; }

    //後で実装*****************************************************
    //public void GivePlayers() { gameManager.players = PhotonNetwork.PlayerList; }
    //後で実装*****************************************************

    [PunRPC]
    private void MasterIsWhite() { masterIsWhite = true; }

    [PunRPC]
    public void LockAction(GameObject masterAvatar)
    {
        Debug.Log("LockAction");
        if (masterAvatar.GetPhotonView().Owner.IsMasterClient)
        {
            //後で実装*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = false;
            //後で実装*****************************************************
        }
    }

    [PunRPC]
    public void AllowAction(GameObject masterAvatar)
    {
        if (masterAvatar.GetPhotonView().Owner.IsMasterClient)
        {
            //後で実装*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = true;
            //後で実装*****************************************************
        }
    }

    private static Hashtable prop = new Hashtable();
    private static bool gotMasterColor = false;
    public static void SetMasterColor(bool isMaster)
    {
        Debug.Log("SetMasterColor");
        prop["masterIsBlue"] = isMaster;
        prop["GotMasterColor"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        prop.Clear();
    }
    public static bool GetMasterColor()
    {
        Debug.Log("GetMasterColor");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["masterIsBlue"]);
        gotMasterColor = (PhotonNetwork.CurrentRoom.CustomProperties["GotMasterColor"] is bool got) ? got : false;
        return (PhotonNetwork.CurrentRoom.CustomProperties["masterIsBlue"] is bool isMaster) ? isMaster : false;
    }
}