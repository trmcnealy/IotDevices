using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Mcp25xxx.Register;

namespace RaspberryPiDevices;
public static class I2CScanner
{
    private const int BusId = 1;
    private const int FirstAddress = 0x08;
    private const int LastAddress = 0x80;

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public static List<byte> ScanDeviceAddresses(CancellationToken token)
    {
        List<byte> validAddresses = new List<byte>(LastAddress - FirstAddress + 1);

        foreach (byte address in ScanDeviceAddress(token).ToBlockingEnumerable())
        {
            validAddresses.Add(address);
        }

        return validAddresses;
    }

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    private static async IAsyncEnumerable<byte> ScanDeviceAddress([EnumeratorCancellation] CancellationToken token)
    {
        for (byte address = FirstAddress; address < LastAddress; ++address)
        {
            if (await ScanDeviceAddress(token, address))
            {
                yield return address;
            }
        }
    }

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    private static async Task<bool> ScanDeviceAddress(CancellationToken token, byte address)
    {
        try
        {
            await Task.Run(() =>
            {
                I2cDevice i2c = I2cDevice.Create(new I2cConnectionSettings(BusId, address));
                byte read = i2c.ReadByte();
                i2c.Dispose();
            }).WaitAsync(token);

            return true;
        }
        catch (IOException)
        {
            // No device found.
            return false;
        }
    }
}
