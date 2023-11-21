using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    public class LightSource
    {
        public Vector3 Position { get; set; }
        public Color Color {  get; set; }
        public LightSource(Vector3 position, Color color)
        {
            this.Position = position;
            this.Color = color;
        }   
        public static LightSource GetWhite(Vector3 position)
        {
            return new LightSource(position, Color.White);
        }
    }
}
