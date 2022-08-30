using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
public class Ball : MonoBehaviourPunCallbacks
{
    private enum State
    {
        Wait,
        Ready,
        Init,
        Update
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
    [SerializeField] private State state;
    public MoveState moveState = MoveState.Idle;
    public StrikeState strikeState = StrikeState.Idle;
    public ToPlayerState toPlayerState = ToPlayerState.Idle;
    public Owners owner_Ball;

    Rigidbody rb;
    [SerializeField] bool visualizeSphereCast = false;
    [SerializeField] GameObject sphereCast;
    int randomNumber;
    [SerializeField] private Vector3 firstDirection;
    [SerializeField] private Vector3 struckDirection;

    [SerializeField] float speed = 3;
    [SerializeField] float margin = 1;

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
        Random.InitState(System.DateTime.Now.Millisecond);
        moveState = MoveState.First;

        state = State.Wait;
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        //yield return new WaitForSeconds(1);

        line0 = GameObject.Find("Lines_Player0");
        line1 = GameObject.Find("Lines_Player1");
        racket0 = GameObject.Find("Racket0");
        racket1 = GameObject.Find("Racket1");
        rb = GetComponent<Rigidbody>();
        randomNumber = Random.Range(-3, 3);

        Reversal();
        //count = 0;
        //points = new Vector3[passingPointsVolume + 1];
        //normals = new Vector3[passingPointsVolume + 1];
        //for (int a = 0; a < passingPointsVolume; a++)
        //{
        //    ProcessReflect_Middle(a);
        //    StartCoroutine(Wait(a));
        //}
        //StartCoroutine(Wait(passingPointsVolume));

        moveState = MoveState.Move;
        state = State.Ready;
    }


    private void FixedUpdate()
    {
        if (state != State.Ready) return;
        if (moveState == MoveState.Reflect)
        {
            Reversal();
            //count = 0;
            //points = new Vector3[passingPointsVolume + 1];
            //normals = new Vector3[passingPointsVolume + 1];
            //points[0] = transform.position + lastNormal * margin;
            //normals[0] = lastNormal;
            //for (int b = 0; b < passingPointsVolume; b++)
            //{
            //    ProcessReflect_Middle(b);
            //    StartCoroutine(Wait(b));
            //}
            //StartCoroutine(Wait(passingPointsVolume));
            moveState = MoveState.Move;
        }
        if (moveState == MoveState.Move) Move();
        if (toPlayerState != ToPlayerState.Idle) Process();
    }

    private void Reversal()
    {
        count = 0;
        points = new Vector3[passingPointsVolume + 1];
        normals = new Vector3[passingPointsVolume + 1];
        points[0] = transform.position + lastNormal * margin;
        normals[0] = lastNormal;
        for (int a = 0; a < passingPointsVolume; a++)
        {
            ProcessReflect_Middle(a);
            StartCoroutine(Wait(a));
        }
        StartCoroutine(Wait(passingPointsVolume));
    }

   
    [PunRPC]
    private void P1(Vector3 position, int a)
    {
        transform.position = position; 

        //���]�̓���***********************************
        moveState = MoveState.Move;
        
        outDirection = firstDirection.normalized;
    }
    [PunRPC]
    private void P2(Vector3 position, int a)
    {
        transform.position = position;

        //���]�̓���***********************************
        outDirection = struckDirection.normalized;
    }
        [PunRPC]
    private void P3(Vector3 position, int a)
    {
        transform.position = position;

        //���]�̓���***********************************
        Vector3 inDirection = (points[0] - lastPoint).normalized;

        outDirection = (OutDestination_General(inDirection, normals[0]) - points[0]).normalized;
    }
    [PunRPC]
    private void W(Vector3 position, int a)
    {
        transform.position = position;

        //���]�̓���***********************************
        lastPoint = points[a - 1];
        lastNormal = normals[a];
        Debug.Log("�O�̍Ō�  " + lastPoint);
        Debug.Log("�O�̖@��  " + lastNormal);
        if (owner_Ball == Owners.player0)
        {
            owner_Ball = Owners.player1; //Debug.Log("�I�[�i�[�`�F���W0  Wait");
        }
        else if (owner_Ball == Owners.player1)
        {
            owner_Ball = Owners.player0; //Debug.Log("�I�[�i�[�`�F���W1  Wait");
        }
        toPlayerState = ToPlayerState.Idle;
        moveState = MoveState.Reflect;
    }

