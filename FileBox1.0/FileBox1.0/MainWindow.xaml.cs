using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileBox1._0
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool accedi = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(accedi)
            {
                scritta.Content = "Hai già un account?";
                btnAccedi.Content = "Registrati";
                lblSwitch.Text = "ACCEDI!";  
                accedi = false;
            }
            else
            {
                scritta.Content = "Non sei iscritto?";
                btnAccedi.Content = "Accedi";
                lblSwitch.Text = "REGISTRATI!";
                accedi = true;
            }
            
        }

        private void btnAccedi_Click(object sender, RoutedEventArgs e)
        {
            string mex = null;
            byte[] msg;
            if (accedi)
            {
                mex = "acc|"+txtNome.Text+"|"+txtPass.Password;
                gestore accedi = new gestore();
                accedi.invia(Encoding.ASCII.GetBytes(mex));
                accedi.riceviMex();
                if(accedi.risposta.Contains("ok"))
                {
                    var window = new FileBox(accedi.risposta.Split('|')[3], accedi.risposta.Split('|')[1],accedi.risposta.Split('|')[2]);
                    window.Show();
                    this.Close();
                }
                //MessageBox.Show(accedi.risposta);
            }
            else
            {
                mex = "reg|" + txtNome.Text + "|" + txtPass.Password;
                gestore registra = new gestore();
                registra.invia(Encoding.ASCII.GetBytes(mex));
                registra.riceviMex();
                MessageBox.Show(registra.risposta);
            }

        }
    }
}
