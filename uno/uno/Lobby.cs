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


        private List<User> usersArrayList;
        private readonly int startNumber = 1;

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
            usersArrayList=new List<User>();
        }

        public void AddUser(User pUser)
        {
            usersArrayList.Add(pUser);
            if (usersArrayList.Count == startNumber)
            {
                MakeMatch();
            }
        }

        

        private void MakeMatch()
        {
            Console.WriteLine("A new match is going to be created with "+usersArrayList.Count+" players ...");
            Match m = new Match
            {
                actualPlayer = ((User) usersArrayList[0]).connectionId,
                reverse = false
            };
            foreach (User player in usersArrayList) { m.AddPlayer(player); }

            usersArrayList.Clear();
            m.RunMatch();
            Game.Instance.AddMatch(m);
        }

        
    }
}
