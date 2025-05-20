using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceBallCrusher1.Entities
{
    public class DangerousAsteroid : SpaceObject
    {
        private PointF[] shape;

        public DangerousAsteroid(float x, float y, float radius, float speed, Color color)
            : base(x, y, radius, color)
        {
            Speed = speed;
            float angle = (float)(new Random().NextDouble() * Math.PI * 2);
            dx = (float)Math.Cos(angle) * Speed;
            dy = (float)Math.Sin(angle) * Speed;

            // Создаем неправильную форму
            shape = new PointF[8];
            for (int i = 0; i < 8; i++)
            {
                float a = (float)(i * Math.PI / 4);
                float r = Radius * (0.7f + (new Random().Next(0, 30) / 100f));
                shape[i] = new PointF(
                    (float)(r * Math.Cos(a)),
                    (float)(r * Math.Sin(a)));
            }
        }

        public override void Update(int width, int height)
        {
            X += dx;
            Y += dy;

            if (X < Radius || X > width - Radius) dx = -dx;
            if (Y < Radius || Y > height - Radius) dy = -dy;
        }

        public override void Draw(Graphics g)
        {
            var transform = new Matrix();
            transform.Translate(X, Y);
            g.Transform = transform;

            g.FillPolygon(new SolidBrush(Color), shape);
            g.DrawPolygon(new Pen(Color.DarkRed, 2), shape);

            g.ResetTransform();
        }
    }
}
