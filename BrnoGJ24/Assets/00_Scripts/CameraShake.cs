using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    CinemachineVirtualCamera _cvcamera;
    CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;

    [Header("Camera shake")]
    [SerializeField] float _maxAmplitude = 2f;
    [SerializeField] float _maxFrequency = 2f;
    [SerializeField] float _duration = 0.5f;

    private void Awake()
    {
        _cvcamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineBasicMultiChannelPerlin = _cvcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }


    public void CameraShakeMethod()
    {
        StartCoroutine(CameraShakeCoroutine());
    }

    IEnumerator CameraShakeCoroutine()
    {
        float timer = 0;
        float lerp;

        while (timer < _duration)
        {
            timer += Time.deltaTime;
            lerp = timer / _duration;

            _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_maxAmplitude, 0, lerp);
            _cinemachineBasicMultiChannelPerlin.m_FrequencyGain = Mathf.Lerp(_maxFrequency, 0, lerp);

            yield return null;
        }
    }
}
