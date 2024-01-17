using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryPiDevices
{
    public class SerialPortController : IDisposable
    {
        private static readonly int[] BaudRates = new int[] { 256000, 128000, 115200, 57600, 38400, 19200, 14400, 9600, 4800, 2400, 1200, 600, 300, 110 };

        private bool disposedValue;

        private SerialPort sp;

        public SerialPortController(string portName)
        {
            sp = new SerialPort(portName);

            sp.Encoding = Encoding.UTF8;
            sp.ReadTimeout = 1000;
            sp.WriteTimeout = 1000;

            foreach (int baudRate in BaudRates)
            {
                sp.BaudRate = baudRate;
                sp.Open();

                if (sp.IsOpen)
                {
                    break;
                }
            }
        }

        public SerialPortController(string portName, int baudRate)
        {
            sp = new SerialPort(portName);

            sp.Encoding = Encoding.UTF8;
            sp.ReadTimeout = 1000;
            sp.WriteTimeout = 1000;
            sp.BaudRate = baudRate;
            sp.Open();

            if (!sp.IsOpen)
            {
                throw new Exception("Error:Baud Rate");
            }
        }

        ~SerialPortController()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    sp.Dispose();
                    // managed objects
                }

                // unmanaged objects
                disposedValue = true;
            }
        }

        public void Start()
        {
            bool finished = false;

            Console.CancelKeyPress += (a, b) =>
            {
                finished = true;
                sp.Close();
            };

            while (!finished)
            {
                string? line = Console.ReadLine();

                if (line is object && line == "!q")
                {
                    break;
                }

                try
                {
                    sp.WriteLine(line);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("ERROR: Sending command timed out");
                }

                if (finished)
                {
                    break;
                }

                // if RATE is set to really high Arduino may fail to respond in time
                // then on the next command you might get an old message
                // ReadExisting will read everything from the internal buffer

                string existingData = sp.ReadExisting();

                Console.Write(existingData);

                if (!existingData.Contains('\n') && !existingData.Contains('\r'))
                {
                    // we didn't get the response yet, let's wait for it then
                    try
                    {
                        Console.WriteLine(sp.ReadLine());
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine($"ERROR: No response in {sp.ReadTimeout}ms.");
                    }
                }
            }
        }


    }
}
