using UnityEngine;
using UnityEngine.UI;

public class ShaderUpdate : MonoBehaviour
{
    public RawImage image;

    void Update()
    {
        image.material.SetFloat("_UnscaledTime", Time.unscaledDeltaTime);
    }
}
