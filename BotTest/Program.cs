using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Discord_Bot_Tut
{
    class Program
    {
        DiscordSocketClient client;
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.MessageReceived += CommandsHandler;
            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, Token());//Логиним бота, получаем значение токена.
            await client.StartAsync();

            Console.ReadLine();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandsHandler(SocketMessage msg)
        {
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

        private string Token()
        {
            try
            {
                StreamReader sr = new StreamReader("Token.txt"); //обьявляем и указываем путь до файла.
                var token = sr.ReadToEnd(); //читаем всё из указанного файла.
                return token; //возвращаем значение токена.
            }
            catch
            {
                TokenE();
                return null;
            }
        }

        private void TokenE()//говнокод, надо пофиксить и дать возможноть записать файл без закрытия.
        {
            Console.WriteLine("Введите путь до файла в Token.txt");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}