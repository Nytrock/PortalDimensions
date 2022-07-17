using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    public bool isWorking;
    private TextMeshProUGUI text;
    private float timer;
    [SerializeField] private float _hudRefreshRate = 1f;

    private void Awake()
    {
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
                timer = Time.unscaledTime + _hudRefreshRate;
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
