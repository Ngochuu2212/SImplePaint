using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Paint.HinhVe
{
    public abstract class Shape
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Color BorderColor { get; set; }
        public Color FillColor { get; set; }
        public float BorderWidth { get; set; }
        public DashStyle BorderStyle { get; set; }
        public bool IsSelected { get; set; }
        public Rectangle Bounds { get; protected set; }

        protected Shape()
        {
            BorderColor = Color.Black;
            FillColor = Color.White;
            BorderWidth = 1;
            BorderStyle = DashStyle.Solid;
            IsSelected = false;
        }

        public abstract void Draw(Graphics g);

        public virtual bool Contains(Point p)
        {
            return Bounds.Contains(p);
        }

        public virtual void Move(int deltaX, int deltaY)
        {
            StartPoint = new Point(StartPoint.X + deltaX, StartPoint.Y + deltaY);
            EndPoint = new Point(EndPoint.X + deltaX, EndPoint.Y + deltaY);
            UpdateBounds();
        }

        public virtual void Scale(float scaleX, float scaleY)
        {
            EndPoint = new Point(
                StartPoint.X + (int)((EndPoint.X - StartPoint.X) * scaleX),
                StartPoint.Y + (int)((EndPoint.Y - StartPoint.Y) * scaleY)
            );
            UpdateBounds();
        }

        protected virtual void UpdateBounds()
        {
            int x = Math.Min(StartPoint.X, EndPoint.X);
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(EndPoint.X - StartPoint.X);
            int height = Math.Abs(EndPoint.Y - StartPoint.Y);
            Bounds = new Rectangle(x, y, width, height);
        }

        protected void DrawSelectionMarkers(Graphics g)
        {
            if (IsSelected)
            {
                using (Pen selectionPen = new Pen(Color.Blue, 1))
                {
                    selectionPen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(selectionPen, Bounds);

                    // Draw selection handles
                    Rectangle[] handles = GetSelectionHandles();
                    foreach (var handle in handles)
                    {
                        g.FillRectangle(Brushes.White, handle);
                        g.DrawRectangle(Pens.Blue, handle);
                    }
                }
            }
        }

        protected Rectangle[] GetSelectionHandles()
        {
            int handleSize = 6;
            return new Rectangle[]
            {
                new Rectangle(Bounds.Left - handleSize/2, Bounds.Top - handleSize/2, handleSize, handleSize),
                new Rectangle(Bounds.Right - handleSize/2, Bounds.Top - handleSize/2, handleSize, handleSize),
                new Rectangle(Bounds.Left - handleSize/2, Bounds.Bottom - handleSize/2, handleSize, handleSize),
                new Rectangle(Bounds.Right - handleSize/2, Bounds.Bottom - handleSize/2, handleSize, handleSize)
            };
        }
    }
} 