using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;
using uno;

namespace unoClient
{


    
    public sealed class ConnectionManager
    {
        private TcpClient clientSocket;

        private static volatile ConnectionManager instance;
        private static object syncRoot = new Object();


        public static ConnectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ConnectionManager();
                    }
                }

                return instance;
            }
        }

        private ConnectionManager()
        {
            
        }

        public void ConnectToServer()
        {
            clientSocket = new TcpClient();
            clientSocket.Connect("localhost", 8888);
            var hs = new HandleServer(clientSocket);
            hs.handle();
        }

        public void SendMessage<T>(Message<T> m)
        {
            NetworkStream serverStream = clientSocket.GetStream();
            StreamWriter sw = new StreamWriter(serverStream);
            string line;
            StringWriter stringWriter = new StringWriter();

            var js = new JsonSerializer();
            js.Serialize(stringWriter, m);
            line = stringWriter.ToString();
            Console.WriteLine(line);
            sw.WriteLine(line);
            sw.Flush();
        }
        



    }

    class HandleServer
    {
        private TcpClient clientSocket;



        public HandleServer(TcpClient clientSocket)
        {
            this.clientSocket = clientSocket;
        }
        
        public void handle()
        {
            var sThread = new Thread(start);
            sThread.Start();
        }

        private void start()
        {
            NetworkStream serverStream = clientSocket.GetStream();
            StreamReader sr = new StreamReader(serverStream);
            JsonSerializer js = new JsonSerializer();
            
            string line;
            while (true)
            {
                    line = sr.ReadLine();
                    Console.WriteLine("Message :"+line);
                    StringReader stringReader = new StringReader(line);

                    var m = (Message<object>) js.Deserialize(stringReader, typeof (Message<object>));
                    var obj = (Newtonsoft.Json.Linq.JObject) m.Objects;

                    switch (m.Code)
                    {
                        case "updateUser":
                            User newUser = obj.ToObject<User>();
                            ClientGame.Instance.SetUser(newUser);
                            break;

                        case "updateCards":
                            HandCards newHandCards = obj.ToObject<HandCards>();
                            ClientGame.Instance.SetCards(newHandCards);
                            break;

                        case "showThrowDeck":
                            Card topCard = obj.ToObject<Card>();
                            ClientGame.Instance.ShowCards();
                            ClientGame.Instance.ShowThrowDeck(topCard);
                            break;




                    }
                    
                }

            
        }
    }
}
