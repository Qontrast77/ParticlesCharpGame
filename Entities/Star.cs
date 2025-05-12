using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class Star : BackgroundObject
    {
        public Star(int width, int height)
        {
            var random = new Random();
            X = random.Next(0, width);
            Y = random.Next(0, height);
            Speed = random.Next(1, 4);
            Size = random.Next(1, 3);
            Color = Color.FromArgb(random.Next(150, 255), random.Next(150, 255), random.Next(150, 255));
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color), X, Y, Size, Size);
        }
    }
}