using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uno
{
    public class Card
    {
        public Card(int pNumber, string pColor)
        {
            Number = pNumber;
            Color = pColor;
        }
        
        public int Number { get; set; }
        public string Color { get; set; }

        public override string ToString()
        {
            return Color + " " + Number;
        }
    }
}
