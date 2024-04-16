using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject levelMenu;
    public GameObject customizeMenu;
    public GameObject optionsMenu;
    public GameObject menuCar;
    public GameObject menuCarTransform;
    public GameObject rotationStart;
    public GameObject rotationStop;
    public AudioClip clickSound;
    public AudioClip paintSound;
    public AudioClip partSound;
    private AudioSource audioSource;
    private OpponentGenerator carInstance;
    public bool stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        carInstance = Instantiate(menuCar, menuCarTransform.transform, false).GetComponent<OpponentGenerator>();
        carInstance.isEnemy = false;
        carInstance.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!stopped)
            menuCarTransform.transform.Rotate(0, Time.deltaTime * 20, 0);
    }
    public void Quit()
    {
        audioSource.PlayOneShot(clickSound, 1f);
        Application.Quit();
    }
    public void ToMain()
    {
        RotationStart();
        mainMenu.SetActive(true);
        levelMenu.SetActive(false);
        customizeMenu.SetActive(false);
        audioSource.PlayOneShot(clickSound, 1f);
    }
    public void ToLevels()
    {
        mainMenu.SetActive(false);
        levelMenu.SetActive(true);
        audioSource.PlayOneShot(clickSound, 1f);
    }
    public void ToCustomize()
    {
        mainMenu.SetActive(false);
        customizeMenu.SetActive(true);
        audioSource.PlayOneShot(clickSound, 1f);
    }
    public void ToOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        audioSource.PlayOneShot(clickSound, 1f);
    }
    public void ReloadCar()
    {
        stopped = false;
        Destroy(carInstance.gameObject);
        carInstance = Instantiate(menuCar, menuCarTransform.transform, false).GetComponent<OpponentGenerator>();
        carInstance.isEnemy = false;
        carInstance.transform.localPosition = new Vector3(0, 0, 0);
        optionsMenu.SetActive(false);
        audioSource.PlayOneShot(clickSound, 1f);
        ToMain();
    }

    public void ExplodeOptions()
    {
        stopped = true;
        carInstance.Explode();
    }

    public void RotationStop(){
        rotationStop.SetActive(false);
        rotationStart.SetActive(true);
        stopped = true;
        audioSource.PlayOneShot(clickSound, 1f);
    }
    public void RotationStart(){
        rotationStart.SetActive(false);
        rotationStop.SetActive(true);
        stopped = false;
        audioSource.PlayOneShot(clickSound, 1f);
    }

    public void ChangePaint(){
        carInstance.PaintSwitch();
        audioSource.PlayOneShot(paintSound, 2f);
    }

    public void ChangeRims(){
        carInstance.RimSwitch();
        audioSource.PlayOneShot(partSound, 1f);
    }

    public void ChangePart(int i){
        carInstance.PartSwitch(i);
        audioSource.PlayOneShot(partSound, 1f);
    }

    public void Randomize(){
        carInstance.Randomize();
        audioSource.PlayOneShot(clickSound, 1f);
    }
    public void RaceStart(string race){
        GameManager.Instance.gameState = GameManager.GameState.BeforeRace;
        SceneManager.LoadScene(race);
    }

}
