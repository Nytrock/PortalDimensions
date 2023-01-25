using UnityEngine;

[CreateAssetMenu(menuName = "Interfaces/World")]
public class World : ScriptableObject, WorldInterface
{
    public int id => _id;
    public string header => _name;
    public Sprite icon => _icon;
    public Sprite menuImage => _menuImage;
    public int countLevels => _levelsCount;
    public int completedLevels
    {
        get => _levelsCompleted;
        set => _levelsCompleted = value;
    }
    public Level[] levels => _levels;

    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private Sprite _menuImage;
    [SerializeField] private int _levelsCount;
    [SerializeField] private int _levelsCompleted;
    [SerializeField] private Level[] _levels;

}

