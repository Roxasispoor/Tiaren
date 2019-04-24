public enum SkillTypes
{
    PUSH, // 0
    CREATE,
    DESTROY,
    SWORDATTACK,
    BLEEDING,
    LEGSWIPE, // 5
    SPINNINGATTACK,
    TACKLE,
    BOWSHOT,
    PIERCINGSHOT,
    HIGHGROUND, // 10
    ZIPLINE, 
    ACROBATIC,
    MAGICMISSILE,
    FIREBALL,
    WALL, //15
    FISSURE,
    EARTHBENDING,
    REPULSIVEGRENADE,
    GRAPPLE,
    MAKIBISHI, //20
    CREATETOTEMHP,
    CREATETOTEMAP,
    TOTEMLINK,
    TOTEMDESTROY,
    DOWNZIP, //25
    PICKOBJECT 
}


/// <summary>
/// To know what you are supposed to be targetting
/// </summary>
public enum TargetType
{
    /// <summary>
    /// skill targeting a living (character)
    /// </summary>
    HURTABLE,
    /// <summary>
    /// skill targeting a block
    /// </summary>
    BLOCK,
    /// <summary>
    /// skill targeting a blockside (1 click on the block, then 1 click on the right side)
    /// </summary>
    BLOCKSIDE,
    /// <summary>
    /// skill can be cast on ether living or ether blocks
    /// </summary>
    PLACEABLE,
    /// <summary>
    /// skill dealing effect in an area
    /// </summary>
    AREA,
    /// <summary>
    /// skill used by a character on a preloaded target
    /// </summary>
    ALREADYTARGETED

}