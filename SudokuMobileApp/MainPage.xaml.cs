using SudokuBoardLibrary;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using WinRT;
namespace SudokuMobileApp
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public SudokuBoardLibrary.Cell _cell { get; set; }
        public int CellValue { get; set; }
        public bool isGiven { get; set; }
        public bool isPopulated { get; set; }

        public List<string> possable { get; set; }

        public CellViewModel(SudokuBoardLibrary.Cell cell)
        {
            CellValue = cell.CellValue;
            isGiven = cell.IsGiven;
            isPopulated = cell.IsPopulated;
            possable = [.. cell.CellPossible.OrderBy(p => p).Select(p => p.ToString()).ToList()];
            _cell = cell;
        }

        // Notify UI of property change
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ContainsValueToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList<int>? list = value as IList<int>;
            int target = int.Parse(parameter.ToString());

            if(list != null && list.Contains(target))
            {

                return target.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MainPage : ContentPage
    {
        int count = 0;
        public static string inValue = "0";
        public Button last;
        public static Board board;
        Generator BoardGenerator = new Generator();
        //public static List<SudokuBoardLibrary.Cell> cells { get; set; } = [];

        public ObservableCollection<CellViewModel> cells { get; set; } = [];

        public static bool NumberSelected = false;
        public MainPage()
        {
            InitializeComponent();
            board = BoardGenerator.SetBoard(40);

            views.ColumnSpacing = 2;
            views.RowSpacing = 2;
            views.RowSpacing = 3;
            views.BackgroundColor = Colors.Transparent;
            board.BoardSize = 9;
            foreach(KeyValuePair<string, object> f in Resources)
            {
                Debug.WriteLine(f.Key);
            }
            for(int i = 0; i < 9; i++)
            {
                views.AddColumnDefinition(new ColumnDefinition());
                views.AddRowDefinition(new RowDefinition());
            }
            GenrateGame();
            foreach(SudokuBoardLibrary.Cell item in board.Grid)
            {
                cells.Add(new CellViewModel(item));
            }
            //testCol
            //testCol.ItemsSource = cells;

            inValue = "0";
        }


        // TODO: remaining number count.
        // have the number of the digits rem,ain displayed


        // TODO: Add a note Notes to cells
        // Have it added to bottom row hat allows for user notes.


        // TODO: Add all notes button. 


        // TODO: Add save to disk to keep between sessions

        // TODO: Add diffiicylty rating








        //void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    CollectionView cv = (CollectionView)sender;
        //    if(cv.SelectedItem == null)
        //    {
        //
        //        return;
        //    }
        //}
        //void UpdateSelectionData(IEnumerable<object> previousSelectedItems, IEnumerable<object> currentSelectedItems)
        //{
        //          var previous = ToList(previousSelectedItems);
        //          var current = ToList(currentSelectedItems);
        //          previousSelectedItemLabel.Text = string.IsNullOrWhiteSpace(previous) ? "[none]" : previous;
        //          currentSelectedItemLabel.Text = string.IsNullOrWhiteSpace(current) ? "[none]" : current;
        //}

        private void OnClicked(object sender, EventArgs e)
        {
            Button? btnSender = sender as Button;
            string pos = btnSender.AutomationId;

            string x = pos.Split(':')[0];
            string y = pos.Split(':')[1];
            if(btnSender.Text == inValue)
            {
                btnSender.Text = " ";
                board.Attempt(Convert.ToInt32(x), Convert.ToInt32(y), 0);



                Resources.TryGetValue("HighlightedCell", out object? high);
                btnSender.Style = (Style)high;
            }
            else
            {
                if(board.Attempt(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(inValue)))
                {
                    if(inValue == "0")
                    {
                        btnSender.Text = " ";
                    }
                    else
                    {
                        btnSender.Text = inValue;
                        App.Current.Resources.TryGetValue("HighlightedCell", out object? high);
                        btnSender.Style = (Style)high;
                        UpdateProps(btnSender);

                    }
                }
                if(board.VerifyBoard())
                {
                    Co.Background = Brush.Green;
                    foreach(Button bt in views)
                    {
                        bt.IsEnabled = false;
                    }
                }
            }
        }

        private void UpdateProps(object sender)
        {
            Button? btnSender = sender as Button;
            string pos = btnSender.AutomationId;

            string x = pos.Split(':')[0];
            string y = pos.Split(':')[1];
            string b = pos.Split(':')[2];

            foreach(var cellItem in views.Where(ro => ro.AutomationId.Split(':')[0] == x))
            {


                int line = 0;
                //if(cellItem)
                //{
                //}
                var cb =cellItem as Button;

                if(cb.Text.Length > 1)
                {
                    string pops = "";
                    foreach(int p in board.GetCell(Convert.ToInt32(cb.AutomationId.Split(':')[0]),
                        Convert.ToInt32(cb.AutomationId.Split(':')[1])).CellPossible)
                    {
                        line++;
                        pops += p.ToString() + " ";
                        if(line % 3 == 0)
                        {
                            pops += "\n";
                        }
                    }
                    cb.Text = pops;
                    Debug.WriteLine(cb.Text);
                    Debug.WriteLine("-------");
                }
            }


            foreach(var cellItem in views.Where(ro => ro.AutomationId.Split(':')[1] == y))
            {


                int line = 0;
                //if(cellItem)
                //{
                //}
                var cb =cellItem as Button;

                if(cb.Text.Length > 1)
                {
                    string pops = "";
                    foreach(int p in board.GetCell(Convert.ToInt32(cb.AutomationId.Split(':')[0]),
                        Convert.ToInt32(cb.AutomationId.Split(':')[1])).CellPossible)
                    {
                        line++;
                        pops += p.ToString() + " ";
                        if(line % 3 == 0)
                        {
                            pops += "\n";
                        }
                    }
                    cb.Text = pops;
                    Debug.WriteLine(cb.Text);
                    Debug.WriteLine("-------");
                }
            }

            foreach(var cellItem in views.Where(ro => ro.AutomationId.Split(':')[2] == b))
            {


                int line = 0;
                //if(cellItem)
                //{
                //}
                var cb =cellItem as Button;

                if(cb.Text.Length > 1)
                {
                    string pops = "";
                    foreach(int p in board.GetCell(Convert.ToInt32(cb.AutomationId.Split(':')[0]),
                        Convert.ToInt32(cb.AutomationId.Split(':')[1])).CellPossible)
                    {
                        line++;
                        pops += p.ToString() + " ";
                        if(line % 3 == 0)
                        {
                            pops += "\n";
                        }
                    }
                    cb.Text = pops;
                    Debug.WriteLine(cb.Text);
                    Debug.WriteLine("-------");
                }
            }
        }


        private void HighLight(string con)
        {
            foreach(Button btn in views)
            {
                string pos = btn.AutomationId;

                string x = pos.Split(':')[0];
                string y = pos.Split(':')[1];
                if(btn.Text.ToString() == con)
                {
                    App.Current.Resources.TryGetValue("HighlightedCell", out object? high);
                    btn.Style = (Style)high;
                }
                else
                {
                    App.Current.Resources.TryGetValue("PopCell", out object? high);
                    btn.Style = (Style)high;
                }
            }
        }
        private void DeHighLight(string con)
        {
            foreach(Button v in views)
            {
                string pos = v.AutomationId;

                string x = pos.Split(':')[0];
                string y = pos.Split(':')[1];
                if(v.Text.ToString() != con)
                {
                    SudokuBoardLibrary.Cell cell = board.Grid[Convert.ToInt32(x), Convert.ToInt32(y)];
                    if(cell.IsGiven)
                    {
                        App.Current.Resources.TryGetValue("GivenCell", out object? high);
                        v.Style = (Style)high;
                    }
                    else
                    {
                        if(cell.IsPopulated)
                        {
                            App.Current.Resources.TryGetValue("PopCell", out object? high);
                            v.Style = (Style)high;
                        }
                        else
                        {
                            App.Current.Resources.TryGetValue("OpenCell", out object? high);
                            v.Style = (Style)high;
                        }
                    }
                }
            }
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton? radioButton = sender as RadioButton;
            object g = radioButton.FindByName("rdoDefualt");
            RadioButton? f = g as RadioButton;
            if(radioButton?.IsChecked == true)
            {
                if(radioButton.Value.ToString() == inValue)
                {
                    radioButton.IsChecked = false;
                    inValue = null;
                }
                else
                {
                    inValue = radioButton.Value.ToString();
                    HighLight(inValue);
                    DeHighLight(inValue);
                }
            }
        }

        private void Reveal_Clicked(object sender, EventArgs e)
        {
            rdoRemove.IsChecked = true;
            board.ResetBoard();
            foreach(Button btn in views)
            {
                if(btn.Text == " ")
                {
                    btn.TextColor = Colors.Red;
                }
                btn.IsEnabled = false;
                string pos = btn.AutomationId;

                string x = pos.Split(':')[0];
                string y = pos.Split(':')[1];

                SudokuBoardLibrary.Cell cell = board.Grid[Convert.ToInt32(x), Convert.ToInt32(y)];

                btn.Text = cell.CellSolution.ToString();

            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            GenrateGame();
        }

        public void Reset_Clicked(object sender, EventArgs e)
        {
            rdoRemove.IsChecked = true;
            board.ResetBoard();
            foreach(Button btn in views)
            {
                btn.IsEnabled = true;
                string pos = btn.AutomationId;

                string x = pos.Split(':')[0];
                string y = pos.Split(':')[1];

                SudokuBoardLibrary.Cell cell = board.Grid[Convert.ToInt32(x), Convert.ToInt32(y)];
                if(cell.CellValue == 0)
                {
                    btn.Text = " ";
                }
                else
                {
                    btn.Text = cell.CellValue.ToString();
                }
                if(cell.IsGiven)
                {
                    App.Current.Resources.TryGetValue("GivenCell", out object? high);
                    btn.Style = (Style)high;
                }
                else
                {
                    App.Current.Resources.TryGetValue("OpenCell", out object? high);
                    btn.Style = (Style)high;
                }
            }
        }
        public void GenrateGame()
        {
            views.Clear();
            Co.Background = Brush.Orange;

            views.IsVisible = true;

            int wd =60;
            for(int row = 0; row < board.Grid.GetLength(0); row++)
            {
                for(int col = 0; col < board.Grid.GetLength(1); col++)
                {
                    SudokuBoardLibrary.Cell cell = board.Grid[row, col];
                    Button cellButton = new Button
                    {
                        CornerRadius = 50,
                        BindingContext = cell
                    };
                    if(cell.CellValue == 0)
                    {
                        cellButton.Text = " ";
                    }
                    else
                    {
                        cellButton.Text = cell.CellValue.ToString();
                    }

                    if(cell.IsGiven)
                    {
                        if(App.Current.Resources.TryGetValue("GivenCell", out object? high))
                        {
                            cellButton.Style = (Style)high;
                        }
                        else
                        {
                            cellButton.Background = Brush.Blue;
                        }
                    }
                    else
                    {
                        int line = 0;

                        if(App.Current.Resources.TryGetValue("OpenCell", out object? high))
                        {
                            cellButton.Style = (Style)high;
                        }
                        else
                        {
                            if(App.Current.Resources.TryGetValue("OpenCellBackGround", out object? colorV))
                            {
                                cellButton.Background = (Color)colorV;
                            }
                            else
                            {
                                Debug.WriteLine("OpenCellBackGround");
                            }
                        }
                        string pops = "";
                        foreach(int p in cell.CellPossible)
                        {
                            line++;
                            pops += p.ToString() + " ";
                            if(line % 3 == 0)
                            {
                                pops += "\n";
                            }
                        }
                        cellButton.Text = pops;
                        //Grid miniGrid = PropsGrid(cell.CellPossible);

                    }
                    cellButton.AutomationId = $"{row}:{col}:{cell.CellBlock}";// new SudukoBoardLibary.Cell(row, col, cell);
                    cellButton.Clicked += (sender, e) => OnClicked(sender, e);
                    cellButton.IsEnabled = true;
                    Grid.SetRow(cellButton, row);
                    Grid.SetColumn(cellButton, col);

                    views.Add(cellButton);
                }
            }
        }
        public Grid PropsGrid(List<int> props)
        {
            Grid grdProps = [];

            grdProps.AddColumnDefinition(new ColumnDefinition());
            grdProps.AddColumnDefinition(new ColumnDefinition());
            grdProps.AddColumnDefinition(new ColumnDefinition());

            grdProps.AddRowDefinition(new RowDefinition());
            grdProps.AddRowDefinition(new RowDefinition());
            grdProps.AddRowDefinition(new RowDefinition());

            foreach(int i in props)
            {
                Label newCon = new Label
                {
                    Text = i.ToString()
                };
                if(i == 0)
                {
                    continue;
                }
                else if(i == 1)
                {
                    Grid.SetRow(newCon, 0);
                    Grid.SetColumn(newCon, 0);
                }
                else if(i == 2)
                {
                    Grid.SetRow(newCon, 0);
                    Grid.SetColumn(newCon, 1);
                }
                else if(i == 3)
                {
                    Grid.SetRow(newCon, 0);
                    Grid.SetColumn(newCon, 2);
                }
                else if(i == 4)
                {
                    Grid.SetRow(newCon, 1);
                    Grid.SetColumn(newCon, 0);
                }
                else if(i == 5)
                {
                    Grid.SetRow(newCon, 1);
                    Grid.SetColumn(newCon, 1);
                }
                else if(i == 6)
                {
                    Grid.SetRow(newCon, 1);
                    Grid.SetColumn(newCon, 2);
                }
                else if(i == 7)
                {
                    Grid.SetRow(newCon, 2);
                    Grid.SetColumn(newCon, 0);
                }
                else if(i == 8)
                {
                    Grid.SetRow(newCon, 2);
                    Grid.SetColumn(newCon, 1);
                }
                else if(i == 9)
                {
                    Grid.SetRow(newCon, 2);
                    Grid.SetColumn(newCon, 2);
                }
                grdProps.AddLogicalChild(newCon);
            }
            return grdProps;
        }
    }

    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GivenCellTemplate { get; set; }
        public DataTemplate PopulatedCellTemplate { get; set; }
        public DataTemplate OpenCellTemplate { get; set; }
        public DataTemplate HighlightedCellTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            CellViewModel? cell = item as CellViewModel;

            if(cell == null)
            {
                return OpenCellTemplate;
            }

            if(cell.isGiven)
            {
                return GivenCellTemplate;
            }
            if(cell.isPopulated)
            {
                return PopulatedCellTemplate;
            }

            return OpenCellTemplate;
        }
    }
}
