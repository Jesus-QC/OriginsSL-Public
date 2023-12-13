using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace OriginsBot;

public class LevelingImageBuilder
{
    private static class FontRetriever
    {
        public static readonly string DataDirectory = Path.Combine(Environment.CurrentDirectory, "Data");
        
        public static Font UsernameFont { get; }
        public static Font LevelFont { get; }
        public static Font ExpFont { get; }
        public static Font RankFont { get; }

        public static readonly Color MainColor = new (new Rgb24(179, 152, 255));
        public static readonly Color SecondaryColor = new (new Rgb24(158, 210, 255));

        public static RichTextOptions GetCenteredOptions(Font font, PointF position) => new (font)
        {
            Origin = position,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        
        static FontRetriever()
        {
            FontCollection collection = new();
            FontFamily family = collection.Add(Path.Combine(DataDirectory, "Starborn.ttf"));
            UsernameFont = family.CreateFont(49.7f, FontStyle.Regular);
            LevelFont = family.CreateFont(40.97f, FontStyle.Regular);
            ExpFont = family.CreateFont(24.54f, FontStyle.Regular);
            RankFont = family.CreateFont(50, FontStyle.Regular);
        }
    }
    
    public LevelingImageBuilder(string username, int level, int xp, int xpToNextLevel, int rank)
    {
        Username = username;
        Level = level;
        Xp = xp;
        XpToNextLevel = xpToNextLevel;
        Rank = rank;
    }

    private string Username { get; }
    private int Level { get; }
    private int Xp { get; }
    private int XpToNextLevel { get; }
    private int Rank { get; }

    public void Build()
    {
        Image image = Image.Load(Path.Combine(FontRetriever.DataDirectory, "Blank_75.png"));
        
        image.Mutate(x => x.DrawText(Username, FontRetriever.UsernameFont, new SolidBrush(FontRetriever.MainColor), new SolidPen(Color.Black, 4), new PointF(36, 26)));
        image.Mutate(x => x.DrawText(FontRetriever.GetCenteredOptions(FontRetriever.LevelFont, new PointF(175, 89)), Level.ToString().PadLeft(4, '0'), new SolidBrush(FontRetriever.SecondaryColor), new SolidPen(Color.Black, 4)));
        image.Mutate(x => x.DrawText(FontRetriever.GetCenteredOptions(FontRetriever.ExpFont, new PointF(286, 161)),Xp.ToString().PadLeft(4, '0') + "/" + XpToNextLevel.ToString().PadLeft(4, '0'), new SolidBrush(FontRetriever.SecondaryColor), new SolidPen(Color.Black, 2)));
        image.Mutate(x => x.DrawText(FontRetriever.GetCenteredOptions(FontRetriever.RankFont, new PointF(702, 90)),"#" + Rank, new SolidBrush(FontRetriever.SecondaryColor), new SolidPen(Color.Black, 4)));
        
        image.Save(Path.Combine(FontRetriever.DataDirectory, "output.png"));
    }
}