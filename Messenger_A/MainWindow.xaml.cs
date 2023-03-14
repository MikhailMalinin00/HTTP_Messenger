using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Messenger_A
{
    public partial class MainWindow : Window
    {
        private HttpListener _HttpListener = new HttpListener()
                .Set_Prefixes_Add("http://127.0.0.1:8888/connection/", _IsOpen: false)
                .Set_Start();
        private Style TextBlockStyle = (Style)Application.Current.Resources["TextBlockStyle"];
        private Style BorderStyle = (Style)Application.Current.Resources["BorderStyle"];
        private Brush Gray = (Brush)Application.Current.Resources["Gray"];
        public static BackgroundWorker BackgroundWorker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 26;
            BackgroundWorker.DoWork += GetMessegesWork;
            BackgroundWorker.RunWorkerAsync(null);
        }
        public void GetMessegesWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                AddTB(new HttpClient().GetStringAsync("http://127.0.0.2:8889/connection/?qweBFYUWEFGUYSDADSDFW").GetAwaiter().GetResult(), HorizontalAlignment.Left, TextAlignment.Left);
            }
        }
        private void AddTB(string Text, HorizontalAlignment ha, TextAlignment ta)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TextBlock tb = new TextBlock
                {
                    Text = Text,
                    Style = TextBlockStyle,
                    TextAlignment = ta,
                    HorizontalAlignment = ha,
                    Margin = new Thickness(5)
                };

                Border border = new Border
                {
                    Style = BorderStyle,
                    Background = Gray,
                    HorizontalAlignment = ha,
                    Child = tb
                };

                MessegesSP.Children.Add(border);
            });
        }
        private void SendClick(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
        private void TBKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }
        private void SendMessage()
        {
            AddTB(SendText.Text, HorizontalAlignment.Right, TextAlignment.Right);
            
            _HttpListener
                    .Get_ContextAsync(a =>
                    {
                        a.Response.Set_Bytes(SendText.Text.Get_Encoding_UTF8_Bytes());
                    }
                );
            SendText.Text = string.Empty;
        }
    }
    public static class Ext_HttpListener
    {
        public static System.Net.HttpListener Set_Prefixes_Add(this System.Net.HttpListener _this, string _str = "http://127.0.0.1:8888/connection/", System.Boolean _IsOpen = false)
        {
            _this.Prefixes.Add(_str);
            if (_IsOpen) System.Diagnostics.Process.Start(_str);
            return _this;
        }
        /// <summary>
        /// Начинаем фоном прослушивать входящие подключения
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static System.Net.HttpListener Set_Start(this System.Net.HttpListener _this)
        { _this.Start(); return _this; }
        public static System.Net.HttpListener Set_Stop(this System.Net.HttpListener _this)
        { _this.Stop(); return _this; }
        public static System.Net.HttpListener Get_ContextAsync(this System.Net.HttpListener _this, System.Action<HttpListenerContext> A)
        { A(_this.GetContextAsync().GetAwaiter().GetResult()); return _this; }
        public static System.Net.HttpListener Get_ContextAsync_WhileTrue(this System.Net.HttpListener _this, System.Action<HttpListenerContext> A)
        { while (true) _this.Get_ContextAsync(A); return _this; }
    }
    public static class Ext_String
    {
        public static byte[] Get_Encoding_UTF8_Bytes(this System.String _this) { return Encoding.UTF8.GetBytes(_this); }
    }
    public static class Ext_HttpListenerResponse
    {
        public static System.Net.HttpListenerResponse Set_Bytes(this System.Net.HttpListenerResponse _this, byte[] _Bytes)
        {
            _this.ContentLength64 = _Bytes.Length;
            Stream output = _this.OutputStream;
            output.WriteAsync(_Bytes, 0, _Bytes.Length);
            output.FlushAsync();
            return _this;
        }
    }
}
