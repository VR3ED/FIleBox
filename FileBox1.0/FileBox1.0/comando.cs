using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox1._0.imm
{
    public class comando
    {
        public DateTime data { get; set; }
        public string operazione { get; set; }

        public comando(string c, DateTime d)
        {
            operazione = c;
            data = d;
        }
    }
}
