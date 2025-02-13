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
        attitudeCityState = GetEnumValueBasedOnAlignment<AttitudeCityState>(raceAligmentOnArrival);
    }

    private T GetEnumValueBasedOnAlignment<T>(int alignment) where T : System.Enum
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        int range = Mathf.Clamp((int)((alignment / 100f) * values.Length), 1, values.Length);
        int randomIndex = Random.Range(0, range);
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
