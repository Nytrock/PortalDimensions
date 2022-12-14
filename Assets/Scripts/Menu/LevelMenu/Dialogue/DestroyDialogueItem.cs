using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDialogueItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player _)) {
            Destroy(GetComponentInChildren<ItemStartDialogue>().gameObject);
            Destroy(this);
        }
    }
}
