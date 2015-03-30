using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uno
{
    public class Message<T>
    {
        public T Objects { get; set; }
        public string Code { get; set; }

        public Message()
        {
            Code = "none";
        }
    }
}
