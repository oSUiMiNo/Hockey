using System.Collections;
using System.Collections.Generic;
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
    public GameObject hockeySet = null;


    private void Start()
    {
        gameManager = GameManager.instance;
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    public void ConnectToMasterServer()
    {
        tryingConectingToMasterServer = true;
        isConectedToMasterServer = false;
        Debug.Log("マスターサーバーへの接続を試みます");
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("A");
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
        Debug.Log("connected");
        Debug.Log("マスターサーバーに接続しました");
        tryingConectingToMasterServer = false;
        isConectedToMasterServer = true;
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


    private bool masterIsBlue = false;
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
           

            if (randomNumber == 0) SetMasterColor(true);
            else SetMasterColor(false);

            StartCoroutine(CreateAvatar());
        }
        else
        {
            Debug.Log("自分は参加者1");
            StartCoroutine(CreateAvatar());
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

       // if (GetMasterColor()) avatar0.GetComponent<Avatar>().photonView.RPC(nameof(AllowAction), RpcTarget.All);
        //else avatar1.GetComponent<Avatar>().photonView.RPC(nameof(AllowAction), RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer + " が退出しました");

        //後で実装*****************************************************
        //GivePlayers();
        //後で実装*****************************************************

        Debug.Log("退出します");
        Leave();
        gameManager.Load_Menu();
    }

    private IEnumerator CreateAvatar()
    {
        yield return new WaitUntil(() => creatable);
        //Debug.Log("マスターが白かどうか " + GetMasterColor());
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("生成1");
            if (avatar0 || avatar1) yield return null;
            Debug.Log("生成2");
            if (GetMasterColor())
            {
                Debug.Log("マスターの色は白 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 5f, -80f), Quaternion.identity);

                //avatar0.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("マスターの色は黒 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));

                //avatar1.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            //PhotonNetwork.InstantiateRoomObject("Ball", Vector3.zero, Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("Ball", new Vector3(0f, 10f, 0f), Quaternion.identity);
        }
        else
        {
            Debug.Log("自分は参加者2");
            Debug.Log(GetMasterColor());
            if (GetMasterColor())
            {
                Debug.Log("自分は参加者3");
                Debug.Log("自分の色は黒 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));
            }
            else
            {
                Debug.Log("自分は参加者4");
                Debug.Log("自分の色は白 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 5f, -80f), Quaternion.identity);
            }
        }

    }


    private bool creatable = false;
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log(GetMasterColor());
        if (!gotMasterColor) return;
        creatable = true;
        gotMasterColor = false;
    }


    public Player[] GetPlayers() { return PhotonNetwork.PlayerList; }

    public void GivePlayers() { gameManager.players = PhotonNetwork.PlayerList; }


    [PunRPC]
    private void MasterIsBlue() { masterIsBlue = true; }

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

            if (GameObject.Find("FirstFloor") != null)
                GameObject.Find("FirstFloor").SetActive(false);
        }
    }

    private static ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
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


    //private void Update()
    //{
    //   // Ready();
    //}

    public bool Ready()
    {
        if (!avatar0) return false;
        if (!avatar1) return false;
        if (!hockeySet) return false;

        ready = true;
        return true;
    }
}