using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarMovement : CarMovementBase
{
    
    [SerializeField] List<float> maxSpeeds;
    float currentMaxSpeed = 150;


    [SerializeField] float speedChangePerSec = 5;

    [SerializeField] float sideSpeed = 2;

    // gears 0 az maxGEar
    [SerializeField] int maxGear = 5;
    int currentGear = 1;
    float delta;

    private float currentRPM = 0f;
    private float maxRPM = 10000f;

    private float acceleration = 5;

    [SerializeField] int linesCount = 5;
    public int currentLine = 3;

    readonly float lineSpacing = 2;

    bool gameStarted = false;
    bool isChangingLines = false;

    [SerializeField]
    TextMeshProUGUI speedText;
    [SerializeField]
    TextMeshProUGUI RPMText;

    private void Awake()
    {
        myTransform = gameObject.transform;

        gameStarted = true;
    }


    public int currentLine = 3;

    
    //bool isChangingLines = false;

   



    protected override void Update()
    {
        if (GameManager.Instance.isRaceInProgress)
        {

            delta = Time.deltaTime;



            if (Input.GetKey(KeyCode.UpArrow))
            {
                GearChange(true);
            }

            else if (Input.GetKey(KeyCode.DownArrow))
            {
                GearChange(false);
            }

            if (Input.GetKey(KeyCode.RightArrow) && !isChangingLines && (currentLine < linesCount))
            {
                ChangeLine(true);
            }

            else if (Input.GetKey(KeyCode.LeftArrow) && !isChangingLines && (currentLine > 1))
            {
                ChangeLine(false);
            }

            currentSpeed += acceleration * delta;

            MoveForward();

        }
        speedText.text = ((int) currentSpeed).ToString() + "km/h";

    }



    private void GearChange(bool up)
    {
    
        if (up && (currentGear < maxGear))
        {
            currentGear++;
            
        }
            
    
        else if(!up && (currentGear > 0))
        {
            currentGear--;
        }


        //UpdateMaxSpeed(currentGear);


    }

    private void MoveForward()
    {
        if (currentSpeed > currentMaxSpeed)
            currentSpeed = currentMaxSpeed;
        else if (currentSpeed < 0)
            currentSpeed = 0;

        myTransform.Translate(Vector3.forward * currentSpeed * delta);
    }

    public override void LaneSwitch(bool right)
    {
        int _laneSwitcher = 0;

        if (right)
            _laneSwitcher++;
        else
            _laneSwitcher--;

        //aby neprapalil a soupl se max o jednu lajnu
        if (Mathf.Abs((desiredLane + _laneSwitcher) - lane) > 1)
            return;

        desiredLane += _laneSwitcher;

        
        
    }

    //TAHLE ODPOZNAMKOVANA COROUTINE JE PEKNA A KLIDNE BYCH JI POUZIL, ADAM CHCE ALE SMOOTHSTEP:)

    //private void ChangeLine(bool right)
    //{
    //    StartCoroutine(ChangeLineCoroutine(right));
    //}
    //
    //IEnumerator ChangeLineCoroutine(bool right)
    //{
    //    float _distanceChanged = 0;
    //    isChangingLines = true;
    //
    //    Vector3 direction;
    //    if (right == true)
    //    {
    //        currentLine++;
    //        direction = Vector3.right;
    //    }
    //
    //    else
    //    {
    //        currentLine--;
    //        direction = Vector3.left;
    //    }
    //        
    //    
    //
    //    while(_distanceChanged < TrackData.Instance.linesSpacing)
    //    {
    //        float _movement = sideSpeed * delta;
    //        _distanceChanged += _movement;
    //        myTransform.Translate(direction * _movement);
    //
    //
    //        yield return null;
    //    }
    //
    //    isChangingLines = false;
    //}

    private void UpdateMaxSpeed(int gear)
    {
        currentMaxSpeed = maxSpeeds[gear];
    }
        
}
