using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovement : MonoBehaviour
{
    protected Transform myTransform;

    //rychlosti v metrech/sekundu
    public float maxSpeed = 5;
    public float sideSpeed = 2;
    public float currentSpeed = 1;
    public float velocityChange = 1;
    public int gear = 1;
    public int lane = 1;

    protected float moveX;
    protected float moveZ;

    protected bool isChangingLanes = false;


    private void Awake()
    {
        myTransform = gameObject.transform;
        myTransform.position = Vector3.right * lane * TrackData.Instance.LanesSpacing;
    }

    public void SpeedUp(float velocityChange)
    {
        if (currentSpeed < maxSpeed)
            currentSpeed += velocityChange;

        moveZ = currentSpeed * Time.deltaTime;
    }

    public void SpeedUp()
    {
        if (currentSpeed < maxSpeed)
            currentSpeed += velocityChange;

        moveZ = currentSpeed * Time.deltaTime;
    }


    protected void Move()
    {
        Vector3 _move = new Vector3(moveX, 0, moveZ);
        myTransform.Translate(_move);
        //Debug.Log(_move);
    }

    protected void ChangeLane(bool right)
    {
        if (isChangingLanes) return;
        StartCoroutine(ChangeLineCoroutine(right));
    }
    
    IEnumerator ChangeLineCoroutine(bool right)
    {
        Debug.Log("lane change START");
        float _distanceChanged = 0;
        isChangingLanes = true;


        //-1 for left, +1 for right
        int direction;
        if (right == true)
        {
            lane++;
            direction = 1;
        }
    
        else
        {
            lane--;
            direction = -1;
        }
            
        
    
        while(Mathf.Abs(_distanceChanged) < TrackData.Instance.LanesSpacing)
        {
            moveX = sideSpeed * Time.deltaTime * direction;
            _distanceChanged += moveX;

            Debug.Log(_distanceChanged);
            yield return null;
        }

        //zarovna presne tam, kde ho chceme
        float _xPosition = lane * TrackData.Instance.LanesSpacing;
        myTransform.position = new Vector3(_xPosition, 0, myTransform.position.z);

        moveX = 0;
    
        isChangingLanes = false;
        Debug.Log("lane change FINISHED");
    }
}
