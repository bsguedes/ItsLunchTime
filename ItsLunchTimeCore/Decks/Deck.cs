﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore.Decks
{
    public abstract class Deck<T> where T : Card
    {
        public Deck()
        {
            this.Cards = new List<T>();
            this.Cards.AddRange(GetCards());
            this.Shuffle();
        }

        protected List<T> Cards { get; }

        public abstract IEnumerable<T> GetCards();

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

    public abstract class Card
    {
    }
}