using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;
using System;

public class CarMovement : MonoBehaviour
{
    //Car states
    bool carStarted = false;

    //Speed
    float maxSpeed = 150;
    public float currentSpeed = 0;

    //Gears
    [SerializeField] int maxGear = 5;
    int currentGear = 0;
    bool carCrashed = false;

    public Transform rpmGauge;
    public Transform speedGauge;

    //RPM
    public float currentRPM = 0f;
    private float tmp = 0f;
    private float lastVelocity = 0f;
    private static float maxRPM = 10000f;
    [SerializeField]
    private static float baseRPMChange = 500f;

    //Acceleration
    [SerializeField]
    private static float baseAcceleration = 4;
    private float acceleration = 0;

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
    TextMeshProUGUI RPMText;
    [SerializeField] Transform visualTransform;
    [SerializeField]
    TextMeshProUGUI shiftText;

    private void Awake()
    {

    }

    private void LateUpdate(){
        float xVelocity = tmp - transform.position.x;

        

        lastVelocity = Mathf.Lerp(lastVelocity, xVelocity, 15f * Time.deltaTime);
        visualTransform.rotation = Quaternion.Euler(0, lastVelocity * -150, 0);

    }
    
    private void Update()
    {

        rpmGauge.rotation = Quaternion.Euler(0, 0, 140 - (currentRPM/36));

        if (currentSpeed > 20){
            
            speedGauge.rotation = Quaternion.Euler(0, 0, 140 - ((currentSpeed - 20) * 280/180));
        }

        tmp = transform.position.x;

        if (carCrashed) return;

        RPMText.text = "RPM: " + ((int)(currentRPM*0.7f)).ToString();
        CalculateAcceleration();

        switch (GameManager.Instance.gameState)
        {
            //RACE IS STARTING
            case GameManager.GameState.BeforeRace:

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    currentRPM = Mathf.Min(currentRPM + 4500 * Time.deltaTime, maxRPM);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
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
                CheckFinish();

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    GearChange(true);
                }

                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //GearChange(false);
                    GetComponent<Rigidbody>().AddForce(0, 0, 10);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow) && !isChangingLines && (currentLine < linesCount) && IsLineClear(1))
                {
                    ChangeLine(true);
                }

                else if (Input.GetKeyDown(KeyCode.LeftArrow) && !isChangingLines && (currentLine > 1) && IsLineClear(-1))
                {
                    ChangeLine(false);
                }

                currentSpeed += (acceleration + (acceleration * 0.25f) * (maxSpeed - currentSpeed) / maxSpeed) * Time.deltaTime;

                MoveForward();

                speedText.text = ((int)currentSpeed).ToString() + "km/h";
                break;
        }

        HandleFirstGearChange();
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            shiftText.text = "Crashed!";
            GetComponent<Rigidbody>().AddForce(0, 0, -currentSpeed);
            currentSpeed = 0;
            carCrashed = true;
            UnityEngine.Debug.Log("Car crashed!");
            //TODO carModel explode
        } else if (other.gameObject.tag == "Car") {
            Debug.Log("Collision with other car!");
            transform.Translate(new Vector3(0, 0, (transform.position.z - other.gameObject.transform.position.z) * 0.5f));
            //GetComponent<Rigidbody>().AddForce((other.gameObject.transform.position - transform.position) * 100);
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

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void ChangeLine(bool right)
    {
        StartCoroutine(ChangeLineCoroutine(right));
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

    private void HandleFirstGearChange()
    {
        if (!carStarted)
        {
            if (GameManager.Instance.gameState == GameManager.GameState.Race)
            {
                GearChange(true);
                carStarted = true;
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

    private float CalculateBonusSpeed()
    {
        StartCoroutine(ClearShiftText());

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
                GameManager.Instance.gameState = GameManager.GameState.AfterRace;
                GameManager.Instance.clearedLevels.Add(SceneManager.GetActiveScene().name);
                StartCoroutine(GameManager.Instance.LoadLevelSelect());
            } else if (GameManager.Instance.whowon == GameManager.WhoWon.opponent)
            {
                Debug.Log("Player lost!");
                StartCoroutine(GameManager.Instance.LoadLevelSelect());

            }
        }
    }
}
