using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{    
	public Animator[] Hearts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(PlayerController.instance.Life){
            case 0:
                Hearts[3].Play("Empty");
                Hearts[2].Play("Empty");
                Hearts[1].Play("Empty");
                Hearts[0].Play("Empty");
            break;

            case 1:
                Hearts[3].Play("Empty");
                Hearts[2].Play("Empty");
                Hearts[1].Play("Empty");
                Hearts[0].Play("Full");
            break;

            case 2:
                Hearts[3].Play("Empty");
                Hearts[2].Play("Empty");
                Hearts[1].Play("Full");
                Hearts[0].Play("Full");
            break;

            case 3:
                Hearts[3].Play("Empty");
                Hearts[2].Play("Full");
                Hearts[1].Play("Full");
                Hearts[0].Play("Full");
            break;

            case 4:
                Hearts[3].Play("Full");
                Hearts[2].Play("Full");
                Hearts[1].Play("Full");
                Hearts[0].Play("Full");
            break;
        }
    }
}
