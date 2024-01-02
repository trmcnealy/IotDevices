using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Webcam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage _image;
        private readonly WebCam _webcam;

        public MainWindow()
        {
            InitializeComponent();

            _image = new BitmapImage();

            StreamCapture del = StreamDelegateCallback;
            //CameraCapture del = CameraDelegateCallback;

            _webcam = new WebCam(streamDelegate: del);
        }

        public void StreamDelegateCallback(MemoryStream ms)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    _image = new BitmapImage();
                    _image.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    _image.StreamSource = ms;
                    _image.EndInit();
                    WebCamControl.Source = _image;
                }));
        }

        public void CameraDelegateCallback(OpenCvSharp.Mat mat)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    WebCamControl.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(mat, 144.0, 144.0, PixelFormats.Bgr24, null);
                }));
        }
    }
}