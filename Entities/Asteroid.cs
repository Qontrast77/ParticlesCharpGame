using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceBallCrusher1.Entities
{
    public class Asteroid : BackgroundObject
    {
        private PointF[] shape;

        public Asteroid(int width, int height)
        {
            var random = new Random();
            X = random.Next(0, width);
            Y = random.Next(0, height);
            Speed = random.Next(1, 3);
            Size = random.Next(3, 8);
            Color = Color.FromArgb(random.Next(50, 100), random.Next(50, 100), random.Next(50, 100));

            // Создаем неправильную форму астероида
            shape = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = (float)(i * Math.PI / 3);
                float radius = Size * (0.7f + (random.Next(0, 30) / 100f));
                shape[i] = new PointF(
                    (float)(radius * Math.Cos(angle)),
                    (float)(radius * Math.Sin(angle)));
            }
        }

        public override void Draw(Graphics g)
        {
            var transform = new Matrix();
            transform.Translate(X, Y);
            g.Transform = transform;

            g.FillPolygon(new SolidBrush(Color), shape);
            g.DrawPolygon(Pens.DarkGray, shape);

            g.ResetTransform();
        }
    }
}