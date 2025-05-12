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
            // Корпус корабля (основной треугольник)
            PointF[] hull = new PointF[3]
            {
                new PointF(X, Y - ShipHeight/2), // Нос корабля
                new PointF(X - ShipWidth/2, Y + ShipHeight/2), // Левая задняя точка
                new PointF(X + ShipWidth/2, Y + ShipHeight/2)  // Правая задняя точка
            };

            // Кабина (полукруг)
            RectangleF cockpitRect = new RectangleF(
                X - CockpitSize / 2,
                Y - ShipHeight / 2 - CockpitSize / 4, // немного подняли кабину
                CockpitSize,
                CockpitSize);

            // Двигатели (задние прямоугольники)
            RectangleF leftEngine = new RectangleF(
                X - ShipWidth / 3,
                Y + ShipHeight / 2,
                EngineSize / 2,
                EngineSize);

            RectangleF rightEngine = new RectangleF(
                X + ShipWidth / 3 - EngineSize / 2,
                Y + ShipHeight / 2,
                EngineSize / 2,
                EngineSize);

            // Центральный двигатель
            RectangleF centerEngine = new RectangleF(
                X - EngineSize / 3,
                Y + ShipHeight / 2,
                EngineSize * 2 / 3,
                EngineSize);

            // Рисуем корпус
            g.FillPolygon(new SolidBrush(Color.FromArgb(0, 255, 200)), hull);
            g.DrawPolygon(Pens.White, hull);

            // Рисуем кабину
            g.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 200)), cockpitRect);
            g.DrawEllipse(Pens.White, cockpitRect);

            // Рисуем двигатели
            g.FillRectangle(Brushes.DarkSlateGray, leftEngine);
            g.FillRectangle(Brushes.DarkSlateGray, rightEngine);
            g.FillRectangle(Brushes.DarkSlateGray, centerEngine);

            // Огни двигателей (анимация)
            if (DateTime.Now.Millisecond % 500 < 250)
            {
                PointF[] leftFlame = new PointF[3]
                {
                    new PointF(X - ShipWidth/3 + EngineSize/4, Y + ShipHeight/2 + EngineSize),
                    new PointF(X - ShipWidth/3, Y + ShipHeight/2 + EngineSize + 5), // уменьшена длина пламени
                    new PointF(X - ShipWidth/3 + EngineSize/2, Y + ShipHeight/2 + EngineSize + 5)
                };

                PointF[] rightFlame = new PointF[3]
                {
                    new PointF(X + ShipWidth/3 - EngineSize/4, Y + ShipHeight/2 + EngineSize),
                    new PointF(X + ShipWidth/3 - EngineSize/2, Y + ShipHeight/2 + EngineSize + 5),
                    new PointF(X + ShipWidth/3, Y + ShipHeight/2 + EngineSize + 5)
                };

                PointF[] centerFlame = new PointF[3]
                {
                    new PointF(X, Y + ShipHeight/2 + EngineSize),
                    new PointF(X - EngineSize/3, Y + ShipHeight/2 + EngineSize + 8), // уменьшена длина пламени
                    new PointF(X + EngineSize/3, Y + ShipHeight/2 + EngineSize + 8)
                };

                g.FillPolygon(Brushes.OrangeRed, leftFlame);
                g.FillPolygon(Brushes.OrangeRed, rightFlame);
                g.FillPolygon(Brushes.OrangeRed, centerFlame);
            }
        }
    }
}