namespace XamarinWifiCarApp.MpegViewerTranslated
{
    public class Constant
    {
        public static string PREF_KEY_ROUTER_URL = "pref_key_router_url";
        public static string PREF_KEY_CAMERA_URL = "pref_key_camera_url";

        public static string PREF_KEY_TEST_MODE_ENABLED = "pref_key_test_enabled";
        public static string PREF_KEY_ROUTER_URL_TEST = "pref_key_router_url_test";
        public static string PREF_KEY_CAMERA_URL_TEST = "pref_key_camera_url_test";

        public static string PREF_KEY_LEN_ON = "pref_key_len_on";
        public static string PREF_KEY_LEN_OFF = "pref_key_len_off";

        public static string DEFAULT_VALUE_CAMERA_URL = "http://192.168.1.1:8080/?action=stream";
        public static string DEFAULT_VALUE_ROUTER_URL = "192.168.1.1:2001";
        public static string DEFAULT_VALUE_CAMERA_URL_TEST = "";
        public static string DEFAULT_VALUE_ROUTER_URL_TEST = "192.168.1.1:2001";

        public static string DEFAULT_VALUE_LEN_ON = "FF040100FF";
        public static string DEFAULT_VALUE_LEN_OFF = "FF040000FF";


        public static int COMMAND_LENGTH = 5;
        public static int COMMAND_RADIOX = 16;
        public static int MIN_COMMAND_REC_INTERVAL = 1000;//ms

        public static string ACTION_TAKE_PICTURE_DONE = "hanry.take_picture_done";
        public static string EXTRA_RES = "res";
        public static string EXTRA_PATH = "path";

        public static int CAM_RES_OK = 6;
        public static int CAM_RES_FAIL_FILE_WRITE_ERROR = 7;
        public static int CAM_RES_FAIL_FILE_NAME_ERROR = 8;
        public static int CAM_RES_FAIL_NO_SPACE_LEFT = 9;
        public static int CAM_RES_FAIL_BITMAP_ERROR = 10;
        public static int CAM_RES_FAIL_UNKNOW = 20;
    }
}