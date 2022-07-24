using UnityEngine;

public class DialogueChoiceManager : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;

    public void DoSomethingFromId(int id)
    {
        switch (id) {
            case 1: object1.SetActive(true); break;
            case 2: object2.SetActive(true); break;
        }
    }
}
