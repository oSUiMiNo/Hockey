using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketMove : MonoBehaviour
{
    [SerializeField] private float movepower = 3;

    private Rigidbody rb = null;
    private ZoneDiffinitionOfRoom zone = null;
    //[SerializeField] Vector3 racketVelocity;
    [SerializeField] public Vector3 swingDirection = Vector3.zero;


    private void Component()
    {
        rb = GetComponent<Rigidbody>();
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
    }

    private void Start()
    {
        Component();
        record = new Vector3[recVolume];
    }

    void FixedUpdate()
    {
        RacketCoordinate();
        Moove();
        //racketVelocity = rb.velocity;
    }

    [SerializeField]
    public Vector3 racketCoordinate;
    [SerializeField]
    Vector3 primitiveCoordinate;
    private void RacketCoordinate()
    {
        racketCoordinate = zone.CoordinateFromPosition(transform.position);
        primitiveCoordinate = zone.primitiveCoordinate;
    }

    private void Moove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 IdouHoukou = new Vector3(x, y, 0);

        rb.velocity = IdouHoukou * movepower;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    [SerializeField] private GameObject ball = null;
    //[SerializeField] private bool rec = true;
    [SerializeField] private int recVolume = 3;
    [SerializeField] private Vector3[] record = new Vector3[10];
    public void Record()
    {
        //if (!rec) return;

        for (int i = 0; i < recVolume - 1; i++)
        {
            //if (!rec) break;
            record[(recVolume - 1) - i] = record[(recVolume - 1) - i - 1];
            record[0] = transform.position;

            //Debug.Log((recVolume - 1 - i).ToString() + " " + record[recVolume - 1 - i].ToString());
            //if (i == recVolume - 2) Debug.Log("0 " + record[0].ToString());
        }

        swingDirection = (record[0] - record[(recVolume - 1)]).normalized;
    }
}