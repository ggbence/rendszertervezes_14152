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
            actualPlayer = 0;
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
            actualPlayer = 0;
            round();


        }

        private void round()
        {
            User user = (User)playersArrayList[actualPlayer];
            HandleClient hc = (HandleClient)ConnectionManager.Instance.clientsList[user.connectionId];
            hc.waitForData = true;
            hc.NotifyMatch = this;
            ConnectionManager.Instance.SendMessage(new Message<Card> { Code = "showThrowDeck", Objects = throwDeck.getTopCard() }, user.connectionId);
            
            
        }

        public void roundContinue()
        {
            User user = (User)playersArrayList[actualPlayer];
            HandleClient hc = (HandleClient)ConnectionManager.Instance.clientsList[user.connectionId];
            var m = hc.LastMessage;
            Card card = ((JObject)m.Objects).ToObject<Card>();
            throwDeck.cardsList.Add(card);

            if (reverse) actualPlayer--;
            else actualPlayer++;
            if (actualPlayer == -1) actualPlayer = playersArrayList.Count - 1;
            if (actualPlayer == playersArrayList.Count) actualPlayer = 0;
            round();
        }
        
    }
}
