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
            string strNum;
            strNum = Number.ToString();
            if (Number == 10) strNum = "skip";
            if (Number == 11) strNum = "reverse";
            if (Number == 12) strNum = "plus2";

            if (Color == "black")
            {
                if (Number == 0) strNum = "select";
                else strNum = "select+4";
            }

            return Color + " " + strNum;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (Card)) return base.Equals(obj);
            var c = (Card) obj;
            return ((c.Color == this.Color) && (c.Number == this.Number));
        }
    }
}
