public enum GravityType
{
    /// <summary>
    /// gravite de type rien en dessous=> tombe
    /// </summary>
    GRAVITE_SIMPLE,
    /// <summary>
    /// Gravite de type reste accroché tant qu'un de ses voisin l'est
    /// </summary>
    GRAVITE_CONNEXE,
    /// <summary>
    /// Flotte dans les airs, non soumis à la gravité
    /// </summary>
    GRAVITE_NULLE

}