using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SudokuBoardLibrary;

namespace SudokuMobileApp
{
    public partial class CellsViewModel : INotifyPropertyChanged
    {
        readonly IList<SudokuBoardLibrary.Cell> source;
        SudokuBoardLibrary.Cell selectedCell;
        int selectionCount = 1;

        public ObservableCollection<SudokuBoardLibrary.Cell> Cells { get; private set; }
        public IList<SudokuBoardLibrary.Cell> EmptyCells { get; private set; }
        public IList<SudokuBoardLibrary.Cell> SameCells { get; private set; }

        public SudokuBoardLibrary.Cell SelectedCell
        {
            get
            {
                return selectedCell;
            }
            set
            {
                if(selectedCell != value)
                {
                    selectedCell = value;
                }
            }
        }

        ObservableCollection<object> selectedCells;
        public ObservableCollection<object> SelectedCells
        {
            get
            {
                return selectedCells;
            }
            set
            {
                if(selectedCells != value)
                {
                    selectedCells = value;
                }
            }
        }

        public string SelectedCellMessage { get; private set; }

        public ICommand CellSelectionChangedCommand => new Command(CellSelectionChanged);
        public ICommand SameCommand => new Command<string>(SameItems);
        public void CreateCellsCollection()
        {
            Generator sudukoBoardGenerator = new Generator();
            //sudukoBoardGenerator.SetBoard(70)

            foreach(SudokuBoardLibrary.Cell cell in sudukoBoardGenerator.SetBoard(40).Grid)
            {
                source.Add(cell);
            }
            Cells = new ObservableCollection<SudokuBoardLibrary.Cell>(source);
        }

        public CellsViewModel()
        {
            source = [];
            CreateCellsCollection();

        }
        public void SameItems(string filter)
        {
            List<SudokuBoardLibrary.Cell> filteredItems = source.Where(cell => cell.CellValue == Convert.ToInt32(filter)).ToList();
            //foreach(Cell monkey in source)
            //{
            //    SameCells.Add(monkey);
            //}
            SameCells = filteredItems;
        }
        public void CellSelectionChanged()
        {
            SelectedCellMessage = $"Selection {selectionCount}: {SelectedCell}";
            OnPropertyChanged("SelectedCellMessage");

            SameItems(SelectedCell.CellValue.ToString());
            selectionCount++;
        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
