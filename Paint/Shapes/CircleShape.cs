using System;
using System.Drawing;

namespace Paint.Shapes
{
    public class CircleShape : EllipseShape
    {
        public override bool Contains(Point p)
        {
            Rectangle rect = GetRectangle();
            float centerX = rect.X + rect.Width / 2f;
            float centerY = rect.Y + rect.Height / 2f;
            float radius = rect.Width / 2f;

            // Tính khoảng cách từ điểm đến tâm
            float distance = (float)Math.Sqrt(
                (p.X - centerX) * (p.X - centerX) + 
                (p.Y - centerY) * (p.Y - centerY)
            );

            // Cho phép dung sai 3 pixel
            return Math.Abs(distance - radius) <= 3;
        }

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