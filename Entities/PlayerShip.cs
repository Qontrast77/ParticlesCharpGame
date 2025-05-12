using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class PlayerShip
    {
        public float X { get; set; }
        public float Y { get; set; }
        private const int Size = 30;

        public PlayerShip(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void UpdatePosition(Point target)
        {
            X += (target.X - X) * 0.1f;
            Y += (target.Y - Y) * 0.1f;
        }

        public void Draw(Graphics g)
        {
            PointF[] shipPoints = new PointF[3]
            {
                new PointF(X, Y - Size),
                new PointF(X - Size/2, Y + Size/2),
                new PointF(X + Size/2, Y + Size/2)
            };
            
            g.FillPolygon(Brushes.LightBlue, shipPoints);
            g.DrawPolygon(Pens.White, shipPoints);
        }
    }
}