using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using OpenCvSharp.Dnn;

namespace Webcam
{
    public delegate void BitmapCapture(System.Drawing.Bitmap bitmap);
    
    public delegate void StreamCapture(MemoryStream stream);

    public delegate void CameraCapture(Mat mat);

    public sealed class WebCam
    {
        const int FrameWidth = 3840;
        const int FrameHeight = 2160;
        const double dpiX = 144.0;
        const double dpiY = 144.0;
        const double Fps = 30.0;

        private Thread _camera;
        private volatile bool _isCameraRunning = false;

        private readonly VideoCapture _capture;
        private readonly Mat _frame;                
        private readonly FrameSource _frameSource;

        private readonly BitmapCapture? _bitmapDelegate;
        private readonly StreamCapture? _streamDelegate;
        private readonly CameraCapture? _cameraDelegate;

        private readonly CancellationTokenSource cts;

        public WebCam(bool autoActivate = true, CameraCapture? cameraDelegate = null, BitmapCapture? bitmapDelegate = null, StreamCapture? streamDelegate = null)
        {
            _frame = new Mat();
            _capture = new VideoCapture();

            _frameSource = OpenCvSharp.Cv2.CreateFrameSource_Camera(0);
            

            cts = new CancellationTokenSource();

            _bitmapDelegate = bitmapDelegate;
            _streamDelegate = streamDelegate;
            _cameraDelegate = cameraDelegate;

            _camera = new Thread(CaptureCameraCallback)
            {
                IsBackground = true
            };

            //cts.Token.Register(() =>
            //{
            //    _camera.c
            //});


            if (autoActivate)
            {

                _camera.Start();
                _isCameraRunning = true;
            }
        }

        ~WebCam()
        {
            //_camera.Abort();
            _capture.Release();
            _isCameraRunning = false;

            _capture.Dispose();
            _frame.Dispose();
        }

        public void Cancel()
        {
            cts.Token.ThrowIfCancellationRequested();
        }

        private void CaptureCameraCallback()
        {
            if (!_isCameraRunning)
            {
                return;
            }

            //while (_isCameraRunning)
            //{
            //    _frameSource.NextFrame(_frame);
            //    _cameraDelegate?.Invoke(_frame);
            //}

            _capture.Open(0, VideoCaptureAPIs.DSHOW);

            if (_capture.IsOpened())
            {
                _capture.FrameWidth = FrameWidth;
                _capture.FrameHeight = FrameHeight;
                _capture.Fps = Fps;
                _capture.AutoFocus = true;

                while (_isCameraRunning)
                {
                    _capture.Read(_frame);

                    //_bitmapDelegate?.Invoke(_frame.ToBitmap());
                    _streamDelegate?.Invoke(_frame.ToMemoryStream());
                }
            }
        }

    }
}
