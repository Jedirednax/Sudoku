using System.Text;

namespace SudokuBoardLibrary
{

    public class Board : IBoard
    {
        #region Properties
        public int BoardSize = 9;
        public int BlockSize = 3;
        public int NumberOfCells { get; set; }
        public int NumberOfBlocks { get; set; }
        private int[,] BlockIndex { get; set; }

        public Cell[,] Grid { get; private set; }

        public List<Cell> openCells = [];

        public int[] digitCount = new int[9];

        private string? Difficulty { get; set; }
        #endregion

        #region Constructor
        public Board(int size)
        {
            BlockSize = size;
            BoardSize = BlockSize * BlockSize;
            BlockIndex = SetCellBlock(BlockSize);
            Grid = new Cell[BoardSize, BoardSize];
        }

        public Board(int[,] board)
        {
            BoardSize = board.GetLength(0);
            BlockSize = (int)Math.Sqrt(board.GetLength(0));
            BlockIndex = SetCellBlock(BlockSize);
            Grid = new Cell[BoardSize, BoardSize];

            for(int row = 0; row < board.GetLength(0); row++)
            {
                for(int col = 0; col < board.GetLength(1); col++)
                {
                    int cell = board[row, col];
                    Cell nCell = new Cell(row, col, cell);
                    if(cell == 0)
                    {
                        openCells.Add(nCell);
                    }
                    int blockPos = GetBlockPos(row, col);
                    nCell.CellBlock = blockPos;
                    Grid[row, col] = nCell;
                }
            }

            for(int row = 0; row < board.GetLength(0); row++)
            {
                for(int col = 0; col < board.GetLength(1); col++)
                {
                    GetCell(row, col).CellPossible = CalcCellPoss(row, col);
                }
            }
        }

        public Board(Cell[,] board)
        {
            BoardSize = board.GetLength(0);
            BlockSize = (int)Math.Sqrt(board.GetLength(0));
            BlockIndex = SetCellBlock(BlockSize);
            Grid = new Cell[BoardSize, BoardSize];

            for(int row = 0; row < board.GetLength(0); row++)
            {
                for(int col = 0; col < board.GetLength(1); col++)
                {
                    int cell = board[row, col].CellValue;
                    Cell nCell = new Cell(row, col, cell);
                    if(cell == 0)
                    {
                        openCells.Add(nCell);
                    }
                    int blockPos = GetBlockPos(row, col);
                    nCell.CellBlock = blockPos;
                    Grid[row, col] = nCell;
                }
            }

            for(int row = 0; row < board.GetLength(0); row++)
            {
                for(int col = 0; col < board.GetLength(1); col++)
                {
                    GetCell(row, col).SetPossibilities(
                        CalcCellPoss(row, col));
                }
            }
        }
        #endregion

        #region Cell Manipulation
        /// <summary>
        /// Gets the values of the selected cell in the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <returns> Return the integer value of the selected cell. </returns>
        public Cell GetCell(int inRow, int inCol)
        {
            if(inRow < 0 || inRow >= BoardSize ||
               inCol < 0 || inCol >= BoardSize)
            {
                throw new ArgumentException("Values out side of board range");
            }
            return Grid[inRow, inCol];
        }

        /// <summary>
        /// Sets the values of the cell in the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <param name="cellValue"> Value to set the cell. </param>
        /// <returns> True </returns>
        public bool Set(int inRow, int inCol, int cellValue)
        {
            if(inRow < 0 || inRow >= BoardSize ||
               inCol < 0 || inCol >= BoardSize)
            {
                throw new ArgumentException("Values out side of board range");
            }
            if(cellValue == 0)
            {
                openCells.Add(GetCell(inRow, inCol));
            }
            else
            {
                openCells.Remove(GetCell(inRow, inCol));
            }
            Grid[inRow, inCol].Set(cellValue);
            UpdatePoss(inRow, inCol, cellValue);
            return true;
        }

