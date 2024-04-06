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

    

    private void Awake()
    {
        myTransform = gameObject.transform;
    }

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();

        currentPositionX = myTransform.position.x;
        currentPositionZ = myTransform.position.z;
        finalPositionX = currentPositionX;
        desiredLane = lane;
    }


    protected virtual void Update()
    {
        
        if(desiredLane != lane)
        {
            finalPositionX = desiredLane * TrackData.Instance.LanesSpacing;
            currentPositionX = Mathf.SmoothStep(currentPositionX, finalPositionX, 30f * Time.deltaTime);
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
        currentPositionZ += currentSpeed * Time.deltaTime;
        myTransform.position = new Vector3(currentPositionX, 0, currentPositionZ);
    }

    public virtual void LaneSwitch(bool right) { }
       
}