    Vector3 outDirection = Vector3.zero;
    private void ProcessReflect_Middle(int a)
    {
        Color rayColor = Color.white;
        float radius = transform.localScale.x / 2;
        RaycastHit hitInfo;
        if (a < passingPointsVolume - 1)                                //�Ō�̃J�E���g�ȊO
        {
            if (moveState == MoveState.First)            //�Q�[���J�n��̈�ԍŏ�
            {
                Debug.Log("First  " + a);
                //���]�̓���***********************************
                //points[a] = transform.position;
                //outDirection = firstDirection.normalized;
                //moveState = MoveState.Move;
                if (PhotonNetwork.IsMasterClient) photonView.RPC(nameof(P1), RpcTarget.All, transform.position, a);
            }
            else if (strikeState != StrikeState.Idle)    //���P�b�g�őł��ꂽ����
            {
                Debug.Log("Middle0  " + a);
                //���]�̓���***********************************
                //outDirection = struckDirection.normalized;
                if (PhotonNetwork.IsMasterClient) photonView.RPC(nameof(P2), RpcTarget.All, points[a], a);
            }
            else                                         //����ȊO
            {
                Debug.Log("Middle1  " + a);
                if (a == 0)                   //a��0�̍ŏ��̃��[�v
                {
                    //���]�̓���***********************************
                    //inDirection = (points[a] - lastPoint).normalized;
                    //Debug.Log("����  " + inDirection);
                    //Debug.Log(points[a] + ",  " + normals[a]);
                    //normal = normals[a];
                    //outDirection = (OutDestination_General(inDirection, normal) - points[a]).normalized;
                    if (PhotonNetwork.IsMasterClient) photonView.RPC(nameof(P3), RpcTarget.All, points[a], a);
                }
                else                          //a��1�`�Ō�܂ł̃��[�v
                {
                    Vector3 inDirection = (points[a] - points[a - 1]).normalized;
                    
                    Vector3 lineDirection = Vector3.zero;
                    if (owner_Ball == Owners.player1) lineDirection = line1.transform.position - points[1];
                    if (owner_Ball == Owners.player0) lineDirection = line0.transform.position - points[1];
                    int divisionVolume = passingPointsVolume - 2;
                    distance_Z = (lineDirection / divisionVolume).z;
                    Vector3 goal_Z = new Vector3(0, 0, points[a].z + distance_Z);
                    
                    outDirection = (OutDestination_Flex(inDirection, normals[a], goal_Z) - points[a]).normalized;
                }
            }
            rayColor = Color.red;
            strikeState = StrikeState.Idle;
        }
        else if (a == passingPointsVolume - 1)                      //�Ō�̃J�E���g�̎�
        {
            Debug.Log("Final  " + a);
            outDirection = (GetPlayerTargetPosition() - points[a]).normalized;
            rayColor = Color.blue;
        }
        Debug.Log("���C��΂�����  " + outDirection + ", ���C�̌��_  " + points[a]);
        Physics.SphereCast(points[a], radius, outDirection, out hitInfo, 10000f, layerMask_Wall);
        Debug.Log("���C���������ꏊ  " + hitInfo.point + ", ���C�̒���  " + hitInfo.distance);
        Debug.DrawRay(points[a], outDirection * hitInfo.distance, rayColor, 8f, false);
        Debug.DrawRay(points[a], outDirection * 100, Color.green, 2f, false);
        //Instantiate(sphereCast, hitInfo.point + hitInfo.normal * margin, Quaternion.identity);
        if (a + 1 < points.Length) Debug.Log("���̃C���f�b�N�X  " + (a + 1));
        if (a + 1 < points.Length) points[a + 1] = hitInfo.point + hitInfo.normal * margin;
        if (a + 1 < normals.Length) normals[a + 1] = hitInfo.normal;
    }
    private IEnumerator Wait(int a)
    {
        Debug.Log("Wait0  " + a);
        if (count <= passingPointsVolume)   //�J�E���g��0�`�C���f�b�N�X�̍ő�+1�܂�
        {
            Debug.Log("Wait1-0  " + a);
            yield return new WaitUntil(() => transform.position == points[a]);
            Debug.Log("Wait1-1  " + a);
            count++;
            Debug.Log("Count  " + count);
            if (count == passingPointsVolume)
            {
                if (owner_Ball == Owners.player0) toPlayerState = ToPlayerState.ToPlayer0;
                if (owner_Ball == Owners.player1) toPlayerState = ToPlayerState.ToPlayer1;
            }
        }

        //���]�̓���***********************************
        if (count == passingPointsVolume + 1)
        {
            Debug.Log("Wait2-0  " + a);
            yield return new WaitUntil(() => transform.position == points[a]);
            Debug.Log("Wait2-1  " + a);
            Debug.Log("Count  " + count);

            //lastPoint = points[a - 1];
            //lastNormal = normals[a];
            //Debug.Log("�O�̍Ō�  " + lastPoint);
            //Debug.Log("�O�̖@��  " + lastNormal);
            //if (owner_Ball == Owners.player0)
            //{
            //    owner_Ball = Owners.player1; //Debug.Log("�I�[�i�[�`�F���W0  Wait");
            //}
            //else if (owner_Ball == Owners.player1)
            //{
            //    owner_Ball = Owners.player0; //Debug.Log("�I�[�i�[�`�F���W1  Wait");
            //}
            //toPlayerState = ToPlayerState.Idle;
            //moveState = MoveState.Reflect;
            if (PhotonNetwork.IsMasterClient) photonView.RPC(nameof(W), RpcTarget.All, transform.position, a);
        }
    }
    private void Move()
    {
        Vector3 goal;
        if (count < passingPointsVolume + 1)
        {
            //Debug.Log("Move  " + count);
            goal = points[count];
            transform.position = Vector3.MoveTowards(transform.position, goal, speed);
        }
    }


