using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ChatARendre_Serveur
{
    class Serveur
    {
        TcpListener tcpServeur;
        List<Thread> ListeThread;
        List<Client> ListeClient;
        public List<Client> ListeClientAEnvoyer = new List<Client>();
        private bool _EstConnecte;
        private bool _ListeModified;
        private bool ListeClientAEnvoyerBool;

        public Serveur()
        {
            this.tcpServeur = new TcpListener(IPAddress.Any, 5035); ;
            ListeThread = new List<Thread>(); 
            ListeClient = new List<Client>(); 
            _EstConnecte = true;
            _ListeModified = false;
            ListeClientAEnvoyerBool = false;
        }

        public void Run()
        {
            tcpServeur.Start();
            while (_EstConnecte)
            {
                 if (this.tcpServeur.Pending())
                 {
                    Socket s = tcpServeur.AcceptSocket();
                    Client clnt = new Client(s, ListeClient, "Anonymous");
                    Thread th = new Thread(clnt.Ecoute);
                    lock (clnt)
                    {
                        _ListeModified = true;
                        ListeClient.Add(clnt);
                        ListeClientAEnvoyer.Add(clnt);
                        ListeClientAEnvoyerBool = true;
                    }
                    ListeThread.Add(th);
                    th.Start();
                }
                Thread.Sleep(1);

                if (ListeClient.Count > 0 && _ListeModified == true)
                {
                    int indexY = 6;
                    for (int i = 6; i < 15; i++)
                    {
                        Console.SetCursorPosition(10, indexY);
                        Console.Write("                  ");
                        indexY++;
                    }
                    indexY = 6;

                    // écriture des connectés dans l'interface SERVEUR
                    foreach (Client item in ListeClient)
                    {
                        Console.SetCursorPosition(10, indexY);
                        Console.WriteLine(item.Nom);
                        indexY++;
                    }
                    _ListeModified = false;
                }

                // envoie des Client connectés 
                if (ListeClientAEnvoyerBool == true)
                {
                    foreach (Client item in ListeClientAEnvoyer)
                    {
                        item.Envoyer("&&*" + item.Nom);
                    }
                    ListeClientAEnvoyerBool = false;
                }

                if (Console.KeyAvailable)
                {
                    var input = Console.ReadKey(true);
                    switch (input.Key)
                    {
                        case ConsoleKey.Q:
                            Console.SetCursorPosition(5, 20);
                            Console.WriteLine("Je déconnecte le serveur");
                            foreach (Client item in ListeClient)
                            {
                                item.EstConnecte = false;
                            }
                            _EstConnecte = false;
                            break;
                        default:
                            Console.WriteLine("Commande non reconnue");
                            break;
                    }
                }

                


            }
            Console.WriteLine("Je quitte");
        }
    }
}
