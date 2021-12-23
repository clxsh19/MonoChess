using System;

public class Perft
{
    public static string moveToAlgebra(string move)
    {
        string moveString = "";
        moveString += "" + (char)(move[1] + 49);
        moveString += "" + ('8' - move[0]);
        moveString += "" + (char)(move[3] + 49);
        moveString += "" + ('8' - move[2]);
        return moveString;
    }
    public static int perftTotalMoveCounter=0;
    public static int perftMoveCounter = 0;
    static int perftMaxDepth = 1;
    public static void perftRoot(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool WhiteToMove, int depth)
    {
        if (depth < perftMaxDepth)
        {
            string moves;
            if (WhiteToMove)
            {
                moves = Move.possibleWhiteMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ);
            }
            else
            {
                moves = Move.possibleBlackMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ);
            }
            for (int i = 0; i < moves.Length; i += 4)
            {
                string move = moves.Substring(i, 4);
                long WPt = Move.makeMove(WP, move, 'P'), WNt = Move.makeMove(WN, move, 'N'),
                     WBt = Move.makeMove(WB, move, 'B'), WRt = Move.makeMove(WR, move, 'R'),
                     WQt = Move.makeMove(WQ, move, 'Q'), WKt = Move.makeMove(WK, move, 'K'),
                     BPt = Move.makeMove(BP, move, 'p'), BNt = Move.makeMove(BN, move, 'n'),
                     BBt = Move.makeMove(BB, move, 'b'), BRt = Move.makeMove(BR, move, 'r'),
                     BQt = Move.makeMove(BQ, move, 'q'), BKt = Move.makeMove(BK, move, 'k'),
                     EPt = Move.makeMoveEP(WP | BP, move);
                     WRt = Move.makeMoveCastle(WRt, WK | BK, move, 'R');
                     BRt = Move.makeMoveCastle(BRt, WK | BK, move, 'r');
                bool CWKt = CWK, CWQt = CWQ, CBKt = CBK, CBQt = CBQ;
                if (char.IsDigit(move[3]))
                {//'regular' move
                    int start = (int)((char.GetNumericValue(move[0]) * 8) + (char.GetNumericValue(move[1])));
                    if (((1L << start) & WK) != 0) { CWKt = false; CWQt = false; }
                    if (((1L << start) & BK) != 0) { CBKt = false; CBQt = false; }
                    if (((1L << start) & WR & (1L << 63)) != 0) { CWKt = false; }
                    if (((1L << start) & WR & (1L << 56)) != 0) { CWQt = false; }
                    if (((1L << start) & BR & (1L << 7)) != 0) { CBKt = false; }
                    if (((1L << start) & BR & 1L) != 0) { CBQt = false; }
                }
                if (((WKt & Piece.unsafeForWhite(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt)) == 0 && WhiteToMove) ||
                     ((BKt & Piece.unsafeForBlack(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt)) == 0 && !WhiteToMove))
                {
                    perft(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !WhiteToMove, depth + 1);
                    Console.WriteLine(moveToAlgebra(move) + " " + perftMoveCounter);
                    perftTotalMoveCounter += perftMoveCounter;
                    perftMoveCounter = 0;
                }
            }
        }
    }
    public static void perft(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool WhiteToMove, int depth)
    {
        if (depth < perftMaxDepth)
        {
            string moves;
            if (WhiteToMove)
            {
                moves = Move.possibleWhiteMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ);
            }
            else
            {
                moves = Move.possibleBlackMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ);
            }
            
            for (int i = 0; i < moves.Length; i += 4)
            {
                string move = moves.Substring(i, 4);
                long WPt = Move.makeMove(WP, move, 'P'), WNt = Move.makeMove(WN, move, 'N'),
                     WBt = Move.makeMove(WB, move, 'B'), WRt = Move.makeMove(WR, move, 'R'),
                     WQt = Move.makeMove(WQ, move, 'Q'), WKt = Move.makeMove(WK, move, 'K'),
                     BPt = Move.makeMove(BP, move, 'p'), BNt = Move.makeMove(BN, move, 'n'),
                     BBt = Move.makeMove(BB, move, 'b'), BRt = Move.makeMove(BR, move, 'r'),
                     BQt = Move.makeMove(BQ, move, 'q'), BKt = Move.makeMove(BK, move, 'k'),
                     EPt = Move.makeMoveEP(WP | BP, move);
                     WRt = Move.makeMoveCastle(WRt, WK | BK, move, 'R');
                     BRt = Move.makeMoveCastle(BRt, WK | BK, move, 'r');
                bool CWKt = CWK, CWQt = CWQ, CBKt = CBK, CBQt = CBQ;
                if (char.IsDigit(move[3]))
                {//'regular' move
                    int start = (int)((char.GetNumericValue(move[0]) * 8) + (char.GetNumericValue(move[1])));
                    if (((1L << start) & WK) != 0) { CWKt = false; CWQt = false; }
                    if (((1L << start) & BK) != 0) { CBKt = false; CBQt = false; }
                    if (((1L << start) & WR & (1L << 63)) != 0) { CWKt = false; }
                    if (((1L << start) & WR & (1L << 56)) != 0) { CWQt = false; }
                    if (((1L << start) & BR & (1L << 7)) != 0) { CBKt = false; }
                    if (((1L << start) & BR & 1L) != 0) { CBQt = false; }
                }
                if ( ((WKt & Piece.unsafeForWhite(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt)) == 0 && WhiteToMove) ||
                     ((BKt & Piece.unsafeForBlack(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt)) == 0 && !WhiteToMove) )
                {
                    if (depth + 1 == perftMaxDepth) { perftMoveCounter++; }
                    perft(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !WhiteToMove, depth + 1);
                }
            }
        }
    }
}
