using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class MonitorBall : MonoBehaviourPunCallbacks
{
    public GameObject ball;
    public Rigidbody rb;
    public float time = 0;
    private enum State
    {
        Wait,
        Ready
    }
    private State state;
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        state = State.Wait;
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        ball = GameObject.Find("Ball(Clone)");
        rb = ball.GetComponent<Rigidbody>();
        state = State.Ready;
    }


    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (state != State.Ready) return;
        time += Time.deltaTime;

        if (time >= 3)
        {
            photonView.RPC(nameof(BallPos), RpcTarget.All);
            time = 0;
        }
    }

    [PunRPC]
    private void BallPos()
    {
        ball.transform.position = ball.transform.position;
        rb.velocity = rb.velocity;
    }
}
