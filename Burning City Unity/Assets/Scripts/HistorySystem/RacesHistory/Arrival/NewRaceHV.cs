using UnityEngine;

[CreateAssetMenu(fileName = "NewRaceHV", menuName = "History System/Race Arrival Tale")]
public class NewRaceHV : BaseHistoryEvent
{
    [Header("Newcomers info")]
    public CityRaces raceName;
    [Range(0, 100)]
    public int raceAligmentOnArrival;
    public string raceDescription;
    public WorldGods religionOnArrival;
    public ArrivalReason arrivalReason;
    public ArrivalState arrivalType;
    public TreatmentReason treatmentReason;
    public CityZones moveInLocation;
    public AttitudeCityState attitudeCityState;

    [Header("Race arrival tale")]
    [TextArea(20, 40)]
    public string raceArrivalText;

    private void OnValidate()
    {
        AssignValuesBasedOnAlignment();
        raceArrivalText = GenerateNarrative() + "\n\n" + GenerateRaceArrivalNarrative();
    }

    private void AssignValuesBasedOnAlignment()
    {
        religionOnArrival = GetEnumValueBasedOnAlignment<WorldGods>(raceAligmentOnArrival);
        arrivalReason = GetEnumValueBasedOnAlignment<ArrivalReason>(raceAligmentOnArrival);
        arrivalType = GetEnumValueBasedOnAlignment<ArrivalState>(raceAligmentOnArrival);
        treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(raceAligmentOnArrival);
        moveInLocation = GetEnumValueBasedOnAlignment<CityZones>(raceAligmentOnArrival);

        // Condicionales para evitar combinaciones que no tienen sentido
        if (arrivalType == ArrivalState.honored && treatmentReason == TreatmentReason.hatred)
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival + 20, 0, 100));
        }
        if (arrivalType == ArrivalState.welcomed && treatmentReason == TreatmentReason.fear)
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival + 20, 0, 100));
        }
        if (arrivalType == ArrivalState.rejected && treatmentReason == TreatmentReason.respect)
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival - 20, 0, 100));
        }
        if (arrivalType == ArrivalState.celebrated && (treatmentReason == TreatmentReason.fear || treatmentReason == TreatmentReason.hatred))
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival + 20, 0, 100));
        }
        if (arrivalType == ArrivalState.enslaved && (treatmentReason == TreatmentReason.friendship || treatmentReason == TreatmentReason.cooperation))
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival - 20, 0, 100));
        }
        if (arrivalType == ArrivalState.liberated && (treatmentReason == TreatmentReason.hatred || treatmentReason == TreatmentReason.fear))
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival + 20, 0, 100));
        }
        if (arrivalType == ArrivalState.persecuted && (treatmentReason == TreatmentReason.respect || treatmentReason == TreatmentReason.admiration))
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival - 20, 0, 100));
        }
        if (arrivalType == ArrivalState.segregated && (treatmentReason == TreatmentReason.friendship || treatmentReason == TreatmentReason.cooperation))
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival - 20, 0, 100));
        }
        if (arrivalType == ArrivalState.ignored && (treatmentReason == TreatmentReason.respect || treatmentReason == TreatmentReason.admiration))
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival - 20, 0, 100));
        }
        if (arrivalType == ArrivalState.exiled && (treatmentReason == TreatmentReason.respect || treatmentReason == TreatmentReason.admiration))
        {
            treatmentReason = GetEnumValueBasedOnAlignment<TreatmentReason>(Mathf.Clamp(raceAligmentOnArrival - 20, 0, 100));
        }

        // Modificadores al alineamiento para elegir attitudeCityState en función de arrivalType
        int attitudeAlignmentModifier = 0;
        switch (arrivalType)
        {
            case ArrivalState.welcomed:
            case ArrivalState.celebrated:
            case ArrivalState.honored:
            case ArrivalState.integrated:
                attitudeAlignmentModifier = 20;
                break;
            case ArrivalState.rejected:
            case ArrivalState.persecuted:
            case ArrivalState.enslaved:
            case ArrivalState.exiled:
                attitudeAlignmentModifier = -20;
                break;
            default:
                attitudeAlignmentModifier = 0;
                break;
        }

        attitudeCityState = GetEnumValueBasedOnAlignment<AttitudeCityState>(Mathf.Clamp(raceAligmentOnArrival + attitudeAlignmentModifier, 0, 100));
    }

    private T GetEnumValueBasedOnAlignment<T>(int alignment) where T : System.Enum
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        int rangeStart, rangeEnd;

        if (alignment <= 30)
        {
            rangeStart = 0;
            rangeEnd = values.Length / 2;
        }
        else if (alignment >= 70)
        {
            rangeStart = values.Length / 2;
            rangeEnd = values.Length;
        }
        else
        {
            rangeStart = 0;
            rangeEnd = values.Length;
        }

        int randomIndex = Random.Range(rangeStart, rangeEnd);
        return (T)values.GetValue(randomIndex);
    }

    private string GenerateRaceArrivalNarrative()
    {
        string text = $"The {raceName} arrived in the year {yearOfEvent}. They arrived while they were on a {arrivalReason} journey. At the time of their arrival, they were {raceDescription}." +
            $" At that time they were believers of the god of {religionOnArrival}." +
            "\n\n" +
            $"They arrived at the city and were {arrivalType} due to {treatmentReason} of the city's inhabitants. They ended up settling in the {moveInLocation}, where they formed a community " +
            $"with the objective of {attitudeCityState} the city." + "\n\n";
        return text;
    }
}
