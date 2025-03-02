using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TowerOfHanoi
{
    public class TowerOfHanoiForm : Form
    {
        private List<Stack<int>> rods;
        private List<Move> moves;
        private int currentMove;
        private int numDisks = 5; // Number of disks

        public TowerOfHanoiForm()
        {
            this.Text = "🗼 Tower of Hanoi";
            this.Size = new Size(650, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.BackColor = Color.WhiteSmoke;

            rods = new List<Stack<int>> { new Stack<int>(), new Stack<int>(), new Stack<int>() };
            moves = new List<Move>();
            currentMove = 0;

            for (int i = numDisks; i > 0; i--)
                rods[0].Push(i);

            Solve(numDisks, 0, 2, 1);

            this.MouseClick += TowerOfHanoiForm_MouseClick;
        }

        private void TowerOfHanoiForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentMove < moves.Count)
            {
                var move = moves[currentMove++];
                int disk = rods[move.From].Pop();

                if (rods[move.To].Count == 0 || disk < rods[move.To].Peek())
                {
                    rods[move.To].Push(disk);
                    Invalidate(); // refresh drawing
                }
                else
                {
                    rods[move.From].Push(disk);
                    MessageBox.Show("Invalid move detected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("🎉 Puzzle solved!", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Solve(int n, int from, int to, int aux)
        {
            if (n == 0) return;
            Solve(n - 1, from, aux, to);
            moves.Add(new Move(from, to));
            Solve(n - 1, aux, to, from);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int rodWidth = 10;
            int rodHeight = 220;
            int rodSpacing = 180;
            int baseY = 360;

            // Draw rods with bases
            for (int i = 0; i < 3; i++)
            {
                int rodX = 120 + i * rodSpacing;

                // Rod
                g.FillRectangle(Brushes.SaddleBrown, rodX - rodWidth / 2, baseY - rodHeight, rodWidth, rodHeight);

                // Rod Base
                g.FillRectangle(Brushes.Peru, rodX - 60, baseY, 120, 10);
            }

            // Define disk colors
            Color[] diskColors = { Color.CornflowerBlue, Color.Orange, Color.MediumSeaGreen, Color.Tomato, Color.MediumPurple, Color.Gold };

            // Draw disks (bottom-to-top visually)
            for (int i = 0; i < 3; i++)
            {
                int[] disksArray = rods[i].ToArray();
                Array.Reverse(disksArray);

                int diskY = baseY - 20; // bottom position for disks

                foreach (var disk in disksArray)
                {
                    int diskWidth = disk * 30;
                    int rodX = 120 + i * rodSpacing;

                    // Rounded rectangle disks
                    Rectangle diskRect = new Rectangle(rodX - diskWidth / 2, diskY, diskWidth, 20);
                    GraphicsPath roundedRect = RoundedRect(diskRect, 8);
                    using (SolidBrush diskBrush = new SolidBrush(diskColors[(disk - 1) % diskColors.Length]))
                    {
                        g.FillPath(diskBrush, roundedRect);
                    }

                    diskY -= 22; // move up for next disk
                }
            }
        }
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.Run(new TowerOfHanoiForm());
        //}
    }

    public class Move
    {
        public int From { get; }
        public int To { get; }

        public Move(int from, int to)
        {
            From = from;
            To = to;
        }
    }
}
