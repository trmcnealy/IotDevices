using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Ads1115;
using Iot.Device.Board;

using UnitsNet;

namespace RaspberryPiDevices.Hat;






/// <summary>
/// RESOLUTION	32 Bits
/// INPUT CHANNELS	10
/// SAMPLING RATE (MAX)	38.4 kSPS
/// PGA MAGNIFICATION (MAX)	32
/// BUS	SPI
/// STRUCTURE	Delta-Sigma
/// INPUT TYPE	differential, single-end
/// REFERENCE VOLTAGE	internal, external
/// INPUT VOLTAGE RANGE (MAX)	2.5V, 5V
/// INPUT VOLTAGE RANGE (MIN)	-2.5V, 0V
/// </summary>
public class Ads1263// : IDisposable
{
    #region Ads1263 Enums

    ///* gain channel*/
    //public enum ADS1263_GAIN
    //{
    public const int ADS1263_GAIN_1 = 0;    /*GAIN  1 */
    public const int ADS1263_GAIN_2 = 1;    /*GAIN  2 */
    public const int ADS1263_GAIN_4 = 2;    /*GAIN  4 */
    public const int ADS1263_GAIN_8 = 3;    /*GAIN  8 */
    public const int ADS1263_GAIN_16 = 4;    /*GAIN  16 */
    public const int ADS1263_GAIN_32 = 5;    /*GAIN  32 */
    public const int ADS1263_GAIN_64 = 6;    /*GAIN  64 */
    //}

    //public enum ADS1263_DRATE
    //{
    public const int ADS1263_2d5SPS = 0;
    public const int ADS1263_5SPS = 1;
    public const int ADS1263_10SPS = 2;
    public const int ADS1263_16d6SPS = 3;
    public const int ADS1263_20SPS = 4;
    public const int ADS1263_50SPS = 5;
    public const int ADS1263_60SPS = 6;
    public const int ADS1263_100SPS = 7;
    public const int ADS1263_400SPS = 8;
    public const int ADS1263_1200SPS = 9;
    public const int ADS1263_2400SPS = 10;
    public const int ADS1263_4800SPS = 11;
    public const int ADS1263_7200SPS = 12;
    public const int ADS1263_14400SPS = 13;
    public const int ADS1263_19200SPS = 14;
    public const int ADS1263_38400SPS = 15;
    //}

    //public enum ADS1263_DELAY
    //{
    public const int ADS1263_DELAY_0s = 0;
    public const int ADS1263_DELAY_8d7us = 1;
    public const int ADS1263_DELAY_17us = 2;
    public const int ADS1263_DELAY_35us = 3;
    public const int ADS1263_DELAY_169us = 4;
    public const int ADS1263_DELAY_139us = 5;
    public const int ADS1263_DELAY_278us = 6;
    public const int ADS1263_DELAY_555us = 7;
    public const int ADS1263_DELAY_1d1ms = 8;
    public const int ADS1263_DELAY_2d2ms = 9;
    public const int ADS1263_DELAY_4d4ms = 10;
    public const int ADS1263_DELAY_8d8ms = 11;
    //}

    //public enum ADS1263_ADC2_DRATE
    //{
    public const int ADS1263_ADC2_10SPS = 0;
    public const int ADS1263_ADC2_100SPS = 1;
    public const int ADS1263_ADC2_400SPS = 2;
    public const int ADS1263_ADC2_800SPS = 3;
    //}

    //public enum ADS1263_ADC2_GAIN
    //{
    public const int ADS1263_ADC2_GAIN_1 = 0;
    public const int ADS1263_ADC2_GAIN_2 = 1;
    public const int ADS1263_ADC2_GAIN_4 = 2;
    public const int ADS1263_ADC2_GAIN_8 = 3;
    public const int ADS1263_ADC2_GAIN_16 = 4;
    public const int ADS1263_ADC2_GAIN_32 = 5;
    public const int ADS1263_ADC2_GAIN_64 = 6;
    public const int ADS1263_ADC2_GAIN_128 = 7;
    //}

    //public enum ADS1263_DAC_VOLT
    //{
    public const int ADS1263_DAC_VLOT_4_5 = 0b01001;      //4.5V
    public const int ADS1263_DAC_VLOT_3_5 = 0b01000;
    public const int ADS1263_DAC_VLOT_3 = 0b00111;
    public const int ADS1263_DAC_VLOT_2_75 = 0b00110;
    public const int ADS1263_DAC_VLOT_2_625 = 0b00101;
    public const int ADS1263_DAC_VLOT_2_5625 = 0b00100;
    public const int ADS1263_DAC_VLOT_2_53125 = 0b00011;
    public const int ADS1263_DAC_VLOT_2_515625 = 0b00010;
    public const int ADS1263_DAC_VLOT_2_5078125 = 0b00001;
    public const int ADS1263_DAC_VLOT_2_5 = 0b00000;
    public const int ADS1263_DAC_VLOT_2_4921875 = 0b10001;
    public const int ADS1263_DAC_VLOT_2_484375 = 0b10010;
    public const int ADS1263_DAC_VLOT_2_46875 = 0b10011;
    public const int ADS1263_DAC_VLOT_2_4375 = 0b10100;
    public const int ADS1263_DAC_VLOT_2_375 = 0b10101;
    public const int ADS1263_DAC_VLOT_2_25 = 0b10110;
    public const int ADS1263_DAC_VLOT_2 = 0b10111;
    public const int ADS1263_DAC_VLOT_1_5 = 0b11000;
    public const int ADS1263_DAC_VLOT_0_5 = 0b11001;
    //}

