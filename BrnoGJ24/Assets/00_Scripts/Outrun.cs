using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outrun : MonoBehaviour
{
    public Transform playerTransform;
    public CarMovement player;
    public Transform outrunGauge;
    public GameManager _manager;
    public OppoentController opponent;
    public float distance = 0;
    public float outrunTarget = -100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(distance < outrunTarget){
            player.StartBraking();
            opponent.StartBraking();
            //GameManager.Instance.gameState = GameManager.GameState.AfterRace;
            GameManager.Instance.whowon = GameManager.WhoWon.player;
            Debug.Log("Player won!");
            StartCoroutine(GameManager.Instance.LoadLevelSelect());
        } else if(distance > -outrunTarget){
            player.StartBraking();
            opponent.StartBraking();
            //GameManager.Instance.gameState = GameManager.GameState.AfterRace;
            GameManager.Instance.whowon = GameManager.WhoWon.opponent;
            Debug.Log("Player lost!");
            StartCoroutine(GameManager.Instance.LoadLevelSelect());
        } else{
            distance = transform.position.z - playerTransform.position.z;
            outrunGauge.localPosition = new Vector3(distance*0.475f, outrunGauge.localPosition.y, outrunGauge.localPosition.z);
        }
    }
}
