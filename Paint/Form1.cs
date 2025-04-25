using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        private Point startPoint;
        private Point endPoint;
        private bool isDrawing = false;
        private List<Shape> shapes = new List<Shape>();
        private Shape currentShape;
        private Color penColor = Color.Black;
        private Color brushColor = Color.White;
        private float penWidth = 1;
        private DashStyle penStyle = DashStyle.Solid;
        private ShapeType currentShapeType = ShapeType.Line;
        private bool isMoving = false;
        private Shape selectedShape = null;
        private Point lastPoint;

        private enum ShapeType
        {
            Line,
            Ellipse,
            Rectangle,
            Square,
            Circle,
            Curve,
            Polygon,
            BezierCurve
        }

        private abstract class Shape
        {
            public Point StartPoint { get; set; }
            public Point EndPoint { get; set; }
            public Color PenColor { get; set; }
            public Color BrushColor { get; set; }
            public float PenWidth { get; set; }
            public DashStyle PenStyle { get; set; }
            public List<Point> Points { get; set; } = new List<Point>();

            public abstract void Draw(Graphics g);
            public abstract bool Contains(Point point);
        }

        private class LineShape : Shape
        {
            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                {
                    g.DrawLine(pen, StartPoint, EndPoint);
                }
            }

            public override bool Contains(Point point)
            {
                float distance = DistanceToLine(point, StartPoint, EndPoint);
                return distance <= 5;
            }

            private float DistanceToLine(Point point, Point lineStart, Point lineEnd)
            {
                float lineLength = (float)Math.Sqrt(Math.Pow(lineEnd.X - lineStart.X, 2) + Math.Pow(lineEnd.Y - lineStart.Y, 2));
                if (lineLength == 0) return (float)Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2));

                float t = ((point.X - lineStart.X) * (lineEnd.X - lineStart.X) + (point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) / (lineLength * lineLength);
                t = Math.Max(0, Math.Min(1, t));

                float projectionX = lineStart.X + t * (lineEnd.X - lineStart.X);
                float projectionY = lineStart.Y + t * (lineEnd.Y - lineStart.Y);

                return (float)Math.Sqrt(Math.Pow(point.X - projectionX, 2) + Math.Pow(point.Y - projectionY, 2));
            }
        }

        private class RectangleShape : Shape
        {
            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                using (Brush brush = new SolidBrush(BrushColor))
                {
                    Rectangle rect = GetRectangle();
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(pen, rect);
                }
            }

            public override bool Contains(Point point)
            {
                Rectangle rect = GetRectangle();
                return rect.Contains(point);
            }

            public Rectangle GetRectangle()
            {
                int x = Math.Min(StartPoint.X, EndPoint.X);
                int y = Math.Min(StartPoint.Y, EndPoint.Y);
                int width = Math.Abs(EndPoint.X - StartPoint.X);
                int height = Math.Abs(EndPoint.Y - StartPoint.Y);
                return new Rectangle(x, y, width, height);
            }
        }

        private class EllipseShape : Shape
        {
            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                using (Brush brush = new SolidBrush(BrushColor))
                {
                    Rectangle rect = GetRectangle();
                    g.FillEllipse(brush, rect);
                    g.DrawEllipse(pen, rect);
                }
            }

            public override bool Contains(Point point)
            {
                Rectangle rect = GetRectangle();
                float a = rect.Width / 2f;
                float b = rect.Height / 2f;
                float x = point.X - (rect.X + a);
                float y = point.Y - (rect.Y + b);
                return (x * x) / (a * a) + (y * y) / (b * b) <= 1;
            }

            private Rectangle GetRectangle()
            {
                int x = Math.Min(StartPoint.X, EndPoint.X);
                int y = Math.Min(StartPoint.Y, EndPoint.Y);
                int width = Math.Abs(EndPoint.X - StartPoint.X);
                int height = Math.Abs(EndPoint.Y - StartPoint.Y);
                return new Rectangle(x, y, width, height);
            }
        }

        private class CurveShape : Shape
        {
            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                {
                    if (Points.Count > 1)
                    {
                        g.DrawCurve(pen, Points.ToArray());
                    }
                }
            }

            public override bool Contains(Point point)
            {
                for (int i = 0; i < Points.Count - 1; i++)
                {
                    float distance = DistanceToLine(point, Points[i], Points[i + 1]);
                    if (distance <= 5) return true;
                }
                return false;
            }

            private float DistanceToLine(Point point, Point lineStart, Point lineEnd)
            {
                float lineLength = (float)Math.Sqrt(Math.Pow(lineEnd.X - lineStart.X, 2) + Math.Pow(lineEnd.Y - lineStart.Y, 2));
                if (lineLength == 0) return (float)Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2));

                float t = ((point.X - lineStart.X) * (lineEnd.X - lineStart.X) + (point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) / (lineLength * lineLength);
                t = Math.Max(0, Math.Min(1, t));

                float projectionX = lineStart.X + t * (lineEnd.X - lineStart.X);
                float projectionY = lineStart.Y + t * (lineEnd.Y - lineStart.Y);

                return (float)Math.Sqrt(Math.Pow(point.X - projectionX, 2) + Math.Pow(point.Y - projectionY, 2));
            }
        }

        private class PolygonShape : Shape
        {
            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                using (Brush brush = new SolidBrush(BrushColor))
                {
                    if (Points.Count > 2)
                    {
                        g.FillPolygon(brush, Points.ToArray());
                        g.DrawPolygon(pen, Points.ToArray());
                    }
                }
            }

            public override bool Contains(Point point)
            {
                if (Points.Count < 3) return false;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(Points.ToArray());
                    return path.IsVisible(point);
                }
            }
        }

        private class BezierCurveShape : Shape
        {
            private float curvature = 0.5f; // Độ cong mặc định

            public override void Draw(Graphics g)
            {
                if (StartPoint == null || EndPoint == null) return;

                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                {
                    // Tính toán điểm điều khiển cho đường cong Bezier
                    Point controlPoint1 = new Point(
                        StartPoint.X + (int)((EndPoint.X - StartPoint.X) * 0.5f),
                        StartPoint.Y - (int)((EndPoint.Y - StartPoint.Y) * curvature)
                    );

                    Point controlPoint2 = new Point(
                        StartPoint.X + (int)((EndPoint.X - StartPoint.X) * 0.5f),
                        StartPoint.Y - (int)((EndPoint.Y - StartPoint.Y) * curvature)
                    );

                    g.DrawBezier(pen, StartPoint, controlPoint1, controlPoint2, EndPoint);
                }
            }

            public override bool Contains(Point point)
            {
                // Đơn giản hóa việc kiểm tra bằng cách sử dụng khoảng cách đến đường thẳng
                float distance = DistanceToBezier(point);
                return distance <= 5;
            }

            private float DistanceToBezier(Point point)
            {
                // Đơn giản hóa tính toán khoảng cách
                float minDistance = float.MaxValue;
                int steps = 20;

                for (int i = 0; i < steps; i++)
                {
                    float t = i / (float)(steps - 1);
                    Point bezierPoint = GetBezierPoint(t);
                    float distance = (float)Math.Sqrt(
                        Math.Pow(point.X - bezierPoint.X, 2) + 
                        Math.Pow(point.Y - bezierPoint.Y, 2)
                    );
                    minDistance = Math.Min(minDistance, distance);
                }

                return minDistance;
            }

            private Point GetBezierPoint(float t)
            {
                Point controlPoint1 = new Point(
                    StartPoint.X + (int)((EndPoint.X - StartPoint.X) * 0.5f),
                    StartPoint.Y - (int)((EndPoint.Y - StartPoint.Y) * curvature)
                );

                Point controlPoint2 = new Point(
                    StartPoint.X + (int)((EndPoint.X - StartPoint.X) * 0.5f),
                    StartPoint.Y - (int)((EndPoint.Y - StartPoint.Y) * curvature)
                );

                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;
                float uuu = uu * u;
                float ttt = tt * t;

                int x = (int)(uuu * StartPoint.X +
                    3 * uu * t * controlPoint1.X +
                    3 * u * tt * controlPoint2.X +
                    ttt * EndPoint.X);

                int y = (int)(uuu * StartPoint.Y +
                    3 * uu * t * controlPoint1.Y +
                    3 * u * tt * controlPoint2.Y +
                    ttt * EndPoint.Y);

                return new Point(x, y);
            }

            public void SetCurvature(float value)
            {
                curvature = Math.Max(0, Math.Min(1, value));
            }
        }

        private class SquareShape : Shape
        {
            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                using (Brush brush = new SolidBrush(BrushColor))
                {
                    Rectangle rect = GetSquare();
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(pen, rect);
                }
            }

            public override bool Contains(Point point)
            {
                Rectangle rect = GetSquare();
                return rect.Contains(point);
            }

            private Rectangle GetSquare()
            {
                int size = Math.Max(Math.Abs(EndPoint.X - StartPoint.X), Math.Abs(EndPoint.Y - StartPoint.Y));
                int x = StartPoint.X;
                int y = StartPoint.Y;

                if (EndPoint.X < StartPoint.X) x = StartPoint.X - size;
                if (EndPoint.Y < StartPoint.Y) y = StartPoint.Y - size;

                return new Rectangle(x, y, size, size);
            }
        }

        private class CircleShape : Shape
        {
            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(PenColor, PenWidth) { DashStyle = PenStyle })
                using (Brush brush = new SolidBrush(BrushColor))
                {
                    Rectangle rect = GetCircle();
                    g.FillEllipse(brush, rect);
                    g.DrawEllipse(pen, rect);
                }
            }

            public override bool Contains(Point point)
            {
                Rectangle rect = GetCircle();
                float centerX = rect.X + rect.Width / 2f;
                float centerY = rect.Y + rect.Height / 2f;
                float radius = rect.Width / 2f;

                float distance = (float)Math.Sqrt(
                    Math.Pow(point.X - centerX, 2) + 
                    Math.Pow(point.Y - centerY, 2)
                );

                return distance <= radius;
            }

            private Rectangle GetCircle()
            {
                int size = Math.Max(Math.Abs(EndPoint.X - StartPoint.X), Math.Abs(EndPoint.Y - StartPoint.Y));
                int x = StartPoint.X;
                int y = StartPoint.Y;

                if (EndPoint.X < StartPoint.X) x = StartPoint.X - size;
                if (EndPoint.Y < StartPoint.Y) y = StartPoint.Y - size;

                return new Rectangle(x, y, size, size);
            }
        }

        private BezierCurveShape currentBezierCurve;
        private TrackBar trackBarCurvature;

        public Form1()
        {
            InitializeComponent();
            
            // Enable double buffering for smooth drawing
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic, 
                null, panel_khungve, new object[] { true });

            // Set panel properties
            panel_khungve.BackColor = Color.White;
            panel_khungve.BorderStyle = BorderStyle.FixedSingle;

            // Create and setup curvature trackbar
            trackBarCurvature = new TrackBar();
            trackBarCurvature.Minimum = 0;
            trackBarCurvature.Maximum = 100;
            trackBarCurvature.Value = 50;
            trackBarCurvature.Location = new Point(10, panel_chucnang.Height - 40);
            trackBarCurvature.Width = 150;
            trackBarCurvature.Visible = false;
            trackBarCurvature.ValueChanged += TrackBarCurvature_ValueChanged;
            panel_chucnang.Controls.Add(trackBarCurvature);

            // Attach events to panel_khungve
            panel_khungve.MouseDown += Panel_khungve_MouseDown;
            panel_khungve.MouseMove += Panel_khungve_MouseMove;
            panel_khungve.MouseUp += Panel_khungve_MouseUp;
            panel_khungve.Paint += Panel_khungve_Paint;
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Line;
            isMoving = true;
            trackBarCurvature.Visible = false;
        }

        private void màuSắcToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnDuongThang_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Line;
            trackBarCurvature.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbKieuVe.Items.AddRange(new string[] { "Solid", "Dash", "Dot", "DashDot" });
            cmbKieuVe.SelectedIndex = 0;
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Ellipse;
            trackBarCurvature.Visible = false;
        }

        private void btnHcn_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Rectangle;
            trackBarCurvature.Visible = false;
        }

        private void btnHV_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Square;
            trackBarCurvature.Visible = false;
        }

        private void btnHinhTron_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Circle;
            trackBarCurvature.Visible = false;
        }

        private void btnDuongCong_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.BezierCurve;
            trackBarCurvature.Visible = true;
        }

        private void btnDaGiac_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Polygon;
            trackBarCurvature.Visible = false;
        }

        private void btnMauDuong_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                penColor = colorDialog.Color;
            }
        }

        private void btnMauNen_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                brushColor = colorDialog.Color;
            }
        }

        private void numDoDay_ValueChanged(object sender, EventArgs e)
        {
            penWidth = (float)numDoDay.Value;
        }

        private void cmbKieuVe_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbKieuVe.SelectedIndex)
            {
                case 0:
                    penStyle = DashStyle.Solid;
                    break;
                case 1:
                    penStyle = DashStyle.Dash;
                    break;
                case 2:
                    penStyle = DashStyle.Dot;
                    break;
                case 3:
                    penStyle = DashStyle.DashDot;
                    break;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel_chucnang_Paint(object sender, PaintEventArgs e)
        {
            // Không cần xử lý gì trong sự kiện này
        }

        private void MofileMoi_Click(object sender, EventArgs e)
        {
            if (shapes.Count > 0)
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có chắc muốn tạo bản vẽ mới không?\nCác hình vẽ hiện tại sẽ bị xóa.",
                    "Xác nhận tạo mới",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = MessageBox.Show(
                        "Bạn có muốn lưu bản vẽ hiện tại không?",
                        "Lưu bản vẽ",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (saveResult == DialogResult.Yes)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Bitmap bmp = new Bitmap(panel_khungve.Width, panel_khungve.Height);
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.Clear(panel_khungve.BackColor);
                                foreach (Shape shape in shapes)
                                {
                                    shape.Draw(g);
                                }
                            }
                            bmp.Save(saveFileDialog.FileName);
                        }
                    }
                    
                    // Xóa tất cả các hình sau khi đã xử lý việc lưu
                    shapes.Clear();
                    panel_khungve.Invalidate();
                }
                // Nếu người dùng chọn No, không làm gì cả, giữ nguyên bản vẽ
            }
            else
            {
                // Nếu không có hình vẽ nào, không cần hỏi
                shapes.Clear();
                panel_khungve.Invalidate();
            }
        }

        private void Luufiledave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(panel_khungve.Width, panel_khungve.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(panel_khungve.BackColor);
                    foreach (Shape shape in shapes)
                    {
                        shape.Draw(g);
                    }
                }
                bmp.Save(saveFileDialog.FileName);
            }
        }

        private void Chonvungdave_Click(object sender, EventArgs e)
        {

        }

        private void NhomCacHinh_Click(object sender, EventArgs e)
        {

        }

        private Point ClampToPanel(Point point)
        {
            return new Point(
                Math.Max(0, Math.Min(point.X, panel_khungve.Width)),
                Math.Max(0, Math.Min(point.Y, panel_khungve.Height))
            );
        }

        private void Panel_khungve_MouseDown(object sender, MouseEventArgs e)
        {
            if (!panel_khungve.ClientRectangle.Contains(e.Location)) return;

            if (e.Button == MouseButtons.Left)
            {
                if (isMoving)
                {
                    selectedShape = null;
                    foreach (Shape shape in shapes.Reverse<Shape>())
                    {
                        if (shape.Contains(e.Location))
                        {
                            selectedShape = shape;
                            lastPoint = e.Location;
                            break;
                        }
                    }
                }
                else if (currentShapeType == ShapeType.Curve || currentShapeType == ShapeType.Polygon)
                {
                    if (!isDrawing)
                    {
                        isDrawing = true;
                        startPoint = ClampToPanel(e.Location);
                        CreateNewShape();
                        currentShape.Points.Add(startPoint);
                    }
                    else
                    {
                        Point newPoint = ClampToPanel(e.Location);
                        currentShape.Points.Add(newPoint);
                        if (currentShapeType == ShapeType.Curve)
                        {
                            isDrawing = false;
                            shapes.Add(currentShape);
                            currentShape = null;
                        }
                    }
                    panel_khungve.Invalidate();
                }
                else
                {
                    isDrawing = true;
                    startPoint = ClampToPanel(e.Location);
                    CreateNewShape();
                }
            }
            else if (e.Button == MouseButtons.Right && currentShapeType == ShapeType.Polygon)
            {
                if (isDrawing && currentShape.Points.Count >= 3)
                {
                    isDrawing = false;
                    shapes.Add(currentShape);
                    currentShape = null;
                    panel_khungve.Invalidate();
                }
            }
        }

        private void Panel_khungve_MouseMove(object sender, MouseEventArgs e)
        {
            Point clampedLocation = ClampToPanel(e.Location);

            if (isMoving && selectedShape != null && e.Button == MouseButtons.Left)
            {
                int dx = clampedLocation.X - lastPoint.X;
                int dy = clampedLocation.Y - lastPoint.Y;

                // Kiểm tra xem việc di chuyển có đưa hình ra ngoài panel không
                Rectangle bounds = GetShapeBounds(selectedShape);
                bounds.Offset(dx, dy);

                if (panel_khungve.ClientRectangle.Contains(bounds))
                {
                    selectedShape.StartPoint = new Point(selectedShape.StartPoint.X + dx, selectedShape.StartPoint.Y + dy);
                    selectedShape.EndPoint = new Point(selectedShape.EndPoint.X + dx, selectedShape.EndPoint.Y + dy);

                    for (int i = 0; i < selectedShape.Points.Count; i++)
                    {
                        selectedShape.Points[i] = new Point(selectedShape.Points[i].X + dx, selectedShape.Points[i].Y + dy);
                    }

                    lastPoint = clampedLocation;
                    panel_khungve.Invalidate();
                }
            }
            else if (isDrawing)
            {
                endPoint = clampedLocation;
                if (currentShape != null)
                {
                    if (currentShapeType != ShapeType.Curve && currentShapeType != ShapeType.Polygon)
                    {
                        currentShape.EndPoint = endPoint;
                    }
                }
                panel_khungve.Invalidate();
            }
        }

        private Rectangle GetShapeBounds(Shape shape)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            // Kiểm tra điểm đầu và cuối
            minX = Math.Min(minX, Math.Min(shape.StartPoint.X, shape.EndPoint.X));
            minY = Math.Min(minY, Math.Min(shape.StartPoint.Y, shape.EndPoint.Y));
            maxX = Math.Max(maxX, Math.Max(shape.StartPoint.X, shape.EndPoint.X));
            maxY = Math.Max(maxY, Math.Max(shape.StartPoint.Y, shape.EndPoint.Y));

            // Kiểm tra các điểm trong danh sách Points
            foreach (Point p in shape.Points)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private void Panel_khungve_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                selectedShape = null;
            }
            else if (isDrawing && e.Button == MouseButtons.Left)
            {
                if (currentShapeType != ShapeType.Curve && currentShapeType != ShapeType.Polygon)
                {
                    isDrawing = false;
                    endPoint = ClampToPanel(e.Location);
                    currentShape.EndPoint = endPoint;

                    // Kiểm tra xem hình có nằm trong panel không
                    Rectangle bounds = GetShapeBounds(currentShape);
                    if (panel_khungve.ClientRectangle.Contains(bounds))
                    {
                        shapes.Add(currentShape);
                    }
                    
                    currentShape = null;
                    panel_khungve.Invalidate();
                }
            }
        }

        private void Panel_khungve_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            foreach (Shape shape in shapes)
            {
                shape.Draw(e.Graphics);
            }
            if (currentShape != null)
            {
                currentShape.Draw(e.Graphics);
            }
            if (selectedShape != null)
            {
                using (Pen pen = new Pen(Color.Red, 2) { DashStyle = DashStyle.Dash })
                {
                    if (selectedShape is RectangleShape || selectedShape is EllipseShape)
                    {
                        Rectangle rect = ((RectangleShape)selectedShape).GetRectangle();
                        e.Graphics.DrawRectangle(pen, rect);
                    }
                    else if (selectedShape is LineShape)
                    {
                        e.Graphics.DrawLine(pen, selectedShape.StartPoint, selectedShape.EndPoint);
                    }
                    else if (selectedShape is CurveShape && selectedShape.Points.Count > 1)
                    {
                        e.Graphics.DrawCurve(pen, selectedShape.Points.ToArray());
                    }
                    else if (selectedShape is PolygonShape && selectedShape.Points.Count > 2)
                    {
                        e.Graphics.DrawPolygon(pen, selectedShape.Points.ToArray());
                    }
                }
            }
        }

        private void TrackBarCurvature_ValueChanged(object sender, EventArgs e)
        {
            if (currentShape is BezierCurveShape bezierCurve)
            {
                bezierCurve.SetCurvature(trackBarCurvature.Value / 100f);
                panel_khungve.Invalidate();
            }
        }

        private void CreateNewShape()
        {
            switch (currentShapeType)
            {
                case ShapeType.Line:
                    currentShape = new LineShape();
                    break;
                case ShapeType.Rectangle:
                    currentShape = new RectangleShape();
                    break;
                case ShapeType.Ellipse:
                    currentShape = new EllipseShape();
                    break;
                case ShapeType.Square:
                    currentShape = new SquareShape();
                    break;
                case ShapeType.Circle:
                    currentShape = new CircleShape();
                    break;
                case ShapeType.BezierCurve:
                    currentShape = new BezierCurveShape();
                    currentBezierCurve = (BezierCurveShape)currentShape;
                    currentBezierCurve.SetCurvature(trackBarCurvature.Value / 100f);
                    break;
                case ShapeType.Curve:
                    currentShape = new CurveShape();
                    break;
                case ShapeType.Polygon:
                    currentShape = new PolygonShape();
                    break;
            }

            if (currentShape != null)
            {
                currentShape.StartPoint = startPoint;
                currentShape.EndPoint = startPoint;
                currentShape.PenColor = penColor;
                currentShape.BrushColor = brushColor;
                currentShape.PenWidth = penWidth;
                currentShape.PenStyle = penStyle;
            }
        }
    }
}
