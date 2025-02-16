using UnityEngine;
using System.Collections.Generic;

public class HistoryEventsData : ScriptableObject
{
    //--------------------------------------------------------------------------------
    public enum CityRaces
    {
        Human, Elf, Dwarf, Orc, Goblin, Troll, Ogre, Halfling, Gnome, Dragonborn, Tiefling, Aasimar,
    }
    public enum WorldGods
    {
        Light, Dark, Nature, Death, War, Knowledge, Trickery, Life, Tempest, Arcana,
        Forge, Grave, Order, Chaos, Love, Hate, Time, Space, Fire, Water, Earth, Air,
        Good, Evil, Law, Balance
    }
    public enum OrdersOfChivalry
    {
        arcana, key, skull, word
    }
    public enum CityGuilds
    {
        Alchemist, Blacksmith, Carpenter, Cartographer, Cook, Engineer, Farmer, Fisherman,
    }
    public enum CityBuildings
    {
        Barracks, Castle, Cathedral, CityHall, Dock, Farm, Forge, GuildHall, Inn, Library, Lighthouse,
        Market, Mill, Mine, Monument, Observatory, Palace, Park, Port, Prison, Quarry, School, Shipyard,
        Shrine, Stable, Tavern, Temple, Tower, University, Wall, Warehouse, Watchtower, Watermill, Windmill, Workshop,
    }
    public enum CityZones
    {
        Residential, Commercial, Industrial, Agricultural, Military, Religious,
        Cultural, Administrative, Port, Slums, Sewers, Catacombs, Cemetary, Ruins, Wilderness,
    }
    public enum ArcaneSchools
    {
        Abjuration, Conjuration, Divination, Enchantment, Evocation, Illusion, Necromancy, Transmutation,
    }
    public enum WildMonsters
    {
        Trolls, Giants, Dragons, Wyverns, Griffons, Manticores, Chimeras, Hydras, Basilisks, Cockatrices,
    }

    //--------------------------------------------------------------------------------

    public enum EventType
    {
        arrival, conflict, discovery, migration, technology, disaster, foundation, creation, destruction,
    }

    //--------------------------------------------------------------------------------

    // Estructura para almacenar título y descripción juntos
    public struct EventInfo
    {
        public string title;
        public string description;

        public EventInfo(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }

