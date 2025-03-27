using System.Net.Sockets;
using System.Text;

namespace VirtualOperatorServer.Services
{
    public class BackSocket : IDisposable
    {
        private readonly string _server;
        private readonly int _port;
        private TcpClient? _client;
        private NetworkStream? _stream;
        private readonly object _lock = new(); // Thread safety

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

        private void EnsureConnected()
        {
            if (_client != null && _client.Connected)
                return; // Already connected

            Console.WriteLine("🔌 Establishing socket connection...");

            try
            {
                _client?.Close(); // Close previous connection if exists
                _client = new TcpClient();
                _client.Connect(_server, _port);
                _stream = _client.GetStream();

                Console.WriteLine("✅ Connected to socket server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection failed: {ex.Message}");
                _client = null; // Mark as disconnected
            }
        }

        public byte[] SendCommand(byte[] command)
        {
            lock (_lock)
            {
                EnsureConnected(); // ✅ Only connect if needed

                if (_client == null || _stream == null)
                {
                    Console.WriteLine("Error: Socket not connected.");
                    return [];
                }
                
                try
                {
                    // put the command to a packet
                    byte[] packet = new byte[command.Length + 2];
                    packet[0] = 0xCC;
                    packet[1] = (byte)command.Length;
                    Array.Copy(command, 0, packet, 2, command.Length);

                    _stream.Write(packet, 0, packet.Length);

                    byte[] cache = new byte[1024];
                    byte[] tmp = new byte[8];
                    byte expectedLength = 0;
                    byte replyLength = 0;
                    ReplyState state = ReplyState.TAG;
                    bool bError = false;
                    while(!bError)
                    {
                        int bytesRead = _stream.Read(tmp, 0, 1);
                        if(bytesRead == 0)
                        {
                            Console.WriteLine("Peer socket is closed while reading, closing socket");
                            _stream?.Close();
                            _client?.Close();
                            _client = null;
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

                    lock (_lock)
                    {
                        _client?.Close();
                        _client = null; // Mark connection as closed
                    }
                }
            }

            return [];
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _stream?.Close();
                _client?.Close();
                _client = null;
            }
        }
    }
}
