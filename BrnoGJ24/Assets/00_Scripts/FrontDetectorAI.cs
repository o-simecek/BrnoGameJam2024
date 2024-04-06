using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDetectorAI : MonoBehaviour
{
    [SerializeField] CarMovementBase carMovement;

    private void OnTriggerStay(Collider other)
    {
        bool right = false;

        if (other.gameObject.layer == 6 || other.gameObject.layer == 3)
        {
            if (Random.Range(0, 2) == 0)
            {
                right = true;
            }
            carMovement.LaneSwitch(right);
        }
    }
}
