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
        private List<AmmoPickup> ammoPickups;
        private List<DangerousAsteroid> dangerousAsteroids;
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
        private const int AmmoPickupsCount = 2;
        private const int DangerousAsteroidsCount = 3;

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
            ammoPickups = new List<AmmoPickup>();
            dangerousAsteroids = new List<DangerousAsteroid>();
            random = new Random();
            score = 0;
            level = 1;
            ammo = 7;
            laserAmmo = 5;
            gameOver = false;
            laserMode = false;
            uiFont = new Font("Arial", 12, FontStyle.Bold);
            gameOverFont = new Font("Arial", 48, FontStyle.Bold);

            InitializeBackgroundObjects();
            SpawnAmmoPickups();
            SpawnDangerousAsteroids();

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

        private void SpawnAmmoPickups()
        {
            for (int i = 0; i < AmmoPickupsCount; i++)
            {
                ammoPickups.Add(new AmmoPickup(
                    random.Next(50, ClientSize.Width - 50),
                    random.Next(50, ClientSize.Height - 50),
                    15,
                    Color.LimeGreen));
            }
        }

        private void SpawnDangerousAsteroids()
        {
            for (int i = 0; i < DangerousAsteroidsCount; i++)
            {
                dangerousAsteroids.Add(new DangerousAsteroid(
                    random.Next(50, ClientSize.Width - 50),
                    random.Next(50, ClientSize.Height - 50),
                    25,
                    2,
                    Color.DarkRed));
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

            foreach (var ammoPickup in ammoPickups)
            {
                ammoPickup.Draw(g);
            }

            foreach (var asteroid in dangerousAsteroids)
            {
                asteroid.Draw(g);
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

            // Обновление фоновых объектов
            foreach (var obj in backgroundObjects)
            {
                obj.Update(ClientSize.Width, ClientSize.Height);
            }

            // Обновление патронов для сбора
            foreach (var ammoPickup in ammoPickups)
            {
                ammoPickup.Update(ClientSize.Width, ClientSize.Height);
            }

            // Обновление опасных метеоритов
            foreach (var asteroid in dangerousAsteroids)
            {
                asteroid.Update(ClientSize.Width, ClientSize.Height);
            }

            // Обновление позиции игрока
            player.UpdatePosition(PointToClient(Cursor.Position));

            // Обновление пуль
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();
                if (bullets[i].IsOutOfBounds(ClientSize))
                {
                    bullets.RemoveAt(i);
                }
            }

            // Обновление лазеров
            for (int i = lasers.Count - 1; i >= 0; i--)
            {
                lasers[i].Update();
                if (lasers[i].IsComplete)
                {
                    lasers.RemoveAt(i);
                }
            }

            // Обновление шаров
            foreach (var ball in balls)
            {
                ball.Update(ClientSize.Width, ClientSize.Height);
            }

            // Проверка столкновений
            CheckCollisions();
            CheckAmmoPickupCollisions();
            CheckDangerousCollisions();
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
                    if (BulletHitsBall(bullets[i], balls[j]))
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

        private bool BulletHitsBall(Bullet bullet, Ball ball)
        {
            float dx = bullet.X - ball.X;
            float dy = bullet.Y - ball.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            return distance < (bullet.Size / 2 + ball.Radius);
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

        private void CheckAmmoPickupCollisions()
        {
            for (int i = ammoPickups.Count - 1; i >= 0; i--)
            {
                if (PlayerHitsAmmoPickup(player, ammoPickups[i]))
                {
                    if (laserMode)
                        laserAmmo += 3;
                    else
                        ammo += 5;

                    ammoPickups.RemoveAt(i);
                    continue;
                }

                if (ammoPickups[i].IsOutOfBounds(ClientSize))
                {
                    ammoPickups.RemoveAt(i);
                }
            }

            if (ammoPickups.Count < AmmoPickupsCount)
            {
                SpawnAmmoPickups();
            }
        }

        private bool PlayerHitsAmmoPickup(PlayerShip player, AmmoPickup ammoPickup)
        {
            float dx = player.X - ammoPickup.X;
            float dy = player.Y - ammoPickup.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            return distance < (player.Size / 2 + ammoPickup.Radius);
        }

        private void CheckDangerousCollisions()
        {
            foreach (var asteroid in dangerousAsteroids)
            {
                if (PlayerHitsAsteroid(player, asteroid))
                {
                    gameOver = true;
                    gameTimer.Stop();
                    break;
                }
            }
        }

        private bool PlayerHitsAsteroid(PlayerShip player, DangerousAsteroid asteroid)
        {
            float dx = player.X - asteroid.X;
            float dy = player.Y - asteroid.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            return distance < (player.Size / 2 + asteroid.Radius);
        }

        private bool IsColliding(SpaceObject obj1, SpaceObject obj2)
        {
            float dx = obj1.X - obj2.X;
            float dy = obj1.Y - obj2.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            return distance < (obj1.Radius + obj2.Radius);
        }

        private void CreateExplosionEffect(Ball ball)
        {
            int particlesCount = 20 + random.Next(10);
            float explosionPower = 3 + level * 0.5f;

            for (int i = 0; i < particlesCount; i++)
            {
                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float speed = 0.5f + (float)random.NextDouble() * explosionPower;
                int size = 2 + random.Next(4);

                Color color = Color.FromArgb(
                    Math.Clamp(ball.Color.R + random.Next(-40, 40), 0, 255),
                    Math.Clamp(ball.Color.G + random.Next(-40, 40), 0, 255),
                    Math.Clamp(ball.Color.B + random.Next(-40, 40), 0, 255));

                explosionParticles.Add(new ExplosionParticle(
                    ball.X, ball.Y,
                    size,
                    color,
                    angle,
                    speed,
                    30 + random.Next(30)));
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
                laserAmmo = 5 + level;
                SpawnBallsForLevel();
                SpawnDangerousAsteroids();
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