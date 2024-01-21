
using SkiaSharp;

namespace RaspberryPiDevices;

public sealed class OLEDDisplayPage
{
    public string Text
    {
        get; set;
    }

    public OLEDDisplayPage(int numberOfChars)
    {
        Text = new string(new char[numberOfChars]);
    }
}



public sealed class OLEDDisplay
{
    public int Width
    {
        get; private set;
    }

    public int Height
    {
        get; private set;
    }

    public List<OLEDDisplayPage> Pages
    {
        get; private set;
    }

    private SKFont _sKFont;

    public OLEDDisplay(int widthPixel, int heightPixel, int fontSize = 12, string fontFamilyName = "Cascadia Code")
    {
        Width = widthPixel;
        Height = heightPixel;
        Pages = new List<OLEDDisplayPage>();

        _sKFont = new SKFont(SKTypeface.FromFamilyName(fontFamilyName), fontSize);
    }

    public OLEDDisplayPage this[int index]
    {
        get
        {
            return Pages[index];
        }
        set
        {
            Pages[index] = value;
        }
    }

    public bool InBounds(string text, out float measuredWidth)
    {
        measuredWidth = MeasureText(text, out SKRect bounds);

        if((bounds.Height > Height) || (bounds.Width > Width))
        {
            return false;
        }
        return true;
    }

    public float MeasureText(string text, out SKRect bounds)
    {
        Span<ushort> glyphs = new ushort[_sKFont.CountGlyphs(text)];

        _sKFont.GetGlyphs(text, glyphs);

        return _sKFont.MeasureText(glyphs, out bounds);
    }
    
    public OLEDDisplayPage CreatePage()
    {
        OLEDDisplayPage page = new OLEDDisplayPage(Width * Height);
        Pages.Add(page);
        return page;
    }

    public void CreatePages(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreatePage();
        }
    }
}
