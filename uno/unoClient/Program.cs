using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionManager cm = ConnectionManager.Instance;
            cm.ConnectToServer();
            ClientGame.Instance.Start();
        }
    }
}
