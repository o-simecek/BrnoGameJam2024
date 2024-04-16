using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class StartCounter : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textObject;
    float timer = 3f;

    private bool counterStarted = false;
    private bool counterEnded = false;
    private Vector3 baseScale;
    // Start is called before the first frame update
    void Start()
    {
        baseScale = textObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (counterStarted && !counterEnded)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                textObject.transform.localScale = new Vector3(1 - timer * 0.1f, 1 - timer * 0.1f, 1);
                textObject.text = "Go!";
                GameManager.Instance.gameState = GameManager.GameState.Race;
            }
            else if (timer < 1)
            {
                textObject.text = "1";
            }
            else if (timer < 2)
            {
                textObject.text = "2";
            }

            if (timer < -3)
            {
                textObject.text = "";
                counterEnded = true;
                textObject.transform.localScale = baseScale;
            }
        }
        

        if(Input.anyKey && !counterStarted)
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        counterStarted = true;
        textObject.text = "3";
    }
}
