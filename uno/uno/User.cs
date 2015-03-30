using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace uno
{
    public class User
    {
        public int connectionId { get; set; }

        public HandCards handCards { get; set; }


        public User()
        {
            handCards=new HandCards();
        }


        [BsonId]
        public Oid Id { get; private set; }

        [BsonElement]
        public string nickName { get; set; }



    }
}
