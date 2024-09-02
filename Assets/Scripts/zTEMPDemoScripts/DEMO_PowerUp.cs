using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DEMO_PowerUp : MonoBehaviour
{
    public enum PowerUpType { puDoublejump, puWalljump}

    [Header("What does this power up do?")]
    public PowerUpType powerUp;

    [Header("Pop Up text that appears when this is picked up.")]
    public GameObject popUp;
    //Activates the powerup's effect
    private void OnTriggerEnter2D(Collider2D player)
    {
        //checks for Movement script and Player tag
        if (player.gameObject.GetComponent<Movement>() != null && player.gameObject.CompareTag("Player"))
        {
            Movement mov = player.gameObject.GetComponent<Movement>();
            
            switch (powerUp)
            {
                //increases the amount of times the player can jump by 1
                case PowerUpType.puDoublejump:
                    mov.bonusJumps += 1;
                    break;
                //turns on wall jumping, gives warnings if the player's movement isn't set up
                case PowerUpType.puWalljump:
                    if (mov.wallJump == false)
                    {
                        mov.wallJump = true;
                    } else if (mov.wallJump == true)
                    {
                        Debug.LogWarning("Player can already wall jump.");
                    }
                    if (mov.wallJumpTime == new Vector2(0, 0))
                    {
                        Debug.LogWarning("No values were set for wallJumpTime. Setting fallback values 1, 0.1");
                        mov.wallJumpTime = new Vector2(1, .01f);
                    }
                    break;
            }
            //Creates pop up text if it exists
            if (popUp != null)
            {
                Vector3 popUpPos = player.gameObject.transform.position + new Vector3(0, 2, 0);
                GameObject popUpGO = Instantiate(popUp);
                popUpGO.transform.position = popUpPos;
                if (popUpGO.GetComponent<DEMO_PopUpText>() != null)
                {
                    DEMO_PopUpText textScript = popUpGO.GetComponent<DEMO_PopUpText>();
                    textScript.TextChange(this.gameObject.name);
                }
                else
                {
                    Destroy(popUpGO);
                }
            }
            //Makes this item disappear after its work is done. 
            Destroy(this.gameObject);
        }
    }
}