    // Diccionario para almacenar listas de EventInfo
    public Dictionary<EventType, List<EventInfo>> eventInfos = new Dictionary<EventType, List<EventInfo>>()
    {
        { EventType.arrival, new List<EventInfo> {
            new EventInfo("Arrival of the Hero", "A hero has arrived in the city, bringing hope and courage to the inhabitants."),
            new EventInfo("Arrival of the Colonists", "A group of colonists has arrived in new lands, ready to settle and prosper."),
            new EventInfo("Arrival of the Sage", "A sage from distant lands has arrived, sharing his knowledge and wisdom."),
            new EventInfo("Arrival of the Merchant", "A merchant has arrived at the market, bringing exotic goods from distant lands."),
            new EventInfo("Arrival of the Refugees", "A group of refugees has arrived, fleeing from war and seeking safety."),
            new EventInfo("Arrival of the Diplomat", "A diplomat has arrived to negotiate peace between warring factions."),
            new EventInfo("Arrival of the Bandits", "A group of bandits has arrived, causing trouble and unrest."),
            new EventInfo("Arrival of the Nomads", "A tribe of nomads has arrived, bringing their unique culture and traditions."),
            new EventInfo("Arrival of the Healer", "A renowned healer has arrived, offering aid to the sick and injured."),
            new EventInfo("Arrival of the Miners", "A group of miners has arrived, seeking to exploit the rich mineral resources.")
        }},
        { EventType.conflict, new List<EventInfo> {
            new EventInfo("Battle in the Valley", "A great battle took place in the valley, where the armies of humans and orcs clashed."),
            new EventInfo("Slave Rebellion", "The slaves rebelled against their masters, fighting for their freedom."),
            new EventInfo("Clan War", "The clans of dwarves and elves clashed in a war for control of the mines."),
            new EventInfo("Siege of the Castle", "The castle was besieged by an enemy army, testing the resilience of its defenders."),
            new EventInfo("Civil War", "A civil war has broken out, dividing the kingdom and causing widespread chaos."),
            new EventInfo("Invasion of the Goblins", "A horde of goblins has invaded, wreaking havoc and destruction."),
            new EventInfo("Skirmish at the Border", "A skirmish occurred at the border, with both sides suffering heavy losses."),
            new EventInfo("Revolt of the Peasants", "The peasants have revolted against the oppressive rule of the nobility."),
            new EventInfo("War of the Wizards", "A war between powerful wizards has erupted, causing magical devastation."),
            new EventInfo("Conflict of the Gods", "The gods themselves are in conflict, causing turmoil in the mortal realm.")
        }},
        { EventType.discovery, new List<EventInfo> {
            new EventInfo("Discovery of the Island", "An unknown island has been discovered, full of mysteries and riches."),
            new EventInfo("Artifact Found", "An ancient artifact has been found, which could change the course of history."),
            new EventInfo("Discovery of the Lost Library", "An ancient library has been discovered, revealing forgotten knowledge."),
            new EventInfo("Exploration of the Ruins", "The ruins of an ancient civilization have been explored, uncovering hidden secrets."),
            new EventInfo("Discovery of the Hidden Cave", "A hidden cave has been discovered, containing valuable treasures."),
            new EventInfo("Discovery of the Ancient Temple", "An ancient temple has been discovered, revealing long-lost religious practices."),
            new EventInfo("Discovery of the Underground City", "An underground city has been discovered, revealing a forgotten civilization."),
            new EventInfo("Discovery of the Magical Forest", "A magical forest has been discovered, full of enchanted creatures and plants."),
            new EventInfo("Discovery of the Crystal Mine", "A mine full of precious crystals has been discovered, promising great wealth."),
            new EventInfo("Discovery of the Lost Tribe", "A lost tribe has been discovered, revealing unique customs and traditions.")
        }},
        { EventType.migration, new List<EventInfo> {
            new EventInfo("Exodus of the Elves", "The elves have abandoned their ancestral lands, seeking a new home."),
            new EventInfo("Migration of the Orcs", "The orcs are migrating south, in search of new lands to conquer."),
            new EventInfo("Displacement of the Gnomes", "The gnomes have left their mountain homes, seeking refuge in human cities."),
            new EventInfo("Exodus of the Halflings", "The halflings have left their peaceful villages, fleeing from an unknown threat."),
            new EventInfo("Migration of the Trolls", "The trolls are migrating, causing fear and unrest among the local population."),
            new EventInfo("Relocation of the Dwarves", "The dwarves are relocating to a new mountain range, seeking richer mineral deposits."),
            new EventInfo("Nomadic Journey of the Ogres", "The ogres are on a nomadic journey, moving from place to place."),
            new EventInfo("Migration of the Dragonborn", "The dragonborn are migrating, seeking to establish a new kingdom."),
            new EventInfo("Flight of the Goblins", "The goblins are fleeing from a powerful enemy, seeking safety in numbers."),
            new EventInfo("Migration of the Tieflings", "The tieflings are migrating, seeking acceptance and a place to call home.")
        }},
        { EventType.technology, new List<EventInfo> {
            new EventInfo("Invention of the Wheel", "The wheel has been invented, revolutionizing transportation and construction."),
            new EventInfo("Discovery of Gunpowder", "Gunpowder has been discovered, forever changing the nature of warfare."),
            new EventInfo("Creation of the Telescope", "An engineer has created the first telescope, allowing for detailed observation of the stars."),
            new EventInfo("Development of the Printing Press", "The printing press has been developed, enabling the mass dissemination of knowledge."),
            new EventInfo("Invention of the Steam Engine", "The steam engine has been invented, ushering in a new era of industrialization."),
            new EventInfo("Discovery of Electricity", "Electricity has been discovered, leading to numerous technological advancements."),
            new EventInfo("Creation of the Mechanical Clock", "A mechanical clock has been created, allowing for precise timekeeping."),
            new EventInfo("Development of the Compass", "The compass has been developed, greatly improving navigation."),
            new EventInfo("Invention of the Flying Machine", "A flying machine has been invented, opening up the skies to exploration."),
            new EventInfo("Discovery of the Printing Press", "The printing press has been discovered, revolutionizing the spread of information.")
        }},
        { EventType.disaster, new List<EventInfo> {
            new EventInfo("Volcanic Eruption", "A volcano has erupted, devastating nearby villages."),
            new EventInfo("River Flood", "The river has overflowed, causing a flood that destroys crops and homes."),
            new EventInfo("Earthquake in the City", "An earthquake has shaken the city, causing destruction and chaos."),
            new EventInfo("Devastating Plague", "A plague has swept through the region, decimating the population and leaving desolation in its wake."),
            new EventInfo("Hurricane Strikes", "A hurricane has struck, causing widespread damage and loss of life."),
            new EventInfo("Forest Fire", "A forest fire has broken out, consuming vast tracts of land."),
            new EventInfo("Drought", "A severe drought has struck, causing crop failures and famine."),
            new EventInfo("Tsunami", "A tsunami has hit the coast, causing massive destruction and loss of life."),
            new EventInfo("Avalanche", "An avalanche has occurred, burying villages and cutting off vital supply routes."),
            new EventInfo("Sandstorm", "A sandstorm has swept through the desert, causing chaos and destruction.")
        }},
        { EventType.foundation, new List<EventInfo> {
            new EventInfo("Foundation of the City", "A new city has been founded, symbolizing hope and prosperity."),
            new EventInfo("Creation of the Empire", "A new empire has been created, uniting diverse races under one banner."),
            new EventInfo("Establishment of the Guild", "A new guild has been established, promoting trade and craftsmanship."),
            new EventInfo("Construction of the Castle", "A majestic castle has been built, symbolizing power and protection."),
            new EventInfo("Foundation of the University", "A university has been founded, dedicated to the pursuit of knowledge."),
            new EventInfo("Creation of the Order of Chivalry", "An order of chivalry has been created, upholding the values of honor and bravery."),
            new EventInfo("Establishment of the Temple", "A temple has been established, serving as a center of worship and community."),
            new EventInfo("Foundation of the Market", "A market has been founded, becoming a bustling center of commerce."),
            new EventInfo("Creation of the Library", "A library has been created, preserving the knowledge of the ages."),
            new EventInfo("Establishment of the Barracks", "A barracks has been established, training soldiers to defend the realm.")
        }},
        { EventType.creation, new List<EventInfo> {
            new EventInfo("Creation of the World", "The world has been created by the gods, full of wonders and mysteries."),
            new EventInfo("Birth of the Gods", "The gods have been born, bringing order and chaos to the universe."),
            new EventInfo("Formation of the Mountains", "The mountains have formed, rising majestically towards the sky."),
            new EventInfo("Birth of the First Dragon", "The first dragon has been born, a creature of immense power and wisdom."),
            new EventInfo("Creation of the Stars", "The stars have been created, illuminating the night sky."),
            new EventInfo("Formation of the Oceans", "The oceans have formed, teeming with life and mystery."),
            new EventInfo("Birth of Magic", "Magic has been born, changing the fabric of reality."),
            new EventInfo("Creation of the Forests", "The forests have been created, full of life and beauty."),
            new EventInfo("Formation of the Rivers", "The rivers have formed, bringing life and sustenance to the land."),
            new EventInfo("Birth of the First Civilization", "The first civilization has been born, marking the dawn of a new era.")
        }},
        { EventType.destruction, new List<EventInfo> {
            new EventInfo("Destruction of the City", "A city has been destroyed, reduced to rubble and ashes."),
            new EventInfo("Fall of the Empire", "A great empire has fallen, plunged into chaos and anarchy."),
            new EventInfo("Ruin of the Temple", "An ancient temple has been reduced to ruins, its past glory now forgotten."),
            new EventInfo("Collapse of the Mine", "A mine has collapsed, trapping the miners inside."),
            new EventInfo("Destruction of the Forest", "A forest has been destroyed, its trees reduced to ashes."),
            new EventInfo("Fall of the Castle", "A castle has fallen, its walls breached and its defenders slain."),
            new EventInfo("Destruction of the Library", "A library has been destroyed, its knowledge lost forever."),
            new EventInfo("Collapse of the Bridge", "A bridge has collapsed, cutting off vital trade routes."),
            new EventInfo("Destruction of the Port", "A port has been destroyed, its ships sunk and its docks in ruins."),
            new EventInfo("Fall of the Tower", "A tower has fallen, its stones scattered and its purpose lost.")
        }}
    };

    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
}
