using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Discord_Bot_Tut
{
    class Program
    {
        private DiscordSocketClient client;
        private string botToken;
        private string log;
        static void Main(string[] args)
        {
            main:
            Console.Write("What should be done?\n 1.Start bot\n 2.Reset bot settings(if bot crashed)\n 3.Log clear\n ---->");
            var value = Console.ReadLine();

            switch (value)
            {
                case "1":
                    {
                        Console.WriteLine("Bot starting...");
                        Console.Clear();
                        new Program().MainAsync().GetAwaiter().GetResult();
                        break;
                    }

                case "2":
                    {
                        Console.WriteLine("Bot settings restore...");
                        if (File.Exists("Token.txt"))
                        {
                            File.Delete("Token.txt");
                        }
                        Console.Clear();
                        goto main;
                    }

                case "3":
                    {
                        Console.WriteLine("Log clearing...");

                        using (StreamWriter sw = new StreamWriter("Log.txt"))
                        {
                            sw.WriteLine("---log---");
                        }
                        Console.Clear();
                        goto main;
                    }

                default:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You entered a wrong value.");
                        Console.ResetColor();

                        Thread.Sleep(500);
                        Console.Clear();
                        goto main;
                    }

            }
        }

        private async Task MainAsync()
        {
            if (!File.Exists("Token.txt"))
            {
                tokenWriter();
            }
            tokenReader();

            client = new DiscordSocketClient();
            client.MessageReceived += CommandsHandler;
            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, botToken);//Логиним бота, получаем значение токена.

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Attention, if there were no errors, the token was loaded successfully! If not, restore bot setting in menu.");
            Console.ResetColor();

            await client.StartAsync();

            Console.WriteLine("---Log---");
            Console.ReadLine();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandsHandler(SocketMessage msg)
        {
            var nowLog = $"{DateTime.Now}:{msg.Author} - {msg}";
            Console.WriteLine(nowLog);

            using (StreamReader sr = new StreamReader("Log.txt"))
            {
                log = sr.ReadToEnd(); //читаем всё из указанного файла.
            }

            using (StreamWriter sw = new StreamWriter("Log.txt"))
            {
                sw.WriteLine(log + nowLog);
            }

            if (!msg.Author.IsBot)
                switch (msg.Content)
                {
                    case "!инфо":
                        {
                            string squad = msg.Author.PublicFlags.ToString();
                            if (squad.Length > 1)
                            {
                                squad = squad.Replace("d", "d ");
                                msg.Channel.SendMessageAsync($"Привет, {msg.Author.Mention} из {squad} ");
                            }
                            else
                            {
                                msg.Channel.SendMessageAsync($"Привет, {msg.Author.Mention}");
                            }
                            break;
                        }
                    case "!рандом":
                        {
                            Random rnd = new Random();
                            msg.Channel.SendMessageAsync($"Выпало число {rnd.Next(0, 10)}");
                            break;
                        }
                }
            return Task.CompletedTask;
        }

        private void tokenWriter()
        {
            using (StreamWriter sw = new StreamWriter("Token.txt"))
            {
                Console.Write("Write your token here ----->");
                var token = Console.ReadLine();
                sw.WriteLine(token);
            }
        }
        private void tokenReader()
        {
            using (StreamReader sr = new StreamReader("Token.txt"))
            {
                var tokenS = sr.ReadToEnd(); //читаем всё из указанного файла.
                botToken = tokenS;
            }
        }
    }
}