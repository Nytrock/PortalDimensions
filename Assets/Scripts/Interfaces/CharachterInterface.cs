using UnityEngine;

public interface CharacterInterface
{
    public enum RarityLevel
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Unique
    }
    int id { get; }
    GameObject prefab { get; }
    int price { get; }
    RarityLevel rarity { get; }
    bool available { get; set; }
}