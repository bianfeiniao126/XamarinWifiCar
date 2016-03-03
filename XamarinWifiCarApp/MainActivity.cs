using System;
using System.Net;
using System.Net.Sockets;
using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Encoding = System.Text.Encoding;
using Environment = System.Environment;
using SocketType = System.Net.Sockets.SocketType;
using Uri = Android.Net.Uri;

namespace XamarinWifiCarApp
{
    [Activity(Label = "Wifi Car", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : Activity
    {
        private string ControlIp = "192.168.1.1";
        private string Port = "2001";
        private string SnapShootUrl = "http://192.168.1.1:8080/?action=snapshot";
        private string CameraUrl = "http://192.168.1.1:8080/?action=stream";
        private string CMD_Forward = "w",
             CMD_Backward = "s",
             CMD_TurnLeft = "a",
             CMD_TurnRight = "d",
             CMD_TurnLeft1 = "",
             CMD_TurnRight1 = "",
             CMD_TurnLeft2 = "",
             CMD_TurnRight2 = "",
             CMD_Stop = "0",
             CMD_ServoUp = "",
             CMD_ServoDown = "",
             CMD_ServoLeft = "",
             CMD_ServoRight = "",
             CMD_ledon = "",
             CMD_ledoff = "";

        private VideoView Video { get; set; }
        private TextView LblMensaje { get; set; }
        private ImageButton BtnForward { get; set; }
        private ImageButton BtnBackward { get; set; }
        private ImageButton BtnLeft { get; set; }
        private ImageButton BtnRight { get; set; }
        private ImageButton BtnStop { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Video = FindViewById<VideoView>(Resource.Id.videoView1);
            LblMensaje = FindViewById<TextView>(Resource.Id.textMensaje);
            BtnForward = FindViewById<ImageButton>(Resource.Id.imageButtonForward);
            BtnBackward = FindViewById<ImageButton>(Resource.Id.imageButtonBackward);
            BtnLeft = FindViewById<ImageButton>(Resource.Id.imageButtonLeft);
            BtnRight = FindViewById<ImageButton>(Resource.Id.imageButtonRight);
            BtnStop = FindViewById<ImageButton>(Resource.Id.imageButtonStop);



            //var uri = Android.Net.Uri.Parse("http://ia600507.us.archive.org/25/items/Cartoontheater1930sAnd1950s1/PigsInAPolka1943.mp4");
            var uri = Android.Net.Uri.Parse("http://192.168.1.1:8080/?action=stream");
            Video.SetVideoURI(uri);
            Video.Visibility = ViewStates.Visible;
            Video.Start();
            //Video.SetVideoURI(Uri.Parse(CameraUrl));
            //Video.SetMediaController(new MediaController(this));

            BtnForward.Touch += (sender, args) =>
            {
                if (args.Event.Action == MotionEventActions.Down)
                {
                    BtnForward.SetImageResource(Resource.Drawable.sym_forward_1);
                    SendData(CMD_Forward);
                }
                else if (args.Event.Action == MotionEventActions.Up)
                {
                    SendData(CMD_Stop);
                    BtnForward.SetImageResource(Resource.Drawable.sym_forward);
                }
            };
            BtnBackward.Touch += (sender, args) =>
            {
                if (args.Event.Action == MotionEventActions.Down)
                {
                    BtnBackward.SetImageResource(Resource.Drawable.sym_backward_1);
                    SendData(CMD_Backward);
                }
                else if (args.Event.Action == MotionEventActions.Up)
                {
                    SendData(CMD_Stop);
                    BtnBackward.SetImageResource(Resource.Drawable.sym_backward);
                }
            };
            BtnLeft.Touch += (sender, args) =>
            {
                if (args.Event.Action == MotionEventActions.Down)
                {
                    BtnLeft.SetImageResource(Resource.Drawable.sym_left_1);
                    SendData(CMD_TurnLeft);
                }
                else if (args.Event.Action == MotionEventActions.Up)
                {
                    SendData(CMD_Stop);
                    BtnLeft.SetImageResource(Resource.Drawable.sym_left);
                }
            };
            BtnRight.Touch += (sender, args) =>
            {
                if (args.Event.Action == MotionEventActions.Down)
                {
                    BtnRight.SetImageResource(Resource.Drawable.sym_right_1);
                    SendData(CMD_TurnRight);
                }
                else if (args.Event.Action == MotionEventActions.Up)
                {
                    SendData(CMD_Stop);
                    BtnRight.SetImageResource(Resource.Drawable.sym_right);
                }
            };
            BtnStop.Touch += (sender, args) =>
            {
                try
                {
                    if(args.Event.Action == MotionEventActions.Down)
                    {

                        Video.SetVideoURI(Uri.Parse(CameraUrl));
                        Video.Start();
                    }
                }
                catch (Exception ex)
                {
                    LblMensaje.Text = ex.Message;
                }
            };
            // Get our button from the layout resource,
            // and attach an event to it
            //var button = FindViewById<Button>(Resource.Id.btnAlert);
            //LblMensaje = FindViewById<TextView>(Resource.Id.lblMessage);
            //button.Click += (sender, args) =>
            //{
            //    //var textBox = FindViewById<TextView>(Resource.Id.txtBox1);
            //    //Console.WriteLine(textBox.Text);
            //    //var alert = new AlertDialog.Builder(this);
            //    //alert.SetMessage(textBox.Text);
            //    //alert.SetTitle("Toma mensajaco");
            //    //alert.SetNeutralButton("OK", new EventHandler<DialogClickEventArgs>((sender, args) => { }));
            //    //alert.Show();
            //    DetectNetwork();
            //};
        }

        //private void DetectNetwork()
        //{
        //    var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
        //    var activeConnection = connectivityManager.ActiveNetworkInfo;

        //    if (activeConnection != null && activeConnection.IsConnected)
        //    {
        //        LblMensaje.Text = $"TypeName: {activeConnection.TypeName}{Environment.NewLine}";
        //    }
        //    else
        //    {
        //        LblMensaje.Text = "No hay una conexión disponible";
        //    }
        //}

        private void SendData(string data)
        {
            try
            {
                LblMensaje.Text = $"SendData({data})";
                var ips = IPAddress.Parse(ControlIp);
                var ipe = new IPEndPoint(ips, Convert.ToInt32(Port));
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(ipe);

                var bs = Encoding.ASCII.GetBytes(data);
                socket.Send(bs, bs.Length, 0);
                socket.Close();
            }
            catch (Exception e)
            {
                LblMensaje.Text = $"SendData error: { e.Message }";
            }
        }
    }
}