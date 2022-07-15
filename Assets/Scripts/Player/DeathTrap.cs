using UnityEngine;
using TMPro;

public class DeathTrap : MonoBehaviour
{
    public TextMeshProUGUI causeOfDeath;
    [SerializeField]
    private string keyNameDeath;
    public void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player player)) {
            causeOfDeath.text = causeOfDeath.text + " " + LocalizationManager.GetTranslate(keyNameDeath);
            player.Death();
        }
    }
}
