using UnityEngine;

[CreateAssetMenu(menuName = "Interfaces/Character")]
public class Character : ScriptableObject, CharacterInterface
{
    public int id => _id;
    public GameObject prefab => _prefab;
    public int price => _price;
    public CharacterInterface.RarityLevel rarity => _rarity;
    public bool available {
        get => _available;
        set => _available = value;
    }

    [SerializeField] private int _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _price;
    [SerializeField] private CharacterInterface.RarityLevel _rarity;
    [SerializeField] private bool _available;
}
