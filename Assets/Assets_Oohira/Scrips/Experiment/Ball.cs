using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    //private ZoneDiffinitionOfRoom zoon = null;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        //zoon = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        randomNumber = Random.Range(0, 10);
        FirstMove();
    }

    [SerializeField] private LayerMask layerMask;

    void FixedUpdate()
    {
        Debug.Log("a");
        Debug.DrawRay(origin, direction * hitInfo.distance, Color.white, 100f, false);
        gameObject.transform.position = Vector3.MoveTowards(start, goal, 10);
    }

    int randomNumber;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 goal;
    [SerializeField] bool isHit;

    Vector3 origin;
    float colliderRadius;
    Vector3 direction;
    RaycastHit hitInfo;
    private void FirstMove()
    {
        Debug.Log("FirstMove");
        origin = gameObject.transform.position;
        colliderRadius = transform.localScale.x / 2;
        direction = new Vector3(randomNumber, randomNumber, randomNumber);
        isHit = Physics.SphereCast(origin, colliderRadius, direction, out hitInfo, 10000f, layerMask);

        start = transform.position;
        goal = hitInfo.point;

    }
}
