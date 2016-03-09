using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinWifiCarApp
{
    public class Constant
    {
        public const string PrefKeyRouterUrl = "pref_key_router_url";
        public const string PrefKeyCameraUrl = "pref_key_camera_url";

        public const string PrefKeyCameraUrlTest = "pref_key_camera_url_test";
        public const string PrefKeyTestModeEnabled = "pref_key_test_enabled";
        public const string PrefKeyRouterUrlTest = "pref_key_router_url_test";

        public const string PrefKeyLenOn = "pref_key_len_on";
        public const string PrefKeyLenOff = "pref_key_len_off";

        public const string PrefKeyAvanzar = "pref_key_avanzar";
        public const string PrefKeyRetroceder = "pref_key_retroceder";
        public const string PrefKeyIzquierda = "pref_key_izquierda";
        public const string PrefKeyDerecha = "pref_key_derecha";
        public const string PrefKeyParar = "pref_key_parar";

        public const string PrefKeyGearStep = "pref_key_gear_step";

        public const string PrefKeyCommandStart = "pref_key_command_start";
        public const string PrefKeyCommandServoVertical = "pref_key_command_servo_vertical";
        public const string PrefKeyCommandServoHorizontal = "pref_key_command_servo_horizontal";
        public const string PrefKeyCommandPotencia = "pref_key_command_potencia";
        public const string PrefKeyCommandPotenciaIzquierdo = "pref_key_command_potencia_izquierdo";
        public const string PrefKeyCommandPotenciaDerecho = "pref_key_command_potencia_derecho";
        public const string PrefKeyCommandPotenciaGiro = "pref_key_command_potencia_giro";

        public const string PrefKeyPotencia = "pref_key_potencia";
        public const string PrefKeyPotenciaIzquierdo = "pref_key_potencia_izquierdo";
        public const string PrefKeyPotenciaDerecho = "pref_key_potencia_derecho";
        public const string PrefKeyPotenciaGiro = "pref_key_potencia_giro";

        public const string DefaultValueCameraUrl = "http://192.168.1.1:8080/?action=stream";
        public const string DefaultValueRouterUrl = "192.168.1.1:2001";
        public const string DefaultValueCameraUrlTest = "";
        public const string DefaultValueRouterUrlTest = "192.168.1.1:2001";

        public const string DefaultValueLenOn = "n";
        public const string DefaultValueLenOff = "m";

        public const string DefaultValueAvanzar = "w";
        public const string DefaultValueRetroceder = "s";
        public const string DefaultValueIzquierda = "a";
        public const string DefaultValueDerecha = "d";
        public const string DefaultValueParar = "0";

        public const string DefaultValueGearStep = "2";

        public const string DefaultValueCommandStart = ".";
        public const string DefaultValueCommandServoVertical = "v";
        public const string DefaultValueCommandServoHorizontal = "h";
        public const string DefaultValueCommandPotencia = "p";
        public const string DefaultValueCommandPotenciaIzquierdo = "i";
        public const string DefaultValueCommandPotenciaDerecho = "d";
        public const string DefaultValueCommandPotenciaGiro = "g";

        public const string DefaultValuePotencia = "90";
        public const string DefaultValuePotenciaIzquierdo = "90";
        public const string DefaultValuePotenciaDerecho = "100";
        public const string DefaultValuePotenciaGiro = "70";
        
        public const int CommandLength = 5;
        public const int CommandRadiox = 16;
        public const int MinCommandRecInterval = 1000;//ms

        public const string ActionTakePictureDone = "hanry.take_picture_done";
        public const string ExtraRes = "res";
        public const string ExtraPath = "path";

        public const int CamResOk = 6;
        public const int CamResFailFileWriteError = 7;
        public const int CamResFailFileNameError = 8;
        public const int CamResFailNoSpaceLeft = 9;
        public const int CamResFailBitmapError = 10;
        public const int CamResFailUnknow = 20;
    }
}