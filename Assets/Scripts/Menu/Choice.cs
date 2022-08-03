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
        if (!CanvasManager.isGamePaused) {
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
        if (Input.GetKeyDown(KeyCode.UpArrow) && Buttons.Count - 1 > 0)
            SetPosition(Mathf.Max(NowId - 1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow) && Buttons.Count - 1 > NowId)
            SetPosition(Mathf.Min(NowId + 1, positions.Count - 1));
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) && working)
            Buttons[NowId].onClick.Invoke();

        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log(transform.localPosition.y);
    }

    public void StartPauseWorking()
    {
        foreach (Button button in Buttons)
            button.GetComponent<ChoiceButton>().pauseActive = true;
        StartCoroutine(FixedUpdatePause());
        StartCoroutine(UpdatePause());
    }

    public void StopPauseWorking()
    {
        foreach (Button button in Buttons)
            button.GetComponent<ChoiceButton>().pauseActive = false;
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
            CheckButtons();
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
    }
}
