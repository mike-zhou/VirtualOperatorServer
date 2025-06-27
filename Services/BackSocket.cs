using System.Net.Sockets;
using System.Text;

namespace VirtualOperatorServer.Services
{
    /// <summary>
    /// BackSocket class sends a command to VirtualOperatorServer through a socket,
    /// and returns the reply from peer.
    /// 
    /// The command is a byte[]. Inside the class, the command is put into a 
    /// command packet, then is sent out through the socket. A reply of byte[] is 
    /// expected.
    /// 
    /// The length of command cannot exceed 250 bytes.
    /// </summary>
    public class BackSocket : IDisposable
    {
        private readonly string _server;
        private readonly int _port;
        private TcpClient? _client;
        private NetworkStream? _stream;
        private static readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        private const int MAX_COMMAND_LENGTH = 250;

        private enum ReplyState
        {
            TAG = 0,
            LENGTH,
            VALUE
        }

        public BackSocket(IConfiguration configuration)
        {
            _server = configuration["SocketSettings:Server"] ?? "127.0.0.1";
            _port = int.Parse(configuration["SocketSettings:Port"] ?? "9527");
        }

        private async Task EnsureConnected()
        {
            if (_client != null && _client.Connected)
                return; // Already connected

            Console.WriteLine("🔌 Establishing socket connection...");

            try
            {
                _client?.Close(); // Close previous connection if exists
                _client = new TcpClient();
                await _client.ConnectAsync(_server, _port);
                _stream = _client.GetStream();

                Console.WriteLine("✅ Connected to socket server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection failed: {ex.Message}");
                _client = null; // Mark as disconnected
            }
        }

        public async Task<byte[]> SendAndReceiveAsync(byte[] command)
        {
            if(command.Length > MAX_COMMAND_LENGTH)
            {
                Console.WriteLine($"Error: BackSocket::SendCommand: command is too long ({command.Length})");
                return [];
            }

            await _mutex.WaitAsync();
            try
            {
                await EnsureConnected(); // ✅ Only connect if needed
                if (_client == null || _stream == null)
                {
                    Console.WriteLine("Error: Socket not connected.");
                    return [];
                }
                
                // put the command to a packet
                byte[] packet = new byte[command.Length + 2];
                packet[0] = 0xCC;
                packet[1] = (byte)command.Length;
                Array.Copy(command, 0, packet, 2, command.Length);

                await _stream.WriteAsync(packet, 0, packet.Length);

                byte[] cache = new byte[1024];
                byte[] tmp = new byte[8];
                byte expectedLength = 0;
                byte replyLength = 0;
                ReplyState state = ReplyState.TAG;
                bool bError = false;
                while(!bError)
                {
                    int bytesRead = await _stream.ReadAsync(tmp, 0, 1);
                    if(bytesRead == 0)
                    {
                        Console.WriteLine("Peer socket is closed while reading, closing socket");
                        Dispose();
                        break;
                    }
                    switch(state)
                    {
                        case ReplyState.TAG:
                            if(tmp[0] == 0xDD)
                            {
                                cache[0]= 0xDD;
                                state = ReplyState.LENGTH;
                            }
                            break;
                        
                        case ReplyState.LENGTH:
                            expectedLength = tmp[0];
                            cache[1] = expectedLength;
                            if(expectedLength == 0)
                            {
                                state = ReplyState.TAG;
                            }
                            else
                            {
                                replyLength = 0;
                                state = ReplyState.VALUE;
                            }
                            break;

                        case ReplyState.VALUE:
                            cache[2 + replyLength] = tmp[0];
                            replyLength++;
                            if(replyLength == expectedLength)
                            {
                                byte[] reply = new byte[replyLength];
                                Array.Copy(cache, 2, reply, 0, replyLength);
                                
                                return reply;
                            }
                            break;

                        default:
                            bError = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error sending command: {ex.Message}");
                Dispose(); // disconnect socket connection
            }
            finally
            {
                _mutex.Release();
            }

            return [];
        }

        public void Dispose()
        {
            _stream?.Close();
            _client?.Close();
            _client = null;
        }
    }
}
