using UnityEngine;
using UnityEngine.UI;

public class ConfirmButtons : MonoBehaviour
{
    public ButtonFunctional canvas;
    public GameObject messageSettings;
    public GameObject messageExit;

    public Button Yes;
    public Button No;

    public void ExitToMenuConfirm()
    {
        if (Save.GetConfirmNeed()) {
            Yes.onClick.RemoveAllListeners();
            Yes.onClick.AddListener(canvas.ExitToMenu);

            No.onClick.RemoveAllListeners();
            No.onClick.AddListener(canvas.SetConfirm);

            messageSettings.SetActive(false);
            messageExit.SetActive(true);

            canvas.SetConfirm();
        } else {
            canvas.ExitToMenu();
        }
    }

    public void ExitConfirm()
    {
        if (Save.GetConfirmNeed()) {
            Yes.onClick.RemoveAllListeners();
            Yes.onClick.AddListener(canvas.Exit);

            No.onClick.RemoveAllListeners();
            No.onClick.AddListener(canvas.SetConfirm);

            messageSettings.SetActive(false);
            messageExit.SetActive(true);

            canvas.SetConfirm();
        } else {
            canvas.Exit();
        }
    }
}
