using UnityEngine;

public interface WorldInterface
{
    int id { get; }
    string header { get; }
    Sprite image { get; }
    int countLevels { get; }
    int completedLevels { get; set; }
}

