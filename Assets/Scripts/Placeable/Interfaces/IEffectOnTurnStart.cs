/// <summary>
/// Interface to implement on class with effects on turn start (totem and such)
/// </summary>
public interface IEffectOnTurnStart
{
    /// <summary>
    /// Function to apply effect on target
    /// </summary>
    /// <param name="target"></param>
    void ApplyEffect(Placeable target);
}