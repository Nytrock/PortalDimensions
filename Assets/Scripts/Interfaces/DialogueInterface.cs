using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DialogueInterface
{
    int id { get; }
    string name { get; }
    Sprite CalmImage { get; }
    Sprite AngryImage { get; }
    Sprite AfraidImage { get; }
    Sprite HappyImage { get; }
    Sprite ConfusedImage { get; }
    Sprite TenseImage { get; }
}
