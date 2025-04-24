using System;
using System.Drawing;

namespace Paint.HinhVe
{
    public class RectangleShape : Shape
    {
        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                using (Brush brush = new SolidBrush(FillColor))
                {
                    g.FillRectangle(brush, Bounds);
                    g.DrawRectangle(pen, Bounds);
                }
            }
            DrawSelectionMarkers(g);
        }
    }

    public class SquareShape : RectangleShape
    {
        public override void Scale(float scaleX, float scaleY)
        {
            float scale = Math.Max(scaleX, scaleY);
            base.Scale(scale, scale);
        }

        protected override void UpdateBounds()
        {
            int size = Math.Max(Math.Abs(EndPoint.X - StartPoint.X), Math.Abs(EndPoint.Y - StartPoint.Y));
            int x = StartPoint.X;
            int y = StartPoint.Y;
            
            if (EndPoint.X < StartPoint.X) x = StartPoint.X - size;
            if (EndPoint.Y < StartPoint.Y) y = StartPoint.Y - size;
            
            Bounds = new Rectangle(x, y, size, size);
        }
    }
} 