using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using AlgorithmLib;
using Table = AlgorithmLib.Table;

namespace Lab6Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Algorithm _algorithm = new ();
        public DataTable MyDataTable { get; set; } = null!;
        public double[,] Cells { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            var countOfObjects = int.Parse(CountOfObjectsTextBox.Text);

            var table = _algorithm.GenerateTable(countOfObjects);

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < table.Cells.GetLength(0); i++)
            {
                for (var j = 0; j < table.Cells.GetLength(1); j++)
                {
                    stringBuilder.Append($"| {table.Cells[i, j]:00.00} |");
                }
                stringBuilder.Append(Environment.NewLine);
            }

            TableText.Text = stringBuilder.ToString();
            
            Canvas.Children.Clear();
            var type = AlgorithmType.Min;

            if (MinRadioButton.IsChecked.Value)
            {
                type = AlgorithmType.Min;
            } 
            else if (MaxRadioButton.IsChecked.Value)
            {
                type = AlgorithmType.Max;
            }
            else if (MaxWithReversionRadioButton.IsChecked.Value)
            {
                type = AlgorithmType.MaxWithReversion;
            }

            
            var lastMove = _algorithm.Process(type);
            DrawLines(lastMove, type == AlgorithmType.Max ? _algorithm.Max : lastMove.NewValue.val);
        }

        private void DrawLines(Move lastMove, double max)
        {
            var bottomOffset = 25;
            var widthPerMove = (Canvas.ActualWidth - 100) / 2;
            var heightScale = (Canvas.ActualHeight - bottomOffset - 40) / max;
            var centerX = Canvas.ActualWidth / 2;

            DrawMoveLines(lastMove, widthPerMove, heightScale, centerX, bottomOffset);
        }

        private void DrawMoveLines(Move move, double widthPerMove, double heightScale, double centerX, double bottomOffset)
        {
            var txt1 = new TextBlock
            {
                FontSize = 14,
                Text = $"{move.NewValue.name} | {move.NewValue.val:#0.####}"
            };
            Canvas.SetTop(txt1, Canvas.ActualHeight - move.NewValue.val * heightScale - bottomOffset);
            Canvas.SetLeft(txt1, centerX - 20);
            Canvas.Children.Add(txt1);
            
            if (move.NewValue.val != 0)
            {
                Canvas.Children.Add(new Line()
                {
                    X1 = centerX - widthPerMove / 2,
                    X2 = centerX + widthPerMove / 2,
                    Y1 = Canvas.ActualHeight - move.NewValue.val * heightScale - bottomOffset,
                    Y2 = Canvas.ActualHeight - move.NewValue.val * heightScale - bottomOffset,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                });

                if (move.OldValues.Count == 2)
                {
                    Canvas.Children.Add(new Line()
                    {
                        X1 = centerX - widthPerMove / 2,
                        X2 = centerX - widthPerMove / 2,
                        Y1 = Canvas.ActualHeight - move.NewValue.val * heightScale - bottomOffset,
                        Y2 = Canvas.ActualHeight - move.OldValues[0].val * heightScale - bottomOffset,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });
                    Canvas.Children.Add(new Line()
                    {
                        X1 = centerX + widthPerMove / 2,
                        X2 = centerX + widthPerMove / 2,
                        Y1 = Canvas.ActualHeight - move.NewValue.val * heightScale - bottomOffset,
                        Y2 = Canvas.ActualHeight - move.OldValues[1].val * heightScale - bottomOffset,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    });

                    if (move.OldMoves.Count == 1)
                    {
                        DrawMoveLines(move.OldMoves[0], widthPerMove / 2, heightScale, centerX - widthPerMove / 2, bottomOffset);
                        
                        var txt3 = new TextBlock
                        {
                            FontSize = 14,
                            Text = $"{move.OldValues[1].name}"
                        };
                        Canvas.SetTop(txt3, Canvas.ActualHeight - move.OldValues[1].val * heightScale - bottomOffset);
                        Canvas.SetLeft(txt3, centerX + widthPerMove / 2 - 20);
                        Canvas.Children.Add(txt3);
                    }
                    else if (move.OldMoves.Count == 2)
                    {
                        DrawMoveLines(move.OldMoves[0], widthPerMove / 2, heightScale, centerX - widthPerMove / 2, bottomOffset);
                        DrawMoveLines(move.OldMoves[1], widthPerMove / 2, heightScale, centerX + widthPerMove / 2, bottomOffset);
                    }
                    else
                    {
                        var txt2 = new TextBlock
                        {
                            FontSize = 14,
                            Text = $"{move.OldValues[0].name}"
                        };
                        Canvas.SetTop(txt2, Canvas.ActualHeight - move.OldValues[0].val * heightScale - bottomOffset);
                        Canvas.SetLeft(txt2, centerX - widthPerMove / 2- 20);
                        Canvas.Children.Add(txt2);
                        
                        var txt3 = new TextBlock
                        {
                            FontSize = 14,
                            Text = $"{move.OldValues[1].name}"
                        };
                        Canvas.SetTop(txt3, Canvas.ActualHeight - move.OldValues[1].val * heightScale - bottomOffset);
                        Canvas.SetLeft(txt3, centerX + widthPerMove / 2 - 20);
                        Canvas.Children.Add(txt3);
                    }
                }
            }
        }
    }
}