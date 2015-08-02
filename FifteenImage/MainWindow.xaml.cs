using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Threading;

namespace FifteenImage
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer1 ;
        Res helpRes;

        public event PropertyChangedEventHandler PropertyChanged;
        //позиция текущего отображения
        int count_pos;

        public int Count_pos
        {
            get { return count_pos; }
            set
            {
                count_pos = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Count_pos"));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            helpRes = new Res();
            helpRes.PropertyChanged += onPropertyChanged;
            this.PropertyChanged += onCount_pos_Changed;
            Initial();
            timer1 = new DispatcherTimer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            buttons[15].Loaded += btnSpinner_Loaded;
          
        }

        public int[] Started()
        {
            List<int> ar;
            List<int> buf_ar;
            do
            {
                ar = new List<int>();
                Random rand = new Random();
                int buf = rand.Next(0, 16);
                buf_ar = new List<int>(new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
                while (buf_ar.Count != 0)
                {

                    if (!buf_ar.Contains(buf))
                    {
                        buf = rand.Next(0, 16);
                    }
                    else
                    {
                        ar.Add(buf);
                        buf_ar.Remove(buf);
                    }

                }

            } while (!IDA.DoHaveResolve(ar.ToArray()));
            return ar.ToArray();
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
                (ThreadStart)delegate()
                {
                    Tblock.Text = IDA.DoStop();
                }
              , DispatcherPriority.Normal);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ida.AlgFinished += new EventHandler<SearchEventArgs>(OnFinSolve);
            ida.AlgProgressing += new EventHandler<SearchEventArgs>(OnChangeText);
            ida.AlgStarted += new EventHandler<SearchEventArgs>(OnChangeText);
        }
        // Обработчик события, которое срабатывает при изменении свойства
        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetBackground();
        }
        // Обработчик события, которое срабатывает при изменении свойства Count_pos
        private void onCount_pos_Changed(object sender, PropertyChangedEventArgs e)
        {
            label1.Content = String.Format("Текущий ход:      {0}", count_pos);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (count_pos < IDA.arResult.Length)
            {
                PlaceCells(IDA.arResult[count_pos]);
                ida._start = IDA.arResult[count_pos];
                progressBar1.Value = CalcProgrValue();
                Count_pos++;
            }
            else
            {
                this.timer1.Stop();
            }
        }

        private void NewCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            PlaceCells(targetStat);
            SetBackground();
            progressBar1.Value = CalcProgrValue();
            Count_pos = 0;
        }

        private void ExitCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MixCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ida._start = Started();
            this.buttonStop.Visibility = Visibility.Hidden;
            this.ShowSolve.Visibility = Visibility.Hidden;
            PlaceCells(ida._start);
            this.progressBar1.Value = CalcProgrValue();
            Count_pos = 0;
            Tblock.Text = "";
        }

        private void SolveCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
                (ThreadStart)delegate()
                {
                    this.buttonStop.Visibility = Visibility.Visible;
                    ida.DoStart();
                }
              , DispatcherPriority.Normal);
        }

        private void LoadPictureCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            string path = Environment.CurrentDirectory;
            path = path.Substring(0, path.Length - 10);
            ofd.InitialDirectory = String.Format("{0}\\Images",path);
            ofd.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" + "All files (*.*)|*.*";
            ofd.Title = "Выберите рисунок";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                helpRes.SourceFileName = ofd.FileName;
            }
        }

        private void ShowSolutionCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Count_pos = 0;
            this.timer1.Start();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
