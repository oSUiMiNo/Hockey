using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
public class Ball : MonoBehaviour
{
    private enum State
    {
        Wait,
        Ready
    }
    public enum MoveState
    {
        First,
        Move,
        Reflect,
        ToPlayer,
        Idle
    }
    public enum StrikeState
    {
        StruckByPlayer0,
        StruckByPlayer1,
        Idle
    }
    public enum ToPlayerState
    {
        ToPlayer0,
        ToPlayer1,
        Idle
    }
    private State state;
    public MoveState moveState = MoveState.Idle;
    public StrikeState strikeState = StrikeState.Idle;
    public ToPlayerState toPlayerState = ToPlayerState.Idle;

    Rigidbody rb;
    [SerializeField] public Owners owner_Ball;
    [SerializeField] bool visualizeSphereCast = false;
    [SerializeField] GameObject sphereCast;
    int randomNumber;
    [SerializeField] Vector3 firstFource;

    [SerializeField] float speed = 3;

    [SerializeField] GameObject racket0;
    [SerializeField] GameObject racket1;

    [SerializeField] GameObject line0;
    [SerializeField] GameObject line1;
    [SerializeField] float distance_Z;
    [SerializeField] int divisionPointsVolume;
    [SerializeField] int passingPointsVolume = 5;
    [SerializeField] int count = 0;
    [SerializeField] private Vector3[] points;
    [SerializeField] private Vector3[] normals;

    [SerializeField] private Vector3 lastPoint;
    [SerializeField] private Vector3 lastNormal;
    void Start()
    {
        StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        state = State.Wait;
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        moveState = MoveState.First;
        racket0 = GameObject.Find("Racket0");
        racket1 = GameObject.Find("Racket1");
        rb = GetComponent<Rigidbody>();
        Random.InitState(System.DateTime.Now.Millisecond);
        randomNumber = Random.Range(-3, 3);
        points = new Vector3[passingPointsVolume + 1];
        normals = new Vector3[passingPointsVolume];

        for (int a = 0; a < passingPointsVolume + 1; a++)
        {
            ProcessReflect_Middle(a);
            StartCoroutine(Wait(a));
        }
        //StartCoroutine(Wait(passingPointsVolume + 1));
        moveState = MoveState.Move;
        state = State.Ready;
    }


    private void FixedUpdate()
    {
        if (state != State.Ready) return;
        if (moveState == MoveState.Reflect)
        {
            count = 0;
            points = new Vector3[passingPointsVolume + 1];
            normals = new Vector3[passingPointsVolume];
            points[0] = transform.position;
            for (int b = 0; b < passingPointsVolume + 1; b++)
            {
                ProcessReflect_Middle(b);
                StartCoroutine(Wait(b));
            }
            moveState = MoveState.Move;
        }
        if (moveState == MoveState.Move) Move();
        if (toPlayerState != ToPlayerState.Idle) Process();
    }

