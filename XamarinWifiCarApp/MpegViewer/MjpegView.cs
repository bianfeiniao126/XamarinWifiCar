using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Text;
using String = System.String;

namespace XamarinWifiCarApp.MpegViewer
{
    public class MjpegView : SurfaceView, ISurfaceHolderCallback
    {
        public static int POSITION_UPPER_LEFT = 9;
        public static int POSITION_UPPER_RIGHT = 3;
        public static int POSITION_LOWER_LEFT = 12;
        public static int POSITION_LOWER_RIGHT = 6;

        public static int SIZE_STANDARD = 1;
        public static int SIZE_BEST_FIT = 4;
        public static int SIZE_FULLSCREEN = 8;
        private static string SAVE_TO_DIR = "mjpegview";

        private MjpegViewThread thread;
        private static MjpegInputStream mIn = null;
        private static string mInUrl = null;
        private static bool showFps = true;
        private static bool mRun = false;
        private static bool surfaceDone = false;
        private static Paint overlayPaint;
        private int overlayTextColor;
        private int overlayBackgroundColor;
        private static int ovlPos;
        private static int dispWidth;
        private static int dispHeight;
        private static int displayMode;
        private static int mScreenWidth;
        private static int mScreenHeight;
        private bool resume = false;
        private static bool mtakePic = false;//flag for take a picture
        private static string mFileName = null;//file name to save picture

        private Context context;

