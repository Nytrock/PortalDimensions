using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueProfile")]
public class ProfileDialogue : ScriptableObject, DialogueInterface
{
    public int id => _id;
    public new string name => _name;
    public Sprite CalmImage => _calm;
    public Sprite AngryImage => _angry;
    public Sprite AfraidImage => _afraid;
    public Sprite HappyImage => _happy;
    public Sprite ConfusedImage => _confused;
    public Sprite TenseImage => _tense;

    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _calm;
    [SerializeField] private Sprite _angry;
    [SerializeField] private Sprite _afraid;
    [SerializeField] private Sprite _happy;
    [SerializeField] private Sprite _confused;
    [SerializeField] private Sprite _tense;

}