    //public enum ADS1263_REG
    //{
    /*Register address; followed by reset the default values */
    public const int REG_ID = 0;    // xxh
    public const int REG_POWER = 1;      // 11h
    public const int REG_INTERFACE = 1;  // 05h
    public const int REG_MODE0 = 2;      // 00h
    public const int REG_MODE1 = 3;      // 80h
    public const int REG_MODE2 = 4;      // 04h
    public const int REG_INPMUX = 5;     // 01h
    public const int REG_OFCAL0 = 6;     // 00h
    public const int REG_OFCAL1 = 7;     // 00h
    public const int REG_OFCAL2 = 8;     // 00h
    public const int REG_FSCAL0 = 9;     // 00h
    public const int REG_FSCAL1 = 10;     // 00h
    public const int REG_FSCAL2 = 11;     // 40h
    public const int REG_IDACMUX = 12;    // BBh
    public const int REG_IDACMAG = 13;    // 00h
    public const int REG_REFMUX = 14;     // 00h
    public const int REG_TDACP = 15;      // 00h
    public const int REG_TDACN = 16;      // 00h
    public const int REG_GPIOCON = 17;    // 00h
    public const int REG_GPIODIR = 18;    // 00h
    public const int REG_GPIODAT = 19;    // 00h
    public const int REG_ADC2CFG = 20;    // 00h
    public const int REG_ADC2MUX = 21;    // 01h
    public const int REG_ADC2OFC0 = 22;   // 00h
    public const int REG_ADC2OFC1 = 23;   // 00h
    public const int REG_ADC2FSC0 = 24;   // 00h
    public const int REG_ADC2FSC1 = 25;   // 40h
                                          //}

    //public enum ADS1263_CMD
    //{
    public const int CMD_RESET = 0x06; // Reset the ADC; 0000 011x (06h or 07h)
    public const int CMD_START1 = 0x08; // Start ADC1 conversions; 0000 100x (08h or 09h)
    public const int CMD_STOP1 = 0x0A; // Stop ADC1 conversions; 0000 101x (0Ah or 0Bh)
    public const int CMD_START2 = 0x0C; // Start ADC2 conversions; 0000 110x (0Ch or 0Dh)
    public const int CMD_STOP2 = 0x0E; // Stop ADC2 conversions; 0000 111x (0Eh or 0Fh)
    public const int CMD_RDATA1 = 0x12; // Read ADC1 data; 0001 001x (12h or 13h)
    public const int CMD_RDATA2 = 0x14; // Read ADC2 data; 0001 010x (14h or 15h)
    public const int CMD_SYOCAL1 = 0x16; // ADC1 system offset calibration; 0001 0110 (16h)
    public const int CMD_SYGCAL1 = 0x17; // ADC1 system gain calibration; 0001 0111 (17h)
    public const int CMD_SFOCAL1 = 0x19; // ADC1 self offset calibration; 0001 1001 (19h)
    public const int CMD_SYOCAL2 = 0x1B; // ADC2 system offset calibration; 0001 1011 (1Bh)
    public const int CMD_SYGCAL2 = 0x1C; // ADC2 system gain calibration; 0001 1100 (1Ch)
    public const int CMD_SFOCAL2 = 0x1E; // ADC2 self offset calibration; 0001 1110 (1Eh)
    public const int CMD_RREG = 0x20; // Read registers 001r rrrr (20h+000r rrrr)
    public const int CMD_RREG2 = 0x00; // number of registers to read minus 1; 000n nnnn
    public const int CMD_WREG = 0x40; // Write registers 010r rrrr (40h+000r rrrr)
    public const int CMD_WREG2 = 0x00; // number of registers to write minus 1; 000n nnnn
    public const int CMD_PGA_BYPASSED = 0x80; //0x80:PGA bypassed; 0x00:PGA enabled
                                              //};

    #endregion

    public const int SPI_CS_HIGH = 0x04;                //Chip select high  
    public const int SPI_LSB_FIRST = 0x08;                //LSB  
    public const int SPI_3WIRE = 0x10;                //3-wire mode SI and SO same line
    public const int SPI_LOOP = 0x20;                //Loopback mode  
    public const int SPI_NO_CS = 0x40;                //A single device occupies one SPI bus, so there is no chip select 
    public const int SPI_READY = 0x80;                //Slave pull low to stop data transmission  

    public const int Positive_A6 = 1;
    public const int Negative_A7 = 0;
    public const int Open = 1;
    public const int Close = 0;

    private const int I2cBus = 1;

    private bool disposedValue;

    private int ScanMode = 0;

    public int DEV_RST_PIN;
    public int DEV_CS_PIN;
    public int DEV_DRDY_PIN;

    public int MOSI_PIN;
    public int MISO_PIN;
    public int SCLK_PIN;


