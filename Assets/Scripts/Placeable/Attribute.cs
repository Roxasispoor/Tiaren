public class Attribute
{
    private float baseValue;
    private float flatModif = 0;
    private float percentModif = 0;

    public float Value
    {
        get
        {
            return BaseValue + flatModif * percentModif;
        }
    }

    public float BaseValue
    {
        get
        {
            return baseValue;
        }

        set
        {
            baseValue = value;
        }
    }

    public float FlatModif
    {
        get
        {
            return flatModif;
        }

        set
        {
            flatModif = value;
        }
    }

    public float PercentModif
    {
        get
        {
            return percentModif;
        }

        set
        {
            percentModif = value;
        }
    }
}
