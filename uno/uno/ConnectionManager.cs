using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace uno
{
    /**
     * POC
     */
    [DataContract]
    class User
    {
        [DataMember]
        public int clientId;
    }



    class ConnectionManager
    {
        TcpListener serverSocket;
        Hashtable clientsList;
        int idCounter;
        public ConnectionManager()
        {
            Int32 port = 8888;
            clientsList = new Hashtable();
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            serverSocket = new TcpListener(localAddr, port);
            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();
            Console.WriteLine("Server started listening...");
            idCounter = 0;
            while(true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                clientsList.Add(idCounter, clientSocket);
                Console.WriteLine("New connection accepted with id " + idCounter);
                HandleClient hc = new HandleClient(idCounter, clientsList);
                hc.handle();
                idCounter++;
            }


        }


  


    }

    class HandleClient
    {
        int cId;
        Hashtable clientsList;
        public HandleClient(int cId, Hashtable clientsList)
        {
            this.cId = cId;
            this.clientsList = clientsList;
        }

        public void handle()
        {
            Thread cThread = new Thread(start);
            cThread.Start();
        }

        private void start()
        {
            TcpClient clientSocket = (TcpClient)clientsList[cId];
            Console.WriteLine("Thread started for client " + cId);
            User user = new User()
            {
                clientId = cId
            };
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(User));
            /*MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, user);*/
            NetworkStream clientStream = clientSocket.GetStream();
            js.WriteObject(clientStream, user);
            clientSocket.Close();
            clientsList.Remove(cId);
            Console.WriteLine("Thread stopped for client " + cId);
            Thread.CurrentThread.Abort();
        }

    }
}
