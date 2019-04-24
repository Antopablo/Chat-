using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatARendre_Client
{
    class Program
    {
       static bool EstConnecte = true;
       public static List<string> ListeClientCo = new List<string>();
       public static bool ListeClientCoBool = true;

        static void Main(string[] args)
        {
            Console.SetWindowSize(120, 30);
            Interface();
            TcpClient tcpClnt = new TcpClient();
            IPEndPoint EndPointServer = new IPEndPoint(IPAddress.Loopback, 5035);
            tcpClnt.Connect(EndPointServer);
            NetworkStream stm = tcpClnt.GetStream();
            Thread th = new Thread(ClientListener);
            th.Start(tcpClnt);
            while (EstConnecte)
            {
                if (Console.KeyAvailable)
                {
                    //Console.WriteLine("je rentre dans le if");

                    //J'écris un msg et je l'envoie au serveur
                    Console.SetCursorPosition(43, 16);
                    string saisie = Console.ReadLine();
                    byte[] tabSaisie = Encoding.Default.GetBytes(saisie);
                    stm.Write(tabSaisie, 0, tabSaisie.Length);
                    Thread.Sleep(30);
                    Console.SetCursorPosition(43, 16);
                    for (int i = 0; i < 60; i++)
                    {
                        Console.Write(" ");
                    }
                }
            }
            th.Join();
            Console.WriteLine("Je suis déco");
        }

        static void ClientListener(object obj)
        {
            TcpClient tcpclnt = (TcpClient)obj;
            NetworkStream strm = tcpclnt.GetStream();
            //affichage dans la console du Client
            Console.SetCursorPosition(45, 5);
            Console.WriteLine("Bienvenue");
            int IndexY = 6;
            int IndexYY = 6;

            while (EstConnecte)
            {
                byte[] rep = new byte[4096];
                string reponse = "";
                if (strm.DataAvailable)
                {
                    // je reçois un msg et je l'affiche
                    int size = strm.Read(rep, 0, 4096);
                    reponse += Encoding.Default.GetString(rep, 0, size);
                    if (reponse == "STOP")
                    {
                        EstConnecte = false;
                    } else if (reponse[0] == '&' && reponse[1] == '&')
                    {
                        // écriture dans la liste des connectés dans l'interface CLIENT
                        string[] spli = reponse.Split(new char[] { '*' });
                        ListeClientCo.Add(spli[1]);
                        ListeClientCoBool = true;
                    }
                    else
                    {
                        Console.SetCursorPosition(45, IndexY);
                        Console.WriteLine(reponse);
                        IndexY++;
                    }
                    
                    if (IndexY == 14)
                    {
                        Console.SetCursorPosition(45, 6);
                        Console.WriteLine(reponse);
                        for (int i = 7; i < 14; i++)
                        {
                            Console.SetCursorPosition(45, i);
                            Console.WriteLine("                                                             ");
                        }
                        IndexY = 6;
                    }
                    if (ListeClientCoBool == true)
                    {
                        for (int i = 5; i < 14; i++)
                        {
                            Console.SetCursorPosition(10, i);
                            Console.WriteLine("                   ");
                        }
                        foreach (string item in ListeClientCo)
                        {
                            Console.SetCursorPosition(10, IndexYY);
                            Console.WriteLine(item);
                            IndexYY++;
                        }
                        ListeClientCoBool = false;
                    }

                }
                Thread.Sleep(1);
                
            }
            Console.SetCursorPosition(20, 22);
            Console.WriteLine("Je suis déconnecté");
            Thread.Sleep(50);
            Environment.Exit(0);
        }
        static void Interface()
        {
            string inter = @"
        __________________________________________________________________________________________________
        |   Liste des connectés        |                     Conversation                                 |
        |        max 10                |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        |______________________________|__________________________________________________________________|
        |                              |                                                                  |
        | && Nom : changer pseudo      |                                                                  |
        | && Quit : se déconnecter     |                                                                  |
        |______________________________|__________________________________________________________________|
";

            Console.WriteLine(inter);
        }
    }
}
