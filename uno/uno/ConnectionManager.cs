using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json.Linq;

namespace uno
{
    public sealed class ConnectionManager
    {
        private static volatile ConnectionManager instance;
        private static readonly object syncRoot = new Object();
        public Hashtable clientsList;
        private int idCounter;
        private TcpListener serverSocket;

        private ConnectionManager()
        {
        }

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

        public void SendMessage<T>(Message<T> m, int connectionId)
        {
            var clientStream = ((HandleClient) clientsList[connectionId]).clientSocket.GetStream();
            var sw = new StreamWriter(clientStream);
            string line;
            var stringWriter = new StringWriter();

            var js = new JsonSerializer();
            js.Serialize(stringWriter, m);
            line = stringWriter.ToString();
            Console.WriteLine(line);
            sw.WriteLine(line);
            sw.Flush();
        }

        public void start()
        {
            var port = 8888;
            clientsList = new Hashtable();
            var localAddr = IPAddress.Parse("127.0.0.1");
            serverSocket = new TcpListener(localAddr, port);


            serverSocket.Start();
            Console.WriteLine("Server started listening...");
            idCounter = 0;
            while (true)
            {
                var clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("New connection accepted with id " + idCounter);
                var hc = new HandleClient(idCounter, clientSocket);
                clientsList.Add(idCounter++, hc);
                hc.handle();
            }
        }
    }

    internal class HandleClient
    {
        private readonly int connectionId;

        public HandleClient(int cId, TcpClient clientSocket)
        {
            connectionId = cId;
            this.clientSocket = clientSocket;
            waitForData = false;
        }

        public TcpClient clientSocket { get; set; }
        private NetworkStream clientStream { get; set; }
        public Message<object> LastMessage { get; set; }
        public bool waitForData { get; set; }

        public void handle()
        {
            var cThread = new Thread(start);
            cThread.Start();
        }

        private void start()
        {
            Console.WriteLine("Thread started for client " + connectionId);
            var user = new User
            {
                connectionId = connectionId
            };
            Game.Instance.AddUser(user);


            clientStream = clientSocket.GetStream();


            while (true)
            {
                var m = ReadLine();
                LastMessage = m;
                var obj = (JObject) m.Objects;
                switch (m.Code)
                {
                    case "startGame":
                        Game.Instance.UserToLobby(connectionId);
                        Console.WriteLine(connectionId + " wants to play...");
                        break;
                }
            }


            /*clientSocket.Close();
            clientsList.Remove(connectionId);
            Console.WriteLine("Thread stopped for client " + connectionId);
            Thread.CurrentThread.Abort();*/
        }

        public Message<object> ReadLine()
        {
            var sr = new StreamReader(clientStream);
            var line = sr.ReadLine();
            Console.WriteLine(line);
            var stringReader = new StringReader(line);
            var js = new JsonSerializer();
            if (waitForData) waitForData = false;
            return (Message<object>) js.Deserialize(stringReader, typeof (Message<object>));
        }
    }
}