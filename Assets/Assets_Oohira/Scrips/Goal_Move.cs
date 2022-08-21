using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Move : MonoBehaviour
{
    [SerializeField] private Vector3 coordinate_Goal = Vector3.zero;
    [SerializeField] private float speed_Move = 1;
    [SerializeField] bool turn = false;
    private Rigidbody rb = null;
    private ZoneDiffinitionOfRoom zone = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        rb.velocity = new Vector3(1, 0, 0) * speed_Move;
    }

    private void FixedUpdate()
    {
        coordinate_Goal = zone.CoordinateFromPosition(transform.position);
        Move();
    }

    private void Move()
    {
        if (coordinate_Goal.x > 7 && !turn)
        {
            turn = true;
            rb.velocity *= -1;
        }
        if (coordinate_Goal.x < -7 && turn)
        {
            turn = false;
            rb.velocity *= -1;
        }
    }
}