    private Vector3 struckDirection;
    private void ProcessReflect_Middle(int a)
    {
        Color rayColor = Color.white;
        float radius = transform.localScale.x / 2;
        Vector3 outDirection = Vector3.zero;
        if (a < passingPointsVolume - 1)
        {
            if (moveState == MoveState.First)
            {
                Debug.Log("First  " + a);
                points[a] = transform.position;
                //outDirection = new Vector3(randomNumber, randomNumber, randomNumber).normalized;
                outDirection = firstFource.normalized;  //ここにラケットで打った際の方向を入れる。
                moveState = MoveState.Move;
            }
            else if (strikeState != StrikeState.Idle)
            {
                Debug.Log("Middle0  " + a);
                outDirection = struckDirection.normalized;
            }
            else
            {
                Debug.Log("Middle1  " + a);
                Vector3 inDirection;
                Vector3 normal;
                if (a == 0)
                {
                    inDirection = points[a] - lastPoint;
                    normal = lastNormal;
                    outDirection = OutDestination_General(inDirection, normal - points[a]).normalized;
                }
                else
                {
                    inDirection = points[a] - points[a - 1];
                    normal = normals[a - 1];
                    Vector3 lineDirection = Vector3.zero;
                    Debug.Log(owner_Ball);
                    if (owner_Ball == Owners.player1) lineDirection = line1.transform.position - points[1];
                    if (owner_Ball == Owners.player0) lineDirection = line0.transform.position - points[1];
                    Debug.Log("ラインの方向  " + lineDirection);
                    int divisionVolume = passingPointsVolume - 2;
                    distance_Z = (lineDirection / divisionVolume).z;
                    Vector3 goal_Z = new Vector3(0, 0, points[a].z + distance_Z);
                    outDirection = (OutDestination_Flex(inDirection, normal, goal_Z) - points[a]).normalized;
                }
            }
            //Physics.SphereCast(points[a], radius, outDirection, out RaycastHit hitInfo, 10000f, layerMask_Room);
            //Debug.DrawRay(points[a], outDirection * hitInfo.distance, Color.red, 5f, false);
            //Instantiate(sphereCast, hitInfo.point, Quaternion.identity);
            //points[a + 1] = hitInfo.point;
            //normals[a] = hitInfo.normal;
            rayColor = Color.red;
            strikeState = StrikeState.Idle;
        }
        else if (a == passingPointsVolume - 1)
        {
            Debug.Log("Final  " + a);
            outDirection = (GetPlayerTargetPosition() - points[a]).normalized;
            rayColor = Color.blue;
        }
        Debug.Log("レイ飛ばす方向  " + outDirection);
        Physics.SphereCast(points[a], radius, outDirection, out RaycastHit hitInfo, 10000f, layerMask_Room);
        Debug.DrawRay(points[a], outDirection * hitInfo.distance, rayColor, 5f, false);
        Instantiate(sphereCast, hitInfo.point, Quaternion.identity);
        Debug.Log("レイ当たった場所  " + hitInfo.point);
        if (a + 1 < points.Length) points[a + 1] = hitInfo.point;
        if (a < normals.Length) normals[a] = hitInfo.normal;
    }
    private IEnumerator Wait(int a)
    {
        Debug.Log("Wait0  " + a);
        yield return new WaitUntil(() => transform.position == points[a]);
        Debug.Log("Wait1  " + a);
        count++;
        if (count == passingPointsVolume)
        {
            if (owner_Ball == Owners.player0) toPlayerState = ToPlayerState.ToPlayer0;
            if (owner_Ball == Owners.player1) toPlayerState = ToPlayerState.ToPlayer1;
        }
        if (count == passingPointsVolume + 1)
        {
            lastPoint = points[a - 1];
            lastNormal = normals[a - 1];
            Debug.Log("前の最後  " + lastPoint);
            Debug.Log("前の法線  " + lastNormal);
            if (owner_Ball == Owners.player0)
            {
                owner_Ball = Owners.player1; Debug.Log("オーナーチェンジ0  Wait");
            }
            else if (owner_Ball == Owners.player1)
            {
                owner_Ball = Owners.player0; Debug.Log("オーナーチェンジ1  Wait");
            }
            toPlayerState = ToPlayerState.Idle;
            moveState = MoveState.Reflect;
        }
    }
    private void Move()
    {
        Vector3 goal;
        if (count < passingPointsVolume + 1)
        {
            Debug.Log("Move  " + count);
            goal = points[count];
            transform.position = Vector3.MoveTowards(transform.position, goal, speed);
        }
    }


    private Vector3 OutDestination_General(Vector3 inDirection, Vector3 contactPointNormal)
    {
        Debug.Log("反射0  " + inDirection.normalized + ",  " + contactPointNormal);
        float inNormal_Volume_Magnitude = Vector3.Dot(contactPointNormal, inDirection);

        Vector3 inNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 outNormal_Volume = inNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 Spherecast_direction = Vector3.Normalize(horizontal_Volume + outNormal_Volume);
        Debug.Log("反射1  " + Spherecast_direction.normalized);
        float offset = Physics.defaultContactOffset * 2;
        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2 + offset;
        Physics.SphereCast(origin, colliderRadius, Spherecast_direction, out RaycastHit hitInfo, 10000f, layerMask_Room);

        Vector3 destination = hitInfo.point;

        return destination;
    }
    private Vector3 OutDestination_Flex(Vector3 inDirection, Vector3 contactPointNormal, Vector3 destinationElement2)
    {
        float inNormal_Volume_Magnitude = Vector3.Dot(contactPointNormal, inDirection);

        Vector3 inNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 outNormal_Volume = inNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 Spherecast_direction = Vector3.Normalize(horizontal_Volume + outNormal_Volume);

        float offset = Physics.defaultContactOffset * 2;
        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2 + offset;
        Physics.SphereCast(origin, colliderRadius, Spherecast_direction, out RaycastHit hitInfo, 10000f, layerMask_Room);

        Vector3 destinationElemen1 = hitInfo.point;
        Vector3 destination = new Vector3(destinationElemen1.x, destinationElemen1.y, destinationElement2.z);

        return destination;
    }


