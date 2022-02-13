using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFileBox1._1
{
    public class filec
    {
        public string idU { get; set; }
        public string nome { get; set; }
        public string peso { get; set; }

        public filec()
        {

        }

        public filec(string n, string i, string p)
        {
            idU = i;
            nome = n;
            peso = p;
        }
    }

}
