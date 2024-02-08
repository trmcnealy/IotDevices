
using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

using Iot.Device.Board;
using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Iot.Device.Ssd1351;

using SkiaSharp;

using UnitsNet;

using static System.Net.Mime.MediaTypeNames;

namespace RaspberryPiDevices;

public sealed class PageChangedEventArgs : EventArgs
{
    public int Index
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get;
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        set;
    }

    public string PageText
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get;
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        set;
    }

    public PageChangedEventArgs(int index, string pageText)
    {
        Index = index;
        PageText = pageText;
    }
}

public sealed class OLEDDisplay : IDisposable
{
    private bool disposedValue;

    public int Width
    {
        get; private set;
    }

    public int Height
    {
        get; private set;
    }

    //public int PageIndex;

    //public Dictionary<int, string> Pages
    //{
    //    get; private set;
    //}

    private SKFont _sKFont;

    //private TimeSpan _PageChangingInterval;
    //private readonly System.Timers.Timer _Timer;

    public int FontSize
    {
        get; private set;
    }
    public string FontFamilyName
    {
        get; private set;
    }

    public BitmapImage Image
    {
        get; private set;
    }

    public OLEDDisplay(in int widthPixel, in int heightPixel, in int fontSize = 12, in string fontFamilyName = "Cascadia Code")
    {
        Width = widthPixel;
        Height = heightPixel;
        FontSize = fontSize;
        FontFamilyName = fontFamilyName;
        _sKFont = new SKFont(SKTypeface.FromFamilyName(FontFamilyName), FontSize);
        Image = BitmapImage.CreateBitmap(Width, Height, PixelFormat.Format32bppArgb);
    }



    //public event EventHandler<PageChangedEventArgs>? PageChangedEvent;

    //private EventWaitHandle autoResetEventHandle;

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public OLEDDisplay(in int numberOfPages, in int widthPixel, in int heightPixel, in int fontSize = 12, in string fontFamilyName = "Cascadia Code")
    //    : this(numberOfPages, widthPixel, heightPixel, fontSize, fontFamilyName)
    //{
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public OLEDDisplay(in int numberOfPages, in int widthPixel, in int heightPixel, in int fontSize = 12, in string fontFamilyName = "Cascadia Code")
    //{
    //    Width = widthPixel;
    //    Height = heightPixel;

    //    FontSize = fontSize;
    //    FontFamilyName = fontFamilyName;

    //    _sKFont = new SKFont(SKTypeface.FromFamilyName(fontFamilyName), fontSize);

    //    Image = BitmapImage.CreateBitmap(Width, Height, PixelFormat.Format32bppArgb);

    //    //Pages = new Dictionary<int, string>(numberOfPages);
    //    //CreatePages(numberOfPages);

    //    //_PageChangingInterval = pageChangingInterval;

    //    //autoResetEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

    //    //_Timer = new Timer(DisplayCallback, autoResetEventHandle, _PageChangingInterval, _PageChangingInterval);
    //}

    //public bool Wait()
    //{
    //    return autoResetEventHandle.WaitOne();
    //}


    #region Dctor
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    ~OLEDDisplay()
    {
        Dispose(disposing: false);
    }

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    private void Dispose(bool disposing)
    {
        lock (this)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //_Timer.Dispose();

                    //for (int i = 0; i < Pages.Count; i++)
                    //{
                    //    Pages[i].Page.Dispose();
                    //}

                    _sKFont.Dispose();

                    //autoResetEventHandle.Dispose();
                }

