using ApplicationForTransfer_Lib;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace ApplicationForTransfer_GUI
{
    public partial class MainForm : Form
    {
        private string _fileToTransfer = "";
        private bool _tempZip = false;
        private Broadcaster _broadcaster;
        public MainForm()
        {
            InitializeComponent();
            InitBroadcaster();
        }

        private void InitBroadcaster()
        {
            _broadcaster = new Broadcaster();
            _broadcaster.SayHello();
            _broadcaster.Listen();
            _broadcaster.MessageReceived += Broadcaster_MessageReceived;

        }

        private void AddToClientList(IPEndPoint client) 
        {
            if (!lstNodes.Items.Contains(client)) 
            {
                lstNodes.Items.Add(client);
            }
        }


        private void Broadcaster_MessageReceived(object sender, BroadcastPayload e) 
        {
            var broadcaster = sender as Broadcaster;
            switch (e.Message)
            {
                case BroadcastMessage.Hello:
                    broadcaster.Asknowledge(e.Client);
                    CheckAndAdd(e.Client);
                    break;

                case BroadcastMessage.HelloAsknowledge:
                    // Добавляем клиента в список
                    CheckAndAdd(e.Client);
                    break;
                case BroadcastMessage.Confirm:
                    var receiver = new ReceiveFile(54000);
                    receiver.TransferComplete += FileReceived_Complete;
                    receiver.Listen();
                    break;
                case BroadcastMessage.SendRequest:
                    if (MessageBox.Show($"Receive {e.Filename} from {e.Hostname}?", "Receive File?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        broadcaster.SendFileAcknowledgement(e.Client, e.Filename);
                    }
                    break;
                case BroadcastMessage.SendAcknowledge:
                    _broadcaster.InitiatingTransfer(e.Client);
                    var transfer = new TransferFille(_fileToTransfer, e.Client.Address.ToString());
                    transfer.TransferComplete += Transfer_Complete;
                    transfer.Start();
                    break;
            }
        }

        private void CheckAndAdd(IPEndPoint client)
        {
            var infs = NetworkInterface.GetAllNetworkInterfaces();
            bool found = false;
            foreach (var i in infs)
            {
                var addrs = i.GetIPProperties();
                foreach (var ip in addrs.UnicastAddresses)
                {
                    if (ip.Address.Equals(client.Address))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            if (!found)
            {
                Invoke((Action)(() => AddToClientList(client)));
            }
        }

        private void mmuSendFile_Click(object sender, System.EventArgs e)
        {
            if (lstNodes.SelectedItem == null)
            {
                MessageBox.Show("Выберите елемент");
            }
            else 
            {
                var ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK) 
                {
 
                    var client = lstNodes.SelectedItem as IPEndPoint;
                    _fileToTransfer = ofd.FileNames[0];
                    _tempZip = ofd.FileNames.Length > 1;

                    if (ofd.FileNames.Length > 0) 
                    {
                        _fileToTransfer = MakeZipFile(ofd.FileNames);
                    }
                    var hostName = $"{Environment.UserName}@{Environment.MachineName}";
                    _broadcaster.SendFileRequest(client, hostName, _fileToTransfer);
                }
            }
        }

        private string MakeZipFile(string[] files) 
        {
            var tempFile = Path.GetTempFileName().Replace(".tmp", "") + ".zip";
            using (var archive = ZipFile.Open(tempFile, ZipArchiveMode.Create)) 
            {
                foreach (var f in files)
                {
                    var filename = Path.GetFileName(f);
                    archive.CreateEntryFromFile(f, filename);
                 }
            }

            return tempFile;
        }

        private void FileReceived_Complete(object sender, EventArgs e) 
        {
            var receiveFile = sender as ReceiveFile;
            receiveFile.Stop();
            MessageBox.Show("Transfer complete!");
        }

        private void Transfer_Complete(object send, EventArgs e) 
        {
            MessageBox.Show("Transfer complete!");
            if (_tempZip && _fileToTransfer.EndsWith(".zip")) 
            {
                File.Delete(_fileToTransfer);
            }
        }

    }
}
