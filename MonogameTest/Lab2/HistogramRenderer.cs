using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using MonoGame.Extended;

public class HistogramRenderer
{
    private GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;

    public HistogramRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public void DrawHistogram(SpriteBatch spriteBatch, Dictionary<byte, int> histogram, Color color, Rectangle bounds)
    {
        _spriteBatch.Begin();

        int maxCount = histogram.Values.Max();
        int columnWidth = bounds.Width / histogram.Count;

        foreach (var kvp in histogram)
        {
            float normalizedHeight = (float)kvp.Value / maxCount;
            var columnRect = new Rectangle(bounds.Left + kvp.Key * columnWidth, bounds.Bottom - (int)(normalizedHeight * bounds.Height), columnWidth, (int)(normalizedHeight * bounds.Height));

            // Отрисовка столбцов гистограммы
            _spriteBatch.DrawRectangle(columnRect, Color.Black);
            _spriteBatch.FillRectangle(columnRect, color);
        }

        _spriteBatch.End();
    }
}
