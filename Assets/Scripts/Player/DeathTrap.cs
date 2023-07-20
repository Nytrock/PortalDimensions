using TMPro;
using UnityEngine;

public class DeathTrap : MonoBehaviour
{
    public TextMeshProUGUI causeOfDeath;
    [SerializeField] private string keyNameDeath;
    
    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out PlayerStateManager player)) {
            causeOfDeath.text = causeOfDeath.text + " " + LocalizationManager.GetTranslate(keyNameDeath);
            player.SwitchState(player.deathState);
        }
    }
}
