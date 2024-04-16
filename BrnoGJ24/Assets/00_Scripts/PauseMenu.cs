using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject HUD;
    public GameObject pauseMenu;
    public bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!paused){
                HUD.SetActive(false);
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                paused = true;
                Cursor.visible = true; 
            } else{
                HUD.SetActive(true);
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
                paused = false;
                Cursor.visible = false; 
            }
        }
    }
    public void RestartRace()
    {
        GameManager.Instance.gameState = GameManager.GameState.BeforeRace;
        GameManager.Instance.whowon = GameManager.WhoWon.nobody;
        Time.timeScale = 1;
        paused = false;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ToMenu()
    {
        GameManager.Instance.gameState = GameManager.GameState.Menu;
        GameManager.Instance.whowon = GameManager.WhoWon.nobody;
        Time.timeScale = 1;
        paused = false;
        Cursor.visible = true; 
        SceneManager.LoadScene("Menu");
    }
}
