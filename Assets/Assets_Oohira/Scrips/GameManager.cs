using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] public int point_ToWin = 1;
    public int score_Player0 = 0;
    public int score_Player1 = 0;
    [SerializeField] public TextMeshProUGUI text_Score_Player0 = null;
    [SerializeField] public TextMeshProUGUI text_Score_Player1 = null;
    [SerializeField] public bool inGameScene = false;
    

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "FirstScene_Oohira")
            Start_GameScene();
    }

    private void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) UnityEditor.EditorApplication.isPaused = true;
        Update_GameScene();
    }

    public void Goal(Owners player)
    {
        if (player == Owners.player0)
        {
            score_Player0++;
            text_Score_Player0.text = (score_Player0 + " / " + point_ToWin);
        }
        else
        {
            score_Player1++;
            text_Score_Player1.text = (score_Player1 + " / " + point_ToWin);
        }
    }

    private IEnumerator Load_StartScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("StartScene");
    }

    public void Load_GameScene()
    {
        SceneManager.LoadScene("FirstScene_Oohira");
        Start_GameScene();
    }

    private void Start_GameScene()
    {
        inGameScene = true;
        text_Score_Player0 = GameObject.Find("Score_Player0").GetComponent<TextMeshProUGUI>();
        text_Score_Player1 = GameObject.Find("Score_Player1").GetComponent<TextMeshProUGUI>();
        text_Score_Player0.text = (0 + " / " + point_ToWin);
        text_Score_Player1.text = (0 + " / " + point_ToWin);
    }
    private void Update_GameScene()
    {
        if (!inGameScene) return;
        if (score_Player0 >= point_ToWin || score_Player1 >= point_ToWin)
            StartCoroutine(Load_StartScene(3f));
    }
}
