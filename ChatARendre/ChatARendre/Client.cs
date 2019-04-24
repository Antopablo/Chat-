using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ChatARendre_Serveur
{
    class Client
    {
        int indexYY = 6;
        Socket s;
        private bool _ListeModified;
        private bool _estConnecte;
        public bool EstConnecte
        {
            get { return _estConnecte; }
            set { _estConnecte = value; }
        }
        private List<Client> ListeClient;

        private string _nom;


        public Client(Socket s, List<Client> listeClient, string nom)
        {
            this.s = s;
            EstConnecte = true;
            ListeClient = listeClient;
            Nom = nom;
            _ListeModified = false;
        }

        public string Nom
        {
            get { return _nom; }
            set { _nom = value; }
        }

        public void Envoyer(string msg)
        {
            byte[] tmp = Encoding.Default.GetBytes(msg, 0, msg.Length);
            s.Send(tmp, tmp.Length, SocketFlags.None);
        }

        public void Ecoute()
        {
            
            while (EstConnecte)
            {
                if (s.Available > 0)
                {
                    string msg = "";
                    byte[] tmp = new byte[4096];
                    int size = s.Receive(tmp, 4096, SocketFlags.None);
                    msg += Encoding.Default.GetString(tmp, 0, size);
                    if (msg[0] == '&' && msg[1] == '&')
                    {
                       string[] commande = msg.Split(new char[] {' '});
                        switch (commande[1])
                        {
                            case "Nom":
                                _ListeModified = true;
                                Nom = commande[2];
                                if (ListeClient.Count >= 0 && _ListeModified == true)
                                {
                                    int indexY = 6;
                                    for (int i = 6; i < 15; i++)
                                    {
                                        Console.SetCursorPosition(10, indexY);
                                        Console.Write("                  ");
                                        indexY++;
                                    }
                                    indexY = 6;
                                    foreach (Client item in ListeClient)
                                    {
                                        Console.SetCursorPosition(10, indexY);
                                        Console.WriteLine(item.Nom);
                                        indexY++;
                                    }
                                    _ListeModified = false;
                                }
                                break;
                            case "Quit":
                                EstConnecte = false;
                                lock (ListeClient)
                                {
                                    foreach (Client item in ListeClient)
                                    {
                                        if (s.RemoteEndPoint != item.s.RemoteEndPoint)
                                        {
                                            item.Envoyer(Nom + " s'est déconnecté");
                                        }
                                        if (s.RemoteEndPoint == item.s.RemoteEndPoint)
                                        {
                                            item.Envoyer("Je me déconnecte");
                                            item.EstConnecte = false;
                                        }
                                    }
                                    _ListeModified = true;
                                    ListeClient.Remove(this);
                                    if (ListeClient.Count >= 0 && _ListeModified == true)
                                    {
                                        int indexY = 6;
                                        for (int i = 6; i < 15; i++)
                                        {
                                            Console.SetCursorPosition(10, indexY);
                                            Console.Write("                  ");
                                            indexY++;
                                        }
                                        indexY = 6;
                                        lock (ListeClient)
                                        {
                                            // actualise la liste des connecté coté SERVEUR si quelqu'un se déco
                                            foreach (Client item in ListeClient)
                                            {
                                                Console.SetCursorPosition(10, indexY);
                                                Console.WriteLine(item.Nom);
                                                indexY++;
                                                foreach (Client item2 in ListeClient)
                                                {
                                                    item.Envoyer(item2.Nom);
                                                }
                                            }
                                        }
                                        _ListeModified = false;
                                    }
                                    break;
                                }
                            default:
                                Console.WriteLine("Commande non trouvé");
                                break;
                        }
                    }
                    if (msg == "STOP")
                    {
                        EstConnecte = false;
                    }
                    foreach (Client item in ListeClient)
                    {
                        // envoie du msg à tout le monde
                        item.Envoyer((DateTime.Now.ToString("[HH:mm:ss] ") +Nom + " : " +msg));
                        Thread.Sleep(10);
                    }

                    //sauvegarde dans un fichier log
                    StreamWriter stm = new StreamWriter("log.txt", true);
                    lock (stm)
                    {
                        stm.WriteLine(Nom + " : " + msg + " à " + (DateTime.Now.ToString("[HH:mm:ss]")));
                    }
                    stm.Close();

                    Console.SetCursorPosition(45, indexYY);
                    Console.WriteLine(Nom + " : " + msg+ " à "+(DateTime.Now.ToString("[HH:mm:ss]                ")));
                    indexYY++;
                    if (indexYY == 19)
                    {
                        Console.SetCursorPosition(45, 5);
                        Console.WriteLine(Nom + " : " + msg + " à " + (DateTime.Now.ToString("[HH:mm:ss]                ")));
                        for (int i = 6; i < 19; i++)
                        {
                            Console.SetCursorPosition(45, i);
                            Console.WriteLine("                                                             ");
                        }
                        indexYY = 6;
                    }
                }
                
            }
             Envoyer("STOP");
            Thread.Sleep(100);
        }
    }
}
