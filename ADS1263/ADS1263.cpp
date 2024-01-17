
#include "ADS1263.hpp"

#pragma region DEV

#include <fcntl.h>

/**
 * GPIO
**/
int DEV_RST_PIN;
int DEV_CS_PIN;
int DEV_DRDY_PIN;

/**
 * GPIO read and write
**/
void DEV_Digital_Write(UWORD Pin, UBYTE Value)
{
#ifdef RPI
#ifdef USE_BCM2835_LIB
    bcm2835_gpio_write(Pin, Value);
#elif USE_WIRINGPI_LIB
    digitalWrite(Pin, Value);
#elif USE_DEV_LIB
    SYSFS_GPIO_Write(Pin, Value);
#endif
#endif

#ifdef JETSON
#ifdef USE_DEV_LIB
    SYSFS_GPIO_Write(Pin, Value);
#elif USE_HARDWARE_LIB
    Debug("not support");
#endif
#endif
}

UBYTE DEV_Digital_Read(UWORD Pin)
{
    UBYTE Read_value = 0;
#ifdef RPI
#ifdef USE_BCM2835_LIB
    Read_value = bcm2835_gpio_lev(Pin);
#elif USE_WIRINGPI_LIB
    Read_value = digitalRead(Pin);
#elif USE_DEV_LIB
    Read_value = SYSFS_GPIO_Read(Pin);
#endif
#endif

#ifdef JETSON
#ifdef USE_DEV_LIB
    Read_value = SYSFS_GPIO_Read(Pin);
#elif USE_HARDWARE_LIB
    Debug("not support");
#endif
#endif
    return Read_value;
}

/**
 * SPI
**/
UBYTE DEV_SPI_WriteByte(uint8_t Value)
{
    UBYTE temp = 0;
    // printf("write %x \r\n", Value);
#ifdef RPI
#ifdef USE_BCM2835_LIB
    temp = bcm2835_spi_transfer(Value);
#elif USE_WIRINGPI_LIB
    wiringPiSPIDataRW(0, &Value, 1);
    temp = Value;
#elif USE_DEV_LIB
    temp = DEV_HARDWARE_SPI_TransferByte(Value);
#endif
#endif

#ifdef JETSON
#ifdef USE_DEV_LIB
    temp = SYSFS_software_spi_transfer(Value);
#elif USE_HARDWARE_LIB
    Debug("not support");
#endif
#endif
    // printf("Read %x \r\n", temp);
    return temp;
}

UBYTE DEV_SPI_ReadByte()
{
    return DEV_SPI_WriteByte(0x00);
}

/**
 * GPIO Mode
**/
void DEV_GPIO_Mode(UWORD Pin, UWORD Mode)
{
#ifdef RPI
#ifdef USE_BCM2835_LIB
    if (Mode == 0 || Mode == BCM2835_GPIO_FSEL_INPT) {
        bcm2835_gpio_fsel(Pin, BCM2835_GPIO_FSEL_INPT);
    }
    else {
        bcm2835_gpio_fsel(Pin, BCM2835_GPIO_FSEL_OUTP);
    }
#elif USE_WIRINGPI_LIB
    if (Mode == 0 || Mode == INPUT) {
        pinMode(Pin, INPUT);
        pullUpDnControl(Pin, PUD_UP);
    }
    else {
        pinMode(Pin, OUTPUT);
        // Debug (" %d OUT \r\n",Pin);
    }
#elif USE_DEV_LIB
    SYSFS_GPIO_Export(Pin);
    if (Mode == 0 || Mode == SYSFS_GPIO_IN) {
        SYSFS_GPIO_Direction(Pin, SYSFS_GPIO_IN);
        // Debug("IN Pin = %d\r\n",Pin);
    }
    else {
        SYSFS_GPIO_Direction(Pin, SYSFS_GPIO_OUT);
        // Debug("OUT Pin = %d\r\n",Pin);
    }
#endif
#endif

#ifdef JETSON
#ifdef USE_DEV_LIB
    SYSFS_GPIO_Export(Pin);
    SYSFS_GPIO_Direction(Pin, Mode);
#elif USE_HARDWARE_LIB
    Debug("not support");
#endif
#endif
}

/**
 * delay x ms
**/
void DEV_Delay_ms(UDOUBLE xms)
{
#ifdef RPI
#ifdef USE_BCM2835_LIB
    bcm2835_delay(xms);
#elif USE_WIRINGPI_LIB
    delay(xms);
#elif USE_DEV_LIB
    UDOUBLE i;
    for (i = 0; i < xms; i++) {
        usleep(1000);
    }
#endif
#endif

#ifdef JETSON
    UDOUBLE i;
    for (i = 0; i < xms; i++) {
        usleep(1000);
    }
#endif
}

static int DEV_Equipment_Testing()
{
    int i;
    int fd;
    char value_str[20];
    fd = open("/etc/issue", O_RDONLY);
    printf("Current environment: ");
    while (1) {
        if (fd < 0) {
            Debug("Read failed Pin\n");
            return -1;
        }
        for (i = 0;; i++) {
            if (read(fd, &value_str[i], 1) < 0) {
                Debug("failed to read value!\n");
                return -1;
            }
            if (value_str[i] == 32) {
                printf("\r\n");
                break;
            }
            printf("%c", value_str[i]);
        }
        break;
    }
#ifdef RPI
    if (i < 5) {
        printf("Unrecognizable\r\n");
    }
    else {
        char RPI_System[10] = { "Raspbian" };
        for (i = 0; i < 6; i++) {
            if (RPI_System[i] != value_str[i]) {
                printf("Please make JETSON !!!!!!!!!!\r\n");
                return -1;
            }
        }
    }
#endif
#ifdef JETSON
    if (i < 5) {
        Debug("Unrecognizable\r\n");
    }
    else {
        char JETSON_System[10] = { "Ubuntu" };
        for (i = 0; i < 6; i++) {
            if (JETSON_System[i] != value_str[i]) {
                printf("Please make RPI !!!!!!!!!!\r\n");
                return -1;
            }
        }
    }
#endif
    return 0;
}

