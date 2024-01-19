using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Graphics.SkiaSharpAdapter;

namespace RaspberryPiDevices;

public static class Module
{
    [ModuleInitializer]
    public static void Initializer()
    {
        SkiaSharpAdapter.Register();
    }
}
