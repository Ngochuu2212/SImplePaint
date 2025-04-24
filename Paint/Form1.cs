using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
//using Paint.Shapes;
//using Paint.HinhVe;

namespace Paint
{
    public partial class Form1 : Form
    {
        private List<Shape> shapes = new List<Shape>();
        private Shape currentShape;
        private bool isDrawing = false;
        private Point lastPoint;
        private DrawingMode currentMode = DrawingMode.Line;
        private List<Shape> selectedShapes = new List<Shape>();
        private bool isMoving = false;
        private Point moveStartPoint;
        private bool isCtrlPressed = false;
        private bool isZoomingPoint = false;
        private Point zoomStartPoint;
        private Bitmap bufferBitmap;
        private Graphics bufferGraphics;
        private bool isDrawingCurve = false;
        private int curveStep = 0;

        private enum DrawingMode
        {
            Line,
            Ellipse,
            Rectangle,
            Square,
            Circle,
            Curve,
            Polygon
        }

        public Form1()
        {
            InitializeComponent();
            SetupDrawingPanel();
            cmbKieuVe.SelectedIndex = 0;
            numDoDay.Value = 1;

            // Cố định kích thước form
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Size = new Size(1406, 688); // Đặt kích thước cố định cho form
        }

        private void SetupDrawingPanel()
        {
            panel_khungve.BackColor = Color.White;
            panel_khungve.Paint += Panel_khungve_Paint;
            panel_khungve.MouseDown += Panel_khungve_MouseDown;
            panel_khungve.MouseMove += Panel_khungve_MouseMove;
            panel_khungve.MouseUp += Panel_khungve_MouseUp;
            panel_khungve.Resize += Panel_khungve_Resize;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            // Enable double buffering for the form
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            
            // Enable double buffering for the panel
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panel_khungve, new object[] { true });
        }

