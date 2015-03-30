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


    }
}
