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
        Debug.Log("�}�X�^�[�T�[�o�[�ւ̐ڑ������݂܂�");
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("A");
    }
    public void Join()
    {
        ready = false;
      
        //��Ŏ���*****************************************************
        //PhotonNetwork.NickName = gameManager.playerName;
        //��Ŏ���*****************************************************

        Debug.Log("�����_���ȃ��[���ւ̎Q�������݂܂�");
        PhotonNetwork.JoinRandomRoom();  // �����_���ȃ��[���ɎQ������
    }

    public void Leave()
    {
        Debug.Log("���[������̑ޏo�����݂܂�");
        PhotonNetwork.LeaveRoom();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("connected");
        Debug.Log("�}�X�^�[�T�[�o�[�ɐڑ����܂���");
        tryingConectingToMasterServer = false;
        isConectedToMasterServer = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Photon�T�[�o�[����ؒf���܂���");
        Debug.Log(cause);
        tryingConectingToMasterServer = true;
        isConectedToMasterServer = false;
        Debug.Log("�ēx�}�X�^�[�T�[�o�[�ւ̐ڑ������݂܂�");
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���[���������̂ō쐬���܂�");
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;  // ���[���̎Q���l����2�l�ɐݒ肷��
        PhotonNetwork.CreateRoom(null, roomOptions);  // �����_���ŎQ���ł��郋�[�������݂��Ȃ��Ȃ�A�V�K�Ń��[�����쐬����
    }


    private bool masterIsBlue = false;
    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ�����܂���");

        //��Ŏ���*****************************************************
        //GivePlayers();
        //��Ŏ���*****************************************************

        int randomNumber = Random.Range(0, 2);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("�����_���Ȑ��l" + randomNumber);
           

            if (randomNumber == 0) SetMasterColor(true);
            else SetMasterColor(false);

            StartCoroutine(CreateAvatar());
        }
        else
        {
            Debug.Log("�����͎Q����1");
            StartCoroutine(CreateAvatar());
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("���[������ޏo���܂���");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer + " ���Q�����܂���");

        //��Ŏ���*****************************************************
        //GivePlayers();
        //��Ŏ���*****************************************************

       // if (GetMasterColor()) avatar0.GetComponent<Avatar>().photonView.RPC(nameof(AllowAction), RpcTarget.All);
        //else avatar1.GetComponent<Avatar>().photonView.RPC(nameof(AllowAction), RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer + " ���ޏo���܂���");

        //��Ŏ���*****************************************************
        //GivePlayers();
        //��Ŏ���*****************************************************

        Debug.Log("�ޏo���܂�");
        Leave();
        gameManager.Load_Menu();
    }

    private IEnumerator CreateAvatar()
    {
        yield return new WaitUntil(() => creatable);
        //Debug.Log("�}�X�^�[�������ǂ��� " + GetMasterColor());
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("����1");
            if (avatar0 || avatar1) yield return null;
            Debug.Log("����2");
            if (GetMasterColor())
            {
                Debug.Log("�}�X�^�[�̐F�͔� OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 5f, -80f), Quaternion.identity);

                //avatar0.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("�}�X�^�[�̐F�͍� OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));

                //avatar1.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            //PhotonNetwork.InstantiateRoomObject("Ball", Vector3.zero, Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("Ball", new Vector3(0f, 10f, 0f), Quaternion.identity);
        }
        else
        {
            Debug.Log("�����͎Q����2");
            Debug.Log(GetMasterColor());
            if (GetMasterColor())
            {
                Debug.Log("�����͎Q����3");
                Debug.Log("�����̐F�͍� OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));
            }
            else
            {
                Debug.Log("�����͎Q����4");
                Debug.Log("�����̐F�͔� OnRoomPropertiesUpdate");
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
            //��Ŏ���*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = false;
            //��Ŏ���*****************************************************
        }
    }

    [PunRPC]
    public void AllowAction(GameObject masterAvatar)
    {
        if (masterAvatar.GetPhotonView().Owner.IsMasterClient)
        {
            //��Ŏ���*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = true;
            //��Ŏ���*****************************************************

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