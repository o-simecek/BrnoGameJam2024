using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float maxSpeed = 20;
    public float fixedSpeed = 20;
    public bool hasFixedSpeed = false;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(1, maxSpeed);
        if (hasFixedSpeed)
            speed = fixedSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.gameState == GameManager.GameState.Race)
            transform.Translate(new Vector3(speed, 0 , 0) * Time.deltaTime);
    }
}
