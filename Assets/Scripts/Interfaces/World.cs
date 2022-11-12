using UnityEngine;

[CreateAssetMenu(menuName = "Interfaces/World")]
public class World : ScriptableObject, WorldInterface
{
    public int id => _id;
    public string header => _name;
    public Sprite image => _image;
    public int countLevels => _levelsCount;
    public int completedLevels
    {
        get => _levelsCompleted;
        set => _levelsCompleted = value;
    }
    public Level[] levels => _levels;

    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _image;
    [SerializeField] private int _levelsCount;
    [SerializeField] private int _levelsCompleted;
    [SerializeField] private Level[] _levels;

}

