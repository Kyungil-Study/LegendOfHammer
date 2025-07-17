public static class EnumMapper
{
    public static EnemyRank ToEnemyRank(this EnemySpawnRankType rankType)
    {
        return rankType switch
        {
            EnemySpawnRankType.Normal => EnemyRank.Normal,
            EnemySpawnRankType.Elite => EnemyRank.Elite,
            EnemySpawnRankType.Boss => EnemyRank.Boss,
            _ => throw new System.ArgumentOutOfRangeException(nameof(rankType), rankType, null)
        };
    }
}