        private void Panel_khungve_Resize(object sender, EventArgs e)
        {
            if (bufferBitmap != null)
            {
                bufferBitmap.Dispose();
                bufferGraphics.Dispose();
            }
            bufferBitmap = new Bitmap(panel_khungve.Width, panel_khungve.Height);
            bufferGraphics = Graphics.FromImage(bufferBitmap);
            bufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void Panel_khungve_Paint(object sender, PaintEventArgs e)
        {
            if (bufferBitmap == null)
            {
                bufferBitmap = new Bitmap(panel_khungve.Width, panel_khungve.Height);
                bufferGraphics = Graphics.FromImage(bufferBitmap);
                bufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            // Clear the buffer
            bufferGraphics.Clear(panel_khungve.BackColor);

            // Draw all shapes to the buffer
            foreach (var shape in shapes)
            {
                shape.Draw(bufferGraphics);
            }
            if (currentShape != null)
            {
                currentShape.Draw(bufferGraphics);
            }

            // Draw the buffer to the screen
            e.Graphics.DrawImage(bufferBitmap, 0, 0);
        }

        private void Panel_khungve_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = e.Location;
                bool hitShape = false;

                // Tìm hình được click
                foreach (var shape in shapes.AsEnumerable().Reverse())
                {
                    if (shape.Contains(e.Location))
                    {
                        hitShape = true;

                        if (!isCtrlPressed)
                        {
                            // Nếu không nhấn Ctrl, bỏ chọn tất cả và chỉ chọn hình hiện tại
                            selectedShapes.ForEach(s => s.IsSelected = false);
                            selectedShapes.Clear();
                            shape.IsSelected = true;
                            selectedShapes.Add(shape);

                            // Xử lý các điểm điều khiển của đa giác và đường cong
                            if (shape is Polygon polygon)
                            {
                                polygon.SelectPoint(e.Location);
                                if (polygon.SelectedPoint >= 0)
                                {
                                    isZoomingPoint = true;
                                    zoomStartPoint = e.Location;
                                    break;
                                }
                            }
                            else if (shape is CurvedLine curve)
                            {
                                curve.SelectPoint(e.Location);
                                if (curve.SelectedPoint >= 0)
                                {
                                    isZoomingPoint = true;
                                    zoomStartPoint = e.Location;
                                    break;
                                }
                            }

                            isMoving = true;
                            moveStartPoint = e.Location;
                        }
                        else
                        {
                            // Khi nhấn Ctrl
                            if (selectedShapes.Contains(shape))
                            {
                                // Nếu hình đã được chọn, bỏ chọn nó
                                shape.IsSelected = false;
                                selectedShapes.Remove(shape);
                            }
                            else
                            {
                                // Nếu hình chưa được chọn, thêm vào danh sách chọn
                                shape.IsSelected = true;
                                selectedShapes.Add(shape);
                            }
                        }
                        break;
                    }
                }

                if (!hitShape && !isCtrlPressed)
                {
                    // Click vào vùng trống và không nhấn Ctrl
                    selectedShapes.ForEach(s => s.IsSelected = false);
                    selectedShapes.Clear();

                    if (currentMode == DrawingMode.Curve)
                    {
                        isDrawing = true;
                        isDrawingCurve = true;
                        CreateNewShape(e.Location);
                        (currentShape as CurvedLine)?.AddPoint(e.Location);
                    }
                    else
                    {
                        isDrawing = true;
                        CreateNewShape(e.Location);
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (isDrawingCurve && currentShape != null)
                {
                    shapes.Add(currentShape);
                    currentShape = null;
                    isDrawingCurve = false;
                    isDrawing = false;
                }
            }

            panel_khungve.Invalidate();
        }

        private void Panel_khungve_MouseMove(object sender, MouseEventArgs e)
        {
            // Kiểm tra giới hạn vùng vẽ
            if (e.X < 0 || e.X > panel_khungve.Width || 
                e.Y < 0 || e.Y > panel_khungve.Height)
            {
                return;
            }

            if (isDrawing && currentShape != null)
            {
                if (currentMode == DrawingMode.Curve && isDrawingCurve)
                {
                    if (ModifierKeys == Keys.Control)
                    {
                        (currentShape as CurvedLine)?.UpdateControlPoint(e.Location);
                    }
                    else
                    {
                        (currentShape as CurvedLine)?.UpdateEndPoint(e.Location);
                    }
                }
                else if (currentMode == DrawingMode.Polygon)
                {
                    // Kiểm tra giới hạn khi vẽ đa giác
                    if (e.X >= 0 && e.X <= panel_khungve.Width && 
                        e.Y >= 0 && e.Y <= panel_khungve.Height)
                    {
                        currentShape.EndPoint = e.Location;
                    }
                }
                else
                {
                    // Kiểm tra giới hạn khi vẽ các hình khác
                    if (e.X >= 0 && e.X <= panel_khungve.Width && 
                        e.Y >= 0 && e.Y <= panel_khungve.Height)
                    {
                        currentShape.EndPoint = e.Location;
                    }
                }
                panel_khungve.Invalidate();
            }
            else if (isMoving && selectedShapes.Count > 0)
            {
                // Kiểm tra giới hạn khi di chuyển
                int deltaX = e.X - moveStartPoint.X;
                int deltaY = e.Y - moveStartPoint.Y;
                
                foreach (var shape in selectedShapes)
                {
                    // Kiểm tra xem hình có vượt quá giới hạn không
                    Rectangle newBounds = shape.Bounds;
                    newBounds.Offset(deltaX, deltaY);
                    
                    if (newBounds.Left >= 0 && newBounds.Right <= panel_khungve.Width &&
                        newBounds.Top >= 0 && newBounds.Bottom <= panel_khungve.Height)
                    {
                        shape.Move(deltaX, deltaY);
                    }
                }
                moveStartPoint = e.Location;
                panel_khungve.Invalidate();
            }
            else if (isZoomingPoint && selectedShapes.Count > 0)
            {
                // Kiểm tra giới hạn khi phóng to/thu nhỏ
                foreach (var shape in selectedShapes)
                {
                    if (shape is Polygon polygon)
                    {
                        Rectangle newBounds = polygon.Bounds;
                        if (newBounds.Left >= 0 && newBounds.Right <= panel_khungve.Width &&
                            newBounds.Top >= 0 && newBounds.Bottom <= panel_khungve.Height)
                        {
                            polygon.ZoomPoint(zoomStartPoint, e.Location);
                        }
                    }
                    else if (shape is CurvedLine curve)
                    {
                        Rectangle newBounds = curve.Bounds;
                        if (newBounds.Left >= 0 && newBounds.Right <= panel_khungve.Width &&
                            newBounds.Top >= 0 && newBounds.Bottom <= panel_khungve.Height)
                        {
                            curve.ZoomPoint(zoomStartPoint, e.Location);
                        }
                    }
                }
                zoomStartPoint = e.Location;
                panel_khungve.Invalidate();
            }
        }

        private void Panel_khungve_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            if (isDrawing)
            {
                if (currentMode == DrawingMode.Polygon)
                {
                    if (currentShape == null)
                    {
                        CreateNewShape(e.Location);
                    }
                    (currentShape as Polygon)?.AddPoint(e.Location);
                }
                else if (currentMode != DrawingMode.Curve)
                {
                    if (currentShape != null)
                    {
                        shapes.Add(currentShape);
                        currentShape = null;
                    }
                    isDrawing = false;
                }
            }
            isMoving = false;
            isZoomingPoint = false;
            panel_khungve.Invalidate();
        }

        private void CreateNewShape(Point location)
        {
            switch (currentMode)
            {
                case DrawingMode.Line:
                    currentShape = new Line();
                    break;
                case DrawingMode.Ellipse:
                    currentShape = new EllipseShape();
                    break;
                case DrawingMode.Rectangle:
                    currentShape = new RectangleShape();
                    break;
                case DrawingMode.Square:
                    currentShape = new SquareShape();
                    break;
                case DrawingMode.Circle:
                    currentShape = new CircleShape();
                    break;
                case DrawingMode.Curve:
                    currentShape = new CurvedLine();
                    break;
                case DrawingMode.Polygon:
                    if (currentShape == null)
                    {
                        currentShape = new Polygon();
                        shapes.Add(currentShape);
                    }
                    break;
            }

            if (currentShape != null)
            {
                currentShape.StartPoint = location;
                currentShape.EndPoint = location;
                currentShape.BorderColor = btnMauDuong.BackColor;
                currentShape.FillColor = btnMauNen.BackColor;
                currentShape.BorderWidth = (float)numDoDay.Value;
                currentShape.BorderStyle = cmbKieuVe.SelectedIndex == 0 ? DashStyle.Solid : DashStyle.Dash;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnMauDuong.BackColor = Color.Black;
            btnMauNen.BackColor = Color.White;
        }

        private void btnDuongThang_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Line;
            currentShape = null;
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Ellipse;
            currentShape = null;
        }

        private void btnHcn_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Rectangle;
            currentShape = null;
        }

