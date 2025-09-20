namespace SudokuBoardLibrary
{
    public interface IBoard
    {
        int NumberOfBlocks { get; set; }
        int NumberOfCells { get; set; }

        static abstract string RowSep(int size);
        static abstract int[,] SetCellBlock(int size);

        bool Attempt(int inRow, int inCol, int cellValue);
        bool CheckBlockContains(int inRow, int inCol, int cellValue);
        bool CheckColumnContains(int inCol, int cellValue);
        bool CheckRowContains(int inRow, int cellValue);
        void ColourBoardDisplay();
        List<Cell> GetBlockCells(int inRow, int inCol);
        int GetBlockPos(int inRow, int inCol);
        List<int> GetBlockPoss(int inRow, int inCol);
        Cell GetCell(int inRow, int inCol);
        List<int> GetCellPoss(int inRow, int inCol);
        List<int> CalcCellPoss(int inRow, int inCol);
        List<Cell> GetColumnCells(int inCol);
        List<int> GetColumnPoss(int inCol);
        List<Cell> GetPopBlockCells(int inRow, int inCol);
        List<Cell> GetPopColumnCells(int inCol);
        List<Cell> GetPopRowCells(int inRow);
        List<Cell> GetRowCells(int inRow);
        List<int> GetRowPoss(int inRow);
        List<Cell> GetUnPopBlockCells(int inRow, int inCol);
        List<Cell> GetUnPopColumnCells(int inCol);
        List<Cell> GetUnPopRowCells(int inRow);
        bool IsSafe(Cell cell);
        bool IsSafe(int inRow, int inCol, int cellValue);
        bool RemoveBlockPoss(int inRow, int inCol, int cellValue);
        bool RemoveColumnPoss(int inRow, int inCol, int cellValue);
        bool RemovePoss(int inRow, int inCol, int cellValue);
        bool RemovePoss(int inRow, int inCol, List<int> cellValue);
        bool RemoveRowPoss(int inRow, int inCol, int cellValue);

        void ResetBoard();
        bool SafeInsert(int inRow, int inCol, int cellValue);
        bool SafeInsert(int inRow, int inCol, List<int>? cellPValue);

        bool Set(int inRow, int inCol, int cellValue);

        void SetFinal();

        void SetGiven(Cell cell);
        void SetGiven(int inRow, int inCol);

        void SetOpen(Cell cell);
        void SetOpen(int inRow, int inCol);

        void SetValidSolution();

        string ToArray();

        void UpdatePoss(int inRow, int inCol, int cellValue);
        int UpdateRowPoss(int inRow, int cellValue);
        int UpdateColumnPoss(int inCol, int cellValue);
        int UpdateBlockPoss(int inRow, int inCol, int cellValue);

        bool VerifyBoard();
    }
}
