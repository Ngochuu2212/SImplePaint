using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Paint
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

    public class EllipseShape : Shape
    {
        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                using (Brush brush = new SolidBrush(FillColor))
                {
                    g.FillEllipse(brush, Bounds);
                    g.DrawEllipse(pen, Bounds);
                }
            }
            DrawSelectionMarkers(g);
        }

        public override bool Contains(Point p)
        {
            float a = Bounds.Width / 2f;
            float b = Bounds.Height / 2f;
            float centerX = Bounds.X + a;
            float centerY = Bounds.Y + b;

            return Math.Pow((p.X - centerX) / a, 2) + Math.Pow((p.Y - centerY) / b, 2) <= 1;
        }
    }

    public class CircleShape : EllipseShape
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

    public class CurvedLine : Shape
    {
        private List<Point> points = new List<Point>();

        public void AddPoint(Point p)
        {
            points.Add(p);
            UpdateBounds();
        }

        public override void Draw(Graphics g)
        {
            if (points.Count < 2) return;

            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                g.DrawCurve(pen, points.ToArray());
            }
            DrawSelectionMarkers(g);
        }

        protected override void UpdateBounds()
        {
            if (points.Count == 0) return;

            int minX = points[0].X;
            int minY = points[0].Y;
            int maxX = points[0].X;
            int maxY = points[0].Y;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            Bounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public override void Move(int deltaX, int deltaY)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(points[i].X + deltaX, points[i].Y + deltaY);
            }
            UpdateBounds();
        }

        public override void Scale(float scaleX, float scaleY)
        {
            Point center = new Point(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            for (int i = 0; i < points.Count; i++)
            {
                int dx = points[i].X - center.X;
                int dy = points[i].Y - center.Y;
                points[i] = new Point(
                    center.X + (int)(dx * scaleX),
                    center.Y + (int)(dy * scaleY)
                );
            }
            UpdateBounds();
        }
    }

    public class Polygon : Shape
    {
        private List<Point> points = new List<Point>();

        public void AddPoint(Point p)
        {
            points.Add(p);
            UpdateBounds();
        }

        public override void Draw(Graphics g)
        {
            if (points.Count < 3) return;

            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                using (Brush brush = new SolidBrush(FillColor))
                {
                    g.FillPolygon(brush, points.ToArray());
                    g.DrawPolygon(pen, points.ToArray());
                }
            }
            DrawSelectionMarkers(g);
        }

        protected override void UpdateBounds()
        {
            if (points.Count == 0) return;

            int minX = points[0].X;
            int minY = points[0].Y;
            int maxX = points[0].X;
            int maxY = points[0].Y;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            Bounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public override void Move(int deltaX, int deltaY)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(points[i].X + deltaX, points[i].Y + deltaY);
            }
            UpdateBounds();
        }

        public override void Scale(float scaleX, float scaleY)
        {
            Point center = new Point(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            for (int i = 0; i < points.Count; i++)
            {
                int dx = points[i].X - center.X;
                int dy = points[i].Y - center.Y;
                points[i] = new Point(
                    center.X + (int)(dx * scaleX),
                    center.Y + (int)(dy * scaleY)
                );
            }
            UpdateBounds();
        }
    }
} 