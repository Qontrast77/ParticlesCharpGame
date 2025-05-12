using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SpaceBallCrusher1.Entities;

namespace SpaceBallCrusher1
{
    public partial class Form1 : Form
    {
        private PlayerShip player;
        private List<Ball> balls;
        private List<Bullet> bullets;
        private List<LaserBeam> lasers;
        private List<ExplosionParticle> explosionParticles;
        private List<BackgroundObject> backgroundObjects;
        private Random random;
        private int score;
        private int level;
        private int ammo;
        private int laserAmmo;
        private bool gameOver;
        private bool laserMode;
        private Font uiFont;
        private Font gameOverFont;
        private Image backgroundImage;

        private const int StarCount = 150;
        private const int AsteroidCount = 30;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            InitializeGame();
        }

        private void InitializeGame()
        {
            try
            {
                backgroundImage = Image.FromFile("Image/космос.jpg");
            }
            catch
            {
                backgroundImage = null;
            }

            player = new PlayerShip(ClientSize.Width / 2, ClientSize.Height / 2);
            balls = new List<Ball>();
            bullets = new List<Bullet>();
            lasers = new List<LaserBeam>();
            explosionParticles = new List<ExplosionParticle>();
            backgroundObjects = new List<BackgroundObject>();
            random = new Random();
            score = 0;
            level = 1;
            ammo = 7;
            laserAmmo = 1;
            gameOver = false;
            laserMode = false;
            uiFont = new Font("Arial", 12, FontStyle.Bold);
            gameOverFont = new Font("Arial", 48, FontStyle.Bold);

            InitializeBackgroundObjects();

            balls.Add(new Ball(random.Next(50, ClientSize.Width - 50),
                     random.Next(50, ClientSize.Height - 50),
                     40, 3, Color.Red));

            gameTimer.Start();
        }

        private void InitializeBackgroundObjects()
        {
            for (int i = 0; i < StarCount; i++)
            {
                backgroundObjects.Add(new Star(ClientSize.Width, ClientSize.Height));
            }
            for (int i = 0; i < AsteroidCount; i++)
            {
                backgroundObjects.Add(new Asteroid(ClientSize.Width, ClientSize.Height));
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            if (backgroundImage != null)
            {
                g.DrawImage(backgroundImage, 0, 0, ClientSize.Width, ClientSize.Height);
            }

            foreach (var obj in backgroundObjects)
            {
                obj.Draw(g);
            }

            foreach (var particle in explosionParticles)
            {
                particle.Draw(g);
            }

            foreach (var laser in lasers)
            {
                laser.Draw(g);
            }

            foreach (var bullet in bullets)
            {
                bullet.Draw(g);
            }

            foreach (var ball in balls)
            {
                ball.Draw(g);
            }

            player.Draw(g);
            DrawUI(g);

            if (gameOver)
            {
                DrawGameOver(g);
            }
        }

        private void DrawUI(Graphics g)
        {
            string uiText = $"Уровень: {level}  Очки: {score}  " +
                          (laserMode ? $"Лазеры: {laserAmmo}" : $"Патроны: {ammo}");
            g.DrawString(uiText, uiFont, Brushes.White, 10, 10);
        }

        private void DrawGameOver(Graphics g)
        {
            string gameOverText = "Проиграл";
            SizeF textSize = g.MeasureString(gameOverText, gameOverFont);

            g.DrawString(gameOverText, gameOverFont, Brushes.Black,
                (ClientSize.Width - textSize.Width) / 2 + 2,
                (ClientSize.Height - textSize.Height) / 2 + 2);

            g.DrawString(gameOverText, gameOverFont, Brushes.Red,
                (ClientSize.Width - textSize.Width) / 2,
                (ClientSize.Height - textSize.Height) / 2);

            string restartText = "Нажмите R для рестарта";
            SizeF restartSize = g.MeasureString(restartText, uiFont);
            g.DrawString(restartText, uiFont, Brushes.White,
                (ClientSize.Width - restartSize.Width) / 2,
                (ClientSize.Height - textSize.Height) / 2 + textSize.Height + 10);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            // Обновление частиц взрыва
            for (int i = explosionParticles.Count - 1; i >= 0; i--)
            {
                explosionParticles[i].Update();
                if (explosionParticles[i].IsDead)
                {
                    explosionParticles.RemoveAt(i);
                }
            }

            foreach (var obj in backgroundObjects)
            {
                obj.Update(ClientSize.Width, ClientSize.Height);
            }

            player.UpdatePosition(PointToClient(Cursor.Position));

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();
                if (bullets[i].IsOutOfBounds(ClientSize))
                {
                    bullets.RemoveAt(i);
                }
            }

            for (int i = lasers.Count - 1; i >= 0; i--)
            {
                lasers[i].Update();
                if (lasers[i].IsComplete)
                {
                    lasers.RemoveAt(i);
                }
            }

            foreach (var ball in balls)
            {
                ball.Update(ClientSize.Width, ClientSize.Height);
            }

            CheckCollisions();
            CheckLevelCompletion();
            CheckAmmo();

            Invalidate();
        }