void DEV_GPIO_Init()
{
#ifdef RPI
    DEV_RST_PIN = 18;
    DEV_CS_PIN = 22;
    DEV_DRDY_PIN = 17;
#elif JETSON
    DEV_RST_PIN = GPIO18;
    DEV_CS_PIN = GPIO22;
    DEV_DRDY_PIN = GPIO17;
#endif

    DEV_GPIO_Mode(DEV_RST_PIN, 1);
    DEV_GPIO_Mode(DEV_CS_PIN, 1);

    DEV_GPIO_Mode(DEV_DRDY_PIN, 0);

    DEV_Digital_Write(DEV_CS_PIN, 1);
}

/******************************************************************************
function:	Module Initialize, the library and initialize the pins, SPI protocol
parameter:
Info:
******************************************************************************/
UBYTE DEV_Module_Init()
{
    printf("/***********************************/ \r\n");
    if (DEV_Equipment_Testing() < 0) {
        return 1;
    }
#ifdef RPI
#ifdef USE_BCM2835_LIB
    if (!bcm2835_init()) {
        printf("bcm2835 init failed  !!! \r\n");
        return 1;
    }
    else {
        printf("bcm2835 init success !!! \r\n");
    }

    // GPIO Config
    DEV_GPIO_Init();

    bcm2835_spi_begin();                                         //Start spi interface, set spi pin for the reuse function
    bcm2835_spi_setBitOrder(BCM2835_SPI_BIT_ORDER_MSBFIRST);     //High first transmission
    bcm2835_spi_setDataMode(BCM2835_SPI_MODE1);                  //spi mode 1, '0, 1'
    bcm2835_spi_setClockDivider(BCM2835_SPI_CLOCK_DIVIDER_32);  //Frequency
#elif USE_WIRINGPI_LIB
    // if(wiringPiSetup() < 0) {//use wiringpi Pin number table
    if (wiringPiSetupGpio() < 0) { //use BCM2835 Pin number table
        printf("set wiringPi lib failed	!!! \r\n");
        return 1;
    }
    else {
        printf("set wiringPi lib success !!! \r\n");
    }

    // GPIO Config
    DEV_GPIO_Init();
    // wiringPiSPISetup(0,10000000);
    wiringPiSPISetupMode(0, 1000000, 1);
#elif USE_DEV_LIB
    printf("Write and read /dev/spidev0.0 \r\n");
    DEV_GPIO_Init();
    DEV_HARDWARE_SPI_begin("/dev/spidev0.0");
    DEV_HARDWARE_SPI_setSpeed(1000000);
    DEV_HARDWARE_SPI_Mode(SPI_MODE_1);
#endif


#elif JETSON
#ifdef USE_DEV_LIB
    DEV_GPIO_Init();
    printf("Software spi\r\n");
    SYSFS_software_spi_begin();
    SYSFS_software_spi_setBitOrder(SOFTWARE_SPI_MSBFIRST);
    SYSFS_software_spi_setDataMode(SOFTWARE_SPI_Mode1);
    SYSFS_software_spi_setClockDivider(SOFTWARE_SPI_CLOCK_DIV16);
#elif USE_HARDWARE_LIB
    printf("Write and read /dev/spidev0.0 \r\n");
    DEV_GPIO_Init();
    DEV_HARDWARE_SPI_begin("/dev/spidev0.0");
#endif

#endif
    printf("/***********************************/ \r\n");
    return 0;
}

/******************************************************************************
function:	Module exits, closes SPI and BCM2835 library
parameter:
Info:
******************************************************************************/
void DEV_Module_Exit()
{
#ifdef RPI
#ifdef USE_BCM2835_LIB
    DEV_Digital_Write(DEV_RST_PIN, LOW);
    DEV_Digital_Write(DEV_CS_PIN, LOW);

    bcm2835_spi_end();
    bcm2835_close();
#elif USE_WIRINGPI_LIB
    DEV_Digital_Write(DEV_RST_PIN, 0);
    DEV_Digital_Write(DEV_CS_PIN, 0);
#elif USE_DEV_LIB
    DEV_HARDWARE_SPI_end();
    DEV_Digital_Write(DEV_RST_PIN, 0);
    DEV_Digital_Write(DEV_CS_PIN, 0);
#endif

#elif JETSON
#ifdef USE_DEV_LIB
    SYSFS_GPIO_Unexport(DEV_RST_PIN);
    SYSFS_GPIO_Unexport(DEV_CS_PIN);
    SYSFS_GPIO_Unexport(DEV_DRDY_PIN);

#elif USE_HARDWARE_LIB
    Debug("not support");
#endif
#endif
}

#pragma endregion

#pragma region SYSFS

int SYSFS_GPIO_Export(int Pin)
{
    char buffer[NUM_MAXBUF];
    int len;
    int fd;

    fd = open("/sys/class/gpio/export", O_WRONLY);
    if (fd < 0) {
        SYSFS_GPIO_Debug("Export Failed: Pin%d\n", Pin);
        return -1;
    }

    len = snprintf(buffer, NUM_MAXBUF, "%d", Pin);
    write(fd, buffer, len);

    SYSFS_GPIO_Debug("Export: Pin%d\r\n", Pin);

    close(fd);
    return 0;
}

