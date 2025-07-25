public class EnemyData
{
    public EnemyID Enemy_ID { get; set; }
    public EnemyRank Enemy_Rank { get; set; }
    public bool Is_Ranged { get; set; }
    public string Enemy_Unit_Name { get; set; }
    public EnemyMovementPattern EnemyMovementPattern { get; set; }
    public EnemyAttackPattern Atk_Pattern { get; set; }
    public int HP { get; set; }
    public int Atk_Power { get; set; }
    public float Move_Speed { get; set; }
    public int Chasing_Increase { get; set; }
    public int First_Appear_Stage { get; set; }
}

public enum EnemyID
{
    // Straight (ID: 11001, 21001, 31001)
    Straight_Normal_001 = 11001,
    Straight_Elite_001  = 21001,
    Straight_Boss_001   = 31001,

    // Zigzag (ID: 11002, 21002, 31002)
    Zigzag_Normal_002   = 11002,
    Zigzag_Elite_002    = 21002,
    Zigzag_Boss_002     = 31002,

    // Suicide (ID: 11003)
    Suicide_Normal_003  = 11003,

    // Shield (ID: 11004, 21004, 31004)
    Shield_Normal_004   = 11004,
    Shield_Elite_004    = 21004,
    Shield_Boss_004     = 31004,

    // Runner (ID: 11005, 21005, 31005)
    Runner_Normal_005   = 11005,
    Runner_Elite_005    = 21005,
    Runner_Boss_005     = 31005,

    // Sniper (ID: 12006, 22006, 32006)
    Sniper_Normal_006   = 12006,
    Sniper_Elite_006    = 22006,
    Sniper_Boss_006     = 32006,

    // Spread (ID: 12007, 22007, 32007)
    Spread_Normal_007   = 12007,
    Spread_Elite_007    = 22007,
    Spread_Boss_007     = 32007,

    // Radial (ID: 22008, 32008)
    Radial_Elite_008    = 22008,
    Radial_Boss_008     = 32008,

    // Flying (ID: 12009)
    Flying_Normal_009   = 12009,
}

public enum EnemyRank
{
    Normal,
    Elite,
    Boss
}

public enum EnemyMovementPattern
{
    Straight,
    Zigzag,
    Chase,
    Flying
}

public enum EnemyAttackPattern
{
    Normal,
    Suicide,
    Shield,
    Sniper,
    Spread,
    Radial,
    Flying
}