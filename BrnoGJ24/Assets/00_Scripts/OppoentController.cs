using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OppoentController : MonoBehaviour
{
    bool carCrashed = false;

    //Speed
    float maxSpeed = 150;
    public float currentSpeed = 0;

    //Gears
    [SerializeField] int maxGear = 5;
    int currentGear = 1;
    private float destinationX = 0f;
    private float currentX = 0f;
    private float startX = 0f;

    //RPM
    private float currentRPM = 4000f;
    private static float maxRPM = 10000f;
    [SerializeField]
    private static float baseRPMChange = 500f;

    //Acceleration
    [SerializeField]
    private static float baseAcceleration = 4;
    private float acceleration = 0;

    public OpponentGenerator carModel;

    //
    [SerializeField]
    private float skill = 0.9f;
    private AudioSource audioSource;

    [SerializeField] int linesCount = 4;
    [SerializeField]
    public int currentLine = 3;
    private bool isChangingLines = false;
    private bool brakes = false;
    [SerializeField] float sideSpeed = 2;
    
    private float tmp = 0f;
    private float lastVelocity = 0f;
    [SerializeField] Transform visualTransform;
    public AudioClip brakeSound;
    public AudioSource brakeSource;

    // Start is called before the first frame update
    void Start()
    {
        destinationX = transform.position.x;
        currentX = transform.position.x;
        startX = transform.position.x;
        audioSource = GetComponent<AudioSource>();
        
    }

    private void LateUpdate(){

    }

    // Update is called once per frame
    void Update()
    {
        currentX = Mathf.SmoothStep(currentX, destinationX, (8.5f + (currentSpeed/8.5f)) * Time.deltaTime);

        if(Mathf.Abs(destinationX - currentX) > GameManager.Instance.lineSpacing/2f){
            visualTransform.rotation = Quaternion.Euler(currentRPM/50000 * Mathf.PerlinNoise(Time.time * 35, 0), (startX - currentX) * -3.5f, currentRPM/50000 * Mathf.PerlinNoise(Time.time * 25, 0));
        } else{
            visualTransform.rotation = Quaternion.Euler(currentRPM/50000 * Mathf.PerlinNoise(Time.time * 35, 0), (destinationX - currentX) * 3.5f, currentRPM/50000 * Mathf.PerlinNoise(Time.time * 25, 0));
        }

        audioSource.pitch = 0.5f + currentRPM/10000f;

        if (Mathf.Abs(destinationX - currentX) < 0.1f){
            isChangingLines = false;
            startX = destinationX;
        }
        if (carCrashed){
            audioSource.volume = 0;
            return;
        }
            

        transform.position = new Vector3 (currentX, transform.position.y, transform.position.z + currentSpeed * Time.deltaTime); 
        
        if(IsChangeNeeded()){
            if (IsLineClear(-1) && IsLineClear(1))
            {
                ChangeLine((Random.Range(0, 2) == 1));
            } else if (IsLineClear(-1))
            {
                ChangeLine(false);
                Debug.Log("Changing Left!");
            } else if (IsLineClear(1))
            {
                Debug.Log("Changing right!");
                ChangeLine(true);
            }
        }

        
        if (GameManager.Instance.gameState == GameManager.GameState.Race)
        {
            if(!brakes){
                UpdateRPM();
                CalculateAcceleration();
                acceleration *= skill;
                currentSpeed += (acceleration + (acceleration * 0.25f) * (maxSpeed - currentSpeed) / maxSpeed) * Time.deltaTime;
                ChangeGear();
                MoveForward();
            } else{
                if(currentSpeed > 0){
                        currentSpeed -= Time.deltaTime*85;
                    } else {
                        currentSpeed = 0;
                }
            }
            //CheckFinish();
                
        }
        

    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            //GetComponent<Rigidbody>().AddForce(0, 0, -currentSpeed);
            currentSpeed = 0;
            carCrashed = true;
            UnityEngine.Debug.Log("Car crashed!");
            carModel.Explode();
        }
    }*/

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Destroy(GetComponent<Rigidbody>());
            //GetComponent<Rigidbody>().AddForce(0, 0, -currentSpeed);
            currentSpeed = 0;
            carCrashed = true;
            UnityEngine.Debug.Log("Opponent crashed!");
            carModel.Explode();
        }
        else if (other.gameObject.tag == "Car")
        {
            Debug.Log("Collision with other car!");
            //GetComponent<Rigidbody>().AddForce(other.gameObject.transform.position - transform.position);
            transform.Translate(new Vector3(0, 0, (transform.position.z - other.gameObject.transform.position.z) * 0.5f));
        }
        else if (other.gameObject.tag == "Finish"){
                StartBraking();
                if(GameManager.Instance.whowon == GameManager.WhoWon.nobody){
                    GameManager.Instance.whowon = GameManager.WhoWon.opponent;
                    Debug.Log("Player lost!");
                    //GameManager.Instance.gameState = GameManager.GameState.AfterRace;
                }
        }
    }

    private void MoveForward()
    {
        if (currentSpeed > maxSpeed)
            currentSpeed = maxSpeed;
        else if (currentSpeed < 0)
            currentSpeed = 0;

        //gameObject.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
    private void CalculateAcceleration()
    {
        float ratio = 0;

        if (currentRPM <= 9000)
        {
            ratio = currentRPM / 9000;

        }
        else
        {
            ratio = 1 - (currentRPM - 9000) / 1000;
        }

        acceleration = ratio * baseAcceleration;
    }
    public void StartBraking()
    {
        if(!brakes){
            brakes = true;
            brakeSource.PlayOneShot(brakeSound);
        }
    }

    private void UpdateRPM()
    {
        //currentRPM = Mathf.Min(currentRPM + (baseRPMChange * (1.25f - speedRatio) * Time.deltaTime), maxRPM);
        float RPMChange = baseRPMChange * (0.25f + (maxSpeed - currentSpeed) / maxSpeed);
        currentRPM = Mathf.Min(maxRPM, currentRPM + RPMChange * Time.deltaTime);
    }

    private void ChangeGear()
    {
        if ((currentGear < maxGear) && (currentRPM > 9000))
        {
            currentGear++;
            float newRPM = Mathf.Max(1000, currentRPM - (maxGear + 1 - currentGear) * 1000);
            currentRPM = newRPM;
        }
    }
    //TODO START


    private bool IsLineClear(int lineDirection)
    {
        Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.right * GameManager.Instance.lineSpacing * lineDirection, new Vector3(1, 0.5f, 0.75f));
        foreach (Collider col in hitDetected)
        {

            if ((col.gameObject.tag == "Car") || (col.gameObject.tag == "Wall"))
            {
                //UnityEngine.Debug.Log("Opponent: Collision detected in line!");
                return false;
            }
        }
        return true;
    }

    private bool IsChangeNeeded()
    {
        //Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.forward * currentSpeed/5, new Vector3(1, 0.5f, 1.5f * currentSpeed / 15));
        Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.forward * currentSpeed/5, new Vector3(1, 0.5f, 1.5f * currentSpeed / 5));
        foreach (Collider col in hitDetected)
        {

            if (/*(col.gameObject.tag == "Car") || */(col.gameObject.tag == "Wall"))
            {
                //UnityEngine.Debug.Log("Opponent: Collision detected in line!");
                return true;
            }
        }
        return false;
    }
    

    private void ChangeLine(bool right)
    {
        if (right && !isChangingLines && (currentLine < linesCount) && IsLineClear(1))
        {
            //StartCoroutine(ChangeLineCoroutine(true));
            isChangingLines = true;
            currentLine++;
            destinationX += GameManager.Instance.lineSpacing;
        }

        else if (!right && !isChangingLines && (currentLine > 1) && IsLineClear(-1))
        {
            isChangingLines = true;
            //StartCoroutine(ChangeLineCoroutine(false));
            currentLine--;
            destinationX -= GameManager.Instance.lineSpacing;
            
        }
    }
    
    IEnumerator ChangeLineCoroutine(bool right)
    {
        float _distanceChanged = 0;
        isChangingLines = true;

        Vector3 direction;
        if (right == true)
        {
            currentLine++;
            direction = Vector3.right;
        }

        else
        {
            currentLine--;
            direction = Vector3.left;
        }
        

        while(_distanceChanged < GameManager.Instance.lineSpacing)
        {
            float _movement =  (currentSpeed/5f) * sideSpeed * Time.deltaTime;

            _distanceChanged += _movement;
            transform.Translate(direction * _movement);


            yield return null;
        }

        isChangingLines = false;
    }
    //TODO END
    private void CheckFinish()
    {
        if (transform.position.z > GameManager.Instance.finishZ)
        {
            if (GameManager.Instance.whowon == GameManager.WhoWon.nobody)
            {
                GameManager.Instance.whowon = GameManager.WhoWon.opponent;
                Debug.Log("Player lost!");
                GameManager.Instance.gameState = GameManager.GameState.AfterRace;
                SceneManager.LoadScene("LevelSelect");
            }
        }
    }

}
