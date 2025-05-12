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
            // �������� �������� �����������
            try
            {
                backgroundImage = Image.FromFile("Image/������.jpg");
            }
            catch
            {
                backgroundImage = null;
            }

            player = new PlayerShip(ClientSize.Width / 2, ClientSize.Height / 2);
            balls = new List<Ball>();
            bullets = new List<Bullet>();
            lasers = new List<LaserBeam>();
            random = new Random();
            score = 0;
            level = 1;
            ammo = 7;
            laserAmmo = 5;
            gameOver = false;
            laserMode = false;
            uiFont = new Font("Arial", 12, FontStyle.Bold);
            gameOverFont = new Font("Arial", 48, FontStyle.Bold);

            // ������������� ������� ��������
            InitializeBackgroundObjects();

            // ������� ��������� ���
            balls.Add(new Ball(random.Next(50, ClientSize.Width - 50),
                     random.Next(50, ClientSize.Height - 50),
                     40, 3, Color.Red));

            gameTimer.Start();
        }

        private void InitializeBackgroundObjects()
        {
            backgroundObjects = new List<BackgroundObject>();
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

            // ������ ���
            g.Clear(Color.Black);

            // ������ ������� ����������� (���� ����)
            if (backgroundImage != null)
            {
                g.DrawImage(backgroundImage, 0, 0, ClientSize.Width, ClientSize.Height);
            }

            // ������ ������� �������
            foreach (var obj in backgroundObjects)
            {
                obj.Draw(g);
            }

            // ������ �������� ����
            foreach (var laser in lasers)
            {
                laser.Draw(g);
            }

            // ������ ����
            foreach (var bullet in bullets)
            {
                bullet.Draw(g);
            }

            // ������ ����
            foreach (var ball in balls)
            {
                ball.Draw(g);
            }

            // ������ ������
            player.Draw(g);

            // ������ UI
            DrawUI(g);

            // ������ ��������� � ���������
            if (gameOver)
            {
                DrawGameOver(g);
            }
        }

        private void DrawUI(Graphics g)
        {
            string uiText = $"�������: {level}  ����: {score}  " +
                          (laserMode ? $"������: {laserAmmo}" : $"�������: {ammo}");
            g.DrawString(uiText, uiFont, Brushes.White, 10, 10);
        }

        private void DrawGameOver(Graphics g)
        {
            string gameOverText = "��������";
            SizeF textSize = g.MeasureString(gameOverText, gameOverFont);

            // ���� ������
            g.DrawString(gameOverText, gameOverFont, Brushes.Black,
                (ClientSize.Width - textSize.Width) / 2 + 2,
                (ClientSize.Height - textSize.Height) / 2 + 2);

            // �������� �����
            g.DrawString(gameOverText, gameOverFont, Brushes.Red,
                (ClientSize.Width - textSize.Width) / 2,
                (ClientSize.Height - textSize.Height) / 2);

            // ����������
            string restartText = "������� R ��� ��������";
            SizeF restartSize = g.MeasureString(restartText, uiFont);
            g.DrawString(restartText, uiFont, Brushes.White,
                (ClientSize.Width - restartSize.Width) / 2,
                (ClientSize.Height - textSize.Height) / 2 + textSize.Height + 10);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            // ��������� ������� �������
            UpdateBackgroundObjects();

            // ��������� ������� ������
            player.UpdatePosition(PointToClient(Cursor.Position));

            // ��������� ����
            UpdateBullets();

            // ��������� ������
            UpdateLasers();

            // ��������� ����
            UpdateBalls();

            // ��������� ������������
            CheckCollisions();

            // ��������� ���������� ������
            CheckLevelCompletion();

            // ��������� ��������� �����������
            CheckAmmo();

            Invalidate();
        }

        private void UpdateBackgroundObjects()
        {
            foreach (var obj in backgroundObjects)
            {
                obj.Update(ClientSize.Width, ClientSize.Height);
            }
        }

        private void UpdateBullets()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();
                if (bullets[i].IsOutOfBounds(ClientSize))
                {
                    bullets.RemoveAt(i);
                }
            }
        }

        private void UpdateLasers()
        {
            for (int i = lasers.Count - 1; i >= 0; i--)
            {
                lasers[i].Update();
                if (lasers[i].IsComplete)
                {
                    lasers.RemoveAt(i);
                }
            }
        }

        private void UpdateBalls()
        {
            foreach (var ball in balls)
            {
                ball.Update(ClientSize.Width, ClientSize.Height);
            }
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
                        CreateFireworkEffect(balls[j]);
                        score += (int)(150 / balls[j].Radius);
                        balls.RemoveAt(j);
                        break;
                    }
                }
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

        private void CreateFireworkEffect(Ball ball)
        {
            for (int i = 0; i < 15; i++)
            {
                int particleSize = random.Next(2, 6);
                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float speed = 2 + random.Next(0, 5);

                Color color = Color.FromArgb(
                    random.Next(200, 255),
                    random.Next(100, 200),
                    random.Next(100, 200));

                PointF velocity = new PointF(
                    (float)(Math.Cos(angle) * speed),
                    (float)(Math.Sin(angle) * speed));

                lasers.Add(new LaserParticle(
                    ball.X, ball.Y,
                    particleSize,
                    color,
                    velocity));
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
            // ���������, ��� ���� ��������� � ������ ������� R
            if (gameOver && e.KeyCode == Keys.R)
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