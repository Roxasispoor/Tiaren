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
    DOWNZIP, //20
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
    LIVING,
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

public enum SkillArea
{
    NONE,
    /// <summary>
    /// Skill targeting the top of a block
    /// </summary>
    TOPBLOCK,
    /// <summary>
    /// Skill trageting blocks in cross shape
    /// </summary>
    CROSS,
    /// <summary>
    /// Skill where the area is a line of blocks
    /// </summary>
    LINE,
    /// <summary>
    /// Skill that can target through blocks
    /// </summary>
    THROUGHBLOCKS,
    /// <summary>
    /// Skill that affect living in the surrounding area (from user position)
    /// </summary>
    SURROUNDINGLIVING,
    /// <summary>
    /// Skill that affect blocks in the surrounding area (from user position)
    /// </summary>
    SURROUNDINGBLOCKS,
    /// <summary>
    /// Where targets are player and blocks in the area
    /// </summary>
    MIXEDAREA,
    /// <summary>
    /// Target can only be the block under playingplaceable
    /// </summary>
    BLOCKUNDER

}

public enum SkillEffect
{
    NONE,
    /// <summary>
    /// Check when a block is moved if the path is clear
    /// </summary>
    MOVE,
    /// <summary>
    /// Check if the block can be destroyed or not
    /// </summary>
    DESTROY,
    /// <summary>
    /// Check if we can create any block on the targeted block
    /// </summary>
    CREATE,
    /// <summary>
    /// Check if the character can go up
    /// </summary>
    UP,
    /// <summary>
    /// Keep only adjacent blocks for the case the character is spinnig
    /// </summary>
    SPINNING,
    /// <summary>
    /// Keep the 8 adjacent targets only
    /// </summary>
    SWORDRANGE
}

public enum ConditionType
{
    NONE,
    UP
}
