using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class AmmoPickup : SpaceObject
    {
        public AmmoPickup(float x, float y, float radius, Color color)
            : base(x, y, radius, color)
        {
            // Движение по синусоиде
            Speed = 1.5f;
            float angle = (float)(new Random().NextDouble() * Math.PI * 2);
            dx = (float)Math.Cos(angle) * Speed;
            dy = (float)Math.Sin(angle) * Speed;
        }

        public override void Update(int width, int height)
        {
            X += dx;
            Y += dy + (float)Math.Sin(X * 0.05) * 0.5f; // Волнообразное движение

            if (X < 0 || X > width) dx = -dx;
            if (Y < 0 || Y > height) dy = -dy;
        }

        public override void Draw(Graphics g)
        {
            // Цвет патрона
            Color bulletColor = Color.Yellow;

            // Рисуем корпус патрона (прямоугольник)
            RectangleF bulletBody = new RectangleF(X - Radius, Y, Radius * 2, Radius * 4);
            g.FillRectangle(new SolidBrush(bulletColor), bulletBody);
            g.DrawRectangle(Pens.Black, bulletBody.X, bulletBody.Y, bulletBody.Width, bulletBody.Height);

            // Рисуем верхнюю часть патрона (круглая "головка")
            g.FillEllipse(new SolidBrush(bulletColor), X - Radius, Y - Radius * 2, Radius * 2, Radius * 2);
            g.DrawEllipse(Pens.Black, X - Radius, Y - Radius * 2, Radius * 2, Radius * 2);

            // Рисуем основание патрона (маленький круг внизу)
            g.FillEllipse(new SolidBrush(Color.Black), X - Radius, Y + Radius * 3, Radius * 2, Radius);
            g.DrawEllipse(Pens.Black, X - Radius, Y + Radius * 3, Radius * 2, Radius);
        }



    }
}