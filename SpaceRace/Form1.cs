/*
 * Koen Gerber
 * ICS3U
 * Mr T
 * May 18, 2023
 * 
 * Space Race Summative
 */

using SpaceRace.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceRace
{
    public partial class Form1 : Form
    {
        List<Rectangle> asteroidList = new List<Rectangle>();
        List<Rectangle> starList = new List<Rectangle>();
        List<int> asteroidSpeedList = new List<int>();
        List<Color> starColorList = new List<Color>();

        bool wDown = false;
        bool sDown = false;
        bool upDown = false;
        bool downDown = false;

        int playerSpeed = 10;
        int playerHeight = 30;
        int playerWidth = 20;

        int player1Score = 0;
        int player2Score = 0;

        int timer;

        bool player1Motion = false;
        bool player2Motion = false;
        bool player1Up = false;
        bool player2Up = false; 
        bool player1Down = false;
        bool player2Down = false;
        bool player1Explodes = false;
        bool player2Explodes = false;

        string state = "waiting";

        Random random = new Random();

        Pen whitePen = new Pen(Color.White, 10);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush orangeBrush = new SolidBrush(Color.Orange);

        Font drawFont = new Font("Cascadia Code", 20, FontStyle.Bold);

        SoundPlayer explosionSound = new SoundPlayer(Properties.Resources.explosionSound);
        SoundPlayer pointSound = new SoundPlayer(Properties.Resources.pointSound);
        Point topTimerLinePoint = new Point();
        Point bottomTimerLinePoint = new Point();

        Rectangle player1 = new Rectangle();
        Rectangle player2 = new Rectangle();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.Down:
                    downDown = true;
                    break;
                case Keys.Up:
                    upDown = true;
                    break;
                case Keys.Space:
                    //start game if in waiting or game over screen
                    if (state == "waiting" || state == "game over")
                    {
                        InitializeGame();
                    }
                    break;
                case Keys.Escape:
                    //exit game if in waiting or game over screen
                    if (state == "waiting" || state == "game over")
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.Down:
                    downDown = false;
                    break;
                case Keys.Up:
                    upDown = false;
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (state == "waiting")
            {
                titleLabel.Text = "Welcome to the Space Race";
                subtitleLabel.Text = "Press Space to Begin or Esc to Exit";
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
            }
            else if (state == "playing")
            {
                //draw stars
                for (int i = 1; i < starList.Count; i++)
                {
                    SolidBrush starBrush = new SolidBrush(starColorList[i]);
                    e.Graphics.FillEllipse(starBrush, starList[i]);
                }

                titleLabel.Visible = false;
                subtitleLabel.Visible = false;

                //timer line
                topTimerLinePoint = new Point(this.Width /2, this.Height - timer / 4);
                bottomTimerLinePoint = new Point(this.Width/2, this.Height);
                e.Graphics.DrawLine(whitePen, topTimerLinePoint, bottomTimerLinePoint);

                //draw score
                e.Graphics.DrawString($"{player1Score}", drawFont, blueBrush, this.Width/2 - 40, this.Height - 30);
                e.Graphics.DrawString($"{player2Score}", drawFont, redBrush, this.Width/2 + 10, this.Height - 30);

                //player 1
                if (player1Up == true) //player is upright
                {
                    if (player1Motion == true) // if moving draw fire
                    {
                        e.Graphics.FillPie(orangeBrush, player1.X + playerWidth / 4, player1.Y + (playerHeight / 4) * 3, playerWidth/2, (playerHeight / 2) * 3, 240, 60);
                        player1Motion = false;                     
                    }

                    e.Graphics.FillRectangle(blueBrush, player1.X, player1.Y + playerHeight / 2, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillRectangle(blueBrush, player1.X + (playerWidth / 4) * 3, player1.Y + playerHeight / 2, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillEllipse(whiteBrush, player1.X, player1.Y, playerWidth, (playerHeight / 6) * 5);
                    e.Graphics.FillPie(blueBrush, player1.X, player1.Y, playerWidth, (playerHeight / 6) * 5, 180, 180);
                }
                if (player1Down == true) //player is upside down
                {
                    if (player1Motion == true) //if moving draw fire
                    {
                        e.Graphics.FillPie(orangeBrush, player1.X + playerWidth / 4, player1.Y - playerHeight, playerWidth/2, (playerHeight / 2) * 3, 60, 60);
                        player1Motion = false;
                    }

                    e.Graphics.FillRectangle(blueBrush, player1.X, player1.Y, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillRectangle(blueBrush, player1.X + (playerWidth / 4) * 3, player1.Y, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillEllipse(whiteBrush, player1.X, player1.Y + playerHeight / 6, playerWidth, (playerHeight / 6) * 5);
                    e.Graphics.FillPie(blueBrush, player1.X, player1.Y + playerHeight / 6, playerWidth, (playerHeight / 6) * 5, 0, 180);
                }

                //player 2
                if (player2Up == true) //player is upright
                {
                    if (player2Motion == true) //if moving draw fire
                    {
                        //fire
                        e.Graphics.FillPie(orangeBrush, player2.X + playerWidth / 4, player2.Y + (playerHeight / 4) * 3, playerWidth/2, (playerHeight / 2) * 3, 240, 60);
                        player2Motion = false;
                    }

                    e.Graphics.FillRectangle(redBrush, player2.X, player2.Y + playerHeight / 2, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillRectangle(redBrush, player2.X + (playerWidth / 4) * 3, player2.Y + playerHeight / 2, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillEllipse(whiteBrush, player2.X, player2.Y, playerWidth, (playerHeight / 6) * 5);
                    e.Graphics.FillPie(redBrush, player2.X, player2.Y, playerWidth, (playerHeight / 6) * 5, 180, 180);
                }
                if (player2Down == true) //player is upside down
                {
                    if (player2Motion == true) //if moving draw fire
                    {
                        //fire
                        e.Graphics.FillPie(orangeBrush, player2.X + playerWidth / 4, player2.Y - playerHeight, playerWidth/2, (playerHeight / 2) * 3, 60, 60);
                        player2Motion = false;
                    }

                    e.Graphics.FillRectangle(redBrush, player2.X, player2.Y, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillRectangle(redBrush, player2.X + (playerWidth / 4) * 3, player2.Y, playerWidth / 4, playerHeight / 2);
                    e.Graphics.FillEllipse(whiteBrush, player2.X, player2.Y + playerHeight / 6, playerWidth, (playerHeight / 6) * 5);
                    e.Graphics.FillPie(redBrush, player2.X, player2.Y + playerHeight / 6, playerWidth, (playerHeight / 6) * 5, 0, 180);
                }

                //explosions when player gets hit by asteroid
                if (player1Explodes == true)
                {
                    DrawExplosion(g, new Point(player1.X, player1.Y), 100);
                    player1.Y = this.Height - playerHeight;
                    player1Explodes = false;
                    explosionSound.Play();
                }
                if (player2Explodes == true)
                {
                    DrawExplosion(g, new Point(player2.X, player2.Y), 100);
                    player2.Y = this.Height - playerHeight;
                    player2Explodes = false;
                    explosionSound.Play();
                }

                //draw asteroids
                for (int i = 1; i < asteroidList.Count; i++)
                {
                    e.Graphics.FillRectangle(whiteBrush, asteroidList[i]);
                }
            }
            else if (state == "game over")
            {
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;

                //check who wins and display
                if (player1Score > player2Score)
                {
                    titleLabel.Text = "Player 1 Wins!";
                    subtitleLabel.Text = "Press Space to Play Again or Esc to Exit";
                }
                else if (player1Score < player2Score)
                {
                    titleLabel.Text = "Player 2 Wins!";
                    subtitleLabel.Text = "Press Space to Play Again or Esc to Exit";
                }
                else
                {
                    titleLabel.Text = "Tie Game!";
                    subtitleLabel.Text = "Press Space to Play Again or Esc to Exit";
                }
            }
            else if (state == "loading") //loading screen to allow asteroids to go onto the screen
            {
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
                subtitleLabel.Text = "";
                titleLabel.Text = "Loading...";

                if (timer < 0)
                {
                    state = "playing";
                    timer = this.Height * 4;
                }
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            Refresh();
            timer--;
            
            //create new asteroids
            int randNum = random.Next(1, 101);
            if (randNum > 80)
            {
                int y = random.Next(0, this.Height - 100);

                if (randNum > 90)
                {
                    Rectangle asteroid = new Rectangle(-30, y, 30, 4);
                    asteroidList.Add(asteroid);
                    asteroidSpeedList.Add(random.Next(3, 8));
                }
                else
                {
                    Rectangle asteroid = new Rectangle(this.Width, y, 30, 4);
                    asteroidList.Add(asteroid);
                    asteroidSpeedList.Add(random.Next(-8, -3));
                }
            }

            //move asteroids
            for (int i = 0; i < asteroidList.Count; i++)
            {
                int x = asteroidList[i].X + asteroidSpeedList[i];
                int y = asteroidList[i].Y;
                asteroidList[i] = new Rectangle(x, y, 30, 4);
            }

            //delete asteroids
            for (int i = 0; i < asteroidList.Count; i++)
            {
                if ((asteroidList[i].X > this.Width && asteroidSpeedList[i] == 10) || (asteroidList[i].X < -30 && asteroidSpeedList[i] == -10))
                {
                    asteroidList.RemoveAt(i);
                    asteroidSpeedList.RemoveAt(i);
                }
            }

            if (state == "playing")
            {
                //move players
                if (upDown == true)
                {
                    player2.Y -= playerSpeed;
                    player2Up = true;
                    player2Down = false;
                    player2Motion = true;
                }
                if (downDown == true && player2.Y < this.Height - playerHeight)
                {
                    player2.Y += playerSpeed;
                    player2Down = true;
                    player2Up = false;
                    player2Motion = true;
                }
                if (sDown == true && player1.Y < this.Height - playerHeight)
                {
                    player1.Y += playerSpeed;
                    player1Down = true;
                    player1Up = false;
                    player1Motion = true;
                }
                if (wDown == true)
                {
                    player1.Y -= playerSpeed;
                    player1Up = true;
                    player1Down = false;
                    player1Motion = true;
                }

                //check if player reaches the top
                if (player1.Y <= 0)
                {
                    player1.Y = this.Height - playerHeight;
                    player1Score++;
                    pointSound.Play();
                }
                if (player2.Y <= 0)
                {
                    player2.Y = this.Height - playerHeight;
                    player2Score++;
                    pointSound.Play();
                }

                //check if intersects
                for (int i = 0; i < asteroidList.Count; i++)
                {
                    if (player1.IntersectsWith(asteroidList[i]))
                    {
                        player1Explodes = true;
                    }
                    if (player2.IntersectsWith(asteroidList[i]))
                    {
                        player2Explodes = true;
                    }
                }
                
                //check if timer is done
                if (timer == 0)
                {
                    state = "game over";

                    gameTimer.Enabled = false;
                    Refresh();
                }

            }
        }

        public void InitializeGame()
        {
            //draw stars in the background
            starList.Clear();
            starColorList.Clear();
            MakeStars();
            
            player1 = new Rectangle(this.Width/4, this.Height - playerHeight, playerWidth, playerHeight);
            player2 = new Rectangle((this.Width/4)*3, this.Height - playerHeight, playerWidth, playerHeight);

            player1Up = true;
            player2Up = true;
            player1Score = 0;
            player2Score = 0;
            wDown = false;
            sDown = false;
            upDown = false;
            downDown = false;

            timer = 100;

            state = "loading";
            gameTimer.Enabled = true;
        }

        public void DrawExplosion(Graphics g, Point center, int size)
        {
            Color[] colors = {Color.Yellow, Color.Orange, Color.Red};
            Brush[] brushes = new Brush[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                brushes[i] = new SolidBrush(colors[i]);
            }

            int numCircles = 24;
            int maxRadius = size / 2;
            int minRadius = size / 8;

            for (int i = 0; i < numCircles; i++)
            {
                int currentRadius = maxRadius - ((maxRadius - minRadius) * i / numCircles);
                int currentDiameter = currentRadius * 2;
                int currentX = center.X - currentRadius;
                int currentY = center.Y - currentRadius;
                Rectangle currentRect = new Rectangle(currentX, currentY, currentDiameter, currentDiameter);

                Color shapeColor = colors[i % colors.Length];
                Brush currentBrush = brushes[i % colors.Length];

                g.FillEllipse(currentBrush, currentRect);

                int smallerRadius = currentRadius / 3;
                int smallerDiameter = smallerRadius * 2;
                int smallerX = center.X - smallerRadius;
                int smallerY = center.Y - smallerRadius;
                Rectangle smallerRect = new Rectangle(smallerX, smallerY, smallerDiameter, smallerDiameter);

                g.FillEllipse(currentBrush, smallerRect);
            }
        }

        public void MakeStars()
        {
            //create new stars
            for (int i = 1; i <= 100;  i++)
            {
                int x = random.Next(0, this.Width);
                int y = random.Next(0, this.Height);
                int size = random.Next(2, 10);

                Rectangle star = new Rectangle(x, y, size, size);
                starList.Add(star);

                int color = random.Next(1, 9);
                switch (color)
                {
                    case 1:
                        starColorList.Add(Color.White);
                        break;
                    case 2:
                        starColorList.Add(Color.LightBlue);
                        break;
                    case 3:
                        starColorList.Add(Color.LightGreen);
                        break;
                    case 4:
                        starColorList.Add(Color.LightPink);
                        break;
                    case 5:
                        starColorList.Add(Color.LightYellow);
                        break;
                    case 6:
                        starColorList.Add(Color.Orange);
                        break;
                    case 7:
                        starColorList.Add(Color.LightCyan);
                        break;
                    case 8:
                        starColorList.Add(Color.MediumPurple);
                        break;
                }
            }
        }
    }
}