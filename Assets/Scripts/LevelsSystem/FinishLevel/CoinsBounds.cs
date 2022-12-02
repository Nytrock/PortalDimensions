using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsBounds : MonoBehaviour
{
    public void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }
}
