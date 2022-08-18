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
            if (dissolveAmount == 1.1f) {
                GetComponent<Rigidbody2D>().drag = 1000;
                isDeath = false;
            }
        }
    }

    public void StartDeath(float dissolveSpeed, Color dissolveColor) {
        isDeath = true;
        material.SetColor("_DissolveColor", dissolveColor);
        this.dissolveSpeed = dissolveSpeed;
    }
}
