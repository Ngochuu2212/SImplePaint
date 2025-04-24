using System;
using System.Drawing;
using System.Collections.Generic;

namespace Paint.HinhVe
{
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
} 