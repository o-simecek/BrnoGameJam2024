using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{

    [SerializeField]
    private UnityEngine.UI.Button level2Button;
    [SerializeField]
    private UnityEngine.UI.Button level3Button;
    [SerializeField]
    private UnityEngine.UI.Button level4Button;

    // Start is called before the first frame update
    void Start()
    {
        if (!(GameManager.Instance.clearedLevels.Contains("Race1")))
        {
            level2Button.enabled = false;
        }
        if (!(GameManager.Instance.clearedLevels.Contains("Race2")))
        {
            level3Button.enabled = false;
        }
        if (!(GameManager.Instance.clearedLevels.Contains("Race3")))
        {
            level4Button.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLevel1()
    {
        GameManager.Instance.gameState = GameManager.GameState.BeforeRace;
        SceneManager.LoadScene("Race1");
    }

    public void OnLevel2()
    {
        GameManager.Instance.gameState = GameManager.GameState.BeforeRace;
        SceneManager.LoadScene("Race2");
    }

    public void OnLevel3()
    {
        GameManager.Instance.gameState = GameManager.GameState.BeforeRace;
        SceneManager.LoadScene("Race3");
    }

    public void OnLevel4()
    {
        GameManager.Instance.gameState = GameManager.GameState.BeforeRace;
        SceneManager.LoadScene("Race4");
    }

    public void OnBack()
    {
        SceneManager.LoadScene("Menu");
    }
}
