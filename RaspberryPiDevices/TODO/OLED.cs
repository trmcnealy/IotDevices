using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using RaspberryPiDevices;

namespace RaspberryPiDevices;

#region Enums
/// <summary>
/// Display Flip
/// </summary>
public enum MIRROR_IMAGE : int
{
    NONE = 0x00,

    HORIZONTAL = 0x01,

    VERTICAL = 0x02,

    ORIGIN = 0x03,
}

/// <summary>
/// The size of the point
/// </summary>
public enum DOT_PIXEL : int
{
    /// <summary>
    /// 1 x 1
    /// </summary>
    _1X1 = 1,

    /// <summary>
    /// 2 X 2
    /// </summary>
    _2X2,

    /// <summary>
    /// 3 X 3
    /// </summary>
    _3X3,

    /// <summary>
    /// 4 X 4
    /// </summary>
    _4X4,

    /// <summary>
    /// 5 X 5
    /// </summary>
    _5X5,

    /// <summary>
    /// 6 X 6
    /// </summary>
    _6X6,

    /// <summary>
    /// 7 X 7
    /// </summary>
    _7X7,

    /// <summary>
    /// 8 X 8
    /// </summary>
    _8X8,
}

/// <summary>
/// Point size fill style
/// </summary>
public enum DOT_FILL_STYLE : int
{
    /// <summary>
    /// dot pixel 1 x 1
    /// </summary>
    AROUND = 1,

    /// <summary>
    /// dot pixel 2 X 2
    /// </summary>
    RIGHTUP,
}

/// <summary>
/// Line style, solid or dashed
/// </summary>
public enum LINE_STYLE : int
{
    SOLID = 0,

    DOTTED,
}

/// <summary>
/// Whether the graphic is filled
/// </summary>
public enum DRAW_FILL : int
{
    EMPTY = 0,

    FULL,
}

#endregion

/// <summary>
/// ASCII
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public readonly struct AsciiFont
{
    public enum Size
    {
        Font8 = 8,
        Font12 = 12,
        Font16 = 16,
        Font20 = 20
    }

    public readonly int Width;

    public readonly int Height;

    public readonly byte[] Table;

    public AsciiFont(Size size)
    {
        switch (size)
        {
            case Size.Font8:
            {
                Width = Fonts.Font8_TableWidth;
                Height = Fonts.Font8_TableHeight;
                Table = Fonts.Font8_Table;
                break;
            }
            case Size.Font12:
            {
                Width = Fonts.Font12_TableWidth;
                Height = Fonts.Font12_TableHeight;
                Table = Fonts.Font12_Table;
                break;
            }
            case Size.Font16:
            {
                Width = Fonts.Font16_TableWidth;
                Height = Fonts.Font16_TableHeight;
                Table = Fonts.Font16_Table;
                break;
            }
            case Size.Font20:
            {
                Width = Fonts.Font20_TableWidth;
                Height = Fonts.Font20_TableHeight;
                Table = Fonts.Font20_Table;
                break;
            }
            default:
            {            
                Table = new byte[0];
                break;
            }
        }
    }
}

/// <summary>
/// Custom structure of a time attribute
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct PaintTime
{
    /// <summary>
    /// 0000
    /// </summary>
    public int Year;

    /// <summary>
    /// 1 - 12
    /// </summary>
    public byte Month;

    /// <summary>
    /// 1 - 30
    /// </summary>
    public byte Day;

    /// <summary>
    /// 0 - 23
    /// </summary>
    public byte Hour;

    /// <summary>
    /// 0 - 59
    /// </summary>
    public byte Min;

    /// <summary>
    /// 0 - 59
    /// </summary>
    public byte Sec;
}

public class Paint
{
    public const int BLACK = 0x00;
    public const int WHITE = 0xFF;

    public const int ROTATE_0 = 0;
    public const int ROTATE_90 = 90;
    public const int ROTATE_180 = 180;
    public const int ROTATE_270 = 270;

    public const int IMAGE_BACKGROUND = WHITE;
    public const int FONT_FOREGROUND = BLACK;
    public const int FONT_BACKGROUND = WHITE;

    public byte[] Image;

    public int Width;

    public int Height;

    public int WidthMemory;

    public int HeightMemory;

    public int Color;

    public int Rotate;

    public MIRROR_IMAGE Mirror;

    public int WidthByte;

    public int HeightByte;

    public int Scale;

    public Paint(int width, int height, int scale = 1, int rotate = 0, MIRROR_IMAGE mirror = MIRROR_IMAGE.NONE)
    {
        Width = width;
        Height = height;


        Image = new byte[Width * Height];

        WidthMemory = Width;
        HeightMemory = Height;
        WidthByte = (Width % 8 == 0) ? (Width / 8) : (Width / 8 + 1);
        HeightByte = Height;

        Color = WHITE;
        Rotate = rotate;
        Mirror = mirror;
        Scale = scale;

        if (Rotate == ROTATE_0 || Rotate == ROTATE_180)
        {
            Width = width;
            Height = height;
        }
        else
        {
            Width = height;
            Height = width;
        }
    }

    public void SelectImage(byte[] image)
    {
        Image = image;
    }

    public void SetRotate(int rotate)
    {
        if (rotate == ROTATE_0 || rotate == ROTATE_90 || rotate == ROTATE_180 || rotate == ROTATE_270)
        {
            //Debug("Set image Rotate %d\r\n", Rotate);
            Rotate = rotate;
        }
        else
        {
            //Debug("rotate = 0, 90, 180, 270\r\n");
        }
    }

    public void SetScale(byte scale)
    {
        if (scale == 2)
        {
            Scale = scale;
            WidthByte = (int)((WidthMemory % 8 == 0) ? (WidthMemory / 8) : (WidthMemory / 8 + 1));
        }
        else if (scale == 4)
        {
            Scale = scale;
            WidthByte = (int)((WidthMemory % 4 == 0) ? (WidthMemory / 4) : (WidthMemory / 4 + 1));
        }
        else if (scale == 16)
        {
            Scale = scale;
            WidthByte = (int)((WidthMemory % 2 == 0) ? (WidthMemory / 2) : (WidthMemory / 2 + 1));
        }
        else if (scale == 65)
        {
            Scale = scale;
            WidthByte = (int)(WidthMemory * 2);
        }
        else
        {
            //Debug("Set Scale Input parameter error\r\n");
            //Debug("Scale Only support: 2 4 16 65\r\n");
        }
    }

    public void SetMirroring(MIRROR_IMAGE mirror)
    {
        if (mirror == MIRROR_IMAGE.NONE || mirror == MIRROR_IMAGE.HORIZONTAL || mirror == MIRROR_IMAGE.VERTICAL || mirror == MIRROR_IMAGE.ORIGIN)
        {
            //Debug("mirror image x:%s, y:%s\r\n", (mirror & 0x01) ? "mirror" : "none", ((mirror >> 1) & 0x01) ? "mirror" : "none");
            Mirror = mirror;
        }
        else
        {
            //Debug("mirror should be MIRROR_IMAGE.NONE, MIRROR_IMAGE.HORIZONTAL, MIRROR_IMAGE.VERTICAL or MIRROR_IMAGE.ORIGIN\r\n");
        }
    }

    public void SetPixel(int xpoint, int ypoint, int color)
    {
        if (xpoint > Width || ypoint > Height)
        {
            //Debug("Exceeding display boundaries\r\n");
            return;
        }
        int X, Y;

        switch (Rotate)
        {
            case 0:
                X = xpoint;
                Y = ypoint;
                break;
            case 90:
                X = (int)(WidthMemory - ypoint - 1);
                Y = xpoint;
                break;
            case 180:
                X = (int)(WidthMemory - xpoint - 1);
                Y = (int)(HeightMemory - ypoint - 1);
                break;
            case 270:
                X = ypoint;
                Y = (int)(HeightMemory - xpoint - 1);
                break;
            default:
                return;
        }

        switch (Mirror)
        {
            case MIRROR_IMAGE.NONE:
                break;
            case MIRROR_IMAGE.HORIZONTAL:
                X = (int)(WidthMemory - X - 1);
                break;
            case MIRROR_IMAGE.VERTICAL:
                Y = (int)(HeightMemory - Y - 1);
                break;
            case MIRROR_IMAGE.ORIGIN:
                X = (int)(WidthMemory - X - 1);
                Y = (int)(HeightMemory - Y - 1);
                break;
            default:
                return;
        }

        if (X > WidthMemory || Y > HeightMemory)
        {
            //Debug("Exceeding display boundaries\r\n");
            return;
        }

        if (Scale == 2)
        {
            int Addr = (X / 8 + Y * WidthByte);
            byte Rdata = Image[Addr];
            if (color == BLACK)
            {
                Image[Addr] = (byte)(Rdata & ~(0x80 >> (X % 8)));
            }
            else
            {
                Image[Addr] = (byte)(Rdata | (0x80 >> (X % 8)));
            }
        }
        else if (Scale == 4)
        {
            int Addr = X / 4 + Y * WidthByte;
            color = (int)(color % 4);//Guaranteed color scale is 4  --- 0~3
            byte Rdata = Image[Addr];

            Rdata = (byte)(Rdata & (~(0xC0 >> ((X % 4) * 2))));
            Image[Addr] = (byte)(Rdata | ((color << 6) >> ((X % 4) * 2)));
        }
        else if (Scale == 16)
        {
            int Addr = X / 2 + Y * WidthByte;
            byte Rdata = Image[Addr];
            color = (int)(color % 16);
            Rdata = (byte)(Rdata & (~(0xf0 >> ((X % 2) * 4))));
            Image[Addr] = (byte)(Rdata | ((color << 4) >> ((X % 2) * 4)));
        }
        else if (Scale == 65)
        {
            int Addr = X * 2 + Y * WidthByte;
            Image[Addr] = (byte)(0xff & (color >> 8));
            Image[Addr + 1] = (byte)(0xff & color);
        }

    }

    public void Clear(int color)
    {
        if (Scale == 2 || Scale == 4)
        {
            for (int Y = 0; Y < HeightByte; Y++)
            {
                for (int X = 0; X < WidthByte; X++)
                {//8 pixel =  1 byte
                    int Addr = X + Y * WidthByte;
                    Image[Addr] = (byte)(color);
                }
            }
        }
        else if (Scale == 16)
        {
            for (int Y = 0; Y < HeightByte; Y++)
            {
                for (int X = 0; X < WidthByte; X++)
                {//8 pixel =  1 byte
                    int Addr = X + Y * WidthByte;
                    color = (int)(color & 0x0f);
                    Image[Addr] = (byte)((color << 4) | color);
                }
            }
        }
        else if (Scale == 65)
        {
            for (int Y = 0; Y < HeightByte; Y++)
            {
                for (int X = 0; X < WidthByte; X++)
                {//8 pixel =  1 byte
                    int Addr = X * 2 + Y * WidthByte;
                    Image[Addr] = (byte)(0x0f & (color >> 8));
                    Image[Addr + 1] = (byte)(0x0f & color);
                }
            }
        }
    }

    public void ClearWindows(int xstart, int ystart, int xend, int yend, int color)
    {
        int X, Y;
        for (Y = ystart; Y < yend; Y++)
        {
            for (X = xstart; X < xend; X++)
            {//8 pixel =  1 byte
                SetPixel(X, Y, color);
            }
        }
    }

    public void DrawPoint(int Xpoint, int Ypoint, int color, DOT_PIXEL dot_Pixel, DOT_FILL_STYLE dot_Style)
    {
        if (Xpoint > Width || Ypoint > Height)
        {
            //Debug("DrawPoint Input exceeds the normal display range\r\n");
            Console.Write("Xpoint = %d , Width = %d  \r\n ", Xpoint, Width);
            Console.Write("Ypoint = %d , Height = %d  \r\n ", Ypoint, Height);
            return;
        }

        int XDir_Lim = (int)(2 * (int)dot_Pixel - 1);

        if (dot_Style == DOT_FILL_STYLE.AROUND)
        {
            for (int XDir_Num = 0; XDir_Num < XDir_Lim; XDir_Num++)
            {
                for (int YDir_Num = 0; YDir_Num < XDir_Lim; YDir_Num++)
                {
                    if (Xpoint + XDir_Num - (int)dot_Pixel < 0 || Ypoint + YDir_Num - (int)dot_Pixel < 0)
                    {
                        break;
                    }
                    // Console.Write("x = %d, y = %d\r\n", Xpoint + XDir_Num - Dot_Pixel, Ypoint + YDir_Num - Dot_Pixel);
                    SetPixel((int)(Xpoint + XDir_Num - (int)dot_Pixel), (int)(Ypoint + YDir_Num - (int)dot_Pixel), color);
                }
            }
        }
        else
        {
            for (int XDir_Num = 0; XDir_Num < (int)dot_Pixel; XDir_Num++)
            {
                for (int YDir_Num = 0; YDir_Num < (int)dot_Pixel; YDir_Num++)
                {
                    SetPixel((int)(Xpoint + XDir_Num - 1), (int)(Ypoint + YDir_Num - 1), color);
                }
            }
        }
    }

    public void DrawLine(int xstart, int ystart, int xend, int yend, int color, DOT_PIXEL line_width, LINE_STYLE line_Style)
    {
        if (xstart > Width || ystart > Height ||
            xend > Width || yend > Height)
        {
            //Debug("DrawLine Input exceeds the normal display range\r\n");
            return;
        }

        int Xpoint = xstart;
        int Ypoint = ystart;
        int dx = (int)xend - (int)xstart >= 0 ? xend - xstart : xstart - xend;
        int dy = (int)yend - (int)ystart <= 0 ? yend - ystart : ystart - yend;

        // Increment direction, 1 is positive, -1 is counter;
        int XAddway = xstart < xend ? 1 : -1;
        int YAddway = ystart < yend ? 1 : -1;

        //Cumulative error
        int Esp = dx + dy;
        sbyte Dotted_Len = 0;

        for (; ; )
        {
            Dotted_Len++;
            //Painted dotted line, 2 point is really virtual
            if (line_Style == LINE_STYLE.DOTTED && Dotted_Len % 3 == 0)
            {
                //Debug("LINE_DOTTED\r\n");
                if (color != 0)
                {
                    DrawPoint(Xpoint, Ypoint, BLACK, line_width, DOT_FILL_STYLE.AROUND);
                }
                else
                {
                    DrawPoint(Xpoint, Ypoint, WHITE, line_width, DOT_FILL_STYLE.AROUND);
                }

                Dotted_Len = 0;
            }
            else
            {
                DrawPoint(Xpoint, Ypoint, color, line_width, DOT_FILL_STYLE.AROUND);
            }
            if (2 * Esp >= dy)
            {
                if (Xpoint == xend)
                {
                    break;
                }

                Esp += dy;
                Xpoint += XAddway;
            }
            if (2 * Esp <= dx)
            {
                if (Ypoint == yend)
                {
                    break;
                }

                Esp += dx;
                Ypoint += YAddway;
            }
        }
    }

