using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public bool isRaceInProgress = false;

    public float lineSpacing = 2;

    public float finishZ = 681.7f;

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
    public static GameManager Instance { get { 
            return _instance; } }
    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
