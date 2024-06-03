using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace CurrencyServerApp
{
    public partial class ServerForm : Form
    {
        private TcpListener? server;
        private string logFile = "server_log.txt";
        private Dictionary<string, double> exchangeRates = new Dictionary<string, double>
        {
            { "USD_EURO", 0.85 },
            { "EURO_USD", 1.18 }
            // Додайте інші курси валют за потреби
        };

        public bool InvokeRequired { get; private set; }

        public ServerForm()
        {
            InitializeComponent();
            Task.Run(() => StartServer());
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private async void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 8888);
            server.Start();
            AppendText("Server started...");
            Log("Server started...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Task.Run(() => HandleClient(client));
            }
        }

        private async void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            string clientInfo = $"{endPoint.Address}:{endPoint.Port}";
            AppendText($"Client connected: {clientInfo}");
            Log($"Client connected: {clientInfo}");

            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                AppendText($"Received: {request}");
                Log($"Received from {clientInfo}: {request}");

                string response = GetExchangeRate(request.Trim());
                byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);

                AppendText($"Sent: {response}");
                Log($"Sent to {clientInfo}: {response}");

                if (request.Trim().Equals("Bye", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }

            stream.Close();
            client.Close();
            AppendText($"Client disconnected: {clientInfo}");
            Log($"Client disconnected: {clientInfo}");
        }

        private string GetExchangeRate(string request)
        {
            if (exchangeRates.ContainsKey(request))
            {
                return exchangeRates[request].ToString();
            }
            return "Unknown currency pair";
        }

        private void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendText), new object[] { text });
            }
            else
            {
                textBox1.AppendText(text + Environment.NewLine);
            }
        }

        private void Invoke(Action<string> action, object[] objects)
        {
            throw new NotImplementedException();
        }

        private void Log(string message)
        {
            File.AppendAllText(logFile, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }

    internal class textBox1
    {
        internal static void AppendText(string v)
        {
            throw new NotImplementedException();
        }
    }

    public class Form
    {
    }
}

