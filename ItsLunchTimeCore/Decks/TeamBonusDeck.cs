using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class TeamBonusDeck : Deck<TeamBonusCard>
    {
        internal override IEnumerable<TeamBonusCard> GetCards()
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class TeamBonusCard : Card
    {

    }
}