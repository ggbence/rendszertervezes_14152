using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            bool gameStarted = false;
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
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Játék indítása");
            Console.WriteLine("2. Kilépés");
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

        public void ShowThrowDeck(Card topCard)
        {
            Console.WriteLine();
            Console.WriteLine("ThrowDeck Top: "+topCard.ToString());
            var select = Convert.ToInt32(Console.ReadLine());
            ConnectionManager.Instance.SendMessage(new Message<Card> {Code="throwCard",Objects = GamerUser.handCards.TakeCard(select,false)});

        }

        public void ShowOthersCard(List<KeyValuePair<string, int>> count)
        {
            Console.Clear();
            foreach (var other in count)
            {
                Console.WriteLine(other.Key+": "+other.Value);
            }
        }

        public void ShowNotValid()
        {
            Console.Clear();
            Console.WriteLine("Not a valid move!");
        }
    }
}
