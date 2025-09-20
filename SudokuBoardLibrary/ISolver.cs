namespace SudokuBoardLibrary
{
    public interface ISolver
    {
        bool BackTrackingSolve();
        bool BackTrackingSolve(int inRow, int inCol);

        bool ConstraintSolve();

        bool EliminateSolve();
        List<int>? ElimBlock(int inRow, int inCol);
        List<int>? ElimCol(int currRow, int inCol);
        List<int>? ElimRow(int inRow, int currCol);

        bool HiddenPair();
        bool HiddenPairBlock(int inRow, int inCol);
        bool HiddenPairCol(int inRow, int inCol);
        bool HiddenPairRow(int inRow, int inCol);

        bool HiddenTriple();
        bool HiddenTripleCol(int inRow, int inCol);
        bool HiddenTripleRow(int inRow, int inCol);

        bool ObviousDisTripBlock(int inRow, int inCol);
        bool ObviousDisTripCol(int inRow, int inCol);
        bool ObviousDisTripRow(int inRow, int inCol);

        bool ObviousPair();
        bool ObviousPairBlock(int inRow, int inCol);
        bool ObviousPairCol(int inRow, int inCol);
        bool ObviousPairRow(int inRow, int inCol);

        bool ObviousTrip();
        bool ObviousTripBlock(int inRow, int inCol);
        bool ObviousTripCol(int inRow, int inCol);
        bool ObviousTripRow(int inRow, int inCol);

        bool PointingPair();
        bool PointingPairCol(int inRow, int inCol);
        bool PointingPairRow(int inRow, int inCol);

        bool PointingTriple();
        bool PointingTripleCol(int inRow, int inCol);
        bool PointingTripleRow(int inRow, int inCol);

        bool RemainingBlocks();
        bool RemainingBlockColumn(int inRow, int inCol);
        bool RemainingBlockRow(int inRow, int inCol);

        bool XWing();
        bool XWingColumn(int inRow, int inCol);
        bool XWingRow(int inRow, int inCol);

        bool XYWing();
        bool XYWingBlockCol(int inRow, int inCol);
        bool XYWingBlockRow(int inRow, int inCol);
        bool XYWingRowCol(int inRow, int inCol);

        void SwordFish();

        void YWing();

    }
}