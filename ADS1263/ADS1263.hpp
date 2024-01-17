#pragma once

#pragma region INCLUDES

#include <stdint.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/ioctl.h> 
#include <linux/types.h> 
#include <linux/spi/spidev.h> 
#include <stdio.h>
#include <unistd.h>
#include <errno.h>
#include <stdlib.h>
#include <getopt.h> 
#include <fcntl.h>
#include <string.h>

#pragma endregion

typedef unsigned char  uint8_t;
typedef unsigned short uint16_t;
typedef unsigned int   uint32_t;

#pragma region DEV
/**
 * data
**/
#ifndef UBYTE
#define UBYTE   uint8_t
#endif
#ifndef UWORD
#define UWORD   uint16_t
#endif
#ifndef UDOUBLE
#define UDOUBLE uint32_t
#endif

/**
 * GPIOI config
**/
extern int DEV_RST_PIN;
extern int DEV_CS_PIN;
extern int DEV_DRDY_PIN;

/*------------------------------------------------------------------------------------------------------*/
[[gnu::dllexport]] extern "C" void DEV_Digital_Write(UWORD Pin, UBYTE Value);
[[gnu::dllexport]] extern "C" UBYTE DEV_Digital_Read(UWORD Pin);

[[gnu::dllexport]] extern "C" UBYTE DEV_SPI_WriteByte(UBYTE Value);
[[gnu::dllexport]] extern "C" UBYTE DEV_SPI_ReadByte();

[[gnu::dllexport]] extern "C" UBYTE DEV_Module_Init();
[[gnu::dllexport]] extern "C" void DEV_Module_Exit();

[[gnu::dllexport]] extern "C" void DEV_Delay_ms(UDOUBLE xms);



#pragma endregion

#pragma region SYSFS

#if DEBUG
#define Debug(__info,...) printf("Debug: " __info,##__VA_ARGS__)
#else
#define Debug(__info,...)  
#endif

#ifndef SYSFS_GPIO_IN
#define SYSFS_GPIO_IN  0
#endif
#ifndef SYSFS_GPIO_OUT
#define SYSFS_GPIO_OUT 1
#endif

#ifndef SYSFS_GPIO_LOW
#define SYSFS_GPIO_LOW  0
#endif
#ifndef SYSFS_GPIO_HIGH
#define SYSFS_GPIO_HIGH 1
#endif

#ifndef NUM_MAXBUF
#define NUM_MAXBUF  4
#endif
#ifndef DIR_MAXSIZ
#define DIR_MAXSIZ  60
#endif

#define SYSFS_GPIO_DEBUG 0
#if SYSFS_GPIO_DEBUG 
#define SYSFS_GPIO_Debug(__info,...) printf("Debug: " __info,##__VA_ARGS__)
#else
#define SYSFS_GPIO_Debug(__info,...)  
#endif 

// BCM GPIO for Jetson nano
#define GPIO4 4 // 7, 4
#define GPIO17 7 // 11, 17
#define GPIO18 18 // 12, 18
#define GPIO27 27 // 13, 27
#define GPIO22 22 // 15, 22
#define GPIO23 23 // 16, 23
#define GPIO24 24 // 18, 24
#define SPI0_MOSI 10 // 19, 10
#define SPI0_MISO 9 // 21, 9
#define GPIO25 28 // 22, 25
#define SPI0_SCK 11 // 23, 11
#define SPI0_CS0 8 // 24, 8
#define SPI0_CS1 7 // 26, 7
#define GPIO5 5 // 29, 5
#define GPIO6 6 // 31, 6
#define GPIO12 12 // 32, 12
#define GPIO13 13 // 33, 13
#define GPIO19 19 // 35, 19
#define GPIO16 16 // 36, 16
#define GPIO26 26 // 37, 26
#define GPIO20 20 // 38, 20
#define GPIO21 21 // 40, 21

[[gnu::dllexport]] extern "C" int SYSFS_GPIO_Export(int Pin);
[[gnu::dllexport]] extern "C" int SYSFS_GPIO_Unexport(int Pin);
[[gnu::dllexport]] extern "C" int SYSFS_GPIO_Direction(int Pin, int Dir);
[[gnu::dllexport]] extern "C" int SYSFS_GPIO_Read(int Pin);
[[gnu::dllexport]] extern "C" int SYSFS_GPIO_Write(int Pin, int value);

