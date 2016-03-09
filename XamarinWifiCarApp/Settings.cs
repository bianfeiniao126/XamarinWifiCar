using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace XamarinWifiCarApp
{
    [Activity(Label = "Settings", Icon = "@drawable/sym_setting")]
    public class Settings : PreferenceActivity
    {
        private EditTextPreference mPrefRouterUrl;
        private EditTextPreference mPrefCameraUrl;
        private EditTextPreference mPrefRouterUrlTest;
        private EditTextPreference mPrefCameraUrlTest;

        private EditTextPreference mPrefLenOn;
        private EditTextPreference mPrefLenOff;

        private EditTextPreference mPrefAvanzar;
        private EditTextPreference mPrefRetroceder;
        private EditTextPreference mPrefIzquierda;
        private EditTextPreference mPrefDerecha;
        private EditTextPreference mPrefParar;

        private EditTextPreference txtPotenciaMotor;
        private EditTextPreference txtCorreccionMotorIzquiedo;
        private EditTextPreference txtCorreccionMotorDerecho;
        private EditTextPreference txtPotenciaGiro;

        private EditTextPreference txtCommandStart;
        private EditTextPreference txtCommandPotenciaMotor;
        private EditTextPreference txtCommandCorreccionMotorIzquiedo;
        private EditTextPreference txtCommandCorreccionMotorDerecho;
        private EditTextPreference txtCommandPotenciaGiro;
        private EditTextPreference txtCommandServoVertical;
        private EditTextPreference txtCommandServoHorizontal;

        private ListPreference listGearStep;

        protected override void OnCreate(Bundle savedInstance)
        {
            base.OnCreate(savedInstance);
            AddPreferencesFromResource(Resource.Xml.Preferences);
            mPrefRouterUrl = (EditTextPreference)FindPreference(Constant.PrefKeyRouterUrl);
            mPrefCameraUrl = (EditTextPreference)FindPreference(Constant.PrefKeyCameraUrl);
            mPrefRouterUrlTest = (EditTextPreference)FindPreference(Constant.PrefKeyRouterUrlTest);
            mPrefCameraUrlTest = (EditTextPreference)FindPreference(Constant.PrefKeyCameraUrlTest);

            mPrefLenOn = (EditTextPreference)FindPreference(Constant.PrefKeyLenOn);
            mPrefLenOff = (EditTextPreference)FindPreference(Constant.PrefKeyLenOff);

            mPrefAvanzar = (EditTextPreference)FindPreference(Constant.PrefKeyAvanzar);
            mPrefRetroceder = (EditTextPreference)FindPreference(Constant.PrefKeyRetroceder);
            mPrefIzquierda = (EditTextPreference)FindPreference(Constant.PrefKeyIzquierda);
            mPrefDerecha = (EditTextPreference)FindPreference(Constant.PrefKeyDerecha);
            mPrefParar = (EditTextPreference)FindPreference(Constant.PrefKeyParar);

            txtPotenciaMotor = (EditTextPreference)FindPreference(Constant.PrefKeyPotencia);
            txtCorreccionMotorDerecho = (EditTextPreference)FindPreference(Constant.PrefKeyPotenciaDerecho);
            txtCorreccionMotorIzquiedo = (EditTextPreference)FindPreference(Constant.PrefKeyPotenciaIzquierdo);
            txtPotenciaGiro = (EditTextPreference)FindPreference(Constant.PrefKeyPotenciaGiro);

            txtCommandStart = (EditTextPreference)FindPreference(Constant.PrefKeyCommandStart);
            txtCommandCorreccionMotorDerecho = (EditTextPreference)FindPreference(Constant.PrefKeyCommandPotenciaDerecho);
            txtCommandCorreccionMotorIzquiedo = (EditTextPreference)FindPreference(Constant.PrefKeyCommandPotenciaIzquierdo);
            txtCommandPotenciaGiro = (EditTextPreference)FindPreference(Constant.PrefKeyCommandPotenciaGiro);
            txtCommandPotenciaMotor = (EditTextPreference)FindPreference(Constant.PrefKeyCommandPotencia);
            txtCommandServoHorizontal = (EditTextPreference)FindPreference(Constant.PrefKeyCommandServoHorizontal);
            txtCommandServoVertical = (EditTextPreference)FindPreference(Constant.PrefKeyCommandServoVertical);

            listGearStep = (ListPreference)FindPreference(Constant.PrefKeyGearStep);

            InitValue();
        }
        public void InitValue()
        {
            var settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var cameraUrl = settings.GetString(Constant.PrefKeyCameraUrl, Constant.DefaultValueCameraUrl);
            mPrefCameraUrl.Summary = cameraUrl;

            var routerUrl = settings.GetString(Constant.PrefKeyRouterUrl, Constant.DefaultValueRouterUrl);
            mPrefRouterUrl.Summary = routerUrl;

            var testCameraUrl = settings.GetString(Constant.PrefKeyCameraUrlTest, Constant.DefaultValueCameraUrlTest);
            mPrefCameraUrlTest.Summary = testCameraUrl;

            var testRouterUrl = settings.GetString(Constant.PrefKeyRouterUrlTest, Constant.DefaultValueRouterUrlTest);
            mPrefRouterUrlTest.Summary = testRouterUrl;

            var lenon = settings.GetString(Constant.PrefKeyLenOn, Constant.DefaultValueLenOn);
            mPrefLenOn.Summary = lenon;

            var lenoff = settings.GetString(Constant.PrefKeyLenOff, Constant.DefaultValueLenOff);
            mPrefLenOff.Summary = lenoff;
            
            var cmdAvanzar = settings.GetString(Constant.PrefKeyAvanzar, Constant.DefaultValueAvanzar);
            mPrefAvanzar.Summary = cmdAvanzar;
            var cmdRetroceder = settings.GetString(Constant.PrefKeyRetroceder, Constant.DefaultValueRetroceder);
            mPrefRetroceder.Summary = cmdRetroceder;
            var cmdIzquierda = settings.GetString(Constant.PrefKeyIzquierda, Constant.DefaultValueIzquierda);
            mPrefIzquierda.Summary = cmdIzquierda;
            var cmdDerecha = settings.GetString(Constant.PrefKeyDerecha, Constant.DefaultValueDerecha);
            mPrefDerecha.Summary = cmdDerecha;
            var cmdParar = settings.GetString(Constant.PrefKeyParar, Constant.DefaultValueParar);
            mPrefParar.Summary = cmdParar;

            var potenciaMotor = settings.GetString(Constant.PrefKeyPotencia, Constant.DefaultValuePotencia);
            txtPotenciaMotor.Summary = potenciaMotor;
            var correccionMotorDerecho = settings.GetString(Constant.PrefKeyPotenciaDerecho, Constant.DefaultValuePotenciaDerecho);
            txtCorreccionMotorDerecho.Summary = correccionMotorDerecho;
            var correccionMotorIzquierdo = settings.GetString(Constant.PrefKeyPotenciaIzquierdo, Constant.DefaultValuePotenciaIzquierdo);
            txtCorreccionMotorIzquiedo.Summary = correccionMotorIzquierdo;
            var potenciaGiro = settings.GetString(Constant.PrefKeyPotenciaGiro, Constant.DefaultValuePotenciaGiro);
            txtPotenciaGiro.Summary = potenciaGiro;

            var cmdStart = settings.GetString(Constant.PrefKeyCommandStart, Constant.DefaultValueCommandStart);
            txtCommandStart.Summary = cmdStart;
            var cmdCorreccionDerecho = settings.GetString(Constant.PrefKeyCommandPotenciaDerecho, Constant.DefaultValueCommandPotenciaDerecho);
            txtCommandCorreccionMotorDerecho.Summary = cmdCorreccionDerecho;
            var cmdCorreccionIzquiedo = settings.GetString(Constant.PrefKeyCommandPotenciaIzquierdo, Constant.DefaultValueCommandPotenciaIzquierdo);
            txtCommandCorreccionMotorIzquiedo.Summary = cmdCorreccionIzquiedo;
            var cmdPotenciaGiro = settings.GetString(Constant.PrefKeyCommandPotenciaGiro, Constant.DefaultValueCommandPotenciaGiro);
            txtCommandPotenciaGiro.Summary = cmdPotenciaGiro;
            var cmdPotenciaMotor = settings.GetString(Constant.PrefKeyCommandPotencia, Constant.DefaultValueCommandPotencia);
            txtCommandPotenciaMotor.Summary = cmdPotenciaMotor;
            var cmdServoHorizontal = settings.GetString(Constant.PrefKeyCommandServoHorizontal, Constant.DefaultValueCommandServoHorizontal);
            txtCommandServoHorizontal.Summary = cmdServoHorizontal;
            var cmdServoVertical = settings.GetString(Constant.PrefKeyCommandServoVertical, Constant.DefaultValueCommandServoVertical);
            txtCommandServoVertical.Summary = cmdServoVertical;

            var gearStep = settings.GetString(Constant.PrefKeyGearStep, Constant.DefaultValueGearStep);
            listGearStep.Summary = gearStep;
        }
    }
}