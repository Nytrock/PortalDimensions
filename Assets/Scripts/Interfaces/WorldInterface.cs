using UnityEngine;

public interface WorldInterface
{
    int id { get; }
    string name { get; }
    Sprite image { get; }
    int countLevels { get; }
    int completedLevels { get; set; }
}

