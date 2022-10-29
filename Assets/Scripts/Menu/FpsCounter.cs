using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    public static FpsCounter fpsCounter;

    public bool isWorking;
    private TextMeshProUGUI text;
    private float timer;
    [SerializeField] private float hudRefreshRate = 1f;

    private void Awake()
    {
        fpsCounter = this;
        text = GetComponent<TextMeshProUGUI>();
        if (!isWorking)
            text.text = "";
    }

    private void Update()
    {
        if (isWorking) {
            if (Time.unscaledTime > timer) {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                text.text = fps + " fps";
                timer = Time.unscaledTime + hudRefreshRate;
            }
        }
    }

    public void ChangeWorking(bool newValue)
    {
        isWorking = newValue;
        if (!isWorking)
            text.text = "";
    }
}
