public enum EcraseType
{ 
    /// <summary>
    /// Quand écrasé, détruit le bloc qui lui tombe dessus
    /// </summary>
    ECRASEDESTROYBLOC,
    /// <summary>
    /// Est détruit quand est écrasé
    /// </summary>
    ECRASEDEATH,
    /// <summary>
    /// Flotte jusqu'au sommet quand est écrasé
    /// </summary>
    ECRASELIFT,
    /// <summary>
    /// Reste sous le bloc quand est écrasé
    /// </summary>
    ECRASESTAY
}