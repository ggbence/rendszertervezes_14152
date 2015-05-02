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
        private string selectedColor { get; set; }
        private int shouldSay { get; set; } 

        private DrawDeck drawDeck;
        private ThrowDeck throwDeck;


        public Match()
        {
            PlayersList=new List<User>();
            actualPlayer = 0;
            reverse = false;
            shouldSay = -1;
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

            throwDeck.cardsList.Add(drawDeck.getRandomCards(1)[0]);
            throwDeck.cardsList.Add(drawDeck.cardsList[0]);
            drawDeck.cardsList.RemoveAt(0);


            //add 7 cards each player
            Console.WriteLine("Match started with "+PlayersList.Count+" players...");
            foreach (User user in PlayersList)
            {
                List<Card> cards = drawDeck.getRandomCards(2);
                user.handCards.cardsList = cards;
                Console.WriteLine("Adding "+user.handCards.NumberOfCards()+" cards to player " + user.connectionId);
                ConnectionManager.Instance.SendMessage(new Message<HandCards> {Code = "updateCards", Objects = user.handCards},user.connectionId);
                HandleClient hc = (HandleClient)ConnectionManager.Instance.clientsList[user.connectionId];
                hc.NotifyMatch = this;
                hc.Notify = true;
            }
            
            actualPlayer = 0;

            //if the throwdeck is black
            if (throwDeck.getTopCard().Color == "black")
            {
            
                //plus4 - put back
                while (throwDeck.getTopCard().Number == 1)
                {
                    drawDeck.cardsList.Add(throwDeck.getTopCard());
                    throwDeck.cardsList.RemoveAt(0);
                    throwDeck.cardsList.Add(drawDeck.getRandomCards(1)[0]);
                }


                //select
                if (throwDeck.getTopCard().Number == 0)
                {
                    ConnectionManager.Instance.SendMessage(
                        new Message<object> {Code = "selectColor", Objects = new object()}, PlayersList[0].connectionId);
                    return;
                }
            }

            round();


        }

        private void round()
        {
            User user = PlayersList[actualPlayer];



            //number of cards in hand
            var count = new List<KeyValuePair<string, int>>();
            foreach (var all in PlayersList)
            {
                count.Add(new KeyValuePair<string, int>(all.connectionId.ToString(), all.handCards.NumberOfCards()));
            }

            foreach (var other in PlayersList.Where(other => other.connectionId!=user.connectionId))
            {
                ConnectionManager.Instance.SendMessage(
                    new Message<List<KeyValuePair<string, int>>> { Code = "showNumerOfCards", Objects = count },
                    other.connectionId
                    );
            }






            if (throwDeck.getTopCard().Color == "black")
            {

                ConnectionManager.Instance.SendMessage(
                    new Message<string> {Code = "showThrowDeck", Objects = "black - selected: "+selectedColor},
                    user.connectionId);
            }
            else ConnectionManager.Instance.SendMessage(
                  new Message<string> { Code = "showThrowDeck", Objects = throwDeck.getTopCard().ToString() },
                  user.connectionId);




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
            //said uno
            if (message.Code == "uno" && shouldSay == connectionId) { shouldSay = -1; return; }
            
            
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
                            CheckUno();
                            handleDrop(card);
                            
                        }
                        else
                        {
                            ConnectionManager.Instance.SendMessage(
                                 new Message<object> { Code = "notValid", Objects = new object() },
                                 connectionId);
                            round();
                        }
                        break;
                    case "drawCard":
                        try
                        {
                            user.handCards.AddCards(drawDeck.getRandomCards(1));
                        }
                        catch (NoMoreCardsException)
                        {
                            ThrowDeckToDrawDeck();
                            user.handCards.AddCards(drawDeck.getRandomCards(1));
                        }
                        CheckUno();
                       
                        ConnectionManager.Instance.SendMessage(
                             new Message<HandCards> { Code = "updateCards", Objects = user.handCards },
                             user.connectionId
                              );
                        nextPlayer();
                        round();
                        break;
                    case "selectedColor":
                        //player selected a color
                        if (throwDeck.getTopCard().Color == "black")
                        {
                            //can choose
                            selectedColor = message.Objects.ToString();

                            nextPlayer();
                            if (throwDeck.getTopCard().Number==1) nextPlayer();
                            round();


                        }


                        break;



                }
            }

        }

        private void CheckUno()
        {
            if (shouldSay != -1)
            {
                //penalty
                try
                {
                    PlayersList[shouldSay].handCards.AddCards(drawDeck.getRandomCards(4));
                }
                catch (NoMoreCardsException)
                {
                    ThrowDeckToDrawDeck();
                    PlayersList[shouldSay].handCards.AddCards(drawDeck.getRandomCards(4));
                }

               
                ConnectionManager.Instance.SendMessage(
                    new Message<HandCards> { Code = "updateCards", Objects = PlayersList[shouldSay].handCards }, shouldSay);
                ConnectionManager.Instance.SendMessage(
                   new Message<object> { Code = "unoPenalty", Objects = new object() }, shouldSay);
                shouldSay = -1;
            }

        }

        private void handleDrop(Card card)
        {
            User user = PlayersList[actualPlayer];
            throwDeck.cardsList.Add(card);
            user.handCards.cardsList.Remove(card);

            
           
            
            
            ConnectionManager.Instance.SendMessage(
                new Message<HandCards> { Code = "updateCards", Objects = user.handCards },
                user.connectionId
                );


            //won
            if (user.handCards.cardsList.Count == 0)
            {
                ConnectionManager.Instance.SendMessage(
                    new Message<object> { Code = "won", Objects = new object()}, user.connectionId);

                foreach (var other in PlayersList.Where(other => other.connectionId != user.connectionId))
                {
                    ConnectionManager.Instance.SendMessage(
                        new Message<string> {Code = "lost", Objects = user.connectionId.ToString()},other.connectionId);
                }
                return;
            }


            if (card.Number <= 9 && card.Color != "black")
            {
                //not special card
                nextPlayer();
                round();
            }

            if (card.Number >= 10 && card.Color != "black")
            {
                //special color card
                if (card.Number == 10)
                {
                    //skip
                    nextPlayer();
                    ConnectionManager.Instance.SendMessage(
                        new Message<object> {Code = "gotSkipped", Objects = new object()},PlayersList[actualPlayer].connectionId
                        );
                    nextPlayer();
                    round();
                }

                if (card.Number == 11)
                {
                    //reverse
                    foreach (var all in PlayersList)
                    {
                        ConnectionManager.Instance.SendMessage(new Message<object> { Code = "reversed", Objects = new object()},all.connectionId);
                    }

                    reverse = !reverse;
                    nextPlayer();
                    round();
                }

                if (card.Number == 12)
                {
                    //plus2
                    nextPlayer();
                    user = PlayersList[actualPlayer];
                    try
                    {
                        user.handCards.AddCards(drawDeck.getRandomCards(2));
                    }
                    catch (NoMoreCardsException)
                    {
                        ThrowDeckToDrawDeck();
                        user.handCards.AddCards(drawDeck.getRandomCards(2));
                    }
                    
                    ConnectionManager.Instance.SendMessage(new Message<HandCards> { Code = "updateCards", Objects = user.handCards }, user.connectionId);
                    ConnectionManager.Instance.SendMessage(new Message<object> { Code = "gotPlus2", Objects = new object()},user.connectionId);
                    nextPlayer();
                    round();
                }
            }


            //black
            if (card.Color=="black" && card.Number == 0)
            {
                //select color
                ConnectionManager.Instance.SendMessage(new Message<object> { Code = "selectColor", Objects = new object()},user.connectionId);

            }

            if (card.Color == "black" && card.Number == 1)
            {
                //plus4 and select color
                
                ConnectionManager.Instance.SendMessage(new Message<object> { Code = "selectColor", Objects = new object() }, user.connectionId);
                nextPlayer();
                user = PlayersList[actualPlayer];
                try
                {
                    user.handCards.AddCards(drawDeck.getRandomCards(4));
                }
                catch (NoMoreCardsException)
                {
                    ThrowDeckToDrawDeck();
                    user.handCards.AddCards(drawDeck.getRandomCards(4));
                }
                
                ConnectionManager.Instance.SendMessage(new Message<HandCards> { Code = "updateCards", Objects = user.handCards }, user.connectionId);
                ConnectionManager.Instance.SendMessage(new Message<object> { Code = "gotPlus4", Objects = new object() }, user.connectionId);
                reverse = !reverse;
                nextPlayer();
                reverse = !reverse;
            }

            if (user.handCards.cardsList.Count == 1) shouldSay = user.connectionId;

        }


        private bool isValid(Card card)
        {
            if (card.Number <= 9 && card.Color != "black")
            {
                //not special - throwDeck black
                if (throwDeck.getTopCard().Color == "black" && selectedColor != card.Color) return false;
                if (throwDeck.getTopCard().Color == "black" && selectedColor == card.Color) return true;
                
                //not special card
                return ((throwDeck.getTopCard().Color == card.Color) || (throwDeck.getTopCard().Number == card.Number));
            }

            if (card.Number >= 10 && card.Color != "black")
            {
                //special color card - throwDeck black
                if (throwDeck.getTopCard().Color == "black" && selectedColor != card.Color) return false;
                if (throwDeck.getTopCard().Color == "black" && selectedColor == card.Color) return true;
                
                
                //special color card
                return ((throwDeck.getTopCard().Color == card.Color) || (throwDeck.getTopCard().Number==card.Number));
            }

            //black cards
            return true;

        }

        private void ThrowDeckToDrawDeck()
        {
            var cards = throwDeck.GetOtherCards();
            drawDeck.AddCards(cards);
            drawDeck.Shuffle();
        }
    }
}
