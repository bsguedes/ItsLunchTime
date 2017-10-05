using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class DessertDeck : Deck<DessertCard>
    {        
        public DessertDeck(int count)
        {
            
        }

        public override IEnumerable<DessertCard> GetCards()
        {
            throw new System.NotImplementedException();
        }
    }

    public class DessertCard : Card
    {

    }
}