int SYSFS_GPIO_Unexport(int Pin)
{
    char buffer[NUM_MAXBUF];
    int len;
    int fd;

    fd = open("/sys/class/gpio/unexport", O_WRONLY);
    if (fd < 0) {
        SYSFS_GPIO_Debug("unexport Failed: Pin%d\n", Pin);
        return -1;
    }

    len = snprintf(buffer, NUM_MAXBUF, "%d", Pin);
    write(fd, buffer, len);

    SYSFS_GPIO_Debug("Unexport: Pin%d\r\n", Pin);

    close(fd);
    return 0;
}

int SYSFS_GPIO_Direction(int Pin, int Dir)
{
    const char dir_str[] = "in\0out";
    char path[DIR_MAXSIZ];
    int fd;

    snprintf(path, DIR_MAXSIZ, "/sys/class/gpio/gpio%d/direction", Pin);
    fd = open(path, O_WRONLY);
    if (fd < 0) {
        SYSFS_GPIO_Debug("Set Direction failed: Pin%d\n", Pin);
        return -1;
    }

    if (write(fd, &dir_str[Dir == SYSFS_GPIO_IN ? 0 : 3], Dir == SYSFS_GPIO_IN ? 2 : 3) < 0) {
        SYSFS_GPIO_Debug("failed to set direction!\r\n");
        return -1;
    }

    if (Dir == SYSFS_GPIO_IN) {
        SYSFS_GPIO_Debug("Pin%d:intput\r\n", Pin);
    }
    else {
        SYSFS_GPIO_Debug("Pin%d:Output\r\n", Pin);
    }

    close(fd);
    return 0;
}

int SYSFS_GPIO_Read(int Pin)
{
    char path[DIR_MAXSIZ];
    char value_str[3];
    int fd;

    snprintf(path, DIR_MAXSIZ, "/sys/class/gpio/gpio%d/value", Pin);
    fd = open(path, O_RDONLY);
    if (fd < 0) {
        SYSFS_GPIO_Debug("Read failed Pin%d\n", Pin);
        return -1;
    }

    if (read(fd, value_str, 3) < 0) {
        SYSFS_GPIO_Debug("failed to read value!\n");
        return -1;
    }

    close(fd);
    return(atoi(value_str));
}

int SYSFS_GPIO_Write(int Pin, int value)
{
    const char s_values_str[] = "01";
    char path[DIR_MAXSIZ];
    int fd;

    snprintf(path, DIR_MAXSIZ, "/sys/class/gpio/gpio%d/value", Pin);
    fd = open(path, O_WRONLY);
    if (fd < 0) {
        SYSFS_GPIO_Debug("Write failed : Pin%d,value = %d\n", Pin, value);
        return -1;
    }

    if (write(fd, &s_values_str[value == SYSFS_GPIO_LOW ? 0 : 1], 1) < 0) {
        SYSFS_GPIO_Debug("failed to write value!\n");
        return -1;
    }

    close(fd);
    return 0;
}

#pragma endregion

#pragma region HardwareSPI

HARDWARE_SPI hardware_SPI;

static uint8_t bits = 8;

#ifndef SPI_CS_HIGH
#define SPI_CS_HIGH     0x04                //Chip select high
#endif
#ifndef SPI_LSB_FIRST
#define SPI_LSB_FIRST   0x08                //LSB
#endif
#ifndef SPI_3WIRE
#define SPI_3WIRE       0x10                //3-wire mode SI and SO same line
#endif
#ifndef SPI_LOOP
#define SPI_LOOP        0x20                //Loopback mode
#endif
#ifndef SPI_NO_CS
#define SPI_NO_CS       0x40                //A single device occupies one SPI bus, so there is no chip select
#endif
#ifndef SPI_READY
#define SPI_READY       0x80                //Slave pull low to stop data transmission
#endif

struct spi_ioc_transfer tr;

/******************************************************************************
function:   SPI port initialization
parameter:
    SPI_device : Device name
Info:
    /dev/spidev0.0
    /dev/spidev0.1
******************************************************************************/
void DEV_HARDWARE_SPI_begin(char* SPI_device)
{
    //device
    int ret = 0;
    if ((hardware_SPI.fd = open(SPI_device, O_RDWR)) < 0) {
        perror("Failed to open SPI device.\n");
        DEV_HARDWARE_SPI_Debug("Failed to open SPI device\r\n");
        exit(1);
    }
    else {
        DEV_HARDWARE_SPI_Debug("open : %s\r\n", SPI_device);
    }
    hardware_SPI.mode = 0;

    ret = ioctl(hardware_SPI.fd, SPI_IOC_WR_BITS_PER_WORD, &bits);
    if (ret == -1) {
        DEV_HARDWARE_SPI_Debug("can't set bits per word\r\n");
    }

    ret = ioctl(hardware_SPI.fd, SPI_IOC_RD_BITS_PER_WORD, &bits);
    if (ret == -1) {
        DEV_HARDWARE_SPI_Debug("can't get bits per word\r\n");
    }
    tr.bits_per_word = bits;

    DEV_HARDWARE_SPI_Mode(SPI_MODE_0);
    DEV_HARDWARE_SPI_ChipSelect(SPI_CS_Mode_LOW);
    DEV_HARDWARE_SPI_SetBitOrder(SPI_BIT_ORDER_MSBFIRST);
    DEV_HARDWARE_SPI_setSpeed(20000000);
    DEV_HARDWARE_SPI_SetDataInterval(0);
}

