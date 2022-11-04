using UnityEngine;

public class ExitManager : MonoBehaviour
{
    public Animator canvas;
    private bool playerDown;
    private bool playerAlmostInside;
    private bool playerInside;
    private Player charachter;

    public void CheckColliders(string side, Player player, bool activating)
    {
        if (side == "Horizontal") {
            if (!playerAlmostInside && activating || playerInside && !activating) {
                playerAlmostInside = true;
                playerInside = false;
            } else if (playerAlmostInside && !activating) {
                playerAlmostInside = false;
                playerInside = false;
            } else if (playerAlmostInside && activating) {
                playerAlmostInside = false;
                playerInside = true;
            }
        } else {
            playerDown = activating;
        }

        if (!playerDown && playerInside)
            player.StopWorking();

        if (playerDown && playerInside) {
            player.StopWorking();
            GetComponent<Animator>().SetBool("Exit", true);
        }

        charachter = player;
    }

    public void SetCanvasAnimation()
    {
        canvas.SetBool("isComplete", true);
        LevelManager.levelManager.ChangeButtonsWorking(false);
        LevelManager.levelManager.FixTime();
    }

    public void TurnOffPlayer()
    {
        charachter.gameObject.SetActive(false);
    }
}
