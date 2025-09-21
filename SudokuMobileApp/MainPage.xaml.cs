using SudokuBoardLibrary;

namespace SudokuMobileApp
{
    public class CellDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GivenCell { get; set; }
        public DataTemplate OpenCell { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((SudokuBoardLibrary.Cell)item).IsGiven ? GivenCell : OpenCell;
        }
    }
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public static string inValue = "0";
        public Button last;
        public static Board board;
        Generator sudukoBoardGenerator = new Generator();
        public static List<SudokuBoardLibrary.Cell> cells { get; set; } = [];

        public static bool NumberSelected = false;
        public MainPage()
        {
            InitializeComponent();

            views.ColumnSpacing=2;
            views.RowSpacing=2;
            views.RowSpacing =3;
            views.BackgroundColor = Colors.Transparent;
            //board.BoardSize = 9;
            for(int i = 0; i < 9; i++)
            {
                views.AddColumnDefinition(new ColumnDefinition());
                views.AddRowDefinition(new RowDefinition());
            }
            GenrateGame();

            inValue = "0";
        }

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
                btnSender.Background = Colors.Transparent;
                btnSender.TextColor = Colors.Black;

            }
            else
            {
                if(board.Attempt(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(inValue)))
                {
                    if(inValue =="0")
                    {
                        btnSender.Text = " ";
                    }
                    else
                    {
                        btnSender.Text = inValue;
                        App.Current.Resources.TryGetValue("HighlightedCellBackGround", out object? colorvalue);
                        Color highLighted = (Color)colorvalue;
                        btnSender.Background = highLighted;
                        btnSender.TextColor = Colors.White;

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

        private void HighLight(string con)
        {
            foreach(Button btn in views)
            {

                string pos = btn.AutomationId;

                string x = pos.Split(':')[0];
                string y = pos.Split(':')[1];
                if(btn.Text.ToString() == con)
                {
                    App.Current.Resources.TryGetValue("HighlightedCellBackGround", out object? colorvalue);
                    Color highLighted = (Color)colorvalue;
                    btn.Background = highLighted;
                    //btn.Background = Brush.Blue;
                    btn.TextColor = Colors.White;
                }
                else
                {
                    btn.Background = Brush.Transparent;
                    btn.TextColor = Colors.Black;
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
                        App.Current.Resources.TryGetValue("GivenCellBackGround", out object? colorvalue);
                        Color HighLighted = (Color)colorvalue;
                        v.Background = HighLighted;
                        v.TextColor = Colors.Black;
                    }
                    else
                    {
                        v.Background = Brush.Transparent;
                        v.TextColor = Colors.White;
                    }
                }
            }
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton? radioButton = sender as RadioButton;
            //radioButton.InputTransparent = true;
            object g = radioButton.FindByName("rdoDefualt");
            RadioButton? f = g as RadioButton;
            //f.IsChecked = true;
            //if(NumberSelected == false)
            //{
            //    NumberSelected = true;
            //}
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
                    // Debug.WriteLine($"Selected Value: {inValue}");

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
                if(btn.Text ==" ")
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
                if(cell.CellValue ==0)
                {
                    btn.Text =" ";
                }
                else
                {

                    btn.Text =cell.CellValue.ToString();
                }
                if(cell.IsGiven)
                {
                    App.Current.Resources.TryGetValue("GivenCellBackGround", out object? colorvalue);
                    Color HighLighted = (Color)colorvalue;
                    btn.Background = HighLighted;
                }
                else
                {
                    btn.Background = Brush.Transparent;
                }
            }
        }
        public void GenrateGame()
        {
            views.Clear();
            Co.Background = Brush.Orange;
            board = sudukoBoardGenerator.SetBoard(30);

            views.IsVisible = true;

            int wd =40;
            for(int row = 0; row < board.Grid.GetLength(0); row++)
            {
                for(int col = 0; col < board.Grid.GetLength(1); col++)
                {
                    SudokuBoardLibrary.Cell cell = board.Grid[row, col];
                    Button cellButton = new Button {
                        CornerRadius = 50
                    };
                    if(cell.CellValue ==0)
                    {
                        cellButton.Text =" ";
                    }
                    else
                    {
                        cellButton.Text =cell.CellValue.ToString();
                    }
                    if(cell.IsGiven)
                    {
                        App.Current.Resources.TryGetValue("GivenCellBackGround", out object? colorvalue);
                        Color HighLighted = (Color)colorvalue;
                        cellButton.Background = HighLighted;
                        cellButton.TextColor = Colors.Black;
                    }
                    else
                    {
                        cellButton.Background = Brush.Transparent;
                        cellButton.TextColor = Colors.White;
                    }
                    cellButton.AutomationId =$"{row}:{col}";// new SudukoBoardLibary.Cell(row, col, cell);
                    cellButton.Clicked +=(sender, e) => OnClicked(sender, e);
                    cellButton.IsEnabled = true;
                    Grid.SetRow(cellButton, row);
                    Grid.SetColumn(cellButton, col);

                    views.Add(cellButton);
                }
            }
        }
    }
}