        private void btnHV_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Square;
            currentShape = null;
        }

        private void btnHinhTron_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Circle;
            currentShape = null;
        }

        private void btnDuongCong_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Curve;
            currentShape = null;
        }

        private void btnDaGiac_Click(object sender, EventArgs e)
        {
            currentMode = DrawingMode.Polygon;
            currentShape = null;
        }

        private void btnMauDuong_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                btnMauDuong.BackColor = colorDialog.Color;
                foreach (var shape in selectedShapes)
                {
                    shape.BorderColor = colorDialog.Color;
                }
                panel_khungve.Invalidate();
            }
        }

        private void btnMauNen_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                btnMauNen.BackColor = colorDialog.Color;
                foreach (var shape in selectedShapes)
                {
                    shape.FillColor = colorDialog.Color;
                }
                panel_khungve.Invalidate();
            }
        }

        private void numDoDay_ValueChanged(object sender, EventArgs e)
        {
            foreach (var shape in selectedShapes)
            {
                shape.BorderWidth = (float)numDoDay.Value;
            }
            panel_khungve.Invalidate();
        }

        private void cmbKieuVe_SelectedIndexChanged(object sender, EventArgs e)
        {
            DashStyle style = cmbKieuVe.SelectedIndex == 0 ? DashStyle.Solid : DashStyle.Dash;
            foreach (var shape in selectedShapes)
            {
                shape.BorderStyle = style;
            }
            panel_khungve.Invalidate();
        }

        private void MofileMoi_Click(object sender, EventArgs e)
        {
            shapes.Clear();
            selectedShapes.Clear();
            currentShape = null;
            panel_khungve.Invalidate();
        }

        private void Luufiledave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                saveDialog.Title = "Save drawing";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (Bitmap bmp = new Bitmap(panel_khungve.Width, panel_khungve.Height))
                    {
                        panel_khungve.DrawToBitmap(bmp, new Rectangle(0, 0, panel_khungve.Width, panel_khungve.Height));
                        bmp.Save(saveDialog.FileName);
                    }
                }
            }
        }

        private void Chonvungdave_Click(object sender, EventArgs e)
        {
            // Implement selection mode if needed
        }

        private void NhomCacHinh_Click(object sender, EventArgs e)
        {
            // Implement grouping if needed
        }

        private void GoCacHinh_Click(object sender, EventArgs e)
        {
            selectedShapes.Clear();
            foreach (var shape in shapes)
            {
                shape.IsSelected = false;
            }
            panel_khungve.Invalidate();
        }

        private void PhongToHinh_Click(object sender, EventArgs e)
        {
            if (selectedShapes.Count > 0)
            {
                foreach (var shape in selectedShapes)
                {
                    shape.Scale(1.2f, 1.2f);
                }
                panel_khungve.Invalidate();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // No implementation needed
        }

        private void panel_chucnang_Paint(object sender, PaintEventArgs e)
        {
            // No implementation needed
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                isCtrlPressed = true;
            }

            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedShapes();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                isCtrlPressed = false;
            }
        }

        private void DeleteSelectedShapes()
        {
            if (selectedShapes.Count > 0)
            {
                var shapesToDelete = new List<Shape>(selectedShapes);
                foreach (var shape in shapesToDelete)
                {
                    shapes.Remove(shape);
                }
                selectedShapes.Clear();
                panel_khungve.Invalidate();
            }
        }

        private void XoaMenu_Click(object sender, EventArgs e)
        {
            DeleteSelectedShapes();
        }
    }

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

        protected override Rectangle GetRectangle()
        {
            int size = Math.Max(Math.Abs(EndPoint.X - StartPoint.X), Math.Abs(EndPoint.Y - StartPoint.Y));
            int x = StartPoint.X;
            int y = StartPoint.Y;
            
            if (EndPoint.X < StartPoint.X) x = StartPoint.X - size;
            if (EndPoint.Y < StartPoint.Y) y = StartPoint.Y - size;
            
            return new Rectangle(x, y, size, size);
        }

        public override bool Contains(Point p)
        {
            Rectangle rect = GetRectangle();
            // Kiểm tra điểm có nằm gần viền hình vuông không
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
    }

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

        protected override Rectangle GetRectangle()
        {
            int size = Math.Max(Math.Abs(EndPoint.X - StartPoint.X), Math.Abs(EndPoint.Y - StartPoint.Y));
            int x = StartPoint.X;
            int y = StartPoint.Y;
            
            if (EndPoint.X < StartPoint.X) x = StartPoint.X - size;
            if (EndPoint.Y < StartPoint.Y) y = StartPoint.Y - size;
            
            return new Rectangle(x, y, size, size);
        }

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
    }

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
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw the curve using Bezier
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
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

            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
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

