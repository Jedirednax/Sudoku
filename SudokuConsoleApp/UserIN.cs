//using SudokuBoardLibrary;

//namespace Sudoku
//{
//    internal class UserIN
//    {
//        public static void DisplayPos()
//        {
//            (int Left, int Top) hold = Console.GetCursorPosition();
//            Console.SetCursorPosition(0, 0);
//            Console.WriteLine($"{hold,-10}");
//            Console.SetCursorPosition(hold.Left, hold.Top);
//        }
//        public static void UserIn()
//        {
//            IntergerBoard board = new IntergerBoard();
//            //board.ColourBoardDisplay();
//            //board.GridDisplay();
//            //board.CursorFill();
//            ConsoleKeyInfo inputKey;
//            //Console.WriteLine(Console.GetCursorPosition());
//            Console.SetCursorPosition(2, 2);

//            (int Left, int Top) OffSet = Console.GetCursorPosition();

//            int LRMove = 2;
//            int UDMove = 1;

//            int LRCount = 0;
//            int UDCount = 0;
//            do
//            {
//                DisplayPos();
//                (int Left, int Top) pos = Console.GetCursorPosition();
//                inputKey = Console.ReadKey();
//                if(inputKey.Key == ConsoleKey.LeftArrow)
//                {
//                    if(pos.Left>=3)
//                    {
//                        Console.SetCursorPosition(pos.Left - LRMove, pos.Top);
//                        LRCount++;
//                    }
//                }
//                else if(inputKey.Key == ConsoleKey.RightArrow)
//                {
//                    if(pos.Left<37)
//                    {
//                        Console.SetCursorPosition(pos.Left + LRMove, pos.Top);
//                        LRCount--;
//                    }
//                }
//                else if(inputKey.Key == ConsoleKey.UpArrow)
//                {
//                    if(pos.Top>2)
//                    {
//                        Console.SetCursorPosition(pos.Left, pos.Top -UDMove);
//                        UDCount--;
//                    }
//                }
//                else if(inputKey.Key == ConsoleKey.DownArrow)
//                {
//                    if(pos.Top<27)
//                    {
//                        Console.SetCursorPosition(pos.Left, pos.Top +UDMove);
//                        UDCount++;
//                    }
//                }
//                else
//                {
//                    (int Left, int Top) hold = Console.GetCursorPosition();
//                    (int Left, int Top) posIn = pos;
//                    posIn.Top = (pos.Top-1)/2;
//                    posIn.Left = (pos.Left-1)/3;
//                    if(inputKey.Key == ConsoleKey.D1 || inputKey.Key == ConsoleKey.NumPad1)
//                    {

//                        board.Set(posIn.Top, posIn.Left, 1);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D2 || inputKey.Key == ConsoleKey.NumPad2)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 2);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D3 || inputKey.Key == ConsoleKey.NumPad3)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 3);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D4 || inputKey.Key == ConsoleKey.NumPad4)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 4);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D5 || inputKey.Key == ConsoleKey.NumPad5)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 5);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D6 || inputKey.Key == ConsoleKey.NumPad6)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 6);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D7 || inputKey.Key == ConsoleKey.NumPad7)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 7);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D8 || inputKey.Key == ConsoleKey.NumPad8)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 8);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D9 || inputKey.Key == ConsoleKey.NumPad9)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 9);
//                    }
//                    else if(inputKey.Key == ConsoleKey.D0 || inputKey.Key == ConsoleKey.NumPad0)
//                    {
//                        board.Set(posIn.Top, posIn.Left, 0);
//                    }
//                    else
//                    {

//                    }
//                    Console.Clear();

//                    Console.SetCursorPosition(hold.Left-1, hold.Top);
//                }
//            } while(inputKey.Key != ConsoleKey.Escape); /**/
//        }
//    }
//}
