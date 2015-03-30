using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace uno
{
    public class Match
    {
        public int actualPlayer { get; set; }
        public ArrayList playersArrayList { get; set; }
        public bool reverse { get; set; }

        private DrawDeck drawDeck;
        private ThrowDeck throwDeck;


        public Match()
        {
            playersArrayList=new ArrayList();
        }

        public void RunMatch()
        {
            var mThread = new Thread(start);
            mThread.Start();
        }

        public void AddPlayer(User player)
        {
            playersArrayList.Add(player);
        }



        private void start()
        {
            drawDeck=new DrawDeck();
            throwDeck=new ThrowDeck();
            drawDeck.Shuffle();
            //add 7 cards each player
            Console.WriteLine("Match started with "+playersArrayList.Count+" players...");
            foreach (User user in playersArrayList)
            {
                List<Card> cards = drawDeck.getRandomCards(7);
                user.handCards.cardsList = cards;
                Console.WriteLine("Adding "+user.handCards.NumberOfCards()+" cards to player " + user.connectionId);
                ConnectionManager.Instance.SendMessage(new Message<HandCards> {Code = "updateCards", Objects = user.handCards},user.connectionId);
            }


            throwDeck.cardsList.Add(drawDeck.getRandomCards(1)[0]);



            int i = 0;
            while (true) //change me
            {
                User user = (User)playersArrayList[i];
                HandleClient hc = (HandleClient)ConnectionManager.Instance.clientsList[user.connectionId];
                hc.waitForData = true;
                ConnectionManager.Instance.SendMessage(new Message<Card> {Code="showThrowDeck", Objects = throwDeck.getTopCard()},user.connectionId);
                while (hc.waitForData) Thread.Sleep(500);
                var m = hc.LastMessage;
                Card card = ((JObject) m.Objects).ToObject<Card>();
                throwDeck.cardsList.Add(card);
                
                





                if (reverse) i--;
                else i++;
                if (i == -1) i = playersArrayList.Count-1;
                if (i == playersArrayList.Count) i = 0;
            }


        }
        
    }
}