    //public unsafe void DrawRectangle(int xstart, int ystart, int xend, int yend, int Color, DOT_PIXEL line_width, DRAW_FILL Draw_Fill)
    //{
    //    if (xstart > Width || ystart > Height ||
    //        xend > Width || yend > Height)
    //    {
    //        //Debug("Input exceeds the normal display range\r\n");
    //        return;
    //    }

    //    if (Draw_Fill != 0)
    //    {
    //        int Ypoint;
    //        for (Ypoint = ystart; Ypoint < yend; Ypoint++)
    //        {
    //            DrawLine(xstart, Ypoint, xend, Ypoint, Color, line_width, LINE_STYLE_SOLID);
    //        }
    //    }
    //    else
    //    {
    //        DrawLine(xstart, ystart, xend, ystart, Color, line_width, LINE_STYLE.SOLID);
    //        DrawLine(xstart, ystart, xstart, yend, Color, line_width, LINE_STYLE.SOLID);
    //        DrawLine(xend, yend, xend, ystart, Color, line_width, LINE_STYLE.SOLID);
    //        DrawLine(xend, yend, xstart, yend, Color, line_width, LINE_STYLE.SOLID);
    //    }
    //}


    //public unsafe void DrawCircle(int X_Center, int Y_Center, int Radius, int Color, DOT_PIXEL line_width, DRAW_FILL Draw_Fill)
    //{
    //    if (X_Center > Width || Y_Center >= Height)
    //    {
    //        //Debug("DrawCircle Input exceeds the normal display range\r\n");
    //        return;
    //    }

    //    //Draw a circle from(0, R) as a starting point
    //    int XCurrent, YCurrent;
    //    XCurrent = 0;
    //    YCurrent = Radius;

    //    //Cumulative error,judge the next point of the logo
    //    int Esp = 3 - (Radius << 1);

    //    int sCountY;
    //    if (Draw_Fill == DRAW_FILL.FULL)
    //    {
    //        while (XCurrent <= YCurrent)
    //        { //Realistic circles
    //            for (sCountY = XCurrent; sCountY <= YCurrent; sCountY++)
    //            {
    //                DrawPoint(X_Center + XCurrent, Y_Center + sCountY, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);//1
    //                DrawPoint(X_Center - XCurrent, Y_Center + sCountY, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);//2
    //                DrawPoint(X_Center - sCountY, Y_Center + XCurrent, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);//3
    //                DrawPoint(X_Center - sCountY, Y_Center - XCurrent, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);//4
    //                DrawPoint(X_Center - XCurrent, Y_Center - sCountY, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);//5
    //                DrawPoint(X_Center + XCurrent, Y_Center - sCountY, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);//6
    //                DrawPoint(X_Center + sCountY, Y_Center - XCurrent, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);//7
    //                DrawPoint(X_Center + sCountY, Y_Center + XCurrent, Color, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);
    //            }
    //            if (Esp < 0)
    //            {
    //                Esp += 4 * XCurrent + 6;
    //            }
    //            else
    //            {
    //                Esp += 10 + 4 * (XCurrent - YCurrent);
    //                YCurrent--;
    //            }
    //            XCurrent++;
    //        }
    //    }
    //    else
    //    { //Draw a hollow circle
    //        while (XCurrent <= YCurrent)
    //        {
    //            DrawPoint(X_Center + XCurrent, Y_Center + YCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//1
    //            DrawPoint(X_Center - XCurrent, Y_Center + YCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//2
    //            DrawPoint(X_Center - YCurrent, Y_Center + XCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//3
    //            DrawPoint(X_Center - YCurrent, Y_Center - XCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//4
    //            DrawPoint(X_Center - XCurrent, Y_Center - YCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//5
    //            DrawPoint(X_Center + XCurrent, Y_Center - YCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//6
    //            DrawPoint(X_Center + YCurrent, Y_Center - XCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//7
    //            DrawPoint(X_Center + YCurrent, Y_Center + XCurrent, Color, line_width, DOT_FILL_STYLE.AROUND);//0

    //            if (Esp < 0)
    //            {
    //                Esp += 4 * XCurrent + 6;
    //            }
    //            else
    //            {
    //                Esp += 10 + 4 * (XCurrent - YCurrent);
    //                YCurrent--;
    //            }
    //            XCurrent++;
    //        }
    //    }
    //}


    public unsafe void DrawChar(int Xpoint, int Ypoint, in byte Acsii_Char, ref AsciiFont Font, int Color_Foreground, int Color_Background)
    {
        int Page, Column;

        if (Xpoint > Width || Ypoint > Height)
        {
            //Debug("DrawChar Input exceeds the normal display range\r\n");
            return;
        }

        int Char_Offset = (Acsii_Char - ' ') * Font.Height * (Font.Width / 8 + ((Font.Width % 8) != 0 ? 1 : 0));

        fixed (byte* _ptr = &Font.Table[Char_Offset])
        {
            byte* ptr = _ptr;

            for (Page = 0; Page < Font.Height; Page++)
            {
                for (Column = 0; Column < Font.Width; Column++)
                {
                    //To determine whether the font background color and screen background color is consistent
                    if (FONT_BACKGROUND == Color_Background)
                    { //this process is to speed up the scan
                        if ((*ptr & (0x80 >> (Column % 8))) != 0)
                        {
                            SetPixel(Xpoint + Column, Ypoint + Page, Color_Foreground);
                        }
                        // DrawPoint(Xpoint + Column, Ypoint + Page, Color_Foreground, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);
                    }
                    else
                    {
                        if ((*ptr & (0x80 >> (Column % 8))) != 0)
                        {
                            SetPixel(Xpoint + Column, Ypoint + Page, Color_Foreground);
                            // DrawPoint(Xpoint + Column, Ypoint + Page, Color_Foreground, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);
                        }
                        else
                        {
                            SetPixel(Xpoint + Column, Ypoint + Page, Color_Background);
                            // DrawPoint(Xpoint + Column, Ypoint + Page, Color_Background, DOT_PIXEL._1X1, DOT_FILL_STYLE.AROUND);
                        }
                    }
                    //One pixel is 8 bits
                    if (Column % 8 == 7)
                    {
                        ++ptr;
                    }
                }// Write a line
                if (Font.Width % 8 != 0)
                {
                    ptr++;
                }
            }// Write all
        }
    }


    public unsafe void DrawString(int xstart, int ystart, char[] pString, ref AsciiFont font, int color_Foreground, int color_Background)
    {
        int Xpoint = xstart;
        int Ypoint = ystart;

        if (xstart > Width || ystart > Height)
        {
            //Debug("DrawString_EN Input exceeds the normal display range\r\n");
            return;
        }
     
        byte[] utf8bytes = Encoding.Unicode.GetBytes(pString);

        byte[] asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, utf8bytes);

        for (int charIdx = 0; charIdx < asciiBytes.Length; charIdx++)
        {            
            if ((Xpoint + font.Width) > Width)
            {
                Xpoint = xstart;
                Ypoint += font.Height;
            }

            // If the Y direction is full, reposition to(xstart, ystart)
            if ((Ypoint + font.Height) > Height)
            {
                Xpoint = xstart;
                Ypoint = ystart;
            }

            DrawChar(Xpoint, Ypoint, asciiBytes[charIdx], ref font, color_Background, color_Foreground);

            //The next word of the abscissa increases the font of the broadband
            Xpoint += font.Width;
        }        
    }




    //private const int ARRAY_LEN = 255;

    //public unsafe void DrawNum(int Xpoint, int Ypoint, double number, ref AsciiFont Font, int Digit, int Color_Foreground, int Color_Background)
    //{
    //    int Num_Bit = 0, Str_Bit = 0;
    //    byte[] Str_Array = new byte[ARRAY_LEN];
    //    byte[] Num_Array = new byte[ARRAY_LEN];

    //    fixed (byte* pStr = &Str_Array[0])


    //    int temp = number;

    //    float decimals;
    //    byte i;
    //    if (Xpoint > Width || Ypoint > Height)
    //    {
    //        //Debug("DisNum Input exceeds the normal display range\r\n");
    //        return;
    //    }

    //    if (Digit > 0)
    //    {
    //        decimals = number - temp;
    //        for (i = Digit; i > 0; i--)
    //        {
    //            decimals *= 10;
    //        }
    //        temp = decimals;
    //        //Converts a number to a string
    //        for (i = Digit; i > 0; i--)
    //        {
    //            Num_Array[Num_Bit] = temp % 10 + '0';
    //            Num_Bit++;
    //            temp /= 10;
    //        }
    //        Num_Array[Num_Bit] = '.';
    //        Num_Bit++;
    //    }

    //    temp = number;
    //    //Converts a number to a string
    //    while (temp)
    //    {
    //        Num_Array[Num_Bit] = temp % 10 + '0';
    //        Num_Bit++;
    //        temp /= 10;
    //    }

    //    //The string is inverted
    //    while (Num_Bit > 0)
    //    {
    //        Str_Array[Str_Bit] = Num_Array[Num_Bit - 1];
    //        Str_Bit++;
    //        Num_Bit--;
    //    }

    //    //show
    //    DrawString_EN(Xpoint, Ypoint, (string)pStr, Font, Color_Background, Color_Foreground);
    //}


    //public unsafe void DrawTime(int xstart, int ystart, ref PaintTime pTime, ref AsciiFont Font, int Color_Foreground, int Color_Background)
    //{
    //    byte value[10] = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    //    int Dx = Font.Width;

    //    //Write data into the cache
    //    DrawChar(xstart, ystart, value[pTime.Hour / 10], Font, Color_Background, Color_Foreground);
    //    DrawChar(xstart + Dx, ystart, value[pTime.Hour % 10], Font, Color_Background, Color_Foreground);
    //    DrawChar(xstart + Dx + Dx / 4 + Dx / 2, ystart, ':', Font, Color_Background, Color_Foreground);
    //    DrawChar(xstart + Dx * 2 + Dx / 2, ystart, value[pTime.Min / 10], Font, Color_Background, Color_Foreground);
    //    DrawChar(xstart + Dx * 3 + Dx / 2, ystart, value[pTime.Min % 10], Font, Color_Background, Color_Foreground);
    //    DrawChar(xstart + Dx * 4 + Dx / 2 - Dx / 4, ystart, ':', Font, Color_Background, Color_Foreground);
    //    DrawChar(xstart + Dx * 5, ystart, value[pTime.Sec / 10], Font, Color_Background, Color_Foreground);
    //    DrawChar(xstart + Dx * 6, ystart, value[pTime.Sec % 10], Font, Color_Background, Color_Foreground);
    //}

    public void DrawBitMap(byte[] image_buffer)
    {
        int Addr = 0;

        for (int y = 0; y < HeightByte; y++)
        {
            for (int x = 0; x < WidthByte; x++)
            {//8 pixel =  1 byte
                Addr = x + y * WidthByte;
                Image[Addr] = (byte)image_buffer[Addr];
            }
        }
    }

    public void DrawBitMap_Block(byte[] image_buffer, byte Region)
    {
        int Addr = 0;

        for (int y = 0; y < HeightByte; y++)
        {
            for (int x = 0; x < WidthByte; x++)
            {//8 pixel =  1 byte
                Addr = x + y * WidthByte;
                Image[Addr] = (byte)image_buffer[Addr + (HeightByte) * WidthByte * (Region - 1)];
            }
        }
    }
}

