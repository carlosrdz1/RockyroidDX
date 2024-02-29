using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineHelper : MonoBehaviour
{    
    public static CinemachineHelper instance;
    float _shakeIntensity = 3f;
    float _shakeTime = 0.4f;
    float _shakeTimer;    

    void Awake(){
        if(instance == null){
            instance = this;
        }

        StopShake();
    }

    public void ShakeCamera(){
        CinemachineVirtualCamera _activeCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin _cbmcp = _activeCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = _shakeIntensity;
        _shakeTimer = _shakeTime;
    }

    void StopShake(){
        CinemachineVirtualCamera _activeCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin _cbmcp = _activeCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();        
        _cbmcp.m_AmplitudeGain = 0f;
        _shakeTimer = 0;
    }

    public void ShakeCameraCustom(float amplitude, float timer){
        CinemachineVirtualCamera _activeCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin _cbmcp = _activeCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();        
        _cbmcp.m_AmplitudeGain = amplitude;
        _shakeTimer = timer;
    }

    // Start is called before the first frame update
    void Start()
    {
        StopShake();
    }

    // Update is called once per frame
    void Update()
    {
        if(_shakeTimer > 0){
            _shakeTimer -= Time.deltaTime;
            if(_shakeTimer <= 0){
                _shakeTimer = 0;
                StopShake();
            }
        }
    }
}
