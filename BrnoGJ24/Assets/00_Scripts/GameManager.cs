using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public bool isRaceInProgress = false;

    public float lineSpacing = 2;

    public float finishZ = 681.7f;

    public int[] carHash = new int[8] {0, 0, 0, 0, 0, 0, 0, 0};

    public enum GameState
    {
        Menu,
        BeforeRace,
        Race,
        AfterRace
    }

    public enum WhoWon
    {
        player,
        opponent,
        nobody
    }

    public WhoWon whowon = WhoWon.nobody;

    public GameState gameState = GameState.BeforeRace;

    public List<string> clearedLevels = new List<string> {};
    public static GameManager Instance { get { 
            return _instance; } }
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuit();
        }
    }

    public IEnumerator LoadLevelSelect()
    {
        yield return new WaitForSeconds(2);
        OnLevelSelect();
    }

    public void OnLevelSelect()
    {
        gameState = GameState.Menu;
        whowon = WhoWon.nobody;
        SceneManager.LoadScene("LevelSelect");
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void onSaveCar()
    {
        carHash = FindObjectOfType<OpponentGenerator>().carHash;
    }

}