    private Vector3 OutDestination_General(Vector3 inDirection, Vector3 contactPointNormal)
    {
        float inNormal_Volume_Magnitude = Mathf.Abs(Vector3.Dot(contactPointNormal, inDirection));

        Vector3 outNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 inNormal_Volume = outNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 Spherecast_direction = Vector3.Normalize(horizontal_Volume + outNormal_Volume);

        float offset = Physics.defaultContactOffset * 2;
        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2 + offset;

        Physics.SphereCast(origin, colliderRadius, Spherecast_direction, out RaycastHit hitInfo, 10000f, layerMask_Wall);
        //Debug.DrawRay(origin, Spherecast_direction * 120, Color.black, 5f, false);

        Vector3 destination = hitInfo.point;
        Debug.Log("Destination_Genaral" + hitInfo.point);

        return destination;
    }
    private Vector3 OutDestination_Flex(Vector3 inDirection, Vector3 contactPointNormal, Vector3 destinationElement2)
    {
        float inNormal_Volume_Magnitude = Mathf.Abs(Vector3.Dot(contactPointNormal, inDirection));

        Vector3 outNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 inNormal_Volume = outNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 Spherecast_direction = Vector3.Normalize(horizontal_Volume + outNormal_Volume);

        float offset = Physics.defaultContactOffset * 2;
        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2 + offset;

        Physics.SphereCast(origin, colliderRadius, Spherecast_direction, out RaycastHit hitInfo, 10000f, layerMask_Wall);
        //Debug.DrawRay(origin, Spherecast_direction * 120, Color.black, 5f, false);

        Vector3 destinationElement1 = hitInfo.point;
        Vector3 destination = new Vector3(destinationElement1.x, destinationElement1.y, destinationElement2.z);
        Debug.Log("Destination_Flex" + hitInfo.point);

        return destination;
    }


