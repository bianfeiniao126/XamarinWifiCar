using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Org.Apache.Http.Client;

namespace XamarinWifiCarApp.MpegViewerTranslated
{
    public class MjpegInputStream : DataInputStream
    {
        private byte[] SOI_MARKER = { (byte)0xFF, (byte)0xD8 };
        private byte[] EOF_MARKER = { (byte)0xFF, (byte)0xD9 };
        private string CONTENT_LENGTH = "Content-Length";
        private static int HEADER_MAX_LENGTH = 100;
        private static int FRAME_MAX_LENGTH = 40000 + HEADER_MAX_LENGTH;
        private int mContentLength = -1;

        public MjpegInputStream(Stream inputStream) : base(new BufferedStream(inputStream, FRAME_MAX_LENGTH))
        {
        }

        public static MjpegInputStream Read(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                var response = request.GetResponse();
                return new MjpegInputStream(response.GetResponseStream());
            }
            catch (ClientProtocolException) { }
            catch (System.IO.IOException) { }
            return null;
        }

        private int GetEndOfSeqeunce(DataInputStream stream, byte[] sequence)// throws IOException
        {
            var seqIndex = 0;
            for (int i = 0; i < FRAME_MAX_LENGTH; i++)
            {
                var c = (byte)stream.ReadByte();
                if (c == sequence[seqIndex])
                {
                    seqIndex++;
                    if (seqIndex == sequence.Length)
                    {
                        return i + 1;
                    }
                }
                else
                {
                    seqIndex = 0;
                }
            }
            return -1;
        }
        private int GetStartOfSequence(DataInputStream stream, byte[] sequence)// throws IOException
        {
            var end = GetEndOfSeqeunce(stream, sequence);
            return (end < 0) ? (-1) : (end - sequence.Length);
        }
        private int ParseContentLength(byte[] headerBytes)// throws IOException, NumberFormatException {
        {
            Stream headerIn = new MemoryStream(headerBytes);
            //Properties props = new Properties();
            //props.load(headerIn);
            //return int.parseInt(props.getProperty(CONTENT_LENGTH));
            //TODO: depurar esto
            return int.Parse(headerIn.Length.ToString());
        }
        public Bitmap ReadMjpegFrame()// throws IOException
        {
            Mark(FRAME_MAX_LENGTH);
            var headerLen = GetStartOfSequence(this, SOI_MARKER);
            Reset();
            byte[]
            header = new byte[headerLen];
            ReadFully(header);
            try
            {
                mContentLength = ParseContentLength(header);
            }
            catch (NumberFormatException nfe)
            {
                mContentLength = GetEndOfSeqeunce(this, EOF_MARKER);
            }
            Reset();
            byte[] frameData = new byte[mContentLength];
            SkipBytes(headerLen);
            ReadFully(frameData);
            return BitmapFactory.DecodeStream(new MemoryStream(frameData));
        }
    }
}




/*
package com.hanry;

import java.io.BufferedInputStream;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.util.Properties;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

public class MjpegInputStream extends DataInputStream {
    private final byte[] SOI_MARKER = { (byte) 0xFF, (byte) 0xD8 };
    private final byte[] EOF_MARKER = { (byte) 0xFF, (byte) 0xD9 };
    private final String CONTENT_LENGTH = "Content-Length";
    private final static int HEADER_MAX_LENGTH = 100;
    private final static int FRAME_MAX_LENGTH = 40000 + HEADER_MAX_LENGTH;
    private int mContentLength = -1;
        
    public static MjpegInputStream read(String url) {
        HttpResponse res;
        DefaultHttpClient httpclient = new DefaultHttpClient();         
        try {
            res = httpclient.execute(new HttpGet(URI.create(url)));
            return new MjpegInputStream(res.getEntity().getContent());                          
        } catch (ClientProtocolException e) {
        } catch (IOException e) {}
        return null;
    }
        
    public MjpegInputStream(InputStream in) { super(new BufferedInputStream(in, FRAME_MAX_LENGTH)); }
        
    private int getEndOfSeqeunce(DataInputStream in, byte[] sequence) throws IOException {
        int seqIndex = 0;
        byte c;
        for(int i=0; i < FRAME_MAX_LENGTH; i++) {
            c = (byte) in.readUnsignedByte();
            if(c == sequence[seqIndex]) {
                seqIndex++;
                if(seqIndex == sequence.length) return i + 1;
            } else seqIndex = 0;
        }
        return -1;
    }
        
    private int getStartOfSequence(DataInputStream in, byte[] sequence) throws IOException {
        int end = getEndOfSeqeunce(in, sequence);
        return (end < 0) ? (-1) : (end - sequence.length);
    }

    private int parseContentLength(byte[] headerBytes) throws IOException, NumberFormatException {
        ByteArrayInputStream headerIn = new ByteArrayInputStream(headerBytes);
        Properties props = new Properties();
        props.load(headerIn);
        return Integer.parseInt(props.getProperty(CONTENT_LENGTH));
    }   

    public Bitmap ReadMjpegFrame() throws IOException {
        mark(FRAME_MAX_LENGTH);
        int headerLen = getStartOfSequence(this, SOI_MARKER);
        reset();
        byte[] header = new byte[headerLen];
        readFully(header);
        try {
            mContentLength = parseContentLength(header);
        } catch (NumberFormatException nfe) { 
            mContentLength = getEndOfSeqeunce(this, EOF_MARKER); 
        }
        reset();
        byte[] frameData = new byte[mContentLength];
        skipBytes(headerLen);
        readFully(frameData);
        return BitmapFactory.decodeStream(new ByteArrayInputStream(frameData));
    }
}
    */
