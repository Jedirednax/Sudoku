using System.Diagnostics;

namespace SudokuBoardLibrary
{
    public enum SolveMethod
    {
        Constraint,
        Eliminate,
        RemainingBlocks,
        ObviousPair,
        ObviousTriple,
        HiddenPair,
        HiddenTriple,
        PointingPair,
        PointingTriple,
        XWing,
        XYWing
    }
    /// <summary>
    /// Solves Sudoku boards using basic to advanced Strategies.
    /// </summary>
    public class Solver : ISolver
    {
        public Board board;
        // TODO Add Hidden triple solving
        // TODO Add XY-Wing
        // TODO Add XY-WingBlock
        // TODO Add Y-Wing
        // TODO Add SwordFish

        public Solver(Board board)
        {
            this.board = board;
        }

        public List<SolveMethod> MethodsUsed = [];
        #region Solving Techiques
        #region Constraint
        /// <summary>
        /// Loops through the board's open cells and checks if there is only,
        /// one possible value based off of the constraints.
        /// </summary>
        /// <returns> Returns true if was able to insert a value. </returns>
        public bool ConstraintSolve()
        {
            bool inserted = false;
            foreach(Cell va in board.openCells)
            {
                int outerRow = va.CellRow;
                int innerCol = va.CellColumn;
                if(!board.GetCell(outerRow, innerCol).IsPopulated)
                {
                    List<int>? tempPos = board.GetCellPoss(outerRow, innerCol);
                    if(tempPos == null || tempPos.Count != 1)
                    {
                        continue;
                    }
                    if(board.SafeInsert(outerRow, innerCol, tempPos))
                    {
                        inserted = true;
                        break;
                    }
                }
            }
            return inserted;
        }
        #endregion

        #region Elimination

