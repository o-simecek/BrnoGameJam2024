using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;


public class CarMovement : MonoBehaviour
{
        
    public MusicManager levelMusic;
    private AudioSource audioSource;
    public AudioClip engineSound;
    public AudioClip shiftSound;
    public AudioClip brakeSound;
    public AudioSource brakeSource;

    //Car states
    bool carStarted = false;
    public bool music = false;

    //Speed
    float maxSpeed = 150;
    public float currentSpeed = 0;

    //Gears
    [SerializeField] int maxGear = 5;
    int currentGear = 0;
    bool carCrashed = false;

    public Transform rpmGauge;
    public Transform speedGauge;
    public Transform carCamera;
    public Vector3 baseCamera;

    //RPM
    public float currentRPM = 0f;
    private float destinationX = 0f;
    private float currentX = 0f;
    private float startX = 0f;
    private float startZ = 0f;
    private float startY = 0f;
    private static float maxRPM = 10000f;
    [SerializeField]
    private static float baseRPMChange = 500f;

    //Acceleration
    [SerializeField]
    private static float baseAcceleration = 4;
    private float acceleration = 0;
    private bool brakes = false;
    public bool infiniteMode = false;
    
    public OpponentGenerator carModel;

    //Line changing
    [SerializeField] int linesCount = 4;
    [SerializeField]
    public int currentLine = 2;
    bool isChangingLines = false;
    [SerializeField] float sideSpeed = 2;

    RaycastHit m_Hit;

    //UI
    [SerializeField]
    TextMeshProUGUI speedText;
    [SerializeField]
    TextMeshProUGUI gearText;
    [SerializeField]
    TextMeshProUGUI RPMText;
    [SerializeField] Transform visualTransform;
    [SerializeField]
    TextMeshProUGUI shiftText;
    [SerializeField]
    TextMeshProUGUI wonText;

    private void Awake()
    {
        destinationX = transform.position.x;
        currentX = transform.position.x;
        startX = transform.position.x;
        startZ = transform.position.z;
        startY = transform.position.y;
        audioSource = GetComponent<AudioSource>();
        baseCamera = carCamera.localPosition;
    }

    private void LateUpdate(){
        //float xVelocity = tmp - transform.position.x;

        

        //lastVelocity = Mathf.Lerp(lastVelocity, xVelocity, 15f * Time.deltaTime);
        //visualTransform.rotation = Quaternion.Euler(0, lastVelocity * -150, 0);

    }
    
    private void Update()
    {
        carCamera.localPosition = new Vector3(carCamera.localPosition.x, carCamera.localPosition.y, baseCamera.z + currentSpeed/-65);
        carCamera.rotation = Quaternion.Euler(currentSpeed/1000*Mathf.PerlinNoise(5 + Time.time * currentSpeed/10, 0), currentSpeed/2000*Mathf.PerlinNoise(10 + Time.time * currentSpeed/10, 0), currentSpeed/1000*Mathf.PerlinNoise(Time.time * currentSpeed/10, 0));
        audioSource.pitch = 0.5f + currentRPM/10000f;
        rpmGauge.rotation = Quaternion.Euler(0, 0, 140 - (currentRPM/36));

        if (currentSpeed > 20){
            
            speedGauge.rotation = Quaternion.Euler(0, 0, 140 - ((currentSpeed - 20) * 280/180));
        }

        //tmp = transform.position.x;

        if (carCrashed){
            audioSource.volume = 0;
            return;
        }

        //currentX = Mathf.SmoothStep(startX, destinationX, (Time.time - startTime) * (0.5f + (20f*currentSpeed/500f)));
        currentX = Mathf.SmoothStep(currentX, destinationX, (8.5f + (currentSpeed/8.5f)) * Time.deltaTime);

        if(Mathf.Abs(destinationX - currentX) > GameManager.Instance.lineSpacing/2f){
            visualTransform.rotation = Quaternion.Euler(currentRPM/50000 * Mathf.PerlinNoise(Time.time * 35, 0), (startX - currentX) * -3.5f, currentRPM/50000 * Mathf.PerlinNoise(Time.time * 25, 0));
        } else{
            visualTransform.rotation = Quaternion.Euler(currentRPM/50000 * Mathf.PerlinNoise(Time.time * 35, 0), (destinationX - currentX) * 3.5f, currentRPM/50000 * Mathf.PerlinNoise(Time.time * 25, 0));
        }

        transform.position = new Vector3 (currentX, startY, transform.position.z + currentSpeed * Time.deltaTime); 

        if (Mathf.Abs(destinationX - currentX) < 0.1f){
            isChangingLines = false;
            startX = destinationX;
        }

        RPMText.text = "RPM: " + ((int)(currentRPM*0.7f)).ToString();
        CalculateAcceleration();

        switch (GameManager.Instance.gameState)
        {
            //RACE IS STARTING
            case GameManager.GameState.BeforeRace:

                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    currentRPM = Mathf.Min(currentRPM + 4500 * Time.deltaTime, maxRPM);
                }
                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    currentRPM = Mathf.Max(currentRPM - 4500 * Time.deltaTime, 0);
                }
                else
                {
                    currentRPM = Mathf.Max(currentRPM - 2000 * Time.deltaTime, 0);
                }