public static class Fonts
{
    public static readonly byte Font8_TableWidth = 5;
    public static readonly byte Font8_TableHeight = 8;
    public static readonly byte[] Font8_Table = new byte[]
    {
	    // @0 ' ' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @8 '!' (5 pixels wide)
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x00, //      
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @16 '"' (5 pixels wide)
	    0x50, //  # # 
	    0x50, //  # # 
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @24 '#' (5 pixels wide)
	    0x28, //   # #
	    0x50, //  # # 
	    0xF8, // #####
	    0x50, //  # # 
	    0xF8, // #####
	    0x50, //  # # 
	    0xA0, // # #  
	    0x00, //      

	    // @32 '$' (5 pixels wide)
	    0x20, //   #  
	    0x30, //   ## 
	    0x60, //  ##  
	    0x30, //   ## 
	    0x10, //    # 
	    0x60, //  ##  
	    0x20, //   #  
	    0x00, //      

	    // @40 '%' (5 pixels wide)
	    0x20, //   #  
	    0x20, //   #  
	    0x18, //    ##
	    0x60, //  ##  
	    0x10, //    # 
	    0x10, //    # 
	    0x00, //      
	    0x00, //      

	    // @48 '&' (5 pixels wide)
	    0x00, //      
	    0x38, //   ###
	    0x20, //   #  
	    0x60, //  ##  
	    0x50, //  # # 
	    0x78, //  ####
	    0x00, //      
	    0x00, //      

	    // @56 ''' (5 pixels wide)
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @64 '(' (5 pixels wide)
	    0x10, //    # 
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x10, //    # 
	    0x00, //      

	    // @72 ')' (5 pixels wide)
	    0x40, //  #   
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x40, //  #   
	    0x00, //      

	    // @80 '*' (5 pixels wide)
	    0x20, //   #  
	    0x70, //  ### 
	    0x20, //   #  
	    0x50, //  # # 
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @88 '+' (5 pixels wide)
	    0x00, //      
	    0x20, //   #  
	    0x20, //   #  
	    0xF8, // #####
	    0x20, //   #  
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @96 ',' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x10, //    # 
	    0x20, //   #  
	    0x20, //   #  
	    0x00, //      

	    // @104 '-' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @112 '.' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @120 '/' (5 pixels wide)
	    0x10, //    # 
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x40, //  #   
	    0x40, //  #   
	    0x80, // #    
	    0x00, //      

	    // @128 '0' (5 pixels wide)
	    0x20, //   #  
	    0x50, //  # # 
	    0x50, //  # # 
	    0x50, //  # # 
	    0x50, //  # # 
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @136 '1' (5 pixels wide)
	    0x60, //  ##  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0xF8, // #####
	    0x00, //      
	    0x00, //      

	    // @144 '2' (5 pixels wide)
	    0x20, //   #  
	    0x50, //  # # 
	    0x20, //   #  
	    0x20, //   #  
	    0x40, //  #   
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @152 '3' (5 pixels wide)
	    0x20, //   #  
	    0x50, //  # # 
	    0x10, //    # 
	    0x20, //   #  
	    0x10, //    # 
	    0x60, //  ##  
	    0x00, //      
	    0x00, //      

	    // @160 '4' (5 pixels wide)
	    0x10, //    # 
	    0x30, //   ## 
	    0x50, //  # # 
	    0x78, //  ####
	    0x10, //    # 
	    0x38, //   ###
	    0x00, //      
	    0x00, //      

	    // @168 '5' (5 pixels wide)
	    0x70, //  ### 
	    0x40, //  #   
	    0x60, //  ##  
	    0x10, //    # 
	    0x50, //  # # 
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @176 '6' (5 pixels wide)
	    0x30, //   ## 
	    0x40, //  #   
	    0x60, //  ##  
	    0x50, //  # # 
	    0x50, //  # # 
	    0x60, //  ##  
	    0x00, //      
	    0x00, //      

	    // @184 '7' (5 pixels wide)
	    0x70, //  ### 
	    0x50, //  # # 
	    0x10, //    # 
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @192 '8' (5 pixels wide)
	    0x20, //   #  
	    0x50, //  # # 
	    0x20, //   #  
	    0x50, //  # # 
	    0x50, //  # # 
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @200 '9' (5 pixels wide)
	    0x30, //   ## 
	    0x50, //  # # 
	    0x50, //  # # 
	    0x30, //   ## 
	    0x10, //    # 
	    0x60, //  ##  
	    0x00, //      
	    0x00, //      

	    // @208 ':' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x20, //   #  
	    0x00, //      
	    0x00, //      
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @216 ';' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x10, //    # 
	    0x00, //      
	    0x10, //    # 
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @224 '<' (5 pixels wide)
	    0x00, //      
	    0x10, //    # 
	    0x20, //   #  
	    0xC0, // ##   
	    0x20, //   #  
	    0x10, //    # 
	    0x00, //      
	    0x00, //      

	    // @232 '=' (5 pixels wide)
	    0x00, //      
	    0x70, //  ### 
	    0x00, //      
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @240 '>' (5 pixels wide)
	    0x00, //      
	    0x40, //  #   
	    0x20, //   #  
	    0x18, //    ##
	    0x20, //   #  
	    0x40, //  #   
	    0x00, //      
	    0x00, //      

	    // @248 '?' (5 pixels wide)
	    0x20, //   #  
	    0x50, //  # # 
	    0x10, //    # 
	    0x20, //   #  
	    0x00, //      
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @256 '@' (5 pixels wide)
	    0x30, //   ## 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x58, //  # ##
	    0x48, //  #  #
	    0x40, //  #   
	    0x38, //   ###
	    0x00, //      

	    // @264 'A' (5 pixels wide)
	    0x60, //  ##  
	    0x20, //   #  
	    0x50, //  # # 
	    0x70, //  ### 
	    0x88, // #   #
	    0xD8, // ## ##
	    0x00, //      
	    0x00, //      

	    // @272 'B' (5 pixels wide)
	    0xF0, // #### 
	    0x48, //  #  #
	    0x70, //  ### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0xF0, // #### 
	    0x00, //      
	    0x00, //      

	    // @280 'C' (5 pixels wide)
	    0x70, //  ### 
	    0x50, //  # # 
	    0x40, //  #   
	    0x40, //  #   
	    0x40, //  #   
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @288 'D' (5 pixels wide)
	    0xF0, // #### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0xF0, // #### 
	    0x00, //      
	    0x00, //      

	    // @296 'E' (5 pixels wide)
	    0xF8, // #####
	    0x48, //  #  #
	    0x60, //  ##  
	    0x40, //  #   
	    0x48, //  #  #
	    0xF8, // #####
	    0x00, //      
	    0x00, //      

	    // @304 'F' (5 pixels wide)
	    0xF8, // #####
	    0x48, //  #  #
	    0x60, //  ##  
	    0x40, //  #   
	    0x40, //  #   
	    0xE0, // ###  
	    0x00, //      
	    0x00, //      

	    // @312 'G' (5 pixels wide)
	    0x70, //  ### 
	    0x40, //  #   
	    0x40, //  #   
	    0x58, //  # ##
	    0x50, //  # # 
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @320 'H' (5 pixels wide)
	    0xE8, // ### #
	    0x48, //  #  #
	    0x78, //  ####
	    0x48, //  #  #
	    0x48, //  #  #
	    0xE8, // ### #
	    0x00, //      
	    0x00, //      

	    // @328 'I' (5 pixels wide)
	    0x70, //  ### 
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @336 'J' (5 pixels wide)
	    0x38, //   ###
	    0x10, //    # 
	    0x10, //    # 
	    0x50, //  # # 
	    0x50, //  # # 
	    0x20, //   #  
	    0x00, //      
	    0x00, //      

	    // @344 'K' (5 pixels wide)
	    0xD8, // ## ##
	    0x50, //  # # 
	    0x60, //  ##  
	    0x70, //  ### 
	    0x50, //  # # 
	    0xD8, // ## ##
	    0x00, //      
	    0x00, //      

	    // @352 'L' (5 pixels wide)
	    0xE0, // ###  
	    0x40, //  #   
	    0x40, //  #   
	    0x40, //  #   
	    0x48, //  #  #
	    0xF8, // #####
	    0x00, //      
	    0x00, //      

	    // @360 'M' (5 pixels wide)
	    0xD8, // ## ##
	    0xD8, // ## ##
	    0xD8, // ## ##
	    0xA8, // # # #
	    0x88, // #   #
	    0xD8, // ## ##
	    0x00, //      
	    0x00, //      

	    // @368 'N' (5 pixels wide)
	    0xD8, // ## ##
	    0x68, //  ## #
	    0x68, //  ## #
	    0x58, //  # ##
	    0x58, //  # ##
	    0xE8, // ### #
	    0x00, //      
	    0x00, //      

	    // @376 'O' (5 pixels wide)
	    0x30, //   ## 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @384 'P' (5 pixels wide)
	    0xF0, // #### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x70, //  ### 
	    0x40, //  #   
	    0xE0, // ###  
	    0x00, //      
	    0x00, //      

	    // @392 'Q' (5 pixels wide)
	    0x30, //   ## 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0x30, //   ## 
	    0x18, //    ##
	    0x00, //      

	    // @400 'R' (5 pixels wide)
	    0xF0, // #### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x70, //  ### 
	    0x48, //  #  #
	    0xE8, // ### #
	    0x00, //      
	    0x00, //      

	    // @408 'S' (5 pixels wide)
	    0x70, //  ### 
	    0x50, //  # # 
	    0x20, //   #  
	    0x10, //    # 
	    0x50, //  # # 
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @416 'T' (5 pixels wide)
	    0xF8, // #####
	    0xA8, // # # #
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @424 'U' (5 pixels wide)
	    0xD8, // ## ##
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0x48, //  #  #
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @432 'V' (5 pixels wide)
	    0xD8, // ## ##
	    0x88, // #   #
	    0x48, //  #  #
	    0x50, //  # # 
	    0x50, //  # # 
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @440 'W' (5 pixels wide)
	    0xD8, // ## ##
	    0x88, // #   #
	    0xA8, // # # #
	    0xA8, // # # #
	    0xA8, // # # #
	    0x50, //  # # 
	    0x00, //      
	    0x00, //      

	    // @448 'X' (5 pixels wide)
	    0xD8, // ## ##
	    0x50, //  # # 
	    0x20, //   #  
	    0x20, //   #  
	    0x50, //  # # 
	    0xD8, // ## ##
	    0x00, //      
	    0x00, //      

	    // @456 'Y' (5 pixels wide)
	    0xD8, // ## ##
	    0x88, // #   #
	    0x50, //  # # 
	    0x20, //   #  
	    0x20, //   #  
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @464 'Z' (5 pixels wide)
	    0x78, //  ####
	    0x48, //  #  #
	    0x10, //    # 
	    0x20, //   #  
	    0x48, //  #  #
	    0x78, //  ####
	    0x00, //      
	    0x00, //      

	    // @472 '[' (5 pixels wide)
	    0x30, //   ## 
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x30, //   ## 
	    0x00, //      

	    // @480 '\' (5 pixels wide)
	    0x80, // #    
	    0x40, //  #   
	    0x40, //  #   
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x10, //    # 
	    0x00, //      

	    // @488 ']' (5 pixels wide)
	    0x60, //  ##  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x60, //  ##  
	    0x00, //      

	    // @496 '^' (5 pixels wide)
	    0x20, //   #  
	    0x20, //   #  
	    0x50, //  # # 
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @504 '_' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0xF8, // #####

	    // @512 '`' (5 pixels wide)
	    0x20, //   #  
	    0x10, //    # 
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x00, //      

	    // @520 'a' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x30, //   ## 
	    0x10, //    # 
	    0x70, //  ### 
	    0x78, //  ####
	    0x00, //      
	    0x00, //      

	    // @528 'b' (5 pixels wide)
	    0xC0, // ##   
	    0x40, //  #   
	    0x70, //  ### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0xF0, // #### 
	    0x00, //      
	    0x00, //      

	    // @536 'c' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x70, //  ### 
	    0x40, //  #   
	    0x40, //  #   
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @544 'd' (5 pixels wide)
	    0x18, //    ##
	    0x08, //     #
	    0x38, //   ###
	    0x48, //  #  #
	    0x48, //  #  #
	    0x38, //   ###
	    0x00, //      
	    0x00, //      

	    // @552 'e' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x70, //  ### 
	    0x70, //  ### 
	    0x40, //  #   
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @560 'f' (5 pixels wide)
	    0x10, //    # 
	    0x20, //   #  
	    0x70, //  ### 
	    0x20, //   #  
	    0x20, //   #  
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @568 'g' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x38, //   ###
	    0x48, //  #  #
	    0x48, //  #  #
	    0x38, //   ###
	    0x08, //     #
	    0x30, //   ## 

	    // @576 'h' (5 pixels wide)
	    0xC0, // ##   
	    0x40, //  #   
	    0x70, //  ### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0xE8, // ### #
	    0x00, //      
	    0x00, //      

	    // @584 'i' (5 pixels wide)
	    0x20, //   #  
	    0x00, //      
	    0x60, //  ##  
	    0x20, //   #  
	    0x20, //   #  
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @592 'j' (5 pixels wide)
	    0x20, //   #  
	    0x00, //      
	    0x70, //  ### 
	    0x10, //    # 
	    0x10, //    # 
	    0x10, //    # 
	    0x10, //    # 
	    0x70, //  ### 

	    // @600 'k' (5 pixels wide)
	    0xC0, // ##   
	    0x40, //  #   
	    0x58, //  # ##
	    0x70, //  ### 
	    0x50, //  # # 
	    0xD8, // ## ##
	    0x00, //      
	    0x00, //      

	    // @608 'l' (5 pixels wide)
	    0x60, //  ##  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @616 'm' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0xD0, // ## # 
	    0xA8, // # # #
	    0xA8, // # # #
	    0xA8, // # # #
	    0x00, //      
	    0x00, //      

	    // @624 'n' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0xF0, // #### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0xC8, // ##  #
	    0x00, //      
	    0x00, //      

	    // @632 'o' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x30, //   ## 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @640 'p' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0xF0, // #### 
	    0x48, //  #  #
	    0x48, //  #  #
	    0x70, //  ### 
	    0x40, //  #   
	    0xE0, // ###  

	    // @648 'q' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x38, //   ###
	    0x48, //  #  #
	    0x48, //  #  #
	    0x38, //   ###
	    0x08, //     #
	    0x18, //    ##

	    // @656 'r' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x78, //  ####
	    0x20, //   #  
	    0x20, //   #  
	    0x70, //  ### 
	    0x00, //      
	    0x00, //      

	    // @664 's' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x30, //   ## 
	    0x20, //   #  
	    0x10, //    # 
	    0x60, //  ##  
	    0x00, //      
	    0x00, //      

	    // @672 't' (5 pixels wide)
	    0x00, //      
	    0x40, //  #   
	    0xF0, // #### 
	    0x40, //  #   
	    0x48, //  #  #
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @680 'u' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0xD8, // ## ##
	    0x48, //  #  #
	    0x48, //  #  #
	    0x38, //   ###
	    0x00, //      
	    0x00, //      

	    // @688 'v' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0xC8, // ##  #
	    0x48, //  #  #
	    0x30, //   ## 
	    0x30, //   ## 
	    0x00, //      
	    0x00, //      

	    // @696 'w' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0xD8, // ## ##
	    0xA8, // # # #
	    0xA8, // # # #
	    0x50, //  # # 
	    0x00, //      
	    0x00, //      

	    // @704 'x' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x48, //  #  #
	    0x30, //   ## 
	    0x30, //   ## 
	    0x48, //  #  #
	    0x00, //      
	    0x00, //      

	    // @712 'y' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0xD8, // ## ##
	    0x50, //  # # 
	    0x50, //  # # 
	    0x20, //   #  
	    0x20, //   #  
	    0x60, //  ##  

	    // @720 'z' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x78, //  ####
	    0x50, //  # # 
	    0x28, //   # #
	    0x78, //  ####
	    0x00, //      
	    0x00, //      

	    // @728 '{' (5 pixels wide)
	    0x10, //    # 
	    0x20, //   #  
	    0x20, //   #  
	    0x60, //  ##  
	    0x20, //   #  
	    0x20, //   #  
	    0x10, //    # 
	    0x00, //      

	    // @736 '|' (5 pixels wide)
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x20, //   #  
	    0x00, //      

	    // @744 '}' (5 pixels wide)
	    0x40, //  #   
	    0x20, //   #  
	    0x20, //   #  
	    0x30, //   ## 
	    0x20, //   #  
	    0x20, //   #  
	    0x40, //  #   
	    0x00, //      

	    // @752 '~' (5 pixels wide)
	    0x00, //      
	    0x00, //      
	    0x00, //      
	    0x28, //   # #
	    0x50, //  # # 
	    0x00, //      
	    0x00, //      
	    0x00, //      
    };

