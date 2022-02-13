using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileBox1._0
{
    public class filec
    {
        public string nome { get; set; }
        public string peso { get; set; }
        public BitmapImage imm { get; set; }

        public filec(string path)
        {
            byte[] a;
            a = File.ReadAllBytes(path);
            peso = (a.Length / 1000000).ToString()+"Mb";
            if(peso=="0Mb")
            {
                peso = (a.Length / 1000).ToString() + "Kb";
            }

            nome = path.Split('\\')[path.Split('\\').Length - 1];

            if(nome.Split('.')[1] == "doc" || nome.Split('.')[1] == "docx")
            {
                imm = new BitmapImage(new Uri(@"\imm\doc.png", UriKind.Relative));
            }
            else if(nome.Split('.')[1] == "pdf")
            {
                imm = new BitmapImage(new Uri(@"\imm\pdf.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "ppt" || nome.Split('.')[1] == "pptx")
            {
                imm = new BitmapImage(new Uri(@"\imm\ppt.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "xls" || nome.Split('.')[1] == "xlsx")
            {
                imm = new BitmapImage(new Uri(@"\imm\xlsx.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "jpg" || nome.Split('.')[1] == "png" || nome.Split('.')[1] == "jpeg")
            {
                imm = new BitmapImage(new Uri(@"\imm\jpg.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "mp3" || nome.Split('.')[1] == "wav")
            {
                imm = new BitmapImage(new Uri(@"\imm\mp3.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "mp4")
            {
                imm = new BitmapImage(new Uri(@"\imm\mp4.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "txt")
            {
                imm = new BitmapImage(new Uri(@"\imm\txt.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "zip" || nome.Split('.')[1] == "rar" || nome.Split('.')[1] == "7z")
            {
                imm = new BitmapImage(new Uri(@"\imm\zip.png", UriKind.Relative));
            }
            else
            {
                imm = new BitmapImage(new Uri(@"\imm\blank-file.png", UriKind.Relative));
            }
        }

        public filec(string n, string p)
        {
            nome = n;
            peso = p;

            if (nome.Split('.')[1] == "doc" || nome.Split('.')[1] == "docx")
            {
                imm = new BitmapImage(new Uri(@"\imm\doc.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "pdf")
            {
                imm = new BitmapImage(new Uri(@"\imm\pdf.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "ppt" || nome.Split('.')[1] == "pptx")
            {
                imm = new BitmapImage(new Uri(@"\imm\ppt.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "xls" || nome.Split('.')[1] == "xlsx")
            {
                imm = new BitmapImage(new Uri(@"\imm\xlsx.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "jpg" || nome.Split('.')[1] == "png" || nome.Split('.')[1] == "jpeg")
            {
                imm = new BitmapImage(new Uri(@"\imm\jpg.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "mp3" || nome.Split('.')[1] == "wav")
            {
                imm = new BitmapImage(new Uri(@"\imm\mp3.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "mp4")
            {
                imm = new BitmapImage(new Uri(@"\imm\mp4.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "txt")
            {
                imm = new BitmapImage(new Uri(@"\imm\txt.png", UriKind.Relative));
            }
            else if (nome.Split('.')[1] == "zip" || nome.Split('.')[1] == "rar" || nome.Split('.')[1] == "7z")
            {
                imm = new BitmapImage(new Uri(@"\imm\zip.png", UriKind.Relative));
            }
            else
            {
                imm = new BitmapImage(new Uri(@"\imm\blank-file.png", UriKind.Relative));
            }
        }
    }
}
