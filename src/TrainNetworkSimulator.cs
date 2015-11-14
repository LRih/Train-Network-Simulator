using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TrainNetworkSimulator
{
    public class TrainNetworkSimulator : Form
    {
        //===================================================================== VARIABLES
        private IContainer _components = new Container();
        private Label _lblTime = new Label();
        private Timer _timer;

        private Bitmap _bmpTrainNetwork;
        private TrainNetwork _trainNetwork = new TrainNetwork();

        private float _zoom = 0.05f;
        private Point _origin;

        private bool _isPanning;

        //===================================================================== INITIALIZE
        public TrainNetworkSimulator()
        {
            this.ClientSize = new Size(800, 600);

            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 8);
            this.Text = "Train Network Simulator";

            _lblTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _lblTime.AutoSize = true;
            _lblTime.BackColor = Color.Transparent;
            _lblTime.Font = new Font(this.Font.FontFamily, 18);
            _lblTime.Location = new Point(10, ClientSize.Height - _lblTime.Height - 15);
            _lblTime.Text = _trainNetwork.Time.ToString();
            Button btnTest = new Button();
            btnTest.Location = new Point(10, 10);
            btnTest.Text = "Test";
            btnTest.Click += btnTest_Click;
            _timer = new Timer(_components);
            _timer.Interval = 1000;
            //_timer.Interval = 20;
            _timer.Start();
            _timer.Tick += timer_Tick;
            this.Controls.Add(_lblTime);
            this.Controls.Add(btnTest);
            //RecreateBitmap();
        }

        //===================================================================== TERMINATE
        protected override void Dispose(bool disposing)
        {
            if (disposing && _bmpTrainNetwork != null)
            {
                _bmpTrainNetwork.Dispose();
                _bmpTrainNetwork = null;
            }
            base.Dispose(disposing);
        }

        //===================================================================== FUNCTIONS
        private void RecreateBitmap()
        {
            if (_bmpTrainNetwork != null) _bmpTrainNetwork.Dispose();
            _bmpTrainNetwork = new Bitmap(1000, 1000);
            Graphics g = Graphics.FromImage(_bmpTrainNetwork);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            DrawLinks(g);
            DrawStations(g);
            g.Dispose();
            
        }
        private void DrawLinks(Graphics g)
        {
            foreach (Link link in _trainNetwork.Links)
            {
                GraphicsPath path = new GraphicsPath();
                int x1 = link.InStation.X;
                int y1 = link.InStation.Y;
                foreach (Point node in link.Nodes)
                {
                    int x2 = x1 + node.X;
                    int y2 = y1 + node.Y;
                    path.AddLine(x1, y1, x2, y2);
                    x1 = x2;
                    y1 = y2;
                }
                path.AddLine(x1, y1, link.OutStation.X, link.OutStation.Y);
                TransformToCanvas(path);
                using (Pen p = new Pen(Colors.GetLinkColor(link)))
                {
                    p.LineJoin = LineJoin.Round;
                    p.Width = Dimens.GetLinkWidth(link) * _zoom;
                    g.DrawPath(p, path);
                }
            }
        }
        private void DrawStations(Graphics g)
        {
            using (Font font = new Font("Segoe UI", 250 * _zoom))
            {
                foreach (Station station in _trainNetwork.Stations.Values)
                {
                    GraphicsPath path = new GraphicsPath();
                    int radius = Dimens.GetStationRadius(station);
                    path.AddEllipse(station.X - radius, station.Y - radius, radius * 2, radius * 2);
                    g.DrawString(station.Name, font, Brushes.Black, ToCanvasX(station.X + radius), ToCanvasY(station.Y));
                    TransformToCanvas(path);
                    using (Brush brush = new SolidBrush(Colors.GetStationColor(station)))
                        g.FillPath(brush, path);
                }
            }
        }
        private void DrawTrains(Graphics g)
        {
            foreach (Train train in _trainNetwork.Trains)
            {
                Point pt = train.Position;
                int radius = Dimens.GetTrainRadius();
                g.FillEllipse(Brushes.GreenYellow, ToCanvasX(pt.X - radius), ToCanvasY(pt.Y - radius), radius * 2 * _zoom, radius * 2 * _zoom);
                using (Font font = new Font("Arial", _zoom == 0.01f ? 4 : 150 * _zoom))
                    g.DrawString(train.Name + " : " + train.TrackNumber + "\n" + train.Status, font, Brushes.DarkGreen, ToCanvasX(pt.X + radius), ToCanvasY(pt.Y - 220));
            }
        }

        private void InvalidateRect()
        {
            this.Invalidate(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
        }

        private void TransformToCanvas(GraphicsPath path)
        {
            Matrix m = new Matrix();
            m.Scale(_zoom, _zoom);
            m.Translate(_origin.X, _origin.Y, MatrixOrder.Append);
            path.Transform(m);
        }
        private int ToCanvasX(int x)
        {
            return (int)(x * _zoom + _origin.X);
        }
        private int ToCanvasY(int y)
        {
            return (int)(y * _zoom + _origin.Y);
        }

        //===================================================================== EVENTS
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            DrawLinks(e.Graphics);
            DrawStations(e.Graphics);
            e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            DrawTrains(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            _origin = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
            InvalidateRect();
            base.OnResize(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = true;
                Point startingOrigin = _origin;
                Point startingMouse = MousePosition;
                while (_isPanning)
                {
                    _origin = new Point(startingOrigin.X - (startingMouse.X - MousePosition.X), startingOrigin.Y - (startingMouse.Y - MousePosition.Y));
                    InvalidateRect();
                    Application.DoEvents();
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPanning = false;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float oldZoom = _zoom;
            if (e.Delta < 0)
                _zoom = Math.Max(_zoom / 2, 0.01f);
            else
                _zoom = Math.Min(_zoom * 2, 0.5f);
            if (_zoom != oldZoom) _origin = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
            InvalidateRect();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            // Platforms with no destinations
            string resultNo = "Unconnected Platforms:\n";
            foreach (Station s in _trainNetwork.Stations.Values)
            {
                for (int i = 1; i <= s.PlatformCount; i++)
                {
                    if (s.GetPlatform(i).Destinations.Count == 0)
                        resultNo += s.Name + " " + i + ", ";
                }
            }
            MessageBox.Show(resultNo);

            // Platforms with multiple destinations
            string resultMult = "Platforms/Links Diverge:\n";
            foreach (Station s in _trainNetwork.Stations.Values)
            {
                for (int i = 1; i <= s.PlatformCount; i++)
                {
                    if (s.GetPlatform(i).Destinations.Count > 1)
                        resultMult += s.Name + " " + i + ": " + s.GetPlatform(i).Destinations[0].Link.OutStation.Name + ", " + s.GetPlatform(i).Destinations[1].Link.OutStation.Name + "\n";
                }
            }
            foreach (Link link in _trainNetwork.Links)
            {
                for (int i = 1; i <= link.TrackCount; i++)
                {
                    if (link.GetTrack(i).Destinations.Count > 1)
                        resultMult += "To " + link.GetTrack(i).Destinations[0].Station.Name + ": " + link.GetTrack(i).Destinations[0].Number + ", " + link.GetTrack(i).Destinations[1].Number + "\n";
                }
            }
            MessageBox.Show(resultMult);

            string result = "";
            Platform platform = _trainNetwork.Stations["Frankston"].GetPlatform(1);
            do
            {
                result += platform.Station.Name + " " + platform.Number;
                if (platform.Destinations.Count != 0)
                {
                    result += " > ";
                    platform = platform.Destinations[0].Destinations[0];
                }
                else
                    platform = null;
            }
            while (platform != null && platform.Station.Name != "Frankston");
            MessageBox.Show(result);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            _trainNetwork.Update();
            _lblTime.Text = _trainNetwork.Time.ToString();
            InvalidateRect();
        }
    }
}
