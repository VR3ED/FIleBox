using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFileBox1._1
{
    public class user
    {
        public string id;
        public string username;
        public string pass;
        public string spazio;

        public user()
        {

        }

        public user(string i, string u, string p)
        {
            id = i;
            username = u;
            pass = p;
            spazio = "0";
        }

    }
}
