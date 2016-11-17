using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public static class Client
    {
        public static string MyPcName { get; private set; }

        private static TcpClient client;
        private static StreamWriter writer;
        private static StreamReader reader;


        public delegate void GameStartedHandler(object sender, EventArgs e);
        public static event GameStartedHandler GameStarted;
        private static void OnGameStarted(EventArgs e)
        {
            if (GameStarted != null)
            {
                GameStarted(null, e);
            }
        }

        public delegate void GameStoppedHandler(object sender, EventArgs e);
        public static event GameStoppedHandler GameStopped;
        private static void OnGameStopped(EventArgs e)
        {
            if (GameStopped != null)
            {
                GameStopped(null, e);
            }
        }
        public delegate void PenaltyReceivedHandler(object sender, EventArgs e);
        public static event PenaltyReceivedHandler PenaltyReceived;
        private static void OnPenaltyReceived(EventArgs e)
        {
            if (PenaltyReceived != null)
            {
                PenaltyReceived(null, e);
            }
        }
        public delegate void GamePausedHandler(object sender, EventArgs e);
        public static event GamePausedHandler GamePaused;
        private static void OnGamePaused(EventArgs e)
        {
            if (GamePaused != null)
            {
                GamePaused(null, e);
            }
        }

        private static Thread receiveThread;

        public static void Connect(string YourOwnPCName, string serverIP, int serverPort = 5000)
        {

            if (!string.IsNullOrEmpty(YourOwnPCName))
            {
                if (YourOwnPCName.ToLower().StartsWith("pc") && YourOwnPCName.Length == 3)
                {
                    MyPcName = YourOwnPCName;
                }
            }
            else throw new ArgumentNullException("Vul iets in bij YourOwnPCName!");

            if (receiveThread != null)
            {
                client.Close();
                receiveThread.Abort();
                receiveThread = null;
            }
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            writer = new StreamWriter(client.GetStream());
            reader = new StreamReader(client.GetStream());

            receiveThread = new Thread(HandleIncomingMessage);
            receiveThread.Priority = ThreadPriority.Lowest;
            receiveThread.Start();

            SendMessage("connect");


        }


        private static void SendMessage(string message)
        {
            string toSend = ("#" + MyPcName + "&" + message + "%");
            writer.Write(toSend);
            writer.Flush();
        }

        public static void Punt()
        {
            SendMessage("punt");
        }
        public static void Hit()
        {
            SendMessage("hit");
        }


        private static void HandleIncomingMessage()
        {

            while (true)
            {
                try
                {


                    int incomingInt = reader.Read();
                    string targetPC = "";
                    string incomingMessage = "";

                    if (incomingInt != -1) // is er iets beschikbaar?
                    {
                        char incomingChar = Convert.ToChar(incomingInt);
                        if (incomingChar == '#') // als incoming # is begin
                        {
                            incomingInt = -1;
                            while (incomingInt == -1) // lees totdat er iets beschikbaar is
                            {
                                incomingInt = reader.Read();
                            }
                            incomingChar = Convert.ToChar(incomingInt);
                            while (incomingChar != '&') // ga door met whilen totdat incomingchar & is
                            {
                                targetPC += incomingChar; // stop alle binnenkomende tekens in een string
                                incomingInt = reader.Read();
                                incomingChar = Convert.ToChar(incomingInt);
                            }
                            if (targetPC == MyPcName) // ga door als het bericht voor deze pc bedoeld is
                            {
                                incomingInt = -1; // zet incoming op -1
                                while (incomingInt == -1) // ga door met receiven totdat er iets binnenkomt
                                {
                                    incomingInt = reader.Read();
                                }
                                incomingChar = Convert.ToChar(incomingInt);
                                while (incomingChar != '%') // ga lezen totdat het binnenkomende teken % is
                                {
                                    incomingMessage += incomingChar;
                                    incomingInt = reader.Read();
                                    incomingChar = Convert.ToChar(incomingInt);
                                }
                                if (incomingMessage == "start")
                                {
                                    incomingMessage = "";
                                    OnGameStarted(EventArgs.Empty);
                                }
                                else if (incomingMessage == "stop")
                                {
                                    incomingMessage = "";
                                    OnGameStopped(EventArgs.Empty);
                                }
                                else if (incomingMessage == "penalty")
                                {
                                    incomingMessage = "";
                                    OnPenaltyReceived(EventArgs.Empty);
                                }
                                else if (incomingMessage == "pause")
                                {
                                    incomingMessage = "";
                                    OnGamePaused(EventArgs.Empty);
                                }
                                else
                                {
                                    Console.WriteLine(incomingMessage);
                                }
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Connection closed");
                    receiveThread.Abort();
                }
            }
        }

        public static void Close()
        {
            client.Close();
            receiveThread.Abort();
        }
    }
}
