using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using unoClient;
using uno;

namespace unoClientForm
{
    public class ClientGame
    {

        private static volatile ClientGame instance;
        private static object syncRoot = new Object();
        public User GamerUser;
        private int mode = 0; // 0 - mainmenu; 1 - ingamewaiting; 2 - ingame; 3 - waitforcard; 4 - selectcolor
        private form_main ui = null;



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

        public void Start(form_main ui)
        {
            this.ui = ui;
            ShowMainMenu();
            mode = 0;
        }


        public void Input(string input)
        {
            int select = Convert.ToInt32(input);
            switch (mode)
            {
                case 0:
                    //mainmenu
                    if (select == 1)
                    {
                        mode = 1;
                        StartGame();
                    }
                    break;
                case 1:
                    //ingamewaiting
                    if (select == 1)
                    {
                        ConnectionManager.Instance.SendMessage(new Message<object> { Code = "uno", Objects = new object() });
                    } else if (select == 2)
                    {
                        ShowCards();
                        ShowWaitingMenu();
                    }
                    break;
                case 2:
                    //ingame
                    switch (select)
                    {
                        case 2:
                            ui.Write("Card: ");
                            mode = 3;
                            break;
                        case 1:
                            ConnectionManager.Instance.SendMessage(new Message<object> { Code = "drawCard", Objects = new object() });
                            break;
                    }
                    break;
                case 3:
                    //waitforcard
                    ConnectionManager.Instance.SendMessage(new Message<Card> { Code = "throwCard", Objects = GamerUser.handCards.TakeCard(select, false) });
                    mode = 1;
                    break;
                case 4:
                    //selectcolor
                    string cstr = "";
                    if (select == 1) cstr = "red";
                    if (select == 2) cstr = "yellow";
                    if (select == 3) cstr = "green";
                    if (select == 4) cstr = "blue";
                    ConnectionManager.Instance.SendMessage(new Message<string> {Code = "selectedColor", Objects = cstr});
                    mode = 1;
                    break;
            }
        }

        private void StartGame()
        {
            ConnectionManager.Instance.SendMessage(new Message<object> {Code="startGame",Objects = new object()});
            ui.WriteLine("Waiting for other players...");
            
        }

        private void ShowMainMenu()
        {
            ui.Clear();
            ui.WriteLine("1. Start Game");
            ui.WriteLine("2. Exit");
        }

        private void ShowIngameMenu()
        {
            //Console.Clear();
            mode = 2;
            ui.WriteLine("Your cards:");
            ShowCards();
            ui.WriteLine();
            ui.WriteLine("1. Draw a card");
            ui.WriteLine("2. Drop a card");

        }

        private void ShowWaitingMenu()
        {
            mode = 1;
            ui.WriteLine();
            ui.WriteLine("Waiting for others...");
            
            ui.WriteLine("1. Say UNO");
            ui.WriteLine("2. Show me my cards");
            ui.WriteLine("3. Say Message");
            ui.WriteLine("4. Exit");
        }

  

        public void SetUser(User pUser)
        {
            GamerUser = pUser;
            ui.WriteLine(pUser.connectionId);
        }

        public void SetCards(HandCards handCards)
        {
            GamerUser.handCards.DeleteCards();
            foreach (Card card in handCards.cardsList)
            {
               GamerUser.handCards.cardsList.Add(card);
            }
            //ShowCards();

        }

        public void ShowCards()
        {
            //Console.Clear();
            ui.WriteLine();
            int i = 0;
            foreach (Card card in GamerUser.handCards.cardsList)
            {
                ui.WriteLine(i++ +": "+card.ToString());
            }
        }



        public void ShowThrowDeck(string topCard)
        {
            //ui.Clear();
            ui.WriteLine();
            ui.WriteLine("*YOUR TURN!*");
            ui.WriteLine("ThrowDeck Top: "+topCard);
            ShowIngameMenu();
        }

        public void ShowOthersCard(List<KeyValuePair<string, int>> count)
        {
            foreach (var other in count)
            {
                ui.WriteLine("Player "+other.Key+" has "+other.Value+" cards.");
            }
            ShowWaitingMenu();
            


        }

        public void ShowNotValid()
        {
            //Console.Clear();
            ui.WriteLine("Not a valid move!");
        }

        public void GotSkipped()
        {
            ui.WriteLine("* YOU JUST GOT SKIPPED! *");
        }

        public void Reversed()
        {
            ui.WriteLine("* ORDER REVERSED! *");
        }

        public void GotPlus(int num)
        {
            ui.WriteLine("* JUST GOT SKIPPED AND "+num+" EXTRA CARDS! *");
        }

        public void SelectColor()
        {
            ui.WriteLine();
            ui.WriteLine("Red: 1");
            ui.WriteLine("Yellow: 2");
            ui.WriteLine("Green: 3");
            ui.WriteLine("Blue: 4");
            ui.Write("Select a color: ");
            mode = 4;

        }

        public void GotPenalty()
        {
            ui.WriteLine("* YOU JUST GOT PENALTY! SAY UNO NEXT TIME! *");
        }

        public void Won()
        {
            ui.WriteLine("* YOU WON THE GAME! CONGRAT! *");
        }

        public void Lost(string who)
        {
            ui.WriteLine("* "+who+" WON THE GAME! YOU LOST! *");
        }
    }
}
