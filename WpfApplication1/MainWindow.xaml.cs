using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using CurlSharp;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        private string name {get; set;}
        private string pass {get; set;}

        private CurlEasy sender_easy;

        BackgroundWorker main_worker = new BackgroundWorker();        
        public MainWindow(string n, string p)
        {
            InitializeComponent();
            name = n;
            pass = p;
            chat data = new chat { col="#00CC66", Conmes = " Session Started[ " + DateTime.Now + " ] Loading..." };
            chatbox.Items.Add(data);
            File.AppendAllText("log/chat.txt", " ------------------ Session Started : " + DateTime.Now + "" + Environment.NewLine);
            main_worker.WorkerReportsProgress = true;
            main_worker.DoWork += chat_receiver;
            main_worker.ProgressChanged += OnChatReceive;
            main_worker.RunWorkerCompleted += null;
            main_worker.RunWorkerAsync();
        }         
        private void sender_init()
        {
            string postData = "form_submitted=1&form_username=" + name + "&form_password=" + pass + "&form_autologin=1";
            var postLength = postData.Length;            
            sender_easy.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ubuntu Chromium/32.0.1700.107 Chrome/32.0.1700.107 Safari/537.36";
            sender_easy.FollowLocation = true;
            sender_easy.Url = "https://thepilotslife.com/?";
            string fileName = "cookie2.txt";
            FileInfo f = new FileInfo(fileName);
            sender_easy.CookieJar = f.FullName;
            sender_easy.CookieFile = f.FullName;
            sender_easy.SslVerifyPeer = false;
            sender_easy.CookieSession = true;
            sender_easy.Post = true;
            sender_easy.PostFields = postData;
            sender_easy.PostFieldSize = postLength;
            sender_easy.Perform();
            sender_easy.WriteFunction = CommentSent;
            sender_easy.Post = false;
            sender_easy.WriteData = "a";
            sender_easy.Url = "https://thepilotslife.com/chat";
            sender_easy.Perform();            
            sender_easy.WriteData = "b";
        }        
        void chat_receiver(object sender, DoWorkEventArgs e)
        {
            Curl.GlobalInit(CurlInitFlag.All);
            using (var easy = new CurlEasy())
            {
                
                //sender session init
                sender_easy = new CurlEasy();
                sender_init();
                //sender session init END
                //receiver session init 
                easy.WriteData = null;
                easy.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ubuntu Chromium/32.0.1700.107 Chrome/32.0.1700.107 Safari/537.36";
                easy.FollowLocation = true;               
                easy.Url = "https://thepilotslife.com/chat";
                string fileName = "cookie.txt";
                FileInfo f = new FileInfo(fileName);
                easy.CookieJar = f.FullName;
                easy.CookieFile = f.FullName;
                easy.SslVerifyPeer = false;
                easy.CookieSession = true;
                easy.Perform();
                easy.WriteFunction += OnWriteData;
                easy.Url = "https://thepilotslife.com/assets/chat-output.php";
                short i = (short)((((App)Application.Current).fetchsec * 10) / 3);
                while (true)
                {
                    try
                    {
                        if (i >= (short)((((App)Application.Current).fetchsec * 10) / 3))
                        {
                            Query sQuery = new Query("167.114.145.46", 7777);
                            sQuery.Send('c');
                            int count = sQuery.Receive();
                            string[] info = sQuery.Store(count);
                            main_worker.ReportProgress(96, info);
                            i = 0;
                        }
                    }
                    catch
                    {
                        break;
                    }
                    easy.Perform();
                    i++;
                    System.Threading.Thread.Sleep(300);
                }
            }
            Curl.GlobalCleanup();
        }
        private void OnChatReceive(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == 21)
            {
                sender_box.IsEnabled = true;
                placeholder.Text = "Type here...";
                return;
            }
            string[] str = (string[])e.UserState;
            if (e.ProgressPercentage == 69)
            {
                chat data = new chat();
                if (str[1] == " has disconnected from the server." || str[1] == " has connected to the server.")
                {
                    data = new chat { col = "#00CC66", Conmes = str[0] + str[1] };                   
                }
                else
                {
                    bool flg = false;
                    foreach (var x in ((App)Application.Current).nicks)
                    {
                        if(x.activated)
                            if (str[1].IndexOf(x.name, StringComparison.OrdinalIgnoreCase) >= 0) {
                                flg = true;                  
                                data = new chat { User = str[0], Conmes = str[1], col = x.color };
                                System.Media.SystemSounds.Beep.Play();
                                break;
                            }
                    }
                    if(!flg)data = new chat { User = str[0], Line = str[1] };
                }
                chatbox.Items.Add(data);
                if(((App)Application.Current).chatlogging)File.AppendAllText("log/chat.txt", "[" + DateTime.Now + "]  " + str[0] + " " + str[1] + Environment.NewLine);
            }
            else
            {
                playerbox.Items.Clear();
                for (int j = 0, l = str.Length; j < l; j += 2)
                {
                    playerbox.Items.Add(new players { Online = str[j], Score = str[j+1] });
                }
            }
        }
        private Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            string str = Encoding.UTF8.GetString(buf);           
           // MessageBox.Show(str);
            if(str.Length > 3)
            {
                string[] strar = str.Split('\n');
                for (int i = strar.Length-1; i >= 0; i-- )
                {
                    if (strar[i].Length < 3) continue;
                    Match rname = Regex.Match(strar[i], @"><a>([^)]*)</a>");
                    if (!rname.Success) continue;
                    Match rmsg = Regex.Match(strar[i], @"</a>([^)]*)<br>");
                    if (!rmsg.Success) continue;
                    main_worker.ReportProgress(69, new string[] { rname.Groups[1].Value, rmsg.Groups[1].Value });
                }

            }
                        
            return size * nmemb;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //draggin window movement
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            Keyboard.ClearFocus();
        }
        private void clo_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            settings_window win = new settings_window();
            win.ShowDialog();
        }
        private void mini_Click(object sender, RoutedEventArgs e)
        {
           WindowState = WindowState.Minimized;                             
        }

        private void control_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
           
            if (e.ExtentHeight < e.ViewportHeight)
                return;            
            if (chatbox.Items.Count <= 0)
                return;
            if (e.ExtentHeightChange == 0.0 && e.ViewportHeightChange == 0.0)
                return;
            var oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;
            var oldVerticalOffset = e.VerticalOffset - e.VerticalChange;
            var oldViewportHeight = e.ViewportHeight - e.ViewportHeightChange;
            if (oldVerticalOffset + oldViewportHeight + 5 >= oldExtentHeight)
                chatbox.ScrollIntoView(chatbox.Items[chatbox.Items.Count - 1]);
        }
        private void sender_box_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender_box.Text == "") return;
                sender_box.IsEnabled = false;
                sender_easy.Url = "https://thepilotslife.com/assets/chat-send.php?comment=" + sender_box.Text.Replace(' ', '+');
                sender_easy.Perform();              
            }
        }
        private Int32 CommentSent(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            if(extraData.ToString() == "a")
            {
                string da = Encoding.UTF8.GetString(buf);
                if (da.IndexOf("<title>Pilots Life - Online Chat</title>") != -1 && da.IndexOf("Logged in as") != -1)
                {
                    main_worker.ReportProgress(21);
                }
                else
                {
                    System.Windows.MessageBox.Show("Something went wrong while initializing chat sending session, retrying..\nIf this message box appear continue to appear after clicking 'OK' button then restart the application.");
                    sender_init();
                }
                return size * nmemb;
            }
            sender_box.IsEnabled = true;
            sender_box.Text = "";
            return size * nmemb;
        }        
    }    
}
public class players
{
    public string Online { get; set; }
    public string Score { get; set; }   
}
public class chat
{
    public string User { get; set; }
    public string Line { get; set; }
    public string Conmes { get; set; }
    public string col { get; set; }
}

