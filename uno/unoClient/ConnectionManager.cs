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

namespace unoClient
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
        private NetworkStream serverStream;
        private TcpClient clientSocket;

        public ConnectionManager()
        {
            clientSocket = new System.Net.Sockets.TcpClient();
            serverStream = default(NetworkStream);
            clientSocket.Connect("localhost", 8888);
            serverStream = clientSocket.GetStream();
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(User));
            User user = (User)js.ReadObject(serverStream);
            Console.WriteLine(user.clientId);
        }
        



    }
}
