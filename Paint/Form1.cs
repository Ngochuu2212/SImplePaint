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
        private List<Group> groups = new List<Group>();
        private Shape currentShape;
        private Color penColor = Color.Black;
        private Color brushColor = Color.White;
        private float penWidth = 1;
        private DashStyle penStyle = DashStyle.Solid;
        private ShapeType currentShapeType = ShapeType.Line;
        private bool isMoving = false;
        private Shape selectedShape = null;
        private Point lastPoint;
        private List<Shape> selectedShapes = new List<Shape>();
        private bool isSelecting = false;
        private Rectangle selectionRect;
        private Point selectionStart;
        private bool isGrouping = false;

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
                return distance <= 8;
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
                // Mở rộng vùng chọn thêm 5 pixel cho mỗi cạnh
                rect.Inflate(5, 5);
                return rect.Contains(point);
            }

            public Rectangle GetRectangle()
            {
                int x = Math.Min(StartPoint.X, EndPoint.X);
                int y = Math.Min(StartPoint.Y, EndPoint.Y);
                int width = Math.Abs(EndPoint.X - StartPoint.X) + 1;
                int height = Math.Abs(EndPoint.Y - StartPoint.Y) + 1;
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
                float centerX = rect.X + a;
                float centerY = rect.Y + b;
                float x = point.X - centerX;
                float y = point.Y - centerY;

                float ratio = (x * x) / (a * a) + (y * y) / (b * b);
                return ratio <= 1.1;
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
            private float curvature = 0.5f;

            public float GetCurvature()
            {
                return curvature;
            }

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
                // Mở rộng vùng chọn thêm 5 pixel cho mỗi cạnh
                rect.Inflate(5, 5);
                return rect.Contains(point);
            }

            public Rectangle GetSquare()
            {
                int size = Math.Max(Math.Abs(EndPoint.X - StartPoint.X), Math.Abs(EndPoint.Y - StartPoint.Y)) + 1;
                int x = StartPoint.X;
                int y = StartPoint.Y;

                if (EndPoint.X < StartPoint.X) x = StartPoint.X - size + 1;
                if (EndPoint.Y < StartPoint.Y) y = StartPoint.Y - size + 1;

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

                return distance <= radius + 5;
            }

            public Rectangle GetCircle()
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

        private class Group
        {
            public List<Shape> Shapes { get; set; } = new List<Shape>();
            public List<Group> SubGroups { get; set; } = new List<Group>();
            public bool IsSelected { get; set; } = false;

            public void Draw(Graphics g)
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(g);
                }

                foreach (var group in SubGroups)
                {
                    group.Draw(g);
                }

                if (IsSelected)
                {
                    Rectangle bounds = GetGroupBounds();
                    using (Pen highlightPen = new Pen(Color.Blue, 2) { DashStyle = DashStyle.Dash })
                    {
                        g.DrawRectangle(highlightPen, bounds);
                    }
                }
            }

            public Rectangle GetGroupBounds()
            {
                if (Shapes.Count == 0 && SubGroups.Count == 0) return Rectangle.Empty;

                int minX = int.MaxValue;
                int minY = int.MaxValue;
                int maxX = int.MinValue;
                int maxY = int.MinValue;

                foreach (var shape in Shapes)
                {
                    minX = Math.Min(minX, Math.Min(shape.StartPoint.X, shape.EndPoint.X));
                    minY = Math.Min(minY, Math.Min(shape.StartPoint.Y, shape.EndPoint.Y));
                    maxX = Math.Max(maxX, Math.Max(shape.StartPoint.X, shape.EndPoint.X));
                    maxY = Math.Max(maxY, Math.Max(shape.StartPoint.Y, shape.EndPoint.Y));

                    foreach (Point p in shape.Points)
                    {
                        minX = Math.Min(minX, p.X);
                        minY = Math.Min(minY, p.Y);
                        maxX = Math.Max(maxX, p.X);
                        maxY = Math.Max(maxY, p.Y);
                    }
                }

                foreach (var group in SubGroups)
                {
                    Rectangle bounds = group.GetGroupBounds();
                    minX = Math.Min(minX, bounds.X);
                    minY = Math.Min(minY, bounds.Y);
                    maxX = Math.Max(maxX, bounds.X + bounds.Width);
                    maxY = Math.Max(maxY, bounds.Y + bounds.Height);
                }

                return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            }

            public bool Contains(Point point)
            {
                return GetGroupBounds().Contains(point);
            }

            public void Move(int dx, int dy)
            {
                foreach (var shape in Shapes)
                {
                    shape.StartPoint = new Point(shape.StartPoint.X + dx, shape.StartPoint.Y + dy);
                    shape.EndPoint = new Point(shape.EndPoint.X + dx, shape.EndPoint.Y + dy);

                    for (int i = 0; i < shape.Points.Count; i++)
                    {
                        shape.Points[i] = new Point(shape.Points[i].X + dx, shape.Points[i].Y + dy);
                    }
                }

                foreach (var group in SubGroups)
                {
                    group.Move(dx, dy);
                }
            }

            public List<Shape> GetAllShapes()
            {
                List<Shape> allShapes = new List<Shape>();
                allShapes.AddRange(Shapes);
                foreach (var group in SubGroups)
                {
                    allShapes.AddRange(group.GetAllShapes());
                }
                return allShapes;
            }
        }

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
            panel_khungve.MouseWheel += Panel_khungve_MouseWheel;
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
            isDrawing = false;
            currentShape = null;
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
            isDrawing = false;
            currentShape = null;
            trackBarCurvature.Visible = false;
        }

        private void btnHcn_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Rectangle;
            isDrawing = false;
            currentShape = null;
            trackBarCurvature.Visible = false;
        }

        private void btnHV_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Square;
            isDrawing = false;
            currentShape = null;
            trackBarCurvature.Visible = false;
        }

        private void btnHinhTron_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Circle;
            isDrawing = false;
            currentShape = null;
            trackBarCurvature.Visible = false;
        }

        private void btnDuongCong_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.BezierCurve;
            isDrawing = false;
            currentShape = null;
            trackBarCurvature.Visible = true;
        }

        private void btnDaGiac_Click(object sender, EventArgs e)
        {
            currentShapeType = ShapeType.Polygon;
            isDrawing = false;
            currentShape = null;
            trackBarCurvature.Visible = false;
        }

        private void btnMauDuong_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (selectedShapes.Count > 0)
                {
                    foreach (Shape shape in selectedShapes)
                    {
                        shape.PenColor = colorDialog.Color;
                    }
                    panel_khungve.Invalidate();
                }
                else
                {
                    penColor = colorDialog.Color;
                }
            }
        }

        private void btnMauNen_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (selectedShapes.Count > 0)
                {
                    foreach (Shape shape in selectedShapes)
                    {
                        shape.BrushColor = colorDialog.Color;
                    }
                    panel_khungve.Invalidate();
                }
                else
                {
                    brushColor = colorDialog.Color;
                }
            }
        }

        private void numDoDay_ValueChanged(object sender, EventArgs e)
        {
            if (selectedShapes.Count > 0)
            {
                foreach (Shape shape in selectedShapes)
                {
                    shape.PenWidth = (float)numDoDay.Value;
                }
                panel_khungve.Invalidate();
            }
            else
            {
                penWidth = (float)numDoDay.Value;
            }
        }

        private void cmbKieuVe_SelectedIndexChanged(object sender, EventArgs e)
        {
            DashStyle newStyle;
            switch (cmbKieuVe.SelectedIndex)
            {
                case 0:
                    newStyle = DashStyle.Solid;
                    break;
                case 1:
                    newStyle = DashStyle.Dash;
                    break;
                case 2:
                    newStyle = DashStyle.Dot;
                    break;
                case 3:
                    newStyle = DashStyle.DashDot;
                    break;
                default:
                    newStyle = DashStyle.Solid;
                    break;
            }

            if (selectedShapes.Count > 0)
            {
                foreach (Shape shape in selectedShapes)
                {
                    shape.PenStyle = newStyle;
                }
                panel_khungve.Invalidate();
            }
            else
            {
                penStyle = newStyle;
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
            if (shapes.Count > 0 || groups.Count > 0)
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
                                foreach (var group in groups)
                                {
                                    foreach (var shape in group.Shapes)
                                    {
                                        shape.Draw(g);
                                    }
                                }
                            }
                            bmp.Save(saveFileDialog.FileName);
                        }
                    }

                    // Xóa tất cả các hình và nhóm
                    shapes.Clear();
                    groups.Clear();
                    selectedShapes.Clear();
                    isGrouping = false;
                    panel_khungve.Invalidate();
                }
            }
            else
            {
                // Nếu không có hình vẽ và nhóm nào, không cần hỏi
                shapes.Clear();
                groups.Clear();
                selectedShapes.Clear();
                isGrouping = false;
                panel_khungve.Invalidate();
            }
        }

        private void Luufiledave_Click(object sender, EventArgs e)
        {
            if (groups.Count > 0)
            {
                // Hiển thị dialog cho người dùng chọn nhóm để lưu
                using (var groupSelectForm = new Form())
                {
                    groupSelectForm.Text = "Chọn nhóm để lưu";
                    groupSelectForm.Size = new Size(300, 400);
                    groupSelectForm.StartPosition = FormStartPosition.CenterParent;

                    var listBox = new ListBox();
                    listBox.Dock = DockStyle.Fill;
                    for (int i = 0; i < groups.Count; i++)
                    {
                        listBox.Items.Add($"Nhóm {i + 1} ({groups[i].Shapes.Count} hình)");
                    }
                    listBox.Items.Add("Tất cả các hình");

                    var btnOK = new Button();
                    btnOK.Text = "OK";
                    btnOK.DialogResult = DialogResult.OK;
                    btnOK.Dock = DockStyle.Bottom;

                    groupSelectForm.Controls.Add(listBox);
                    groupSelectForm.Controls.Add(btnOK);

                    if (groupSelectForm.ShowDialog() == DialogResult.OK && listBox.SelectedIndex != -1)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Bitmap bmp = new Bitmap(panel_khungve.Width, panel_khungve.Height);
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.Clear(panel_khungve.BackColor);

                                if (listBox.SelectedIndex == groups.Count)
                                {
                                    // Lưu tất cả các hình
                                    foreach (Shape shape in shapes)
                                    {
                                        shape.Draw(g);
                                    }
                                    foreach (var group in groups)
                                    {
                                        group.Draw(g);
                                    }
                                }
                                else
                                {
                                    // Lưu chỉ nhóm được chọn
                                    groups[listBox.SelectedIndex].Draw(g);
                                }
                            }
                            bmp.Save(saveFileDialog.FileName);
                        }
                    }
                }
            }
            else
            {
                // Nếu không có nhóm nào, lưu tất cả các hình như bình thường
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
        }

        private void Chonvungdave_Click(object sender, EventArgs e)
        {
            isMoving = false;
            isSelecting = true;
            isDrawing = false;
            currentShapeType = ShapeType.Line;
            trackBarCurvature.Visible = false;
            panel_khungve.Cursor = Cursors.Cross;
            panel_khungve.Invalidate();
        }

        private void NhomCacHinh_Click(object sender, EventArgs e)
        {
            if (selectedShapes.Count > 0 || groups.Count(g => g.IsSelected) > 0)
            {
                Group newGroup = new Group();
                
                // Thêm các hình được chọn vào nhóm mới
                newGroup.Shapes.AddRange(selectedShapes);
                shapes.RemoveAll(s => selectedShapes.Contains(s));
                selectedShapes.Clear();

                // Thêm các nhóm được chọn vào nhóm mới
                var selectedGroups = groups.Where(g => g.IsSelected).ToList();
                newGroup.SubGroups.AddRange(selectedGroups);
                groups.RemoveAll(g => selectedGroups.Contains(g));

                groups.Add(newGroup);
                isGrouping = true;
                MessageBox.Show($"Đã tạo nhóm mới với {newGroup.GetAllShapes().Count} hình", "Thông báo");
            }
            panel_khungve.Invalidate();
        }

        private void GoCacHinh_Click(object sender, EventArgs e)
        {
            if (groups.Count > 0)
            {
                var lastGroup = groups[groups.Count - 1];
                
                // Nếu nhóm có nhóm con, tách nhóm con cuối cùng
                if (lastGroup.SubGroups.Count > 0)
                {
                    var lastSubGroup = lastGroup.SubGroups[lastGroup.SubGroups.Count - 1];
                    lastGroup.SubGroups.RemoveAt(lastGroup.SubGroups.Count - 1);
                    groups.Add(lastSubGroup);
                    MessageBox.Show("Đã tách một nhóm con", "Thông báo");
                }
                // Nếu nhóm có hình, tách hình cuối cùng
                else if (lastGroup.Shapes.Count > 0)
                {
                    var lastShape = lastGroup.Shapes[lastGroup.Shapes.Count - 1];
                    lastGroup.Shapes.RemoveAt(lastGroup.Shapes.Count - 1);
                    shapes.Add(lastShape);
                    MessageBox.Show("Đã tách một hình khỏi nhóm", "Thông báo");
                }

                // Nếu nhóm không còn hình và nhóm con, xóa nhóm
                if (lastGroup.Shapes.Count == 0 && lastGroup.SubGroups.Count == 0)
                {
                    groups.RemoveAt(groups.Count - 1);
                    if (groups.Count == 0)
                    {
                        isGrouping = false;
                    }
                }
            }
            panel_khungve.Invalidate();
        }

        private void XoaMenu_Click(object sender, EventArgs e)
        {
            if (selectedShapes.Count > 0)
            {
                foreach (Shape shape in selectedShapes.ToList())
                {
                    shapes.Remove(shape);
                }
                selectedShapes.Clear();
                panel_khungve.Invalidate();
            }
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
                if (isSelecting)
                {
                    selectionStart = e.Location;
                    selectionRect = new Rectangle();
                    selectedShapes.Clear();
                    panel_khungve.Invalidate();
                    return;
                }

                if (currentShapeType == ShapeType.Curve || currentShapeType == ShapeType.Polygon)
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
                }
                else
                {
                    // Kiểm tra xem có click vào nhóm nào không
                    bool hitGroup = false;
                    foreach (var group in groups)
                    {
                        if (group.Contains(e.Location))
                        {
                            if (!ModifierKeys.HasFlag(Keys.Control))
                            {
                                // Bỏ chọn tất cả các nhóm khác
                                foreach (var g in groups)
                                {
                                    g.IsSelected = false;
                                }
                                selectedShapes.Clear();
                            }
                            group.IsSelected = !group.IsSelected;
                            isMoving = true;
                            lastPoint = e.Location;
                            hitGroup = true;
                            break;
                        }
                    }

                    if (!hitGroup)
                    {
                        // Kiểm tra xem có click vào hình đã chọn không
                        bool hitSelected = false;
                        foreach (Shape shape in selectedShapes)
                        {
                            if (shape.Contains(e.Location))
                            {
                                hitSelected = true;
                                isMoving = true;
                                lastPoint = e.Location;
                                break;
                            }
                        }

                        if (!hitSelected)
                        {
                            // Nếu click vào khoảng trống và không giữ Ctrl, bỏ chọn tất cả
                            if (!ModifierKeys.HasFlag(Keys.Control))
                            {
                                selectedShapes.Clear();
                                foreach (var group in groups)
                                {
                                    group.IsSelected = false;
                                }
                            }

                            // Kiểm tra xem có click vào hình nào không
                            foreach (Shape shape in shapes.Reverse<Shape>())
                            {
                                if (shape.Contains(e.Location))
                                {
                                    if (!selectedShapes.Contains(shape))
                                    {
                                        selectedShapes.Add(shape);
                                        isMoving = true;
                                        lastPoint = e.Location;
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Control))
                                    {
                                        selectedShapes.Remove(shape);
                                    }
                                    break;
                                }
                            }

                            // Nếu không click vào hình nào và không đang di chuyển, bắt đầu vẽ hình mới
                            if (!isMoving)
                            {
                                isDrawing = true;
                                startPoint = ClampToPanel(e.Location);
                                CreateNewShape();
                            }
                        }
                    }
                }
                panel_khungve.Invalidate();
            }
            else if (e.Button == MouseButtons.Right && currentShapeType == ShapeType.Polygon)
            {
                if (isDrawing && currentShape != null && currentShape.Points.Count >= 3)
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

            if (isSelecting && e.Button == MouseButtons.Left)
            {
                int x = Math.Min(selectionStart.X, clampedLocation.X);
                int y = Math.Min(selectionStart.Y, clampedLocation.Y);
                int width = Math.Abs(clampedLocation.X - selectionStart.X);
                int height = Math.Abs(clampedLocation.Y - selectionStart.Y);
                selectionRect = new Rectangle(x, y, width, height);
                panel_khungve.Invalidate();
                return;
            }

            if (isMoving && e.Button == MouseButtons.Left)
            {
                int dx = clampedLocation.X - lastPoint.X;
                int dy = clampedLocation.Y - lastPoint.Y;

                bool canMove = true;

                // Kiểm tra giới hạn di chuyển cho các nhóm được chọn
                foreach (var group in groups.Where(g => g.IsSelected))
                {
                    Rectangle bounds = group.GetGroupBounds();
                    bounds.Offset(dx, dy);
                    if (!panel_khungve.ClientRectangle.Contains(bounds))
                    {
                        canMove = false;
                        break;
                    }
                }

                // Kiểm tra giới hạn di chuyển cho các hình được chọn
                foreach (Shape shape in selectedShapes)
                {
                    Rectangle bounds = GetShapeBounds(shape);
                    bounds.Offset(dx, dy);
                    if (!panel_khungve.ClientRectangle.Contains(bounds))
                    {
                        canMove = false;
                        break;
                    }
                }

                if (canMove)
                {
                    // Di chuyển các nhóm được chọn
                    foreach (var group in groups.Where(g => g.IsSelected))
                    {
                        group.Move(dx, dy);
                    }

                    // Di chuyển các hình được chọn
                    foreach (Shape shape in selectedShapes)
                    {
                        shape.StartPoint = new Point(shape.StartPoint.X + dx, shape.StartPoint.Y + dy);
                        shape.EndPoint = new Point(shape.EndPoint.X + dx, shape.EndPoint.Y + dy);

                        for (int i = 0; i < shape.Points.Count; i++)
                        {
                            shape.Points[i] = new Point(shape.Points[i].X + dx, shape.Points[i].Y + dy);
                        }
                    }
                    lastPoint = clampedLocation;
                }
                panel_khungve.Invalidate();
            }
            else if (isDrawing && currentShape != null)
            {
                endPoint = clampedLocation;
                if (currentShapeType != ShapeType.Curve && currentShapeType != ShapeType.Polygon)
                {
                    currentShape.EndPoint = endPoint;
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
            if (isSelecting && e.Button == MouseButtons.Left)
            {
                foreach (Shape shape in shapes)
                {
                    if (IsShapeInSelectionRect(shape, selectionRect))
                    {
                        selectedShapes.Add(shape);
                    }
                }

                selectionRect = Rectangle.Empty;
                isSelecting = false;
                panel_khungve.Cursor = Cursors.Default;
                panel_khungve.Invalidate();
                return;
            }

            if (isMoving)
            {
                isMoving = false;
            }
            else if (isDrawing && e.Button == MouseButtons.Left)
            {
                if (currentShapeType != ShapeType.Curve && currentShapeType != ShapeType.Polygon)
                {
                    endPoint = ClampToPanel(e.Location);
                    currentShape.EndPoint = endPoint;
                    shapes.Add(currentShape);
                    currentShape = null;
                    isDrawing = false;
                }
                panel_khungve.Invalidate();
            }
        }

        private void Panel_khungve_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Vẽ các hình không thuộc nhóm
            foreach (Shape shape in shapes)
            {
                shape.Draw(e.Graphics);
            }

            // Vẽ các nhóm
            foreach (var group in groups)
            {
                foreach (var shape in group.Shapes)
                {
                    shape.Draw(e.Graphics);
                }

                if (group.IsSelected)
                {
                    // Vẽ khung bao quanh nhóm
                    Rectangle bounds = group.GetGroupBounds();
                    using (Pen highlightPen = new Pen(Color.Blue, 2) { DashStyle = DashStyle.Dash })
                    {
                        e.Graphics.DrawRectangle(highlightPen, bounds);
                    }
                }
            }

            if (currentShape != null)
            {
                currentShape.Draw(e.Graphics);
            }

            // Vẽ viền highlight cho các hình được chọn
            foreach (Shape shape in selectedShapes)
            {
                using (Pen highlightPen = new Pen(Color.Red, 2) { DashStyle = DashStyle.Dash })
                {
                    if (shape is RectangleShape)
                    {
                        Rectangle rect = ((RectangleShape)shape).GetRectangle();
                        e.Graphics.DrawRectangle(highlightPen, rect);
                    }
                    else if (shape is EllipseShape)
                    {
                        Rectangle rect = ((EllipseShape)shape).GetRectangle();
                        e.Graphics.DrawEllipse(highlightPen, rect);
                    }
                    else if (shape is SquareShape)
                    {
                        Rectangle rect = ((SquareShape)shape).GetSquare();
                        e.Graphics.DrawRectangle(highlightPen, rect);
                    }
                    else if (shape is CircleShape)
                    {
                        Rectangle rect = ((CircleShape)shape).GetCircle();
                        e.Graphics.DrawEllipse(highlightPen, rect);
                    }
                    else if (shape is LineShape)
                    {
                        e.Graphics.DrawLine(highlightPen, shape.StartPoint, shape.EndPoint);
                    }
                    else if (shape is CurveShape && shape.Points.Count > 1)
                    {
                        e.Graphics.DrawCurve(highlightPen, shape.Points.ToArray());
                    }
                    else if (shape is PolygonShape && shape.Points.Count > 2)
                    {
                        e.Graphics.DrawPolygon(highlightPen, shape.Points.ToArray());
                    }
                    else if (shape is BezierCurveShape)
                    {
                        Point controlPoint1 = new Point(
                            shape.StartPoint.X + (int)((shape.EndPoint.X - shape.StartPoint.X) * 0.5f),
                            shape.StartPoint.Y - (int)((shape.EndPoint.Y - shape.StartPoint.Y) * ((BezierCurveShape)shape).GetCurvature())
                        );

                        Point controlPoint2 = new Point(
                            shape.StartPoint.X + (int)((shape.EndPoint.X - shape.StartPoint.X) * 0.5f),
                            shape.StartPoint.Y - (int)((shape.EndPoint.Y - shape.StartPoint.Y) * ((BezierCurveShape)shape).GetCurvature())
                        );

                        e.Graphics.DrawBezier(highlightPen, shape.StartPoint, controlPoint1, controlPoint2, shape.EndPoint);
                    }
                }
            }

            if (isSelecting && !selectionRect.IsEmpty)
            {
                using (Pen selectionPen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(selectionPen, selectionRect);
                    using (Brush selectionBrush = new SolidBrush(Color.FromArgb(50, Color.LightBlue)))
                    {
                        e.Graphics.FillRectangle(selectionBrush, selectionRect);
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

        private bool IsShapeInSelectionRect(Shape shape, Rectangle selectionRect)
        {
            if (shape is RectangleShape)
            {
                return selectionRect.Contains(((RectangleShape)shape).GetRectangle());
            }
            else if (shape is EllipseShape)
            {
                return selectionRect.Contains(((EllipseShape)shape).GetRectangle());
            }
            else if (shape is SquareShape)
            {
                return selectionRect.Contains(((SquareShape)shape).GetSquare());
            }
            else if (shape is CircleShape)
            {
                return selectionRect.Contains(((CircleShape)shape).GetCircle());
            }
            else if (shape is LineShape)
            {
                return selectionRect.Contains(shape.StartPoint) && selectionRect.Contains(shape.EndPoint);
            }
            else if (shape is CurveShape || shape is PolygonShape)
            {
                foreach (Point point in shape.Points)
                {
                    if (!selectionRect.Contains(point))
                        return false;
                }
                return true;
            }
            else if (shape is BezierCurveShape)
            {
                return selectionRect.Contains(shape.StartPoint) && selectionRect.Contains(shape.EndPoint);
            }

            return false;
        }

        private void Panel_khungve_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                float scale = e.Delta > 0 ? 1.1f : 0.9f;
                Point center = e.Location;

                foreach (Shape shape in selectedShapes)
                {
                    ScaleShape(shape, scale, center);
                }

                foreach (var group in groups.Where(g => g.IsSelected))
                {
                    foreach (var shape in group.Shapes)
                    {
                        ScaleShape(shape, scale, center);
                    }
                }

                panel_khungve.Invalidate();
            }
        }

        private void ScaleShape(Shape shape, float scale, Point center)
        {
            shape.StartPoint = new Point(
                center.X + (int)((shape.StartPoint.X - center.X) * scale),
                center.Y + (int)((shape.StartPoint.Y - center.Y) * scale)
            );

            shape.EndPoint = new Point(
                center.X + (int)((shape.EndPoint.X - center.X) * scale),
                center.Y + (int)((shape.EndPoint.Y - center.Y) * scale)
            );

            for (int i = 0; i < shape.Points.Count; i++)
            {
                shape.Points[i] = new Point(
                    center.X + (int)((shape.Points[i].X - center.X) * scale),
                    center.Y + (int)((shape.Points[i].Y - center.Y) * scale)
                );
            }
        }
    }
}
