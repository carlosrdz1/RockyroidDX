using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectsHelper
{
    public static IEnumerator FlashSprite(SpriteRenderer sprite, int numTimes, float delay, bool disable = false)  {
        // number of times to loop
        for (int loop = 0; loop < numTimes; loop++) {   
            PlayerController.instance.IsInvincible = true;
            if (disable) {
                // for disabling
                sprite.enabled = false;
            } else {
                // for changing the alpha
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay); 

            if (disable) {
                // for disabling
                sprite.enabled = true;
            } else {
                // for changing the alpha
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);
        }
        
        PlayerController.instance.IsInvincible = false;
    } 

    public static IEnumerator FlashEnemy(SpriteRenderer sprite, int numTimes, float delay, bool disable = false)  {
        // number of times to loop
        for (int loop = 0; loop < numTimes; loop++) {   
           // PlayerController.instance.IsInvincible = true;
            if (disable) {
                // for disabling
                sprite.enabled = false;
            } else {
                // for changing the alpha
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay); 

            if (disable) {
                // for disabling
                sprite.enabled = true;
            } else {
                // for changing the alpha
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);
        }
        
      //  PlayerController.instance.IsInvincible = false;
    } 
}
