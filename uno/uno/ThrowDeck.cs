using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uno
{
    class ThrowDeck : Deck
    {
        public ThrowDeck()
        {
            cardsList=new List<Card>();
        }


        public Card getTopCard()
        {
            return cardsList.Last();
        }

        public List<Card> GetOtherCards()
        {
            var ret = new List<Card>();
            for (int i = 0; i < cardsList.Count - 1; i++)
            {
                ret.Add(cardsList[i]);
                cardsList.RemoveAt(i);
            }
            return ret;
        }


    }
}
