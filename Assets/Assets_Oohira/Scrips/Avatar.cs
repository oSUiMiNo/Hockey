using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Avatar : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] private float charge = 0;
    [SerializeField] private Image gageImage = null;
    //[SerializeField] private PhotonManager photonManager = null;
    [SerializeField] private RoomDoorWay roomDoorWay = null;
    [SerializeField] private DifineRackets rackets = null;

    [SerializeField] private GameObject roomCore = null;


    private void Start()
    {
        //StartCoroutine(Init());
        gageImage = GameObject.Find("Gage1").GetComponent<Image>();
        rackets = GameObject.Find("RoomCore").GetComponent<DifineRackets>();
        rackets.Init();
    }
    private IEnumerator Init()
    {
        yield return new WaitWhile(() => RoomDoorWay.instance.Ready());
        gageImage = GameObject.Find("Gage1").GetComponent<Image>();
        rackets = GameObject.Find("RoomCore").GetComponent<DifineRackets>();
        rackets.Init();
    }

    public void Charge(float chargePoint)
    {
        charge += chargePoint;
        if (charge <= 0) charge = 0;
        gageImage.fillAmount = charge;
    }



    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        Debug.Log("çÏÇÁÇÍÇΩ1");
        SetThis(gameObject);

        //photonManager = GameObject.FindGameObjectWithTag("Photon").GetComponent<PhotonManager>();
        roomDoorWay = GameObject.FindGameObjectWithTag("RoomDoorWay").GetComponent<RoomDoorWay>();
        //photonManager.player0 = gameObject;
      
        Debug.Log("çÏÇÁÇÍÇΩ2");
    }


    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (!playerWasSet) return;
        
        if (PhotonNetwork.IsMasterClient)
        {
            if(gameObject.name == "Avatar0(Clone)")
            {
                Debug.Log("player0í«â¡");
                roomDoorWay.avatar0 = GetThis();
            }
            else
            {
                Debug.Log("player1í«â¡");
                roomDoorWay.avatar1 = GetThis();
            }
        }
        else
        {
            if (gameObject.name == "Avatar0(Clone)")
            {
                Debug.Log("player0í«â¡");
                roomDoorWay.avatar0 = GetThis();
            }
            else
            {
                Debug.Log("player1í«â¡");
                roomDoorWay.avatar1 = GetThis();
            }
        }

        playerWasSet = false;
    }
    private static ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
    [SerializeField] private static bool playerWasSet = false;
    public static void SetThis(GameObject player)
    {
        Debug.Log("SetPlayer");
        prop["Player"] = player.name;
        prop["PlayerWasSet"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        prop.Clear();
    }
    public static GameObject GetThis()
    {
        Debug.Log("GetPlayer");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["Player"]);
        playerWasSet = (PhotonNetwork.CurrentRoom.CustomProperties["PlayerWasSet"] is bool got) ? got : false;
        string player = (PhotonNetwork.CurrentRoom.CustomProperties["Player"] is string p) ? p : null;
        return GameObject.Find(player);
    }



    //[PunRPC]
    //public void LockAction()
    //{
    //    Debug.Log("LockAction");
    //    GetComponent<PieceSelector>().enabled = false;
    //}

    //[PunRPC]
    //public void AllowAction()
    //{
    //    GetComponent<PieceSelector>().enabled = true;
    //}
}
