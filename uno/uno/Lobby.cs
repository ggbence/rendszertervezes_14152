using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uno
{
    public sealed class Lobby
    {

        private static volatile Lobby instance;
        private static object syncRoot = new Object();


        private List<User> userList;
        private readonly int startNumber = 3;

        public static Lobby Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Lobby();
                    }
                }

                return instance;
            }
        }

        private Lobby()
        {
            userList = new List<User>();
        }

        public void AddUser(User pUser)
        {
            userList.Add(pUser);
            if (userList.Count == startNumber)
            {
                MakeMatch();
            }
        }

        

        private void MakeMatch()
        {
            Console.WriteLine("A new match is going to be created with " + userList.Count + " players ...");
            Match m = new Match
            {
                actualPlayer = ((User)userList[0]).connectionId,
                reverse = false
            };
            foreach (User player in userList) { m.AddPlayer(player); }

            userList.Clear();
            m.RunMatch();
            Game.Instance.AddMatch(m);
        }

        
    }
}
