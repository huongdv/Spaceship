using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;

namespace WpfGameMini
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int mTimer = 20;
        int mTimerCounter = 0;
        int mLimitTimerCounter = 50;
        int mPlayerSpeed = 10;
        int mSpaceshipSpeed = 5;
        int mCloudsSpeed = 2;
        int mScore = 0;
        string mTagSpaceship = "SPACESHIP";
        string mTagBullet = "BULLET";
        string mTagCloud;
        bool mMoveToLeft, mMoveToRight, mMoveToTop, mMoveToBottom;
        bool mStartGame = false;
        bool mGameOver = false;

        Rect mPlayerRec;
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> mRemoveListItem = new List<Rectangle>();

        public MainWindow()
        {
            InitializeComponent();

            mTagCloud = clouds.Tag.ToString();
            gameTimer.Interval = TimeSpan.FromMilliseconds(mTimer);
            gameTimer.Tick += gameTimerCallBack;
            MainCanvas.Focus();
            scoreText.Content = "Press 'Enter' to start game";
        }

        private void startGame()
        {
            GameReady.Visibility = System.Windows.Visibility.Hidden;
            mStartGame = true;
            mScore = 0;
            gameTimer.Start();
        }

        private void resetGame()
        {
            mGameOver = false;
            GameOver.Visibility = System.Windows.Visibility.Hidden;
            foreach (Rectangle y in mRemoveListItem)
            {
                MainCanvas.Children.Remove(y);
            }
        }

        private void onPressKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                mMoveToLeft = true;
            }
            if (e.Key == Key.Right)
            {
                mMoveToRight = true;
            }
            if (e.Key == Key.Up)
            {
                mMoveToTop = true;
            }
            if (e.Key == Key.Down)
            {
                mMoveToBottom = true;
            }
        }

        private void onPressKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && mStartGame == false)
            {
                startGame();
            }

            if (e.Key == Key.R && mGameOver == true)
            {
                resetGame();
                startGame();
            }

            if (e.Key == Key.Left)
            {
                mMoveToLeft = false;
            }
            if (e.Key == Key.Right)
            {
                mMoveToRight = false;
            }
            if (e.Key == Key.Up)
            {
                mMoveToTop = false;
            }
            if (e.Key == Key.Down)
            {
                mMoveToBottom = false;
            }

            if (e.Key == Key.Space && mStartGame == true && mGameOver == false )
            {
                playSound(1);
                Rectangle newBullet = new Rectangle
                {
                    Tag = mTagBullet,
                    Height = 10,
                    Width = 5,
                    Fill = Brushes.Orange,
                    Stroke = Brushes.Red
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                MainCanvas.Children.Add(newBullet);
            }
        }

        private void makeSpaceship()
        {
            Random random = new Random();
            ImageBrush spaceshipImage = new ImageBrush();
            int imageID = random.Next(1, 6);
            switch (imageID)
            {
                case 1:
                    spaceshipImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceship1.png"));
                    break;
                case 2:
                    spaceshipImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceship2.png"));
                    break;
                case 3:
                    spaceshipImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceship3.png"));
                    break;
                case 4:
                    spaceshipImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceship4.png"));
                    break;
                case 5:
                    spaceshipImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceship5.png"));
                    break;
                case 6:
                    spaceshipImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceship6.png"));
                    break;
                default:
                    spaceshipImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceship1.png"));
                    break;
            }

            Rectangle newSpaceship = new Rectangle
            {
                Tag = mTagSpaceship,
                Height = 50,
                Width = 50,
                Fill = spaceshipImage
            };

            Canvas.SetTop(newSpaceship, -100);
            Canvas.SetLeft(newSpaceship, random.Next(0, 320));
            MainCanvas.Children.Add(newSpaceship);
        }

        private void gameTimerCallBack(object sender, EventArgs e)
        {
            mTimerCounter++;
            mPlayerRec = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            scoreText.Content = "Score: " + mScore;
            if (mTimerCounter > mLimitTimerCounter)
            {
                makeSpaceship();
                mTimerCounter = 0;
            }

            playerMove();

            cloudsMove();

            foreach (var x in MainCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == mTagBullet)
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 10);

                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                    {
                        mRemoveListItem.Add(x);
                    }

                    foreach (var y in MainCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == mTagSpaceship)
                        {
                            Rect spaceship = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(spaceship))
                            {
                                playSound(2);
                                mRemoveListItem.Add(x);
                                mRemoveListItem.Add(y);
                                mScore++;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == mTagSpaceship)
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + mSpaceshipSpeed);
                    Rect spaceship = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (Canvas.GetTop(x) > 650)
                    {
                        mRemoveListItem.Add(x);
                    }
                    if (mPlayerRec.IntersectsWith(spaceship))
                    {
                        playSound(3);
                        mGameOver = true;
                        gameTimer.Stop();
                        scoreText.Content += "   Press R to Try Again";
                        GameOver.Visibility = System.Windows.Visibility.Visible;
                        foreach (var z in MainCanvas.Children.OfType<Rectangle>())
                        {
                            if (z is Rectangle && (string)z.Tag == mTagBullet)
                            {
                                mRemoveListItem.Add(z);
                            }
                            if (z is Rectangle && (string)z.Tag == mTagSpaceship)
                            {
                                mRemoveListItem.Add(z);
                            }
                        }
                        return;
                    }
                }
            }

            foreach (Rectangle y in mRemoveListItem)
            {
                MainCanvas.Children.Remove(y);
            }

        }

        private void playerMove()
        {
            if (mMoveToLeft && Canvas.GetLeft(player) > 0)
            {
                double left = Canvas.GetLeft(player) - mPlayerSpeed;
                if (left < 0)
                {
                    left = 0;
                }
                Canvas.SetLeft(player, left);
            }

            if (mMoveToRight && Canvas.GetLeft(player) + 70 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + mPlayerSpeed);
            }
            else if(mMoveToRight && Canvas.GetLeft(player) + 70 > Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Application.Current.MainWindow.Width - 70);
            }

            if (mMoveToTop && Canvas.GetTop(player) > 0)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) - mPlayerSpeed);
            }

            if (mMoveToBottom && Canvas.GetTop(player) + 100 < Application.Current.MainWindow.Height)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + mPlayerSpeed);
            }
        }

        private void cloudsMove()
        {
            foreach (var x in MainCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == mTagCloud)
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + mCloudsSpeed);
                    if (Canvas.GetTop(x) > 600)
                    {
                        Canvas.SetTop(x, -150);

                        Random random = new Random();
                        int left = random.Next(0, 250);
                        Canvas.SetLeft(x, left);
                    }
                }
            }
        }

        private void playSound(int kind)
        {
            SoundPlayer sound;
            switch (kind)
            {
                case 1: // ポイントを集める
                    sound = new SoundPlayer(WpfGameMini.Properties.Resources.gun_shot);
                    sound.Play();
                    break;
                case 2: // ゲームオーバー
                    sound = new SoundPlayer(WpfGameMini.Properties.Resources.hit);
                    sound.Play();
                    break;
                case 3: // ゲームオーバー
                    sound = new SoundPlayer(WpfGameMini.Properties.Resources.game_over);
                    sound.Play();
                    break;
                default:
                    break;
            }
        }
    }
}