    public static readonly byte Font12_TableWidth = 7;
    public static readonly byte Font12_TableHeight = 12;
    public static readonly byte[] Font12_Table = new byte[]
{
	// @0 ' ' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @12 '!' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x00, //        
	0x00, //        
	0x10, //    #   
	0x00, //        
	0x00, //        
	0x00, //        

	// @24 '"' (7 pixels wide)
	0x00, //        
	0x6C, //  ## ## 
	0x48, //  #  #  
	0x48, //  #  #  
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @36 '#' (7 pixels wide)
	0x00, //        
	0x14, //    # # 
	0x14, //    # # 
	0x28, //   # #  
	0x7C, //  ##### 
	0x28, //   # #  
	0x7C, //  ##### 
	0x28, //   # #  
	0x50, //  # #   
	0x50, //  # #   
	0x00, //        
	0x00, //        

	// @48 '$' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x38, //   ###  
	0x40, //  #     
	0x40, //  #     
	0x38, //   ###  
	0x48, //  #  #  
	0x70, //  ###   
	0x10, //    #   
	0x10, //    #   
	0x00, //        
	0x00, //        

	// @60 '%' (7 pixels wide)
	0x00, //        
	0x20, //   #    
	0x50, //  # #   
	0x20, //   #    
	0x0C, //     ## 
	0x70, //  ###   
	0x08, //     #  
	0x14, //    # # 
	0x08, //     #  
	0x00, //        
	0x00, //        
	0x00, //        

	// @72 '&' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x18, //    ##  
	0x20, //   #    
	0x20, //   #    
	0x54, //  # # # 
	0x48, //  #  #  
	0x34, //   ## # 
	0x00, //        
	0x00, //        
	0x00, //        

	// @84 ''' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @96 '(' (7 pixels wide)
	0x00, //        
	0x08, //     #  
	0x08, //     #  
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x08, //     #  
	0x08, //     #  
	0x00, //        

	// @108 ')' (7 pixels wide)
	0x00, //        
	0x20, //   #    
	0x20, //   #    
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x20, //   #    
	0x20, //   #    
	0x00, //        

	// @120 '*' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x7C, //  ##### 
	0x10, //    #   
	0x28, //   # #  
	0x28, //   # #  
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @132 '+' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0xFE, // #######
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x00, //        
	0x00, //        
	0x00, //        

	// @144 ',' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x18, //    ##  
	0x10, //    #   
	0x30, //   ##   
	0x20, //   #    
	0x00, //        

	// @156 '-' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @168 '.' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x30, //   ##   
	0x30, //   ##   
	0x00, //        
	0x00, //        
	0x00, //        

	// @180 '/' (7 pixels wide)
	0x00, //        
	0x04, //      # 
	0x04, //      # 
	0x08, //     #  
	0x08, //     #  
	0x10, //    #   
	0x10, //    #   
	0x20, //   #    
	0x20, //   #    
	0x40, //  #     
	0x00, //        
	0x00, //        

	// @192 '0' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @204 '1' (7 pixels wide)
	0x00, //        
	0x30, //   ##   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @216 '2' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x04, //      # 
	0x08, //     #  
	0x10, //    #   
	0x20, //   #    
	0x44, //  #   # 
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @228 '3' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x04, //      # 
	0x18, //    ##  
	0x04, //      # 
	0x04, //      # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @240 '4' (7 pixels wide)
	0x00, //        
	0x0C, //     ## 
	0x14, //    # # 
	0x14, //    # # 
	0x24, //   #  # 
	0x44, //  #   # 
	0x7E, //  ######
	0x04, //      # 
	0x0E, //     ###
	0x00, //        
	0x00, //        
	0x00, //        

	// @252 '5' (7 pixels wide)
	0x00, //        
	0x3C, //   #### 
	0x20, //   #    
	0x20, //   #    
	0x38, //   ###  
	0x04, //      # 
	0x04, //      # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @264 '6' (7 pixels wide)
	0x00, //        
	0x1C, //    ### 
	0x20, //   #    
	0x40, //  #     
	0x78, //  ####  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @276 '7' (7 pixels wide)
	0x00, //        
	0x7C, //  ##### 
	0x44, //  #   # 
	0x04, //      # 
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x10, //    #   
	0x10, //    #   
	0x00, //        
	0x00, //        
	0x00, //        

	// @288 '8' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @300 '9' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x3C, //   #### 
	0x04, //      # 
	0x08, //     #  
	0x70, //  ###   
	0x00, //        
	0x00, //        
	0x00, //        

	// @312 ':' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x30, //   ##   
	0x30, //   ##   
	0x00, //        
	0x00, //        
	0x30, //   ##   
	0x30, //   ##   
	0x00, //        
	0x00, //        
	0x00, //        

	// @324 ';' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x18, //    ##  
	0x18, //    ##  
	0x00, //        
	0x00, //        
	0x18, //    ##  
	0x30, //   ##   
	0x20, //   #    
	0x00, //        
	0x00, //        

	// @336 '<' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x0C, //     ## 
	0x10, //    #   
	0x60, //  ##    
	0x80, // #      
	0x60, //  ##    
	0x10, //    #   
	0x0C, //     ## 
	0x00, //        
	0x00, //        
	0x00, //        

	// @348 '=' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x7C, //  ##### 
	0x00, //        
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @360 '>' (7 pixels wide)
	0x00, //        
	0x00, //        
	0xC0, // ##     
	0x20, //   #    
	0x18, //    ##  
	0x04, //      # 
	0x18, //    ##  
	0x20, //   #    
	0xC0, // ##     
	0x00, //        
	0x00, //        
	0x00, //        

	// @372 '?' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x18, //    ##  
	0x24, //   #  # 
	0x04, //      # 
	0x08, //     #  
	0x10, //    #   
	0x00, //        
	0x30, //   ##   
	0x00, //        
	0x00, //        
	0x00, //        

	// @384 '@' (7 pixels wide)
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x4C, //  #  ## 
	0x54, //  # # # 
	0x54, //  # # # 
	0x4C, //  #  ## 
	0x40, //  #     
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        

	// @396 'A' (7 pixels wide)
	0x00, //        
	0x30, //   ##   
	0x10, //    #   
	0x28, //   # #  
	0x28, //   # #  
	0x28, //   # #  
	0x7C, //  ##### 
	0x44, //  #   # 
	0xEE, // ### ###
	0x00, //        
	0x00, //        
	0x00, //        

	// @408 'B' (7 pixels wide)
	0x00, //        
	0xF8, // #####  
	0x44, //  #   # 
	0x44, //  #   # 
	0x78, //  ####  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0xF8, // #####  
	0x00, //        
	0x00, //        
	0x00, //        

	// @420 'C' (7 pixels wide)
	0x00, //        
	0x3C, //   #### 
	0x44, //  #   # 
	0x40, //  #     
	0x40, //  #     
	0x40, //  #     
	0x40, //  #     
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @432 'D' (7 pixels wide)
	0x00, //        
	0xF0, // ####   
	0x48, //  #  #  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x48, //  #  #  
	0xF0, // ####   
	0x00, //        
	0x00, //        
	0x00, //        

	// @444 'E' (7 pixels wide)
	0x00, //        
	0xFC, // ###### 
	0x44, //  #   # 
	0x50, //  # #   
	0x70, //  ###   
	0x50, //  # #   
	0x40, //  #     
	0x44, //  #   # 
	0xFC, // ###### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @456 'F' (7 pixels wide)
	0x00, //        
	0x7E, //  ######
	0x22, //   #   #
	0x28, //   # #  
	0x38, //   ###  
	0x28, //   # #  
	0x20, //   #    
	0x20, //   #    
	0x70, //  ###   
	0x00, //        
	0x00, //        
	0x00, //        

	// @468 'G' (7 pixels wide)
	0x00, //        
	0x3C, //   #### 
	0x44, //  #   # 
	0x40, //  #     
	0x40, //  #     
	0x4E, //  #  ###
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @480 'H' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x44, //  #   # 
	0x7C, //  ##### 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0xEE, // ### ###
	0x00, //        
	0x00, //        
	0x00, //        

	// @492 'I' (7 pixels wide)
	0x00, //        
	0x7C, //  ##### 
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @504 'J' (7 pixels wide)
	0x00, //        
	0x3C, //   #### 
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x48, //  #  #  
	0x48, //  #  #  
	0x48, //  #  #  
	0x30, //   ##   
	0x00, //        
	0x00, //        
	0x00, //        

	// @516 'K' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x48, //  #  #  
	0x50, //  # #   
	0x70, //  ###   
	0x48, //  #  #  
	0x44, //  #   # 
	0xE6, // ###  ##
	0x00, //        
	0x00, //        
	0x00, //        

	// @528 'L' (7 pixels wide)
	0x00, //        
	0x70, //  ###   
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x24, //   #  # 
	0x24, //   #  # 
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @540 'M' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x6C, //  ## ## 
	0x6C, //  ## ## 
	0x54, //  # # # 
	0x54, //  # # # 
	0x44, //  #   # 
	0x44, //  #   # 
	0xEE, // ### ###
	0x00, //        
	0x00, //        
	0x00, //        

	// @552 'N' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x64, //  ##  # 
	0x64, //  ##  # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x4C, //  #  ## 
	0xEC, // ### ## 
	0x00, //        
	0x00, //        
	0x00, //        

	// @564 'O' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @576 'P' (7 pixels wide)
	0x00, //        
	0x78, //  ####  
	0x24, //   #  # 
	0x24, //   #  # 
	0x24, //   #  # 
	0x38, //   ###  
	0x20, //   #    
	0x20, //   #    
	0x70, //  ###   
	0x00, //        
	0x00, //        
	0x00, //        

	// @588 'Q' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x1C, //    ### 
	0x00, //        
	0x00, //        

	// @600 'R' (7 pixels wide)
	0x00, //        
	0xF8, // #####  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x78, //  ####  
	0x48, //  #  #  
	0x44, //  #   # 
	0xE2, // ###   #
	0x00, //        
	0x00, //        
	0x00, //        

	// @612 'S' (7 pixels wide)
	0x00, //        
	0x34, //   ## # 
	0x4C, //  #  ## 
	0x40, //  #     
	0x38, //   ###  
	0x04, //      # 
	0x04, //      # 
	0x64, //  ##  # 
	0x58, //  # ##  
	0x00, //        
	0x00, //        
	0x00, //        

	// @624 'T' (7 pixels wide)
	0x00, //        
	0xFE, // #######
	0x92, // #  #  #
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @636 'U' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @648 'V' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x44, //  #   # 
	0x28, //   # #  
	0x28, //   # #  
	0x28, //   # #  
	0x10, //    #   
	0x10, //    #   
	0x00, //        
	0x00, //        
	0x00, //        

	// @660 'W' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x44, //  #   # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x28, //   # #  
	0x00, //        
	0x00, //        
	0x00, //        

	// @672 'X' (7 pixels wide)
	0x00, //        
	0xC6, // ##   ##
	0x44, //  #   # 
	0x28, //   # #  
	0x10, //    #   
	0x10, //    #   
	0x28, //   # #  
	0x44, //  #   # 
	0xC6, // ##   ##
	0x00, //        
	0x00, //        
	0x00, //        

	// @684 'Y' (7 pixels wide)
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x28, //   # #  
	0x28, //   # #  
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @696 'Z' (7 pixels wide)
	0x00, //        
	0x7C, //  ##### 
	0x44, //  #   # 
	0x08, //     #  
	0x10, //    #   
	0x10, //    #   
	0x20, //   #    
	0x44, //  #   # 
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @708 '[' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x38, //   ###  
	0x00, //        

	// @720 '\' (7 pixels wide)
	0x00, //        
	0x40, //  #     
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x10, //    #   
	0x10, //    #   
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x00, //        
	0x00, //        

	// @732 ']' (7 pixels wide)
	0x00, //        
	0x38, //   ###  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x38, //   ###  
	0x00, //        

	// @744 '^' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x10, //    #   
	0x28, //   # #  
	0x44, //  #   # 
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @756 '_' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0xFE, // #######

	// @768 '`' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x08, //     #  
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        

	// @780 'a' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x3C, //   #### 
	0x44, //  #   # 
	0x44, //  #   # 
	0x3E, //   #####
	0x00, //        
	0x00, //        
	0x00, //        

	// @792 'b' (7 pixels wide)
	0x00, //        
	0xC0, // ##     
	0x40, //  #     
	0x58, //  # ##  
	0x64, //  ##  # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0xF8, // #####  
	0x00, //        
	0x00, //        
	0x00, //        

	// @804 'c' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x3C, //   #### 
	0x44, //  #   # 
	0x40, //  #     
	0x40, //  #     
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @816 'd' (7 pixels wide)
	0x00, //        
	0x0C, //     ## 
	0x04, //      # 
	0x34, //   ## # 
	0x4C, //  #  ## 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x3E, //   #####
	0x00, //        
	0x00, //        
	0x00, //        

	// @828 'e' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x7C, //  ##### 
	0x40, //  #     
	0x40, //  #     
	0x3C, //   #### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @840 'f' (7 pixels wide)
	0x00, //        
	0x1C, //    ### 
	0x20, //   #    
	0x7C, //  ##### 
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @852 'g' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x36, //   ## ##
	0x4C, //  #  ## 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x3C, //   #### 
	0x04, //      # 
	0x38, //   ###  
	0x00, //        

	// @864 'h' (7 pixels wide)
	0x00, //        
	0xC0, // ##     
	0x40, //  #     
	0x58, //  # ##  
	0x64, //  ##  # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0xEE, // ### ###
	0x00, //        
	0x00, //        
	0x00, //        

	// @876 'i' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x00, //        
	0x70, //  ###   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @888 'j' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x00, //        
	0x78, //  ####  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x08, //     #  
	0x70, //  ###   
	0x00, //        

	// @900 'k' (7 pixels wide)
	0x00, //        
	0xC0, // ##     
	0x40, //  #     
	0x5C, //  # ### 
	0x48, //  #  #  
	0x70, //  ###   
	0x50, //  # #   
	0x48, //  #  #  
	0xDC, // ## ### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @912 'l' (7 pixels wide)
	0x00, //        
	0x30, //   ##   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @924 'm' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xE8, // ### #  
	0x54, //  # # # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x54, //  # # # 
	0xFE, // #######
	0x00, //        
	0x00, //        
	0x00, //        

	// @936 'n' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xD8, // ## ##  
	0x64, //  ##  # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0xEE, // ### ###
	0x00, //        
	0x00, //        
	0x00, //        

	// @948 'o' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x38, //   ###  
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x38, //   ###  
	0x00, //        
	0x00, //        
	0x00, //        

	// @960 'p' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xD8, // ## ##  
	0x64, //  ##  # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x78, //  ####  
	0x40, //  #     
	0xE0, // ###    
	0x00, //        

	// @972 'q' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x36, //   ## ##
	0x4C, //  #  ## 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x3C, //   #### 
	0x04, //      # 
	0x0E, //     ###
	0x00, //        

	// @984 'r' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x6C, //  ## ## 
	0x30, //   ##   
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @996 's' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x3C, //   #### 
	0x44, //  #   # 
	0x38, //   ###  
	0x04, //      # 
	0x44, //  #   # 
	0x78, //  ####  
	0x00, //        
	0x00, //        
	0x00, //        

	// @1008 't' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x20, //   #    
	0x7C, //  ##### 
	0x20, //   #    
	0x20, //   #    
	0x20, //   #    
	0x22, //   #   #
	0x1C, //    ### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @1020 'u' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xCC, // ##  ## 
	0x44, //  #   # 
	0x44, //  #   # 
	0x44, //  #   # 
	0x4C, //  #  ## 
	0x36, //   ## ##
	0x00, //        
	0x00, //        
	0x00, //        

	// @1032 'v' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x44, //  #   # 
	0x28, //   # #  
	0x28, //   # #  
	0x10, //    #   
	0x00, //        
	0x00, //        
	0x00, //        

	// @1044 'w' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x54, //  # # # 
	0x28, //   # #  
	0x00, //        
	0x00, //        
	0x00, //        

	// @1056 'x' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xCC, // ##  ## 
	0x48, //  #  #  
	0x30, //   ##   
	0x30, //   ##   
	0x48, //  #  #  
	0xCC, // ##  ## 
	0x00, //        
	0x00, //        
	0x00, //        

	// @1068 'y' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0xEE, // ### ###
	0x44, //  #   # 
	0x24, //   #  # 
	0x28, //   # #  
	0x18, //    ##  
	0x10, //    #   
	0x10, //    #   
	0x78, //  ####  
	0x00, //        

	// @1080 'z' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x7C, //  ##### 
	0x48, //  #  #  
	0x10, //    #   
	0x20, //   #    
	0x44, //  #   # 
	0x7C, //  ##### 
	0x00, //        
	0x00, //        
	0x00, //        

	// @1092 '{' (7 pixels wide)
	0x00, //        
	0x08, //     #  
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x20, //   #    
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x08, //     #  
	0x00, //        

	// @1104 '|' (7 pixels wide)
	0x00, //        
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x00, //        
	0x00, //        

	// @1116 '}' (7 pixels wide)
	0x00, //        
	0x20, //   #    
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x08, //     #  
	0x10, //    #   
	0x10, //    #   
	0x10, //    #   
	0x20, //   #    
	0x00, //        

	// @1128 '~' (7 pixels wide)
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x24, //   #  # 
	0x58, //  # ##  
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
	0x00, //        
};

