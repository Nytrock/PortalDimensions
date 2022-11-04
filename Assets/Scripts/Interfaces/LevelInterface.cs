public interface LevelInterface
{
    int id { get; }
    World world { get; }
    float cameraZoom { get; }
    int coinsNumber { get;  }
    int bestScore { get; set; }
    int bestShootCount { get; }
    int bestTeleportCount { get; }
    bool wasCompleted { get; set; }
    bool hasGun { get; }
    int maxScore { get; }
    int scoreForOneStar { get; }
    int scoreForTwoStar { get; }
    int scoreForThreeStar { get; }
}
