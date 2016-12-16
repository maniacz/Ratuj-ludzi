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

using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Ratuj_ludzi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool humanCaptured = false;

        public MainWindow()
        {
            InitializeComponent();

            enemyTimer.Tick += enemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += targetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);
        }

        void targetTimer_Tick(object sender, EventArgs e)
        {
            pbrProgressBar.Value += 1;
            if (pbrProgressBar.Value >= pbrProgressBar.Maximum)
                EndTheGame();
        }

        private void EndTheGame()
        {
            if (!cnvPlayArea.Children.Contains(tblGameOver))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                btnStartButton.Visibility = Visibility.Visible;
                cnvPlayArea.Children.Add(tblGameOver);
            }
        }

        void enemyTimer_Tick(object sender, EventArgs e)
        {
            AddEnemy();
        }

        private void btnStartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            stpHuman.IsHitTestVisible = true;
            humanCaptured = false;
            pbrProgressBar.Value = 0;
            btnStartButton.Visibility = Visibility.Collapsed;
            cnvPlayArea.Children.Clear();
            cnvPlayArea.Children.Add(rctTarget);
            cnvPlayArea.Children.Add(stpHuman);
            enemyTimer.Start();
            targetTimer.Start();
        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            AnimateEnemy(enemy, 0, cnvPlayArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)cnvPlayArea.ActualHeight - 100),
                random.Next((int)cnvPlayArea.ActualHeight - 100), "(Canvas.Top)");
            cnvPlayArea.Children.Add(enemy);

            enemy.MouseEnter += enemy_MouseEnter;
        }

        void enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
                EndTheGame();
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6))),
            };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void stpHuman_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                stpHuman.IsHitTestVisible = false;
            }
        }

        private void rctTarget_MouseEnter(object sender, MouseEventArgs e)
        {
            if (targetTimer.IsEnabled && humanCaptured)
            {
                pbrProgressBar.Value = 0;
                Canvas.SetLeft(rctTarget, random.Next(100, (int)cnvPlayArea.ActualWidth - 100));
                Canvas.SetTop(rctTarget, random.Next(100, (int)cnvPlayArea.ActualHeight - 100));
                Canvas.SetLeft(stpHuman, random.Next(100, (int)cnvPlayArea.ActualWidth - 100));
                Canvas.SetTop(stpHuman, random.Next(100, (int)cnvPlayArea.ActualHeight - 100));
                humanCaptured = false;
                stpHuman.IsHitTestVisible = true;
            }
        }

        private void cnvPlayArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(cnvPlayArea).Transform(pointerPosition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(stpHuman)) > stpHuman.ActualWidth * 3)
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(stpHuman)) > stpHuman.ActualHeight * 3))
                {
                    humanCaptured = false;
                    stpHuman.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(stpHuman, relativePosition.X - stpHuman.ActualWidth / 2);
                    Canvas.SetTop(stpHuman, relativePosition.Y - stpHuman.ActualWidth / 2);
                }
            }
        }

        private void cnvPlayArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
                EndTheGame();
        }
    }
}
