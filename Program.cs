using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XPTracker.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace XPTracker
{
  public class Program
  {
    // static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

// Old Main Before Commands uncomment to seed data
    public static async Task Main(string[] args)
    {
      var host = CreateWebHostBuilder(args).Build();
      using (var scope = host.Services.CreateScope())
      {
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        Console.WriteLine("Starting to migrate database....");
        await context.Database.MigrateAsync();
        Console.WriteLine("Database is up to date, #party time");
        Program BotLife = new Program();
        BotLife.RunBotAsync().GetAwaiter().GetResult();
      }

      await host.RunAsync();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public static IConfigurationRoot Configuration{get;set;}
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
                          devEnvironmentVariable.ToLower() == "development";
            Console.WriteLine($"{isDevelopment}");
            var builder = new ConfigurationBuilder();
            //only add secrets in development
            if (isDevelopment)
            {
                builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();

            _services = new ServiceCollection()
               .Configure<Settings>(Configuration.GetSection(nameof(Settings)))
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<ISecretRevealer, SecretsRevealer>()
                .AddDbContext<DatabaseContext>()
                .BuildServiceProvider();

            // string token = this.DiscordToken;
            var revealer = _services.GetService<ISecretRevealer>();
            
            string token = revealer.GetToken();

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);

        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }
  }
}
