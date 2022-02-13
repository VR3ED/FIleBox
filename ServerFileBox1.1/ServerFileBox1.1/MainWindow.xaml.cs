using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Xml.Serialization;

namespace ServerFileBox1._1
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread t;
        bool attivo;
        DispatcherTimer timer = new DispatcherTimer();
        string comandi;
        List<user> utenti = new List<user>();
        int cont = 0;
        List<filec> files = new List<filec>();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer q = new XmlSerializer(typeof(List<user>));
                //apro stream reader
                //Uri u = new Uri(path, UriKind.Relative);
                StreamReader sr = new StreamReader(@"..\..\utenti\elenco.xml");
                //registro ciò che leggo
                utenti = ((List<user>)q.Deserialize(sr));
                //chiudo il deseializzatore perchè si
                sr.Close();

                //Istanzio l'oggetto serializzatore
                XmlSerializer a = new XmlSerializer(typeof(List<filec>));
                //apro stream reader
                //Uri u = new Uri(path, UriKind.Relative);
                StreamReader sr2 = new StreamReader(@"..\..\utenti\files.xml");
                //registro ciò che leggo
                files = ((List<filec>)a.Deserialize(sr2));
                //chiudo il deseializzatore perchè si
                sr2.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }

            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(comandi != "")
            {
                if(cont >= 19)
                {
                    txtCmd.Text = "";
                    cont = 0;
                }
                txtCmd.Text += comandi;
                try
                {
                    //cont += comandi.Split('\n').Length -1;
                }
                catch (Exception)
                { }
                comandi = "";
            }
            float tot = 0;
            foreach (user item in utenti)
            {
                tot += float.Parse(item.spazio);
            }
            pbStorage.Value = tot / 2000 * 100;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            attivo = true;

            TcpListener listener;
            IPAddress[] iPs = Dns.GetHostAddresses(Dns.GetHostName());
            txtCmd.Text += "Serever avviato \n";
            finestra.Title = "Server (acceso)";

            t = new Thread(new ThreadStart(() =>
            {
                try
                {
                    listener = new TcpListener(iPs[0], 23456);
                    listener.Start();
                    while (attivo)
                    {
                        TcpClient connection = listener.AcceptTcpClient();

                        Gestore elab = new Gestore(connection, ref comandi, ref utenti, ref files);
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.ToString());
                }

            }));

            t.IsBackground = true;
            t.Start();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            attivo = false;

            txtCmd.Text += "Serever spento \n";
            finestra.Title = "Server (spento)";
        }

        private void finestra_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            attivo = false;
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer s = new XmlSerializer(typeof(List<user>));
                //creo lo stream di dati
                StreamWriter sw = new StreamWriter(@"..\..\utenti\elenco.xml", false);
                //serializz su un file (flusso,ogetto da serisalizzare)
                s.Serialize(sw, utenti);
                //chiudo il seializzatore perchè si
                sw.Close();

                //Istanzio l'oggetto serializzatore
                XmlSerializer x = new XmlSerializer(typeof(List<filec>));
                //creo lo stream di dati
                StreamWriter sw2 = new StreamWriter(@"..\..\utenti\files.xml", false);
                //serializz su un file (flusso,ogetto da serisalizzare)
                x.Serialize(sw2, files);
                //chiudo il seializzatore perchè si
                sw2.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }
    }
}
