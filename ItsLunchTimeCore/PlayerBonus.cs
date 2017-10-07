using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore.Decks
{
    internal class PlayerBonusDeck : Deck<PlayerBonusCard>
    {
        internal override IEnumerable<PlayerBonusCard> GetCards()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class PlayerBonusCard : Card
    {
        public abstract int Points { get; }
    }

    public class LastInATrackAlone : PlayerBonusCard
    {
        public override int Points => 3;


    }
}
