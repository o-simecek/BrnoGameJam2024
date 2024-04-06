using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovement_Player : NewMovement
{
    private void Update()
    {
        SpeedUp();


        if (Input.GetKey(KeyCode.UpArrow))
        {
            
        }

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && (lane < TrackData.Instance.LanesCount))
        {
            ChangeLane(true);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow) && (lane> 1))
        {
            ChangeLane(false);
        }

        Move();
    }
}
