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
        private Random random;
        private int score;
        private int level;
        private int ammo;
        private bool gameOver;
        private Font uiFont;
        private Font gameOverFont;
        private Image backgroundImage;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Загрузка фонового изображения
            try
            {
                backgroundImage = Image.FromFile("Image/космос.jpg");
            }
            catch
            {
                backgroundImage = null; // Если изображение не загружено, будет черный фон
            }

            player = new PlayerShip(ClientSize.Width / 2, ClientSize.Height / 2);
            balls = new List<Ball>();
            bullets = new List<Bullet>();
            random = new Random();
            score = 0;
            level = 1;
            ammo = 10;
            gameOver = false;
            uiFont = new Font("Arial", 12, FontStyle.Bold);
            gameOverFont = new Font("Arial", 48, FontStyle.Bold);

            // Создаем начальный шар
            balls.Add(new Ball(random.Next(50, ClientSize.Width - 50),
                     random.Next(50, ClientSize.Height - 50),
                     40, 3, Color.Red));

            gameTimer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем фон
            if (backgroundImage != null)
            {
                g.DrawImage(backgroundImage, 0, 0, ClientSize.Width, ClientSize.Height);
            }

            // Рисуем игрока
            player.Draw(g);

            // Рисуем шары
            foreach (var ball in balls)
            {
                ball.Draw(g);
            }

            // Рисуем пули
            foreach (var bullet in bullets)
            {
                bullet.Draw(g);
            }

            // Рисуем UI
            DrawUI(g);

            // Рисуем сообщение о проигрыше
            if (gameOver)
            {
                string gameOverText = "Проиграл";
                SizeF textSize = g.MeasureString(gameOverText, gameOverFont);

                // Рисуем текст с красной обводкой для лучшей видимости
                g.DrawString(gameOverText, gameOverFont, Brushes.Black,
                    (ClientSize.Width - textSize.Width) / 2 + 2,
                    (ClientSize.Height - textSize.Height) / 2 + 2);

                g.DrawString(gameOverText, gameOverFont, Brushes.Red,
                    (ClientSize.Width - textSize.Width) / 2,
                    (ClientSize.Height - textSize.Height) / 2);

                // Подпись для рестарта
                string restartText = "Нажмите R, чтобы начать заново";
                SizeF restartSize = g.MeasureString(restartText, uiFont);
                g.DrawString(restartText, uiFont, Brushes.White,
                    (ClientSize.Width - restartSize.Width) / 2,
                    (ClientSize.Height - textSize.Height) / 2 + textSize.Height + 10);
            }
        }

        private void DrawUI(Graphics g)
        {
            string uiText = $"Уровень: {level}  Очки: {score}  Патроны: {ammo}";
            g.DrawString(uiText, uiFont, Brushes.White, 10, 10);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            player.UpdatePosition(PointToClient(Cursor.Position));

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();

                if (bullets[i].IsOutOfBounds(ClientSize))
                {
                    bullets.RemoveAt(i);
                }
            }

            foreach (var ball in balls)
            {
                ball.Update(ClientSize.Width, ClientSize.Height);
            }

            CheckCollisions();

            if (balls.Count == 0)
            {
                level++;
                ammo = 10 + level * 2;
                SpawnBallsForLevel();
            }

            if (ammo <= 0 && bullets.Count == 0 && balls.Count > 0)
            {
                gameOver = true;
                gameTimer.Stop();
            }

            Invalidate();
        }

        private void CheckCollisions()
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

            if (e.Button == MouseButtons.Left && ammo > 0)
            {
                bullets.Add(new Bullet(player.X, player.Y, PointToClient(Cursor.Position)));
                ammo--;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && gameOver)
            {
                InitializeGame();
            }
        }
    }
}