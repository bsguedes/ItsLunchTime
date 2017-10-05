namespace ItsLunchTimeCore.Decks
{
    internal class PreferencesDeck : Deck<PreferenceCard>
    {
        public PreferencesDeck()
        {
            this.Cards.Add(new PreferenceCard(Restaurant.Russo, 5, Restaurant.JoeAndLeos, 3, Restaurant.GustoDiBacio, Punishment.TwoVP, Punishment.OneVP));
        }
    }

    public class PreferenceCard : Card
    {        
        public Restaurant FirstPreference { get; }
        public int FirstPreferenceBonus { get; }
        public Restaurant SecondPreference { get; }
        public int SecondPreferenceBonus { get; }
        public Restaurant Undesired { get; }
        public Punishment Punishment { get; }
        public Punishment SideBan { get; }

        public PreferenceCard(Restaurant p1, int v1, Restaurant p2, int v2, Restaurant u, Punishment up, Punishment side)
        {
            this.FirstPreference = p1;
            this.FirstPreferenceBonus = v1;
            this.SecondPreference = p2;
            this.SecondPreferenceBonus = v2;
            this.Undesired = u;
            this.Punishment = up;
            this.SideBan = side;
        }
    }

    public class Punishment
    {
        public static readonly Punishment OneVP = new Punishment(PunishmentType.VictoryPoints, 1);
        public static readonly Punishment TwoVP = new Punishment(PunishmentType.VictoryPoints, 2);
        public static readonly Punishment OneDollar = new Punishment(PunishmentType.Cash, 1);
        public static readonly Punishment TwoDollar = new Punishment(PunishmentType.Cash, 2);

        public PunishmentType Type { get; }
        public int Value { get; }

        public Punishment(PunishmentType t, int v)
        {
            this.Type = t;
            this.Value = v;
        }        
    }

    public enum PunishmentType
    {
        Cash,
        VictoryPoints
    }

}