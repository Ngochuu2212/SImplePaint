using System;
using System.Drawing;

namespace Paint.Shapes
{
    public class EllipseShape : Shape
    {
        public override void Draw(Graphics g)
        {
            Rectangle rect = GetRectangle();
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                using (Brush brush = new SolidBrush(FillColor))
                {
                    g.FillEllipse(brush, rect);
                    g.DrawEllipse(pen, rect);
                }
            }
            DrawSelectionMarkers(g);
        }

        public override bool Contains(Point p)
        {
            Rectangle rect = GetRectangle();
            // Chuyển điểm về tọa độ tương đối với tâm ellipse
            float centerX = rect.X + rect.Width / 2f;
            float centerY = rect.Y + rect.Height / 2f;
            float a = rect.Width / 2f;
            float b = rect.Height / 2f;

            if (a == 0 || b == 0) return false;

            // Tính khoảng cách tương đối đến ellipse
            float dx = (p.X - centerX) / a;
            float dy = (p.Y - centerY) / b;
            float distance = dx * dx + dy * dy;

            // Cho phép dung sai 0.1 để dễ chọn
            return Math.Abs(distance - 1) <= 0.1;
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