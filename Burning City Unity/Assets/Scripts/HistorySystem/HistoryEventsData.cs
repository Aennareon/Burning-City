using UnityEngine;

public class HistoryEventsData : ScriptableObject
{
    //GENERAL HISTORY DATA -----------------------------------

    public enum CityRaces
    {
        Humans, Elfs, Dwarfs, Orcs, Goblins, Undead, Beasts, Demons, Dragons, Elementals, Giants, Insects, Plants, Spirits, Constructs, Other,
        Merfolk, Centaurs, Minotaurs, Fairies, Vampires, Werewolves, Trolls, Gnomes, Halflings, Nagas, Satyrs, Cyclops, Phoenixes, Griffins, Chimeras
    }

    public enum CityZones
    {
        innercity, outtercity, countryside, refugeecamp, marketplace, residential, industrial, agricultural, military, religious, educational,
        recreational, commercial, governmental, slums, harbor
    }

    public enum WorldGods
    {
        life, light, wisdom, love, justice, order, reason, fertility, harvest, nature, technology, arcana, fire, water, earth, wind, time, space,
        trickery, chaos, war, vengeance, blood, darkness, death
    }

    //RACE ARRIVAL DATA -----------------------------------

    public enum ArrivalState
    {
        welcomed, celebrated, honored, integrated, tolerated, assimilated, liberated, revered, feared, mistrusted, exploited, enslaved, persecuted,
        segregated, rejected, ignored, exiled
    }

    public enum ArrivalReason
    {
        trade, exploration, adventure, pilgrimage, diplomacy, prophecy, research, curiosity, alliance, colonization, migration, survival, mission,
        refuge, escape, search, accident, disaster, war, conquest, exile, unknown
    }

    public enum TreatmentReason
    {
        respect, admiration, friendship, cooperation, kinship, alliance, necessity, curiosity, tradition, superstition, politics, economics, religion,
        suspicion, rivalry, envy, greed, fear, hatred, betrayal, unknown
    }

    public enum AttitudeCityState
    {
        helping, protecting, rebuilding, educating, healing, entertaining, governing, negotiating, trading, innovating, exploring, expanding,
        defending, controlling, spying, sabotaging, plundering, conquering
    }
}
