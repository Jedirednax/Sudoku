namespace SudokuBoardLibrary
{
    public class Cell
    {
        #region Properties
        private int cellRow;
        private int cellColumn;
        private int cellBlock;

        private int cellValue = 0;
        private int cellSolution = 0;

        private bool isGiven = false;
        private bool isPopulated = false;

        private List<int> cellPossibilities = [];

        public int CellRow
        {
            get => cellRow;
            set => cellRow = value;
        }
        public int CellColumn
        {
            get => cellColumn;
            set => cellColumn = value;
        }
        public int CellValue
        {
            get => cellValue;
            set
            {
                if(cellValue != value)
                {
                    cellValue = value;
                }
                IsPopulated = value != 0;
            }
        }
        public int CellBlock
        {
            get => cellBlock;
            set => cellBlock = value;
        }
        public int CellSolution
        {
            get => cellSolution;
            set => cellSolution = value;
        }
        public bool IsGiven
        {
            get => isGiven;
            set => isGiven = value;
        }
        public bool IsPopulated
        {
            get => isPopulated;
            set => isPopulated = value;
        }
        public List<int> CellPossible
        {
            get
            {
                return cellPossibilities;
            }

            set
            {
                cellPossibilities = value;
            }
        }
        #endregion

        #region Constructors
        public Cell(int cellRow, int cellColumn)
        {
            CellRow = cellRow;
            CellColumn = cellColumn;
        }

        public Cell(int cellRow, int cellColumn, int cellValue) :
            this(cellRow, cellColumn)
        {
            CellValue = cellValue;
            CellSolution = CellValue;
            if(cellValue > 0)
            {
                IsGiven = true;
                IsPopulated = true;
            }
        }

        #endregion

        #region Sets
        public void Set(int setValue)
        {
            CellValue = setValue;
            CellPossible.Clear();
        }

        public void SetGiven()
        {
            IsGiven = true;
            CellValue = CellSolution;
            CellPossible = [];
        }

        public void SetGiven(int newValue)
        {
            CellValue = newValue;
            IsGiven = true;
            CellSolution = CellValue;
            CellPossible.Clear();
        }

        public void SetOpen()
        {
            CellValue = 0;
            IsGiven = false;
        }

        public void SetOpen(int newValue)
        {
            CellValue = 0;
            IsGiven = false;
            CellPossible.Clear();
        }

        public void Reset()
        {
            CellValue = 0;
            CellPossible.Clear();
        }
        public void Reveal()
        {
            if(CellSolution != 0)
            {
                CellValue = CellSolution;
            }
        }
        public void SetSolution()
        {
            CellSolution = CellValue;
        }

        public bool SetPossibilities(int newPossibilities)
        {
            if(!IsPopulated)
            {
                if(CellPossible == null)
                {
                    CellPossible = [];
                }
                if(!CellPossible.Contains(newPossibilities))
                {
                    CellPossible.Add(newPossibilities);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetPossibilities(List<int> newPossibilities)
        {
            if(IsPopulated)
            {
                return false;
            }

            if(CellPossible == null)
            {
                CellPossible = [];
            }
            foreach(int newPos in newPossibilities)
            {
                if(!CellPossible.Contains(newPos))
                {
                    CellPossible.Add(newPos);
                }
            }
            return true;
        }

        public bool RemovePossibilities(int valueToRemove)
        {
            if(CellPossible == null ||
                CellPossible.Count == 0 ||
                cellPossibilities.Count == 1)
            {
                return false;
            }
            return CellPossible.Remove(valueToRemove);
        }

        public bool RemovePossibilities(List<int> valuesToRemove)
        {
            bool removed = false;
            if(CellPossible == null ||
                CellPossible.Count == 0 ||
                cellPossibilities.Count == 1)
            {
                return false;
            }

            foreach(int valueToRemove in valuesToRemove)
            {
                if(CellPossible.Remove(valueToRemove))
                {
                    removed = true;
                }
            }
            return removed;
        }
        #endregion

        #region Gets
        #endregion

        #region Checks
        public bool IsCorrect()
        {
            return CellValue == CellSolution;
        }

        public bool ComparePossibilitiesPair(Cell comp)
        {
            if(comp == null)
            {
                return false;
            }
            if(CellPossible == null)
            {
                return false;
            }
            if(comp.CellPossible == null)
            {
                return false;
            }
            if(CellPossible.Count != 2)
            {
                return false;
            }
            if(comp.CellPossible.Count != 2)
            {
                return false;
            }
            return CellPossible.SequenceEqual(comp.CellPossible);
        }

        /// <summary>
        /// Checks the row and column values trurns false if they are diffrent.
        /// </summary>
        /// <param name="comp"> Cell to compare to. </param>
        /// <returns> Bool If the position is the same true, diff false</returns>
        public bool ComparePosition(Cell comp)
        {
            if(comp == null)
            {
                return false;
            }
            if(CellRow != comp.CellRow)
            {
                return false;
            }
            if(CellColumn != comp.CellColumn)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Displays
        public override string ToString()
        {
            string f = "";
            if(CellPossible != null)
            {
                foreach(int po in CellPossible)
                {
                    f += $"{po}:";
                }
            }
            string alt = "";
            if(CellPossible != null && CellPossible.Count > 0)
            {
                alt = $"[{f}]";
            }
            else
            {
                alt = $"{CellValue}";
            }
            return $"{alt,-16}";
        }

        public string PrintDebug()
        {
            string f = "";
            if(CellPossible != null)
            {
                foreach(int po in CellPossible)
                {
                    f += $"{po}:";
                }
            }

            return $"Cell Debug:\n" +
            $"\tRow:{CellRow}\tColumn: {CellColumn}\tBlock:{CellBlock}\t\n" +
            $"\tValue\t{CellValue}:[{f}]:{CellSolution}\n" +
            $"\tGiven\t{IsGiven}\n" +
            $"\tIsPop\t{IsPopulated}";
        }
        #endregion
    }
}