    [SerializeField] LayerMask layerMask_Room;
    [SerializeField] LayerMask layerMask_Racket_Collider;
    [SerializeField] List<GameObject> sphereCasts = new List<GameObject>();
    [SerializeField] int count_ProcessForwardDetection = 0;
    private void Process()
    {
        ProcessForwardDetection("Racket1", layerMask_Racket_Collider, 2f);
        ProcessForwardDetection("Racket0", layerMask_Racket_Collider, 1f);
        count_ProcessForwardDetection = 0;
    }
    private void ProcessForwardDetection(string name_ReflectorObject, LayerMask layerMask, float reflectMargin)
    {
        Vector3 velocity = rb.velocity;

        Vector3 direction;
        if (name_ReflectorObject == "Racket0") direction = (racket0.transform.position - transform.position).normalized;
        else if (name_ReflectorObject == "Racket1") direction = (racket1.transform.position - transform.position).normalized;
        else direction = velocity.normalized;

        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2;
        bool isHit = Physics.SphereCast(origin, colliderRadius, direction, out RaycastHit hitInfo, 10000f, layerMask);
        if (visualizeSphereCast)
        {
            //Debug.Log(name_ReflectorObject + ",  " + hitInfo.collider.gameObject);
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.white, 0.02f, false);
            //sphereCasts[count_ProcessForwardDetection - 1].transform.position = origin + direction * hitInfo.distance;
            //sphereCasts[count_ProcessForwardDetection - 1].transform.localScale = new Vector3(1, 1, 1) * colliderRadius * 2;
        }
        //Debug.Log("プロセス1");
        if (!isHit) return;
        if (hitInfo.collider.gameObject.tag != name_ReflectorObject) return;
        //Debug.Log("プロセス2");
        float distance = hitInfo.distance;
        float nextMoveDistance = speed * Time.fixedDeltaTime;
        //Debug.Log(distance + ", " + (nextMoveDistance + reflectMargin));
        if (distance > nextMoveDistance + reflectMargin) return;
        Debug.Log("プロセス3");

        //rb.velocity = hitInfo.normal * 50f;

        struckDirection = hitInfo.normal;
        count = 0;
        toPlayerState = ToPlayerState.Idle;
        if (name_ReflectorObject == "Racket0")
        {
            strikeState = StrikeState.StruckByPlayer0;
            owner_Ball = Owners.player1; Debug.Log("オーナーチェンジ0  ラケット");
        }
        else
        {
            strikeState = StrikeState.StruckByPlayer1;
            owner_Ball = Owners.player0; Debug.Log("オーナーチェンジ1  ラケット");
        }
        points = new Vector3[passingPointsVolume + 1];
        normals = new Vector3[passingPointsVolume];
        points[0] = transform.position;
        for (int a = 0; a < passingPointsVolume + 1; a++)
        {
            ProcessReflect_Middle(a);
            StartCoroutine(Wait(a));
        }
    }



    [SerializeField] GameObject[] targets_Array = null;
    [SerializeField] List<GameObject> targets_List = null;
    public Vector3 GetPlayerTargetPosition()
    {
        targets_Array = GameObject.FindGameObjectsWithTag("Targets");
        foreach (GameObject aim in targets_Array)
        {
            //Debug.Log("ゴールリスト作る1" + goal);
            //Debug.Log("ゴールリスト作る2" + GetComponent<Reflector_Racket>().playerIdentification);
            //Debug.Log("ゴールリスト作る2" + goal.GetComponent<Goal>().playerIdentification);
            if (aim.GetComponent<Owner>().player == owner_Ball)
            {
                targets_List.Add(aim);
            }
        }

        int index_Random = Random.Range(0, targets_List.Count);
        Vector3 targetPosition_Random = targets_List[index_Random].transform.position;
        Debug.Log("プレイヤーに行く0" + targetPosition_Random);

        targets_List.Clear();
        return targetPosition_Random;
    }
}
