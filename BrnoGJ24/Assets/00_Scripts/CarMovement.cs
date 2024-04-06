using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : CarMovementBase
{
    
    [SerializeField] List<float> maxSpeeds;
    float currentMaxSpeed = 0;


    [SerializeField] float speedChangePerSec = 5;

    [SerializeField] float sideSpeed = 2;

    // gears 0 az maxGEar
    [SerializeField] int maxGear = 5;
    int currentGear = 1;
    float delta;

    public int currentLine = 3;

    
    //bool isChangingLines = false;

   



    protected override void Update()
    {
        //if (!gameStarted) return;

        base.Update();

        if(currentSpeed < maxSpeed)
        {
            currentSpeed += speedChangePerSec * delta;
        }

        delta = Time.deltaTime;
        

        if (Input.GetKey(KeyCode.UpArrow))
        {
            GearChange(true);
        }

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            GearChange(false);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && (currentLine < TrackData.Instance.LanesCount))
        {
            LaneSwitch(true);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow) && (currentLine > 1))
        {
            LaneSwitch(false);
        }

        //MoveForward();
        Move();
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
