using System;
using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class LaserBeam
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float EndX { get; set; }
        public float EndY { get; set; }
        public Color Color { get; set; }
        public int Lifetime { get; set; } = 10;
        public bool IsComplete => Lifetime <= 0;

        public LaserBeam(float startX, float startY, Point target, Color color)
        {
            X = startX;
            Y = startY;
            EndX = target.X;
            EndY = target.Y;
            Color = color;
        }

        // Mark this method as virtual so it can be overridden in derived classes
        public virtual void Update()
        {
            Lifetime--;
        }

        public void Draw(Graphics g)
        {
            // Create the pen for drawing the laser beam
            using (Pen pen = new Pen(Color, 3))  // Corrected line
            {
                g.DrawLine(pen, X, Y, EndX, EndY);
            }

            // Effect for the glow (additional pen with transparency)
            using (Pen glowPen = new Pen(Color.FromArgb(100, Color.R, Color.G, Color.B), 10))
            {
                g.DrawLine(glowPen, X, Y, EndX, EndY);
            }
        }

        public bool CollidesWith(Ball ball)
        {
            // Simple collision check between line and circle
            float distance = (float)DistancePointToLine(ball.X, ball.Y, X, Y, EndX, EndY);
            return distance < ball.Radius;
        }

        private double DistancePointToLine(float px, float py, float x1, float y1, float x2, float y2)
        {
            float A = px - x1;
            float B = py - y1;
            float C = x2 - x1;
            float D = y2 - y1;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            float xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            float dx = px - xx;
            float dy = py - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