    public static readonly byte Font16_TableWidth = 11;
    public static readonly byte Font16_TableHeight = 16;
    public static readonly byte[] Font16_Table = new byte[]
    {
	    // @0 ' ' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @32 '!' (11 pixels wide)
	    0x00, 0x00, //            
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @64 '"' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1D, 0xC0, //    ### ### 
	    0x1D, 0xC0, //    ### ### 
	    0x08, 0x80, //     #   #  
	    0x08, 0x80, //     #   #  
	    0x08, 0x80, //     #   #  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @96 '#' (11 pixels wide)
	    0x00, 0x00, //            
	    0x0D, 0x80, //     ## ##  
	    0x0D, 0x80, //     ## ##  
	    0x0D, 0x80, //     ## ##  
	    0x0D, 0x80, //     ## ##  
	    0x3F, 0xC0, //   ######## 
	    0x1B, 0x00, //    ## ##   
	    0x3F, 0xC0, //   ######## 
	    0x1B, 0x00, //    ## ##   
	    0x1B, 0x00, //    ## ##   
	    0x1B, 0x00, //    ## ##   
	    0x1B, 0x00, //    ## ##   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @128 '$' (11 pixels wide)
	    0x04, 0x00, //      #     
	    0x1F, 0x80, //    ######  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x38, 0x00, //   ###      
	    0x1E, 0x00, //    ####    
	    0x0F, 0x00, //     ####   
	    0x03, 0x80, //       ###  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x3F, 0x00, //   ######   
	    0x04, 0x00, //      #     
	    0x04, 0x00, //      #     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @160 '%' (11 pixels wide)
	    0x00, 0x00, //            
	    0x18, 0x00, //    ##      
	    0x24, 0x00, //   #  #     
	    0x24, 0x00, //   #  #     
	    0x18, 0xC0, //    ##   ## 
	    0x07, 0x80, //      ####  
	    0x1E, 0x00, //    ####    
	    0x31, 0x80, //   ##   ##  
	    0x02, 0x40, //       #  # 
	    0x02, 0x40, //       #  # 
	    0x01, 0x80, //        ##  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @192 '&' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x0F, 0x00, //     ####   
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x0C, 0x00, //     ##     
	    0x1D, 0x80, //    ### ##  
	    0x37, 0x00, //   ## ###   
	    0x33, 0x00, //   ##  ##   
	    0x1D, 0x80, //    ### ##  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @224 ''' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x07, 0x00, //      ###   
	    0x07, 0x00, //      ###   
	    0x02, 0x00, //       #    
	    0x02, 0x00, //       #    
	    0x02, 0x00, //       #    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @256 '(' (11 pixels wide)
	    0x00, 0x00, //            
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x06, 0x00, //      ##    
	    0x0E, 0x00, //     ###    
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0E, 0x00, //     ###    
	    0x06, 0x00, //      ##    
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @288 ')' (11 pixels wide)
	    0x00, 0x00, //            
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x0C, 0x00, //     ##     
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x0C, 0x00, //     ##     
	    0x1C, 0x00, //    ###     
	    0x18, 0x00, //    ##      
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @320 '*' (11 pixels wide)
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x3F, 0xC0, //   ######## 
	    0x3F, 0xC0, //   ######## 
	    0x0F, 0x00, //     ####   
	    0x1F, 0x80, //    ######  
	    0x19, 0x80, //    ##  ##  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @352 '+' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x04, 0x00, //      #     
	    0x04, 0x00, //      #     
	    0x04, 0x00, //      #     
	    0x3F, 0x80, //   #######  
	    0x04, 0x00, //      #     
	    0x04, 0x00, //      #     
	    0x04, 0x00, //      #     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @384 ',' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x04, 0x00, //      #     
	    0x0C, 0x00, //     ##     
	    0x08, 0x00, //     #      
	    0x08, 0x00, //     #      
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @416 '-' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x3F, 0x80, //   #######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @448 '.' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @480 '/' (11 pixels wide)
	    0x00, 0xC0, //         ## 
	    0x00, 0xC0, //         ## 
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x06, 0x00, //      ##    
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @512 '0' (11 pixels wide)
	    0x00, 0x00, //            
	    0x0E, 0x00, //     ###    
	    0x1B, 0x00, //    ## ##   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x1B, 0x00, //    ## ##   
	    0x0E, 0x00, //     ###    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @544 '1' (11 pixels wide)
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x3E, 0x00, //   #####    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x3F, 0xC0, //   ######## 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @576 '2' (11 pixels wide)
	    0x00, 0x00, //            
	    0x0F, 0x00, //     ####   
	    0x19, 0x80, //    ##  ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x03, 0x00, //       ##   
	    0x06, 0x00, //      ##    
	    0x0C, 0x00, //     ##     
	    0x18, 0x00, //    ##      
	    0x30, 0x00, //   ##       
	    0x3F, 0x80, //   #######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @608 '3' (11 pixels wide)
	    0x00, 0x00, //            
	    0x3F, 0x00, //   ######   
	    0x61, 0x80, //  ##    ##  
	    0x01, 0x80, //        ##  
	    0x03, 0x00, //       ##   
	    0x1F, 0x00, //    #####   
	    0x03, 0x80, //       ###  
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x61, 0x80, //  ##    ##  
	    0x3F, 0x00, //   ######   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @640 '4' (11 pixels wide)
	    0x00, 0x00, //            
	    0x07, 0x00, //      ###   
	    0x07, 0x00, //      ###   
	    0x0F, 0x00, //     ####   
	    0x0B, 0x00, //     # ##   
	    0x1B, 0x00, //    ## ##   
	    0x13, 0x00, //    #  ##   
	    0x33, 0x00, //   ##  ##   
	    0x3F, 0x80, //   #######  
	    0x03, 0x00, //       ##   
	    0x0F, 0x80, //     #####  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @672 '5' (11 pixels wide)
	    0x00, 0x00, //            
	    0x1F, 0x80, //    ######  
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x1F, 0x00, //    #####   
	    0x11, 0x80, //    #   ##  
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x21, 0x80, //   #    ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @704 '6' (11 pixels wide)
	    0x00, 0x00, //            
	    0x07, 0x80, //      ####  
	    0x1C, 0x00, //    ###     
	    0x18, 0x00, //    ##      
	    0x30, 0x00, //   ##       
	    0x37, 0x00, //   ## ###   
	    0x39, 0x80, //   ###  ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x19, 0x80, //    ##  ##  
	    0x0F, 0x00, //     ####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @736 '7' (11 pixels wide)
	    0x00, 0x00, //            
	    0x7F, 0x00, //  #######   
	    0x43, 0x00, //  #    ##   
	    0x03, 0x00, //       ##   
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @768 '8' (11 pixels wide)
	    0x00, 0x00, //            
	    0x1F, 0x00, //    #####   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @800 '9' (11 pixels wide)
	    0x00, 0x00, //            
	    0x1E, 0x00, //    ####    
	    0x33, 0x00, //   ##  ##   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x33, 0x80, //   ##  ###  
	    0x1D, 0x80, //    ### ##  
	    0x01, 0x80, //        ##  
	    0x03, 0x00, //       ##   
	    0x07, 0x00, //      ###   
	    0x3C, 0x00, //   ####     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @832 ':' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @864 ';' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x04, 0x00, //      #     
	    0x08, 0x00, //     #      
	    0x08, 0x00, //     #      
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @896 '<' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0xC0, //         ## 
	    0x03, 0x00, //       ##   
	    0x04, 0x00, //      #     
	    0x18, 0x00, //    ##      
	    0x60, 0x00, //  ##        
	    0x18, 0x00, //    ##      
	    0x04, 0x00, //      #     
	    0x03, 0x00, //       ##   
	    0x00, 0xC0, //         ## 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @928 '=' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0xC0, //  ######### 
	    0x00, 0x00, //            
	    0x7F, 0xC0, //  ######### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @960 '>' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x60, 0x00, //  ##        
	    0x18, 0x00, //    ##      
	    0x04, 0x00, //      #     
	    0x03, 0x00, //       ##   
	    0x00, 0xC0, //         ## 
	    0x03, 0x00, //       ##   
	    0x04, 0x00, //      #     
	    0x18, 0x00, //    ##      
	    0x60, 0x00, //  ##        
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @992 '?' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x00, //    #####   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x01, 0x80, //        ##  
	    0x07, 0x00, //      ###   
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1024 '@' (11 pixels wide)
	    0x00, 0x00, //            
	    0x0E, 0x00, //     ###    
	    0x11, 0x00, //    #   #   
	    0x21, 0x00, //   #    #   
	    0x21, 0x00, //   #    #   
	    0x27, 0x00, //   #  ###   
	    0x29, 0x00, //   # #  #   
	    0x29, 0x00, //   # #  #   
	    0x27, 0x00, //   #  ###   
	    0x20, 0x00, //   #        
	    0x11, 0x00, //    #   #   
	    0x0E, 0x00, //     ###    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1056 'A' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x3F, 0x00, //   ######   
	    0x0F, 0x00, //     ####   
	    0x09, 0x00, //     #  #   
	    0x19, 0x80, //    ##  ##  
	    0x19, 0x80, //    ##  ##  
	    0x1F, 0x80, //    ######  
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x79, 0xE0, //  ####  ####
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1088 'B' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0x00, //  #######   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x3F, 0x00, //   ######   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x7F, 0x00, //  #######   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1120 'C' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x40, //    ##### # 
	    0x30, 0xC0, //   ##    ## 
	    0x60, 0x40, //  ##      # 
	    0x60, 0x00, //  ##        
	    0x60, 0x00, //  ##        
	    0x60, 0x00, //  ##        
	    0x60, 0x40, //  ##      # 
	    0x30, 0x80, //   ##    #  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1152 'D' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0x00, //  #######   
	    0x31, 0x80, //   ##   ##  
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x31, 0x80, //   ##   ##  
	    0x7F, 0x00, //  #######   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1184 'E' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0x80, //  ########  
	    0x30, 0x80, //   ##    #  
	    0x30, 0x80, //   ##    #  
	    0x32, 0x00, //   ##  #    
	    0x3E, 0x00, //   #####    
	    0x32, 0x00, //   ##  #    
	    0x30, 0x80, //   ##    #  
	    0x30, 0x80, //   ##    #  
	    0x7F, 0x80, //  ########  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1216 'F' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0xC0, //  ######### 
	    0x30, 0x40, //   ##     # 
	    0x30, 0x40, //   ##     # 
	    0x32, 0x00, //   ##  #    
	    0x3E, 0x00, //   #####    
	    0x32, 0x00, //   ##  #    
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x7C, 0x00, //  #####     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1248 'G' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1E, 0x80, //    #### #  
	    0x31, 0x80, //   ##   ##  
	    0x60, 0x80, //  ##     #  
	    0x60, 0x00, //  ##        
	    0x60, 0x00, //  ##        
	    0x67, 0xC0, //  ##  ##### 
	    0x61, 0x80, //  ##    ##  
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1280 'H' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0xC0, //  #### #### 
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x3F, 0x80, //   #######  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x7B, 0xC0, //  #### #### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1312 'I' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x3F, 0xC0, //   ######## 
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x3F, 0xC0, //   ######## 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1344 'J' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0xC0, //    ####### 
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x63, 0x00, //  ##   ##   
	    0x63, 0x00, //  ##   ##   
	    0x63, 0x00, //  ##   ##   
	    0x3E, 0x00, //   #####    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1376 'K' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0xC0, //  #### #### 
	    0x31, 0x80, //   ##   ##  
	    0x33, 0x00, //   ##  ##   
	    0x36, 0x00, //   ## ##    
	    0x3C, 0x00, //   ####     
	    0x3E, 0x00, //   #####    
	    0x33, 0x00, //   ##  ##   
	    0x31, 0x80, //   ##   ##  
	    0x79, 0xC0, //  ####  ### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1408 'L' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7E, 0x00, //  ######    
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x40, //    ##    # 
	    0x18, 0x40, //    ##    # 
	    0x18, 0x40, //    ##    # 
	    0x7F, 0xC0, //  ######### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1440 'M' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0xE0, 0xE0, // ###     ###
	    0x60, 0xC0, //  ##     ## 
	    0x71, 0xC0, //  ###   ### 
	    0x7B, 0xC0, //  #### #### 
	    0x6A, 0xC0, //  ## # # ## 
	    0x6E, 0xC0, //  ## ### ## 
	    0x64, 0xC0, //  ##  #  ## 
	    0x60, 0xC0, //  ##     ## 
	    0xFB, 0xE0, // ##### #####
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1472 'N' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x73, 0xC0, //  ###  #### 
	    0x31, 0x80, //   ##   ##  
	    0x39, 0x80, //   ###  ##  
	    0x3D, 0x80, //   #### ##  
	    0x35, 0x80, //   ## # ##  
	    0x37, 0x80, //   ## ####  
	    0x33, 0x80, //   ##  ###  
	    0x31, 0x80, //   ##   ##  
	    0x79, 0x80, //  ####  ##  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1504 'O' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x00, //    #####   
	    0x31, 0x80, //   ##   ##  
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1536 'P' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0x00, //  #######   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x3F, 0x00, //   ######   
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x7E, 0x00, //  ######    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1568 'Q' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x00, //    #####   
	    0x31, 0x80, //   ##   ##  
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x0C, 0xC0, //     ##  ## 
	    0x1F, 0x80, //    ######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1600 'R' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0x00, //  #######   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x3E, 0x00, //   #####    
	    0x33, 0x00, //   ##  ##   
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x7C, 0xE0, //  #####  ###
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1632 'S' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x80, //    ######  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x38, 0x00, //   ###      
	    0x1F, 0x00, //    #####   
	    0x03, 0x80, //       ###  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x3F, 0x00, //   ######   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1664 'T' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0x80, //  ########  
	    0x4C, 0x80, //  #  ##  #  
	    0x4C, 0x80, //  #  ##  #  
	    0x4C, 0x80, //  #  ##  #  
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x3F, 0x00, //   ######   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1696 'U' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0xC0, //  #### #### 
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1728 'V' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0xC0, //  #### #### 
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x1B, 0x00, //    ## ##   
	    0x1B, 0x00, //    ## ##   
	    0x1B, 0x00, //    ## ##   
	    0x0A, 0x00, //     # #    
	    0x0E, 0x00, //     ###    
	    0x0E, 0x00, //     ###    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1760 'W' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0xFB, 0xE0, // ##### #####
	    0x60, 0xC0, //  ##     ## 
	    0x64, 0xC0, //  ##  #  ## 
	    0x6E, 0xC0, //  ## ### ## 
	    0x6E, 0xC0, //  ## ### ## 
	    0x2A, 0x80, //   # # # #  
	    0x3B, 0x80, //   ### ###  
	    0x3B, 0x80, //   ### ###  
	    0x31, 0x80, //   ##   ##  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1792 'X' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0xC0, //  #### #### 
	    0x31, 0x80, //   ##   ##  
	    0x1B, 0x00, //    ## ##   
	    0x0E, 0x00, //     ###    
	    0x0E, 0x00, //     ###    
	    0x0E, 0x00, //     ###    
	    0x1B, 0x00, //    ## ##   
	    0x31, 0x80, //   ##   ##  
	    0x7B, 0xC0, //  #### #### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1824 'Y' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x79, 0xE0, //  ####  ####
	    0x30, 0xC0, //   ##    ## 
	    0x19, 0x80, //    ##  ##  
	    0x0F, 0x00, //     ####   
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x1F, 0x80, //    ######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1856 'Z' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x3F, 0x80, //   #######  
	    0x21, 0x80, //   #    ##  
	    0x23, 0x00, //   #   ##   
	    0x06, 0x00, //      ##    
	    0x04, 0x00, //      #     
	    0x0C, 0x00, //     ##     
	    0x18, 0x80, //    ##   #  
	    0x30, 0x80, //   ##    #  
	    0x3F, 0x80, //   #######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1888 '[' (11 pixels wide)
	    0x00, 0x00, //            
	    0x07, 0x80, //      ####  
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x07, 0x80, //      ####  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1920 '\' (11 pixels wide)
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x06, 0x00, //      ##    
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x00, 0xC0, //         ## 
	    0x00, 0xC0, //         ## 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1952 ']' (11 pixels wide)
	    0x00, 0x00, //            
	    0x1E, 0x00, //    ####    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x1E, 0x00, //    ####    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @1984 '^' (11 pixels wide)
	    0x04, 0x00, //      #     
	    0x0A, 0x00, //     # #    
	    0x0A, 0x00, //     # #    
	    0x11, 0x00, //    #   #   
	    0x20, 0x80, //   #     #  
	    0x20, 0x80, //   #     #  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2016 '_' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0xFF, 0xE0, // ###########

	    // @2048 '`' (11 pixels wide)
	    0x08, 0x00, //     #      
	    0x04, 0x00, //      #     
	    0x02, 0x00, //       #    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2080 'a' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x00, //    #####   
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x1F, 0x80, //    ######  
	    0x31, 0x80, //   ##   ##  
	    0x33, 0x80, //   ##  ###  
	    0x1D, 0xC0, //    ### ### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2112 'b' (11 pixels wide)
	    0x00, 0x00, //            
	    0x70, 0x00, //  ###       
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x37, 0x00, //   ## ###   
	    0x39, 0x80, //   ###  ##  
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x39, 0x80, //   ###  ##  
	    0x77, 0x00, //  ### ###   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2144 'c' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1E, 0x80, //    #### #  
	    0x31, 0x80, //   ##   ##  
	    0x60, 0x80, //  ##     #  
	    0x60, 0x00, //  ##        
	    0x60, 0x80, //  ##     #  
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2176 'd' (11 pixels wide)
	    0x00, 0x00, //            
	    0x03, 0x80, //       ###  
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x1D, 0x80, //    ### ##  
	    0x33, 0x80, //   ##  ###  
	    0x61, 0x80, //  ##    ##  
	    0x61, 0x80, //  ##    ##  
	    0x61, 0x80, //  ##    ##  
	    0x33, 0x80, //   ##  ###  
	    0x1D, 0xC0, //    ### ### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2208 'e' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x00, //    #####   
	    0x31, 0x80, //   ##   ##  
	    0x60, 0xC0, //  ##     ## 
	    0x7F, 0xC0, //  ######### 
	    0x60, 0x00, //  ##        
	    0x30, 0xC0, //   ##    ## 
	    0x1F, 0x80, //    ######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2240 'f' (11 pixels wide)
	    0x00, 0x00, //            
	    0x07, 0xE0, //      ######
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x3F, 0x80, //   #######  
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x3F, 0x80, //   #######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2272 'g' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1D, 0xC0, //    ### ### 
	    0x33, 0x80, //   ##  ###  
	    0x61, 0x80, //  ##    ##  
	    0x61, 0x80, //  ##    ##  
	    0x61, 0x80, //  ##    ##  
	    0x33, 0x80, //   ##  ###  
	    0x1D, 0x80, //    ### ##  
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2304 'h' (11 pixels wide)
	    0x00, 0x00, //            
	    0x70, 0x00, //  ###       
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x37, 0x00, //   ## ###   
	    0x39, 0x80, //   ###  ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x7B, 0xC0, //  #### #### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2336 'i' (11 pixels wide)
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x00, 0x00, //            
	    0x1E, 0x00, //    ####    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x3F, 0xC0, //   ######## 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2368 'j' (11 pixels wide)
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x00, 0x00, //            
	    0x3F, 0x00, //   ######   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x03, 0x00, //       ##   
	    0x3E, 0x00, //   #####    
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2400 'k' (11 pixels wide)
	    0x00, 0x00, //            
	    0x70, 0x00, //  ###       
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x37, 0x80, //   ## ####  
	    0x36, 0x00, //   ## ##    
	    0x3C, 0x00, //   ####     
	    0x3C, 0x00, //   ####     
	    0x36, 0x00, //   ## ##    
	    0x33, 0x00, //   ##  ##   
	    0x77, 0xC0, //  ### ##### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2432 'l' (11 pixels wide)
	    0x00, 0x00, //            
	    0x1E, 0x00, //    ####    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x3F, 0xC0, //   ######## 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2464 'm' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7F, 0x80, //  ########  
	    0x36, 0xC0, //   ## ## ## 
	    0x36, 0xC0, //   ## ## ## 
	    0x36, 0xC0, //   ## ## ## 
	    0x36, 0xC0, //   ## ## ## 
	    0x36, 0xC0, //   ## ## ## 
	    0x76, 0xE0, //  ### ## ###
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2496 'n' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x77, 0x00, //  ### ###   
	    0x39, 0x80, //   ###  ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x7B, 0xC0, //  #### #### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2528 'o' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x00, //    #####   
	    0x31, 0x80, //   ##   ##  
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x60, 0xC0, //  ##     ## 
	    0x31, 0x80, //   ##   ##  
	    0x1F, 0x00, //    #####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2560 'p' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x77, 0x00, //  ### ###   
	    0x39, 0x80, //   ###  ##  
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x30, 0xC0, //   ##    ## 
	    0x39, 0x80, //   ###  ##  
	    0x37, 0x00, //   ## ###   
	    0x30, 0x00, //   ##       
	    0x30, 0x00, //   ##       
	    0x7C, 0x00, //  #####     
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2592 'q' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1D, 0xC0, //    ### ### 
	    0x33, 0x80, //   ##  ###  
	    0x61, 0x80, //  ##    ##  
	    0x61, 0x80, //  ##    ##  
	    0x61, 0x80, //  ##    ##  
	    0x33, 0x80, //   ##  ###  
	    0x1D, 0x80, //    ### ##  
	    0x01, 0x80, //        ##  
	    0x01, 0x80, //        ##  
	    0x07, 0xC0, //      ##### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2624 'r' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0x80, //  #### ###  
	    0x1C, 0xC0, //    ###  ## 
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x7F, 0x00, //  #######   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2656 's' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x1F, 0x80, //    ######  
	    0x31, 0x80, //   ##   ##  
	    0x3C, 0x00, //   ####     
	    0x1F, 0x00, //    #####   
	    0x03, 0x80, //       ###  
	    0x31, 0x80, //   ##   ##  
	    0x3F, 0x00, //   ######   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2688 't' (11 pixels wide)
	    0x00, 0x00, //            
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x7F, 0x00, //  #######   
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x00, //    ##      
	    0x18, 0x80, //    ##   #  
	    0x0F, 0x00, //     ####   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2720 'u' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x73, 0x80, //  ###  ###  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x33, 0x80, //   ##  ###  
	    0x1D, 0xC0, //    ### ### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2752 'v' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0xC0, //  #### #### 
	    0x31, 0x80, //   ##   ##  
	    0x31, 0x80, //   ##   ##  
	    0x1B, 0x00, //    ## ##   
	    0x1B, 0x00, //    ## ##   
	    0x0E, 0x00, //     ###    
	    0x0E, 0x00, //     ###    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2784 'w' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0xF1, 0xE0, // ####   ####
	    0x60, 0xC0, //  ##     ## 
	    0x64, 0xC0, //  ##  #  ## 
	    0x6E, 0xC0, //  ## ### ## 
	    0x3B, 0x80, //   ### ###  
	    0x3B, 0x80, //   ### ###  
	    0x31, 0x80, //   ##   ##  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2816 'x' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x7B, 0xC0, //  #### #### 
	    0x1B, 0x00, //    ## ##   
	    0x0E, 0x00, //     ###    
	    0x0E, 0x00, //     ###    
	    0x0E, 0x00, //     ###    
	    0x1B, 0x00, //    ## ##   
	    0x7B, 0xC0, //  #### #### 
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2848 'y' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x79, 0xE0, //  ####  ####
	    0x30, 0xC0, //   ##    ## 
	    0x19, 0x80, //    ##  ##  
	    0x19, 0x80, //    ##  ##  
	    0x0B, 0x00, //     # ##   
	    0x0F, 0x00, //     ####   
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x0C, 0x00, //     ##     
	    0x3E, 0x00, //   #####    
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2880 'z' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x3F, 0x80, //   #######  
	    0x21, 0x80, //   #    ##  
	    0x03, 0x00, //       ##   
	    0x0E, 0x00, //     ###    
	    0x18, 0x00, //    ##      
	    0x30, 0x80, //   ##    #  
	    0x3F, 0x80, //   #######  
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2912 '{' (11 pixels wide)
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x18, 0x00, //    ##      
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x0C, 0x00, //     ##     
	    0x06, 0x00, //      ##    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2944 '|' (11 pixels wide)
	    0x00, 0x00, //            
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @2976 '}' (11 pixels wide)
	    0x00, 0x00, //            
	    0x0C, 0x00, //     ##     
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x03, 0x00, //       ##   
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x06, 0x00, //      ##    
	    0x0C, 0x00, //     ##     
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            

	    // @3008 '~' (11 pixels wide)
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x18, 0x00, //    ##      
	    0x24, 0x80, //   #  #  #  
	    0x03, 0x00, //       ##   
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
	    0x00, 0x00, //            
    };

