using CurrencyServerApp;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace CurrencyClientApp
{
    public partial class ClientForm : Form
    {
        private TcpClient? client;
        private NetworkStream? stream;

        public bool InvokeRequired { get; private set; }

        public ClientForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            string ipAddress = textBoxIpAddress.Text;
            int port = int.Parse(textBoxPort.GetText());

            client = new TcpClient();
            await client.ConnectAsync(ipAddress, port);
            stream = client.GetStream();
            AppendText("Connected to server...");
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            if (client == null || stream == null) return;

            string request = textBoxMessage.Text;
            byte[] requestBytes = Encoding.ASCII.GetBytes(request);
            await stream.WriteAsync(requestBytes, 0, requestBytes.Length);
            AppendText("Sent: " + request);

            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            AppendText("Received: " + response);
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
    }

    internal class textBoxMessage
    {
        public static string Text { get; internal set; }
    }

    internal class textBoxPort
    {
        private static ReadOnlySpan<byte> text;

        public static ReadOnlySpan<byte> Text { get => text; set => text = value; }

        public static ReadOnlySpan<byte> GetText()
        {
            return text;
        }

        internal static void SetText(ReadOnlySpan<byte> value)
        {
            text = value;
        }
    }

    internal class textBoxIpAddress
    {
        public static string Text { get; internal set; }
    }
}