        public MjpegView(Context context) : base(context)
        {
            Init(context);
        }
        public MjpegView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context);
        }

        private void Init(Context context)
        {
            this.context = context;
            //SurfaceHolder holder = getHolder();
            //holder.addCallback(this);
            //thread = new MjpegViewThread(holder, context);
            //setFocusable(true);
            //if (!resume)
            //{
            //    resume = true;
            //    overlayPaint = new Paint();
            //    overlayPaint.setTextAlign(Paint.Align.LEFT);
            //    overlayPaint.setTextSize(12);
            //    overlayPaint.setTypeface(Typeface.DEFAULT);
            //    overlayTextColor = Color.WHITE;
            //    overlayBackgroundColor = Color.BLACK;
            //    ovlPos = MjpegView.POSITION_LOWER_RIGHT;
            //    displayMode = MjpegView.SIZE_STANDARD;
            //    dispWidth = getWidth();
            //    dispHeight = getHeight();

            //    Log.Debug("MjpegView", "init successfully!");
            //}
            //setOverlayTextColor(Color.GREEN);
            //DisplayMetrics dm = getResources().getDisplayMetrics();
            //mScreenWidth = dm.widthPixels;
            //mScreenHeight = dm.heightPixels;
            //setKeepScreenOn(true);
        }

        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
            //thread.setSurfaceSize(w, h);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            surfaceDone = true;
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            surfaceDone = false;
            //StopPlayback();
        }

        class MjpegViewThread : Thread
        {
            private ISurfaceHolder mSurfaceHolder;
            private int frameCounter = 0;
            private long start;
            private Bitmap ovl;

            public MjpegViewThread(ISurfaceHolder surfaceHolder, Context context)
            {
                mSurfaceHolder = surfaceHolder;
            }

            private Rect DestRect(int bmw, int bmh)
            {
                int tempx;
                int tempy;
                if (displayMode == MjpegView.SIZE_STANDARD)
                {
                    tempx = (dispWidth / 2) - (bmw / 2);
                    tempy = (dispHeight / 2) - (bmh / 2);
                    return new Rect(tempx, tempy, bmw + tempx, bmh + tempy);
                }
                if (displayMode == MjpegView.SIZE_BEST_FIT)
                {
                    float bmasp = (float)bmw / (float)bmh;
                    bmw = dispWidth;
                    bmh = (int)(dispWidth / bmasp);
                    if (bmh > dispHeight)
                    {
                        bmh = dispHeight;
                        bmw = (int)(dispHeight * bmasp);
                    }
                    tempx = (dispWidth / 2) - (bmw / 2);
                    tempy = (dispHeight / 2) - (bmh / 2);
                    return new Rect(tempx, tempy, bmw + tempx, bmh + tempy);
                }
                if (displayMode == MjpegView.SIZE_FULLSCREEN)
                    return new Rect(0, 0, dispWidth, dispHeight);
                return null;
            }
            public void SetSurfaceSize(int width, int height)
            {
                synchronized(mSurfaceHolder) {
                    dispWidth = width;
                    dispHeight = height;
                }
            }

            private Bitmap MakeFpsOverlay(Paint p, string text)
            {
                var b = new Rect();
                p.GetTextBounds(text, 0, text.Length, b);
                var bwidth = b.Width() - 2;
                var bheight = b.Height() - 2;
                Bitmap bm = Bitmap.CreateBitmap(bwidth, bheight,
                        Bitmap.Config.Argb8888);
                var c = new Canvas(bm);
                p.SetColor(overlayBackgroundColor);
                c.DrawRect(0, 0, bwidth, bheight, p);
                p.setColor(overlayTextColor);
                c.DrawText(text, -b.Left + 1,
                        (bheight / 2) - ((p.Ascent() + p.Descent()) / 2) + 1, p);
                return bm;
            }

            public void run()
            {
                start = System.currentTimeMillis();
                Log.Debug("MjpegView", "playback thread started! time:" + start);
                var mode = new PorterDuffXfermode(PorterDuff.Mode.DstOver);
                Bitmap bm;
                int width;
                int height;
                Rect destRect;
                Canvas c = null;
                var p = new Paint();
                var fps = "";
                while (mRun)
                {
                    if (surfaceDone)
                    {
                        try
                        {
                            if (false) Log.Debug("MjpegView", "thread run once++++");
                            c = mSurfaceHolder.LockCanvas();
                            synchronized(mSurfaceHolder) {
                                try
                                {
                                    if (mIn == null && mInUrl != null)
                                    {
                                        mIn = MjpegInputStream.Read(mInUrl);
                                    }

                                    bm = mIn.ReadMjpegFrame();

                                    if (mtakePic)
                                    {
                                        Log.Debug("MjpegView", "thread run start to take picture");
                                        var fName = GenerateFileName();
                                        Log.Debug("MjpegView", "mtakePic  " + fName);
                                        int res = SaveBitmapToFile(bm, fName);
                                        BroardCastResult(res, fName);
                                        mFileName = fName;
                                        mtakePic = false;
                                    }
                                    destRect = DestRect(mScreenWidth, mScreenHeight);
                                    c.DrawColor(Color.Black);
                                    c.DrawBitmap(bm, null, destRect, p);
                                    if (showFps)
                                    {
                                        p.SetXfermode(mode);
                                        if (ovl != null)
                                        {
                                            height = ((ovlPos & 1) == 1)
                                                ? destRect.Top
                                                : destRect.Bottom
                                                  - ovl.Height;
                                            width = ((ovlPos & 8) == 8)
                                                ? destRect.Left
                                                : destRect.Right
                                                  - ovl.Width;
                                            c.DrawBitmap(ovl, width, height, null);
                                        }
                                        p.SetXfermode(null);
                                        frameCounter++;
                                        if ((System.CurrentTimeMillis() - start) >= 1000)
                                        {
                                            fps = $"{frameCounter} fps";
                                            frameCounter = 0;
                                            start = System.CurrentTimeMillis();
                                            ovl = MakeFpsOverlay(overlayPaint, fps);
                                        }
                                    }
                                }
                                catch (Java.Lang.Exception)
                                {
                                }
                                catch (System.Exception)
                                {
                                }
                            }
                        }
                        finally
                        {
                            if (c != null)
                                mSurfaceHolder.UnlockCanvasAndPost(c);
                        }
                    }
                }
            }
        }
        
        public void StartPlayback()
        {
            if (mIn != null || mInUrl != null)
            {
                mRun = true;
                try
                {
                    thread.Start();
                }
                catch (IllegalThreadStateException e)
                {
                    Log.Error("MjpegView", "ERROR! " + e.Message);
                }
            }
        }

        public void ResumePlayback()
        {
            mRun = true;
            Init(context);
            Log.Debug("AppLog", "resume");
            thread.Start();
        }

        public void StopPlayback()
        {
            mRun = false;
            bool retry = true;
            while (retry)
            {
                try
                {
                    thread.Join();
                    retry = false;
                }
                catch (InterruptedException e)
                {
                }
            }
        }

        public void ShowFps(bool b)
        {
            showFps = b;
        }

        public void SetSource(MjpegInputStream source)
        {
            mIn = source;
            mInUrl = null;
            StartPlayback();
        }

        public void SetSource(String url)
        {
            mInUrl = url;
            mIn = null;
            StartPlayback();
        }

        public void SetOverlayPaint(Paint p)
        {
            overlayPaint = p;
        }

        public void SetOverlayTextColor(int c)
        {
            overlayTextColor = c;
        }

        public void SetOverlayBackgroundColor(int c)
        {
            overlayBackgroundColor = c;
        }

        public void SetOverlayPosition(int p)
        {
            ovlPos = p;
        }

        public void SetDisplayMode(int s)
        {
            displayMode = s;
        }

        public void SaveBitmap()
        {
            if (mRun)
            {
                Log.Debug("MjpegView", "saveBitmap start!");
                mtakePic = true;
            }
            else {
                Log.Debug("MjpegView", "saveBitmap error, not running!");
            }
        }

        private static string GenerateFileName()
        {
            File sdcard;
            bool sdCardExist = Environment.getExternalStorageState().equals(android.os.Environment.MEDIA_MOUNTED);
            if (sdCardExist)
            {
                sdcard = Environment.getExternalStorageDirectory();//获取跟目录 
            }
            else {
                return null;
            }

            String save2Dir = sdcard.ToString() + "/" + SAVE_TO_DIR;

            File fSave2Dir = new File(save2Dir);
            if (!fSave2Dir.exists())
            {
                fSave2Dir.mkdir();
            }
            
            var save2File = string.Format("{0}/{1}.png", save2Dir, DateTime.Now.ToString("yyyyMMddHHmmss"));

            var fSave2File = new File(save2File);
            if (fSave2File.Exists())
            {
                return save2Dir + "/" + str + System.currentTimeMillis() + ".png";
            }

            return save2File;
        }

        private static void BroardCastResult(int res, String fName)
        {
            Log.Debug("MjpegView", "BroardCastResult res: " + res);
            Intent intent = new Intent(Constant.ACTION_TAKE_PICTURE_DONE);
            intent.putExtra(Constant.EXTRA_RES, res);
            intent.putExtra(Constant.EXTRA_PATH, fName);
            context.sendBroadcast(intent);
        }

        private static int SaveBitmapToFile(Bitmap mBitmap, String bitName)
        {
            FileOutputStream fOut = null;
            Log.Debug("MjpegView", "SaveBitmapToFile enter");
            if (null == bitName || bitName.length() <= 4)
            {
                return Constant.CAM_RES_FAIL_FILE_NAME_ERROR;
            }

            File f = new File(bitName);
            Log.Debug("MjpegView", "SaveBitmapToFile, fname =" + f);
            try
            {
                f.createNewFile();
                Log.Debug("MjpegView", "SaveBitmapToFile, createNewFile success, f=" + f);
                fOut = new FileOutputStream(f);
                Log.Debug("MjpegView", "SaveBitmapToFile, FileOutputStream success, fOut=" + fOut);
            }
            catch (Exception e)
            {
                Log.Debug("MjpegView", "exception, err=" + e.getMessage());
                return Constant.CAM_RES_FAIL_FILE_WRITE_ERROR;
            }

            mBitmap.compress(Bitmap.CompressFormat.PNG, 100, fOut);

            try
            {
                fOut.flush();
                fOut.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                return Constant.CAM_RES_FAIL_BITMAP_ERROR;
            }

            return Constant.CAM_RES_OK;
        }
    }
}

