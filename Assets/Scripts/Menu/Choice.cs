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
    public int speed;
    public bool working;

    private void FixedUpdate()
    {
        MakeStep();
    }

    private void Update()
    {
        if (!ButtonFunctional.isGamePaused && working) {
            CheckButtons();
        }
    }
    public void SetPosition(int id)
    {
        NowId = id;
        TargetPosition = positions[id];
    }

    public void MakeStep()
    {
        for (int i = 0; i < speed; i++) {
            if (Mathf.Abs(NowPosition - TargetPosition) >= Step) {
                if (NowPosition < TargetPosition)
                    transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + Step);
                else
                    transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - Step);
                NowPosition = transform.localPosition.y;
            }
        }
    }

    public void CheckButtons()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && Buttons.Count - 1 > 0)
            SetPosition(Mathf.Max(NowId - 1, 0));
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && Buttons.Count - 1 > NowId)
            SetPosition(Mathf.Min(NowId + 1, positions.Count - 1));
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            Buttons[NowId].onClick.Invoke();
    }

    public void StartPauseWorking()
    {
        foreach (Button button in Buttons)
            button.GetComponent<ChoiceButton>().pauseActive = true;
        working = true;
        StartCoroutine(FixedUpdatePause());
        StartCoroutine(UpdatePause());
    }

    public void StopPauseWorking()
    {
        foreach (Button button in Buttons)
            button.GetComponent<ChoiceButton>().pauseActive = false;
        working = false;
        StopAllCoroutines();
    }

    IEnumerator FixedUpdatePause()
    {
        while (true)
        {
            MakeStep();
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }

    IEnumerator UpdatePause()
    {
        while (true)
        {
            if (working)
                CheckButtons();
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
    }
}
