using System;
using Microsoft.Extensions.Options;
namespace XPTracker
{
    public class SecretsRevealer: ISecretRevealer
    {
        private readonly Settings _secrets;
        // I've injected <em>secrets</em> into the constructor as setup in Program.cs
        public SecretsRevealer(IOptions<Settings> secrets)
        {
        // We want to know if secrets is null so we throw an exception if it is
        _secrets = secrets.Value ?? throw new ArgumentNullException(nameof(secrets));
        }

        public string GetToken()
        {
             Console.WriteLine($"Token: {_secrets.DiscordToken}");
             return _secrets.DiscordToken;
        }
  }

    public interface ISecretRevealer
    {
        string GetToken();
    }
}