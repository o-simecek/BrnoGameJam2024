using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class CarMovement : MonoBehaviour
{
    Transform myTransform;

    //[SerializeField] float maxSpeed = 150;
    [SerializeField] List<float> maxSpeeds;
    float currentMaxSpeed = 150;

    public float currentSpeed = 0;
    [SerializeField] float speedChangePerSec = 5;

    [SerializeField] float sideSpeed = 2;

    [SerializeField] int maxGear = 5;
    int currentGear = 0;
    bool carStarted = false;

    private float currentRPM = 0f;
    private static float maxRPM = 10000f;


    private float acceleration = 5;

    //Line changing
    [SerializeField] int linesCount = 5;
    public int currentLine = 3;
    readonly float lineSpacing = 2;
    bool isChangingLines = false;

    [SerializeField]
    TextMeshProUGUI speedText;
    [SerializeField]
    TextMeshProUGUI RPMText;

    private void Awake()
    {
        myTransform = gameObject.transform;
    }
    

    private void Update()
    {
        RPMText.text = "RPM: " + ((int)currentRPM).ToString(); 

        switch (GameManager.Instance.gameState)
        {
            //RACE IS STARTING
            case GameManager.GameState.BeforeRace:

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    currentRPM = Mathf.Min(currentRPM + 3000 * Time.deltaTime, maxRPM);
                }

                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    currentRPM = Mathf.Max(currentRPM - 3000 * Time.deltaTime, 0);
                }

                break;

            //RACE IS IN PROGRESS
            case GameManager.GameState.Race:
                currentRPM = Mathf.Min(maxRPM, currentRPM + 1000 * Time.deltaTime);

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    GearChange(true);
                }

                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    GearChange(false);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow) && !isChangingLines && (currentLine < linesCount))
                {
                    ChangeLine(true);
                }

                else if (Input.GetKeyDown(KeyCode.LeftArrow) && !isChangingLines && (currentLine > 1))
                {
                    ChangeLine(false);
                }

                currentSpeed += acceleration * Time.deltaTime;

                MoveForward();

                speedText.text = ((int)currentSpeed).ToString() + "km/h";
                break;
        }

        HandleFirstGearChange();
    }


    private void GearChange(bool up)
    {
        if ((up && (currentGear < maxGear) && (currentRPM > 5000)) || (currentGear == 0))
        {
            currentGear++;
            float newRPM = Mathf.Max(1000, currentRPM - (maxGear + 1 - currentGear) * 1000);
            UnityEngine.Debug.Log("Gear: " + currentGear.ToString() + "; " + "RPM change: " + (currentRPM - newRPM).ToString());
            currentRPM = newRPM;
        }

        else if(!up && (currentGear > 0))
        {
            currentGear--;
        }
    }

    private void MoveForward()
    {
        if (currentSpeed > currentMaxSpeed)
            currentSpeed = currentMaxSpeed;
        else if (currentSpeed < 0)
            currentSpeed = 0;

        myTransform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
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
        

        while(_distanceChanged < lineSpacing)
        {
            float _movement = sideSpeed * Time.deltaTime;
            _distanceChanged += _movement;
            myTransform.Translate(direction * _movement);


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
}
