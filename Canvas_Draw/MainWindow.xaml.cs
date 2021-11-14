using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Canvas_Draw
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        Point start, dest;

        int currentStrokeThickness = 1;
        string currentAction;
        string currentShape;

        Color currentStrokeColor = Colors.Black;
        Color currentFillColor = Colors.Transparent;
        Color currentBGColor;
        Brush currentStrokeBrush = new SolidColorBrush(Colors.Black);
        Brush currentFillColorBrush = new SolidColorBrush(Colors.Transparent);
        Brush currentBGBrush;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyCanvas.Cursor = Cursors.Cross;
            start = e.GetPosition(MyCanvas);
            MyLabel.Content = $"座標點{start}";
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            switch (currentAction)
            {
                case "Draw":
                    MyCanvas.Cursor = Cursors.Pen;
                    dest = e.GetPosition(MyCanvas);
                    MyLabel.Content = $"座標點:{start}-{dest}";
                    break;
                case "Erase":
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        var SelectedShape = e.OriginalSource as Shape;
                        MyCanvas.Children.Remove(SelectedShape);
                    } 
                    break;
                default:
                    return;
            }
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (currentShape)
            {
                case "Line":
                    DrawLine();
                    break;
                case "Rectangle":
                    DrawRectangle();
                    break;
                case "Ellipse":
                    DrawEllipse();
                    break;
                case "Polygon"://未完成
                    DrawPolygon();
                    break;
                default:
                    return;
            }
            MyCanvas.Cursor = Cursors.Arrow;
        }

        private void DrawPolygon()
        {
            Polygon newPolygon = new Polygon()
            {              
                Stroke = currentStrokeBrush,
                Fill = currentFillColorBrush,
                StrokeThickness = currentStrokeThickness,               
            };
            Point P3 = new Point(dest.X - start.X +100, dest.Y - start.Y);
            PointCollection pointCollect = new PointCollection();
            pointCollect.Add(start);
            pointCollect.Add(dest);
            pointCollect.Add(P3);
            newPolygon.Points = pointCollect;

            newPolygon.SetValue(Canvas.LeftProperty, start.X);
            newPolygon.SetValue(Canvas.TopProperty, start.Y);
            MyCanvas.Children.Add(newPolygon);
        }//未完成

        private void DrawEllipse()
        {
            AddjustPoint();

            double width = dest.X - start.X;
            double height = dest.Y - start.Y;

            Ellipse newEllipse = new Ellipse()
            {
                Stroke = currentStrokeBrush,
                StrokeThickness = currentStrokeThickness,
                Fill = currentFillColorBrush,
                Width = width,
                Height = height
            };

            newEllipse.SetValue(Canvas.LeftProperty, start.X);
            newEllipse.SetValue(Canvas.TopProperty, start.Y);
            MyCanvas.Children.Add(newEllipse);
        }

        private void DrawRectangle()
        {
            AddjustPoint();

            double width = dest.X - start.X;
            double height = dest.Y - start.Y;

            Rectangle newRectangle = new Rectangle()
            {               
                Stroke = currentStrokeBrush,
                StrokeThickness = currentStrokeThickness,
                Fill = currentFillColorBrush,
                Width = width,
                Height = height
            };

            newRectangle.SetValue(Canvas.LeftProperty, start.X);
            newRectangle.SetValue(Canvas.TopProperty, start.Y);
            MyCanvas.Children.Add(newRectangle);
        }

        private void AddjustPoint()
        {
            double X_Min, Y_Min, X_Max, Y_Max;

            X_Min = Math.Min(start.X, dest.X);
            X_Max = Math.Max(start.X, dest.X);
            Y_Min = Math.Min(start.Y, dest.Y);
            Y_Max = Math.Max(start.Y, dest.Y);

            start.X = X_Min;
            start.Y = Y_Min;
            dest.X = X_Max;
            dest.Y = Y_Max;

            MyCanvas.SetValue(Canvas.LeftProperty, start.X);
            MyCanvas.SetValue(Canvas.TopProperty, start.Y);
        }

        private void DrawLine()
        {
            Line newLine = new Line();

            newLine.Stroke = currentStrokeBrush;
            newLine.StrokeThickness = currentStrokeThickness;
            newLine.X1 = start.X;
            newLine.Y1 = start.Y;
            newLine.X2 = dest.X;
            newLine.Y2 = dest.Y;

            MyCanvas.Children.Add(newLine);
        }

        private void currentStrokeColor_Click(object sender, RoutedEventArgs e)
        {
            currentStrokeColor = getDialogColor();
            currentStrokeBrush = new SolidColorBrush(currentStrokeColor);
            StrokeColor.Background = currentStrokeBrush;
        }

        private void FillColor_Click(object sender, RoutedEventArgs e)
        {
            currentFillColor = getDialogColor();
            currentFillColorBrush = new SolidColorBrush(currentFillColor);
            FillColor.Background = currentFillColorBrush;
        }

        private Color getDialogColor()
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.ShowDialog();

            System.Drawing.Color dlgColor = dlg.Color;
            return Color.FromArgb(dlgColor.A, dlgColor.R, dlgColor.G, dlgColor.B);
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rbt = sender as RadioButton;
            currentShape = rbt.Content.ToString();
            currentAction = "Draw";
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            currentAction = "Erase";
        }

        private void MenuCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (MenuCheckBox.IsChecked == true)
            {
                MyToolBarTray.Visibility = Visibility.Visible;
                MyCanvas.Height -= MyToolBarTray.Height;
            }
            else
            {
                MyToolBarTray.Visibility = Visibility.Collapsed;
                MyCanvas.Height += MyToolBarTray.Height;
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
            MyCanvas.Cursor = Cursors.Arrow;
            MyLabel.Content = "Ready";
        }

        private void BackGroudColor_Click(object sender, RoutedEventArgs e)
        {
            currentBGColor = getDialogColor();
            currentBGBrush = new SolidColorBrush(currentBGColor);
            MyCanvas.Background = currentBGBrush;
        }

        private void SaveCanvasMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int w = Convert.ToInt32(MyCanvas.RenderSize.Width);
            int h = Convert.ToInt32(MyCanvas.RenderSize.Height);

            //將 MyCanvas 轉成 Bitmap
            RenderTargetBitmap rtb = new RenderTargetBitmap(w, h, 64d, 64d, PixelFormats.Default);
            rtb.Render(MyCanvas);

            //將 Bitmap 轉成 png, jpg
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(rtb));

            //存檔
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "儲存檔案";
            saveFileDialog.DefaultExt = "*.png";
            saveFileDialog.Filter = "PNG檔案(*.png)|*.png|JPeg檔案(*.jpg)|*.jpg|全部檔案|*.*";
            saveFileDialog.ShowDialog();

            string Path = saveFileDialog.FileName;

            //儲存 png 檔
            using (var fs = File.Create(Path))
            {
                png.Save(fs);
            }
        }

        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentStrokeThickness = Convert.ToInt32(StrokeThicknessSlider.Value);
        }
    }
}