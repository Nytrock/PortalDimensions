using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStartDialogue : MonoBehaviour
{
    public string KeyToDialogue;
    public bool InstantStart;
    public DialogueManager dialogueManager;
    public void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.TryGetComponent(out Player _))
        {
            if (InstantStart) {
                dialogueManager.SetKey(KeyToDialogue);
                dialogueManager.StartDialogue();
            }
            else
                Debug.Log("Не мгновенный старт диалога");
        }
    }
}
