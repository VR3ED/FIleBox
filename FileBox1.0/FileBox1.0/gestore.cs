using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileBox1._0
{
    class gestore
    {
        List<byte> listaByte = new List<byte>();
        public NetworkStream canale;
        public string risposta;
        public byte[] fileR;

        public void invia (byte[] invio)
        {
            byte[] msg = new byte[1024];

            
            try
            {
                msg = invio;
                TcpClient connessione = new TcpClient(Dns.GetHostName(), 23456);
                canale = connessione.GetStream();
                
                canale.Write(msg, 0, msg.Length);

                //canale.Read(dato, 0, dato.Length);
            }
            catch
            {
                MessageBox.Show("si è verificato un errore nella connessione al server", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                //canale.Close();
            }
        }

        public void inviaFile(byte[] invio)
        {
            try
            {
                TcpClient connessione = new TcpClient(Dns.GetHostName(), 23456);
                canale = connessione.GetStream();

                canale.Write(invio, 0, invio.Length);

                //canale.Read(dato, 0, dato.Length);
            }
            catch
            {
                MessageBox.Show("si è verificato un errore nella connessione al server", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                canale.Close();
            }
        }


        public void riceviFile ()
        {
            try
            {
                byte[] dato = new byte[1024];
                int l = 1024;

                listaByte.Clear();
                try
                {
                    while ((l = canale.Read(dato, 0, dato.Length)) > 0 && canale.DataAvailable)
                    {
                        listaByte.AddRange(dato);
                    }
                }
                catch (Exception)
                { }          

                fileR = listaByte.ToArray();
                canale.Close();
            }
            catch
            {
                MessageBox.Show("si è verificato un errore nella connessione al server", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                //canale.Close();
            }
        }


        public void riceviMex()
        {
            try
            {
                byte[] dato = new byte[1024];

                canale.Read(dato, 0, dato.Length);
                    
                risposta = Encoding.ASCII.GetString(dato);
                canale.Close();
            }
            catch
            {
                MessageBox.Show("si è verificato un errore nella connessione al server", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                canale.Close();
            }
        }
    }
}
