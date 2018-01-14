using ItsLunchTimeCore.Decks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class DessertBuffet
    {
        private List<DessertCard> _buffet;
        public ReadOnlyCollection<DessertCard> Buffet { get; }

        internal DessertDeck DessertDeck { get; }
        public int BuffetSize { get; }

        internal DessertBuffet(int buffer_size, DessertDeck deck)
        {
            this.BuffetSize = buffer_size;
            _buffet = new List<DessertCard>(buffer_size);
            this.Buffet = new ReadOnlyCollection<DessertCard>(_buffet);
            this.DessertDeck = deck;
        }

        internal void RevealCards(int amount)
        {
            this.BuffetSize.Times(() => this._buffet.Add(DessertDeck.Draw()));
        }

        internal IEnumerable<DessertCard> TakeChoices(int size)
        {
            return this.Buffet.Take(size);
        }

        internal DessertCard RemoveDessertAtIndex(int index)
        {
            DessertCard chosen = this._buffet.ElementAt(index);
            for (int i = index + 1; i < BuffetSize; i++)
            {
                this._buffet[i - 1] = this._buffet[i];
            }
            this._buffet[this.BuffetSize - 1] = DessertDeck.Draw();
            return chosen;
        }
    }
}
