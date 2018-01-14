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
            this.RevealCards(this.BuffetSize);
        }

        private void RevealCards(int amount)
        {
            for (int i = 0; i < this.BuffetSize; i++)
            {
                this._buffet.Add(DessertDeck.Draw());
            }
        }

        internal IEnumerable<DessertCard> TakeChoices(int size)
        {
            return this.Buffet.Take(size);
        }

        internal List<DessertCard> RemoveDessertAtIndexes(List<int> indexes)
        {
            List<DessertCard> cards = new List<DessertCard>();
            foreach (int index in indexes)
            {
                cards.Add(this._buffet.ElementAt(index));
            }
            int d = 0;
            foreach (int index in indexes)
            {
                d++;
                for (int i = index + 1; i < BuffetSize; i++)
                {
                    this._buffet[i - d] = this._buffet[i];
                }
            }
            for (; d > 0; d--)
            {
                if (DessertDeck.RemainingCards > 0)
                {
                    this._buffet[this.BuffetSize - d] = DessertDeck.Draw();
                }
                else
                {
                    this._buffet[this.BuffetSize - d] = null;
                }
            }
            return cards;
        }
    }
}
