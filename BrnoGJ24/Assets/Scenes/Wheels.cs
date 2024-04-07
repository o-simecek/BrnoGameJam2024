using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheels : MonoBehaviour
{

    public CarMovement carMovement;
    public OppoentController opponent;

    public int direction = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carMovement != null){
            transform.Rotate(carMovement.currentSpeed * direction, 0, 0);

        }
        if (opponent != null){
            transform.Rotate(opponent.currentSpeed * direction, 0, 0);

        }
    }
}
