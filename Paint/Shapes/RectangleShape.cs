using System;
using System.Drawing;

namespace Paint.Shapes
{
    public class RectangleShape : Shape
    {
        public override void Draw(Graphics g)
        {
            Rectangle rect = GetRectangle();
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                using (Brush brush = new SolidBrush(FillColor))
                {
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(pen, rect);
                }
            }
            DrawSelectionMarkers(g);
        }

        public override bool Contains(Point p)
        {
            Rectangle rect = GetRectangle();
            // Kiểm tra điểm có nằm gần viền hình chữ nhật không
            if (p.X >= rect.Left - 3 && p.X <= rect.Right + 3 &&
                p.Y >= rect.Top - 3 && p.Y <= rect.Bottom + 3)
            {
                // Kiểm tra xem điểm có nằm gần cạnh không
                if (p.X <= rect.Left + 3 || p.X >= rect.Right - 3 ||
                    p.Y <= rect.Top + 3 || p.Y >= rect.Bottom - 3)
                {
                    return true;
                }
            }
            return rect.Contains(p);
        }

        protected virtual Rectangle GetRectangle()
        {
            int x = Math.Min(StartPoint.X, EndPoint.X);
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(EndPoint.X - StartPoint.X);
            int height = Math.Abs(EndPoint.Y - StartPoint.Y);
            return new Rectangle(x, y, width, height);
        }

        protected override void UpdateBounds()
        {
            Bounds = GetRectangle();
        }
    }
} 