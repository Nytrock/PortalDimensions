using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueProfile")]
public class ProfileDialogue : ScriptableObject, DialogueInterface
{
    public int id => _id;
    public string name => _name;
    public Sprite CalmImage => _calm;
    public Sprite AngryImage => _angry;

    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _calm;
    [SerializeField] private Sprite _angry;

}
