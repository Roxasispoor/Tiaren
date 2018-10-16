public enum GravityType
{
    /// <summary>
    /// simple gravity : nothing below => fall
    /// </summary>
    SIMPLE_GRAVITY,
    /// <summary>
    /// related gravity : remains clipped while one of the neighbour is
    /// </summary>
    RELATED_GRAVITY,
    /// <summary>
    /// null gravity : floating in the air, not impacted by any gravity
    /// </summary>
    NULL_GRAVITY

}