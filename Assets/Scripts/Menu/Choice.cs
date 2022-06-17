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
    public List<int> positions;
    public List<Button> Buttons;

    public void Update()
    {
        if (Mathf.Abs(NowPosition - TargetPosition) > Step)
        {
            if (NowPosition < TargetPosition)
                transform.position = new Vector2(transform.position.x, transform.position.y + Step);
            else
                transform.position = new Vector2(transform.position.x, transform.position.y - Step);
            NowPosition = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
            GetPosition(Mathf.Max(NowId - 1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow))
            GetPosition(Mathf.Min(NowId + 1, positions.Count - 1));
        if (Input.GetKeyDown(KeyCode.Space))
            Buttons[NowId].onClick.Invoke();
    }
    public void GetPosition(int id)
    {
        NowId = id;
        TargetPosition = positions[id];
    }
}
