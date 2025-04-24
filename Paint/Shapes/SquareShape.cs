using System;
using System.Drawing;

namespace Paint.Shapes
{
    public class SquareShape : RectangleShape
    {
        public override void Scale(float scaleX, float scaleY)
        {
            float scale = Math.Max(scaleX, scaleY);
            base.Scale(scale, scale);
        }

        protected override Rectangle GetRectangle()
        {
            int size = Math.Max(Math.Abs(EndPoint.X - StartPoint.X), Math.Abs(EndPoint.Y - StartPoint.Y));
            int x = StartPoint.X;
            int y = StartPoint.Y;
            
            if (EndPoint.X < StartPoint.X) x = StartPoint.X - size;
            if (EndPoint.Y < StartPoint.Y) y = StartPoint.Y - size;
            
            return new Rectangle(x, y, size, size);
        }
    }
} 