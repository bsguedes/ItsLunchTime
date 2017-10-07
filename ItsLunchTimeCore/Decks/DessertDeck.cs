using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore.Decks
{
    internal class DessertDeck : Deck<DessertCard>
    {
        private int DessertCount { get; }

        public DessertDeck(int count)
        {
            this.DessertCount = count;
        }

        internal override IEnumerable<DessertCard> GetCards()
        {
            foreach (DessertType type in Enum.GetValues(typeof(DessertType)))
            {
                for (int i = 0; i < DessertCount; i++)
                {
                    yield return new DessertCard(type);
                }
            }
        }
    }

    public class DessertCard : Card
    {
        public DessertType Type { get; }
        
        internal DessertCard( DessertType type)
        {
            this.Type = type;
        }
    }    
    
}