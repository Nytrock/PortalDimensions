using UnityEngine;

public interface WorldInterface
{
    int id { get; }
    string header { get; }
    Sprite icon { get; }
    Sprite menuImage { get; }
    int countLevels { get; }
    int completedLevels { get; set; }
    Level[] levels { get; }
}