void DEV_HARDWARE_SPI_beginSet(char* SPI_device, SPIMode mode, uint32_t speed)
{
    //device
    int ret = 0;
    hardware_SPI.mode = 0;
    if ((hardware_SPI.fd = open(SPI_device, O_RDWR)) < 0) {
        perror("Failed to open SPI device.\n");
        exit(1);
    }
    else {
        DEV_HARDWARE_SPI_Debug("open : %s\r\n", SPI_device);
    }

    ret = ioctl(hardware_SPI.fd, SPI_IOC_WR_BITS_PER_WORD, &bits);
    if (ret == -1)
        DEV_HARDWARE_SPI_Debug("can't set bits per word\r\n");

    ret = ioctl(hardware_SPI.fd, SPI_IOC_RD_BITS_PER_WORD, &bits);
    if (ret == -1)
        DEV_HARDWARE_SPI_Debug("can't get bits per word\r\n");

    DEV_HARDWARE_SPI_Mode(mode);
    DEV_HARDWARE_SPI_ChipSelect(SPI_CS_Mode_LOW);
    DEV_HARDWARE_SPI_setSpeed(speed);
    DEV_HARDWARE_SPI_SetDataInterval(0);
}


/******************************************************************************
function:   SPI device End
parameter:
Info:
******************************************************************************/
void DEV_HARDWARE_SPI_end(void)
{
    hardware_SPI.mode = 0;
    if (close(hardware_SPI.fd) != 0) {
        DEV_HARDWARE_SPI_Debug("Failed to close SPI device\r\n");
        perror("Failed to close SPI device.\n");
    }
}

/******************************************************************************
function:   Set SPI speed
parameter:
Info:   Return 1 success
        Return -1 failed
******************************************************************************/
int DEV_HARDWARE_SPI_setSpeed(uint32_t speed)
{
    uint32_t speed1 = hardware_SPI.speed;

    hardware_SPI.speed = speed;

    //Write speed
    if (ioctl(hardware_SPI.fd, SPI_IOC_WR_MAX_SPEED_HZ, &speed) == -1) {
        DEV_HARDWARE_SPI_Debug("can't set max speed hz\r\n");
        hardware_SPI.speed = speed1;//Setting failure rate unchanged
        return -1;
    }

    //Read the speed of just writing
    if (ioctl(hardware_SPI.fd, SPI_IOC_RD_MAX_SPEED_HZ, &speed) == -1) {
        DEV_HARDWARE_SPI_Debug("can't get max speed hz\r\n");
        hardware_SPI.speed = speed1;//Setting failure rate unchanged
        return -1;
    }
    hardware_SPI.speed = speed;
    tr.speed_hz = hardware_SPI.speed;
    return 1;
}

/******************************************************************************
function:   Set SPI Mode
parameter:
Info:
    SPIMode:
        SPI_MODE0
        SPI_MODE1
        SPI_MODE2
        SPI_MODE3
    Return :
        Return 1 success
        Return -1 failed
******************************************************************************/
int DEV_HARDWARE_SPI_Mode(SPIMode mode)
{
    hardware_SPI.mode &= 0xfC;//Clear low 2 digits
    hardware_SPI.mode |= mode;//Setting mode

    //Write device
    if (ioctl(hardware_SPI.fd, SPI_IOC_WR_MODE, &hardware_SPI.mode) == -1) {
        DEV_HARDWARE_SPI_Debug("can't set spi mode\r\n");
        return -1;
    }
    return 1;
}

/******************************************************************************
function:   Set SPI CS Enable
parameter:
Info:
    EN:
        DISABLE
        ENABLE
    Return :
        Return 1 success
        Return -1 failed
******************************************************************************/
int DEV_HARDWARE_SPI_CSEN(SPICSEN EN)
{
    if (EN == ENABLE) {
        hardware_SPI.mode |= SPI_NO_CS;
    }
    else {
        hardware_SPI.mode &= ~SPI_NO_CS;
    }
    //Write device
    if (ioctl(hardware_SPI.fd, SPI_IOC_WR_MODE, &hardware_SPI.mode) == -1) {
        DEV_HARDWARE_SPI_Debug("can't set spi CS EN\r\n");
        return -1;
    }
    return 1;
}

/******************************************************************************
function:   Chip Select
parameter:
Info:
    CS_Mode:
        SPI_CS_Mode_LOW
        SPI_CS_Mode_HIGH
        SPI_CS_Mode_NONE
    Return :
        Return 1 success
        Return -1 failed
******************************************************************************/
int DEV_HARDWARE_SPI_ChipSelect(SPIChipSelect CS_Mode)
{
    if (CS_Mode == SPI_CS_Mode_HIGH) {
        hardware_SPI.mode |= SPI_CS_HIGH;
        hardware_SPI.mode &= ~SPI_NO_CS;
        DEV_HARDWARE_SPI_Debug("CS HIGH \r\n");
    }
    else if (CS_Mode == SPI_CS_Mode_LOW) {
        hardware_SPI.mode &= ~SPI_CS_HIGH;
        hardware_SPI.mode &= ~SPI_NO_CS;
    }
    else if (CS_Mode == SPI_CS_Mode_NONE) {
        hardware_SPI.mode |= SPI_NO_CS;
    }

    if (ioctl(hardware_SPI.fd, SPI_IOC_WR_MODE, &hardware_SPI.mode) == -1) {
        DEV_HARDWARE_SPI_Debug("can't set spi mode\r\n");
        return -1;
    }
    return 1;
}