                // unmanaged
                disposedValue = true;
            }
        }
    }

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public bool IsDisposed
    {
        get
        {
            return disposedValue;
        }
    }
    #endregion

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private static int InterlockedIncrementMod(ref int location, in int modulo)
    //{
    //    int current = Interlocked.Increment(ref location);
    //    return (current % modulo);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void DisplayCallback(object? sender)
    //{
    //    //autoResetEvent.Set();
    //    //if (sender is OLEDDisplay oLEDDisplay)
    //    {
    //        Interlocked.Increment(ref PageIndex);

    //        PageIndex %= Pages.Count; //InterlockedIncrementMod(ref PageIndex, Pages.Count);

    //        PageChanged(PageIndex, Pages[PageIndex]);
    //    }
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void PageChanged(int index, string pageText)
    //{
    //    PageChangedEventArgs args = new PageChangedEventArgs(index, pageText);
    //    OnPageChangedEvent(args);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void OnPageChangedEvent(PageChangedEventArgs e)
    //{
    //    PageChangedEvent?.Invoke(this, e);
    //}

    //public BitmapImage this[in int index]
    //{
    //    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //    get
    //    {
    //        return Pages[index];
    //    }
    //    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //    set
    //    {
    //        Pages[index] = value;
    //    }
    //}

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public bool InBounds(string text, out float measuredWidth)
    {
        measuredWidth = MeasureText(text, out SKRect bounds);

        if ((bounds.Height > Height) || (bounds.Width > Width))
        {
            return false;
        }
        return true;
    }

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public float MeasureText(string text, out SKRect bounds)
    {
        Span<ushort> glyphs = new ushort[_sKFont.CountGlyphs(text)];

        _sKFont.GetGlyphs(text, glyphs);

        return _sKFont.MeasureText(glyphs, out bounds);
    }

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public float GetFontWidth(ReadOnlySpan<char> text)
    {
        Span<ushort> glyphs = new ushort[_sKFont.CountGlyphs(text)];

        _sKFont.GetGlyphs(text, glyphs);

        _sKFont.MeasureText(glyphs, out SKRect bounds);

        return bounds.Width;
    }

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private BitmapImage CreatePage()
    //{
    //    Pages.Add(new(Pages.Count, BitmapImage.CreateBitmap(Width, Height, PixelFormat.Format32bppArgb)));

    //    return Pages[Pages.Count - 1].Page;
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void CreatePages(in int count)
    //{
    //    Span<char> FullBlockChar = new char[] { '█' };

    //    Pages.Clear();

    //    Pages.EnsureCapacity(count);

    //    int numChars = (int)MathF.Round((128.0f / GetFontWidth(FullBlockChar)), MidpointRounding.ToZero);

    //    for (int i = 0; i < count; i++)
    //    {
    //        Pages.Add(i, new string(new char[numChars]));
    //    }
    //}



    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public bool SetText(in int pageIndex, in string text)
    //{
    //    if (Pages.ContainsKey(pageIndex))
    //    {
    //        Pages[pageIndex] = text;

    //        return true;
    //    }
    //    return false;
    //}

    //private static readonly Color Red = Ssd1351Color.Convert(Color.Red);
    //private static readonly Color Green = Ssd1351Color.Convert(Color.Green);
    //private static readonly Color Blue = Ssd1351Color.Convert(Color.Blue);
    //private static readonly Color White = Ssd1351Color.Convert(Color.White);

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public void DisplayRedText(Ssd1351 ssd1351, in int pageIndex)
    //{
    //    DisplayText(ssd1351, pageIndex, Color.Red);
    //}
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public void DisplayGreenText(Ssd1351 ssd1351, in int pageIndex)
    //{
    //    DisplayText(ssd1351, pageIndex, Color.Green);
    //}
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public void DisplayBlueText(Ssd1351 ssd1351, in int pageIndex)
    //{
    //    DisplayText(ssd1351, pageIndex, Color.Blue);
    //}
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public void DisplayWhiteText(Ssd1351 ssd1351, in int pageIndex)
    //{
    //    DisplayText(ssd1351, pageIndex, Color.White);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public void DisplayText(Ssd1351 ssd1351, in int pageIndex, in Color color)
    //{
    //    _bitmapImage.Clear(Color.Black);
    //    _bitmapImage.GetDrawingApi().DrawText(Pages[pageIndex], _fontFamilyName, _fontSize, Ssd1351Color.Convert(color), Point.Empty);

    //    ssd1351.SendBitmap(_bitmapImage);

    //    Wait();
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void DisplayText(Ssd1351 ssd1351, in int pageIndex, in Color color, in Point offset)
    //{
    //    //_bitmapImage.Clear(Color.Black);
    //    //_bitmapImage.GetDrawingApi().DrawText(Pages[pageIndex], _fontFamilyName, _fontSize, Ssd1351Color.Convert(color), offset);

    //    _bitmapImage = BitmapImage.CreateBitmap(Width, Height, PixelFormat.Format32bppArgb);
    //    _bitmapImage.GetDrawingApi().DrawText(Pages[pageIndex], _fontFamilyName, _fontSize, Ssd1351Color.Convert(color), offset);

    //    ssd1351.SendBitmap(_bitmapImage);

    //    Wait();
    //}


    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public async Task DisplayRedTextAsync(Ssd1351 ssd1351, int pageIndex)
    //{
    //    await DisplayTextAsync(ssd1351, pageIndex, Color.Red);
    //}
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public async Task DisplayGreenTextAsync(Ssd1351 ssd1351, int pageIndex)
    //{
    //    await DisplayTextAsync(ssd1351, pageIndex, Color.Green);
    //}
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public async Task DisplayBlueTextAsync(Ssd1351 ssd1351, int pageIndex)
    //{
    //    await DisplayTextAsync(ssd1351, pageIndex, Color.Blue);
    //}
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public async Task DisplayWhiteTextAsync(Ssd1351 ssd1351, int pageIndex)
    //{
    //    await DisplayTextAsync(ssd1351, pageIndex, Color.White);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public async Task DisplayTextAsync(Ssd1351 ssd1351, int pageIndex, Color color)
    //{
    //    Image.Clear(Color.Black);
    //    Image.GetDrawingApi().DrawText(Pages[pageIndex], FontFamilyName, FontSize, Ssd1351Color.Convert(color), Point.Empty);

    //    await SendBitmapAsync(ssd1351, Image);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private async Task DisplayTextAsync(Ssd1351 ssd1351, int pageIndex, Color color, Point offset)
    //{
    //    //_bitmapImage.Clear(Color.Black);
    //    //_bitmapImage.GetDrawingApi().DrawText(Pages[pageIndex], _fontFamilyName, _fontSize, Ssd1351Color.Convert(color), offset);

    //    _bitmapImage = BitmapImage.CreateBitmap(Width, Height, PixelFormat.Format32bppArgb);
    //    _bitmapImage.GetDrawingApi().DrawText(Pages[pageIndex], _fontFamilyName, _fontSize, Ssd1351Color.Convert(color), offset);

    //    await SendBitmapAsync(ssd1351, _bitmapImage);
    //}


    //private static async Task SendBitmapAsync(Ssd1351 ssd1351, BitmapImage bitmapImage)
    //{
    //    ssd1351.SendBitmap(bitmapImage);

    //    await Task.CompletedTask.ConfigureAwait(false);
    //}


    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void DisplayText(Ssd1351 ssd1351, in int pageIndex, in Color color, in int startX = 0, in int startY = 0)
    //{
    //    _bitmapImage = BitmapImage.CreateBitmap(Width, Height, PixelFormat.Format32bppArgb);
    //    _bitmapImage.GetDrawingApi().DrawText(Pages[pageIndex], _fontFamilyName, _fontSize, Ssd1351Color.Convert(color), new Point(startX, startY));

    //    ssd1351.SendBitmap(_bitmapImage);


    //    Wait();
    //}


    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public void DisplayPage(in int pageIndex)
    //{
    //    BitmapImage page = Pages[pageIndex];

    //    //page.Clear(Color.Black);

    //    IGraphics graphics = page.GetDrawingApi();

    //    graphics.DrawImage(page, _imageOffset.X, _imageOffset.Y);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void DisplayPage(in int pageIndex, in int startX = 0, in int startY = 0)
    //{
    //    BitmapImage page = Pages[pageIndex];

    //    page.Clear(Color.Black);

    //    IGraphics graphics = page.GetDrawingApi();

    //    graphics.DrawImage(page, startX, startY);
    //}













    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public static OLEDDisplay Create_2_42in_OLED_Ssd1351(int fontSize = 12, string fontFamilyName = "Cascadia Code")
    {
        return new OLEDDisplay(128, 128, fontSize, fontFamilyName);
    }

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public static OLEDDisplay Create_2_42in_OLED_Ssd1351(int numberOfPages, TimeSpan pageChangingInterval, int fontSize = 12, string fontFamilyName = "Cascadia Code")
    //{
    //    return new OLEDDisplay(numberOfPages, 128, 128, pageChangingInterval, fontSize, fontFamilyName);
    //}


    //public static async Task WhenPageChanges(OLEDDisplay oLEDDisplay)
    //{
    //    TaskCompletionSource<OLEDDisplay> tcs = new TaskCompletionSource<OLEDDisplay>();

    //    EventHandler<PageChangedEventArgs> onPageChangedEvent = (sender, e) =>
    //    {
    //        tcs.TrySetResult(oLEDDisplay);
    //    };

    //    oLEDDisplay.PageChangedEvent -= onPageChangedEvent;
    //    oLEDDisplay.PageChangedEvent += onPageChangedEvent;

    //    await tcs.Task;
    //}

}
