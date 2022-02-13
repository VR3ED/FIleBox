using FileBox1._0.imm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net;
using System.Linq;
using NAudio.Wave;


namespace FileBox1._0
{
    /// <summary>
    /// Logica di interazione per FileBox.xaml
    /// </summary>
    public partial class FileBox : Window
    {
        string ID;
        string username;
        //DispatcherTimer timer = new DispatcherTimer();
        double vel = 0;
        //Thread speed;
        List<filec> temp = new List<filec>();
        public FileBox(string i, string u, string s)
        {
            InitializeComponent();
            ID = i;
            username = u;

            try
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FileBox"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FileBox");
                }
            }
            catch (Exception)
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FileBox");
            }
            

            try
            {
                txtNome.Content = username;
                aggiornaStorage(s);
                txtNome.Icon = username[0];
                refresh();
            }
            catch (Exception)
            { }

            //timer.Interval = TimeSpan.FromSeconds(5);
            //timer.Tick += Timer_Tick;
            //timer.Start();
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    double vel = 0;
        //    speed = new Thread(new ThreadStart(speedtest));
        //    //speed.IsBackground = true;
        //    speed.Start();
        //    if (speed != null)
        //    {
        //        if(speed.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
        //        {
        //            File.Delete("100MB.dat");
        //        }
        //    }
        //    txtVelocità.Content = string.Format("{0}KB/s", vel);
        //}

        public void speedtest()
        {
            //questo metodo doveva servire per fare un thread che ogni tot mi faceva uno speedtest, 
            //anche se non funziona l'ho voluto lasciare, tanto non lo richiamo mai
            double[] speeds = new double[5];
            for (int i = 0; i < 5; i++)
            {
                int jQueryFileSize = 12208; //Size of File in KB.
                WebClient client = new WebClient();
                DateTime startTime = DateTime.Now;
                
                client.DownloadFile("http://ovh.net/files/100Mb.dat", "100MB.dat");
                DateTime endTime = DateTime.Now;
                speeds[i] = Math.Round((jQueryFileSize / (endTime - startTime).TotalSeconds));
                vel = speeds.Average();
            }
        }

        public void refresh()
        {
            gestore refr = new gestore();
            refr.invia(Encoding.ASCII.GetBytes("refresh|"+ ID));
            refr.riceviMex();

            float dimtot=0;
            if (refr.risposta.Split('\0')[0] != "nulla")
            {
                lxLista.Items.Clear();
                temp.Clear();
                List<string> nomi = new List<string>();
                nomi = refr.risposta.Split('|').ToList<string>();
                foreach (string item in nomi)
                {
                    if(!item.Contains('\0'))
                    {
                        lxLista.Items.Add(new filec(item.Split(',')[0], item.Split(',')[1]));
                        temp.Add(new filec(item.Split(',')[0], item.Split(',')[1]));
                        float dim = 0;
                        if (item.Split(',')[1].Contains("Mb"))
                        {
                            dim = float.Parse(item.Split(',')[1].Split('M')[0]);
                        }
                        if (item.Split(',')[1].Contains("Kb"))
                        {
                            dim = float.Parse(item.Split(',')[1].Split('K')[0]) / 1000;
                        }
                        dimtot += dim;
                    }             
                }
                storage.Value = (dimtot  / 200) * 100;
                txtSpazio.Content = (dimtot).ToString() + "/200mb";
            }
            else
            {
                storage.Value = 0;
                txtSpazio.Content = "0/200mb";
            }
            
        }

        private void aggiornaStorage(string s)
        {
            storage.Value = ((float.Parse(s) + float.Parse(txtSpazio.Content.ToString().Split('/')[0])) / 200) * 100;
            txtSpazio.Content = (float.Parse(s) + float.Parse(txtSpazio.Content.ToString().Split('/')[0])).ToString() + "/200mb";
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //invio file da memoria
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                filec perfavore = new filec( openFileDialog.FileName ) ;

                gestore inviaF = new gestore();
                inviaF.invia(Encoding.ASCII.GetBytes("file|" + ID + "|" + perfavore.nome + "|" + perfavore.peso));
                //inviaF.canale.Close();

                byte[] x = File.ReadAllBytes(openFileDialog.FileName);
                Thread.Sleep(2000);
                inviaF.canale.Write(x,0,x.Length);

                inviaF.riceviMex();
                lxStorico.Items.Add( new comando( inviaF.risposta, DateTime.Now)) ;

                lxLista.Items.Add(perfavore);
                temp.Add(new filec(openFileDialog.FileName));
                float dim =0;
                if (perfavore.peso.Contains("Mb"))
                {
                    dim = float.Parse(perfavore.peso.Split('M')[0]);
                }
                if (perfavore.peso.Contains("Kb"))
                {
                    dim = float.Parse(perfavore.peso.Split('K')[0]) / 1000;
                }
                aggiornaStorage(dim.ToString());
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //log out
            MainWindow m = new MainWindow();
            m.Show();
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //elimina prfilo
            if (MessageBox.Show("sei sicuro di voler cancellare definitivamente l'account?", "Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                gestore cancella = new gestore();
                cancella.invia(Encoding.ASCII.GetBytes("del|" + ID));
                cancella.riceviMex();
                MessageBox.Show(cancella.risposta);
                if (cancella.risposta.Contains("eliminazione avvenuta con successo"))
                {
                    MainWindow m = new MainWindow();
                    m.Show();
                    this.Close();
                }
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //esci dall'aplicazione
            this.Close();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            //refresh
            refresh();
        }

        private void btnElimina_Click(object sender, RoutedEventArgs e)
        {
            //elimina file selezionati
            int M = lxLista.SelectedItems.Count;
            for(int i=0;i<M;i++)
            {
                gestore cancelliere = new gestore();
                float dim = 0;
                if (((filec)lxLista.SelectedItems[0]).peso.Contains("Mb"))                
                {
                    dim = float.Parse(((filec)lxLista.SelectedItems[0]).peso.Split('M')[0]);
                }
                if ((((filec)lxLista.SelectedItems[0]).peso.Contains("Kb")))
                {
                    dim = float.Parse(((filec)lxLista.SelectedItems[0]).peso.Split('K')[0]) / 1000;
                }
                cancelliere.invia(Encoding.ASCII.GetBytes("dllf|" + ID + "|" + ((filec)lxLista.SelectedItems[0]).nome +"|"+ dim.ToString()));
                cancelliere.riceviMex();
                lxStorico.Items.Add(new comando(cancelliere.risposta, DateTime.Now));
                lxLista.Items.Remove(lxLista.SelectedItems[0]);
            }
            refresh();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            //scarica file selezionati
            int M = lxLista.SelectedItems.Count;
            for (int i = 0; i < M; i++)
            {
                gestore scaricatore = new gestore();

                scaricatore.invia(Encoding.ASCII.GetBytes("dwn|" + ID + "|" + ((filec)lxLista.SelectedItems[i]).nome));
                scaricatore.riceviFile();

                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FileBox\"+ ((filec)lxLista.SelectedItems[i]).nome, scaricatore.fileR);
                
                lxStorico.Items.Add(new comando("File scaricato con successo",DateTime.Now));
            }
            if (MessageBox.Show("I file somo stati scaricati correttamente\n Vuoi aprire la cartella download?", "Download", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process.Start(@Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FileBox");
            }
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            lxLista.Items.Clear();
            foreach (filec item in temp)
            {
                if(item.nome.Contains(searchBar.Text))
                {
                    lxLista.Items.Add(item);
                }
            }
        }

        private void lxLista_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string nome = ((filec)lxLista.SelectedItems[0]).nome;
                //MessageBox.Show("ojk");
                if (nome.Split('.')[1] == "jpg" || nome.Split('.')[1] == "png" || nome.Split('.')[1] == "jpeg")
                {
                    gestore scaricatore = new gestore();

                    scaricatore.invia(Encoding.ASCII.GetBytes("dwn|" + ID + "|" + nome));
                    scaricatore.riceviFile();

                    Immagini x = new Immagini(scaricatore.fileR, nome);
                    x.Show();
                }
                if(nome.Split('.')[1] == "mp3" || nome.Split('.')[1] == "wav")
                {
                    // 
                    gestore scaricatore = new gestore();

                    scaricatore.invia(Encoding.ASCII.GetBytes("dwn|" + ID + "|" + nome));
                    scaricatore.riceviFile();

                    byte[] aud = scaricatore.fileR;
                    if(nome.Split('.')[1] == "mp3")
                    {
                        File.WriteAllBytes(nome, scaricatore.fileR);
                        using (Mp3FileReader mp3 = new Mp3FileReader(nome))
                        {
                            using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                            {
                                WaveFileWriter.CreateWaveFile(nome.Split('.')[0]+".waw", pcm);
                            }
                        }
                        aud = File.ReadAllBytes(nome.Split('.')[0] + ".waw");
                    }
                    

                    audio x = new audio(aud, nome);
                    x.Show();
                }
            }
            catch (Exception E)
            { MessageBox.Show(E.Message);  }
        }
    }
}
