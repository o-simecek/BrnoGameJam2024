using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource audioSourceAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartAction()
    {
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn()
    {   
        audioSource.volume = 0;
        audioSourceAction.volume = 0.15f;
        for (float musicVolume = 0.15f; musicVolume >= 0; musicVolume -= 0.1f*Time.deltaTime){
            float actionVolume = 0.15f - musicVolume;
            audioSourceAction.volume = actionVolume;
            audioSource.volume = musicVolume;
            yield return null;
        }
    }

}
