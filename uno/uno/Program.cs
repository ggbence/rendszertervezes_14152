using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace uno
{
    class Program
    {

        
        static void Main(string[] args)
        {
            var g = Game.Instance;
            g.start();
        }
    }
}
