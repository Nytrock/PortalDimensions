using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choice : MonoBehaviour
{
    public float NowPosition;
    public float TargetPosition;
    public int NowId;
    public float Step;
    public List<float> positions;
    public List<Button> Buttons;

    void Start()
    {
        Step = (Step * Screen.width) / 3840;
    }

    void Update()
    {
        if (Mathf.Abs(NowPosition - TargetPosition) >= Step)
        {
            if (NowPosition < TargetPosition)
                transform.position = new Vector2(transform.position.x, transform.position.y + Step);
            else
                transform.position = new Vector2(transform.position.x, transform.position.y - Step);
            NowPosition = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && Buttons.Count - 1 > 0)
            GetPosition(Mathf.Max(NowId - 1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow) && Buttons.Count - 1 > NowId)
            GetPosition(Mathf.Min(NowId + 1, positions.Count - 1));
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Buttons[NowId].onClick.Invoke();
    }
    public void GetPosition(int id)
    {
        NowId = id;
        TargetPosition = positions[id];
    }
}