    [SerializeField] LayerMask layerMask_Wall;
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
        //Debug.Log("�v���Z�X1");
        if (!isHit) return;
        if (hitInfo.collider.gameObject.tag != name_ReflectorObject) return;
        //Debug.Log("�v���Z�X2");
        float distance = hitInfo.distance;
        float nextMoveDistance = speed * Time.fixedDeltaTime;
        //Debug.Log(distance + ", " + (nextMoveDistance + reflectMargin));
        if (distance > nextMoveDistance + reflectMargin) return;
        Debug.Log("�v���Z�X3");

        //���]�̓���***********************************
        //struckDirection = hitInfo.normal;
        //count = 0;
        //toPlayerState = ToPlayerState.Idle;
        //if (name_ReflectorObject == "Racket0")
        //{
        //    strikeState = StrikeState.StruckByPlayer0;
        //    owner_Ball = Owners.player1; Debug.Log("�I�[�i�[�`�F���W0  ���P�b�g");
        //}
        //else
        //{
        //    strikeState = StrikeState.StruckByPlayer1;
        //    owner_Ball = Owners.player0; Debug.Log("�I�[�i�[�`�F���W1  ���P�b�g");
        //}
        //points = new Vector3[passingPointsVolume + 1];
        //normals = new Vector3[passingPointsVolume + 1];
        //points[0] = transform.position;
        //for (int a = 0; a < passingPointsVolume + 1; a++)
        //{
        //    ProcessReflect_Middle(a);
        //    StartCoroutine(Wait(a));
        //}
        if (PhotonNetwork.IsMasterClient) photonView.RPC(nameof(R), RpcTarget.All, transform.position, hitInfo.normal, name_ReflectorObject);
    }

    [PunRPC]
    private void R(Vector3 position, Vector3 struckDirection, string name_ReflectorObject)
    {
        transform.position = position;

        //���]�̓���***********************************
        toPlayerState = ToPlayerState.Idle;
        this.struckDirection = struckDirection;
        count = 0;
        if (name_ReflectorObject == "Racket0")
        {
            strikeState = StrikeState.StruckByPlayer0;
            owner_Ball = Owners.player1; Debug.Log("�I�[�i�[�`�F���W0  ���P�b�g");
        }
        else
        {
            strikeState = StrikeState.StruckByPlayer1;
            owner_Ball = Owners.player0; Debug.Log("�I�[�i�[�`�F���W1  ���P�b�g");
        }
        Reversal();
        //points = new Vector3[passingPointsVolume + 1];
        //normals = new Vector3[passingPointsVolume + 1];
        //points[0] = transform.position;
        //for (int a = 0; a < passingPointsVolume + 1; a++)
        //{
        //    ProcessReflect_Middle(a);
        //    StartCoroutine(Wait(a));
        //}
    }

    [SerializeField] GameObject[] targets_Array = null;
    [SerializeField] List<GameObject> targets_List = null;
    public Vector3 GetPlayerTargetPosition()
    {
        targets_Array = GameObject.FindGameObjectsWithTag("Targets");
        foreach (GameObject aim in targets_Array)
        {
            if (aim.GetComponent<Owner>().player == owner_Ball)
            {
                targets_List.Add(aim);
            }
        }

        int index_Random = Random.Range(0, targets_List.Count);
        Vector3 targetPosition_Random = targets_List[index_Random].transform.position;

        targets_List.Clear();
        return targetPosition_Random;
    }
}