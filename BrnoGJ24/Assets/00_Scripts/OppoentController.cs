using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UIElements;

public class OppoentController : MonoBehaviour
{
    bool carCrashed = false;

    //Speed
    float maxSpeed = 150;
    public float currentSpeed = 0;

    //Gears
    [SerializeField] int maxGear = 5;
    int currentGear = 1;

    //RPM
    private float currentRPM = 4000f;
    private static float maxRPM = 10000f;
    [SerializeField]
    private static float baseRPMChange = 500f;

    //Acceleration
    [SerializeField]
    private static float baseAcceleration = 4;
    private float acceleration = 0;

    //
    [SerializeField]
    private float skill = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carCrashed) return;
        
        if (GameManager.Instance.gameState == GameManager.GameState.Race)
        {
            UpdateRPM();
            CalculateAcceleration();
            acceleration *= skill;
            currentSpeed += (acceleration + (acceleration * 0.25f) * (maxSpeed - currentSpeed) / maxSpeed) * Time.deltaTime;
            ChangeGear();
            MoveForward();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            GetComponent<Rigidbody>().AddForce(0, 0, -currentSpeed);
            currentSpeed = 0;
            carCrashed = true;
            UnityEngine.Debug.Log("Car crashed!");
        }
    }

    private void MoveForward()
    {
        if (currentSpeed > maxSpeed)
            currentSpeed = maxSpeed;
        else if (currentSpeed < 0)
            currentSpeed = 0;

        gameObject.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
    private void CalculateAcceleration()
    {
        float ratio = 0;

        if (currentRPM <= 9000)
        {
            ratio = currentRPM / 9000;

        }
        else
        {
            ratio = 1 - (currentRPM - 9000) / 1000;
        }

        acceleration = ratio * baseAcceleration;
    }

    private void UpdateRPM()
    {
        //currentRPM = Mathf.Min(currentRPM + (baseRPMChange * (1.25f - speedRatio) * Time.deltaTime), maxRPM);
        float RPMChange = baseRPMChange * (0.25f + (maxSpeed - currentSpeed) / maxSpeed);
        currentRPM = Mathf.Min(maxRPM, currentRPM + RPMChange * Time.deltaTime);
    }

    private void ChangeGear()
    {
        if ((currentGear < maxGear) && (currentRPM > 9000))
        {
            currentGear++;
            float newRPM = Mathf.Max(1000, currentRPM - (maxGear + 1 - currentGear) * 1000);
            currentRPM = newRPM;
        }
    }
    //TODO START
    private void CheckForObstacles()
    {
        Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.forward * 10, new Vector3(0.5f, 0.5f, 20f));
        foreach (Collider col in hitDetected)
        {
            if (col.gameObject.tag == "Wall")
            {
                UnityEngine.Debug.Log("Opponent: Collision detected in current line!");
            }
        }
    }

    private bool IsLineClear(int lineDirection)
    {
        Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.right * GameManager.Instance.lineSpacing * lineDirection, new Vector3(1, 0.5f, 0.75f));
        foreach (Collider col in hitDetected)
        {

            if (col.gameObject.tag == "Car")
            {
                UnityEngine.Debug.Log("Opponent: Collision detected in line!");
                return false;
            }
        }
        return true;
    }
    //TODO END


}