#pragma endregion

#pragma region HardwareSPI

#define DEV_HARDWARE_SPI_DEBUG 0
#if DEV_HARDWARE_SPI_DEBUG
#define DEV_HARDWARE_SPI_Debug(__info,...) printf("Debug: " __info,##__VA_ARGS__)
#else
#define DEV_HARDWARE_SPI_Debug(__info,...)
#endif

#define SPI_CPHA        0x01
#define SPI_CPOL        0x02
#define SPI_MODE_0      (0|0)
#define SPI_MODE_1      (0|SPI_CPHA)
#define SPI_MODE_2      (SPI_CPOL|0)
#define SPI_MODE_3      (SPI_CPOL|SPI_CPHA)

typedef enum {
    SPI_MODE0 = SPI_MODE_0,  /*!< CPOL = 0, CPHA = 0 */
    SPI_MODE1 = SPI_MODE_1,  /*!< CPOL = 0, CPHA = 1 */
    SPI_MODE2 = SPI_MODE_2,  /*!< CPOL = 1, CPHA = 0 */
    SPI_MODE3 = SPI_MODE_3   /*!< CPOL = 1, CPHA = 1 */
}SPIMode;

typedef enum {
    DISABLE = 0,
    ENABLE = 1
}SPICSEN;

typedef enum {
    SPI_CS_Mode_LOW = 0,     /*!< Chip Select 0 */
    SPI_CS_Mode_HIGH = 1,     /*!< Chip Select 1 */
    SPI_CS_Mode_NONE = 3  /*!< No CS, control it yourself */
}SPIChipSelect;

typedef enum
{
    SPI_BIT_ORDER_LSBFIRST = 0,  /*!< LSB First */
    SPI_BIT_ORDER_MSBFIRST = 1   /*!< MSB First */
}SPIBitOrder;

typedef enum
{
    SPI_3WIRE_Mode = 0,
    SPI_4WIRE_Mode = 1
}BusMode;


/**
 * Define SPI attribute
**/
typedef struct SPIStruct {
    //GPIO
    uint16_t SCLK_PIN;
    uint16_t MOSI_PIN;
    uint16_t MISO_PIN;

    uint16_t CS0_PIN;
    uint16_t CS1_PIN;


    uint32_t speed;
    uint16_t mode;
    uint16_t delay;
    int fd; //
} HARDWARE_SPI;


[[gnu::dllexport]] extern "C" void DEV_HARDWARE_SPI_begin(char* SPI_device);
[[gnu::dllexport]] extern "C" void DEV_HARDWARE_SPI_beginSet(char* SPI_device, SPIMode mode, uint32_t speed);
[[gnu::dllexport]] extern "C" void DEV_HARDWARE_SPI_end(void);

[[gnu::dllexport]] extern "C" int DEV_HARDWARE_SPI_setSpeed(uint32_t speed);

[[gnu::dllexport]] extern "C" uint8_t DEV_HARDWARE_SPI_TransferByte(uint8_t buf);
[[gnu::dllexport]] extern "C" int DEV_HARDWARE_SPI_Transfer(uint8_t* buf, uint32_t len);

[[gnu::dllexport]] extern "C" void DEV_HARDWARE_SPI_SetDataInterval(uint16_t us);
[[gnu::dllexport]] extern "C" int DEV_HARDWARE_SPI_SetBusMode(BusMode mode);
[[gnu::dllexport]] extern "C" int DEV_HARDWARE_SPI_SetBitOrder(SPIBitOrder Order);
[[gnu::dllexport]] extern "C" int DEV_HARDWARE_SPI_ChipSelect(SPIChipSelect CS_Mode);
[[gnu::dllexport]] extern "C" int DEV_HARDWARE_SPI_CSEN(SPICSEN EN);
[[gnu::dllexport]] extern "C" int DEV_HARDWARE_SPI_Mode(SPIMode mode);

#pragma endregion

#pragma region ADS1263

#define Positive_A6 1
#define Negative_A7 0

#define Open    1 
#define Close   0

