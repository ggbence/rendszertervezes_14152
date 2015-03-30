using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uno
{
    public abstract class Deck
    {
        public List<Card> cardsList { get; set; }

       

        

        public void AddCards(List<Card> c)
        {
            foreach (var card in c)
            {
                cardsList.Add(card);
            }
        }

        public int NumberOfCards()
        {
            return cardsList.Count;
        }

        public Card TakeCard(int number)
        {
            Card ret = cardsList[number];
            cardsList.RemoveAt(number);
            return ret;
        }
    }
}
