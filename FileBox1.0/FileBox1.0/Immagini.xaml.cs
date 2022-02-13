using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileBox1._0
{
    /// <summary>
    /// Logica di interazione per Immagini.xaml
    /// </summary>
    public partial class Immagini : Window
    {
        public Immagini(byte[] imm, string nome)
        {
            InitializeComponent();

            BitmapImage b = new BitmapImage();
            var stream = new MemoryStream(imm);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            txtTitolo.Header = nome;
            img.Source = image;
        }
    }
}