class Query
{
    Socket qSocket;
    IPAddress address;
    int _port = 0;

    string[] results;
    int _count = 0;

    DateTime[] timestamp = new DateTime[2];

    public Query(string IP, int port)
    {
        qSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        qSocket.SendTimeout = 5000;
        qSocket.ReceiveTimeout = 5000;

        try
        {
            address = Dns.GetHostAddresses(IP)[0];
        }

        catch
        {

        }

        _port = port;
    }

    public bool Send(char opcode)
    {
        try
        {
            EndPoint endpoint = new IPEndPoint(address, _port);

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write("SAMP".ToCharArray());

                    string[] SplitIP = address.ToString().Split('.');

                    writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[0])));
                    writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[1])));
                    writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[2])));
                    writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[3])));

                    writer.Write((ushort)_port);

                    writer.Write(opcode);

                    if (opcode == 'p')
                        writer.Write("8493".ToCharArray());

                    timestamp[0] = DateTime.Now;
                }

                if (qSocket.SendTo(stream.ToArray(), endpoint) > 0)
                    return true;
            }
        }

        catch
        {
            return false;
        }

        return false;
    }

    public int Receive()
    {
        try
        {
            _count = 0;

            EndPoint endpoint = new IPEndPoint(address, _port);

            byte[] rBuffer = new byte[3402];
            qSocket.ReceiveFrom(rBuffer, ref endpoint);

            timestamp[1] = DateTime.Now;

            using (MemoryStream stream = new MemoryStream(rBuffer))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    if (stream.Length <= 10)
                        return _count;

                    reader.ReadBytes(10);

                    switch (reader.ReadChar())
                    {
                        case 'c': // Client list
                            {
                                int playercount = reader.ReadInt16();

                                results = new string[playercount * 2];

                                for (int i = 0; i < playercount; i++)
                                {
                                    int namelen = reader.ReadByte();
                                    results[_count++] = new string(reader.ReadChars(namelen));

                                    results[_count++] = Convert.ToString(reader.ReadInt32());
                                }

                                return _count;
                            }                   

                        default:
                            return _count;
                    }
                }
            }
        }

        catch
        {
            return _count;
        }
    }

    public string[] Store(int count)
    {
        string[] rString = new string[count];

        for (int i = 0; i < count && i < _count; i++)
            rString[i] = results[i];

        _count = 0;

        return rString;
    }
}