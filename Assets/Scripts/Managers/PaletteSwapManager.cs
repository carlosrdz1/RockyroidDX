using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PaletteSwapManager : MonoBehaviour
{
    public Material PaletteMaterial;
    public Texture[] Palettes;
    bool _swapPaletteButtonPressed;
    bool _allowSwapPalette;
    int  _activePaletteID;    

    // Start is called before the first frame update
    void Start()
    {
        _activePaletteID = 0;
        _allowSwapPalette = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_swapPaletteButtonPressed && _allowSwapPalette){
            _allowSwapPalette = false;
            if(_activePaletteID == Palettes.Length - 1)
                _activePaletteID = -1;

            _activePaletteID ++;

            PaletteMaterial.SetTexture("_GradientText", Palettes[_activePaletteID]);
        }
    }

    public void PaletteSwap(InputAction.CallbackContext context){
        if(context.started){
            _swapPaletteButtonPressed = true;
        }
        
        if(context.canceled){
            _swapPaletteButtonPressed = false;
            _allowSwapPalette = true;          
        }
    }
}
