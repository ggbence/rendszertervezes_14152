using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uno
{
    class DrawDeck : Deck
    {
        public DrawDeck()
        {
            //adding cards
            cardsList=new List<Card>();
            //adding red cards
            for (int i = 0; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "red"));
            }

            //adding yellow cards
            for (int i = 0; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "yellow"));
  
            }

            //adding green cards
            for (int i = 0; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "green"));
            }

            //adding blue cards
            for (int i = 0; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "blue"));
            }


            //adding red cards
            for (int i = 1; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "red"));
            }

            //adding yellow cards
            for (int i = 1; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "yellow"));

            }

            //adding green cards
            for (int i = 1; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "green"));
            }

            //adding blue cards
            for (int i = 1; i <= 12; i++)
            {
                cardsList.Add(new Card(i, "blue"));
            }

            //adding black cards
            for (int i = 0; i < 4; i++)
            {
                cardsList.Add(new Card(0, "black"));
                cardsList.Add(new Card(1, "black"));
            }
        }
        
        
        public void Shuffle()
        {
            List<Card> newCardsList=new List<Card>();
            Random r = new Random();
            while (cardsList.Count > 0)
            {
                int randomIndex = r.Next(0, cardsList.Count);
                newCardsList.Add(cardsList[randomIndex]);
                cardsList.RemoveAt(randomIndex);
            }

            cardsList = newCardsList;
        }

        public List<Card> getRandomCards(int numberOfCards)
        {
            var ret = new List<Card>();
            for (int i = 0; i < numberOfCards; i++)
            {
                ret.Add(cardsList[0]);
                Shuffle();
            }
            return ret;
        }
    }
}
