using System;
using System.Drawing;

namespace Paint.HinhVe
{
    public class Line : Shape
    {
        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                g.DrawLine(pen, StartPoint, EndPoint);
            }
            DrawSelectionMarkers(g);
        }

        public override bool Contains(Point p)
        {
            const int tolerance = 3;
            var distance = PointToLineDistance(p, StartPoint, EndPoint);
            return distance <= tolerance;
        }

        private float PointToLineDistance(Point p, Point lineStart, Point lineEnd)
        {
            float A = p.X - lineStart.X;
            float B = p.Y - lineStart.Y;
            float C = lineEnd.X - lineStart.X;
            float D = lineEnd.Y - lineStart.Y;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            float xx, yy;

            if (param < 0)
            {
                xx = lineStart.X;
                yy = lineStart.Y;
            }
            else if (param > 1)
            {
                xx = lineEnd.X;
                yy = lineEnd.Y;
            }
            else
            {
                xx = lineStart.X + param * C;
                yy = lineStart.Y + param * D;
            }

            float dx = p.X - xx;
            float dy = p.Y - yy;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
} 