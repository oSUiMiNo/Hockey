using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateManager : MonoBehaviour
{
    [SerializeField] public ZoneDiffinitionOfRoom zone = null;
    [SerializeField] public Vector3 ballCoordinate = new Vector3(0, 0, 0);
    [SerializeField] public float ballCoordinate_Mag = 0;
    [SerializeField] public Vector3 primitiveCoordinate = new Vector3(0, 0, 0);

    private void Start()
    {
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        ballCoordinate = zone.CoordinateFromPosition(transform.position);
        ballCoordinate_Mag = ballCoordinate.magnitude;
        //ƒ‹[ƒ€À•W‚Ì1–Ú·‚è•Ó‚è‚Ì’·‚³‚ğæ“¾
        primitiveCoordinate = zone.primitiveCoordinate;
    }

}
