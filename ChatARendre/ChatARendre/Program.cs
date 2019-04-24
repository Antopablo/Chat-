using System;
using System.Collections.Generic;
using System.Threading;

namespace ChatARendre_Serveur
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SERVEUR");
            Console.SetWindowSize(120, 30);
            Interface();
            Serveur serveur = new Serveur();
            Thread th = new Thread(serveur.Run);
            // le thread start tant que EstConnecté = true;
            th.Start();
            th.Join();

            Console.WriteLine("terminé");
        }

        static void Interface()
        {
            string inter = @"
        __________________________________________________________________________________________________
        |   Liste des connectés        |                      Historique de conversation                  |
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
        |______________________________|                                                                  |
        |                              |                                                                  |
        |                              |                                                                  |
        | (Q) Quitter le serveur       |                                                                  |
        |______________________________|__________________________________________________________________|
";

            Console.WriteLine(inter);
        }
    }
}
