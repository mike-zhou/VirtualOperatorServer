using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualOperatorServer.CommandAndReply;

namespace VirtualOperatorServer.Services
{
    public class BackService : BackgroundService
    {
        private readonly ILogger<BackService> _logger;
        private BackSocket _backSocket;

        public BackService(ILogger<BackService> logger, BackSocket socket)
        {
            _logger = logger;
            _backSocket = socket;
        }

        public static bool Connected { get; private set; } = false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!Connected)
                    {
                        Connected = await FetchStaticStatusAsync();
                    }

                    var success = await FetchDynamicStatusAsync();
                    if (!success)
                    {
                        // static status need to be refreshed after a broken socket connection.
                        Connected = false;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in BackService.");
                }
            }

            _logger.LogInformation("BackService stopped.");
        }

        public async Task<bool> RunCommandAsync(CommandAndReply.CommandAndReply cmd)
        {
            bool success;
            cmd.Reply = await _backSocket.SendAndReceiveAsync(cmd.Command);
            (success, _) = cmd.ParseReply();

            if (!success)
            {
                Connected = false;
            }
            
            return success;
        }

        private async Task<bool> FetchStaticStatusAsync()
        {
            CmdGetVersion version = new();
            if(!await RunCommandAsync(version))
            {
                return false;
            }

            CmdGetGPIOMode mode = new();
            if(!await RunCommandAsync(mode))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> FetchDynamicStatusAsync()
        {
            CmdGetStatus cmd = new();
            if(!await RunCommandAsync(cmd))
            {
                return false;
            }

            return true;
        }
    }
}

