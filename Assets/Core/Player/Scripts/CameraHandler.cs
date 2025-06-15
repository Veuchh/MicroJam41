using Core.Player;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] float rocketScreenShakeDuration;
    [SerializeField] float screenShakeAmplitude;
    [SerializeField] float screenShakeFrequency;


    CinemachineBasicMultiChannelPerlin cinemachineNoise;
    bool isScreenShakeOn;
    float screenShakeEndTime;

    private void Awake()
    {
        cinemachineCamera.transform.parent = null;
        cinemachineNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineNoise.enabled = false;
        PlayerRocket.OnRocketFired += OnRocketFired;
    }

    private void OnDestroy()
    {
        PlayerRocket.OnRocketFired -= OnRocketFired;
    }

    private void Update()
    {
        if (isScreenShakeOn && screenShakeEndTime < Time.time)
        {
            isScreenShakeOn = false;
            cinemachineNoise.AmplitudeGain = 0;
            cinemachineNoise.FrequencyGain = 0;
            cinemachineNoise.enabled = false;
        }
    }

    private void OnRocketFired(Vector2 _, Vector2 __)
    {
        ScreenShakeForDuration(rocketScreenShakeDuration);
    }

    public void ScreenShakeForDuration(float duration)
    {
        if (Time.time + duration > screenShakeEndTime)
        {
            isScreenShakeOn = true;
            screenShakeEndTime = Time.time + duration;
            cinemachineNoise.AmplitudeGain = screenShakeAmplitude;
            cinemachineNoise.FrequencyGain = screenShakeFrequency;
            cinemachineNoise.enabled = true;
        }
    }
}
