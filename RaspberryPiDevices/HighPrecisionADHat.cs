using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Iot.Device.Board;

using Iot.Device.Display;

using Iot.Device.Nmea0183.Ais;

using static System.Runtime.InteropServices.JavaScript.JSType;

using UnitsNet;

namespace RaspberryPiDevices;

/// <summary>
/// Overview
/// High-Precision AD HAT For Raspberry Pi, ADS1263 10-Ch 32-Bit ADC.
/// 
/// Specification
/// Resolution (Bits): 32
/// Input channels: 10
/// Sample rate (MAX): 38 kSPS
/// PGA Magnification (MAX): 32
/// BUS: SPI
/// Structure: Delta-Sigma
/// Input type: Differential, Single-end
/// Reference voltage: Internal, External
/// Input voltage range (MAX): 2.5V, 5V
/// Input voltage range (MIN): -2.5V, 0V
/// 
/// Features
/// Adopts ADS1263 chip, low noise, low-temperature drift, 10-ch 32-bit high precision ADC (5-ch differential input), 38.4kSPS Max sampling rate.
/// with embedded 24-bit auxiliary ADC, internal ADC test signal, IDAC, 2.5V internal reference voltage, 8 x multiplexing GPIO, PGA (32 times Max).
/// Onboard AD header input, compatible with Waveshare sensor pinout, for connecting sorts of sensor modules.
/// Onboard AD screw terminal input, allows connecting analog signal and analog power supply, general-purpose interface.
/// Onboard control header, make it easy to control the module by other hosts in addition to Raspberry Pi.
/// Three-wire RTD (resistor temperature detector) circuit, enabled by soldering 0R resistor.
/// 
/// Pinout
/// PIN     Raspberry Pi(BCM)   Raspberry Pi(WiringPi)  Description
/// DRDY    P17	                P0	                    ADS1263 data output ready, low active
/// RESET	P18	                P1	                    ADS1263 reset input
/// CS      P22	                P3	                    ADS1263 chip select, low active
/// DIN	    P10	                P12	                    SPI data input
/// DOUT	P9	                P13	                    SPI data output
/// SCK	    P11	                P14	                    SPI clock
/// SCK	    P11	                P14	                    SPI clock
/// DIN0	D6	                P22	                    Digits input signal
/// DIN1	D13	                P23	                    Digits input signal
/// DIN2	D19	                P24	                    Digits input signal
/// DIN3	D26	                P25	                    Digits input signal
/// </summary>
public class HighPrecisionADHat
{
}
