using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Avatar : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] private float charge = 0;
    [SerializeField] private Image gageImage = null;
    [SerializeField] private PhotonManager photonManager = null;
    [SerializeField] private DifineRackets rackets = null;

    

    private void Start()
    {
        StartCoroutine(Init(5f));
    }
    private IEnumerator Init(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gageImage = GameObject.Find("Gage1").GetComponent<Image>();
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
        SetPlayer(gameObject);

        photonManager = GameObject.FindGameObjectWithTag("Photon").GetComponent<PhotonManager>();
        //photonManager.player0 = gameObject;
        rackets = GameObject.Find("RoomCore").GetComponent<DifineRackets>();
        rackets.Init();
        Debug.Log("çÏÇÁÇÍÇΩ2");
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (!playerWasSet)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if(PhotonManager.GetMasterPos())
                {
                    Debug.Log("player0í«â¡");
                    photonManager.player0 = GetPlayer();
                }
                else
                {
                    Debug.Log("player1í«â¡");
                    photonManager.player1 = GetPlayer();
                }
            }
            else
            {
                if (PhotonManager.GetMasterPos())
                {
                    Debug.Log("player1í«â¡");
                    photonManager.player1 = GetPlayer();
                }
                else
                {
                    Debug.Log("player0í«â¡");
                    photonManager.player0 = GetPlayer();
                }
            }
        }
    }
    private static ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
    [SerializeField] private static bool playerWasSet = false;
    public static void SetPlayer(GameObject player)
    {
        Debug.Log("SetPlayer");
        prop["Player"] = player.name;
        prop["PlayerWasSet"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        prop.Clear();
    }
    public static GameObject GetPlayer()
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
