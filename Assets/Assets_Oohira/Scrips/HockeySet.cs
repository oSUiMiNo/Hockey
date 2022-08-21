using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HockeySet : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    private void Start()
    {
        StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        SetThis(gameObject);
    }


    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!stageWasSet) return;
        Debug.Log("hockeySet�ǉ�");
        RoomDoorWay.instance.hockeySet = GetThis();
        stageWasSet = false;
    }



    private static ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
    [SerializeField] private static bool stageWasSet = false;
    public static void SetThis(GameObject stage)
    {
        Debug.Log("SetStage");
        prop["Stage"] = stage.name;
        prop["StageWasSet"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        prop.Clear();
    }
    public static GameObject GetThis()
    {
        Debug.Log("GetStage");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["Stage"]);
        stageWasSet = (PhotonNetwork.CurrentRoom.CustomProperties["StageWasSet"] is bool got) ? got : false;
        string stage = (PhotonNetwork.CurrentRoom.CustomProperties["Stage"] is string p) ? p : null;
        return GameObject.Find(stage);
    }
}