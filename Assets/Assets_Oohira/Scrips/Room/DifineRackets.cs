using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifineRackets : MonoBehaviour
{
    [SerializeField] public GameObject racket0_Core;
    [SerializeField] public GameObject racket0_Body;
    [SerializeField] public GameObject racket0_Collider_0;
    [SerializeField] public GameObject racket0_Collider_1;

    [SerializeField] public GameObject racket1_Core;
    [SerializeField] public GameObject racket1_Body;
    [SerializeField] public GameObject racket1_Collider_0;
    [SerializeField] public GameObject racket1_Collider_1;

  
    public void Init()
    {
        racket0_Core = GameObject.Find("Racket0_Core");
        //racket0_Body = 
        //racket0_Collider_0 = 
        //racket0_Collider_1 = 

        racket1_Core = GameObject.Find("Racket1");
        //racket1_Body = 
        //racket1_Collider_0 = 
        //racket1_Collider_1 = 
    }
}
