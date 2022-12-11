using UnityEngine;

public class CoinsBounds : MonoBehaviour
{
    [SerializeField] private AudioSource coinsBound;
    public void OnParticleCollision(GameObject other)
    {
        coinsBound.pitch = 0.9f + Random.Range(-0.1f, 0.1f);
        coinsBound.Play();
    }
}
