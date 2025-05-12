using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public abstract class BackgroundObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Speed { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }

        public abstract void Draw(Graphics g);

        public virtual void Update(int width, int height)
        {
            X += Speed;

            // Если объект вылетел за границы, перемещаем его в начало
            if (X > width)
            {
                X = -Size;
                Y = new Random().Next(0, height);
            }
        }
    }
}