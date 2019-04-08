/// <summary>
/// Interface to implement on class capable of receiving damage.
/// </summary>
public interface IHurtable
{
    /// <summary>
    /// Function to call when receiving damage.
    /// </summary>
    /// <param name="damage"></param>
    void ReceiveDamage(float damage);
}
