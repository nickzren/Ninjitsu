#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using G.GameObjects.Streams;
#endregion

namespace G.GameObjects.Collisions
{
    class PerPixel
    {
        
        public PerPixel()
        {
        
        }

        //Per Pixel Detectoin
        public static Boolean Collision(Rectangle obj1Rectangle, Color[] obj1ColorData, Rectangle obj2Rectangle, Color[] obj2ColorData)
        {
            int top = Math.Max(obj1Rectangle.Top, obj2Rectangle.Top);
            int bottom = Math.Min(obj1Rectangle.Bottom, obj2Rectangle.Bottom);
            int left = Math.Max(obj1Rectangle.Left, obj2Rectangle.Left);
            int right = Math.Min(obj1Rectangle.Right, obj2Rectangle.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Check color of each pixel
                    int obj1Index = (x - obj1Rectangle.Left) + (y - obj1Rectangle.Top) * obj1Rectangle.Width;
                    int obj2Index = (x - obj2Rectangle.Left) + (y - obj2Rectangle.Top) * obj2Rectangle.Width;

                    Color color1 = obj1ColorData[obj1Index];
                    Color color2 = obj2ColorData[obj2Index];

                    if (color1.A != 0 && color2.A != 0)
                        return true;
                }
            }

            return false;
        }

    }
}
