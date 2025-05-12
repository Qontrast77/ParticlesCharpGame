
using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class LaserParticle : LaserBeam
    {
        private PointF velocity;
        private int life = 20;

        public LaserParticle(float x, float y, int size, Color color, PointF velocity)
            : base(x, y, new Point((int)x, (int)y), color)
        {
            this.velocity = velocity;
            Lifetime = 30;
        }

        // Override the Update method from LaserBeam
        public override void Update()
        {
            X += velocity.X;
            Y += velocity.Y;
            EndX = X;
            EndY = Y;
            Lifetime--;
        }

        public new void Draw(Graphics g)
        {
            float alpha = (Lifetime / 30f) * 255;
            Color fadeColor = Color.FromArgb((int)alpha, Color.R, Color.G, Color.B);

            using (Brush brush = new SolidBrush(fadeColor))
            {
                g.FillEllipse(brush, X - 3, Y - 3, 6, 6);
            }
        }
    }

}