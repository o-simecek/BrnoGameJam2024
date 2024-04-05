using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementBase : MonoBehaviour
{
    protected Transform myTransform;

    
    public float maxSpeed = 300;
    protected float currentSpeed = 1;
    protected int gear = 1;
    public int lane = 0;
    public int desiredLane;
    protected float finalPositionX;
    protected float currentPositionX;
    protected float currentPositionZ;
    protected Rigidbody rigid;
    //protected bool switchLock = false;
    

    private void Awake()
    {
        myTransform = gameObject.transform;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // transform.position = new Vector3(transform.position.x, transform.position.y, lane * -3);
        rigid = GetComponent<Rigidbody>();
        //currentPosition = lane * -3;
        currentPositionX = myTransform.position.x;
        currentPositionZ = myTransform.position.z;
        finalPositionX = currentPositionX;
        desiredLane = lane;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //if (currentSpeed > 0)
        //{
        //    currentSpeed += maxSpeed / currentSpeed / 1000;
        //    transform.Translate(currentSpeed * Time.deltaTime, 0, 0);
        //    transform.position = new Vector3(transform.position.x, transform.position.y, currentPosition);
        //    currentPosition = Mathf.SmoothStep(currentPosition, finalPosition, 30f * Time.deltaTime);
        //    if (Mathf.Abs(currentPosition) > Mathf.Abs(finalPosition) - 0.1)
        //    {
        //        switchLock = false;
        //    }
        //}
        if(desiredLane != lane)
        {
            finalPositionX = desiredLane * TrackData.Instance.LanesSpacing;
            currentPositionX = Mathf.SmoothStep(currentPositionX, finalPositionX, 10f * Time.deltaTime);
        }

        if (Mathf.Abs(currentPositionX) > Mathf.Abs(finalPositionX) - 0.1)
        {
            lane = desiredLane;
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

    protected void Move()
    {
        currentPositionZ += currentSpeed;
        myTransform.position = new Vector3(currentPositionX, 0, currentPositionZ);
    }

    public virtual void LaneSwitch(bool right)
    {
        //int laneSwitcher = 0;
        //
        //if (rightCheck)
        //{
        //    laneSwitcher = -1;
        //    if (lane == 0 || leftCheck)
        //        laneSwitcher = 0;
        //}
        //else if (leftCheck)
        //{
        //    laneSwitcher = 1;
        //    if (lane == 3 || rightCheck)
        //        laneSwitcher = 0;
        //}
        //
        //print(laneSwitcher);
        //
        //lane += laneSwitcher;
        //
        //finalPosition = lane * -3;

    }
    //public void LaneSwitch()
    //{
    //    int laneSwitcher = 0;
    //    if (!switchLock)
    //    {
    //
    //        if (Random.Range(0, 1) == 0)
    //        {
    //            laneSwitcher = 1;
    //        }
    //        else
    //        {
    //            laneSwitcher = -1;
    //        }
    //
    //        if (rightCheck)
    //        {
    //            laneSwitcher = -1;
    //            if (lane == 0 || leftCheck)
    //                laneSwitcher = 0;
    //        }
    //        else if (leftCheck)
    //        {
    //            laneSwitcher = 1;
    //            if (lane == 3 || rightCheck)
    //                laneSwitcher = 0;
    //        }
    //
    //        print(laneSwitcher);
    //        switchLock = true;
    //
    //        lane += laneSwitcher;
    //
    //        finalPosition = lane * -3;
    //    }
    //
    //}
}