/* gain channel*/
typedef enum
{
    ADS1263_GAIN_1 = 0,    /*GAIN  1 */
    ADS1263_GAIN_2 = 1,    /*GAIN  2 */
    ADS1263_GAIN_4 = 2,    /*GAIN  4 */
    ADS1263_GAIN_8 = 3,    /*GAIN  8 */
    ADS1263_GAIN_16 = 4,    /*GAIN  16 */
    ADS1263_GAIN_32 = 5,    /*GAIN  32 */
    ADS1263_GAIN_64 = 6,    /*GAIN  64 */
}ADS1263_GAIN;

typedef enum
{
    ADS1263_2d5SPS = 0,
    ADS1263_5SPS,
    ADS1263_10SPS,
    ADS1263_16d6SPS,
    ADS1263_20SPS,
    ADS1263_50SPS,
    ADS1263_60SPS,
    ADS1263_100SPS,
    ADS1263_400SPS,
    ADS1263_1200SPS,
    ADS1263_2400SPS,
    ADS1263_4800SPS,
    ADS1263_7200SPS,
    ADS1263_14400SPS,
    ADS1263_19200SPS,
    ADS1263_38400SPS,
}ADS1263_DRATE;

typedef enum
{
    ADS1263_DELAY_0s = 0,
    ADS1263_DELAY_8d7us,
    ADS1263_DELAY_17us,
    ADS1263_DELAY_35us,
    ADS1263_DELAY_169us,
    ADS1263_DELAY_139us,
    ADS1263_DELAY_278us,
    ADS1263_DELAY_555us,
    ADS1263_DELAY_1d1ms,
    ADS1263_DELAY_2d2ms,
    ADS1263_DELAY_4d4ms,
    ADS1263_DELAY_8d8ms,
}ADS1263_DELAY;

typedef enum
{
    ADS1263_ADC2_10SPS = 0,
    ADS1263_ADC2_100SPS,
    ADS1263_ADC2_400SPS,
    ADS1263_ADC2_800SPS,
}ADS1263_ADC2_DRATE;

typedef enum
{
    ADS1263_ADC2_GAIN_1 = 0,
    ADS1263_ADC2_GAIN_2,
    ADS1263_ADC2_GAIN_4,
    ADS1263_ADC2_GAIN_8,
    ADS1263_ADC2_GAIN_16,
    ADS1263_ADC2_GAIN_32,
    ADS1263_ADC2_GAIN_64,
    ADS1263_ADC2_GAIN_128,
}ADS1263_ADC2_GAIN;

typedef enum
{
    ADS1263_DAC_VLOT_4_5 = 0b01001,      //4.5V
    ADS1263_DAC_VLOT_3_5 = 0b01000,
    ADS1263_DAC_VLOT_3 = 0b00111,
    ADS1263_DAC_VLOT_2_75 = 0b00110,
    ADS1263_DAC_VLOT_2_625 = 0b00101,
    ADS1263_DAC_VLOT_2_5625 = 0b00100,
    ADS1263_DAC_VLOT_2_53125 = 0b00011,
    ADS1263_DAC_VLOT_2_515625 = 0b00010,
    ADS1263_DAC_VLOT_2_5078125 = 0b00001,
    ADS1263_DAC_VLOT_2_5 = 0b00000,
    ADS1263_DAC_VLOT_2_4921875 = 0b10001,
    ADS1263_DAC_VLOT_2_484375 = 0b10010,
    ADS1263_DAC_VLOT_2_46875 = 0b10011,
    ADS1263_DAC_VLOT_2_4375 = 0b10100,
    ADS1263_DAC_VLOT_2_375 = 0b10101,
    ADS1263_DAC_VLOT_2_25 = 0b10110,
    ADS1263_DAC_VLOT_2 = 0b10111,
    ADS1263_DAC_VLOT_1_5 = 0b11000,
    ADS1263_DAC_VLOT_0_5 = 0b11001,
}ADS1263_DAC_VOLT;

