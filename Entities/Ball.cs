using System;
using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class Ball
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public float Speed { get; set; }
        public Color Color { get; set; }
        private float dx, dy;

        public Ball(int x, int y, float radius, float speed, Color color)
        {
            X = x;
            Y = y;
            Radius = radius;
            Speed = speed;
            Color = color;

            Random random = new Random();
            double angle = random.NextDouble() * Math.PI * 2;
            dx = (float)Math.Cos(angle) * speed;
            dy = (float)Math.Sin(angle) * speed;
        }

        public void Update(int width, int height)
        {
            X += dx;
            Y += dy;

            if (X - Radius < 0 || X + Radius > width)
            {
                dx = -dx;
                X = Math.Clamp(X, Radius, width - Radius);
            }

            if (Y - Radius < 0 || Y + Radius > height)
            {
                dy = -dy;
                Y = Math.Clamp(Y, Radius, height - Radius);
            }
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color), X - Radius, Y - Radius, Radius * 2, Radius * 2);
            g.DrawEllipse(Pens.White, X - Radius, Y - Radius, Radius * 2, Radius * 2);
        }
    }
}