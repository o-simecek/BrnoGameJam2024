using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDetectorAI : MonoBehaviour
{
    public bool left = false;

    public CarMovementAI carMovement;
    

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.layer == 6 || other.gameObject.layer == 3)
        {
            if (left)
            {
                carMovement.leftCheck = true;
            }
            else
            {
                carMovement.rightCheck = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.layer == 6 || other.gameObject.layer == 3)
        {
            if (left)
            {
                carMovement.leftCheck = false;
            }
            else
            {
                carMovement.rightCheck = false;
            }
        }
    }
}