    public static readonly byte Font20_TableWidth = 14;
    public static readonly byte Font20_TableHeight = 20;
    public static readonly byte[] Font20_Table = new byte[]
    {
	    // @0 ' ' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @40 '!' (14 pixels wide)
	    0x00, 0x00, //               
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x02, 0x00, //       #       
	    0x02, 0x00, //       #       
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @80 '"' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x1C, 0xE0, //    ###  ###   
	    0x1C, 0xE0, //    ###  ###   
	    0x1C, 0xE0, //    ###  ###   
	    0x08, 0x40, //     #    #    
	    0x08, 0x40, //     #    #    
	    0x08, 0x40, //     #    #    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @120 '#' (14 pixels wide)
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @160 '$' (14 pixels wide)
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x07, 0xE0, //      ######   
	    0x0F, 0xE0, //     #######   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x00, //    ##         
	    0x1F, 0x00, //    #####      
	    0x0F, 0xC0, //     ######    
	    0x00, 0xE0, //         ###   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x1F, 0xC0, //    #######    
	    0x1F, 0x80, //    ######     
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @200 '%' (14 pixels wide)
	    0x00, 0x00, //               
	    0x1C, 0x00, //    ###        
	    0x22, 0x00, //   #   #       
	    0x22, 0x00, //   #   #       
	    0x22, 0x00, //   #   #       
	    0x1C, 0x60, //    ###   ##   
	    0x01, 0xE0, //        ####   
	    0x0F, 0x80, //     #####     
	    0x3C, 0x00, //   ####        
	    0x31, 0xC0, //   ##   ###    
	    0x02, 0x20, //       #   #   
	    0x02, 0x20, //       #   #   
	    0x02, 0x20, //       #   #   
	    0x01, 0xC0, //        ###    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @240 '&' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0xE0, //       #####   
	    0x0F, 0xE0, //     #######   
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x06, 0x00, //      ##       
	    0x0F, 0x30, //     ####  ##  
	    0x1F, 0xF0, //    #########  
	    0x19, 0xE0, //    ##  ####   
	    0x18, 0xC0, //    ##   ##    
	    0x1F, 0xF0, //    #########  
	    0x07, 0xB0, //      #### ##  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @280 ''' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x01, 0x00, //        #      
	    0x01, 0x00, //        #      
	    0x01, 0x00, //        #      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @320 '(' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @360 ')' (14 pixels wide)
	    0x00, 0x00, //               
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @400 '*' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x1B, 0x60, //    ## ## ##   
	    0x1F, 0xE0, //    ########   
	    0x07, 0x80, //      ####     
	    0x07, 0x80, //      ####     
	    0x0F, 0xC0, //     ######    
	    0x0C, 0xC0, //     ##  ##    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @440 '+' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @480 ',' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0x80, //       ###     
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x04, 0x00, //      #        
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @520 '-' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0xE0, //   #########   
	    0x3F, 0xE0, //   #########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @560 '.' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @600 '/' (14 pixels wide)
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x18, 0x00, //    ##         
	    0x18, 0x00, //    ##         
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @640 '0' (14 pixels wide)
	    0x00, 0x00, //               
	    0x0F, 0x80, //     #####     
	    0x1F, 0xC0, //    #######    
	    0x18, 0xC0, //    ##   ##    
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x18, 0xC0, //    ##   ##    
	    0x1F, 0xC0, //    #######    
	    0x0F, 0x80, //     #####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @680 '1' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0x00, //       ##      
	    0x1F, 0x00, //    #####      
	    0x1F, 0x00, //    #####      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @720 '2' (14 pixels wide)
	    0x00, 0x00, //               
	    0x0F, 0x80, //     #####     
	    0x1F, 0xC0, //    #######    
	    0x38, 0xE0, //   ###   ###   
	    0x30, 0x60, //   ##     ##   
	    0x00, 0x60, //          ##   
	    0x00, 0xC0, //         ##    
	    0x01, 0x80, //        ##     
	    0x03, 0x00, //       ##      
	    0x06, 0x00, //      ##       
	    0x0C, 0x00, //     ##        
	    0x18, 0x00, //    ##         
	    0x3F, 0xE0, //   #########   
	    0x3F, 0xE0, //   #########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @760 '3' (14 pixels wide)
	    0x00, 0x00, //               
	    0x0F, 0x80, //     #####     
	    0x3F, 0xC0, //   ########    
	    0x30, 0xE0, //   ##    ###   
	    0x00, 0x60, //          ##   
	    0x00, 0xE0, //         ###   
	    0x07, 0xC0, //      #####    
	    0x07, 0xC0, //      #####    
	    0x00, 0xE0, //         ###   
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x60, 0xE0, //  ##     ###   
	    0x7F, 0xC0, //  #########    
	    0x3F, 0x80, //   #######     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @800 '4' (14 pixels wide)
	    0x00, 0x00, //               
	    0x01, 0xC0, //        ###    
	    0x03, 0xC0, //       ####    
	    0x03, 0xC0, //       ####    
	    0x06, 0xC0, //      ## ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x18, 0xC0, //    ##   ##    
	    0x30, 0xC0, //   ##    ##    
	    0x3F, 0xE0, //   #########   
	    0x3F, 0xE0, //   #########   
	    0x00, 0xC0, //         ##    
	    0x03, 0xE0, //       #####   
	    0x03, 0xE0, //       #####   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @840 '5' (14 pixels wide)
	    0x00, 0x00, //               
	    0x1F, 0xC0, //    #######    
	    0x1F, 0xC0, //    #######    
	    0x18, 0x00, //    ##         
	    0x18, 0x00, //    ##         
	    0x1F, 0x80, //    ######     
	    0x1F, 0xC0, //    #######    
	    0x18, 0xE0, //    ##   ###   
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x30, 0xE0, //   ##    ###   
	    0x3F, 0xC0, //   ########    
	    0x1F, 0x80, //    ######     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @880 '6' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0xE0, //       #####   
	    0x0F, 0xE0, //     #######   
	    0x1E, 0x00, //    ####       
	    0x18, 0x00, //    ##         
	    0x38, 0x00, //   ###         
	    0x37, 0x80, //   ## ####     
	    0x3F, 0xC0, //   ########    
	    0x38, 0xE0, //   ###   ###   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x18, 0xE0, //    ##   ###   
	    0x1F, 0xC0, //    #######    
	    0x07, 0x80, //      ####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @920 '7' (14 pixels wide)
	    0x00, 0x00, //               
	    0x3F, 0xE0, //   #########   
	    0x3F, 0xE0, //   #########   
	    0x30, 0x60, //   ##     ##   
	    0x00, 0x60, //          ##   
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @960 '8' (14 pixels wide)
	    0x00, 0x00, //               
	    0x0F, 0x80, //     #####     
	    0x1F, 0xC0, //    #######    
	    0x38, 0xE0, //   ###   ###   
	    0x30, 0x60, //   ##     ##   
	    0x38, 0xE0, //   ###   ###   
	    0x1F, 0xC0, //    #######    
	    0x1F, 0xC0, //    #######    
	    0x38, 0xE0, //   ###   ###   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x38, 0xE0, //   ###   ###   
	    0x1F, 0xC0, //    #######    
	    0x0F, 0x80, //     #####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1000 '9' (14 pixels wide)
	    0x00, 0x00, //               
	    0x0F, 0x00, //     ####      
	    0x1F, 0xC0, //    #######    
	    0x38, 0xC0, //   ###   ##    
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x38, 0xE0, //   ###   ###   
	    0x1F, 0xE0, //    ########   
	    0x0F, 0x60, //     #### ##   
	    0x00, 0xE0, //         ###   
	    0x00, 0xC0, //         ##    
	    0x03, 0xC0, //       ####    
	    0x3F, 0x80, //   #######     
	    0x3E, 0x00, //   #####       
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1040 ':' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x03, 0x80, //       ###     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1080 ';' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x01, 0xC0, //        ###    
	    0x01, 0xC0, //        ###    
	    0x01, 0xC0, //        ###    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0x80, //       ###     
	    0x03, 0x00, //       ##      
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x04, 0x00, //      #        
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1120 '<' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x30, //           ##  
	    0x00, 0xF0, //         ####  
	    0x03, 0xC0, //       ####    
	    0x07, 0x00, //      ###      
	    0x1C, 0x00, //    ###        
	    0x78, 0x00, //  ####         
	    0x1C, 0x00, //    ###        
	    0x07, 0x00, //      ###      
	    0x03, 0xC0, //       ####    
	    0x00, 0xF0, //         ####  
	    0x00, 0x30, //           ##  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1160 '=' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x7F, 0xF0, //  ###########  
	    0x7F, 0xF0, //  ###########  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x7F, 0xF0, //  ###########  
	    0x7F, 0xF0, //  ###########  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1200 '>' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x30, 0x00, //   ##          
	    0x3C, 0x00, //   ####        
	    0x0F, 0x00, //     ####      
	    0x03, 0x80, //       ###     
	    0x00, 0xE0, //         ###   
	    0x00, 0x78, //          #### 
	    0x00, 0xE0, //         ###   
	    0x03, 0x80, //       ###     
	    0x0F, 0x00, //     ####      
	    0x3C, 0x00, //   ####        
	    0x30, 0x00, //   ##          
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1240 '?' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x0F, 0x80, //     #####     
	    0x1F, 0xC0, //    #######    
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x00, 0x60, //          ##   
	    0x01, 0xC0, //        ###    
	    0x03, 0x80, //       ###     
	    0x03, 0x00, //       ##      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1280 '@' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0x80, //       ###     
	    0x0C, 0x80, //     ##  #     
	    0x08, 0x40, //     #    #    
	    0x10, 0x40, //    #     #    
	    0x10, 0x40, //    #     #    
	    0x11, 0xC0, //    #   ###    
	    0x12, 0x40, //    #  #  #    
	    0x12, 0x40, //    #  #  #    
	    0x12, 0x40, //    #  #  #    
	    0x11, 0xC0, //    #   ###    
	    0x10, 0x00, //    #          
	    0x08, 0x00, //     #         
	    0x08, 0x40, //     #    #    
	    0x07, 0x80, //      ####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1320 'A' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x1F, 0x80, //    ######     
	    0x1F, 0x80, //    ######     
	    0x03, 0x80, //       ###     
	    0x06, 0xC0, //      ## ##    
	    0x06, 0xC0, //      ## ##    
	    0x0C, 0xC0, //     ##  ##    
	    0x0C, 0x60, //     ##   ##   
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x30, 0x30, //   ##      ##  
	    0x78, 0x78, //  ####    #### 
	    0x78, 0x78, //  ####    #### 
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1360 'B' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0x80, //   #######     
	    0x3F, 0xC0, //   ########    
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0xE0, //    ##   ###   
	    0x1F, 0xC0, //    #######    
	    0x1F, 0xE0, //    ########   
	    0x18, 0x70, //    ##    ###  
	    0x18, 0x30, //    ##     ##  
	    0x18, 0x30, //    ##     ##  
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xE0, //   #########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1400 'C' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0xB0, //      #### ##  
	    0x0F, 0xF0, //     ########  
	    0x1C, 0x70, //    ###   ###  
	    0x38, 0x30, //   ###     ##  
	    0x30, 0x00, //   ##          
	    0x30, 0x00, //   ##          
	    0x30, 0x00, //   ##          
	    0x30, 0x00, //   ##          
	    0x38, 0x30, //   ###     ##  
	    0x1C, 0x70, //    ###   ###  
	    0x0F, 0xE0, //     #######   
	    0x07, 0xC0, //      #####    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1440 'D' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x7F, 0x80, //  ########     
	    0x7F, 0xC0, //  #########    
	    0x30, 0xE0, //   ##    ###   
	    0x30, 0x70, //   ##     ###  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x70, //   ##     ###  
	    0x30, 0xE0, //   ##    ###   
	    0x7F, 0xC0, //  #########    
	    0x7F, 0x80, //  ########     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1480 'E' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x18, 0x30, //    ##     ##  
	    0x18, 0x30, //    ##     ##  
	    0x19, 0x80, //    ##  ##     
	    0x1F, 0x80, //    ######     
	    0x1F, 0x80, //    ######     
	    0x19, 0x80, //    ##  ##     
	    0x18, 0x30, //    ##     ##  
	    0x18, 0x30, //    ##     ##  
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1520 'F' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x18, 0x30, //    ##     ##  
	    0x18, 0x30, //    ##     ##  
	    0x19, 0x80, //    ##  ##     
	    0x1F, 0x80, //    ######     
	    0x1F, 0x80, //    ######     
	    0x19, 0x80, //    ##  ##     
	    0x18, 0x00, //    ##         
	    0x18, 0x00, //    ##         
	    0x3F, 0x00, //   ######      
	    0x3F, 0x00, //   ######      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1560 'G' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0xB0, //      #### ##  
	    0x1F, 0xF0, //    #########  
	    0x18, 0x70, //    ##    ###  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x00, //   ##          
	    0x30, 0x00, //   ##          
	    0x31, 0xF8, //   ##   ###### 
	    0x31, 0xF8, //   ##   ###### 
	    0x30, 0x30, //   ##      ##  
	    0x18, 0x30, //    ##     ##  
	    0x1F, 0xF0, //    #########  
	    0x07, 0xC0, //      #####    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1600 'H' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1640 'I' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1680 'J' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x03, 0xF8, //       ####### 
	    0x03, 0xF8, //       ####### 
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x30, 0xE0, //   ##    ###   
	    0x3F, 0xC0, //   ########    
	    0x0F, 0x80, //     #####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1720 'K' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3E, 0xF8, //   ##### ##### 
	    0x3E, 0xF8, //   ##### ##### 
	    0x18, 0xE0, //    ##   ###   
	    0x19, 0x80, //    ##  ##     
	    0x1B, 0x00, //    ## ##      
	    0x1F, 0x00, //    #####      
	    0x1D, 0x80, //    ### ##     
	    0x18, 0xC0, //    ##   ##    
	    0x18, 0xC0, //    ##   ##    
	    0x18, 0x60, //    ##    ##   
	    0x3E, 0x78, //   #####  #### 
	    0x3E, 0x38, //   #####   ### 
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1760 'L' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0x00, //   ######      
	    0x3F, 0x00, //   ######      
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x30, //     ##    ##  
	    0x0C, 0x30, //     ##    ##  
	    0x0C, 0x30, //     ##    ##  
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1800 'M' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x78, 0x78, //  ####    #### 
	    0x78, 0x78, //  ####    #### 
	    0x38, 0x70, //   ###    ###  
	    0x3C, 0xF0, //   ####  ####  
	    0x34, 0xB0, //   ## #  # ##  
	    0x37, 0xB0, //   ## #### ##  
	    0x37, 0xB0, //   ## #### ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x30, 0x30, //   ##      ##  
	    0x7C, 0xF8, //  #####  ##### 
	    0x7C, 0xF8, //  #####  ##### 
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1840 'N' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x39, 0xF0, //   ###  #####  
	    0x3D, 0xF0, //   #### #####  
	    0x1C, 0x60, //    ###   ##   
	    0x1E, 0x60, //    ####  ##   
	    0x1E, 0x60, //    ####  ##   
	    0x1B, 0x60, //    ## ## ##   
	    0x1B, 0x60, //    ## ## ##   
	    0x19, 0xE0, //    ##  ####   
	    0x19, 0xE0, //    ##  ####   
	    0x18, 0xE0, //    ##   ###   
	    0x3E, 0xE0, //   ##### ###   
	    0x3E, 0x60, //   #####  ##   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1880 'O' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0x80, //      ####     
	    0x0F, 0xC0, //     ######    
	    0x1C, 0xE0, //    ###  ###   
	    0x38, 0x70, //   ###    ###  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x38, 0x70, //   ###    ###  
	    0x1C, 0xE0, //    ###  ###   
	    0x0F, 0xC0, //     ######    
	    0x07, 0x80, //      ####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1920 'P' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0xC0, //   ########    
	    0x3F, 0xE0, //   #########   
	    0x18, 0x70, //    ##    ###  
	    0x18, 0x30, //    ##     ##  
	    0x18, 0x30, //    ##     ##  
	    0x18, 0x70, //    ##    ###  
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xC0, //    #######    
	    0x18, 0x00, //    ##         
	    0x18, 0x00, //    ##         
	    0x3F, 0x00, //   ######      
	    0x3F, 0x00, //   ######      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @1960 'Q' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0x80, //      ####     
	    0x0F, 0xC0, //     ######    
	    0x1C, 0xE0, //    ###  ###   
	    0x38, 0x70, //   ###    ###  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x38, 0x70, //   ###    ###  
	    0x1C, 0xE0, //    ###  ###   
	    0x0F, 0xC0, //     ######    
	    0x07, 0x80, //      ####     
	    0x07, 0xB0, //      #### ##  
	    0x0F, 0xF0, //     ########  
	    0x0C, 0xE0, //     ##  ###   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2000 'R' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0xC0, //   ########    
	    0x3F, 0xE0, //   #########   
	    0x18, 0x70, //    ##    ###  
	    0x18, 0x30, //    ##     ##  
	    0x18, 0x70, //    ##    ###  
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xC0, //    #######    
	    0x18, 0xE0, //    ##   ###   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x70, //    ##    ###  
	    0x3E, 0x38, //   #####   ### 
	    0x3E, 0x18, //   #####    ## 
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2040 'S' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x0F, 0xB0, //     ##### ##  
	    0x1F, 0xF0, //    #########  
	    0x38, 0x70, //   ###    ###  
	    0x30, 0x30, //   ##      ##  
	    0x38, 0x00, //   ###         
	    0x1F, 0x80, //    ######     
	    0x07, 0xE0, //      ######   
	    0x00, 0x70, //          ###  
	    0x30, 0x30, //   ##      ##  
	    0x38, 0x70, //   ###    ###  
	    0x3F, 0xE0, //   #########   
	    0x37, 0xC0, //   ## #####    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2080 'T' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x33, 0x30, //   ##  ##  ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x0F, 0xC0, //     ######    
	    0x0F, 0xC0, //     ######    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2120 'U' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x1C, 0xE0, //    ###  ###   
	    0x0F, 0xC0, //     ######    
	    0x07, 0x80, //      ####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2160 'V' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x78, 0xF0, //  ####   ####  
	    0x78, 0xF0, //  ####   ####  
	    0x30, 0x60, //   ##     ##   
	    0x30, 0x60, //   ##     ##   
	    0x18, 0xC0, //    ##   ##    
	    0x18, 0xC0, //    ##   ##    
	    0x0D, 0x80, //     ## ##     
	    0x0D, 0x80, //     ## ##     
	    0x0D, 0x80, //     ## ##     
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2200 'W' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x7C, 0x7C, //  #####   #####
	    0x7C, 0x7C, //  #####   #####
	    0x30, 0x18, //   ##       ## 
	    0x33, 0x98, //   ##  ###  ## 
	    0x33, 0x98, //   ##  ###  ## 
	    0x33, 0x98, //   ##  ###  ## 
	    0x36, 0xD8, //   ## ## ## ## 
	    0x16, 0xD0, //    # ## ## #  
	    0x1C, 0x70, //    ###   ###  
	    0x1C, 0x70, //    ###   ###  
	    0x1C, 0x70, //    ###   ###  
	    0x18, 0x30, //    ##     ##  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2240 'X' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x78, 0xF0, //  ####   ####  
	    0x78, 0xF0, //  ####   ####  
	    0x30, 0x60, //   ##     ##   
	    0x18, 0xC0, //    ##   ##    
	    0x0D, 0x80, //     ## ##     
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x0D, 0x80, //     ## ##     
	    0x18, 0xC0, //    ##   ##    
	    0x30, 0x60, //   ##     ##   
	    0x78, 0xF0, //  ####   ####  
	    0x78, 0xF0, //  ####   ####  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2280 'Y' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x18, 0x60, //    ##    ##   
	    0x0C, 0xC0, //     ##  ##    
	    0x07, 0x80, //      ####     
	    0x07, 0x80, //      ####     
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x0F, 0xC0, //     ######    
	    0x0F, 0xC0, //     ######    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2320 'Z' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0xC0, //    ##   ##    
	    0x01, 0x80, //        ##     
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x06, 0x00, //      ##       
	    0x0C, 0x60, //     ##   ##   
	    0x18, 0x60, //    ##    ##   
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2360 '[' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0xC0, //       ####    
	    0x03, 0xC0, //       ####    
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0xC0, //       ####    
	    0x03, 0xC0, //       ####    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2400 '\' (14 pixels wide)
	    0x18, 0x00, //    ##         
	    0x18, 0x00, //    ##         
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x01, 0x80, //        ##     
	    0x01, 0x80, //        ##     
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0x60, //          ##   
	    0x00, 0x60, //          ##   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2440 ']' (14 pixels wide)
	    0x00, 0x00, //               
	    0x0F, 0x00, //     ####      
	    0x0F, 0x00, //     ####      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x0F, 0x00, //     ####      
	    0x0F, 0x00, //     ####      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2480 '^' (14 pixels wide)
	    0x00, 0x00, //               
	    0x02, 0x00, //       #       
	    0x07, 0x00, //      ###      
	    0x0D, 0x80, //     ## ##     
	    0x18, 0xC0, //    ##   ##    
	    0x30, 0x60, //   ##     ##   
	    0x20, 0x20, //   #       #   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2520 '_' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0xFF, 0xFC, // ##############
	    0xFF, 0xFC, // ##############

	    // @2560 '`' (14 pixels wide)
	    0x00, 0x00, //               
	    0x04, 0x00, //      #        
	    0x03, 0x00, //       ##      
	    0x00, 0x80, //         #     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2600 'a' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x0F, 0xC0, //     ######    
	    0x1F, 0xE0, //    ########   
	    0x00, 0x60, //          ##   
	    0x0F, 0xE0, //     #######   
	    0x1F, 0xE0, //    ########   
	    0x38, 0x60, //   ###    ##   
	    0x30, 0xE0, //   ##    ###   
	    0x3F, 0xF0, //   ##########  
	    0x1F, 0x70, //    ##### ###  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2640 'b' (14 pixels wide)
	    0x00, 0x00, //               
	    0x70, 0x00, //  ###          
	    0x70, 0x00, //  ###          
	    0x30, 0x00, //   ##          
	    0x30, 0x00, //   ##          
	    0x37, 0x80, //   ## ####     
	    0x3F, 0xE0, //   #########   
	    0x38, 0x60, //   ###    ##   
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x38, 0x60, //   ###    ##   
	    0x7F, 0xE0, //  ##########   
	    0x77, 0x80, //  ### ####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2680 'c' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0xB0, //      #### ##  
	    0x1F, 0xF0, //    #########  
	    0x18, 0x30, //    ##     ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x00, //   ##          
	    0x30, 0x00, //   ##          
	    0x38, 0x30, //   ###     ##  
	    0x1F, 0xF0, //    #########  
	    0x0F, 0xC0, //     ######    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2720 'd' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x70, //          ###  
	    0x00, 0x70, //          ###  
	    0x00, 0x30, //           ##  
	    0x00, 0x30, //           ##  
	    0x07, 0xB0, //      #### ##  
	    0x1F, 0xF0, //    #########  
	    0x18, 0x70, //    ##    ###  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x38, 0x70, //   ###    ###  
	    0x1F, 0xF8, //    ########## 
	    0x07, 0xB8, //      #### ### 
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2760 'e' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0x80, //      ####     
	    0x1F, 0xE0, //    ########   
	    0x18, 0x60, //    ##    ##   
	    0x3F, 0xF0, //   ##########  
	    0x3F, 0xF0, //   ##########  
	    0x30, 0x00, //   ##          
	    0x18, 0x30, //    ##     ##  
	    0x1F, 0xF0, //    #########  
	    0x07, 0xC0, //      #####    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2800 'f' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0xF0, //       ######  
	    0x07, 0xF0, //      #######  
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2840 'g' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0xB8, //      #### ### 
	    0x1F, 0xF8, //    ########## 
	    0x18, 0x70, //    ##    ###  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x18, 0x70, //    ##    ###  
	    0x1F, 0xF0, //    #########  
	    0x07, 0xB0, //      #### ##  
	    0x00, 0x30, //           ##  
	    0x00, 0x70, //          ###  
	    0x0F, 0xE0, //     #######   
	    0x0F, 0xC0, //     ######    
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2880 'h' (14 pixels wide)
	    0x00, 0x00, //               
	    0x38, 0x00, //   ###         
	    0x38, 0x00, //   ###         
	    0x18, 0x00, //    ##         
	    0x18, 0x00, //    ##         
	    0x1B, 0xC0, //    ## ####    
	    0x1F, 0xE0, //    ########   
	    0x1C, 0x60, //    ###   ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2920 'i' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x1F, 0x00, //    #####      
	    0x1F, 0x00, //    #####      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @2960 'j' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x1F, 0xC0, //    #######    
	    0x1F, 0xC0, //    #######    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x00, 0xC0, //         ##    
	    0x01, 0xC0, //        ###    
	    0x3F, 0x80, //   #######     
	    0x3F, 0x00, //   ######      
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3000 'k' (14 pixels wide)
	    0x00, 0x00, //               
	    0x38, 0x00, //   ###         
	    0x38, 0x00, //   ###         
	    0x18, 0x00, //    ##         
	    0x18, 0x00, //    ##         
	    0x1B, 0xE0, //    ## #####   
	    0x1B, 0xE0, //    ## #####   
	    0x1B, 0x00, //    ## ##      
	    0x1E, 0x00, //    ####       
	    0x1E, 0x00, //    ####       
	    0x1B, 0x00, //    ## ##      
	    0x19, 0x80, //    ##  ##     
	    0x39, 0xF0, //   ###  #####  
	    0x39, 0xF0, //   ###  #####  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3040 'l' (14 pixels wide)
	    0x00, 0x00, //               
	    0x1F, 0x00, //    #####      
	    0x1F, 0x00, //    #####      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3080 'm' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x7E, 0xE0, //  ###### ###   
	    0x7F, 0xF0, //  ###########  
	    0x33, 0x30, //   ##  ##  ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x33, 0x30, //   ##  ##  ##  
	    0x7B, 0xB8, //  #### ### ### 
	    0x7B, 0xB8, //  #### ### ### 
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3120 'n' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3B, 0xC0, //   ### ####    
	    0x3F, 0xE0, //   #########   
	    0x1C, 0x60, //    ###   ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3160 'o' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0x80, //      ####     
	    0x1F, 0xE0, //    ########   
	    0x18, 0x60, //    ##    ##   
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x18, 0x60, //    ##    ##   
	    0x1F, 0xE0, //    ########   
	    0x07, 0x80, //      ####     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3200 'p' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x77, 0x80, //  ### ####     
	    0x7F, 0xE0, //  ##########   
	    0x38, 0x60, //   ###    ##   
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x38, 0x60, //   ###    ##   
	    0x3F, 0xE0, //   #########   
	    0x37, 0x80, //   ## ####     
	    0x30, 0x00, //   ##          
	    0x30, 0x00, //   ##          
	    0x7C, 0x00, //  #####        
	    0x7C, 0x00, //  #####        
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3240 'q' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0xB8, //      #### ### 
	    0x1F, 0xF8, //    ########## 
	    0x18, 0x70, //    ##    ###  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x30, 0x30, //   ##      ##  
	    0x18, 0x70, //    ##    ###  
	    0x1F, 0xF0, //    #########  
	    0x07, 0xB0, //      #### ##  
	    0x00, 0x30, //           ##  
	    0x00, 0x30, //           ##  
	    0x00, 0xF8, //         ##### 
	    0x00, 0xF8, //         ##### 
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3280 'r' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3C, 0xE0, //   ####  ###   
	    0x3D, 0xF0, //   #### #####  
	    0x0F, 0x30, //     ####  ##  
	    0x0E, 0x00, //     ###       
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x3F, 0xC0, //   ########    
	    0x3F, 0xC0, //   ########    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3320 's' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x07, 0xE0, //      ######   
	    0x1F, 0xE0, //    ########   
	    0x18, 0x60, //    ##    ##   
	    0x1E, 0x00, //    ####       
	    0x0F, 0xC0, //     ######    
	    0x01, 0xE0, //        ####   
	    0x18, 0x60, //    ##    ##   
	    0x1F, 0xE0, //    ########   
	    0x1F, 0x80, //    ######     
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3360 't' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x3F, 0xE0, //   #########   
	    0x3F, 0xE0, //   #########   
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x00, //     ##        
	    0x0C, 0x30, //     ##    ##  
	    0x0F, 0xF0, //     ########  
	    0x07, 0xC0, //      #####    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3400 'u' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x38, 0xE0, //   ###   ###   
	    0x38, 0xE0, //   ###   ###   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0x60, //    ##    ##   
	    0x18, 0xE0, //    ##   ###   
	    0x1F, 0xF0, //    #########  
	    0x0F, 0x70, //     #### ###  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3440 'v' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x78, 0xF0, //  ####   ####  
	    0x78, 0xF0, //  ####   ####  
	    0x30, 0x60, //   ##     ##   
	    0x18, 0xC0, //    ##   ##    
	    0x18, 0xC0, //    ##   ##    
	    0x0D, 0x80, //     ## ##     
	    0x0D, 0x80, //     ## ##     
	    0x07, 0x00, //      ###      
	    0x07, 0x00, //      ###      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3480 'w' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x78, 0xF0, //  ####   ####  
	    0x78, 0xF0, //  ####   ####  
	    0x32, 0x60, //   ##  #  ##   
	    0x32, 0x60, //   ##  #  ##   
	    0x37, 0xE0, //   ## ######   
	    0x1D, 0xC0, //    ### ###    
	    0x1D, 0xC0, //    ### ###    
	    0x18, 0xC0, //    ##   ##    
	    0x18, 0xC0, //    ##   ##    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3520 'x' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x0C, 0xC0, //     ##  ##    
	    0x07, 0x80, //      ####     
	    0x03, 0x00, //       ##      
	    0x07, 0x80, //      ####     
	    0x0C, 0xC0, //     ##  ##    
	    0x3C, 0xF0, //   ####  ####  
	    0x3C, 0xF0, //   ####  ####  
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3560 'y' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x78, 0xF0, //  ####   ####  
	    0x78, 0xF0, //  ####   ####  
	    0x30, 0x60, //   ##     ##   
	    0x18, 0xC0, //    ##   ##    
	    0x18, 0xC0, //    ##   ##    
	    0x0D, 0x80, //     ## ##     
	    0x0F, 0x80, //     #####     
	    0x07, 0x00, //      ###      
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x0C, 0x00, //     ##        
	    0x7F, 0x00, //  #######      
	    0x7F, 0x00, //  #######      
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3600 'z' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x18, 0xC0, //    ##   ##    
	    0x01, 0x80, //        ##     
	    0x03, 0x00, //       ##      
	    0x06, 0x00, //      ##       
	    0x0C, 0x60, //     ##   ##   
	    0x1F, 0xE0, //    ########   
	    0x1F, 0xE0, //    ########   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3640 '{' (14 pixels wide)
	    0x00, 0x00, //               
	    0x01, 0xC0, //        ###    
	    0x03, 0xC0, //       ####    
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x07, 0x00, //      ###      
	    0x0E, 0x00, //     ###       
	    0x07, 0x00, //      ###      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0xC0, //       ####    
	    0x01, 0xC0, //        ###    
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3680 '|' (14 pixels wide)
	    0x00, 0x00, //               
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x03, 0x00, //       ##      
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3720 '}' (14 pixels wide)
	    0x00, 0x00, //               
	    0x1C, 0x00, //    ###        
	    0x1E, 0x00, //    ####       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x07, 0x00, //      ###      
	    0x03, 0x80, //       ###     
	    0x07, 0x00, //      ###      
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x06, 0x00, //      ##       
	    0x1E, 0x00, //    ####       
	    0x1C, 0x00, //    ###        
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               

	    // @3760 '~' (14 pixels wide)
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x0E, 0x00, //     ###       
	    0x3F, 0x30, //   ######  ##  
	    0x33, 0xF0, //   ##  ######  
	    0x01, 0xE0, //        ####   
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
	    0x00, 0x00, //               
    };
}