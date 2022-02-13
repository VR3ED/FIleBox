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
using System.Xml.Serialization;

namespace ServerFileBox1._1
{
    class Gestore
    {
        TcpClient connection;
        string txtCmd = "";
        List<byte> listaByte = new List<byte>();
        List<user> utenti = new List<user>();
        List<filec> files = new List<filec>();
        public Gestore(TcpClient connection,ref string t, ref List<user> lis, ref List<filec> fil)
        {
            this.connection = connection;
            Thread u = new Thread(new ThreadStart(ScambioDati));
            utenti = lis;
            files = fil;
            u.Start();
            while (u.IsAlive);
            t = txtCmd;
            lis = utenti;
            fil = files;
        }

        public void ScambioDati()
        {
            byte[] mex = new byte[1024];

            NetworkStream canale = connection.GetStream();

            canale.Read(mex, 0, mex.Length);
            //string cmd = Encoding.ASCII.GetString(listaByte.ToArray());
            string cmd = Encoding.ASCII.GetString(mex);

            //gestione accessi
            if (cmd.Contains("acc") && !cmd.Contains("file"))
            {
                txtCmd += "richiesta acesso: " + cmd + "\n";
                bool accedi = false;
                user profilo = new user(); ;
                foreach (user item in utenti)
                {
                    if (item.username == cmd.Split('|')[1] && item.pass == cmd.Split('|')[2].Split('\0')[0])
                    {
                        accedi = true;
                        profilo.id = item.id;
                        profilo.username = item.username;
                        profilo.spazio = item.spazio;
                    }
                }
                if (accedi)
                {
                    byte[] dato = Encoding.ASCII.GetBytes("ok|" + profilo.username + "|" + profilo.spazio + "|" + profilo.id + "|");
                    canale.Write(dato, 0, dato.Length);
                    txtCmd += "accesso garantito \n";
                }
                else
                {
                    byte[] dato = Encoding.ASCII.GetBytes("noooo");
                    canale.Write(dato, 0, dato.Length);
                    txtCmd += "accesso non garantito \n";
                }
            }

            //parte registrazione
            if (cmd.Contains("reg") && !cmd.Contains("file"))
            {
                txtCmd += "richiesta iscrizione: " + cmd + "\n";

                user u;
                try
                {
                    u = new user(cmd.Split('|')[1] + (utenti[utenti.Count - 1].id + 1).ToString(), cmd.Split('|')[1], cmd.Split('|')[2].Split('\0')[0]);
                }
                catch (Exception)
                {
                    u = new user(cmd.Split('|')[1] + "1", cmd.Split('|')[1], cmd.Split('|')[2].Split('\0')[0]);
                }
                utenti.Add(u);
                Directory.CreateDirectory(@"..\..\utenti\" + u.id);

                txtCmd += "registrazione effettuata con successo \n";

                byte[] dato = new byte[1024];
                dato = Encoding.ASCII.GetBytes("ok");
                canale.Write(dato, 0, dato.Length);
                canale.Close();
            }

            //cancella account
            if (cmd.Contains("del") && !cmd.Contains("file"))
            {
                string id = cmd.Split('|')[1].Split('\0')[0];

                try
                {
                    txtCmd += "richiesta cancellazione accounto:" + cmd + "\n";
                    Directory.Delete(@"..\..\utenti\" + id);
                    foreach (user item in utenti)
                    {
                        if (item.id.ToString() == id)
                        {
                            utenti.Remove(item);
                            break;
                        }
                    }
                    List<int> ll = new List<int>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i].idU == id)
                        {
                            ll.Add(i);
                        }
                    }
                    foreach (int item in ll)
                    {
                        files.RemoveAt(item);
                    }
                    byte[] dato = Encoding.ASCII.GetBytes("eliminazione avvenuta con successo");
                    txtCmd += "eliminazione avvenuta con successo \n";
                    canale.Write(dato, 0, dato.Length);
                }
                catch (Exception E)
                {
                    byte[] dato = Encoding.ASCII.GetBytes("si è verificato un problema, assicurati di cancellare prima tutti i tuoi file e poi riprova");
                    txtCmd += "eliminazione non avvenuta, errore \n";
                    canale.Write(dato, 0, dato.Length);
                }
            }

