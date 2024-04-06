using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementAI : CarMovementBase
{
    bool switchLock = false;

    public bool leftCheck = false;
    public bool rightCheck = false;

    protected override void Update()
    {
        base.Update();

        if (currentSpeed > 0)
        {
            //Adame, wtf je tenhle vypocet rychlosti?:D
            currentSpeed += maxSpeed / currentSpeed / 1000;

            if (Mathf.Abs(currentPositionX) > Mathf.Abs(finalPositionX) - 0.1)
            {
                switchLock = false;
            }

            Move();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 6)
        {
            rigid.AddForce(currentSpeed * 1000, currentSpeed * 10, 0);
            currentSpeed = 0;
        }
    }

    public override void LaneSwitch(bool right)
    {
        int _laneSwitcher = 0;
        if (!switchLock)
        {
            if (right)
                _laneSwitcher = 1;
            else
                _laneSwitcher = -1;


            if (rightCheck)
            {
                _laneSwitcher = -1;
                if (lane == 0 || leftCheck)
                    _laneSwitcher = 0;
            }
            else if (leftCheck)
            {
                _laneSwitcher = 1;
                if (lane == 3 || rightCheck)
                    _laneSwitcher = 0;
            }

            //aby neprapalil a soupl se max o jednu lajnu
            if (Mathf.Abs((desiredLane + _laneSwitcher) - lane) > 1)
                return;

            desiredLane += _laneSwitcher;
        }
    }

    
}