        private void CheckCollisions()
        {
            CheckBulletCollisions();
            CheckLaserCollisions();
        }

        private void CheckBulletCollisions()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = balls.Count - 1; j >= 0; j--)
                {
                    if (bullets[i].CollidesWith(balls[j]))
                    {
                        score += (int)(100 / balls[j].Radius);

                        if (balls[j].Radius > 10)
                        {
                            SplitBall(balls[j]);
                        }

                        balls.RemoveAt(j);
                        bullets.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void CheckLaserCollisions()
        {
            for (int i = lasers.Count - 1; i >= 0; i--)
            {
                for (int j = balls.Count - 1; j >= 0; j--)
                {
                    if (lasers[i].CollidesWith(balls[j]))
                    {
                        CreateExplosionEffect(balls[j]);
                        score += (int)(150 / balls[j].Radius);
                        balls.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        private void CreateExplosionEffect(Ball ball)
        {
            // Увеличиваем количество частиц
            int particlesCount = 50 + random.Next(20); // Больше частиц
            float explosionPower = 3 + level * 0.5f;

            for (int i = 0; i < particlesCount; i++)
            {
                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float speed = 0.5f + (float)random.NextDouble() * explosionPower;
                int size = 3 + random.Next(5); // Увеличиваем размер частиц

                // Яркие цвета, случайные оттенки, чтобы частицы выглядели ярче
                Color color = Color.FromArgb(
                    Math.Clamp(ball.Color.R + random.Next(-30, 30), 0, 255),
                    Math.Clamp(ball.Color.G + random.Next(-30, 30), 0, 255),
                    Math.Clamp(ball.Color.B + random.Next(-30, 30), 0, 255));

                // Добавляем новые частицы
                explosionParticles.Add(new ExplosionParticle(
                    ball.X, ball.Y,
                    size,
                    color,
                    angle,
                    speed,
                    40 + random.Next(20))); // Увеличиваем продолжительность жизни частиц
            }
        }


        private void SplitBall(Ball ball)
        {
            int newRadius = (int)(ball.Radius / 2);
            if (newRadius < 5) return;

            for (int i = 0; i < 2; i++)
            {
                int offsetX = random.Next(-20, 20);
                int offsetY = random.Next(-20, 20);

                Color newColor = Color.FromArgb(
                    ball.Color.R,
                    Math.Min(ball.Color.G + 50, 255),
                    Math.Min(ball.Color.B + 50, 255));

                balls.Add(new Ball(
                    (int)(ball.X + offsetX),
                    (int)(ball.Y + offsetY),
                    newRadius,
                    ball.Speed,
                    newColor));
            }
        }

        private void CheckLevelCompletion()
        {
            if (balls.Count == 0)
            {
                level++;
                ammo = 7 + (level - 1) * 7;
                laserAmmo = 1 + (level - 1);
                SpawnBallsForLevel();
            }
        }

        private void CheckAmmo()
        {
            if ((ammo <= 0 && !laserMode && bullets.Count == 0 && balls.Count > 0) ||
                (laserAmmo <= 0 && laserMode && lasers.Count == 0 && balls.Count > 0))
            {
                gameOver = true;
                gameTimer.Stop();
            }
        }

        private void SpawnBallsForLevel()
        {
            for (int i = 0; i < level; i++)
            {
                balls.Add(new Ball(
                    random.Next(50, ClientSize.Width - 50),
                    random.Next(50, ClientSize.Height - 50),
                    40,
                    3,
                    Color.FromArgb(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255))));
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (gameOver) return;

            if (e.Button == MouseButtons.Left)
            {
                if (!laserMode && ammo > 0)
                {
                    bullets.Add(new Bullet(player.X, player.Y, PointToClient(Cursor.Position)));
                    ammo--;
                }
                else if (laserMode && laserAmmo > 0)
                {
                    lasers.Add(new LaserBeam(
                        player.X, player.Y,
                        PointToClient(Cursor.Position),
                        Color.FromArgb(255, 50, 50)));
                    laserAmmo--;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && gameOver)
            {
                InitializeGame();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            laserMode = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            laserMode = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (backgroundObjects != null && backgroundObjects.Count > 0)
            {
                InitializeBackgroundObjects();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}