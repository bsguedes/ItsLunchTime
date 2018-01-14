using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal abstract class Deck<T> where T : Card
    {
        public int RemainingCards
        {
            get
            {
                return this.Cards.Count;
            }
        }

        internal Deck()
        {
            this.Cards = new List<T>();
            this.Cards.AddRange(GetCards());
            this.Shuffle();
        }

        protected List<T> Cards { get; }

        internal abstract IEnumerable<T> GetCards();

        internal void Shuffle()
        {
            this.Cards.Shuffle();
        }

        internal T Draw()
        {
            T c = this.Cards[0];
            this.Cards.RemoveAt(0);
            return c;
        }

        internal void Recreate()
        {
            this.Cards.Clear();
            this.Cards.AddRange(GetCards());
            this.Shuffle();
        }
    }

    public abstract class Card
    {
    }
}
