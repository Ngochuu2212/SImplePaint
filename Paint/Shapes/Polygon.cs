using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Paint.Shapes
{
    public class Polygon : Shape
    {
        private List<Point> points = new List<Point>();
        private int selectedPoint = -1;

        public int SelectedPoint { get => selectedPoint; set => selectedPoint = value; }

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
                using (Brush brush = new SolidBrush(FillColor))
                {
                    if (points.Count > 2)
                    {
                        g.FillPolygon(brush, points.ToArray());
                        g.DrawPolygon(pen, points.ToArray());
                    }
                    else
                    {
                        g.DrawLine(pen, points[0], points[1]);
                    }
                }
            }

            if (IsSelected)
            {
                using (var selectionBrush = new SolidBrush(Color.Blue))
                {
                    foreach (var point in points)
                    {
                        g.FillRectangle(selectionBrush, new Rectangle(point.X - 3, point.Y - 3, 6, 6));
                    }
                }
            }
            DrawSelectionMarkers(g);
        }

        public void SelectPoint(Point location)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if ((points[i].X - 10 <= location.X && location.X <= points[i].X + 10)
                    && (points[i].Y - 10 <= location.Y && location.Y <= points[i].Y + 10))
                {
                    selectedPoint = i;
                    return;
                }
            }
            selectedPoint = -1;
        }

        public void ZoomPoint(Point firstPoint, Point currentPoint)
        {
            if (selectedPoint < 0) return;
            int dx = currentPoint.X - firstPoint.X;
            int dy = currentPoint.Y - firstPoint.Y;
            
            for (int i = selectedPoint - 1; i <= selectedPoint + 1; i++)
            {
                if (i < 0 || i >= points.Count) continue;
                points[i] = new Point(points[i].X + dx, points[i].Y + dy);
            }
            UpdateBounds();
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