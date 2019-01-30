public enum CrushType
{
    /// <summary>
    /// destroys bloc that falls above when crushed
    /// </summary>
    CRUSHDESTROYBLOC,
    /// <summary>
    /// is destroyed when crushed
    /// </summary>
    CRUSHDEATH,
    /// <summary>
    /// floats to the edge when crushed
    /// </summary>
    CRUSHLIFT,
    /// <summary>
    /// remains under bloc when crushed
    /// </summary>
    CRUSHSTAY,
    /// <summary>
    /// Take damage when crushed
    /// </summary>
    CRUSHDAMAGE
}