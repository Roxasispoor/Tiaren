public enum SkillType
{
    /// <summary>
    /// skill used by a character on himself
    /// </summary>
    SELF,
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
    /// skill used by a character on a preloaded skill
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
    /// Skill trageting blocks in straight line
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
    SURROUNDINGBLOCKS


}