typedef enum
{
    /*Register address, followed by reset the default values */
    REG_ID = 0,    // xxh
    REG_POWER,      // 11h
    REG_INTERFACE,  // 05h
    REG_MODE0,      // 00h
    REG_MODE1,      // 80h
    REG_MODE2,      // 04h
    REG_INPMUX,     // 01h
    REG_OFCAL0,     // 00h
    REG_OFCAL1,     // 00h
    REG_OFCAL2,     // 00h
    REG_FSCAL0,     // 00h
    REG_FSCAL1,     // 00h
    REG_FSCAL2,     // 40h
    REG_IDACMUX,    // BBh
    REG_IDACMAG,    // 00h
    REG_REFMUX,     // 00h
    REG_TDACP,      // 00h
    REG_TDACN,      // 00h
    REG_GPIOCON,    // 00h
    REG_GPIODIR,    // 00h
    REG_GPIODAT,    // 00h
    REG_ADC2CFG,    // 00h
    REG_ADC2MUX,    // 01h
    REG_ADC2OFC0,   // 00h
    REG_ADC2OFC1,   // 00h
    REG_ADC2FSC0,   // 00h
    REG_ADC2FSC1,   // 40h
}ADS1263_REG;

typedef enum
{
    CMD_RESET = 0x06, // Reset the ADC, 0000 011x (06h or 07h)
    CMD_START1 = 0x08, // Start ADC1 conversions, 0000 100x (08h or 09h)
    CMD_STOP1 = 0x0A, // Stop ADC1 conversions, 0000 101x (0Ah or 0Bh)
    CMD_START2 = 0x0C, // Start ADC2 conversions, 0000 110x (0Ch or 0Dh)
    CMD_STOP2 = 0x0E, // Stop ADC2 conversions, 0000 111x (0Eh or 0Fh)
    CMD_RDATA1 = 0x12, // Read ADC1 data, 0001 001x (12h or 13h)
    CMD_RDATA2 = 0x14, // Read ADC2 data, 0001 010x (14h or 15h)
    CMD_SYOCAL1 = 0x16, // ADC1 system offset calibration, 0001 0110 (16h)
    CMD_SYGCAL1 = 0x17, // ADC1 system gain calibration, 0001 0111 (17h)
    CMD_SFOCAL1 = 0x19, // ADC1 self offset calibration, 0001 1001 (19h)
    CMD_SYOCAL2 = 0x1B, // ADC2 system offset calibration, 0001 1011 (1Bh)
    CMD_SYGCAL2 = 0x1C, // ADC2 system gain calibration, 0001 1100 (1Ch)
    CMD_SFOCAL2 = 0x1E, // ADC2 self offset calibration, 0001 1110 (1Eh)
    CMD_RREG = 0x20, // Read registers 001r rrrr (20h+000r rrrr)
    CMD_RREG2 = 0x00, // number of registers to read minus 1, 000n nnnn
    CMD_WREG = 0x40, // Write registers 010r rrrr (40h+000r rrrr)
    CMD_WREG2 = 0x00, // number of registers to write minus 1, 000n nnnn
}ADS1263_CMD;

[[gnu::dllexport]] extern "C" UBYTE ADS1263_init_ADC1(ADS1263_DRATE rate);
[[gnu::dllexport]] extern "C" UBYTE ADS1263_init_ADC2(ADS1263_ADC2_DRATE rate);
[[gnu::dllexport]] extern "C" void ADS1263_SetMode(UBYTE Mode);
[[gnu::dllexport]] extern "C" UDOUBLE ADS1263_GetChannalValue(UBYTE Channel);
[[gnu::dllexport]] extern "C" void ADS1263_GetAll(UBYTE* List, UDOUBLE* Value, int Number);
[[gnu::dllexport]] extern "C" void ADS1263_GetAll_ADC2(UDOUBLE* ADC_Value);
[[gnu::dllexport]] extern "C" UDOUBLE ADS1263_RTD(ADS1263_DELAY delay, ADS1263_GAIN gain, ADS1263_DRATE drate);
[[gnu::dllexport]] extern "C" void ADS1263_DAC(ADS1263_DAC_VOLT volt, UBYTE isPositive, UBYTE isClose);

//__declspec(dllexport) KOKKOS_FUNCTION uint64 View_##TYPE_NAME##_##EXECUTION_SPACE##_8D::GetStride(uint32 dim) const
//{
//    return view.stride(dim);
//}

#pragma endregion