    private readonly RaspberryPiBoard _board;
    //private readonly I2cDevice _i2cDevice;
    private readonly GpioController _gpioController;
    private readonly SpiDevice _spiDevice;


    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public Ads1263(RaspberryPiBoard raspberryPiBoard)
    {
        _board = raspberryPiBoard;

        _board.ReservePin(DEV_RST_PIN, PinUsage.Gpio, this);
        _board.ReservePin(DEV_CS_PIN, PinUsage.Gpio, this);
        _board.ReservePin(DEV_DRDY_PIN, PinUsage.Gpio, this);

        _board.ReservePin(MOSI_PIN, PinUsage.Spi, this);
        _board.ReservePin(MISO_PIN, PinUsage.Spi, this);
        _board.ReservePin(SCLK_PIN, PinUsage.Spi, this);

        //_i2cDevice = raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBus, (int)I2cAddress.GND));
        _gpioController = raspberryPiBoard.CreateGpioController();

        DEV_RST_PIN = 18;
        DEV_CS_PIN = 22;
        DEV_DRDY_PIN = 17;

        MOSI_PIN = 19;
        MISO_PIN = 20;
        SCLK_PIN = 23;

        DEV_GPIO_Mode(DEV_RST_PIN, PinMode.Output);
        DEV_GPIO_Mode(DEV_CS_PIN, PinMode.Output);
        DEV_GPIO_Mode(DEV_DRDY_PIN, PinMode.Input);

        DEV_Digital_Write(DEV_CS_PIN, 1);

        int busId = 0;
        int chipSelectLine = -1;

        _spiDevice = raspberryPiBoard.CreateSpiDevice(new SpiConnectionSettings(busId, chipSelectLine)
        {
            Mode = SpiMode.Mode1,
            DataBitLength = 24,
            ClockFrequency = 38_400,
            DataFlow = DataFlow.MsbFirst,
            ChipSelectLineActiveState = PinValue.Low
        });
    }