        /// <summary>
        /// Sets the values of the cell in the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <param name="cellValue"> Value to set the cell. </param>
        /// <returns>  True </returns>
        public bool Attempt(int inRow, int inCol, int cellValue)
        {
            if(cellValue == 0)
            {
                openCells.Add(GetCell(inRow, inCol));
            }
            else
            {
                openCells.Remove(GetCell(inRow, inCol));
            }
            if(!GetCell(inRow, inCol).IsGiven)
            {
                GetCell(inRow, inCol).Set(cellValue);
                UpdatePoss(inRow, inCol, cellValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the values of the cell in the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        public void SetGiven(int inRow, int inCol)
        {
            GetCell(inRow, inCol).SetGiven();
            openCells.Remove(GetCell(inRow, inCol));
            UpdatePoss(inRow, inCol, GetCell(inRow, inCol).CellSolution);
        }

        /// <summary>
        /// Sets the values of the cell in the grid.
        /// </summary>
        /// <param name="cell"> The cell to be set as Given </param>
        public void SetGiven(Cell cell)
        {
            GetCell(cell.CellRow, cell.CellColumn).SetGiven();
            openCells.Remove(cell);
            UpdatePoss(cell.CellRow, cell.CellColumn,
                GetCell(cell.CellRow, cell.CellColumn).CellSolution);
        }

        /// <summary>
        /// Sets the values of the cell in the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        public void SetOpen(int inRow, int inCol)
        {
            GetCell(inRow, inCol).SetOpen();
            openCells.Add(GetCell(inRow, inCol));
            UpdatePoss(inRow, inCol, 0);
        }

        /// <summary>
        /// Sets the values of the cell in the grid.
        /// </summary>
        /// <param name="cell"> The cell to be set as Open </param>
        public void SetOpen(Cell cell)
        {
            Grid[cell.CellRow, cell.CellColumn].SetOpen();
            UpdatePoss(cell.CellRow, cell.CellColumn, 0);
            openCells.Add(cell);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetValidSolution()
        {
            if(VerifyBoard())
            {
                for(int row = 0; row < Grid.GetLength(0); row++)
                {
                    for(int col = 0; col < Grid.GetLength(1); col++)
                    {
                        GetCell(row, col).SetSolution();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetBoard()
        {
            openCells = [];
            for(int row = 0; row < Grid.GetLength(0); row++)
            {
                for(int col = 0; col < Grid.GetLength(1); col++)
                {
                    if(!GetCell(row, col).IsGiven)
                    {
                        GetCell(row, col).Set(0);
                        openCells.Add(GetCell(row, col));
                    }
                }
            }
            foreach(Cell cell in openCells)
            {
                GetCell(cell.CellRow, cell.CellColumn).SetPossibilities(
                CalcCellPoss(cell.CellRow, cell.CellColumn));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFinal()
        {
            if(VerifyBoard())
            {
                for(int row = 0; row < Grid.GetLength(0); row++)
                {
                    for(int col = 0; col < Grid.GetLength(1); col++)
                    {
                        GetCell(row, col).SetGiven();
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a single value and checks to make sure it is within
        /// the board and not overwriting a clue cell.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <param name="cellValue"> Value to set the cell. </param>
        /// <returns> Return if the cells values was set. </returns>
        public bool SafeInsert(int inRow, int inCol, int cellValue)
        {
            if(inRow < 0 || inRow >= BoardSize ||
               inCol < 0 || inCol >= BoardSize)
            {
                throw new ArgumentException("Values out side of board range");
            }
            if(GetCell(inRow, inCol).IsPopulated)
            {
                return false;
            }
            if(IsSafe(inRow, inCol, cellValue))
            {
                Set(inRow, inCol, cellValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Inserts a single value with checks to make sure it is within
        /// the board, and not overwriting a clue cell.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <param name="cellPValue"> Value to set the cell as a list. </param>
        /// <returns> Return if the cells values was set. </returns>
        public bool SafeInsert(int inRow, int inCol, List<int>? cellPValue)
        {
            if(inRow < 0 || inRow >= BoardSize ||
               inCol < 0 || inCol >= BoardSize)
            {
                throw new ArgumentException("Values out side of board range");
            }
            if(cellPValue == null || cellPValue.Count < 1)
            {
                return false;
            }
            int cellValue = cellPValue[0];
            if(GetCell(inRow, inCol).IsPopulated)
            {
                return false;
            }
            if(IsSafe(inRow, inCol, cellValue))
            {
                Set(inRow, inCol, cellValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a smaller grid to represent the block number in a Grid.
        /// </summary>
        /// <param name="size"> The size of the blocks of the Grid. </param>
        /// <returns> Returns a populated map.</returns>
        public static int[,] SetCellBlock(int size)
        {
            int[,] blockIndex = new int[size, size];
            int Block = 0;
            for(int row = 0; row < size; row++)
            {
                for(int col = 0; col < size; col++)
                {
                    blockIndex[row, col] = Block;
                    Block++;
                }
            }
            return blockIndex;
        }

        /// <summary>
        /// Gets the block number from values.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <returns> Returns the block number. </returns>
        public int GetBlockPos(int inRow, int inCol)
        {
            int rowBlk = inRow / BlockSize;
            int colBlk = inCol / BlockSize;
            return BlockIndex[rowBlk, colBlk];
        }

        #endregion

        #region CellPosibilities
        private (int StartRow, int StartCol) GetBlockStart(int row, int col)
        {
            return (row / BlockSize * BlockSize, col / BlockSize * BlockSize);
        }

        /// <summary>
        /// Calculates the cells possibilities.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns a list of the values that can be safely inserted
        /// in to the cell. </returns>
        public List<int> CalcCellPoss(int inRow, int inCol)
        {
            List<int> possible = [];
            if(!GetCell(inRow, inCol).IsPopulated)
            {
                for(int i = 1; i <= BoardSize; i++)
                {
                    if(IsSafe(inRow, inCol, i))
                    {
                        possible.Add(i);
                    }
                }
            }
            return possible;
        }

        /// <summary>
        /// Gets the possibilities associated with a cell.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns a list of the values that can be safely inserted
        /// in to the cell. </returns>
        public List<int> GetCellPoss(int inRow, int inCol)
        {
            List<int> possible = [];
            Cell cell = GetCell(inRow,inCol);
            if(!cell.IsPopulated)
            {
                possible = cell.CellPossible;
            }
            return possible;
        }

        /// <summary>
        /// Gets all the possible values for each open cell in the
        /// selected inRow of the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <returns> Returns a list of the values that can be safely inserted
        /// in to the column's open cells. </returns>
        public List<int> GetRowPoss(int inRow)
        {
            List<int> possibilities = [];
            for(int col = 0; col < BoardSize; col++)
            {
                List<int> possibleCells = GetCellPoss(inRow, col);
                possibilities.AddRange(possibleCells);
            }
            return possibilities;
        }

        /// <summary>
        /// Gets all the possible values for each open cell in the
        /// selected column of the grid.
        /// </summary>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns a list of the values that can be safely
        /// inserted in to the column's open cells. </returns>
        public List<int> GetColumnPoss(int inCol)
        {
            List<int> possibilities = [];
            for(int row = 0; row < BoardSize; row++)
            {
                List<int> possibleCells = GetCellPoss(row, inCol);
                possibilities.AddRange(possibleCells);
            }
            return possibilities;
        }

        /// <summary>
        /// Gets all the possible values for each open cell in the 
        /// selected block of the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns a list of the values that can be safely inserted
        /// in to the block's open cells. </returns>
        public List<int> GetBlockPoss(int inRow, int inCol)
        {
            List<int> possibilities = [];

            (int rowBlk, int colBlk) = GetBlockStart(inRow, inCol);

            for(int row = rowBlk; row < rowBlk + BlockSize; row++)
            {
                for(int col = colBlk; col < colBlk + BlockSize; col++)
                {
                    List<int> possibleCells = GetCellPoss(row, col);
                    possibilities.AddRange(possibleCells);
                }
            }
            return possibilities;
        }

        /// <summary>
        /// Updates the cells possibilities in the row,
        /// column and block, by removing it.
        /// <see cref="UpdateRowPoss(int, int)"/>
        /// <see cref="UpdateColumnPoss(int, int)"/>
        /// <see cref="UpdateBlockPoss(int, int, int)"/>
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <param name="cellValue"> Value to remove from th cells. </param>
        /// <returns> Returns hte total number of cells updated. </returns>
        public void UpdatePoss(int inRow, int inCol, int cellValue)
        {
            UpdateRowPoss(inRow, cellValue);
            UpdateColumnPoss(inCol, cellValue);
            UpdateBlockPoss(inRow, inCol, cellValue);
            //return total;
        }

        /// <summary>
        /// Updates the cells possibilities in the row by removing it.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="cellValue"> Value to remove from th cells. </param>
        /// <returns> Returns the total number of cells updated. </returns>
        public int UpdateRowPoss(int inRow, int cellValue)
        {
            int numUpdated = 0;
            if(cellValue != 0)
            {
                //int numUpdated = 0;
                for(int col = 0; col < BoardSize; col++)
                {
                    if(GetCell(inRow, col).RemovePossibilities(cellValue))
                    {
                        numUpdated++;
                    }
                }
                return numUpdated;
            }

            for(int col = 0; col < BoardSize; col++)
            {
                GetCell(inRow, col).SetPossibilities(CalcCellPoss(inRow, col));
            }

            return numUpdated;
        }

        /// <summary>
        /// Updates the cells possibilities in the column by removing it.
        /// </summary>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <param name="cellValue"> Value to remove from th cells. </param>
        /// <returns> Returns the total number of cells updated. </returns>
        public int UpdateColumnPoss(int inCol, int cellValue)
        {
            int numUpdated = 0;
            if(cellValue != 0)
            {
                for(int row = 0; row < BoardSize; row++)
                {
                    if(GetCell(row, inCol).RemovePossibilities(cellValue))
                    {
                        numUpdated++;
                    }
                }
                return numUpdated;
            }

            for(int row = 0; row < BoardSize; row++)
            {
                GetCell(row, inCol).SetPossibilities(CalcCellPoss(row, inCol));
            }

            return numUpdated;
        }

        /// <summary>
        /// Updates the cells possibilities in the block, by removing it.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the selected cell. </param>
        /// <param name="cellValue"> Value to remove from th cells. </param>
        /// <returns> Returns the total number of cells updated. </returns>
        public int UpdateBlockPoss(int inRow, int inCol, int cellValue)
        {
            int numUpdated = 0;
            (int rowBlk, int colBlk) = GetBlockStart(inRow, inCol);

            if(cellValue != 0)
            {
                for(int row = rowBlk; row < rowBlk + BlockSize; row++)
                {
                    for(int col = colBlk; col < colBlk + BlockSize; col++)
                    {
                        if(GetCell(row, col).RemovePossibilities(cellValue))
                        {
                            numUpdated++;
                        }
                    }
                }
                return numUpdated;
            }
            for(int row = rowBlk; row < rowBlk + BlockSize; row++)
            {
                for(int col = colBlk; col < colBlk + BlockSize; col++)
                {
                    GetCell(row, col).SetPossibilities(CalcCellPoss(row, col));
                }
            }
            return numUpdated;
        }

        public bool RemovePoss(int inRow, int inCol, int cellValue)
        {
            return GetCell(inRow, inCol).RemovePossibilities(cellValue);
        }
        public bool RemovePoss(int inRow, int inCol, List<int> cellValue)
        {
            return GetCell(inRow, inCol).RemovePossibilities(cellValue);
        }

        public bool RemoveRowPoss(int inRow, int inCol, int cellValue)
        {
            bool inserted = false;
            for(int col = 0; col < BoardSize; col++)
            {
                if(col == inCol)
                {
                    continue;
                }
                if(GetCell(inRow, col).RemovePossibilities(cellValue))
                {
                    inserted = true;
                }
            }
            return inserted;
        }
        public bool RemoveColumnPoss(int inRow, int inCol, int cellValue)
        {
            bool inserted = false;
            for(int row = 0; row < BoardSize; row++)
            {
                if(row == inRow)
                {
                    continue;
                }
                if(GetCell(row, inCol).RemovePossibilities(cellValue))
                {
                    inserted = true;
                }
            }
            return inserted;
        }
        public bool RemoveBlockPoss(int inRow, int inCol, int cellValue)
        {
            bool inserted = false;
            (int rowBlk, int colBlk) = GetBlockStart(inRow, inCol);

            for(int row = rowBlk; row < rowBlk + BlockSize; row++)
            {
                for(int col = colBlk; col < colBlk + BlockSize; col++)
                {
                    Cell cell = GetCell(row, col);
                    if(cell.RemovePossibilities(cellValue))
                    {
                        inserted = true;
                    }
                }
            }
            return inserted;
        }
        #endregion

        #region GetUnpopulated
        /// <summary>
        /// Gets all the Given and Populated
        /// cells <see cref="Cell"/> in the Row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <returns> Returns all empty cells <see cref="Cell"/>. </returns>
        public List<Cell> GetUnPopRowCells(int inRow)
        {
            List<Cell> res = [];

            for(int col = 0; col < BoardSize; col++)
            {
                Cell cell = GetCell(inRow, col);
                if(!cell.IsPopulated)
                {
                    res.Add(cell);
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the selected column's values from the grid.
        /// </summary>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns all the know values in the column. </returns>
        public List<Cell> GetUnPopColumnCells(int inCol)
        {
            List<Cell> res = [];

            for(int innerRow = 0; innerRow < BoardSize; innerRow++)
            {
                Cell cell = GetCell(innerRow, inCol);
                if(!cell.IsPopulated)
                {
                    res.Add(cell);
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the selected block's values from the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns all the know values in the block. </returns>
        public List<Cell> GetUnPopBlockCells(int inRow, int inCol)
        {
            List<Cell> res = [];

            int rowBlk = inRow / BlockSize * BlockSize;
            int colBlk = inCol / BlockSize * BlockSize;

            for(int row = rowBlk; row < rowBlk + BlockSize; row++)
            {
                for(int col = colBlk; col < colBlk + BlockSize; col++)
                {
                    Cell cell = GetCell(row, col);
                    if(!cell.IsPopulated)
                    {
                        res.Add(cell);
                    }
                }
            }
            return res;
        }
        #endregion

        #region GetPopulated
        /// <summary>
        /// Gets all the Given and Populated
        /// cells <see cref="Cell"/> in the Row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <returns> Return the Given  cells <see cref="Cell"/>. </returns>
        public List<Cell> GetPopRowCells(int inRow)
        {
            List<Cell> res = [];

            for(int col = 0; col < BoardSize; col++)
            {
                Cell cell = GetCell(inRow, col);
                if(cell.IsPopulated)
                {
                    res.Add(cell);
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the selected column's values from the grid.
        /// </summary>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns all the know values
        /// in the selected column. </returns>
        public List<Cell> GetPopColumnCells(int inCol)
        {
            List<Cell> res = [];

            for(int innerRow = 0; innerRow < BoardSize; innerRow++)
            {
                Cell cell = GetCell(innerRow, inCol);
                if(cell.IsPopulated)
                {
                    res.Add(cell);
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the selected block's values from the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns all the know values
        public List<Cell> GetPopBlockCells(int inRow, int inCol)
        /// in the selected block. </returns>
        {
            List<Cell> res = [];
            (int rowBlk, int colBlk) = GetBlockStart(inRow, inCol);

            for(int row = rowBlk; row < rowBlk + BlockSize; row++)
            {
                for(int col = colBlk; col < colBlk + BlockSize; col++)
                {
                    Cell cell = GetCell(row, col);
                    if(cell.IsPopulated)
                    {
                        res.Add(cell);
                    }
                }
            }
            return res;
        }
        #endregion

        #region GetFull
        /// <summary>
        /// Gets all thecells <see cref="Cell"/> Populated Or Not in the Row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <returns> Returns all the Given and Populated cells
        /// <see cref="Cell"/>. </returns>
        public List<Cell> GetRowCells(int inRow)
        {
            List<Cell> res = [];

            for(int col = 0; col < BoardSize; col++)
            {
                res.Add(GetCell(inRow, col));
            }
            return res;
        }

        /// <summary>
        /// Gets the selected column's values from the grid.
        /// </summary>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns all the know values
        /// in the selected column. </returns>
        public List<Cell> GetColumnCells(int inCol)
        {
            List<Cell> res = [];

            for(int innerRow = 0; innerRow < BoardSize; innerRow++)
            {
                res.Add(GetCell(innerRow, inCol));
            }
            return res;
        }

        /// <summary>
        /// Gets the selected block's values from the grid.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns all the know values
        /// in the selected block. </returns>
        public List<Cell> GetBlockCells(int inRow, int inCol)
        {
            List<Cell> res = [];

            (int rowBlk, int colBlk) = GetBlockStart(inRow, inCol);

            for(int row = rowBlk; row < rowBlk + BlockSize; row++)
            {
                for(int col = colBlk; col < colBlk + BlockSize; col++)
                {
                    res.Add(GetCell(row, col));
                }
            }
            return res;
        }
        #endregion

        #region GetSameValues

        #endregion

        #region CheckConstrains

        /// <summary>
        /// Checks if the selected inRow contains the given value.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="cellValue"> Value to check in Selected inRow. </param>
        /// <returns> Returns true if not present. </returns>
        public bool CheckRowContains(int inRow, int cellValue)
        {
            List<Cell> cell = GetPopRowCells(inRow);
            List<int> cells = [];
            foreach(Cell cellCell in cell)
            {
                cells.Add(cellCell.CellValue);
            }
            if(cells.Count != BoardSize)
            {
                return !cells.Contains(cellValue);
            }
            else
            {
                return cells.Where(y => y == cellValue).Count() == 1;
            }
        }

        /// <summary>
        /// Checks if the selected column contains the given value.
        /// </summary>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <param name="cellValue"> Value to check in Selected column.</param>
        /// <returns> Returns true if not present. </returns>
        public bool CheckColumnContains(int inCol, int cellValue)
        {
            List<Cell> cell = GetPopColumnCells(inCol);
            List<int> cells = [];
            foreach(Cell cellCell in cell)
            {
                cells.Add(cellCell.CellValue);
            }
            if(cells.Count != BoardSize)
            {
                return !cells.Contains(cellValue);
            }
            else
            {
                return cells.Where(y => y == cellValue).Count() == 1;
            }
        }

        /// <summary>
        /// Checks if the selected block contains the given value.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <param name="cellValue"> Value to check in Selected block. </param>
        /// <returns> Returns true if not present. </returns>
        public bool CheckBlockContains(int inRow, int inCol, int cellValue)
        {
            List<Cell> cell = GetPopBlockCells(inRow, inCol);
            List<int> cells = [];
            foreach(Cell cellCell in cell)
            {
                cells.Add(cellCell.CellValue);
            }
            if(cells.Count != BoardSize)
            {
                return !cells.Contains(cellValue);
            }
            else
            {
                return cells.Where(y => y == cellValue).Count() == 1;
            }
        }

        /// <summary>
        /// Checks if the selected inRow contains the given value.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="cellValue"> Value to check in Selected inRow. </param>
        /// <returns> Returns true if not present. </returns>
        public bool CheckRowContains(Cell inCell)
        {
            int cellValue = inCell.CellValue;
            List<Cell> cell = GetPopRowCells(inCell.CellRow);
            List<int> cells = [];
            foreach(Cell cellCell in cell)
            {
                cells.Add(cellCell.CellValue);
            }
            if(cells.Count != BoardSize)
            {
                return !cells.Contains(cellValue);
            }
            else
            {
                return cells.Where(y => y == cellValue).Count() == 1;
            }
        }

        /// <summary>
        /// Checks if the selected column contains the given value.
        /// </summary>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <param name="cellValue"> Value to check in Selected column.</param>
        /// <returns> Returns true if not present. </returns>
        public bool CheckColumnContains(Cell inCell)
        {
            int cellValue = inCell.CellValue;
            List<Cell> cell = GetPopColumnCells(inCell.CellColumn);
            List<int> cells = [];
            foreach(Cell cellCell in cell)
            {
                cells.Add(cellCell.CellValue);
            }
            if(cells.Count != BoardSize)
            {
                return !cells.Contains(cellValue);
            }
            else
            {
                return cells.Where(y => y == cellValue).Count() == 1;
            }
        }

        /// <summary>
        /// Checks if the selected block contains the given value.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <param name="cellValue"> Value to check in Selected block. </param>
        /// <returns> Returns true if not present. </returns>
        public bool CheckBlockContains(Cell inCell)
        {
            int cellValue = inCell.CellValue;
            List<Cell> cell =GetPopBlockCells(inCell.CellRow,inCell.CellColumn);
            List<int> cells = [];
            foreach(Cell cellCell in cell)
            {
                cells.Add(cellCell.CellValue);
            }
            if(cells.Count != BoardSize)
            {
                return !cells.Contains(cellValue);
            }
            else
            {
                return cells.Where(y => y == cellValue).Count() == 1;
            }
        }
        /// <summary>
        /// Check the inRow, column and block, for the selected value.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <param name="cellValue"> Value to check against constraint.</param>
        /// <returns> Returns true if not present. </returns>
        public bool IsSafe(int inRow, int inCol, int cellValue)
        {
            if(!CheckRowContains(inRow, cellValue))
            {
                return false;
            }
            if(!CheckColumnContains(inCol, cellValue))
            {
                return false;
            }
            if(!CheckBlockContains(inRow, inCol, cellValue))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check the inRow, column and block, for the selected value.
        /// </summary>
        /// <param name="cell"> Cell to chick in grid. </param>
        /// <returns> Returns true if not present. </returns>
        public bool IsSafe(Cell cell)
        {
            if(cell.CellValue == 0)
            {
                return false;
            }
            if(!CheckRowContains(cell))
            {
                return false;
            }
            if(!CheckColumnContains(cell))
            {
                return false;
            }
            if(!CheckBlockContains(cell))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks that all the cells in the board meet
        /// the required constraints.
        /// </summary>
        /// <returns></returns>
        public bool VerifyBoard()
        {
            foreach(Cell cell in Grid)
            {
                if(!IsSafe(cell))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Displays
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string RowSep(int size)
        {
            StringBuilder sb = new StringBuilder("+");

            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    sb.Append("--");
                }
                sb.Append("-+");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Only use in ConsoleApp
        /// </summary>
        public void ColourBoardDisplay()
        {
            for(int row = 0; row < BoardSize; row++)
            {
                if(row % BlockSize == 0)
                {
                    Console.Write($"{RowSep(BlockSize * 3)}\n|");
                }
                else
                {
                    Console.Write($"|");
                }
                Console.WriteLine();
                for(int column = 0; column < BoardSize; column++)
                {
                    Cell cell = GetCell(row, column);
                    Console.Write($"|");
                    if(cell.IsGiven)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        if(cell.IsPopulated)
                        {
                            if(cell.IsCorrect())
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }
                    }
                    Console.Write($"{cell.ToString(),-4}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if(((column + 1) % BlockSize) == 0)
                    {
                        Console.Write($"|");
                    }
                }
                Console.Write($"|");
                Console.WriteLine();
            }
            Console.WriteLine($"\n{RowSep(BlockSize)}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToArray()
        {
            StringBuilder sb = new StringBuilder();
            for(int row = 0; row < BoardSize; row++)
            {
                sb.AppendLine();
                sb.Append('{');
                for(int column = 0; column < BoardSize; column++)
                {
                    sb.Append($"{GetCell(row, column).CellValue},");
                }
                sb.Append("},");
            }
            sb.AppendLine();
            return sb.ToString();
        }
        #endregion

    }
}
