using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore
{
    public abstract class Deck<T> where T : Card
    {
        public Deck()
        {
            this.Cards = new List<T>();
        }

        protected List<T> Cards { get; private set; }

        public void Shuffle()
        {
            this.Cards.Shuffle();
        }

        public T Draw()
        {
            T c = this.Cards[0];
            this.Cards.RemoveAt(0);
            return c;
        }
    }
}
