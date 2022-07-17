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

    void Start()
    {
        Step = (Step * Screen.width) / 3840;
    }

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
    public void GetPosition(int id)
    {
        NowId = id;
        TargetPosition = positions[id];
    }

    public void MakeStep()
    {
        for (int i = 0; i < speed; i++) {
            if (Mathf.Abs(NowPosition - TargetPosition) >= Step) {
                if (NowPosition < TargetPosition)
                    transform.position = new Vector2(transform.position.x, transform.position.y + Step);
                else
                    transform.position = new Vector2(transform.position.x, transform.position.y - Step);
                NowPosition = transform.position.y;
            }
        }
    }

    public void CheckButtons()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && Buttons.Count - 1 > 0)
            GetPosition(Mathf.Max(NowId - 1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow) && Buttons.Count - 1 > NowId)
            GetPosition(Mathf.Min(NowId + 1, positions.Count - 1));
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            Buttons[NowId].onClick.Invoke();
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
        StopCoroutine(FixedUpdatePause());
        StopCoroutine(UpdatePause());
    }

    IEnumerator FixedUpdatePause()
    {
        while (true)
        {
            MakeStep();
            yield return new WaitForSecondsRealtime(0.02f);
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
