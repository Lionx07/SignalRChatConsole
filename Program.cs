using Microsoft.AspNetCore.SignalR.Client;
using SignalRChatConsole.Models;

namespace SignalRChatConsole
{
    class Program
    {
        static HubConnection connection;
        static string UserName { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Bem-vindo!");
            Console.WriteLine("Digite seu nome:");
            UserName = Console.ReadLine();

            Console.Clear();

            // Se conecta ao servidor
            Connect();

            while (true)
            {
                var message = Console.ReadLine();
                SendMessage(message);
            }
        }

        public static async void Connect()
        {
            // Abre a conexão com o servidor
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7020/chat")
                .Build();

            connection.On<string>("newUser", (user) =>
            {
                var message = user == UserName ? "Você entrou no chat" : $"{user} entrou no chat";
                Console.WriteLine(message);
            });

            connection.On<string>("userDisconnected", (user) =>
            {
                Console.WriteLine($"{user} saiu do chat");
            });

            connection.On<string, string>("newMessage", (user, message) =>
            {
                if (user != UserName)
                {
                    Console.WriteLine($"{user}: {message}");
                }
                else
                {
                    Console.WriteLine($"You: {message}");
                }
            });

            connection.On<List<Message>>("previousMessages", (messages) =>
            {
                foreach (var message in messages)
                {
                    if(message.UserName != UserName)
                        Console.WriteLine($"{message.UserName}: {message.Text}");
                    else
                        Console.WriteLine($"You: {message.Text}");
                }
            });

            //connection.On<string>("typeMessage", (user) =>
            //{
            //    Console.WriteLine($"{user} está digitando...");
            //});

            try
            {
                await connection.StartAsync();
                await connection.SendAsync("newUser", UserName, connection.ConnectionId);
            }
            catch
            {
                Console.WriteLine("Não foi possível conectar ao servidor");
            }
        }
    
        public static async void SendMessage(string message)
        {
            //await connection.SendAsync("typeMessage", UserName);
            await connection.SendAsync("newMessage", UserName, message);
        }
    }
}
