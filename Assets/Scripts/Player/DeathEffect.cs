using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour {

    [SerializeField] private Material material;

    private float dissolveAmount;
    private float dissolveSpeed = 1f;
    private bool isDeath;

    private void Start() {
        material.SetFloat("_DissolveAmount", 0);
    }

    private void Update() {
        if (isDeath) {
            dissolveAmount = Mathf.Clamp(dissolveAmount + dissolveSpeed * Time.deltaTime, 0, 1.1f);
            material.SetFloat("_DissolveAmount", dissolveAmount);
        }

        if (Input.GetKeyDown(KeyCode.T))
            isDeath = true;
    }

    public void StartDeath(float dissolveSpeed, Color dissolveColor) {
        isDeath = true;
        material.SetColor("_DissolveColor", dissolveColor);
        this.dissolveSpeed = dissolveSpeed;
    }
}
