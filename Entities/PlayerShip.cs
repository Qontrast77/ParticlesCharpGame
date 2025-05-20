using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class PlayerShip
    {
        public float X { get; set; }
        public float Y { get; set; }
        private const int ShipWidth = 30;
        private const int ShipHeight = 45;
        private const int EngineSize = 10;
        private const int CockpitSize = 15;
        public int Size { get; } = 30;

        private Image shipImage;

        public PlayerShip(float x, float y)
        {
            X = x;
            Y = y;
            shipImage = Image.FromFile("..\\..\\..\\Image\\PlayerShip.png"); // Загружаем изображение корабля
        }

        public void UpdatePosition(Point target)
        {
            X += (target.X - X) * 0.1f;
            Y += (target.Y - Y) * 0.1f;
        }

        public void Draw(Graphics g)
        {
            // Новые размеры корабля (уменьшаем в два раза, например)
            float scaledWidth = shipImage.Width / 10;
            float scaledHeight = shipImage.Height / 10;

            // Рисуем изображение с новыми размерами
            g.DrawImage(shipImage, X - scaledWidth / 2, Y - scaledHeight / 2, scaledWidth, scaledHeight);

            // Огни двигателей (анимированные)
            if (DateTime.Now.Millisecond % 500 < 250)
            {
                // Рисуем пламя слева
                PointF[] leftFlame = new PointF[3]
                {
                    new PointF(X - ShipWidth / 3 + EngineSize / 4, Y + ShipHeight / 2 + EngineSize),
                    new PointF(X - ShipWidth / 3, Y + ShipHeight / 2 + EngineSize + 5), // уменьшена длина пламени
                    new PointF(X - ShipWidth / 3 + EngineSize / 2, Y + ShipHeight / 2 + EngineSize + 5)
                };

                // Рисуем пламя справа
                PointF[] rightFlame = new PointF[3]
                {
                    new PointF(X + ShipWidth / 3 - EngineSize / 4, Y + ShipHeight / 2 + EngineSize),
                    new PointF(X + ShipWidth / 3 - EngineSize / 2, Y + ShipHeight / 2 + EngineSize + 5),
                    new PointF(X + ShipWidth / 3, Y + ShipHeight / 2 + EngineSize + 5)
                };

                // Рисуем центральное пламя
                PointF[] centerFlame = new PointF[3]
                {
                    new PointF(X, Y + ShipHeight / 2 + EngineSize),
                    new PointF(X - EngineSize / 3, Y + ShipHeight / 2 + EngineSize + 8), // уменьшена длина пламени
                    new PointF(X + EngineSize / 3, Y + ShipHeight / 2 + EngineSize + 8)
                };

                // Заполняем пламя оранжевым цветом
                g.FillPolygon(Brushes.OrangeRed, leftFlame);
                g.FillPolygon(Brushes.OrangeRed, rightFlame);
                g.FillPolygon(Brushes.OrangeRed, centerFlame);
            }
        }
    }
}
