
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ActiveWindow active = new ActiveWindow();
        Database database = new Database();
        FileHandler fileHandler = new FileHandler();
        Helpers helpers = new Helpers();
        private delegate void NoargDelegate();
        Dictionary<string,long[]> tempData = new Dictionary<string,long[]>();
        Dictionary<string, int> Apptime = new Dictionary<string, int>();
        System.Windows.Forms.NotifyIcon NotifyIcon;

        public MainWindow()
        {

            InitializeComponent();
            
           
            NotifyIcon = new System.Windows.Forms.NotifyIcon();
            NotifyIcon.Icon = new System.Drawing.Icon(@"stopwatch.ico");

            NotifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(NotifyIconMouseMove);
            NotifyIcon.DoubleClick +=
            delegate (object sender, EventArgs args)
            {
                NotifyIcon.Visible = false;
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            Thread thread = new Thread(new ThreadStart(FocusedWindowTracker));
            thread.IsBackground = true;
            thread.Start();

    }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }

        private void NotifyIconMouseMove(Object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
                NotifyIcon.Visible = true;
                base.OnStateChanged(e);
            }
        }
        void AppClosing(object sender, CancelEventArgs e)
        {
            NotifyIcon.Visible = false;
            tempData[AppName.Text].SetValue(DateTimeOffset.Now.ToUnixTimeSeconds(), 1);
            Database.SendData(tempData);
            Environment.Exit(Environment.ExitCode);

        }
        private void FocusedWindowTracker()
        {
            while (true)
            {

                try {
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() =>
                    {
                        string ActiveApp = active.GetActive();
                        if(ActiveApp != "TimeCounter")
                        {

                       

                            if (AppName.Text != ActiveApp )
                        {
                            if (!Apptime.ContainsKey(ActiveApp))
                            {
                                Apptime.Add(ActiveApp, Database.GetAppTime(ActiveApp));
                            }
                           


                            if (!tempData.ContainsKey(AppName.Text))
                            {
                                tempData.Add(AppName.Text, new long[]{ DateTimeOffset.Now.ToUnixTimeSeconds(), DateTimeOffset.Now.ToUnixTimeSeconds() } );
                            }
                            else
                            {
                                tempData[AppName.Text].SetValue(DateTimeOffset.Now.ToUnixTimeSeconds(), 1) ;
                            }


                            if (!tempData.ContainsKey(ActiveApp))
                            {
                                tempData.Add(ActiveApp, new long[] { DateTimeOffset.Now.ToUnixTimeSeconds(), DateTimeOffset.Now.ToUnixTimeSeconds() });
                            }

                            AppName.Text = ActiveApp;
                           


                        }
                        
                        TodayTime.Text = helpers.UnixToTimeFormat((DateTimeOffset.Now.ToUnixTimeSeconds() - tempData[AppName.Text][0]));
                        long totaltime = Apptime[AppName.Text] + (DateTimeOffset.Now.ToUnixTimeSeconds() - tempData[AppName.Text][0]);
                        TotalTime.Text = helpers.UnixToTimeFormat(totaltime);
                        NotifyIcon.Text = "Double Click To Open";
                        }
                    });    

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                } 
                
            }

            
        }

        private void CloseApp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MimalizeToTry(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            NotifyIcon.Visible = true;
        }
    }
}