        /// <summary>
        /// Checks for if a value can only be placed in a single cell, 
        /// based off of it is the only value in the inRow, column or block.
        /// If a single value is return in the Elim lists,
        /// then is is inserted. <see cref="SafeInsert(int, int, List{int})"/>
        /// </summary>
        /// <returns> Returns true if was able to insert a value. </returns>
        public bool EliminateSolve()
        {
            bool inserted = false;
            foreach(Cell va in board.openCells)
            {
                int outerRow = va.CellRow;
                int innerCol = va.CellColumn;
                if(board.SafeInsert(outerRow, innerCol,
                    ElimRow(outerRow, innerCol)))
                {
                    inserted = true;
                    break;
                }
                else if(board.SafeInsert(outerRow, innerCol,
                    ElimCol(outerRow, innerCol)))
                {
                    inserted = true;
                    break;
                }
                else if(board.SafeInsert(outerRow, innerCol,
                    ElimBlock(outerRow, innerCol)))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Loops through the selected inRow, and get all the possibilities,
        /// skipping the selected cell.
        /// Then loops though and checks if it has any values.
        /// If they are not contained in the remaining cells.
        /// Then if a single value is left returns those values.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="currCol"> Column of the Selected cell. </param>
        /// <returns> Returns a list of values the selected cell contains,
        /// that are nor present in the other Cells
        /// as possibilities in the inRow. </returns>
        public List<int> ElimRow(int inRow, int currCol)
        {
            List<int> currRow = board.GetCellPoss(inRow, currCol);

            List<int> hold = [];
            for(int innerCol = 0; innerCol < board.BoardSize; innerCol++)
            {
                if(innerCol == currCol)
                {
                    continue;
                }
                List<int> possibleCells = board.GetCellPoss(inRow, innerCol);
                hold.AddRange(possibleCells);
            }
            List<int> res = [];
            foreach(int i in currRow)
            {
                if(!hold.Contains(i))
                {
                    res.Add(i);
                }
            }
            return res;
        }

        /// <summary>
        /// Loops through the selected column, and get all the possibilities,
        /// skipping the selected cell.
        /// Then loops though and checks if it has any values
        /// that are not contained in the remaining cells.
        /// THen if a single value is left returns those values.
        /// </summary>
        /// <param name="currRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns a list of values the selected cell contains
        /// that are nor present in the other Cells as
        /// possibilities in the Column. </returns>
        public List<int> ElimCol(int currRow, int inCol)
        {
            List<int> currCol = board.GetCellPoss(currRow, inCol);

            List<int> hold = [];
            for(int innerRow = 0; innerRow < board.BoardSize; innerRow++)
            {
                if(innerRow == currRow)
                {
                    continue;
                }
                List<int>? possibleCells = board.GetCellPoss(innerRow, inCol);
                if(possibleCells != null)
                {
                    hold.AddRange(possibleCells);
                }
            }
            List<int> res = [];
            foreach(int i in currCol)
            {
                if(!hold.Contains(i))
                {
                    res.Add(i);
                }
            }
            return res;
        }

        /// <summary>
        /// Loops through the selected Block, and get all the possibilities,
        /// skipping the selected cell.
        /// Then loops though and checks if it has any values
        /// that are not contained in the remaining cells.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns a list of values the selected cell contains
        /// that are nor present in the other Cells as
        /// possibilities in the block. </returns>
        public List<int>? ElimBlock(int inRow, int inCol)
        {
            List<int> currBlk = board.GetCellPoss(inRow, inCol);

            List<int> hold = [];
            int rowBlock = inRow / board.BlockSize * board.BlockSize;
            int columnBlock = inCol / board.BlockSize * board.BlockSize;

            for(int outerRow = rowBlock;
                outerRow < rowBlock + board.BlockSize; outerRow++)
            {
                for(int innerCol = columnBlock;
                    innerCol < columnBlock + board.BlockSize; innerCol++)
                {
                    if(!(outerRow == inRow && innerCol == inCol))
                    {
                        continue;
                    }
                    List<int> possibleCells = board.GetCellPoss(outerRow, innerCol);
                    hold.AddRange(possibleCells);
                }
            }
            List<int> res = [];

            foreach(int g in currBlk)
            {
                if(!hold.Contains(g))
                {
                    res.Add(g);
                }
            }
            return res;
        }
        #endregion

        #region BackTracking
        /// <summary>
        /// Helper method to call backtracking solve with out specifying cords.
        /// </summary>
        /// <returns>  </returns>
        public bool BackTrackingSolve()
        {
            return BackTrackingSolve(0, 0);
        }
        /// <summary>
        /// A recursive method to solve a board using backtracking,
        /// with cell possibilities,
        /// and will generate a solution,
        /// even with "Unsolvable pairs".
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a cell was possible </returns>
        public bool BackTrackingSolve(int inRow, int inCol)
        {
            if(inRow == board.BoardSize - 1 && inCol == board.BoardSize)
            {
                return true;
            }
            if(inCol == board.BoardSize)
            {
                inRow += 1;
                inCol = 0;
            }
            if(board.GetCell(inRow, inCol).IsGiven)
            {
                inCol += 1;
                return BackTrackingSolve(inRow, inCol);
            }

            foreach(int cell in board.CalcCellPoss(inRow, inCol))
            {
                if(board.SafeInsert(inRow, inCol, cell))
                {
                    if(BackTrackingSolve(inRow, inCol + 1))
                    {
                        return true;
                    }
                }
                board.Set(inRow, inCol, 0);
            }
            return false;
        }
        #endregion

        #region Remaing
        /// <summary>
        /// Checks values do not eliminate a row column or block by having only a certain value,
        /// present on only one row or column in a block.
        /// </summary>
        /// <returns>  </returns>
        public bool RemainingBlocks()
        {
            bool inserted = false;
            foreach(Cell t in board.openCells)
            {
                if(RemainingBlockRow(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
                if(RemainingBlockColumn(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks the columns for pointing pairs.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if an instance of pointing pairs was found. </returns>
        public bool RemainingBlockRow(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            int cellBlock = startCell.CellBlock;
            HashSet<int> tempList = [];
            foreach(Cell outtter in board.GetUnPopRowCells(startCell.CellRow))
            {
                if(outtter.CellBlock == startCell.CellBlock)
                {
                    continue;
                }
                foreach(int cp in outtter.CellPossible)
                {
                    tempList.Add(cp);
                }
            }
            List<int> tempHOld = [];
            List<int> hold = startCell.CellPossible;
            foreach(int g in hold)
            {
                if(!tempList.Contains(g))
                {
                    tempHOld.Add(g);
                }
            }

            List<Cell> blockOther = board.GetUnPopBlockCells(inRow, inCol);

            foreach(Cell cl in blockOther)
            {
                if(cl.CellRow == startCell.CellRow)
                {
                    continue;
                }
                if(board.RemovePoss(cl.CellRow, cl.CellColumn, tempHOld))
                {
                    inserted = true;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks the columns for pointing pairs.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if an instance of pointing pairs was found. </returns>
        public bool RemainingBlockColumn(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            int cellBlock = startCell.CellBlock;
            HashSet<int> tempList = [];
            foreach(Cell outtter in board.GetUnPopColumnCells(startCell.CellColumn))
            {
                if(outtter.CellBlock == startCell.CellBlock)
                {
                    continue;
                }
                foreach(int cp in outtter.CellPossible)
                {
                    tempList.Add(cp);
                }
            }
            List<int> tempHOld = [];
            List<int> hold = startCell.CellPossible;
            foreach(int g in hold)
            {
                if(!tempList.Contains(g))
                {
                    tempHOld.Add(g);
                }
            }

            List<Cell> blockOther = board.GetUnPopBlockCells(inRow, inCol);
            foreach(Cell cl in blockOther)
            {
                if(cl.CellColumn == startCell.CellColumn)
                {
                    continue;
                }
                if(cl.RemovePossibilities(tempHOld))
                {
                    inserted = true;
                }
            }
            return inserted;
        }
        #endregion

        #region ObviousPairs
        /// <summary>
        /// Checks the board for two cells that only certain values.
        /// That can be entered in and no where else.
        /// Like two cells both only contains 5 and 7,
        /// so the rest cannot be 5 or 7 in the block row or column.
        /// </summary>
        /// <returns> Returns if a cell was filled. </returns>
        public bool ObviousPair()
        {
            bool inserted = false;
            foreach(Cell cl in board.openCells)
            {
                if(cl.CellPossible.Count == 2)
                {
                    if(ObviousPairRow(cl.CellRow, cl.CellColumn))
                    {
                        inserted = true;
                        break;
                    }
                    if(ObviousPairCol(cl.CellRow, cl.CellColumn))
                    {
                        inserted = true;
                        break;
                    }
                    if(ObviousPairBlock(cl.CellRow, cl.CellColumn))
                    {
                        inserted = true;
                        break;
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious pairs in a selected row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool ObviousPairRow(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            List<Cell> hold = board.GetUnPopRowCells(inRow);
            hold.Remove(mainCell);

            foreach(Cell g in hold)
            {
                if(mainCell.ComparePossibilitiesPair(g))
                {
                    hold.Remove(g);
                    foreach(Cell t in hold)
                    {
                        if(t.RemovePossibilities(mainCell.CellPossible))
                        {
                            inserted = true;
                        }
                    }
                    return inserted;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious pairs in a selected column.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool ObviousPairCol(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            List<Cell> hold = board.GetUnPopColumnCells(inCol);
            hold.Remove(mainCell);

            foreach(Cell g in hold)
            {
                if(mainCell.ComparePossibilitiesPair(g))
                {
                    hold.Remove(g);
                    foreach(Cell t in hold)
                    {
                        if(t.RemovePossibilities(mainCell.CellPossible))
                        {
                            inserted = true;
                        }
                    }
                    return inserted;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious pairs in a selected block.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool ObviousPairBlock(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            List<Cell> hold = board.GetUnPopBlockCells(inRow, inCol);
            hold.Remove(mainCell);

            foreach(Cell g in hold)
            {
                if(mainCell.ComparePossibilitiesPair(g))
                {
                    hold.Remove(g);
                    foreach(Cell t in hold)
                    {
                        if(t.RemovePossibilities(mainCell.CellPossible))
                        {
                            inserted = true;
                        }
                    }
                    return inserted;
                }
            }
            return inserted;
        }
        #endregion

        #region ObviousTriples

        /// <summary>
        /// Checks the board for three cells that only certain values.
        /// can be entered in and no where else.
        /// like three cells that can only contain 5, 8 and 7,
        /// so the rest cannot be 5,8  or 7 in the block row or column.
        /// </summary>
        /// <returns> Returns if a cell was filled. </returns>
        public bool ObviousTrip()
        {
            bool inserted = false;
            foreach(Cell cl in board.openCells)
            {
                if(ObviousTripRow(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }
                else if(ObviousDisTripRow(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }

                if(ObviousTripCol(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }
                else if(ObviousDisTripCol(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }

                if(ObviousTripBlock(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }
                else if(ObviousDisTripBlock(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }
        #region Tripple
        /// <summary>
        /// Checks if the cells contain the tree same possible values only.
        /// </summary>
        /// <param name="main"> the cell to compare to another cell. </param>
        /// <param name="comp"> the Cell being compared. </param>
        /// <returns> Returns if the cells have the same three possible values. </returns>
        public static bool ComparePossibilitiesTripple(Cell main, Cell comp)
        {
            if(main == null || comp == null)
            {
                return false;
            }
            if(main.CellPossible == null)
            {
                return false;
            }
            if(comp.CellPossible == null)
            {
                return false;
            }
            if(main.CellPossible.Count > 3)
            {
                return false;
            }
            if(comp.CellPossible.Count > 3)
            {
                return false;
            }
            foreach(int ce in comp.CellPossible)
            {
                if(!main.CellPossible.Contains(ce))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks for obvious triples in a selected row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool ObviousTripRow(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;

            List<Cell> hold = board.GetUnPopRowCells(inRow);
            hold.Remove(mainCell);
            List<Cell> temp = [];
            foreach(Cell g in hold)
            {
                if(ComparePossibilitiesTripple(mainCell, g))
                {
                    temp.Add(g);
                }
            }

            if(temp.Count == 2)
            {
                foreach(Cell g in temp)
                {
                    hold.Remove(g);
                }
                foreach(Cell t in hold)
                {
                    if(t.RemovePossibilities(mainCell.CellPossible))
                    {
                        inserted = true;
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious triples in a selected column.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool ObviousTripCol(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;

            List<Cell> hold = board.GetUnPopColumnCells(inCol);

            hold.Remove(mainCell);
            List<Cell> temp = [];
            foreach(Cell g in hold)
            {
                if(ComparePossibilitiesTripple(mainCell, g))
                {
                    temp.Add(g);
                }
            }

            if(temp.Count == 2)
            {
                foreach(Cell g in temp)
                {
                    hold.Remove(g);
                }
                foreach(Cell t in hold)
                {
                    if(t.RemovePossibilities(mainCell.CellPossible))
                    {
                        inserted = true;
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious triples in a selected block.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool ObviousTripBlock(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;

            List<Cell> hold = board.GetUnPopBlockCells(inRow, inCol);
            List<Cell> temp = [];

            hold.Remove(mainCell);
            foreach(Cell g in hold)
            {
                if(ComparePossibilitiesTripple(mainCell, g))
                {
                    temp.Add(g);
                }
            }

            if(temp.Count == 2)
            {
                foreach(Cell g in temp)
                {
                    hold.Remove(g);
                }
                foreach(Cell t in hold)
                {
                    if(t.RemovePossibilities(mainCell.CellPossible))
                    {
                        inserted = true;
                    }
                }
            }
            return inserted;
        }
        #endregion
        #region DistributedTripple
        /// <summary>
        /// Checks a row for three values across three cells,
        /// and if they are distributed in sets of two across a row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns>  </returns>
        public bool ObviousDisTripRow(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            if(mainCell.CellPossible.Count == 2)
            {
                List<Cell> hold = board.GetUnPopRowCells(inRow);
                List<Cell> temp = [mainCell];
                hold.Remove(mainCell);
                HashSet<int> ints = [.. mainCell.CellPossible];
                foreach(Cell g in hold)
                {
                    if(g.CellPossible.Count == 2)
                    {
                        foreach(int t in g.CellPossible)
                        {
                            if(ints.Contains(t))
                            {
                                foreach(int k in g.CellPossible)
                                {
                                    ints.Add(k);
                                }
                                if(ints.Count > 3)
                                {
                                    return inserted;
                                }
                                temp.Add(g);
                                break;
                            }
                        }
                    }
                }
                if(temp.Count == 3)
                {
                    foreach(Cell g in temp)
                    {
                        hold.Remove(g);
                    }
                    foreach(Cell t in hold)
                    {
                        if(t.RemovePossibilities(mainCell.CellPossible))
                        {
                            inserted = true;
                        }
                    }
                }
            }
            return inserted;
        }
        /// <summary>
        /// Checks a row for three values across three cells,
        /// and if they are distributed in sets of two across a row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns>  </returns>
        public bool ObviousDisTripCol(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            if(mainCell.CellPossible.Count == 2)
            {
                List<Cell> hold = board.GetUnPopColumnCells( inCol);
                List<Cell> temp = [mainCell];
                hold.Remove(mainCell);
                HashSet<int> ints = [.. mainCell.CellPossible];
                foreach(Cell g in hold)
                {
                    if(g.CellPossible.Count == 2)
                    {
                        foreach(int t in g.CellPossible)
                        {
                            if(ints.Contains(t))
                            {
                                foreach(int k in g.CellPossible)
                                {
                                    ints.Add(k);
                                }
                                if(ints.Count > 3)
                                {
                                    return inserted;
                                }
                                temp.Add(g);
                                break;
                            }
                        }
                    }
                }
                if(temp.Count == 3)
                {
                    foreach(Cell g in temp)
                    {
                        hold.Remove(g);
                    }
                    foreach(Cell t in hold)
                    {
                        if(t.RemovePossibilities(mainCell.CellPossible))
                        {
                            inserted = true;
                        }
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks a row for three values across three cells,
        /// and if they are distributed in sets of two across a row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns>  </returns>
        public bool ObviousDisTripBlock(int inRow, int inCol)
        {
            Cell mainCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            if(mainCell.CellPossible.Count == 2)
            {
                List<Cell> hold = board.GetUnPopBlockCells(inRow, inCol);
                List<Cell> temp = [mainCell];
                hold.Remove(mainCell);
                HashSet<int> ints = [.. mainCell.CellPossible];
                foreach(Cell g in hold)
                {
                    if(g.CellPossible.Count == 2)
                    {
                        foreach(int t in g.CellPossible)
                        {
                            if(ints.Contains(t))
                            {
                                foreach(int k in g.CellPossible)
                                {
                                    ints.Add(k);
                                }
                                if(ints.Count > 3)
                                {
                                    return inserted;
                                }
                                temp.Add(g);
                                break;
                            }
                        }
                    }
                }
                if(temp.Count == 3)
                {
                    foreach(Cell g in temp)
                    {
                        hold.Remove(g);
                    }
                    foreach(Cell t in hold)
                    {
                        if(t.RemovePossibilities(mainCell.CellPossible))
                        {
                            inserted = true;
                        }
                    }
                }
            }
            return inserted;
        }
        #endregion
        #endregion

        #region Hidden Paris
        /// <summary>
        /// Checks the board for two cells that only certain values.
        /// That can be entered in and no where else.
        /// Like two cells both only contains 5 and 7,
        ///
        /// get the test cell
        /// check what values it contains
        /// Then search for if any other cells contain those values,
        /// check that only one other cell has those values.
        /// so the rest cannot be 5 or 7 in the block row or column.
        /// </summary>
        /// <returns> Returns if a cell was filled. </returns>
        public bool HiddenPair()
        {
            bool inserted = false;
            foreach(Cell cl in board.openCells)
            {
                if(HiddenPairRow(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }
                if(HiddenPairCol(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }
                if(HiddenPairBlock(cl.CellRow, cl.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious pairs in a selected row.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool HiddenPairRow(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow,inCol);
            bool inserted = false;
            List<int> working = board.GetRowPoss(inRow);
            List<int> possv = [];
            foreach(int cellPos in startCell.CellPossible)
            {
                if(working.Where(y => y == cellPos).Count() == 2)
                {
                    possv.Add(cellPos);
                }
            }
            if(possv.Count == 2)
            {
                foreach(Cell cell in board.GetUnPopRowCells(inRow))
                {
                    if(cell.ComparePosition(startCell) ||
                        cell.CellPossible.Count == 2)
                    {
                        continue;
                    }

                    if(cell.CellPossible.Contains(possv[0]) &&
                        cell.CellPossible.Contains(possv[1]))
                    {
                        startCell.CellPossible.RemoveAll(x => !possv.Contains(x));
                        cell.CellPossible.RemoveAll(x => !possv.Contains(x));
                        inserted = true;
                        break;
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious pairs in a selected column.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool HiddenPairCol(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow,inCol);
            bool inserted = false;
            List<int> working = board.GetColumnPoss(inCol);
            List<int> possv = [];
            foreach(int cellPos in startCell.CellPossible)
            {
                if(working.Where(y => y == cellPos).Count() == 2)
                {
                    possv.Add(cellPos);
                }
            }
            if(possv.Count == 2)
            {
                foreach(Cell cell in board.GetUnPopColumnCells(inCol))
                {
                    if(cell.ComparePosition(startCell) ||
                        cell.CellPossible.Count == 2)
                    {
                        continue;
                    }

                    if(cell.CellPossible.Contains(possv[0]) &&
                        cell.CellPossible.Contains(possv[1]))
                    {
                        startCell.CellPossible.RemoveAll(x => !possv.Contains(x));
                        cell.CellPossible.RemoveAll(x => !possv.Contains(x));
                        inserted = true;
                        break;
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks for obvious pairs in a selected block.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if a possibility was found or entered. </returns>
        public bool HiddenPairBlock(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow,inCol);
            bool inserted = false;
            List<int> working = board.GetBlockPoss(inRow,inCol);
            List<int> possv = [];
            foreach(int cellPos in startCell.CellPossible)
            {
                if(working.Where(y => y == cellPos).Count() == 2)
                {
                    possv.Add(cellPos);
                }
            }
            if(possv.Count == 2)
            {
                foreach(Cell cell in board.GetUnPopBlockCells(inRow, inCol))
                {
                    if(cell.ComparePosition(startCell) ||
                        cell.CellPossible.Count == 2)
                    {
                        continue;
                    }

                    if(cell.CellPossible.Contains(possv[0]) &&
                        cell.CellPossible.Contains(possv[1]))
                    {
                        startCell.CellPossible.RemoveAll(x => !possv.Contains(x));
                        cell.CellPossible.RemoveAll(x => !possv.Contains(x));
                        inserted = true;
                        break;
                    }
                }
            }
            return inserted;
        }
        #endregion

        #region HiddenTriple

        public bool HiddenTriple()
        {
            bool inserted = false;
            foreach(Cell t in board.openCells)
            {
                //Debug.WriteLine(t.PrintDebug(), "HD Start");
                //if(HiddenTripleRow(t.CellRow, t.CellColumn))
                //{
                //    inserted = true;
                //    break;
                //}
                if(HiddenTripleCol(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        public bool HiddenTripleRow(int inRow, int inCol)
        {

            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;

            if(startCell.CellPossible.Count < 3)
            {
                return false;
            }
            List<int> rowPoss = board.GetRowPoss(inRow);
            List<int> targetValues = [];

            foreach(int cellPos in startCell.CellPossible)
            {
                int count = rowPoss.Count(y => y == cellPos);
                if(count >= 2 && count <= 3)
                {
                    targetValues.Add(cellPos);
                }
            }

            if(targetValues.Count != 3)
            {
                return false;
            }
            // find all cells in the row that contain any of the target values
            List<Cell> candidateCells = board.GetUnPopRowCells(inRow)
        .Where(c => c.CellPossible.Any(p => targetValues.Contains(p)))
        .ToList();

            if(candidateCells.Count != 3)
            {
                return false;
            }
            foreach(Cell tgCell in candidateCells)
            {
                int removed = tgCell.CellPossible.RemoveAll(x => !targetValues.Contains(x));
                if(removed > 0)
                {
                    Debug.WriteLine(tgCell.PrintDebug(), "HiddenTripleRow Remove");
                    inserted = true;
                }
            }

            return inserted;
        }

        // TODO FIX InfinateLoop
        public bool HiddenTripleCol(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;

            if(startCell.CellPossible.Count < 3)
            {
                return false;
            }

            List<int> colPoss = board.GetColumnPoss(inCol);
            List<int> targetValues = [];

            foreach(int cellPos in startCell.CellPossible)
            {
                int count = colPoss.Count(y => y == cellPos);
                if(count >= 2 && count <= 3)
                {
                    targetValues.Add(cellPos);
                }
            }

            if(targetValues.Count != 3)
            {
                return false;
            }

            // find all cells in the col that contain any of the target values
            List<Cell> candidateCells = board.GetUnPopColumnCells(inCol)
        .Where(c => c.CellPossible.Any(p => targetValues.Contains(p)))
        .ToList();

            if(candidateCells.Count != 3)
            {
                return false;
            }

            foreach(Cell tgCell in candidateCells)
            {
                int removed = tgCell.CellPossible.RemoveAll(x => !targetValues.Contains(x));
                if(removed > 0)
                {
                    Debug.WriteLine(tgCell.PrintDebug(), "HiddenTripleCol Remove");
                    inserted = true;
                }
            }

            return inserted;
        }
        #endregion

        #region PointingPairs
        /// <summary>
        /// Checks values do not eliminate a row column or block by having only a certain value,
        /// present on only one row or column in a block.
        /// </summary>
        /// <returns>  </returns>
        public bool PointingPair()
        {
            bool inserted = false;
            foreach(Cell t in board.openCells)
            {
                if(PointingPairRow(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
                if(PointingPairCol(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks the columns for pointing pairs.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if an instance of pointing pairs was found. </returns>
        public bool PointingPairCol(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            // GetCell cells block.
            // Loop through other columns of block.
            List<int> fullBlock = board.GetBlockPoss(inRow, inCol);

            HashSet<int> uniqueValues = [.. fullBlock];

            // Check if column has possibilities.
            int blockSize = board.BlockSize;
            int rowOffset = inRow / board.BlockSize * board.BlockSize;
            int colOffset = inCol / board.BlockSize * board.BlockSize;
            int co = 0;
            for(int blkRow = rowOffset; blkRow < (rowOffset + blockSize); blkRow++)
            {
                List<int>? j = board.GetCellPoss(blkRow, inCol);
                if(j != null)
                {
                    // If they don't remove from current column.
                    foreach(int f in j)
                    {
                        fullBlock.Remove(f);
                    }
                    co++;
                }
            }
            if(co < 2)
            {
                return inserted;
            }

            // Check oif the contain any values form current column.
            int missingValue = 0;
            foreach(int g in uniqueValues)
            {
                if(!fullBlock.Contains(g))
                {
                    missingValue = g;
                    break;
                }
            }

            if(missingValue > 0)
            {
                for(int row = 0; row < board.BoardSize; row++)
                {
                    if(board.GetCell(row, inCol).CellBlock != startCell.CellBlock)
                    {
                        if(board.GetCell(row, inCol).RemovePossibilities(missingValue))
                        {
                            inserted = true;
                        }
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks the columns for pointing pairs.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if an instance of pointing pairs was found. </returns>
        public bool PointingPairRow(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            // GetCell cells block.
            // Loop through other columns of block.
            List<int> fullBlock = board.GetBlockPoss(inRow, inCol);

            HashSet<int> uniqueValues = [.. fullBlock];

            // CHeck if column has possibilities.
            int blockSize = board.BlockSize;
            int rowOffset = inRow / board.BlockSize * board.BlockSize;
            int colOffset = inCol / board.BlockSize * board.BlockSize;
            int ro = 0;
            for(int blkCol = colOffset; blkCol < (colOffset + blockSize); blkCol++)
            {
                List<int>? j = board.GetCellPoss(inRow, blkCol);
                if(j != null)
                {
                    // If they don't remove from current column.
                    foreach(int f in j)
                    {
                        fullBlock.Remove(f);
                    }
                    ro++;
                }
            }
            if(ro < 2)
            {
                return inserted;
            }

            // Check oif the contain any values form current column.
            int missingValue = 0;
            foreach(int g in uniqueValues)
            {
                if(!fullBlock.Contains(g))
                {
                    missingValue = g;
                    break;
                }
            }

            if(missingValue > 0)
            {
                for(int col = 0; col < board.BoardSize; col++)
                {
                    if(board.GetCell(inRow, col).CellBlock != startCell.CellBlock)
                    {
                        if(board.GetCell(inRow, col).RemovePossibilities(missingValue))
                        {
                            inserted = true;
                        }
                    }
                }
            }
            return inserted;
        }
        #endregion

        #region Pointing Triple
        public bool PointingTriple()
        {
            bool inserted = false;
            foreach(Cell t in board.openCells)
            {
                if(PointingTripleRow(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
                if(PointingTripleCol(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks the columns for pointing pairs.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if an instance of pointing triples was found. </returns>
        public bool PointingTripleCol(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            // GetCell cells block.
            // Loop through other columns of block.
            List<int> fullBlock = board.GetBlockPoss(inRow, inCol);

            HashSet<int> uniqueValues = [.. fullBlock];

            // Check if column has possibilities.
            int blockSize = board.BlockSize;
            int rowOffset = inRow / board.BlockSize * board.BlockSize;
            int colOffset = inCol / board.BlockSize * board.BlockSize;
            int co = 0;
            for(int blkRow = rowOffset; blkRow < (rowOffset + blockSize); blkRow++)
            {
                List<int>? j = board.GetCellPoss(blkRow, inCol);
                if(j != null)
                {
                    // If they don't remove from current column.
                    foreach(int f in j)
                    {
                        fullBlock.Remove(f);
                    }
                    co++;
                }
            }
            if(co < 2)
            {
                return inserted;
            }

            // Check oif the contain any values form current column.
            int missingValue = 0;
            foreach(int g in uniqueValues)
            {
                if(!fullBlock.Contains(g))
                {
                    missingValue = g;
                    break;
                }
            }

            if(missingValue > 0)
            {
                for(int row = 0; row < board.BoardSize; row++)
                {
                    if(board.GetCell(row, inCol).CellBlock != startCell.CellBlock)
                    {
                        if(board.GetCell(row, inCol).RemovePossibilities(missingValue))
                        {
                            inserted = true;
                        }
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks the columns for pointing pairs.
        /// </summary>
        /// <param name="inRow"> Row of the Selected cell. </param>
        /// <param name="inCol"> Column of the Selected cell. </param>
        /// <returns> Returns if an instance of pointing triples was found. </returns>
        public bool PointingTripleRow(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            // GetCell cells block.
            // Loop through other columns of block.
            List<int> fullBlock = board.GetBlockPoss(inRow, inCol);

            HashSet<int> uniqueValues = [.. fullBlock];

            // CHeck if column has possibilities.
            int blockSize = board.BlockSize;
            int rowOffset = inRow / board.BlockSize * board.BlockSize;
            int colOffset = inCol / board.BlockSize * board.BlockSize;
            int ro = 0;
            for(int blkCol = colOffset; blkCol < (colOffset + blockSize); blkCol++)
            {
                List<int>? j = board.GetCellPoss(inRow, blkCol);
                if(j != null)
                {
                    // If they don't remove from current column.
                    foreach(int f in j)
                    {
                        fullBlock.Remove(f);
                    }
                    ro++;
                }
            }
            if(ro < 2)
            {
                return inserted;
            }

            // Check oif the contain any values form current column.
            int missingValue = 0;
            foreach(int g in uniqueValues)
            {
                if(!fullBlock.Contains(g))
                {
                    missingValue = g;
                    break;
                }
            }

            if(missingValue > 0)
            {
                for(int col = 0; col < board.BoardSize; col++)
                {
                    if(board.GetCell(inRow, col).CellBlock == startCell.CellBlock)
                    {
                        continue;
                    }
                    if(board.GetCell(inRow, col).RemovePossibilities(missingValue))
                    {
                        inserted = true;
                    }
                }
            }
            return inserted;
        }
        #endregion

        #region XY-Wing
        public bool XYWing()
        {
            bool inserted = false;
            foreach(Cell t in board.openCells)
            {
                if(XYWingRowCol(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
                if(XYWingBlockRow(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
                if(XYWingBlockCol(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        public bool XYWingRowCol(int inRow, int inCol)
        {
            // Get starting cell
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            if(startCell.CellPossible.Count != 2)
            {
                return inserted;
            }

            Cell rowCell = FindRow(startCell);
            Cell columnCell = FindColumn(startCell, rowCell);
            // Check each Column in the row for matching values

            Cell targetCell = board.GetCell(columnCell.CellRow, rowCell.CellColumn);
            if(targetCell != null &&
                targetCell.CellPossible != null &&
                !targetCell.ComparePosition(rowCell) &&
                !targetCell.ComparePosition(columnCell) &&
                !targetCell.ComparePosition(startCell))
            {
                HashSet<int> allPos = [.. rowCell.CellPossible, .. columnCell.CellPossible];

                foreach(int h in allPos)
                {
                    if(rowCell.CellPossible.Contains(h) &&
                        columnCell.CellPossible.Contains(h) &&
                        !startCell.CellPossible.Contains(h))
                    {
                        return targetCell.RemovePossibilities(h);
                    }
                }
            }
            return inserted;
        }

        public Cell FindRow(Cell startCell)
        {
            for(int i = 0; i < board.BoardSize; i++)
            {
                if(i == startCell.CellColumn)
                {
                    continue;
                }
                //row value contating Y
                Cell tempCell = board.GetCell(startCell.CellRow, i);

                if(tempCell.CellPossible != null &&
                    tempCell.CellPossible.Count == 2 &&
                    !tempCell.ComparePosition(startCell))
                {

                    if(tempCell.CellPossible.Contains(startCell.CellPossible[0]) ||
                        tempCell.CellPossible.Contains(startCell.CellPossible[1]))
                    {
                        return tempCell;
                    }
                }
            }
            return startCell;
        }

        public Cell FindColumn(Cell startCell, Cell rowCell)
        {
            for(int i = 0; i < board.BoardSize; i++)
            {
                if(i == startCell.CellRow)
                {
                    continue;
                }
                //row value contating Y
                Cell tempCell = board.GetCell(i, startCell.CellColumn);

                if(tempCell.CellPossible != null &&
                    tempCell.CellPossible.Count == 2 &&
                    !tempCell.ComparePosition(startCell) &&
                    !tempCell.ComparePosition(rowCell))
                {
                    if(!tempCell.ComparePossibilitiesPair(startCell))
                    {
                        if(tempCell.CellPossible.Contains(startCell.CellPossible[0]) ||
                        tempCell.CellPossible.Contains(startCell.CellPossible[1]))
                        {
                            return tempCell;
                        }
                    }
                }
            }
            return startCell;
        }

        public bool XYWingBlockRow(int inRow, int inCol)
        {
            // get start cell.(X:Y)
            // Start block
            // get nebouring cell.(X:Z)
            // Get Row value
            // Get local block cell.(Y:Z)
            // Get other Block Cell
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            int targetVal = 0;

            if(startCell.CellPossible.Count != 2)
            {
                return inserted;
            }

            Cell? rowCell = null;
            List<Cell> rows = board.GetUnPopRowCells(inRow);
            foreach(int pos in startCell.CellPossible)
            {
                foreach(Cell cell in rows)
                {
                    if(cell.ComparePosition(startCell))
                    {
                        continue;
                    }
                    if(cell.CellPossible.Count == 2)
                    {
                        if(cell.CellPossible.Contains((int)pos))
                        {
                            rowCell = cell;
                            targetVal = cell.CellPossible.Where(x => x != pos).First();
                            break;
                        }
                    }
                }
            }

            Cell? blockCell = null;
            List<Cell> block = board.GetUnPopBlockCells(inRow,inCol);
            foreach(int pos in startCell.CellPossible)
            {
                foreach(Cell cell in block)
                {
                    if(cell.ComparePosition(startCell) || cell.CellRow == startCell.CellRow)
                    {
                        continue;
                    }
                    if(cell.CellPossible.Count == 2)
                    {
                        if(cell.CellPossible.Contains((int)pos))
                        {
                            blockCell = cell;
                        }
                    }
                }
            }
            if(blockCell == null || rowCell == null)
            {
                return inserted;
            }
            List<Cell> rowREp = board.GetUnPopRowCells(blockCell.CellRow);

            foreach(Cell item in rowREp)
            {
                if(item.ComparePosition(startCell))
                {
                    continue;
                }

                if(item.CellBlock == rowCell.CellBlock)
                {
                    item.RemovePossibilities(targetVal);
                    inserted = true;
                }
            }

            List<Cell> blockRep = board.GetUnPopRowCells(startCell.CellRow);
            foreach(Cell item in blockRep)
            {
                if(item.ComparePosition(startCell))
                {
                    continue;
                }
                if(item.CellBlock == startCell.CellBlock)
                {
                    item.RemovePossibilities(targetVal);
                    inserted = true;
                }
            }
            return inserted;
        }

        public bool XYWingBlockCol(int inRow, int inCol)
        {
            // get start cell.(X:Y)
            // Start block
            // get nebouring cell.(X:Z)
            // Get Row value
            // Get local block cell.(Y:Z)
            // Get other Block Cell
            Cell startCell = board.GetCell(inRow, inCol);
            bool inserted = false;
            int targetVal = 0;

            if(startCell.CellPossible.Count != 2)
            {
                return inserted;
            }

            Cell? colCell = null;
            List<Cell> rows = board.GetUnPopColumnCells(inCol);
            foreach(int pos in startCell.CellPossible)
            {
                foreach(Cell cell in rows)
                {
                    if(cell.ComparePosition(startCell))
                    {
                        continue;
                    }
                    if(cell.CellPossible.Count == 2)
                    {
                        if(cell.CellPossible.Contains((int)pos))
                        {
                            colCell = cell;
                            targetVal = cell.CellPossible.Where(x => x != pos).First();
                            break;
                        }
                    }
                }
            }

            Cell? blockCell = null;
            List<Cell> block = board.GetUnPopBlockCells(inRow,inCol);
            foreach(int pos in startCell.CellPossible)
            {
                foreach(Cell cell in block)
                {
                    if(cell.ComparePosition(startCell) ||
                        cell.CellColumn == startCell.CellColumn ||
                        cell.CellPossible.SequenceEqual(startCell.CellPossible))
                    {
                        continue;
                    }
                    if(cell.CellPossible.Count == 2)
                    {
                        if(cell.CellPossible.Contains((int)pos))
                        {
                            blockCell = cell;
                            break;
                        }
                    }
                }
            }
            if(blockCell == null || colCell == null)
            {
                return inserted;
            }
            List<Cell> colRep = board.GetUnPopColumnCells(blockCell.CellColumn);

            foreach(Cell item in colRep)
            {
                if(item.ComparePosition(startCell))
                {
                    continue;
                }

                if(item.CellBlock == colCell.CellBlock && item.CellPossible.Contains(targetVal))
                {
                    item.RemovePossibilities(targetVal);
                    inserted = true;
                }
            }

            List<Cell> blockRep = board.GetUnPopBlockCells(startCell.CellRow,startCell.CellColumn);
            foreach(Cell item in blockRep)
            {
                if(item.ComparePosition(startCell))
                {
                    continue;
                }
                if(item.CellBlock == startCell.CellBlock && item.CellPossible.Contains(targetVal))
                {
                    item.RemovePossibilities(targetVal);
                    inserted = true;
                }
            }
            return inserted;
        }
        #endregion

        #region X-Wing
        public bool XWing()
        {
            bool inserted = false;
            foreach(Cell t in board.openCells)
            {
                if(XWingRow(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    return inserted;
                }
                if(XWingColumn(t.CellRow, t.CellColumn))
                {
                    inserted = true;
                    break;
                }
            }
            return inserted;
        }

        public bool XWingRow(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);

            List<int> workRow = board.GetRowPoss(inRow);
            bool inserted = false;

            Cell? bottomRowCell = null;
            Cell? topRowCell = null;
            Cell? targetCell = null;

            foreach(int cellPos in startCell.CellPossible)
            {
                List<Cell>? topRow = board.GetUnPopRowCells(startCell.CellRow);
                if(workRow.Where(y => y == cellPos).Count() == 2)
                {
                    // Get top cell in top row
                    foreach(Cell trowCheck in topRow)
                    {
                        if(trowCheck.CellPossible.Contains(cellPos) &&
                            trowCheck.CellColumn != startCell.CellColumn)
                        {
                            topRowCell = trowCheck;
                            break;
                        }
                    }
                    // Get bottom left cell
                    if(topRowCell == null)
                    {
                        continue;
                    }
                    List<Cell> leftColumn = board.GetColumnCells(startCell.CellColumn);
                    foreach(Cell colCell in leftColumn)
                    {
                        if(colCell.CellRow == startCell.CellRow)
                        {
                            continue;
                        }
                        if(colCell.CellPossible != null &&
                            colCell.CellPossible.Contains(cellPos))
                        {
                            List<int> botRow = board.GetRowPoss(colCell.CellRow);

                            if(botRow.Where(y => y == cellPos).Count() != 2)
                            {
                                continue;
                            }
                            targetCell = board.GetCell(colCell.CellRow, topRowCell.CellColumn);
                            if(targetCell.CellPossible != null &&
                                targetCell.CellPossible.Contains(cellPos))
                            {
                                bottomRowCell = targetCell;
                                if(bottomRowCell == null)
                                {
                                    continue;
                                }
                                for(int row = 0; row < board.BoardSize; row++)
                                {
                                    if(row != startCell.CellRow &&
                                        row != bottomRowCell.CellRow)
                                    {
                                        if(board.RemovePoss(row, startCell.CellColumn, cellPos))
                                        {
                                            inserted = true;
                                        }
                                        if(board.RemovePoss(row, topRowCell.CellColumn, cellPos))
                                        {
                                            inserted = true;
                                        }
                                        if(inserted)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return inserted;
        }

        public bool XWingColumn(int inRow, int inCol)
        {
            Cell startCell = board.GetCell(inRow, inCol);
            List<int> workColumn = board.GetColumnPoss(inCol);
            bool inserted = false;
            Cell? rightColumnCell = null;
            Cell? leftColCell = null;
            Cell? targetCell = null;

            foreach(int cellPos in startCell.CellPossible)
            {
                List<Cell>? leftColumn = board.GetUnPopColumnCells(startCell.CellColumn);
                if(workColumn.Where(y => y == cellPos).Count() == 2)
                {
                    // Get left cell in left column
                    foreach(Cell colCheck in leftColumn)
                    {
                        if(colCheck.CellPossible.Contains(cellPos) &&
                            colCheck.CellRow != startCell.CellRow)
                        {
                            leftColCell = colCheck;
                            break;
                        }
                    }
                    // Get right top cell
                    if(leftColCell == null)
                    {
                        continue;
                    }

                    List<Cell> topRow = board.GetRowCells(startCell.CellRow);
                    foreach(Cell rowCell in topRow)
                    {
                        if(rowCell.CellColumn == startCell.CellColumn)
                        {
                            continue;
                        }

                        if(rowCell.CellPossible != null &&
                            rowCell.CellPossible.Contains(cellPos))
                        {
                            List<int> rightColumn =
                                board.GetColumnPoss(rowCell.CellColumn);
                            if(rightColumn.Where(y => y == cellPos).Count() != 2)
                            {
                                continue;
                            }

                            targetCell =
                        board.GetCell(leftColCell.CellRow, rowCell.CellColumn);
                            if(targetCell.CellPossible != null &&
                                targetCell.CellPossible.Contains(cellPos))
                            {
                                rightColumnCell = targetCell;
                                if(rightColumnCell == null)
                                {
                                    continue;
                                }

                                for(int column = 0; column < board.BoardSize; column++)
                                {
                                    if(column != startCell.CellColumn &&
                                        column != rightColumnCell.CellColumn)
                                    {
                                        if(board.RemovePoss(startCell.CellRow, column, cellPos))
                                        {
                                            inserted = true;
                                        }
                                        if(board.RemovePoss(leftColCell.CellRow, column, cellPos))
                                        {
                                            inserted = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return inserted;
        }
        #endregion

        #region Y-Wing
        public void YWing()
        {

        }
        #endregion

        #region SwordFish
        public void SwordFish()
        {

        }
        #endregion
        #endregion
        #region Sloving Logic
        public bool SolveBoard()
        {
            bool Inserted = false;
            bool disp = false;
            do
            {
                Inserted = ConstraintSolve();
                if(Inserted)
                {
                    MethodsUsed.Add(SolveMethod.Constraint);
                }
                if(Inserted && disp)
                {
                    Debug.WriteLine(SolveMethod.Constraint);
                    //Console.WriteLine(SolveMethod.Constraint);
                    //board.ColourBoardDisplay();
                }
                if(!Inserted)
                {
                    Inserted = EliminateSolve();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.Eliminate);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.Eliminate);
                        //Console.WriteLine(SolveMethod.Eliminate);
                        //board.ColourBoardDisplay();
                    }
                }
                if(!Inserted)
                {
                    Inserted = RemainingBlocks();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.RemainingBlocks);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.RemainingBlocks);
                        //Console.WriteLine(SolveMethod.RemainingBlocks);
                        //board.ColourBoardDisplay();
                    }
                }
                if(!Inserted)
                {
                    Inserted = ObviousPair();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.ObviousPair);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.ObviousPair);
                        //Console.WriteLine(SolveMethod.ObviousPair);
                        //board.ColourBoardDisplay();
                    }
                }
                if(!Inserted)
                {
                    Inserted = ObviousTrip();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.ObviousTriple);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.ObviousTriple);
                        //Console.WriteLine(SolveMethod.ObviousTriple);
                        //board.ColourBoardDisplay();
                    }
                }

                if(!Inserted)
                {
                    Inserted = HiddenPair();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.HiddenPair);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.HiddenPair);
                        //Console.WriteLine(SolveMethod.HiddenPair);
                        //board.ColourBoardDisplay();
                    }
                }
                if(!Inserted)
                {
                    Inserted = HiddenTriple();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.HiddenTriple);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.HiddenTriple);
                        //Console.WriteLine(SolveMethod.HiddenTriple);
                        //board.ColourBoardDisplay();
                    }
                }

                if(!Inserted)
                {
                    Inserted = PointingPair();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.PointingPair);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.PointingPair);
                        //Console.WriteLine(SolveMethod.PointingPair);
                        //board.ColourBoardDisplay();
                    }
                }
                if(!Inserted)
                {
                    Inserted = PointingTriple();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.PointingTriple);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.PointingTriple);
                        //Console.WriteLine(SolveMethod.PointingTriple);
                        //board.ColourBoardDisplay();
                    }
                }
                if(!Inserted)
                {
                    Inserted = XWing();
                    if(Inserted)
                    {
                        MethodsUsed.Add(SolveMethod.XWing);
                    }
                    if(Inserted && disp)
                    {
                        Debug.WriteLine(SolveMethod.XWing);
                        //Console.WriteLine(SolveMethod.XWing);
                        //board.ColourBoardDisplay();
                    }
                }
                //if(!Inserted)
                //{
                //    Inserted = XYWing();
                //    if(Inserted)
                //    {
                //        MethodsUsed.Add(SolveMethod.XYWing);
                //    }
                //    if(Inserted && disp)
                //    {
                //        Debug.WriteLine(SolveMethod.XYWing);
                //        //Console.WriteLine(SolveMethod.XYWing);
                //        //board.ColourBoardDisplay();
                //    }
                //}
                //Console.ReadLine();
            }
            while(Inserted);
            return board.VerifyBoard();
        }
        #endregion
    }
}
/*
Console.WriteLine("Point Tripl");
if(Inserted)
{
    board.ColourBoardDisplay();
}
*/