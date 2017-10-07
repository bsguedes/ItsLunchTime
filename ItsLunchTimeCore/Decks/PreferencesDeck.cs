using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class PreferencesDeck : Deck<PreferenceCard>
    {
        internal override IEnumerable<PreferenceCard> GetCards()
        {
            yield return new PreferenceCard(Restaurant.Russo, 5, Restaurant.JoeAndLeos, 3, Restaurant.GustoDiBacio, Punishment.TwoVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.Russo, 5, Restaurant.Panorama, 1, Restaurant.Silva, Punishment.TwoVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.Russo, 4, Restaurant.Silva, 3, Restaurant.Palatus, Punishment.TwoVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.Russo, 4, Restaurant.GustoDiBacio, 2, Restaurant.Silva, Punishment.OneVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.Russo, 4, Restaurant.Palatus, 2, Restaurant.GustoDiBacio, Punishment.TwoVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.Palatus, 6, Restaurant.JoeAndLeos, 3, Restaurant.Panorama, Punishment.OneVP, Punishment.OneDollar);
            yield return new PreferenceCard(Restaurant.Palatus, 5, Restaurant.Panorama, 4, Restaurant.JoeAndLeos, Punishment.OneVP, Punishment.OneDollar);
            yield return new PreferenceCard(Restaurant.Palatus, 5, Restaurant.Silva, 1, Restaurant.Russo, Punishment.TwoVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.Palatus, 4, Restaurant.GustoDiBacio, 3, Restaurant.Silva, Punishment.OneVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.Palatus, 4, Restaurant.Russo, 2, Restaurant.Silva, Punishment.OneVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.GustoDiBacio, 6, Restaurant.JoeAndLeos, 1, Restaurant.Silva, Punishment.OneVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.GustoDiBacio, 6, Restaurant.Panorama, 4, Restaurant.JoeAndLeos, Punishment.OneVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.GustoDiBacio, 5, Restaurant.Silva, 3, Restaurant.Palatus, Punishment.OneVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.GustoDiBacio, 5, Restaurant.Palatus, 2, Restaurant.Russo, Punishment.OneVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.GustoDiBacio, 4, Restaurant.Russo, 2, Restaurant.Palatus, Punishment.OneDollar, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.Silva, 7, Restaurant.JoeAndLeos, 4, Restaurant.Palatus, Punishment.TwoVP, Punishment.OneDollar);
            yield return new PreferenceCard(Restaurant.Silva, 6, Restaurant.Panorama, 3, Restaurant.GustoDiBacio, Punishment.OneVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.Silva, 6, Restaurant.GustoDiBacio, 3, Restaurant.Panorama, Punishment.OneVP, Punishment.OneDollar);
            yield return new PreferenceCard(Restaurant.Silva, 5, Restaurant.Palatus, 1, Restaurant.JoeAndLeos, Punishment.OneVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.Silva, 5, Restaurant.Russo, 2, Restaurant.Panorama, Punishment.OneVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.Panorama, 7, Restaurant.JoeAndLeos, 3, Restaurant.Russo, Punishment.OneVP, Punishment.OneDollar);
            yield return new PreferenceCard(Restaurant.Panorama, 7, Restaurant.Silva, 3, Restaurant.Russo, Punishment.TwoVP, Punishment.OneDollar);
            yield return new PreferenceCard(Restaurant.Panorama, 6, Restaurant.GustoDiBacio, 4, Restaurant.JoeAndLeos, Punishment.OneVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.Panorama, 6, Restaurant.Palatus, 1, Restaurant.JoeAndLeos, Punishment.OneDollar, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.Panorama, 5, Restaurant.Russo, 2, Restaurant.JoeAndLeos, Punishment.OneVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.JoeAndLeos, 7, Restaurant.Panorama, 4, Restaurant.GustoDiBacio, Punishment.OneVP, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.JoeAndLeos, 7, Restaurant.Silva, 4, Restaurant.Panorama, Punishment.OneVP, Punishment.OneDollar);
            yield return new PreferenceCard(Restaurant.JoeAndLeos, 7, Restaurant.GustoDiBacio, 1, Restaurant.Russo, Punishment.OneVP, Punishment.TwoVP);
            yield return new PreferenceCard(Restaurant.JoeAndLeos, 6, Restaurant.Palatus, 3, Restaurant.GustoDiBacio, Punishment.OneDollar, Punishment.OneVP);
            yield return new PreferenceCard(Restaurant.JoeAndLeos, 6, Restaurant.Russo, 2, Restaurant.Palatus, Punishment.OneDollar, Punishment.OneVP);
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
        public static readonly Punishment OneVP = new Punishment(PunishmentType.VictoryPoints, -1);
        public static readonly Punishment TwoVP = new Punishment(PunishmentType.VictoryPoints, -2);
        public static readonly Punishment OneDollar = new Punishment(PunishmentType.Cash, -1);
        public static readonly Punishment TwoDollar = new Punishment(PunishmentType.Cash, -2);

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