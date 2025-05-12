using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class Bullet
    {
        public float X { get; set; }
        public float Y { get; set; }
        private float dx, dy;
        private const float Speed = 15;
        private const int Size = 5;

        public Bullet(float startX, float startY, Point target)
        {
            X = startX;
            Y = startY;

            float angle = (float)Math.Atan2(target.Y - startY, target.X - startX);
            dx = (float)Math.Cos(angle) * Speed;
            dy = (float)Math.Sin(angle) * Speed;
        }

        public void Update()
        {
            X += dx;
            Y += dy;
        }

        public bool IsOutOfBounds(Size bounds)
        {
            return X < 0 || X > bounds.Width || Y < 0 || Y > bounds.Height;
        }

        public bool CollidesWith(Ball ball)
        {
            float distance = (float)Math.Sqrt(Math.Pow(X - ball.X, 2) + Math.Pow(Y - ball.Y, 2));
            return distance < ball.Radius + Size / 2;
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Yellow, X - Size / 2, Y - Size / 2, Size, Size);
        }
    }
}