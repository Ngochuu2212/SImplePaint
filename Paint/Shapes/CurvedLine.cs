using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Paint.Shapes
{
    public class CurvedLine : Shape
    {
        private List<Point> points;
        private int selectedPoint = -1;
        private Point controlPoint;

        public int SelectedPoint { get => selectedPoint; set => selectedPoint = value; }

        public CurvedLine()
        {
            points = new List<Point>();
        }

        public void AddPoint(Point p)
        {
            if (points.Count == 0)
            {
                points.Add(p); // Start point
                points.Add(p); // End point (initially same as start)
                controlPoint = p;
            }
            UpdateBounds();
        }

        public void UpdateEndPoint(Point p)
        {
            if (points.Count == 2)
            {
                points[1] = p; // Update end point
                // Automatically calculate control point for a nice curve
                controlPoint = new Point(
                    (points[0].X + points[1].X) / 2,
                    Math.Min(points[0].Y, points[1].Y) - Math.Abs(points[1].X - points[0].X) / 3
                );
                UpdateBounds();
            }
        }

        public void UpdateControlPoint(Point p)
        {
            if (points.Count == 2)
            {
                controlPoint = p;
                UpdateBounds();
            }
        }

        public override void Draw(Graphics g)
        {
            if (points.Count < 2) return;

            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.DashStyle = BorderStyle;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw the curve using Bezier
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddBezier(
                        points[0],
                        controlPoint,
                        controlPoint,
                        points[1]
                    );
                    g.DrawPath(pen, path);
                }
            }

            if (IsSelected)
            {
                using (var brush = new SolidBrush(Color.Blue))
                {
                    // Draw control points
                    g.FillRectangle(brush, new Rectangle(points[0].X - 4, points[0].Y - 4, 8, 8));
                    g.FillRectangle(brush, new Rectangle(points[1].X - 4, points[1].Y - 4, 8, 8));
                    g.FillRectangle(brush, new Rectangle(controlPoint.X - 4, controlPoint.Y - 4, 8, 8));

                    // Draw control lines when selected
                    using (Pen controlPen = new Pen(Color.Blue, 1))
                    {
                        controlPen.DashStyle = DashStyle.Dash;
                        g.DrawLine(controlPen, points[0], controlPoint);
                        g.DrawLine(controlPen, points[1], controlPoint);
                    }
                }
            }
        }

        public override bool Contains(Point p)
        {
            if (points.Count < 2) return false;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddBezier(points[0], controlPoint, controlPoint, points[1]);
                return path.IsOutlineVisible(p, new Pen(Color.Black, 10));
            }
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

            // Include control point in bounds
            minX = Math.Min(minX, controlPoint.X);
            minY = Math.Min(minY, controlPoint.Y);
            maxX = Math.Max(maxX, controlPoint.X);
            maxY = Math.Max(maxY, controlPoint.Y);

            Bounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public override void Move(int deltaX, int deltaY)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(points[i].X + deltaX, points[i].Y + deltaY);
            }
            controlPoint = new Point(controlPoint.X + deltaX, controlPoint.Y + deltaY);
            UpdateBounds();
        }

        public void SelectPoint(Point location)
        {
            // Check end points
            for (int i = 0; i < points.Count; i++)
            {
                if ((points[i].X - 10 <= location.X && location.X <= points[i].X + 10)
                    && (points[i].Y - 10 <= location.Y && location.Y <= points[i].Y + 10))
                {
                    selectedPoint = i;
                    return;
                }
            }

            // Check control point
            if ((controlPoint.X - 10 <= location.X && location.X <= controlPoint.X + 10)
                && (controlPoint.Y - 10 <= location.Y && location.Y <= controlPoint.Y + 10))
            {
                selectedPoint = 2; // Control point
                return;
            }

            selectedPoint = -1;
        }

        public void ZoomPoint(Point firstPoint, Point currentPoint)
        {
            if (selectedPoint < 0) return;

            int dx = currentPoint.X - firstPoint.X;
            int dy = currentPoint.Y - firstPoint.Y;

            if (selectedPoint == 2) // Control point
            {
                controlPoint = new Point(controlPoint.X + dx, controlPoint.Y + dy);
            }
            else if (selectedPoint < points.Count)
            {
                points[selectedPoint] = new Point(points[selectedPoint].X + dx, points[selectedPoint].Y + dy);
            }
            UpdateBounds();
        }
    }
} 