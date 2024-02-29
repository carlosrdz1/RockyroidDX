using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EffectsManager : MonoBehaviour
{
    public Material PaletteMaterial; 
    public static EffectsManager instance;
    const float PALETTE_MAX_BRIGHT = 5.5f;
    const float PALETTE_NORMAL = 0.95f;
    const float PALETTE_FULL_DARK = 0.20f;
    const float VELOCITY_OF_PALETTE = 0.2f;
    float _fakeAlpha;
    bool _triggerGoFromBrightToNormal = false;
    bool _triggerGoFromNormalToBright = false;
    public bool TriggerGoFromBrightToNormal { get => _triggerGoFromBrightToNormal; set => _triggerGoFromBrightToNormal = value; }
    public bool TriggerGoFromNormalToBright { get => _triggerGoFromNormalToBright; set => _triggerGoFromNormalToBright = value; }

    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TriggerGoFromBrightToNormal){
            GoFromBrightToNormal();
        }

        if(TriggerGoFromNormalToBright){
            GoFromNormalToBright();
        }
    }

    public void SetMaxBright(){
        PaletteMaterial.SetFloat("Vector1_738CE5A5", PALETTE_MAX_BRIGHT);
    }

    void GoFromBrightToNormal(){
        _fakeAlpha = PaletteMaterial.GetFloat("Vector1_738CE5A5");

        if(_fakeAlpha > PALETTE_NORMAL){
            _fakeAlpha -= VELOCITY_OF_PALETTE;
            PaletteMaterial.SetFloat("Vector1_738CE5A5", _fakeAlpha);
        }

        if(_fakeAlpha <= PALETTE_NORMAL){
            _fakeAlpha = PALETTE_NORMAL;
            PaletteMaterial.SetFloat("Vector1_738CE5A5", _fakeAlpha);
            TriggerGoFromBrightToNormal = false;
        }
    }

    void GoFromNormalToBright(){
        _fakeAlpha = PaletteMaterial.GetFloat("Vector1_738CE5A5");

        if(_fakeAlpha < PALETTE_MAX_BRIGHT){
            _fakeAlpha += VELOCITY_OF_PALETTE;
            PaletteMaterial.SetFloat("Vector1_738CE5A5", _fakeAlpha);
        }

        if(_fakeAlpha >= PALETTE_MAX_BRIGHT){
            _fakeAlpha = PALETTE_MAX_BRIGHT;
            PaletteMaterial.SetFloat("Vector1_738CE5A5", _fakeAlpha);
            TriggerGoFromNormalToBright = false;
        }
    }
}
