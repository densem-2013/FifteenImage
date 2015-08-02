using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Animation;


namespace FifteenImage
{
    
    public partial class MainWindow
    {
        Point zero_position,but_position;
        List<Button> buttons;
        IDA ida;
        Window ThisWindow;
        int[] targetStat;
        public void Initial()
        {

            ThisWindow = this;
            buttons = new List<Button>(16);
            targetStat = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            ida = new IDA(targetStat);
            for (int i = 0; i < 16; i++)
            {
                Button bt = new Button();
                //bt.Template = Resources["BT"] as ControlTemplate;
                bt.Click += button_Click;
                buttons.Add(bt);
                grid1.Children.Add(bt);
            }
            PlaceCells(targetStat);
            SetBackground();
            progressBar1.Value = CalcProgrValue();
         }
        private void SetBackground() 
        {
            for (int i = 0; i < 16; i++)
            {
                ImageBrush ibr = new ImageBrush();
                ibr.ImageSource = helpRes.ImSource;
                ibr.Viewbox = new Rect(i % 4 * 0.25, i / 4 * 0.25, 0.25, 0.25);
                buttons[i].Background = ibr;
            }
        }
        private void OnChangeText(object sender, SearchEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
                (ThreadStart)delegate()
            {
                Tblock.Text = e.Mes;
            }
              ,DispatcherPriority.Normal  );

        }
        private void OnFinSolve(object sender, SearchEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
                (ThreadStart)delegate()
                {
                    this.ShowSolve.Visibility = Visibility.Visible;
                }
              , DispatcherPriority.Normal);
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            zero_position.X = (int)buttons[15].GetValue(Grid.ColumnProperty) ;
            zero_position.Y = (int)buttons[15].GetValue(Grid.RowProperty) ;
            but_position.X = (int)but.GetValue(Grid.ColumnProperty);
            but_position.Y = (int)but.GetValue(Grid.RowProperty);
            if (Math.Abs(but_position.X - zero_position.X) + Math.Abs(but_position.Y - zero_position.Y) == 1)
            {
                buttons[15].SetValue(Grid.RowProperty, (int)but_position.Y);
                buttons[15].SetValue(Grid.ColumnProperty, (int)but_position.X);
                but.SetValue(Grid.RowProperty, (int)zero_position.Y);
                but.SetValue(Grid.ColumnProperty, (int)zero_position.X);
                int null_pos = (int)(zero_position.Y * 4 + zero_position.X);
                int clik_pos = (int)(but_position.Y * 4 + but_position.X);
                ida._start[null_pos] = ida._start[clik_pos];
                ida._start[clik_pos] = 0;
                this.progressBar1.Value = CalcProgrValue();
            }
            Count_pos++;
        }

        private int CalcProgrValue()
        {
            int val = 0;
            for (int i = 0; i < 16; i++)
            {
                if (targetStat[i] == ida._start[i]) val++;
            }
            return val;
        }
       private void PlaceCells(int[] val) 
        {
            for (int i = 0; i < val.Length;i++ )
            {
                buttons[val[i]].SetValue(Grid.RowProperty, (int)i / 4);
                buttons[val[i]].SetValue(Grid.ColumnProperty, (int)i % 4);
            }
        }
       private void btnSpinner_Loaded(object sender, RoutedEventArgs e)
       {
           DoubleAnimation dblAnim = new DoubleAnimation();
           dblAnim.From = 1.0;
           dblAnim.To = 0.0;

           // Reverse when done.
           dblAnim.AutoReverse = true;

           // Loop forever.
           dblAnim.RepeatBehavior = RepeatBehavior.Forever;
           buttons[15].BeginAnimation(Button.OpacityProperty, dblAnim);
       }


        
    }
}
