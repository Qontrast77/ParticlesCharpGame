using System.Drawing;

namespace SpaceBallCrusher1.Entities
{
    public abstract class SpaceObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public Color Color { get; set; }
        protected float Speed { get; set; }
        protected float dx, dy;

        public SpaceObject(float x, float y, float radius, Color color)
        {
            X = x;
            Y = y;
            Radius = radius;
            Color = color;
        }

        public abstract void Update(int width, int height);
        public abstract void Draw(Graphics g);

        public bool IsOutOfBounds(Size bounds)
        {
            return X < -Radius || X > bounds.Width + Radius ||
                   Y < -Radius || Y > bounds.Height + Radius;
        }
    }
}