/******************************************************************************
function:   Sets the SPI bit order
parameter:
Info:
    Order:
        SPI_BIT_ORDER_LSBFIRST
        SPI_BIT_ORDER_MSBFIRST
    Return :
        Return 1 success
        Return -1 failed
******************************************************************************/
int DEV_HARDWARE_SPI_SetBitOrder(SPIBitOrder Order)
{
    if (Order == SPI_BIT_ORDER_LSBFIRST) {
        hardware_SPI.mode |= SPI_LSB_FIRST;
        DEV_HARDWARE_SPI_Debug("SPI_LSB_FIRST\r\n");
    }
    else if (Order == SPI_BIT_ORDER_MSBFIRST) {
        hardware_SPI.mode &= ~SPI_LSB_FIRST;
        DEV_HARDWARE_SPI_Debug("SPI_MSB_FIRST\r\n");
    }

    // DEV_HARDWARE_SPI_Debug("hardware_SPI.mode = 0x%02x\r\n", hardware_SPI.mode);
    int fd = ioctl(hardware_SPI.fd, SPI_IOC_WR_MODE, &hardware_SPI.mode);
    DEV_HARDWARE_SPI_Debug("fd = %d\r\n", fd);
    if (fd == -1) {
        DEV_HARDWARE_SPI_Debug("can't set spi SPI_LSB_FIRST\r\n");
        return -1;
    }
    return 1;
}

/******************************************************************************
function:   Sets the SPI Bus Mode
parameter:
Info:
    Order:
        SPI_3WIRE_Mode
        SPI_4WIRE_Mode
    Return :
        Return 1 success
        Return -1 failed
******************************************************************************/
int DEV_HARDWARE_SPI_SetBusMode(BusMode mode)
{
    if (mode == SPI_3WIRE_Mode) {
        hardware_SPI.mode |= SPI_3WIRE;
    }
    else if (mode == SPI_4WIRE_Mode) {
        hardware_SPI.mode &= ~SPI_3WIRE;
    }
    if (ioctl(hardware_SPI.fd, SPI_IOC_WR_MODE, &hardware_SPI.mode) == -1) {
        DEV_HARDWARE_SPI_Debug("can't set spi mode\r\n");
        return -1;
    }
    return 1;
}

/******************************************************************************
function:
    Time interval after transmission of one byte during continuous transmission
parameter:
    us :   Interval time (us)
Info:
******************************************************************************/
void DEV_HARDWARE_SPI_SetDataInterval(uint16_t us)
{
    hardware_SPI.delay = us;
    tr.delay_usecs = hardware_SPI.delay;
}

/******************************************************************************
function: SPI port sends one byte of data
parameter:
    buf :   Sent data
Info:
******************************************************************************/
uint8_t DEV_HARDWARE_SPI_TransferByte(uint8_t buf)
{
    uint8_t rbuf[1];
    tr.len = 1;
    tr.tx_buf = (unsigned long)&buf;
    tr.rx_buf = (unsigned long)rbuf;

    //ioctl Operation, transmission of data
    if (ioctl(hardware_SPI.fd, SPI_IOC_MESSAGE(1), &tr) < 1)
        DEV_HARDWARE_SPI_Debug("can't send spi message\r\n");
    return rbuf[0];
}

/******************************************************************************
function: The SPI port reads a byte
parameter:
Info: Return read data
******************************************************************************/
int DEV_HARDWARE_SPI_Transfer(uint8_t* buf, uint32_t len)
{
    tr.len = len;
    tr.tx_buf = (unsigned long)buf;
    tr.rx_buf = (unsigned long)buf;

    //ioctl Operation, transmission of data
    if (ioctl(hardware_SPI.fd, SPI_IOC_MESSAGE(1), &tr) < 1) {
        DEV_HARDWARE_SPI_Debug("can't send spi message\r\n");
        return -1;
    }

    return 1;
}

#pragma endregion

#pragma region ADS1263

UBYTE ScanMode = 0;

/******************************************************************************
function:   Module reset
parameter:
Info:
******************************************************************************/
static void ADS1263_reset()
{
    DEV_Digital_Write(DEV_RST_PIN, 1);
    DEV_Delay_ms(300);
    DEV_Digital_Write(DEV_RST_PIN, 0);
    DEV_Delay_ms(300);
    DEV_Digital_Write(DEV_RST_PIN, 1);
    DEV_Delay_ms(300);
}

/******************************************************************************
function:   send command
parameter:
        Cmd: command
Info:
******************************************************************************/
static void ADS1263_WriteCmd(UBYTE Cmd)
{
    DEV_Digital_Write(DEV_CS_PIN, 0);
    DEV_SPI_WriteByte(Cmd);
    DEV_Digital_Write(DEV_CS_PIN, 1);
}

/******************************************************************************
function:   Write a data to the destination register
parameter:
        Reg : Target register
        data: Written data
Info:
******************************************************************************/
static void ADS1263_WriteReg(UBYTE Reg, UBYTE data)
{
    DEV_Digital_Write(DEV_CS_PIN, 0);
    DEV_SPI_WriteByte(CMD_WREG | Reg);
    DEV_SPI_WriteByte(0x00);
    DEV_SPI_WriteByte(data);
    DEV_Digital_Write(DEV_CS_PIN, 1);
}

/******************************************************************************
function:   Read a data from the destination register
parameter:
        Reg : Target register
Info:
    Return the read data
******************************************************************************/
static UBYTE ADS1263_Read_data(UBYTE Reg)
{
    UBYTE temp = 0;
    DEV_Digital_Write(DEV_CS_PIN, 0);
    DEV_SPI_WriteByte(CMD_RREG | Reg);
    DEV_SPI_WriteByte(0x00);
    // DEV_Delay_ms(1);
    temp = DEV_SPI_ReadByte();
    DEV_Digital_Write(DEV_CS_PIN, 1);
    return temp;
}