            //ricevi file
            if (cmd.Contains("file"))
            {
                txtCmd += "richiesta storage file \n";
                string id = cmd.Split('|')[1].Split('\0')[0];

                try
                {
                    byte[] file = new byte[1024];
                    int l = 1024;
                    float dim = 0;
                    if (cmd.Contains("Mb"))
                    {
                        dim = float.Parse(cmd.Split('|')[3].Split('M')[0]);
                    }
                    if (cmd.Contains("Kb"))
                    {
                        dim = float.Parse(cmd.Split('|')[3].Split('K')[0]) / 1000;
                    }

                    foreach (user item in utenti)
                    {
                        if (item.id.ToString() == id)
                        {
                            if (float.Parse(item.spazio) + dim < 200)
                            {
                                listaByte.Clear();
                                try
                                {
                                    lock (this)
                                    {
                                        try
                                        {
                                            while ((l = canale.Read(file, 0, file.Length)) > 0 && canale.DataAvailable)
                                            {
                                                listaByte.AddRange(file);
                                            }
                                        }
                                        catch (Exception E)
                                        {
                                            MessageBox.Show(E.ToString());
                                        }
                                    }
                                    string path = @"..\..\utenti\" + id + @"\" + cmd.Split('|')[2];
                                    File.WriteAllBytes(path, listaByte.ToArray());

                                    item.spazio = (float.Parse(item.spazio) + dim).ToString();
                                    txtCmd += "file ricevuto con successo \n";

                                    byte[] ris = Encoding.ASCII.GetBytes("file inviato con successo");
                                    canale.Write(ris, 0, ris.Length);
                                    files.Add(new filec(cmd.Split('|')[2], id, cmd.Split('|')[3].Split('\0')[0]));
                                }
                                catch (Exception E)
                                {
                                    MessageBox.Show(E.ToString());
                                }
                            }
                            else
                            {
                                byte[] ris = Encoding.ASCII.GetBytes("Hai utilizzato troppo spazio, elimina dei file e rirova");
                                canale.Write(ris, 0, ris.Length);
                            }
                        }
                    }
                }
                catch
                {
                    byte[] ris = Encoding.ASCII.GetBytes("si è verificato un errore");
                    canale.Write(ris, 0, ris.Length);
                    MessageBox.Show("si è verificato un errore ricevimento file", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    canale.Close();
                }
            }

            //referesh
            if (cmd.Contains("refresh") && !cmd.Contains("file"))
            {
                string ris = "";
                string id = cmd.Split('|')[1].Split('\0')[0];

                foreach (filec item in files)
                {
                    if (item.idU == id)
                    {
                        ris += item.nome + "," + item.peso + "|";
                    }
                }
                if (ris == "")
                {
                    ris = "nulla";
                }
                byte[] rix = Encoding.ASCII.GetBytes(ris);
                Thread.Sleep(500);
                canale.Write(rix, 0, rix.Length);
                txtCmd += "invio dati file \n";
            }

            //elimina un file dal server
            if (cmd.Contains("dllf") && !cmd.Contains("file"))
            {
                txtCmd += "richiesta eliminazione file\n";

                string id = cmd.Split('|')[1];
                string nome = cmd.Split('|')[2];
                float dim = float.Parse(cmd.Split('|')[3].Split('\0')[0]);


                List<int> ll = new List<int>();
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].idU == id && files[i].nome == nome)
                    {
                        ll.Add(i);
                        File.Delete(@"..\..\utenti\" + id + @"\" + nome);
                    }
                }
                foreach (user item in utenti)
                {
                    if (item.id == id)
                    {
                        item.spazio = (float.Parse(item.spazio) - dim).ToString();
                    }
                }
                foreach (int item in ll)
                {
                    files.RemoveAt(item);
                }
                byte[] dato = Encoding.ASCII.GetBytes("eliminazione file avvenuta con successo");
                txtCmd += "eliminazione avvenuta con successo \n";
                canale.Write(dato, 0, dato.Length);
            }

            //scarrica
            if (cmd.Contains("dwn") && !cmd.Contains("file"))
            {
                txtCmd += "richiesta scaricamento file\n";
                string id = cmd.Split('|')[1];
                string nome = cmd.Split('|')[2].Split('\0')[0];
                byte[] dato = File.ReadAllBytes(@"..\..\utenti\" + id + @"\" + nome);
                canale.Write(dato, 0, dato.Length);
                txtCmd += "file inviato al client con successo\n";
            }

        }
    }
}
