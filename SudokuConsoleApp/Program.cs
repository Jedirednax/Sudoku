using SudokuBoardLibrary;
namespace SudokuConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(210, 55);

            int[,] InBoard = new int[,]
            {
                {2,0,6,1,3,4,0,5,0,},
                {1,5,0,2,0,8,0,0,7,},
                {0,3,4,0,0,7,1,0,0,},
                {0,0,0,7,0,6,4,1,0,},
                {4,0,0,0,9,0,0,0,6,},
                {5,0,2,0,0,0,9,0,0,},
                {9,0,0,6,4,0,7,2,1,},
                {6,2,1,0,7,0,0,3,4,},
                {7,0,5,0,1,0,0,0,8,}
                //
                //{1,2,3,4,5,6,7,8,9},
                //{4,5,6,7,8,9,1,2,3},
                //{7,8,9,1,2,3,4,5,6},
                //{9,1,2,3,4,5,6,7,8},
                //{3,4,5,6,0,8,9,1,2},
                //{6,7,8,9,1,2,3,4,5},
                //{8,9,1,2,3,4,5,6,7},
                //{2,3,4,5,6,7,8,9,1},
                //{5,6,7,8,9,1,2,3,4},
            };
            // InBoard = BoardTypes.LockedCandidatesBlockWithinRow;
            // InBoard = BoardTypes.HiddenPair;
            // InBoard = BoardTypes.XYWingRowCol;
            // InBoard = BoardTypes.HiddenPairInAColumn;
            // InBoard = BoardTypes.XWing;
            // InBoard = BoardTypes.XWingCol;
            // InBoard = BoardTypes.HiddenPairInAColumn;
            // InBoard = BoardTypes.XYWingBlockRow;
            // InBoard = BoardTypes.HiddenTripletInAColumn;
            InBoard = BoardTypes.XYWingBlockCol;

            bool a = true;
            //a = false;
            Generator generator = new Generator();
            //var board = generator.GenerateBoard();
            Board board;
            for(int i = 0; i < 1; i++)
            {
                Console.WriteLine("Started");
                if(a && InBoard != null)
                {
                    board = new Board(InBoard);
                }
                else
                {
                    board = generator.SetBoard(30);
                }

                Solver solver = new Solver(board);
                //board.ColourBoardDisplay();

                //Console.WriteLine(board.ToArray());
                Console.WriteLine(81 - board.openCells.Count);
                //solver.XWingRow(3,8);

                board.ColourBoardDisplay();
                solver.SolveBoard();
                //Console.WriteLine();
                //Console.WriteLine("-----------------------------------------------------------------");
                //Console.WriteLine();
                //            solver.OldSolverLogicSeq();
                board.ColourBoardDisplay();
                //Console.WriteLine(board.ToArray());
                foreach(string method in solver.MethodsUsed)
                {
                    Console.WriteLine(method);
                }
            }
        }
    }
}
/*
}
public static void ColourBoardDisplay(Cell[,] Grid, int[,] Clues, int[,] Solution)
{
    int BoardSize = Grid.GetLength(0);
    int BlockSize = (int)Math.Sqrt(Grid.GetLength(1));
    for(int row = 0; row < BoardSize; row++)
    {
        Console.WriteLine();
        if(row%BlockSize == 0)
        {
            Console.Write($"{RowSep(BlockSize)}\n|");
        }
        else
        {
            Console.Write($"|");
        }
        for(int column = 0; column < BoardSize; column++)
        {
            Console.Write($"{" "}");
            if(Grid[row, column].CellValue == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{" "}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                if(Grid[row, column].CellValue == Clues[row, column])
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write($"{Grid[row, column]}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if(Grid[row, column].CellValue == Solution[row, column])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{Grid[row, column]}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{Grid[row, column]}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            if(((column+1)%BlockSize) == 0)
            {
                Console.Write($"{" "}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"|");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
    Console.WriteLine($"\n{RowSep(BlockSize)}");
}

public static void ColourBoardDisplay(IntergerBoard board)
{
    for(int row = 0; row < board.BoardSize; row++)
    {
        Console.WriteLine();
        if(row%board.BlockSize == 0)
        {
            Console.Write($"{RowSep(board.BlockSize)}\n|");
        }
        else
        {
            Console.Write($"|");
        }
        for(int column = 0; column < board.BoardSize; column++)
        {
            Console.Write($"{" "}");
            if(board.Grid[row, column] == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{" "}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                if(board.Grid[row, column] == board.Clues[row, column])
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write($"{board.Grid[row, column]}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if(board.Grid[row, column] == board.Solution[row, column])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{board.Grid[row, column]}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{board.Grid[row, column]}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            if(((column+1)%board.BlockSize) == 0)
            {
                Console.Write($"{" "}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"|");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
    Console.WriteLine($"\n{RowSep(board.BlockSize)}");
}

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
}
}
/*
* Begginer = {
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* };
* Easy = {
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* {0,0,0,0,0,0,0,0,0},
* };
* Medium = {
* {9,8,0,0,0,4,7,0,3},
* {4,0,0,1,2,0,0,0,0},
* {0,0,2,0,0,8,0,0,1},
* {7,0,0,5,0,0,1,8,0},
* {0,5,3,7,1,0,0,0,2},
* {0,6,0,0,8,2,0,5,0},
* {3,4,0,0,0,0,2,0,9},
* {2,1,0,0,4,0,6,3,0},
* {6,0,8,0,0,0,0,0,0},
* };
* Hard = {
* {0,0,0,0,0,0,9,0,1},
* {0,0,0,0,6,3,0,2,0},
* {0,0,2,9,0,0,5,3,0},
* {7,0,1,0,2,0,0,0,5},
* {4,0,6,0,0,0,0,8,7},
* {0,0,0,0,0,0,0,0,0},
* {0,1,0,2,0,4,0,5,3},
* {0,2,4,0,5,0,0,1,0},
* {0,7,3,0,0,0,0,0,0},
* };
* Extreme = {
* {7,0,0,0,8,0,0,3,0},
* {0,0,0,0,2,0,9,0,0},
* {0,0,0,5,6,0,0,0,8},
* {0,0,3,0,0,0,8,0,0},
* {0,0,0,0,0,0,0,5,9},
* {8,0,0,0,3,9,1,0,7},
* {0,7,0,0,0,8,0,0,0},
* {0,0,4,0,0,1,0,6,0},
* {3,0,1,0,0,6,7,0,0},
* };
*/