    #region Dctor
    ~Ads1263()
    {
        Dispose(disposing: false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // managed
                _board.ReservePin(DEV_RST_PIN, PinUsage.Gpio, this);
                _board.ReservePin(DEV_CS_PIN, PinUsage.Gpio, this);
                _board.ReservePin(DEV_DRDY_PIN, PinUsage.Gpio, this);

                _board.ReservePin(MOSI_PIN, PinUsage.Spi, this);
                _board.ReservePin(MISO_PIN, PinUsage.Spi, this);
                _board.ReservePin(SCLK_PIN, PinUsage.Spi, this);

                _board.Dispose();
            }

            // unmanaged
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    public byte[] SpiTransferArray(Span<byte> dataOut)
    {
        byte[] result = new byte[dataOut.Length];
        _spiDevice.TransferFullDuplex(dataOut, result);
        return result;
    }


    public void DEV_Digital_Write(int Pin, PinValue Value)
    {
        _gpioController.Write(Pin, Value);
    }

    public PinValue DEV_Digital_Read(int Pin)
    {
        PinValue Read_value = _gpioController.Read(Pin);

        return Read_value;
    }

    public int DEV_SPI_WriteByte(int Value)
    {
        Span<byte> dataOut = stackalloc byte[1];

        byte[] result = SpiTransferArray(dataOut);

        return result[0];
    }

    public int DEV_SPI_ReadByte()
    {
        return DEV_SPI_WriteByte(0x00);
    }

    public void DEV_Module_Exit()
    {
        DEV_Digital_Write(DEV_RST_PIN, 0);
        DEV_Digital_Write(DEV_CS_PIN, 0);
    }

    public void DEV_GPIO_Mode(int Pin, PinMode Mode)
    {
        _gpioController.SetPinMode(Pin, Mode);
    }

    public void DEV_Delay_ms(int xms)
    {

    }




    public void ADS1263_reset()
    {
        DEV_Digital_Write(DEV_RST_PIN, 1);
        Utilities.DelayMilliseconds(300);
        DEV_Digital_Write(DEV_RST_PIN, 0);
        Utilities.DelayMilliseconds(300);
        DEV_Digital_Write(DEV_RST_PIN, 1);
        Utilities.DelayMilliseconds(300);
    }

    public void ADS1263_WriteCmd(int Cmd)
    {
        DEV_Digital_Write(DEV_CS_PIN, 0);
        DEV_SPI_WriteByte(Cmd);
        DEV_Digital_Write(DEV_CS_PIN, 1);
    }


    public void ADS1263_WriteReg(int Reg, int data)
    {
        DEV_Digital_Write(DEV_CS_PIN, 0);
        DEV_SPI_WriteByte(CMD_WREG | Reg);
        DEV_SPI_WriteByte(0x00);
        DEV_SPI_WriteByte(data);
        DEV_Digital_Write(DEV_CS_PIN, 1);
    }


    public int ADS1263_Read_data(int Reg)
    {
        int temp = 0;
        DEV_Digital_Write(DEV_CS_PIN, 0);
        DEV_SPI_WriteByte(CMD_RREG | Reg);
        DEV_SPI_WriteByte(0x00);
        // Utilities.DelayMilliseconds(1);
        temp = DEV_SPI_ReadByte();
        DEV_Digital_Write(DEV_CS_PIN, 1);
        return temp;
    }


    public int ADS1263_Checksum(int val, int byt)
    {
        int sum = 0;
        int mask = -1;        // 8 bits mask, 0xff
        while (val != 0)
        {
            sum += val & mask;  // only add the lower values
            val >>= 8;          // shift down
        }
        sum += 0x9b;
        // Console.Write("--- %x %x--- \r\n", sum, byt);
        return sum ^ byt;       // if equal, this will be 0
    }

    /******************************************************************************
    function:   Waiting for a busy end
    parameter: 
    Info:
        Timeout indicates that the operation is not working properly.
    ******************************************************************************/
    public void ADS1263_WaitDRDY()
    {
        // Console.Write("ADS1263_WaitDRDY \r\n");
        int i = 0;
        while (true)
        {
            if (DEV_Digital_Read(DEV_DRDY_PIN) == 0)
            {
                break;
            }

            if (i >= 4000000)
            {
                Console.Write("Time Out ...\r\n");
                break;
            }
        }
        // Console.Write("ADS1263_WaitDRDY Release \r\n");
    }

    /******************************************************************************
    function:  Read device ID
    parameter: 
    Info:
    ******************************************************************************/
    private int ADS1263_ReadChipID()
    {
        int id;
        id = ADS1263_Read_data(REG_ID);
        return id >> 5;
    }

    /******************************************************************************
    function:  Setting mode
    parameter: 
        Mode : 0 Single-ended input
               1 channel1 Differential input
    Info:
    ******************************************************************************/
    private void ADS1263_SetMode(int Mode)
    {
        if (Mode == 0)
        {
            ScanMode = 0;
        }
        else
        {
            ScanMode = 1;
        }
    }

    /******************************************************************************
    function:  Configure ADC gain and sampling speed
    parameter: 
        gain : Enumeration type gain
        drate: Enumeration type sampling speed
    Info:
    ******************************************************************************/
    private void ADS1263_ConfigADC1(int gain, int drate, int delay)
    {
        int MODE2 = CMD_PGA_BYPASSED; //0x80:PGA bypassed, 0x00:PGA enabled

        MODE2 |= (gain << ADS1263_GAIN_16) | drate;

        ADS1263_WriteReg(REG_MODE2, MODE2);

        Utilities.DelayMilliseconds(1);

        if (ADS1263_Read_data(REG_MODE2) == MODE2)
        {
            Console.Write("REG_MODE2 success \r\n");
        }
        else
        {
            Console.Write("REG_MODE2 unsuccess \r\n");
        }

        int REFMUX = 0x24;        //0x00:+-2.5V as REF, 0x24:VDD,VSS as REF
        ADS1263_WriteReg(REG_REFMUX, REFMUX);
        Utilities.DelayMilliseconds(1);
        if (ADS1263_Read_data(REG_REFMUX) == REFMUX)
        {
            Console.Write("REG_REFMUX success \r\n");
        }
        else
        {
            Console.Write("REG_REFMUX unsuccess \r\n");
        }

        int MODE0 = delay;
        ADS1263_WriteReg(REG_MODE0, MODE0);
        Utilities.DelayMilliseconds(1);
        if (ADS1263_Read_data(REG_MODE0) == MODE0)
        {
            Console.Write("REG_MODE0 success \r\n");
        }
        else
        {
            Console.Write("REG_MODE0 unsuccess \r\n");
        }

        int MODE1 = 0x84; // Digital Filter; 0x84:FIR, 0x64:Sinc4, 0x44:Sinc3, 0x24:Sinc2, 0x04:Sinc1
        ADS1263_WriteReg(REG_MODE1, MODE1);
        Utilities.DelayMilliseconds(1);
        if (ADS1263_Read_data(REG_MODE1) == MODE1)
        {
            Console.Write("REG_MODE1 success \r\n");
        }
        else
        {
            Console.Write("REG_MODE1 unsuccess \r\n");
        }
    }

    /******************************************************************************
    function:  Configure ADC gain and sampling speed
    parameter: 
        gain : Enumeration type gain
        drate: Enumeration type sampling speed
    Info:
    ******************************************************************************/
    private void ADS1263_ConfigADC2(int gain, int drate, int delay)
    {
        int ADC2CFG = 0x20;               //REF, 0x20:VAVDD and VAVSS, 0x00:+-2.5V
        ADC2CFG |= (drate << 6) | gain;
        ADS1263_WriteReg(REG_ADC2CFG, ADC2CFG);
        Utilities.DelayMilliseconds(1);
        if (ADS1263_Read_data(REG_ADC2CFG) == ADC2CFG)
        {
            Console.Write("REG_ADC2CFG success \r\n");
        }
        else
        {
            Console.Write("REG_ADC2CFG unsuccess \r\n");
        }

        int MODE0 = delay;
        ADS1263_WriteReg(REG_MODE0, MODE0);
        Utilities.DelayMilliseconds(1);
        if (ADS1263_Read_data(REG_MODE0) == MODE0)
        {
            Console.Write("REG_MODE0 success \r\n");
        }
        else
        {
            Console.Write("REG_MODE0 unsuccess \r\n");
        }
    }

    /******************************************************************************
    function:  Device initialization
    parameter: 
    Info:
    ******************************************************************************/
    private int ADS1263_init_ADC1(int rate)
    {
        ADS1263_reset();
        if (ADS1263_ReadChipID() == 1)
        {
            Console.Write("ID Read success \r\n");
        }
        else
        {
            Console.Write("ID Read failed \r\n");
            return 1;
        }
        ADS1263_WriteCmd(CMD_STOP1);
        ADS1263_ConfigADC1(ADS1263_GAIN_1, rate, ADS1263_DELAY_35us);
        ADS1263_WriteCmd(CMD_START1);
        return 0;
    }

    private int ADS1263_init_ADC2(int rate)
    {
        ADS1263_reset();
        if (ADS1263_ReadChipID() == 1)
        {
            Console.Write("ID Read success \r\n");
        }
        else
        {
            Console.Write("ID Read failed \r\n");
            return 1;
        }
        ADS1263_WriteCmd(CMD_STOP2);
        ADS1263_ConfigADC2(ADS1263_ADC2_GAIN_1, rate, ADS1263_DELAY_35us);
        return 0;
    }

    /******************************************************************************
    function:  Set the channel to be read
    parameter: 
        Channal : Set channel number
    Info:
    ******************************************************************************/
    public void ADS1263_SetChannal(int Channal)
    {
        if (Channal > 10)
        {
            return;
        }
        int INPMUX = (Channal << 4) | 0x0a;       //0x0a:VCOM as Negative Input
        ADS1263_WriteReg(REG_INPMUX, INPMUX);
        if (ADS1263_Read_data(REG_INPMUX) == INPMUX)
        {
            // Console.Write("ADS1263_ADC1_SetChannal success \r\n");
        }
        else
        {
            Console.Write("ADS1263_ADC1_SetChannal unsuccess \r\n");
        }
    }

    /******************************************************************************
    function:  Set the channel to be read
    parameter: 
        Channal : Set channel number
    Info:
    ******************************************************************************/
    public void ADS1263_SetChannal_ADC2(int Channal)
    {
        if (Channal > 10)
        {
            return;
        }

        int INPMUX = (Channal << 4) | 0x0a;       //0x0a:VCOM as Negative Input

        ADS1263_WriteReg(REG_ADC2MUX, INPMUX);

        if (ADS1263_Read_data(REG_ADC2MUX) == INPMUX)
        {
            // Console.Write("ADS1263_ADC2_SetChannal success \r\n");
        }
        else
        {
            Console.Write("ADS1263_ADC2_SetChannal unsuccess \r\n");
        }
    }

    /******************************************************************************
    function:  Set the channel to be read
    parameter: 
        Channal : Set channel number
    Info:
    ******************************************************************************/
    private void ADS1263_SetDiffChannal(int Channal)
    {
        int INPMUX = 0;

        if (Channal == 0)
        {
            INPMUX = (0 << 4) | 1;    //DiffChannal   AIN0-AIN1
        }
        else if (Channal == 1)
        {
            INPMUX = (2 << 4) | 3;    //DiffChannal   AIN2-AIN3
        }
        else if (Channal == 2)
        {
            INPMUX = (4 << 4) | 5;    //DiffChannal   AIN4-AIN5
        }
        else if (Channal == 3)
        {
            INPMUX = (6 << 4) | 7;    //DiffChannal   AIN6-AIN7
        }
        else if (Channal == 4)
        {
            INPMUX = (8 << 4) | 9;    //DiffChannal   AIN8-AIN9
        }
        ADS1263_WriteReg(REG_INPMUX, INPMUX);
        if (ADS1263_Read_data(REG_INPMUX) == INPMUX)
        {
            // Console.Write("ADS1263_SetDiffChannal success \r\n");
        }
        else
        {
            Console.Write("ADS1263_SetDiffChannal unsuccess \r\n");
        }
    }

    /******************************************************************************
    function:  Set the channel to be read
    parameter: 
        Channal : Set channel number
    Info:
    ******************************************************************************/
    private void ADS1263_SetDiffChannal_ADC2(int Channal)
    {
        int INPMUX = 0;

        if (Channal == 0)
        {
            INPMUX = (0 << 4) | 1;    //DiffChannal   AIN0-AIN1
        }
        else if (Channal == 1)
        {
            INPMUX = (2 << 4) | 3;    //DiffChannal   AIN2-AIN3
        }
        else if (Channal == 2)
        {
            INPMUX = (4 << 4) | 5;    //DiffChannal   AIN4-AIN5
        }
        else if (Channal == 3)
        {
            INPMUX = (6 << 4) | 7;    //DiffChannal   AIN6-AIN7
        }
        else if (Channal == 4)
        {
            INPMUX = (8 << 4) | 9;    //DiffChannal   AIN8-AIN9
        }

        ADS1263_WriteReg(REG_ADC2MUX, INPMUX);

        if (ADS1263_Read_data(REG_ADC2MUX) == INPMUX)
        {
            // Console.Write("ADS1263_SetDiffChannal_ADC2 success \r\n");
        }
        else
        {
            Console.Write("ADS1263_SetDiffChannal_ADC2 unsuccess \r\n");
        }
    }

    /******************************************************************************
    function:  Read ADC data
    parameter: 
    Info:
    ******************************************************************************/
    public int ADS1263_Read_ADC1_Data()
    {
        int read = 0;
        int[] buf = { 0, 0, 0, 0 };
        int Status, CRC;
        DEV_Digital_Write(DEV_CS_PIN, 0);
        do
        {
            DEV_SPI_WriteByte(CMD_RDATA1);
            // Utilities.DelayMilliseconds(10);
            Status = DEV_SPI_ReadByte();
        } while ((Status & 0x40) == 0);

        buf[0] = DEV_SPI_ReadByte();
        buf[1] = DEV_SPI_ReadByte();
        buf[2] = DEV_SPI_ReadByte();
        buf[3] = DEV_SPI_ReadByte();
        CRC = DEV_SPI_ReadByte();
        DEV_Digital_Write(DEV_CS_PIN, 1);
        read |= (buf[0] << 24);
        read |= (buf[1] << 16);
        read |= (buf[2] << 8);
        read |= buf[3];
        // Console.Write("%x %x %x %x %x %x\r\n", Status, buf[0], buf[1], buf[2], buf[3], CRC);
        if (ADS1263_Checksum(read, CRC) != 0)
        {
            Console.Write("ADC1 Data read error! \r\n");
        }

        return read;
    }

    /******************************************************************************
    function:  Read ADC data
    parameter: 
    Info:
    ******************************************************************************/
    public int ADS1263_Read_ADC2_Data()
    {
        int read = 0;
        int[] buf = new int[] { 0, 0, 0, 0 };
        int Status, CRC;

        DEV_Digital_Write(DEV_CS_PIN, 0);
        do
        {
            DEV_SPI_WriteByte(CMD_RDATA2);
            // Utilities.DelayMilliseconds(10);
            Status = DEV_SPI_ReadByte();
        } while ((Status & 0x80) == 0);

        buf[0] = DEV_SPI_ReadByte();
        buf[1] = DEV_SPI_ReadByte();
        buf[2] = DEV_SPI_ReadByte();
        buf[3] = DEV_SPI_ReadByte();
        CRC = DEV_SPI_ReadByte();
        DEV_Digital_Write(DEV_CS_PIN, 1);
        read |= (buf[0] << 16);
        read |= (buf[1] << 8);
        read |= buf[2];
        // Console.Write("%x %x %x %x %x\r\n", Status, buf[0], buf[1], buf[2], CRC);
        if (ADS1263_Checksum(read, CRC) != 0)
        {
            Console.Write("ADC2 Data read error! \r\n");
        }

        return read;
    }

    /******************************************************************************
    function:  Read ADC specified channel data
    parameter: 
        Channel: Channel number
    Info:
    ******************************************************************************/
    private int ADS1263_GetChannalValue(int Channel)
    {
        int Value = 0;
        if (ScanMode == 0)
        {// 0  Single-ended input  10 channel1 Differential input  5 channe 
            if (Channel > 10)
            {
                return 0;
            }
            ADS1263_SetChannal(Channel);
            // Utilities.DelayMilliseconds(2);
            // ADS1263_WriteCmd(CMD_START1);
            // Utilities.DelayMilliseconds(2);
            ADS1263_WaitDRDY();
            Value = ADS1263_Read_ADC1_Data();
        }
        else
        {
            if (Channel > 4)
            {
                return 0;
            }
            ADS1263_SetDiffChannal(Channel);
            // Utilities.DelayMilliseconds(2);
            // ADS1263_WriteCmd(CMD_START1);
            // Utilities.DelayMilliseconds(2);
            ADS1263_WaitDRDY();
            Value = ADS1263_Read_ADC1_Data();
        }
        // Console.Write("Get IN%d value success \r\n", Channel);
        return Value;
    }

    /******************************************************************************
    function:  Read ADC specified channel data
    parameter: 
        Channel: Channel number
    Info:
    ******************************************************************************/
    private int ADS1263_GetChannalValue_ADC2(int Channel)
    {
        int Value = 0;
        if (ScanMode == 0)
        {// 0  Single-ended input  10 channel1 Differential input  5 channe 
            if (Channel > 10)
            {
                return 0;
            }
            ADS1263_SetChannal_ADC2(Channel);
            // Utilities.DelayMilliseconds(2);
            ADS1263_WriteCmd(CMD_START2);
            // Utilities.DelayMilliseconds(2);
            Value = ADS1263_Read_ADC2_Data();
        }
        else
        {
            if (Channel > 4)
            {
                return 0;
            }
            ADS1263_SetDiffChannal_ADC2(Channel);
            // Utilities.DelayMilliseconds(2);
            ADS1263_WriteCmd(CMD_START2);
            // Utilities.DelayMilliseconds(2);
            Value = ADS1263_Read_ADC2_Data();
        }
        // Console.Write("Get IN%d value success \r\n", Channel);
        return Value;
    }

    /******************************************************************************
    function:  Read data from all channels
    parameter: 
        ADC_Value : ADC Value
    Info:
    ******************************************************************************/
    private void ADS1263_GetAll(int[] List, int[] Value, int Number)
    {
        for (int i = 0; i < Number; i++)
        {
            Value[i] = ADS1263_GetChannalValue(List[i]);
            // ADS1263_WriteCmd(CMD_STOP1);
            // Utilities.DelayMilliseconds(20);
        }
    }

    /******************************************************************************
    function:  Read data from all channels
    parameter: 
        ADC_Value : ADC Value
    Info:
    ******************************************************************************/
    private void ADS1263_GetAll_ADC2(int[] ADC_Value)
    {
        for (int i = 0; i < 10; i++)
        {
            ADC_Value[i] = ADS1263_GetChannalValue_ADC2(i);
            ADS1263_WriteCmd(CMD_STOP2);
            // Utilities.DelayMilliseconds(20);
        }
        // Console.Write("----------Read ADC2 value success----------\r\n");
    }

    /******************************************************************************
    function:  RTD Test function
    parameter: 
        delay : Conversion delay
        gain :  Conversion gain
        drate : speed
    Info:
    ******************************************************************************/
    private int ADS1263_RTD(int delay, int gain, int drate)
    {
        int Value;

        //MODE0 (CHOP OFF)
        int MODE0 = delay;
        ADS1263_WriteReg(REG_MODE0, MODE0);
        Utilities.DelayMilliseconds(1);

        //(IDACMUX) IDAC2 AINCOM,IDAC1 AIN3
        int IDACMUX = (0x0a << 4) | 0x03;
        ADS1263_WriteReg(REG_IDACMUX, IDACMUX);
        Utilities.DelayMilliseconds(1);

        //((IDACMAG)) IDAC2 = IDAC1 = 250uA
        int IDACMAG = (0x03 << 4) | 0x03;
        ADS1263_WriteReg(REG_IDACMAG, IDACMAG);
        Utilities.DelayMilliseconds(1);

        int MODE2 = (gain << 4) | drate;
        ADS1263_WriteReg(REG_MODE2, MODE2);
        Utilities.DelayMilliseconds(1);

        //INPMUX (AINP = AIN7, AINN = AIN6)
        int INPMUX = (0x07 << 4) | 0x06;
        ADS1263_WriteReg(REG_INPMUX, INPMUX);
        Utilities.DelayMilliseconds(1);

        // REFMUX AIN4 AIN5
        int REFMUX = (0x03 << 3) | 0x03;
        ADS1263_WriteReg(REG_REFMUX, REFMUX);
        Utilities.DelayMilliseconds(1);

        //Read one conversion
        ADS1263_WriteCmd(CMD_START1);
        Utilities.DelayMilliseconds(10);
        ADS1263_WaitDRDY();
        Value = ADS1263_Read_ADC1_Data();
        ADS1263_WriteCmd(CMD_STOP1);

        return Value;
    }

    /******************************************************************************
    function:  DAC Test function
    parameter: 
        volt :          output volt value
        isPositive :    postive or negative
        isOpen :        open or close
    Info:
    ******************************************************************************/
    private void ADS1263_DAC(int volt, int isPositive, int isOpen)
    {
        int Reg, Value;

        if (isPositive != 0)
        {
            Reg = REG_TDACP;        // IN6
        }
        else
        {
            Reg = REG_TDACN;        // IN7
        }

        if (isOpen != 0)
        {
            Value = volt | 0x80;
        }
        else
        {
            Value = 0x00;
        }

        ADS1263_WriteReg(Reg, Value);
    }

}


//#region Ads1263 Enums

/////* gain channel*/
////public enum ADS1263_GAIN
////{
//    public const int ADS1263_GAIN_1 = 0;    /*GAIN  1 */
//    public const int ADS1263_GAIN_2 = 1;    /*GAIN  2 */
//    public const int ADS1263_GAIN_4 = 2;    /*GAIN  4 */
//    public const int ADS1263_GAIN_8 = 3;    /*GAIN  8 */
//    public const int ADS1263_GAIN_16 = 4;    /*GAIN  16 */
//    public const int ADS1263_GAIN_32 = 5;    /*GAIN  32 */
//    public const int ADS1263_GAIN_64 = 6;    /*GAIN  64 */
////}

////public enum ADS1263_DRATE
////{
//    public const int ADS1263_2d5SPS = 0;
//    public const int ADS1263_5SPS;
//    public const int ADS1263_10SPS;
//    public const int ADS1263_16d6SPS;
//    public const int ADS1263_20SPS;
//    public const int ADS1263_50SPS;
//    public const int ADS1263_60SPS;
//    public const int ADS1263_100SPS;
//    public const int ADS1263_400SPS;
//    public const int ADS1263_1200SPS;
//    public const int ADS1263_2400SPS;
//    public const int ADS1263_4800SPS;
//    public const int ADS1263_7200SPS;
//    public const int ADS1263_14400SPS;
//    public const int ADS1263_19200SPS;
//    public const int ADS1263_38400SPS;
////}

////public enum ADS1263_DELAY
////{
//    public const int ADS1263_DELAY_0s = 0;
//    public const int ADS1263_DELAY_8d7us;
//    public const int ADS1263_DELAY_17us;
//    public const int ADS1263_DELAY_35us;
//    public const int ADS1263_DELAY_169us;
//    public const int ADS1263_DELAY_139us;
//    public const int ADS1263_DELAY_278us;
//    public const int ADS1263_DELAY_555us;
//    public const int ADS1263_DELAY_1d1ms;
//    public const int ADS1263_DELAY_2d2ms;
//    public const int ADS1263_DELAY_4d4ms;
//    public const int ADS1263_DELAY_8d8ms;
////}

////public enum ADS1263_ADC2_DRATE
////{
//    public const int ADS1263_ADC2_10SPS = 0;
//    public const int ADS1263_ADC2_100SPS;
//    public const int ADS1263_ADC2_400SPS;
//    public const int ADS1263_ADC2_800SPS;
////}

////public enum ADS1263_ADC2_GAIN
////{
//    public const int ADS1263_ADC2_GAIN_1 = 0;
//    public const int ADS1263_ADC2_GAIN_2;
//    public const int ADS1263_ADC2_GAIN_4;
//    public const int ADS1263_ADC2_GAIN_8;
//    public const int ADS1263_ADC2_GAIN_16;
//    public const int ADS1263_ADC2_GAIN_32;
//    public const int ADS1263_ADC2_GAIN_64;
//    public const int ADS1263_ADC2_GAIN_128;
////}

////public enum ADS1263_DAC_VOLT
////{
//    public const int ADS1263_DAC_VLOT_4_5 = 0b01001;      //4.5V
//    public const int ADS1263_DAC_VLOT_3_5 = 0b01000;
//    public const int ADS1263_DAC_VLOT_3 = 0b00111;
//    public const int ADS1263_DAC_VLOT_2_75 = 0b00110;
//    public const int ADS1263_DAC_VLOT_2_625 = 0b00101;
//    public const int ADS1263_DAC_VLOT_2_5625 = 0b00100;
//    public const int ADS1263_DAC_VLOT_2_53125 = 0b00011;
//    public const int ADS1263_DAC_VLOT_2_515625 = 0b00010;
//    public const int ADS1263_DAC_VLOT_2_5078125 = 0b00001;
//    public const int ADS1263_DAC_VLOT_2_5 = 0b00000;
//    public const int ADS1263_DAC_VLOT_2_4921875 = 0b10001;
//    public const int ADS1263_DAC_VLOT_2_484375 = 0b10010;
//    public const int ADS1263_DAC_VLOT_2_46875 = 0b10011;
//    public const int ADS1263_DAC_VLOT_2_4375 = 0b10100;
//    public const int ADS1263_DAC_VLOT_2_375 = 0b10101;
//    public const int ADS1263_DAC_VLOT_2_25 = 0b10110;
//    public const int ADS1263_DAC_VLOT_2 = 0b10111;
//    public const int ADS1263_DAC_VLOT_1_5 = 0b11000;
//    public const int ADS1263_DAC_VLOT_0_5 = 0b11001;
////}

////public enum ADS1263_REG
////{
//    public const int /*Register address; followed by reset the default values */
//    public const int REG_ID = 0;    // xxh
//    public const int REG_POWER;      // 11h
//    public const int REG_INTERFACE;  // 05h
//    public const int REG_MODE0;      // 00h
//    public const int REG_MODE1;      // 80h
//    public const int REG_MODE2;      // 04h
//    public const int REG_INPMUX;     // 01h
//    public const int REG_OFCAL0;     // 00h
//    public const int REG_OFCAL1;     // 00h
//    public const int REG_OFCAL2;     // 00h
//    public const int REG_FSCAL0;     // 00h
//    public const int REG_FSCAL1;     // 00h
//    public const int REG_FSCAL2;     // 40h
//    public const int REG_IDACMUX;    // BBh
//    public const int REG_IDACMAG;    // 00h
//    public const int REG_REFMUX;     // 00h
//    public const int REG_TDACP;      // 00h
//    public const int REG_TDACN;      // 00h
//    public const int REG_GPIOCON;    // 00h
//    public const int REG_GPIODIR;    // 00h
//    public const int REG_GPIODAT;    // 00h
//    public const int REG_ADC2CFG;    // 00h
//    public const int REG_ADC2MUX;    // 01h
//    public const int REG_ADC2OFC0;   // 00h
//    public const int REG_ADC2OFC1;   // 00h
//    public const int REG_ADC2FSC0;   // 00h
//    public const int REG_ADC2FSC1;   // 40h
////}

////public enum ADS1263_CMD
////{
//    public const int CMD_RESET = 0x06; // Reset the ADC; 0000 011x (06h or 07h)
//    public const int CMD_START1 = 0x08; // Start ADC1 conversions; 0000 100x (08h or 09h)
//    public const int CMD_STOP1 = 0x0A; // Stop ADC1 conversions; 0000 101x (0Ah or 0Bh)
//    public const int CMD_START2 = 0x0C; // Start ADC2 conversions; 0000 110x (0Ch or 0Dh)
//    public const int CMD_STOP2 = 0x0E; // Stop ADC2 conversions; 0000 111x (0Eh or 0Fh)
//    public const int CMD_RDATA1 = 0x12; // Read ADC1 data; 0001 001x (12h or 13h)
//    public const int CMD_RDATA2 = 0x14; // Read ADC2 data; 0001 010x (14h or 15h)
//    public const int CMD_SYOCAL1 = 0x16; // ADC1 system offset calibration; 0001 0110 (16h)
//    public const int CMD_SYGCAL1 = 0x17; // ADC1 system gain calibration; 0001 0111 (17h)
//    public const int CMD_SFOCAL1 = 0x19; // ADC1 self offset calibration; 0001 1001 (19h)
//    public const int CMD_SYOCAL2 = 0x1B; // ADC2 system offset calibration; 0001 1011 (1Bh)
//    public const int CMD_SYGCAL2 = 0x1C; // ADC2 system gain calibration; 0001 1100 (1Ch)
//    public const int CMD_SFOCAL2 = 0x1E; // ADC2 self offset calibration; 0001 1110 (1Eh)
//    public const int CMD_RREG = 0x20; // Read registers 001r rrrr (20h+000r rrrr)
//    public const int CMD_RREG2 = 0x00; // number of registers to read minus 1; 000n nnnn
//    public const int CMD_WREG = 0x40; // Write registers 010r rrrr (40h+000r rrrr)
//    public const int CMD_WREG2 = 0x00; // number of registers to write minus 1; 000n nnnn
//    public const int CMD_PGA_BYPASSED = 0x80; //0x80:PGA bypassed; 0x00:PGA enabled
////};

//#endregion
