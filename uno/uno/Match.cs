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
        public List<User> PlayersList { get; set; }
        public bool reverse { get; set; }

        private DrawDeck drawDeck;
        private ThrowDeck throwDeck;


        public Match()
        {
            PlayersList=new List<User>();
            actualPlayer = 0;
        }

        public void RunMatch()
        {
            var mThread = new Thread(start);
            mThread.Start();
        }

        public void AddPlayer(User player)
        {
            PlayersList.Add(player);
        }



        private void start()
        {
            drawDeck=new DrawDeck();
            throwDeck=new ThrowDeck();
            drawDeck.Shuffle();
            //add 7 cards each player
            Console.WriteLine("Match started with "+PlayersList.Count+" players...");
            foreach (User user in PlayersList)
            {
                List<Card> cards = drawDeck.getRandomCards(7);
                user.handCards.cardsList = cards;
                Console.WriteLine("Adding "+user.handCards.NumberOfCards()+" cards to player " + user.connectionId);
                ConnectionManager.Instance.SendMessage(new Message<HandCards> {Code = "updateCards", Objects = user.handCards},user.connectionId);
                HandleClient hc = (HandleClient)ConnectionManager.Instance.clientsList[user.connectionId];
                hc.NotifyMatch = this;
                hc.Notify = true;
            }
            throwDeck.cardsList.Add(drawDeck.getRandomCards(1)[0]);
            actualPlayer = 0;
            round();


        }

        private void round()
        {
            User user = PlayersList[actualPlayer];
            ConnectionManager.Instance.SendMessage(
                new Message<Card> { Code = "showThrowDeck", Objects = throwDeck.getTopCard() },
                user.connectionId);

            //number of cards in hand
            var count = new List<KeyValuePair<string, int>>();
            foreach (var all in PlayersList)
            {
                count.Add(new KeyValuePair<string, int>(all.connectionId.ToString(),all.handCards.NumberOfCards()));
            }

            foreach (var other in PlayersList.Where(other => other.connectionId != user.connectionId))
            {
                ConnectionManager.Instance.SendMessage(
                    new Message<List<KeyValuePair<string, int>>> { Code = "showNumerOfCards", Objects = count},
                    other.connectionId
                    );
            }
        }




        private void nextPlayer()
        {
            if (reverse) actualPlayer--;
            else actualPlayer++;
            if (actualPlayer == -1) actualPlayer = PlayersList.Count - 1;
            if (actualPlayer == PlayersList.Count) actualPlayer = 0;
        }

        public void Notify(Message<object> message, int connectionId)
        {
            User user = PlayersList[actualPlayer];
            if (user.connectionId != connectionId)
            {
                ConnectionManager.Instance.SendMessage(
                    new Message<object> { Code = "notNow", Objects = new object()},
                    connectionId);
            }
            else
            {
                switch (message.Code)
                {
                    case "throwCard":
                        Card card = ((JObject)message.Objects).ToObject<Card>();
                        if (isValid(card))
                        {
                            throwDeck.cardsList.Add(card);
                            user.handCards.cardsList.Remove(card);
                            ConnectionManager.Instance.SendMessage(
                                new Message<HandCards> { Code = "updateCards", Objects = user.handCards },
                                user.connectionId
                                );
                            nextPlayer();
                            round();
                        }
                        else
                        {
                            ConnectionManager.Instance.SendMessage(
                                 new Message<object> { Code = "notValid", Objects = new object() },
                                 connectionId);
                            round();
                        }
                        break;
                }
            }

        }


        private bool isValid(Card card)
        {
            return ((throwDeck.getTopCard().Color == card.Color) || (throwDeck.getTopCard().Number == card.Number));

        }
    }
}
