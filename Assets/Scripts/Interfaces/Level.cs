using UnityEngine;

[CreateAssetMenu(menuName = "Interfaces/Level")]
public class Level : ScriptableObject, LevelInterface
{
    public int id => _id;
    public World world => _world;
    public float cameraZoom => _cameraZoom;
    public int coinsNumber => _coinsNumber;
    public int bestScore
    {
        get => _bestScore;
        set => _bestScore = value;
    }
    public int bestShootCount => _bestShootCount;
    public int bestTeleportCount => _bestTeleportCount;
    public int bestTime => _bestTime;
    public bool wasCompleted
    {
        get => _wasCompleted;
        set => _wasCompleted = value;
    }
    public bool hasGun => _hasGun;
    public int maxScore => _maxScore;
    public int scoreForOneStar => _scoreForOneStar;
    public int scoreForTwoStar => _scoreForTwoStar;
    public int scoreForThreeStar => _scoreForThreeStar;

    [SerializeField] private int _id;
    [SerializeField] private World _world;
    [SerializeField] private float _cameraZoom;
    [SerializeField] private int _coinsNumber;
    [SerializeField] private int _bestScore;
    [SerializeField] private int _bestShootCount;
    [SerializeField] private int _bestTeleportCount;
    [SerializeField] private int _bestTime;
    [SerializeField] private bool _wasCompleted;
    [SerializeField] private bool _hasGun;
    [SerializeField] private int _maxScore;
    [SerializeField] private int _scoreForOneStar;
    [SerializeField] private int _scoreForTwoStar;
    [SerializeField] private int _scoreForThreeStar;

}