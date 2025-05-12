using System;
using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public class ExplosionParticle
    {
        // Позиция частиц
        public float X { get; set; }
        public float Y { get; set; }

        // Цвет частицы
        public Color Color { get; set; }

        // Скорость частицы
        private PointF velocity;

        // Размер частицы
        private int size;

        // Время жизни частицы
        private int maxLifetime;
        public int Lifetime { get; private set; }

        // Дополнительные параметры для эффекта взрыва
        private float angle;  // угол
        private float speed;  // скорость

        // Конструктор с 7 параметрами
        public ExplosionParticle(float x, float y, int size, Color color, float angle, float speed, int lifetime)
        {
            this.X = x;
            this.Y = y;
            this.size = size;
            this.Color = color;
            this.angle = angle;
            this.speed = speed;
            this.maxLifetime = lifetime;
            this.Lifetime = lifetime;
            this.velocity = new PointF((float)(Math.Cos(angle) * speed), (float)(Math.Sin(angle) * speed));
        }

        // Метод обновления позиции частицы
        public void Update()
        {
            // Двигаем частицу
            X += velocity.X;
            Y += velocity.Y;

            // Замедляем частицу (уменьшаем скорость)
            velocity.X *= 0.95f;
            velocity.Y *= 0.95f;

            // Уменьшаем время жизни
            Lifetime--;
        }

        // Метод для рисования частицы
        public void Draw(Graphics g)
        {
            if (Lifetime <= 0) return; // Если частица "умерла", не рисуем ее

            // Рассчитываем прозрачность в зависимости от оставшегося времени жизни
            float alpha = (Lifetime / (float)maxLifetime) * 255;
            Color fadeColor = Color.FromArgb((int)alpha, Color);

            using (Brush brush = new SolidBrush(fadeColor))
            {
                g.FillEllipse(brush, X - size / 2, Y - size / 2, size, size);
            }
        }

        // Свойство для проверки, "умерла ли" частица
        public bool IsDead => Lifetime <= 0;
    }
}
