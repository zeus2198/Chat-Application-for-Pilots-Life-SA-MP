using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Net;
using System.ComponentModel;
using CurlSharp;
using System.IO;
using System.Xml.Linq;

namespace WpfApplication1
{
   
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        BackgroundWorker worker = new BackgroundWorker();
        DoubleAnimation da = new DoubleAnimation();
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //draggin window movement
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();           
            //((App)Application.Current).sts = 5; // MAIN 
        }

        private void worker_checknet(object sender, DoWorkEventArgs e)
        {
            bool rr;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://google.com");
                request.Timeout = 5000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK) rr = true;
                else rr = false;
            }                
            catch
            {
                rr = false;               
            }
            e.Result = rr;

            // ---- Setting file init
            ((App)Application.Current).nicks = new List<nickalert>();
            if(!File.Exists("settings.xml"))
            {
                ((App)Application.Current).fetchsec = 10;
                ((App)Application.Current).chatlogging = true;
               // ((App)Application.Current).nicks.Add(new nickalert { name = "Zeus", activated = true, color = "#ff694b" });
                ((App)Application.Current).SaveSettings();
            }
            ((App)Application.Current).LoadSettings();
        }

        private void worker_checknetCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bool)e.Result)
            {               
                da.From = 1;
                da.To = 0;
                da.Duration = new Duration(TimeSpan.FromSeconds(0.50));
                infolab.BeginAnimation(Label.OpacityProperty, da);
                infolab.Content = "Connection failed, retrying..";
                da.From = 0;
                da.To = 1;
                infolab.BeginAnimation(Label.OpacityProperty, da);                
                BackgroundWorker worker2 = new BackgroundWorker();
                worker2.DoWork += worker_checknet;
                worker2.RunWorkerCompleted += worker_checknetCompleted;
                worker2.WorkerSupportsCancellation = true;
                worker2.RunWorkerAsync();
            }
            else
            {
                ThicknessAnimation ta = new ThicknessAnimation();             
                ta.From = grd.Margin;           
                ta.To = new Thickness(30, 30, 30, 100);
                ta.Duration = new Duration(TimeSpan.FromSeconds(1));
                CubicEase easingFunction = new CubicEase();
                easingFunction.EasingMode = EasingMode.EaseOut;
                ta.EasingFunction = easingFunction;                
                Timeline.SetDesiredFrameRate(ta, 60);
                grd.BeginAnimation(Grid.MarginProperty, ta);
                userDock.Visibility = Visibility.Visible;
                passDock.Visibility = Visibility.Visible;                
                da.From = 1;
                da.To = 0;
                da.Completed += hideLabel;
                da.Duration = new Duration(TimeSpan.FromSeconds(0.7));
                infolab.BeginAnimation(Label.OpacityProperty, da);
                da.From = 0;
                da.To = 1;
                da.Completed -= hideLabel;
                userDock.BeginAnimation(DockPanel.OpacityProperty, da);
                passDock.BeginAnimation(DockPanel.OpacityProperty, da);               
            }
        }
       

        private void hideLabel(object sender, EventArgs e)
        {
            infolab.Visibility = Visibility.Collapsed;
        }

        private void passKeyDown(object sender, KeyEventArgs e)
        {            
            if(e.Key == Key.Enter)
            {                               
                if (userBox.Text == "Username" || passBox.Text == "Password")
                {
                    infolab.Foreground = Brushes.Yellow;
                    infolab.Content = "Fill in the empty feild";
                    infolab.Visibility = Visibility.Visible;
                    da.From = 0;
                    da.To = 1;
                    da.Duration = new Duration(TimeSpan.FromSeconds(0.50));
                    infolab.BeginAnimation(Label.OpacityProperty, da);
                    return;
                }
                else if (userBox.Text.Length < 3 || passBox.Text.Length < 3)
                {
                    infolab.Foreground = Brushes.Yellow;
                    infolab.Content = "Username/Password too short";
                    infolab.Visibility = Visibility.Visible;                    
                    da.From = 0;
                    da.To = 1;
                    da.Duration = new Duration(TimeSpan.FromSeconds(0.50));
                    infolab.BeginAnimation(Label.OpacityProperty, da);
                    return;
                }
                Keyboard.ClearFocus();               
                loginWaitLayer.Visibility = Visibility.Visible;
                userBox.IsEnabled = false;
                passBox.IsEnabled = false;
                passBox.Foreground = Brushes.White;             
                da.From = 0;
                da.To = 1;                
                da.Duration = new Duration(TimeSpan.FromSeconds(0.7));
                loginWaitLayer.BeginAnimation(Control.OpacityProperty, da);
                worker.DoWork -= worker_checknet;
                worker.RunWorkerCompleted -= worker_checknetCompleted;
                worker.DoWork -= logQuery;
                worker.ProgressChanged -= loginProgress;
                worker.DoWork += logQuery;
                worker.ProgressChanged += loginProgress;
                worker.WorkerReportsProgress = true;
                String[] passArr = {userBox.Text, passBox.Text};    
                worker.RunWorkerAsync(passArr);
            }
        }
        private void loginProgress(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == 1)
            {               
                infolab.Foreground = Brushes.Red;
                infolab.Content = "Wrong Username/Password!";
                infolab.Visibility = Visibility.Visible;
                da.From = 0;
                da.To = 1;
                da.Duration = new Duration(TimeSpan.FromSeconds(0.9));
                infolab.BeginAnimation(Label.OpacityProperty, da);
                da.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                da.From = 1;
                da.To = 0;
                da.Completed += hideLayer;
                loginWaitLayer.BeginAnimation(Control.OpacityProperty, da);                
                userBox.IsEnabled = true;
                passBox.IsEnabled = true;
            }
            else
            {                
                //MainWindow mm = new MainWindow(userBox.Text);
                // changes
                MainWindow mm = new MainWindow(userBox.Text, passBox.Text);
                this.Close();
                mm.Show();
            }
        }
        private void logQuery(object sender, DoWorkEventArgs e)
        {
            Curl.GlobalInit(CurlInitFlag.All);
            using (var easy = new CurlEasy())
            {
                String[] ar = e.Argument as String[];
                string postData = "form_submitted=1&form_username=" + ar[0] + "&form_password=" + ar[1] + "&form_autologin=1";                
                var postLength = postData.Length;
                easy.WriteFunction += OnWriteData;
                easy.WriteData = null;
                easy.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ubuntu Chromium/32.0.1700.107 Chrome/32.0.1700.107 Safari/537.36";
                easy.FollowLocation = true;
                easy.Url = "https://thepilotslife.com/?";
                string fileName = "cookie.txt";
                FileInfo f = new FileInfo(fileName);
                easy.CookieJar = f.FullName;
                easy.CookieFile = f.FullName;
                easy.SslVerifyPeer = false;
                easy.CookieSession = true;
                easy.Post = true;
                easy.PostFields = postData;
                easy.PostFieldSize = postLength;
                var code = easy.Perform();
            }
            Curl.GlobalCleanup();
        }
        private Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {            
            if (Encoding.UTF8.GetString(buf).IndexOf("Logged in as") == -1)worker.ReportProgress(1);
            else worker.ReportProgress(2);
            return size * nmemb;
        }

        private void hideLayer(object sender, EventArgs e)
        {
            loginWaitLayer.Visibility = Visibility.Collapsed;
            da.Completed -= hideLayer;
        }
        
        private void txt_gotfocus(object sender, RoutedEventArgs e)
        {
            var tt = sender as TextBox;
            if (tt.Text == "Username" || tt.Text == "Password") tt.Text = "";
        }
        private void txt_lostfocus_pass(object sender, RoutedEventArgs e)
        {
            var tt = sender as TextBox;
            if (tt.Text == "") tt.Text = "Password";
        }
        private void txt_lostfocus_user(object sender, RoutedEventArgs e)
        {
            var tt = sender as TextBox;
            if (tt.Text == "") tt.Text = "Username";           
        }

        private void Intialize_custom(object sender, EventArgs e)
        {           
            timer.Stop();    
            worker.DoWork += worker_checknet;
            worker.RunWorkerCompleted += worker_checknetCompleted;           
            worker.RunWorkerAsync();
            Directory.CreateDirectory("log/");          
        }
        
        
        public Window1()
        {            
            InitializeComponent();            
            timer.Tick += new EventHandler(Intialize_custom);
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Start();            
        }

        private void clo_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void mini_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }      
       
    }
}
