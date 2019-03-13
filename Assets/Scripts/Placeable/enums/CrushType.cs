public enum CrushType
{
    /// <summary>
    /// NOT IMPLEMENTED - destroys bloc that falls above when crushed
    /// </summary>
    CRUSHDESTROYBLOC,
    /// <summary>
    /// NOT IMPLEMENTED - is destroyed when crushed
    /// </summary>
    CRUSHDEATH,
    /// <summary>
    /// NOT IMPLEMENTED - floats to the edge when crushed
    /// </summary>
    CRUSHLIFT,
    /// <summary>
    /// remains under bloc when crushed
    /// </summary>
    CRUSHSTAY,
    /// <summary>
    /// Take damage when crushed and destroy the block falling
    /// </summary>
    CRUSHDAMAGE
}