/*    

public class MjpegView extends SurfaceView implements SurfaceHolder.Callback {

    


}
*/

/*
package com.hanry;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;

import com.hanry.Constant;

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.PorterDuff;
import android.graphics.PorterDuffXfermode;
import android.graphics.Rect;
import android.graphics.Typeface;
import android.os.Environment;
import android.util.AttributeSet;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.SurfaceHolder;
import android.view.SurfaceView;


public class MjpegView extends SurfaceView implements SurfaceHolder.Callback {
    public final static int POSITION_UPPER_LEFT = 9;
    public final static int POSITION_UPPER_RIGHT = 3;
    public final static int POSITION_LOWER_LEFT = 12;
    public final static int POSITION_LOWER_RIGHT = 6;

    public final static int SIZE_STANDARD = 1;
    public final static int SIZE_BEST_FIT = 4;
    public final static int SIZE_FULLSCREEN = 8;
    private final static String SAVE_TO_DIR = "mjpegview";
    
    private MjpegViewThread thread;
    private MjpegInputStream mIn = null;
    private String mInUrl = null;
    private boolean showFps = true;
    private boolean mRun = false;
    private boolean surfaceDone = false;
    private Paint overlayPaint;
    private int overlayTextColor;
    private int overlayBackgroundColor;
    private int ovlPos;
    private int dispWidth;
    private int dispHeight;
    private int displayMode;
    private static int mScreenWidth; 
    private static int mScreenHeight;
    private boolean resume = false;
    private boolean mtakePic = false;//flag for take a picture
    private String mFileName = null;//file name to save picture
    
    private Context context;

    public class MjpegViewThread extends Thread {
        private SurfaceHolder mSurfaceHolder;
        private int frameCounter = 0;
        private long start;
        private Bitmap ovl;

        public MjpegViewThread(SurfaceHolder surfaceHolder, Context context) {
            mSurfaceHolder = surfaceHolder;
        }

        private Rect destRect(int bmw, int bmh) {
            int tempx;
            int tempy;
            if (displayMode == MjpegView.SIZE_STANDARD) {
                tempx = (dispWidth / 2) - (bmw / 2);
                tempy = (dispHeight / 2) - (bmh / 2);
                return new Rect(tempx, tempy, bmw + tempx, bmh + tempy);
            }
            if (displayMode == MjpegView.SIZE_BEST_FIT) {
                float bmasp = (float) bmw / (float) bmh;
                bmw = dispWidth;
                bmh = (int) (dispWidth / bmasp);
                if (bmh > dispHeight) {
                    bmh = dispHeight;
                    bmw = (int) (dispHeight * bmasp);
                }
                tempx = (dispWidth / 2) - (bmw / 2);
                tempy = (dispHeight / 2) - (bmh / 2);
                return new Rect(tempx, tempy, bmw + tempx, bmh + tempy);
            }
            if (displayMode == MjpegView.SIZE_FULLSCREEN)
                return new Rect(0, 0, dispWidth, dispHeight);
            return null;
        }

        public void setSurfaceSize(int width, int height) {
            synchronized (mSurfaceHolder) {
                dispWidth = width;
                dispHeight = height;
            }
        }

        private Bitmap makeFpsOverlay(Paint p, String text) {
            Rect b = new Rect();
            p.getTextBounds(text, 0, text.length(), b);
            int bwidth = b.width() - 2;
            int bheight = b.height() - 2;
            Bitmap bm = Bitmap.createBitmap(bwidth, bheight,
                    Bitmap.Config.ARGB_8888);
            Canvas c = new Canvas(bm);
            p.setColor(overlayBackgroundColor);
            c.drawRect(0, 0, bwidth, bheight, p);
            p.setColor(overlayTextColor);
            c.drawText(text, -b.left + 1,
                    (bheight / 2) - ((p.ascent() + p.descent()) / 2) + 1, p);
            return bm;
        }

        public void run() {
            start = System.currentTimeMillis();
            Log.Debug("MjpegView", "playback thread started! time:" + start);
            PorterDuffXfermode mode = new PorterDuffXfermode(
                    PorterDuff.Mode.DST_OVER);
            Bitmap bm;
            int width;
            int height;
            Rect destRect;
            Canvas c = null;
            Paint p = new Paint();
            String fps = "";
            while (mRun) {
                if (surfaceDone) {
                    try {
                    	if (false) Log.Debug("MjpegView", "thread run once++++");
                        c = mSurfaceHolder.lockCanvas();
                        synchronized (mSurfaceHolder) {
                            try {
                            	if (mIn == null && mInUrl != null) {
                            		mIn = MjpegInputStream.read(mInUrl);
                            	}
                            	
                                bm = mIn.ReadMjpegFrame();
                                
                                if (mtakePic) {
                                	Log.Debug("MjpegView", "thread run start to take picture");
                                	String fName = generateFileName();
                                	Log.Debug("MjpegView", "mtakePic  " + fName);
                                	int res = saveBitmapToFile(bm, fName);
                                	BroardCastResult(res, fName);
                                	mFileName = fName;
                                	mtakePic = false;
                                }
                                destRect = destRect(mScreenWidth, mScreenHeight);
                                c.drawColor(Color.BLACK);
                                c.drawBitmap(bm, null, destRect, p);
                                if (showFps) {
                                    p.setXfermode(mode);
                                    if (ovl != null) {
                                        height = ((ovlPos & 1) == 1) ? destRect.top
                                                : destRect.bottom
                                                        - ovl.getHeight();
                                        width = ((ovlPos & 8) == 8) ? destRect.left
                                                : destRect.right
                                                        - ovl.getWidth();
                                        c.drawBitmap(ovl, width, height, null);
                                    }
                                    p.setXfermode(null);
                                    frameCounter++;
                                    if ((System.currentTimeMillis() - start) >= 1000) {
                                        fps = String.valueOf(frameCounter)
                                                + "fps";
                                        frameCounter = 0;
                                        start = System.currentTimeMillis();
                                        ovl = makeFpsOverlay(overlayPaint, fps);
                                    }
                                }
                            } catch (Exception e) {
                            }
                        }
                    } finally {
                        if (c != null)
                            mSurfaceHolder.unlockCanvasAndPost(c);
                    }
                }
            }
        }
    }

    private void init(Context context) {

        this.context = context;
        SurfaceHolder holder = getHolder();
        holder.addCallback(this);
        thread = new MjpegViewThread(holder, context);
        setFocusable(true);
        if (!resume) {
            resume = true;
            overlayPaint = new Paint();
            overlayPaint.setTextAlign(Paint.Align.LEFT);
            overlayPaint.setTextSize(12);
            overlayPaint.setTypeface(Typeface.DEFAULT);
            overlayTextColor = Color.WHITE;
            overlayBackgroundColor = Color.BLACK;
            ovlPos = MjpegView.POSITION_LOWER_RIGHT;
            displayMode = MjpegView.SIZE_STANDARD;
            dispWidth = getWidth();
            dispHeight = getHeight();

            Log.Debug("MjpegView", "init successfully!");
        }
        setOverlayTextColor(Color.GREEN);
        DisplayMetrics dm = getResources().getDisplayMetrics(); 
        mScreenWidth = dm.widthPixels; 
        mScreenHeight = dm.heightPixels;
        setKeepScreenOn(true);
    }

    public void startPlayback() {
        if (mIn != null || mInUrl != null) {
            mRun = true;
            try {
            	thread.start();
            } catch (IllegalThreadStateException e) {
            	Log.e("MjpegView", "ERROR! " + e.getMessage());
            }
        }
    }

    public void resumePlayback() {
        mRun = true;
        init(context);
        Log.Debug("AppLog", "resume");
        thread.start();
    }

    public void stopPlayback() {
        mRun = false;
        boolean retry = true;
        while (retry) {
            try {
                thread.join();
                retry = false;
            } catch (InterruptedException e) {
            }
        }
    }

    public MjpegView(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }

    public void surfaceChanged(SurfaceHolder holder, int f, int w, int h) {
        thread.setSurfaceSize(w, h);
    }

    public void surfaceDestroyed(SurfaceHolder holder) {
        surfaceDone = false;
        stopPlayback();
    }

    public MjpegView(Context context) {
        super(context);
        init(context);
    }

    public void surfaceCreated(SurfaceHolder holder) {
        surfaceDone = true;
    }

    public void showFps(boolean b) {
        showFps = b;
    }

    public void setSource(MjpegInputStream source) {
        mIn = source;
        mInUrl = null; 
        startPlayback();
    }

    public void setSource(String url) {
    	mInUrl = url;
    	mIn = null;
        startPlayback();
    }
    
    public void setOverlayPaint(Paint p) {
        overlayPaint = p;
    }

    public void setOverlayTextColor(int c) {
        overlayTextColor = c;
    }

    public void setOverlayBackgroundColor(int c) {
        overlayBackgroundColor = c;
    }

    public void setOverlayPosition(int p) {
        ovlPos = p;
    }

    public void setDisplayMode(int s) {
        displayMode = s;
    }
    
    public void saveBitmap () {
    	if(mRun) {
            Log.Debug("MjpegView", "saveBitmap start!" );
    		mtakePic = true;
    	} else {
    		Log.Debug("MjpegView", "saveBitmap error, not running!" );
    	}
    }
    
    private String generateFileName() {
    	File sdcard;
    	boolean sdCardExist = Environment.getExternalStorageState().equals(android.os.Environment.MEDIA_MOUNTED);
    	if (sdCardExist) 
    	{ 
    		sdcard = Environment.getExternalStorageDirectory();//获取跟目录 
    	} else {
    		return null;
    	}
    	
    	String save2dir = sdcard.toString() + "/" + SAVE_TO_DIR;
    	
    	File fSave2dir  = new File(save2dir);
    	//判断文件夹是否存在,如果不存在则创建文件夹
    	if (!fSave2dir.exists()) {
    		fSave2dir.mkdir();
    	}
    	
    	SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMddHHmmss");
    	Date curDate = new Date(System.currentTimeMillis());//get current time
    	String str = formatter.format(curDate);
    	
    	String save2file = save2dir + "/" + str + ".png";
    	
    	File fSave2file = new File(save2file);
    	if(fSave2file.exists()) {
            return save2dir + "/" + str + System.currentTimeMillis() + ".png";
    	}
    	
    	return save2file;
    }
    
    private void BroardCastResult (int res, String fName) {
    	Log.Debug("MjpegView", "BroardCastResult res: " + res);
    	Intent intent = new Intent(Constant.ACTION_TAKE_PICTURE_DONE);
    	intent.putExtra(Constant.EXTRA_RES, res);
    	intent.putExtra(Constant.EXTRA_PATH, fName);
    	context.sendBroadcast(intent);
    }
    
    private int saveBitmapToFile(Bitmap mBitmap, String bitName) {
    	FileOutputStream fOut = null;
    	Log.Debug("MjpegView", "saveBitmapToFile enter");
    	if (null == bitName || bitName.length() <= 4) {
    		return Constant.CAM_RES_FAIL_FILE_NAME_ERROR;
    	}
    	
    	File f = new File(bitName);
    	Log.Debug("MjpegView", "saveBitmapToFile, fname =" + f);
    	try {
	    	f.createNewFile();
	    	Log.Debug("MjpegView", "saveBitmapToFile, createNewFile success, f=" + f);
	    	fOut = new FileOutputStream(f);
	    	Log.Debug("MjpegView", "saveBitmapToFile, FileOutputStream success, fOut=" + fOut);
    	} catch (Exception e) {
    		Log.Debug("MjpegView", "exception, err=" + e.getMessage());
    		return Constant.CAM_RES_FAIL_FILE_WRITE_ERROR;
    	}
    	
    	mBitmap.compress(Bitmap.CompressFormat.PNG, 100, fOut);
    	
    	try {
    		fOut.flush();
    		fOut.close();
    	} catch (Exception e) {
    		e.printStackTrace();
    		return Constant.CAM_RES_FAIL_BITMAP_ERROR;
    	}
    	
    	return Constant.CAM_RES_OK;
    }
}
*/
