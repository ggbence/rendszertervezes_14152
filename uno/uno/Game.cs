using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace uno
{
    public sealed class Game
    {

        private Hashtable usersHashtable;
        private List<Match> matchesList;
        public Hashtable userFinder;




        private static volatile Game instance;
        private static object syncRoot = new Object();

        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Game();
                    }
                }

                return instance;
            }
        }

        private Game()
        {
            usersHashtable = new Hashtable();
            matchesList=new List<Match>();
            userFinder=new Hashtable();
        }

        public void AddUser(User pUser)
        {
            usersHashtable.Add(pUser.connectionId, pUser);
            Message<User> m = new Message<User> {Code = "updateUser", Objects = pUser};
            ConnectionManager.Instance.SendMessage(m,pUser.connectionId);
        }



        public void start()
        {
            Lobby lobby = Lobby.Instance;
            ConnectionManager cm = ConnectionManager.Instance;
            cm.start();

        }

        public void UserToLobby(int connectionId)
        {
            Lobby.Instance.AddUser((User)usersHashtable[connectionId]);
            usersHashtable.Remove(connectionId);
        }

        public void AddMatch(Match m)
        {
            Console.WriteLine("Match created...");

            matchesList.Add(m);

            
        }


    }
}
