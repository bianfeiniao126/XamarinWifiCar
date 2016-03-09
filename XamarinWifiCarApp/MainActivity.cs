using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using XamarinWifiCarApp.Custom;
using Encoding = System.Text.Encoding;
using SocketType = System.Net.Sockets.SocketType;
using Com.Camera.Simplemjpeg;
using System.Threading.Tasks;
using Android.Content;
using Android.Preferences;

namespace XamarinWifiCarApp
{
    [Activity(Label = "Wifi Car", MainLauncher = true, Icon = "@drawable/coche_ico", ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : Activity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private const int MinCameraServoValue = 5;
        private const int MaxCameraServoValue = 180;
        private string ControlIp = "192.168.1.1";
        private string Port = "2001";
        private string SnapShootUrl = "http://192.168.1.1:8080/?action=snapshot";
        private string CameraUrl = "http://192.168.1.1:8080/?action=stream";
        private int widthVideoResolution = 320;
        private int heightVideoResolution = 240;
        private bool suspendingVideo = false;
        private bool verFPS = false;
        private bool ledsOn = false;
        private string CMD_Forward = "w",
             CMD_Backward = "s",
             CMD_TurnLeft = "a",
             CMD_TurnRight = "d",
             CMD_Stop = "0",
             CMD_ServoUp = "i",
             CMD_ServoDown = "k",
             CMD_ServoLeft = "j",
             CMD_ServoRight = "l",
             CMD_ledon = "n",
             CMD_ledoff = "m",
             CMD_SetServoX = ".h",
             CMD_SetServoY = ".v";
        private MjpegView VisorVideo { get; set; }
        private TextView LblMensaje { get; set; }
        private ImageButton BtnForward { get; set; }
        private ImageButton BtnBackward { get; set; }
        private ImageButton BtnLeft { get; set; }
        private ImageButton BtnRight { get; set; }
        private ImageButton BtnStop { get; set; }
        private ImageButton BtnLuces { get; set; }
        private ImageButton BtnSettings { get; set; }
        private Button BtnCentrarCamara { get; set; }
        private SeekBar HorizontalSeekBar { get; set; }
        private VerticalSeekBar VerticalSeekBar { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            var view = Resource.Layout.Main;
            SetContentView(view);

            VisorVideo = FindViewById<MjpegView>(Resource.Id.mjpegView);
            LblMensaje = FindViewById<TextView>(Resource.Id.textMensaje);
            BtnForward = FindViewById<ImageButton>(Resource.Id.imageButtonForward);
            BtnBackward = FindViewById<ImageButton>(Resource.Id.imageButtonBackward);
            BtnLeft = FindViewById<ImageButton>(Resource.Id.imageButtonLeft);
            BtnRight = FindViewById<ImageButton>(Resource.Id.imageButtonRight);
            BtnStop = FindViewById<ImageButton>(Resource.Id.imageButtonStop);
            BtnLuces = FindViewById<ImageButton>(Resource.Id.imageButtonLeds);
            BtnSettings = FindViewById<ImageButton>(Resource.Id.imageButtonSetting);
            BtnCentrarCamara = FindViewById<Button>(Resource.Id.btnCentrarCamara);
            HorizontalSeekBar = FindViewById<SeekBar>(Resource.Id.horizontalSeekBar);
            VerticalSeekBar = FindViewById<VerticalSeekBar>(Resource.Id.verticalSeekBar);

            VisorVideo?.SetResolution(widthVideoResolution, heightVideoResolution);

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
                    if (args.Event.Action == MotionEventActions.Down)
                    {
                        //BeginStreaming(CameraUrl);
                        //RequestedOrientation = RequestedOrientation == ScreenOrientation.Landscape
                        //    ? ScreenOrientation.ReverseLandscape
                        //    : ScreenOrientation.Landscape;
                        verFPS = !verFPS;
                        VisorVideo.SetDisplayMode(MjpegView.SizeStandard);
                        VisorVideo.SetDisplayMode(MjpegView.SizeFullscreen);
                        VisorVideo.ShowFps(verFPS);
                    }
                }
                catch (Exception ex)
                {
                    LblMensaje.Text = ex.Message;
                }
            };
            BtnLuces.Touch += (sender, args) =>
            {
                try
                {
                    if (args.Event.Action == MotionEventActions.Down)
                    {
                        ledsOn = !ledsOn;
                        SendData(ledsOn ? CMD_ledon : CMD_ledoff);
                        BtnLuces.SetImageResource(ledsOn ? Resource.Drawable.sym_light : Resource.Drawable.sym_light_off);
                    }
                }
                catch (Exception ex)
                {
                    LblMensaje.Text = ex.Message;
                }
            };
            BtnSettings.Touch += (sender, args) =>
            {
                try
                {
                    if (args.Event.Action == MotionEventActions.Down)
                    {
                        StartActivity(typeof(Settings));
                    }
                }
                catch (Exception ex)
                {
                    LblMensaje.Text = ex.Message;
                }
            };
            HorizontalSeekBar.Max = MaxCameraServoValue;
            HorizontalSeekBar.Progress = 90;
            var waitingX = false;
            HorizontalSeekBar.ProgressChanged += (sender, args) =>
            {
                if (args.Progress >= MinCameraServoValue)
                {
                    if (!waitingX)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            waitingX = true;
                            Thread.Sleep(50);
                            waitingX = false;
                            RunOnUiThread(() => SendData($"{CMD_SetServoX}{args.Progress}"));
                        });
                    }
                }
            };
            VerticalSeekBar.Max = MaxCameraServoValue;
            VerticalSeekBar.Progress = 90;
            VerticalSeekBar.LayoutParameters.Width = ViewGroup.LayoutParams.WrapContent;
            var waitingY = false;
            VerticalSeekBar.ProgressChanged += (sender, args) =>
            {
                if (args.Progress >= MinCameraServoValue)
                {
                    if (!waitingY)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            waitingY = true;
                            Thread.Sleep(50);
                            waitingY = false;
                            RunOnUiThread(() => SendData($"{CMD_SetServoY}{args.Progress}"));
                        });
                    }
                }
            };
            BtnCentrarCamara.Click += (sender, args) =>
            {
                VerticalSeekBar.Progress = 90;
                Thread.Sleep(100);
                HorizontalSeekBar.Progress = 90;
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
            BeginStreaming(CameraUrl);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (VisorVideo != null)
            {
                if (suspendingVideo)
                {
                    BeginStreaming(CameraUrl);
                    suspendingVideo = false;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (VisorVideo != null)
            {
                VisorVideo.FreeCameraMemory();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (VisorVideo != null)
            {
                if (VisorVideo.IsStreaming)
                {
                    VisorVideo.StopPlayback();
                    suspendingVideo = true;
                }
            }
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

        private void CargarConfiguracion()
        {
            PreferenceManager.GetDefaultSharedPreferences(this).RegisterOnSharedPreferenceChangeListener(this);
        }
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
        public void BeginStreaming(string url)
        {
            //Create a new task with a MjpegInputStream return
            Task.Factory.StartNew(() =>
            {
                try
                {
                    //inicialize streaming
                    return MjpegInputStream.Read(url);
                }
                catch (Exception e)
                {
                    //if something was wrong return null
                    Console.WriteLine(e.Message);
                }
                return null;
            }).ContinueWith((t) =>
            {
                //check if the result was fine
                VisorVideo.SetSource(t.Result);
                if (t.Result != null)
                {
                    //set skip to result
                    t.Result.SetSkip(1);
                    LblMensaje.Text = "Connected";
                }
                else
                {
                    LblMensaje.Text = "Disconnected";
                }
                //set display mode
                VisorVideo.SetDisplayMode(MjpegView.SizeFullscreen);
                //set if you need to see FPS
                VisorVideo.ShowFps(verFPS);
            });
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key == Constant.PrefKeyRouterUrl)
            {
                var algo = sharedPreferences.GetString(Constant.PrefKeyRouterUrl, Constant.DefaultValueRouterUrl);
            }
        }
    }
}