                break;

            //RACE IS IN PROGRESS
            case GameManager.GameState.Race:
                
                UpdateRPM();
                //CheckFinish();

                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftShift))
                {
                    GearChange(true);
                }

                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //GearChange(false);
                    GetComponent<Rigidbody>().AddForce(0, 0, 10);
                }

                if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && !isChangingLines && (currentLine < linesCount) && IsLineClear(1))
                {
                    ChangeLine(true);
                }

                else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && !isChangingLines && (currentLine > 1) && IsLineClear(-1))
                {
                    ChangeLine(false);
                }

                if(!brakes){

                    currentSpeed += (acceleration + (acceleration * 0.25f) * (maxSpeed - currentSpeed) / maxSpeed) * Time.deltaTime;

                    MoveForward();
                } else {
                    currentRPM = 0;
                    if(currentSpeed > 0){
                        currentSpeed -= Time.deltaTime*85;
                    } else {
                        currentSpeed = 0;
                    }
                }

                speedText.text = ((int)currentSpeed).ToString() + "km/h";
                break;
                /*
            case GameManager.GameState.AfterRace:currentRPM = 0;
                if(currentSpeed > 0){
                    currentSpeed -= Time.deltaTime*85;
                } else {
                    currentSpeed = 0;
                }
                break;
                */
        }
        if (GameManager.Instance.whowon == GameManager.WhoWon.player)
        {
            wonText.text = "YOU WON!";
            StartCoroutine(GameManager.Instance.LoadLevelSelect());
        }
        if (GameManager.Instance.whowon == GameManager.WhoWon.opponent)
        {
            wonText.text = "YOU LOST!";
            StartCoroutine(GameManager.Instance.LoadLevelSelect());
        }

        HandleFirstGearChange();
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            if(infiniteMode){
                wonText.text = "SCORE: " +  (int)(transform.position.z - startZ);
            } else{
                wonText.text = "CRASHED!";
            }
            //shiftText.text = "Crashed!";
            GetComponent<Rigidbody>().AddForce(0, 0, -currentSpeed);
            currentSpeed = 0;
            carCrashed = true;
            UnityEngine.Debug.Log("Car crashed!");
            carModel.Explode();
            StartCoroutine(GameManager.Instance.LoadLevelSelect());
        } else if (other.gameObject.tag == "Car") {
            Debug.Log("Collision with other car!");
            transform.Translate(new Vector3(0, 0, (transform.position.z - other.gameObject.transform.position.z) * 0.5f));
            //GetComponent<Rigidbody>().AddForce((other.gameObject.transform.position - transform.position) * 100);
        } else if (other.gameObject.tag == "Finish"){
            StartBraking();
            if (GameManager.Instance.gameState == GameManager.GameState.Race){
                //GameManager.Instance.gameState = GameManager.GameState.AfterRace;
                if (GameManager.Instance.whowon == GameManager.WhoWon.nobody)
                {
                    GameManager.Instance.whowon = GameManager.WhoWon.player;
                    Debug.Log("Player won!");
                    GameManager.Instance.clearedLevels.Add(SceneManager.GetActiveScene().name);
                    //StartCoroutine(GameManager.Instance.LoadLevelSelect());
                }
                else if (GameManager.Instance.whowon == GameManager.WhoWon.opponent)
                {
                    Debug.Log("Player lost!");

                }
            }
        }
    }



    private void GearChange(bool up)
    {
        if ((up && (currentGear < maxGear) && (currentRPM > 5000)) || (currentGear == 0))
        {
            float bonusSpeed = CalculateBonusSpeed();
            if (currentGear != 0)
            {
                currentSpeed += bonusSpeed;
            }

            if (currentGear == 0 && currentRPM > 9000)
            {
                currentRPM = 1000;
                UnityEngine.Debug.Log("Bad start!");
            }

            currentGear++;
            float newRPM = Mathf.Max(1000, currentRPM - (maxGear + 1 - currentGear) * 1000);
            currentRPM = newRPM;


            UnityEngine.Debug.Log("Gear: " + currentGear.ToString() + "; " + "RPM change: " + (currentRPM - newRPM).ToString());
        }

        else if(!up && (currentGear > 0))
        {
            currentGear--;
        }
    }

    private void MoveForward()
    {
        if (currentSpeed > maxSpeed)
            currentSpeed = maxSpeed;
        else if (currentSpeed < 0)
            currentSpeed = 0;

        //transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void ChangeLine(bool right)
    {
        //StartCoroutine(ChangeLineCoroutine(right));
        isChangingLines = true;
        //startTime = Time.time;
        if (right){
            currentLine++;
            destinationX += GameManager.Instance.lineSpacing;
        }else{
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
            float _movement =  sideSpeed * Time.deltaTime;

            _distanceChanged += _movement;
            transform.Translate(direction * _movement);


            yield return null;
        }

        isChangingLines = false;
    }

    private void HandleFirstGearChange()
    {
        if (!carStarted)
        {
            if (GameManager.Instance.gameState == GameManager.GameState.Race)
            {
                GearChange(true);
                carStarted = true;
                if(music)
                    levelMusic.StartAction();
            }
        }
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

    private void UpdateRPM()
    {
        //currentRPM = Mathf.Min(currentRPM + (baseRPMChange * (1.25f - speedRatio) * Time.deltaTime), maxRPM);
        float RPMChange = baseRPMChange * (0.25f + (maxSpeed - currentSpeed) / maxSpeed);
        currentRPM = Mathf.Min(maxRPM, currentRPM + RPMChange * Time.deltaTime);
    }
    public void StartBraking()
    {
        if(!brakes){
            brakes = true;
            brakeSource.PlayOneShot(brakeSound);
        }
    }

    private float CalculateBonusSpeed()
    {
        StartCoroutine(ClearShiftText());
        gearText.text = (currentGear + 1).ToString();
        audioSource.PlayOneShot(shiftSound);
        List<float> bonus = new List<float> { -15, 0, -3, -6 };
        if (currentRPM > 9000)
        {
            //UnityEngine.Debug.Log("bad!");
            shiftText.text = "BAD shift!";
            shiftText.color = Color.red;
            return bonus[0];
        }
        else if (currentRPM > 8800)
        {
            //UnityEngine.Debug.Log("perfect!");
            shiftText.text = "PERFECT shift!";
            shiftText.color = Color.blue;
            return bonus[1];
        }
        else if (currentRPM > 8000)
        {
            //UnityEngine.Debug.Log("good!");
            shiftText.text = "GOOD shift";
            shiftText.color = Color.green;
            return bonus[2];
        }
        else
        {
            //UnityEngine.Debug.Log("ok");
            shiftText.text = "OK shift";
            shiftText.color = Color.white;
            return bonus[3];
        }
    }

    IEnumerator ClearShiftText()
    {
        yield return new WaitForSeconds(1);
        shiftText.text = "";
        shiftText.color = Color.white;
    }

    //-1 for left, +1 for right
    private bool IsLineClear(int lineDirection) {
        Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.right * GameManager.Instance.lineSpacing * lineDirection, new Vector3(1, 0.5f, 1.5f));
        foreach (Collider col in hitDetected) {

            if ((col.gameObject.tag == "Car") || (col.gameObject.tag == "Wall"))
            {
                UnityEngine.Debug.Log("Collision detected in line!");
                return false;
            }
        }
        return true;
    }

    private void CheckFinish()
    {
        if (transform.position.z > GameManager.Instance.finishZ)
        {
            if (GameManager.Instance.whowon == GameManager.WhoWon.nobody)
            {
                GameManager.Instance.whowon = GameManager.WhoWon.player;
                Debug.Log("Player won!");
                wonText.text = "You have won!";
                GameManager.Instance.gameState = GameManager.GameState.AfterRace;
                GameManager.Instance.clearedLevels.Add(SceneManager.GetActiveScene().name);
                StartCoroutine(GameManager.Instance.LoadLevelSelect());
            } else if (GameManager.Instance.whowon == GameManager.WhoWon.opponent)
            {
                Debug.Log("Player lost!");
                wonText.text = "You have lost!";
                StartCoroutine(GameManager.Instance.LoadLevelSelect());

            }
        }
    }
}
