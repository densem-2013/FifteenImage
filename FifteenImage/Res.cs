using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Input;

namespace FifteenImage
{
    public class Res : INotifyPropertyChanged
    {
        public string SFN = @"6.jpg";
        public  event PropertyChangedEventHandler PropertyChanged;
        private  ImageSource imsource;
        public ImageSource ImSource
        {
            get
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();
                string path = string.Format(@"{0}\{1}", (System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)), SFN);
                imsource = (ImageSource)imgConv.ConvertFromString(SFN);
                return imsource;
            }
            set { imsource = value; }
        }
        public string SourceFileName
        {
             get { return SFN; }
             set 
             {
                 SFN = value;
                 var imgConv = new ImageSourceConverter();
                 imsource = (ImageSource)imgConv.ConvertFromString(value); 
                 if (PropertyChanged != null)
                 {
                     PropertyChanged(this, new PropertyChangedEventArgs("SourceFileName"));
                 }
             }
        }
        public Res()
        {
            
        }
    }
   public class DConverter : IMultiValueConverter
   {
       public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
       {
           int par1 = System.Convert.ToInt32(value[0]);
           int par2 = System.Convert.ToInt32(value[1]);
           return (object)String.Format("{0:0.00},{1:0.00},0.25,0.25", par1 * 0.25, (par2-1) * 0.25);
           //return (object)new System.Drawing.Rectangle( par1 * 25, (par2-1) * 25, 25, 25);
       }

       public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
       {
           return (object[])null;
       }
   }
   public class DataCommands
   {
       private static RoutedUICommand escape;
       private static RoutedUICommand newGame;
       private static RoutedUICommand mix;
       private static RoutedUICommand solve;
       private static RoutedUICommand loadPicture;
       private static RoutedUICommand showSolution;
       static DataCommands()
       {
           //// Инициализация команды.
           InputGestureCollection inputs = new InputGestureCollection();
           inputs.Add(new KeyGesture(Key.Escape, ModifierKeys.None, "Esc"));
           escape = new RoutedUICommand(
             "Escape", "Escape", typeof(DataCommands), inputs);

           InputGestureCollection inputs2 = new InputGestureCollection();
           inputs2.Add(new KeyGesture(Key.N, ModifierKeys.Control, "Ctr+N"));
           newGame = new RoutedUICommand(
             "NewGame", "NewGame", typeof(DataCommands), inputs2);

           InputGestureCollection inputs3 = new InputGestureCollection();
           inputs3.Add(new KeyGesture(Key.M, ModifierKeys.Control, "Ctr+M"));
           mix = new RoutedUICommand(
             "Mix", "Mix", typeof(DataCommands), inputs3);

           InputGestureCollection inputs4 = new InputGestureCollection();
           inputs4.Add(new KeyGesture(Key.S, ModifierKeys.Control, "Ctr+S"));
           solve = new RoutedUICommand(
             "Solve", "Solve", typeof(DataCommands), inputs4);

           InputGestureCollection inputs5 = new InputGestureCollection();
           inputs5.Add(new KeyGesture(Key.L, ModifierKeys.Control, "Ctr+L"));
           loadPicture = new RoutedUICommand(
             "LoadPicture", "LoadPicture", typeof(DataCommands), inputs5);

           InputGestureCollection inputs6 = new InputGestureCollection();
           inputs6.Add(new KeyGesture(Key.P, ModifierKeys.Control));
           showSolution = new RoutedUICommand(
             "ShowSolution", "ShowSolution", typeof(DataCommands), inputs6);
       }
       public static RoutedUICommand Escape
       {
           get { return escape; }
       }
       public static RoutedUICommand NewGame
       {
           get { return newGame; }
       }
       public static RoutedUICommand Mix
       {
           get { return mix; }
       }
       public static RoutedUICommand Solve
       {
           get { return solve; }
       }
       public static RoutedUICommand LoadPicture
       {
           get { return loadPicture; }
       }
       public static RoutedUICommand ShowSolution
       {
           get { return showSolution; }
       }
   }
}
