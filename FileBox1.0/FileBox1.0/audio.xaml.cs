using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
using System.Windows.Threading;

namespace FileBox1._0
{
    /// <summary>
    /// Logica di interazione per audio.xaml
    /// </summary>
    public partial class audio : Window
    {
        bool play = false;
        SoundPlayer player;
        MemoryStream ms;

        public audio(byte[] aud, string nome)
        {
            InitializeComponent();

            ms = new MemoryStream(aud);
            player = new SoundPlayer(ms);
           
            txtTitolo.Header = nome;

        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(play)
            {
                icona.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                player.Stop();
                play = false;
            }
            else
            {
                try
                {
                    icona.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                    play = true;
                    player.Play();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }            
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            icona.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            player.Stop();
            play = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ms.Close();
        }
    }
}