/******************************************************************************
function:   Check data
parameter:
        val : 4 bytes(ADC2 is 3 bytes) data
        byt : CRC byte
Info:
        Check success, return 0
******************************************************************************/
static UBYTE ADS1263_Checksum(UDOUBLE val, UBYTE byt)
{
    UBYTE sum = 0;
    UBYTE mask = -1;        // 8 bits mask, 0xff
    while (val) {
        sum += val & mask;  // only add the lower values
        val >>= 8;          // shift down
    }
    sum += 0x9b;
    // printf("--- %x %x--- \r\n", sum, byt);
    return sum ^ byt;       // if equal, this will be 0
}

/******************************************************************************
function:   Waiting for a busy end
parameter:
Info:
    Timeout indicates that the operation is not working properly.
******************************************************************************/
static void ADS1263_WaitDRDY()
{
    // printf("ADS1263_WaitDRDY \r\n");
    UDOUBLE i = 0;
    while (1) {
        if (DEV_Digital_Read(DEV_DRDY_PIN) == 0)
            break;
        if (i >= 4000000) {
            printf("Time Out ...\r\n");
            break;
        }
    }
    // printf("ADS1263_WaitDRDY Release \r\n");
}

/******************************************************************************
function:  Read device ID
parameter:
Info:
******************************************************************************/
UBYTE ADS1263_ReadChipID()
{
    UBYTE id;
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
void ADS1263_SetMode(UBYTE Mode)
{
    if (Mode == 0) {
        ScanMode = 0;
    }
    else {
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
void ADS1263_ConfigADC1(ADS1263_GAIN gain, ADS1263_DRATE drate, ADS1263_DELAY delay)
{
    UBYTE MODE2 = 0x80;             //0x80:PGA bypassed, 0x00:PGA enabled
    MODE2 |= (gain << 4) | drate;
    ADS1263_WriteReg(REG_MODE2, MODE2);
    DEV_Delay_ms(1);
    if (ADS1263_Read_data(REG_MODE2) == MODE2)
        printf("REG_MODE2 success \r\n");
    else
        printf("REG_MODE2 unsuccess \r\n");

    UBYTE REFMUX = 0x24;        //0x00:+-2.5V as REF, 0x24:VDD,VSS as REF
    ADS1263_WriteReg(REG_REFMUX, REFMUX);
    DEV_Delay_ms(1);
    if (ADS1263_Read_data(REG_REFMUX) == REFMUX)
        printf("REG_REFMUX success \r\n");
    else
        printf("REG_REFMUX unsuccess \r\n");

    UBYTE MODE0 = delay;
    ADS1263_WriteReg(REG_MODE0, MODE0);
    DEV_Delay_ms(1);
    if (ADS1263_Read_data(REG_MODE0) == MODE0)
        printf("REG_MODE0 success \r\n");
    else
        printf("REG_MODE0 unsuccess \r\n");

    UBYTE MODE1 = 0x84; // Digital Filter; 0x84:FIR, 0x64:Sinc4, 0x44:Sinc3, 0x24:Sinc2, 0x04:Sinc1
    ADS1263_WriteReg(REG_MODE1, MODE1);
    DEV_Delay_ms(1);
    if (ADS1263_Read_data(REG_MODE1) == MODE1)
        printf("REG_MODE1 success \r\n");
    else
        printf("REG_MODE1 unsuccess \r\n");
}

/******************************************************************************
function:  Configure ADC gain and sampling speed
parameter:
    gain : Enumeration type gain
    drate: Enumeration type sampling speed
Info:
******************************************************************************/
void ADS1263_ConfigADC2(ADS1263_ADC2_GAIN gain, ADS1263_ADC2_DRATE drate, ADS1263_DELAY delay)
{
    UBYTE ADC2CFG = 0x20;               //REF, 0x20:VAVDD and VAVSS, 0x00:+-2.5V
    ADC2CFG |= (drate << 6) | gain;
    ADS1263_WriteReg(REG_ADC2CFG, ADC2CFG);
    DEV_Delay_ms(1);
    if (ADS1263_Read_data(REG_ADC2CFG) == ADC2CFG)
        printf("REG_ADC2CFG success \r\n");
    else
        printf("REG_ADC2CFG unsuccess \r\n");

    UBYTE MODE0 = delay;
    ADS1263_WriteReg(REG_MODE0, MODE0);
    DEV_Delay_ms(1);
    if (ADS1263_Read_data(REG_MODE0) == MODE0)
        printf("REG_MODE0 success \r\n");
    else
        printf("REG_MODE0 unsuccess \r\n");
}

/******************************************************************************
function:  Device initialization
parameter:
Info:
******************************************************************************/
UBYTE ADS1263_init_ADC1(ADS1263_DRATE rate)
{
    ADS1263_reset();
    if (ADS1263_ReadChipID() == 1) {
        printf("ID Read success \r\n");
    }
    else {
        printf("ID Read failed \r\n");
        return 1;
    }
    ADS1263_WriteCmd(CMD_STOP1);
    ADS1263_ConfigADC1(ADS1263_GAIN_1, rate, ADS1263_DELAY_35us);
    ADS1263_WriteCmd(CMD_START1);
    return 0;
}
UBYTE ADS1263_init_ADC2(ADS1263_ADC2_DRATE rate)
{
    ADS1263_reset();
    if (ADS1263_ReadChipID() == 1) {
        printf("ID Read success \r\n");
    }
    else {
        printf("ID Read failed \r\n");
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
static void ADS1263_SetChannal(UBYTE Channal)
{
    if (Channal > 10) {
        return;
    }
    UBYTE INPMUX = (Channal << 4) | 0x0a;       //0x0a:VCOM as Negative Input
    ADS1263_WriteReg(REG_INPMUX, INPMUX);
    if (ADS1263_Read_data(REG_INPMUX) == INPMUX) {
        // printf("ADS1263_ADC1_SetChannal success \r\n");
    }
    else {
        printf("ADS1263_ADC1_SetChannal unsuccess \r\n");
    }
}

/******************************************************************************
function:  Set the channel to be read
parameter:
    Channal : Set channel number
Info:
******************************************************************************/
static void ADS1263_SetChannal_ADC2(UBYTE Channal)
{
    if (Channal > 10) {
        return;
    }
    UBYTE INPMUX = (Channal << 4) | 0x0a;       //0x0a:VCOM as Negative Input
    ADS1263_WriteReg(REG_ADC2MUX, INPMUX);
    if (ADS1263_Read_data(REG_ADC2MUX) == INPMUX) {
        // printf("ADS1263_ADC2_SetChannal success \r\n");
    }
    else {
        printf("ADS1263_ADC2_SetChannal unsuccess \r\n");
    }
}

/******************************************************************************
function:  Set the channel to be read
parameter:
    Channal : Set channel number
Info:
******************************************************************************/
void ADS1263_SetDiffChannal(UBYTE Channal)
{
    UBYTE INPMUX;
    if (Channal == 0) {
        INPMUX = (0 << 4) | 1;    //DiffChannal   AIN0-AIN1
    }
    else if (Channal == 1) {
        INPMUX = (2 << 4) | 3;    //DiffChannal   AIN2-AIN3
    }
    else if (Channal == 2) {
        INPMUX = (4 << 4) | 5;    //DiffChannal   AIN4-AIN5
    }
    else if (Channal == 3) {
        INPMUX = (6 << 4) | 7;    //DiffChannal   AIN6-AIN7
    }
    else if (Channal == 4) {
        INPMUX = (8 << 4) | 9;    //DiffChannal   AIN8-AIN9
    }
    ADS1263_WriteReg(REG_INPMUX, INPMUX);
    if (ADS1263_Read_data(REG_INPMUX) == INPMUX) {
        // printf("ADS1263_SetDiffChannal success \r\n");
    }
    else {
        printf("ADS1263_SetDiffChannal unsuccess \r\n");
    }
}

/******************************************************************************
function:  Set the channel to be read
parameter:
    Channal : Set channel number
Info:
******************************************************************************/
void ADS1263_SetDiffChannal_ADC2(UBYTE Channal)
{
    UBYTE INPMUX;
    if (Channal == 0) {
        INPMUX = (0 << 4) | 1;    //DiffChannal   AIN0-AIN1
    }
    else if (Channal == 1) {
        INPMUX = (2 << 4) | 3;    //DiffChannal   AIN2-AIN3
    }
    else if (Channal == 2) {
        INPMUX = (4 << 4) | 5;    //DiffChannal   AIN4-AIN5
    }
    else if (Channal == 3) {
        INPMUX = (6 << 4) | 7;    //DiffChannal   AIN6-AIN7
    }
    else if (Channal == 4) {
        INPMUX = (8 << 4) | 9;    //DiffChannal   AIN8-AIN9
    }
    ADS1263_WriteReg(REG_ADC2MUX, INPMUX);
    if (ADS1263_Read_data(REG_ADC2MUX) == INPMUX) {
        // printf("ADS1263_SetDiffChannal_ADC2 success \r\n");
    }
    else {
        printf("ADS1263_SetDiffChannal_ADC2 unsuccess \r\n");
    }
}

/******************************************************************************
function:  Read ADC data
parameter:
Info:
******************************************************************************/
static UDOUBLE ADS1263_Read_ADC1_Data()
{
    UDOUBLE read = 0;
    UBYTE buf[4] = { 0, 0, 0, 0 };
    UBYTE Status, CRC;
    DEV_Digital_Write(DEV_CS_PIN, 0);
    do {
        DEV_SPI_WriteByte(CMD_RDATA1);
        // DEV_Delay_ms(10);
        Status = DEV_SPI_ReadByte();
    } while ((Status & 0x40) == 0);

    buf[0] = DEV_SPI_ReadByte();
    buf[1] = DEV_SPI_ReadByte();
    buf[2] = DEV_SPI_ReadByte();
    buf[3] = DEV_SPI_ReadByte();
    CRC = DEV_SPI_ReadByte();
    DEV_Digital_Write(DEV_CS_PIN, 1);
    read |= ((UDOUBLE)buf[0] << 24);
    read |= ((UDOUBLE)buf[1] << 16);
    read |= ((UDOUBLE)buf[2] << 8);
    read |= (UDOUBLE)buf[3];
    // printf("%x %x %x %x %x %x\r\n", Status, buf[0], buf[1], buf[2], buf[3], CRC);
    if (ADS1263_Checksum(read, CRC) != 0)
        printf("ADC1 Data read error! \r\n");
    return read;
}

/******************************************************************************
function:  Read ADC data
parameter:
Info:
******************************************************************************/
static UDOUBLE ADS1263_Read_ADC2_Data()
{
    UDOUBLE read = 0;
    UBYTE buf[4] = { 0, 0, 0, 0 };
    UBYTE Status, CRC;

    DEV_Digital_Write(DEV_CS_PIN, 0);
    do {
        DEV_SPI_WriteByte(CMD_RDATA2);
        // DEV_Delay_ms(10);
        Status = DEV_SPI_ReadByte();
    } while ((Status & 0x80) == 0);

    buf[0] = DEV_SPI_ReadByte();
    buf[1] = DEV_SPI_ReadByte();
    buf[2] = DEV_SPI_ReadByte();
    buf[3] = DEV_SPI_ReadByte();
    CRC = DEV_SPI_ReadByte();
    DEV_Digital_Write(DEV_CS_PIN, 1);
    read |= ((UDOUBLE)buf[0] << 16);
    read |= ((UDOUBLE)buf[1] << 8);
    read |= (UDOUBLE)buf[2];
    // printf("%x %x %x %x %x\r\n", Status, buf[0], buf[1], buf[2], CRC);
    if (ADS1263_Checksum(read, CRC) != 0)
        printf("ADC2 Data read error! \r\n");
    return read;
}

/******************************************************************************
function:  Read ADC specified channel data
parameter:
    Channel: Channel number
Info:
******************************************************************************/
UDOUBLE ADS1263_GetChannalValue(UBYTE Channel)
{
    UDOUBLE Value = 0;
    if (ScanMode == 0) {// 0  Single-ended input  10 channel1 Differential input  5 channe 
        if (Channel > 10) {
            return 0;
        }
        ADS1263_SetChannal(Channel);
        // DEV_Delay_ms(2);
        // ADS1263_WriteCmd(CMD_START1);
        // DEV_Delay_ms(2);
        ADS1263_WaitDRDY();
        Value = ADS1263_Read_ADC1_Data();
    }
    else {
        if (Channel > 4) {
            return 0;
        }
        ADS1263_SetDiffChannal(Channel);
        // DEV_Delay_ms(2);
        // ADS1263_WriteCmd(CMD_START1);
        // DEV_Delay_ms(2);
        ADS1263_WaitDRDY();
        Value = ADS1263_Read_ADC1_Data();
    }
    // printf("Get IN%d value success \r\n", Channel);
    return Value;
}

/******************************************************************************
function:  Read ADC specified channel data
parameter:
    Channel: Channel number
Info:
******************************************************************************/
UDOUBLE ADS1263_GetChannalValue_ADC2(UBYTE Channel)
{
    UDOUBLE Value = 0;
    if (ScanMode == 0) {// 0  Single-ended input  10 channel1 Differential input  5 channe 
        if (Channel > 10) {
            return 0;
        }
        ADS1263_SetChannal_ADC2(Channel);
        // DEV_Delay_ms(2);
        ADS1263_WriteCmd(CMD_START2);
        // DEV_Delay_ms(2);
        Value = ADS1263_Read_ADC2_Data();
    }
    else {
        if (Channel > 4) {
            return 0;
        }
        ADS1263_SetDiffChannal_ADC2(Channel);
        // DEV_Delay_ms(2);
        ADS1263_WriteCmd(CMD_START2);
        // DEV_Delay_ms(2);
        Value = ADS1263_Read_ADC2_Data();
    }
    // printf("Get IN%d value success \r\n", Channel);
    return Value;
}

/******************************************************************************
function:  Read data from all channels
parameter:
    ADC_Value : ADC Value
Info:
******************************************************************************/
void ADS1263_GetAll(UBYTE* List, UDOUBLE* Value, int Number)
{
    UBYTE i;
    for (i = 0; i < Number; i++) {
        Value[i] = ADS1263_GetChannalValue(List[i]);
        // ADS1263_WriteCmd(CMD_STOP1);
        // DEV_Delay_ms(20);
    }
}

/******************************************************************************
function:  Read data from all channels
parameter:
    ADC_Value : ADC Value
Info:
******************************************************************************/
void ADS1263_GetAll_ADC2(UDOUBLE* ADC_Value)
{
    UBYTE i;
    for (i = 0; i < 10; i++) {
        ADC_Value[i] = ADS1263_GetChannalValue_ADC2(i);
        ADS1263_WriteCmd(CMD_STOP2);
        // DEV_Delay_ms(20);
    }
    // printf("----------Read ADC2 value success----------\r\n");
}

/******************************************************************************
function:  RTD Test function
parameter:
    delay : Conversion delay
    gain :  Conversion gain
    drate : speed
Info:
******************************************************************************/
UDOUBLE ADS1263_RTD(ADS1263_DELAY delay, ADS1263_GAIN gain, ADS1263_DRATE drate)
{
    UDOUBLE Value;

    //MODE0 (CHOP OFF)
    UBYTE MODE0 = delay;
    ADS1263_WriteReg(REG_MODE0, MODE0);
    DEV_Delay_ms(1);

    //(IDACMUX) IDAC2 AINCOM,IDAC1 AIN3
    UBYTE IDACMUX = (0x0a << 4) | 0x03;
    ADS1263_WriteReg(REG_IDACMUX, IDACMUX);
    DEV_Delay_ms(1);

    //((IDACMAG)) IDAC2 = IDAC1 = 250uA
    UBYTE IDACMAG = (0x03 << 4) | 0x03;
    ADS1263_WriteReg(REG_IDACMAG, IDACMAG);
    DEV_Delay_ms(1);

    UBYTE MODE2 = (gain << 4) | drate;
    ADS1263_WriteReg(REG_MODE2, MODE2);
    DEV_Delay_ms(1);

    //INPMUX (AINP = AIN7, AINN = AIN6)
    UBYTE INPMUX = (0x07 << 4) | 0x06;
    ADS1263_WriteReg(REG_INPMUX, INPMUX);
    DEV_Delay_ms(1);

    // REFMUX AIN4 AIN5
    UBYTE REFMUX = (0x03 << 3) | 0x03;
    ADS1263_WriteReg(REG_REFMUX, REFMUX);
    DEV_Delay_ms(1);

    //Read one conversion
    ADS1263_WriteCmd(CMD_START1);
    DEV_Delay_ms(10);
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
void ADS1263_DAC(ADS1263_DAC_VOLT volt, UBYTE isPositive, UBYTE isOpen)
{
    UBYTE Reg, Value;

    if (isPositive)
        Reg = REG_TDACP;        // IN6
    else
        Reg = REG_TDACN;        // IN7

    if (isOpen)
        Value = volt | 0x80;
    else
        Value = 0x00;

    ADS1263_WriteReg(Reg, Value);
}

#pragma endregion
