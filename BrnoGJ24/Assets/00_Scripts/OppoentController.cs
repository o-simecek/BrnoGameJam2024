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

    public OpponentGenerator carModel;

    //
    [SerializeField]
    private float skill = 0.9f;

    [SerializeField] int linesCount = 4;
    public int currentLine = 4;
    bool isChangingLines = false;
    bool haveToChange = false;
    [SerializeField] float sideSpeed = 2;
    
    private float tmp = 0f;
    private float lastVelocity = 0f;
    [SerializeField] Transform visualTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate(){
        float xVelocity = tmp - transform.position.x;

        lastVelocity = Mathf.Lerp(lastVelocity, xVelocity, 15f * Time.deltaTime);
        visualTransform.rotation = Quaternion.Euler(0, lastVelocity * -150, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (carCrashed) return;

        
        tmp = transform.position.x;

        if(IsChangeNeeded()){
            ChangeLine((Random.Range(0, 2) == 1));
        }

        
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            //GetComponent<Rigidbody>().AddForce(0, 0, -currentSpeed);
            currentSpeed = 0;
            carCrashed = true;
            UnityEngine.Debug.Log("Car crashed!");
            carModel.Explode();
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


    private bool IsLineClear(int lineDirection)
    {
        Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.right * GameManager.Instance.lineSpacing * lineDirection, new Vector3(1, 0.5f, 0.75f));
        foreach (Collider col in hitDetected)
        {

            if ((col.gameObject.tag == "Car") || (col.gameObject.tag == "Wall"))
            {
                //UnityEngine.Debug.Log("Opponent: Collision detected in line!");
                return false;
            }
        }
        return true;
    }

    private bool IsChangeNeeded()
    {
        Collider[] hitDetected = Physics.OverlapBox(transform.position + Vector3.forward * 10 * GameManager.Instance.lineSpacing, new Vector3(1, 0.5f, 0.75f));
        foreach (Collider col in hitDetected)
        {

            if ((col.gameObject.tag == "Car") || (col.gameObject.tag == "Wall"))
            {
                //UnityEngine.Debug.Log("Opponent: Collision detected in line!");
                return true;
            }
        }
        return false;
    }
    

    private void ChangeLine(bool right)
    {
        if (right && !isChangingLines && (currentLine < linesCount) && IsLineClear(1))
                {
                    StartCoroutine(ChangeLineCoroutine(true));
                }

        else if (!right && !isChangingLines && (currentLine > 1) && IsLineClear(-1))
        {
            StartCoroutine(ChangeLineCoroutine(false));
        }
    }
    
    IEnumerator ChangeLineCoroutine(bool right)
    {
        float _distanceChanged = 0;
        isChangingLines = true;

        Vector3 direction;
        if (right == true)
        {
            currentLine++;
            direction = Vector3.right;
        }

        else
        {
            currentLine--;
            direction = Vector3.left;
        }
        

        while(_distanceChanged < GameManager.Instance.lineSpacing)
        {
            float _movement =  (currentSpeed/5f) * sideSpeed * Time.deltaTime;

            _distanceChanged += _movement;
            transform.Translate(direction * _movement);


            yield return null;
        }

        isChangingLines = false;
    }
    //TODO END


}
