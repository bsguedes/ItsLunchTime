using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore
{
    public abstract class Deck
    {
        public Deck()
        {
            this.Cards = new List<Card>();
        }

        protected List<Card> Cards { get; private set; }

        public void Shuffle()
        {

        }
    }
}
