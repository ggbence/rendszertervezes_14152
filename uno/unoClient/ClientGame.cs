using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uno;

namespace unoClient
{
    public class ClientGame
    {

        private static volatile ClientGame instance;
        private static object syncRoot = new Object();
        public User GamerUser;



        public static ClientGame Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ClientGame();
                    }
                }

                return instance;
            }
        }

        private ClientGame() { }

        public void Start()
        {
            ShowMainMenu();
            var gameStarted = false;
            var menu = Convert.ToInt32(Console.ReadLine());
            while (!gameStarted && (menu != 2))
            {
                switch (menu)
                {
                    case 1:
                        gameStarted = true;
                        StartGame();
                        break;
                }
                
                if (!gameStarted) menu = Convert.ToInt32(Console.ReadLine());
            }
            
        }

        private void StartGame()
        {
            ConnectionManager.Instance.SendMessage(new Message<object> {Code="startGame",Objects = new object()});
            Console.WriteLine("Waiting for other players...");
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Start Game");
            Console.WriteLine("2. Exit");
        }

        private void ShowIngameMenu()
        {
            //Console.Clear();
            Console.WriteLine("Your cards:");
            ShowCards();
            Console.WriteLine();
            Console.WriteLine("1. Draw a card");
            Console.WriteLine("2. Drop a card");

        }

        private void ShowWaitingMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Waiting for others...");
            Console.WriteLine("0. Continue game");
            Console.WriteLine("1. Say UNO");
            Console.WriteLine("2. Say Message");
            Console.WriteLine("3. Exit");
            WaitingMenu();
        }

        private void WaitingMenu()
        {
            var select = Convert.ToInt32(Console.ReadLine());
            switch (select)
            {
                case 1:
                    ConnectionManager.Instance.SendMessage(new Message<object> { Code = "uno", Objects = new object() });
                    break;
            }
        }

        public void SetUser(User pUser)
        {
            GamerUser = pUser;
            Console.WriteLine(pUser.connectionId);
        }

        public void SetCards(HandCards handCards)
        {
            GamerUser.handCards.DeleteCards();
            foreach (Card card in handCards.cardsList)
            {
               GamerUser.handCards.cardsList.Add(card);
            }
            ShowCards();

        }

        public void ShowCards()
        {
            //Console.Clear();
            int i = 0;
            foreach (Card card in GamerUser.handCards.cardsList)
            {
                Console.WriteLine(i++ +": "+card.ToString());
            }
        }



        public void ShowThrowDeck(string topCard)
        {


            
               

            
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("*YOUR TURN!*");
            Console.WriteLine("ThrowDeck Top: "+topCard);
            ShowIngameMenu();
            var select = Convert.ToInt32(Console.ReadLine());
            switch (select)
            {
                case 2:
                    Console.Write("Card: ");
                    var selectedCard = Convert.ToInt32(Console.ReadLine());
                    ConnectionManager.Instance.SendMessage(new Message<Card> {Code="throwCard",Objects = GamerUser.handCards.TakeCard(selectedCard,false)});
                    break;
                case 1:
                    ConnectionManager.Instance.SendMessage(new Message<object> { Code = "drawCard", Objects = new object()});
                    break;
            }
            Console.Clear();
           

        }

        public void ShowOthersCard(List<KeyValuePair<string, int>> count)
        {
            //Console.Clear();
            foreach (var other in count)
            {
                Console.WriteLine("Player "+other.Key+" has "+other.Value+" cards.");
            }
            ShowWaitingMenu();
            


        }

        public void ShowNotValid()
        {
            //Console.Clear();
            Console.WriteLine("Not a valid move!");
        }

        public void GotSkipped()
        {
            Console.WriteLine("* YOU JUST GOT SKIPPED! *");
        }

        public void Reversed()
        {
            Console.WriteLine("* ORDER REVERSED! *");
        }

        public void GotPlus(int num)
        {
            Console.WriteLine("* JUST GOT SKIPPED AND "+num+" EXTRA CARDS! *");
        }

        public void SelectColor()
        {
            Console.WriteLine();
            Console.WriteLine("Red: 1");
            Console.WriteLine("Yellow: 2");
            Console.WriteLine("Green: 3");
            Console.WriteLine("Blue: 4");
            Console.Write("Select a color: ");
            int select = Convert.ToInt32(Console.ReadLine());
            string cstr = "";
            if (select == 1) cstr = "red";
            if (select == 2) cstr = "yellow";
            if (select == 3) cstr = "green";
            if (select == 4) cstr = "blue";
            ConnectionManager.Instance.SendMessage(new Message<string> {Code = "selectedColor", Objects = cstr});
        }

        public void GotPenalty()
        {
            Console.WriteLine("* YOU JUST GOT PENALTY! SAY UNO NEXT TIME! *");
        }
    }
}
