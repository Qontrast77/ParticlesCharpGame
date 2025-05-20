using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class AmmoPickup : SpaceObject
    {
        private Image ammoImage;

        public AmmoPickup(float x, float y, float radius, Color color)
            : base(x, y, radius, color)
        {
            // Загружаем изображение патрона
            ammoImage = Image.FromFile("..\\..\\..\\Image\\Ammo.png"); // Убедитесь, что путь верный

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
            // Новые размеры патрона (уменьшаем в два раза, например)
            float scaledWidth = ammoImage.Width / 10;
            float scaledHeight = ammoImage.Height / 10;

            // Рисуем изображение с новыми размерами
            g.DrawImage(ammoImage, X - scaledWidth / 2, Y - scaledHeight / 2, scaledWidth, scaledHeight);
        }
    }
}
