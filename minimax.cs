using System;
using Mono_Chess;
public class Player
{

    public static int[] pv_length = new int[64];
    public static string[] pv = new string[64];
    public static string[,] pv_table = new string[64,64];
    public static bool follow_pv, score_pv;
    static int full_depth_moves = 4;
    static int reduction_limit = 3;

    static double[] pawn_score = new double[]
    {
         9,   9,   9,   9,   9,   9,   9,   9,
         3,   3,   3,   4,   4,   3,   3,   3,
         2,   2,   2,   3,   3,   3,   2,   2,
         1,   1,   1,   2,   2,   1,   1,   1,
        .5,  .5,   1,   2,   2,  .5,  .5,  .5,
         0,   0,   0,  .5,  .5,   0,   0,   0,
         0,   0,   0,  -1,  -1,   0,   0,   0,
         0,   0,   0,   0,   0,   0,   0,   0
    };

    // knight positional score
    static double[] knight_score =
    {
         -.5,   0,   0,   0,   0,   0,   0, -.5,
         -.5,   0,   0,   1,   1,   0,   0, -.5,
         -.5,  .5,   2,   2,   2,   2,  .5, -.5,
         -.5,   1,   2,   3,   3,   2,   1, -.5,
         -.5,   1,   2,   3,   3,   2,   1, -.5,
         -.5,  .5,   2,   1,   1,   2,  .5, -.5,
         -.5,   0,   0,   0,   0,   0,   0, -.5,
         -.5,  -1,   0,   0,   0,   0,  -1, -.5 
    };

    // bishop positional score
    static double[] bishop_score =
    {
         0,   0,   0,   0,   0,   0,   0,   0,
         0,   0,   0,   0,   0,   0,   0,   0,
         0,   0,   0,   1,   1,   0,   0,   0,
         0,   0,   1,   2,   2,   1,   0,   0,
         0,   0,   1,   2,   2,   1,   0,   0,
         0,   1,   0,   0,   0,   0,   1,   0,
         0,   3,   0,   0,   0,   0,   3,   0,
         0,   0,  -1,   0,   0,  -1,   0,   0 
    };

    // rook positional score
    static double [] rook_score =
    {
          5,   5,   5,   5,   5,   5,   5,   5,
          5,   5,   5,   5,   5,   5,   5,   5,
          0,   0,   1,   2,   2,   1,   0,   0,
          0,   0,   1,   2,   2,   1,   0,   0,
          0,   0,   1,   2,   2,   1,   0,   0,
          0,   0,   1,   2,   2,   1,   0,   0,
          0,   0,   1,   2,   2,   1,   0,   0,
          0,   0,   0,   2,   2,   0,   0,   0
    };

    // king positional score
    static double[] king_score =
    {
         0,   0,   0,   0,   0,   0,   0,   0,
         0,   0,  .5,  .5,  .5,  .5,   0,   0,
         0,  .5,  .5,   1,   1,  .5,  .5,   0,
         0,  .5,   1,   2,   2,   1,  .5,   0,
         0,  .5,   1,   2,   2,   1,  .5,   0,
         0,   0,   5,   1,   1,   5,   0,   0,
         0,  .5,  .5, -.5, -.5,   0,  .5,   0,
         0,   0,  .5,   0,-1.5,   0,   1,   0
    };

    static int[] mirror_index = new int[]
    {
        56,  57,  58,  59,  60,  61,  62,  63,
        48,  50,  51,  52,  53,  54,  56,  55,
        40,  41,  42,  43,  44,  45,  46,  47,
        32,  33,  34,  35,  36,  37,  38,  39,
        24,  25,  26,  27,  28,  29,  30,  31,
        16,  17,  18,  19,  20,  21,  22,  23,
         8,   9,  10,  11,  12,  13,  14,  15,
         0,   1,   2,   3,   4,   5,   6,   7
    };

    public static double evalPosition(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, bool blackMove)
    {
        double WhiteScore = 0;
        double BlackScore = 0;
        for (int i = 0; i < 64; i++)
        {
            if (((WP >> i) & 1) == 1) { WhiteScore += 10 + pawn_score[i]; }
            else if (((WN >> i) & 1) == 1) { WhiteScore += 30 + knight_score[i]; }
            else if (((WB >> i) & 1) == 1) { WhiteScore += 35 + bishop_score[i]; }
            else if (((WR >> i) & 1) == 1) { WhiteScore += 50 + rook_score[i]; }
            else if (((WQ >> i) & 1) == 1) { WhiteScore += 90; }
            else if (((WK >> i) & 1) == 1) { WhiteScore += 900 + king_score[i]; }
            else if (((BP >> i) & 1) == 1) { BlackScore += 10 + pawn_score[mirror_index[i]]; }
            else if (((BN >> i) & 1) == 1) { BlackScore += 30 + knight_score[mirror_index[i]]; }
            else if (((BB >> i) & 1) == 1) { BlackScore += 35 + bishop_score[mirror_index[i]]; }
            else if (((BR >> i) & 1) == 1) { BlackScore += 50 + rook_score[mirror_index[i]]; }
            else if (((BQ >> i) & 1) == 1) { BlackScore += 90; }
            else if (((BK >> i) & 1) == 1) { BlackScore += 900 + king_score[mirror_index[i]]; }
        }

        if (blackMove)
        {
            return BlackScore - WhiteScore;
        }
        else
        {
            return WhiteScore - BlackScore;
        }
    }

    public static double quiescent(double alpha, double beta, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool blackMove, int depth)
    {
        double eval = evalPosition(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove);
 
        if (eval >= beta)
        {
            return beta;
        }
        if (eval > alpha)
        {
            alpha = eval;
        }
        // all possible moves for the player
        string all_moves = Helper.captureMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, blackMove, depth);
        for (int i = 0; i < all_moves.Length; i += 4)
        {
            string move = all_moves.Substring(i, 4);
            // updating temporary boards
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
            eval = -quiescent(-beta, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, depth+1);
            if (eval >= beta)
            {
                return beta;
            }
            if (eval > alpha)
            {
                alpha = eval;
            }
        }
        return alpha;
    }

    //public static (string bestMove, double eval) negamax(int depth, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool blackMove, double mate)
    //{
    //    double bestEval = double.MinValue;
    //    string bestMove = "";
    //    if (depth == 0)
    //    {
    //        double e = evalPosition(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove);
    //        return ("", e);
    //    }

    //    // all possible moves for the player
    //    string all_moves = Helper.getLegalMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, blackMove);
    //    if (all_moves.Length == 0)
    //    {
    //        if (Helper.inCheck(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove) == true) { return ("", -mate); }
    //        else { return ("", 0); }
    //    }

    //    for (int i = 0; i < all_moves.Length; i += 4) 
    //    { 
    //        string move = all_moves.Substring(i, 4);
    //        // updating temporary boards
    //        long WPt = Move.makeMove(WP, move, 'P'), WNt = Move.makeMove(WN, move, 'N'),
    //             WBt = Move.makeMove(WB, move, 'B'), WRt = Move.makeMove(WR, move, 'R'),
    //             WQt = Move.makeMove(WQ, move, 'Q'), WKt = Move.makeMove(WK, move, 'K'),
    //             BPt = Move.makeMove(BP, move, 'p'), BNt = Move.makeMove(BN, move, 'n'),
    //             BBt = Move.makeMove(BB, move, 'b'), BRt = Move.makeMove(BR, move, 'r'),
    //             BQt = Move.makeMove(BQ, move, 'q'), BKt = Move.makeMove(BK, move, 'k'),
    //             EPt = Move.makeMoveEP(WP | BP, move);
    //        WRt = Move.makeMoveCastle(WRt, WK | BK, move, 'R');
    //        BRt = Move.makeMoveCastle(BRt, WK | BK, move, 'r');
    //        bool CWKt = CWK, CWQt = CWQ, CBKt = CBK, CBQt = CBQ;
    //        if (char.IsDigit(move[3]))
    //        {
    //            int start = (int)((char.GetNumericValue(move[0]) * 8) + (char.GetNumericValue(move[1])));
    //            if (((1L << start) & WK) != 0) { CWKt = false; CWQt = false; }
    //            if (((1L << start) & BK) != 0) { CBKt = false; CBQt = false; }
    //            if (((1L << start) & WR & (1L << 63)) != 0) { CWKt = false; }
    //            if (((1L << start) & WR & (1L << 56)) != 0) { CWQt = false; }
    //            if (((1L << start) & BR & (1L << 7)) != 0) { CBKt = false; }
    //            if (((1L << start) & BR & 1L) != 0) { CBQt = false; }
    //        }

    //        var result = negamax(depth - 1, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, mate-1);
    //        double result_eval = -result.eval;
    //        if (result_eval > bestEval)
    //        {
    //            bestEval = result_eval;
    //            bestMove = move;
    //        } 
    //    }
    //    return (bestMove, bestEval);
    //}
    //public static (string bestMove, double eval) alpha_beta(int depth, double alpha, double beta, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool blackMove, double mate)
    //{
    //    pv_length[depth] = depth;
    //    string bestMove = "";
    //    if (depth == 0)
    //    {
    //        double e = evalPosition(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove);
    //        return ("", e);
    //    }

    //    // all possible moves for the player
    //    string all_moves = Helper.getLegalMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, blackMove, depth);
    //    if (all_moves.Length == 0)
    //    {
    //        if (Helper.inCheck(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove) == true) { return ("", -mate); }
    //        else { return ("", 0); }
    //    }

    //    for (int i = 0; i < all_moves.Length; i += 4)
    //    {
    //        string move = all_moves.Substring(i, 4);
    //        // updating temporary boards
    //        long WPt = Move.makeMove(WP, move, 'P'), WNt = Move.makeMove(WN, move, 'N'),
    //             WBt = Move.makeMove(WB, move, 'B'), WRt = Move.makeMove(WR, move, 'R'),
    //             WQt = Move.makeMove(WQ, move, 'Q'), WKt = Move.makeMove(WK, move, 'K'),
    //             BPt = Move.makeMove(BP, move, 'p'), BNt = Move.makeMove(BN, move, 'n'),
    //             BBt = Move.makeMove(BB, move, 'b'), BRt = Move.makeMove(BR, move, 'r'),
    //             BQt = Move.makeMove(BQ, move, 'q'), BKt = Move.makeMove(BK, move, 'k'),
    //             EPt = Move.makeMoveEP(WP | BP, move);
    //        WRt = Move.makeMoveCastle(WRt, WK | BK, move, 'R');
    //        BRt = Move.makeMoveCastle(BRt, WK | BK, move, 'r');
    //        bool CWKt = CWK, CWQt = CWQ, CBKt = CBK, CBQt = CBQ;
    //        if (char.IsDigit(move[3]))
    //        {
    //            int start = (int)((char.GetNumericValue(move[0]) * 8) + (char.GetNumericValue(move[1])));
    //            if (((1L << start) & WK) != 0) { CWKt = false; CWQt = false; }
    //            if (((1L << start) & BK) != 0) { CBKt = false; CBQt = false; }
    //            if (((1L << start) & WR & (1L << 63)) != 0) { CWKt = false; }
    //            if (((1L << start) & WR & (1L << 56)) != 0) { CWQt = false; }
    //            if (((1L << start) & BR & (1L << 7)) != 0) { CBKt = false; }
    //            if (((1L << start) & BR & 1L) != 0) { CBQt = false; }
    //        }

    //        var result = alpha_beta(depth - 1, -beta, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, mate - 1);
    //        double result_eval = -result.eval;
    //        if (result_eval >= beta)
    //        {
    //            return (move, beta);
    //        }
    //        if (result_eval > alpha)
    //        {
    //            alpha = result_eval;
    //            bestMove = move;
    //        }
    //    }
    //    return (bestMove, alpha);
    //}

    public static (string, double) pvs(int depth, double alpha, double beta, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool blackMove, int surface)
    {
        // pv length
        pv_length[depth] = depth;

        // end the recursion
        if (depth == 0)
        {
            return (" ",quiescent(alpha, beta, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, blackMove, 20));
        }
        //if (depth > 63) { return (" ", evalPosition(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove)); }

        string bestMove = "";

        //null move pruning
        if (depth >= 3 &&
            Helper.inCheck(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove) == false 
            && depth != surface
            )
        {
            //search moves with reduced depth to find beta cutoffs ,R = 2 is a reduction limit
            var result = pvs(depth - 1 - 2, beta - 1, beta, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, !blackMove, surface);
            double result_eval = -result.Item2;
            if (result_eval >= beta)
            {
                return (bestMove, beta);
            }
        }
        // all possible moves for the player
        string all_moves = Helper.getLegalMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, blackMove, depth, surface);       

        if (all_moves.Length == 0)
        {
            if (Helper.inCheck(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove) == true) { return ("", -99999-depth); }
            else { return ("", 0); } // stalemate
        }
        int move_searched = 0;

        for (int i = 0; i < all_moves.Length; i += 4)
        {
            string move = all_moves.Substring(i, 4);
            // updating temporary boards
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

            double result_eval;
            // do a full depth search on first move
            if (move_searched == 0)
            {
                var result = pvs(depth - 1, -beta, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);
                result_eval = -result.Item2;
            }
            else // late move reduction
            {
                if ( move_searched >= full_depth_moves &&
                     depth >= reduction_limit &&
                     //Helper.inCheck(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, !blackMove) == false &&
                     Helper.IsCaptureMove(move, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove) == 0 &&
                     move[3] != 'P'
                    )
                {
                    var result = pvs(depth-2, -alpha - 1, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);
                    result_eval = -result.Item2;
                }
                else { result_eval = alpha + 1; }

                if (result_eval > alpha)
                {
                    var result = pvs(depth - 1, -alpha - 1, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);
                    result_eval = -result.Item2;
                    if (result_eval > alpha && result_eval < beta) 
                    {
                        result = pvs(depth - 1, -beta, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);
                        result_eval = -result.Item2;
                    }
                }
            }

            move_searched++;
            if (result_eval >= beta)
            {
                if (Helper.isQuiteMove(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove, move) == 0)
                {
                    Helper.killer_moves[1, depth] = Helper.killer_moves[0, depth];
                    Helper.killer_moves[0, depth] = move;
                }
                return (move, beta);
            }
            if (result_eval > alpha)
            {
                alpha = result_eval;
                bestMove = move;
                pv_table[depth, depth] = move;
                pv[depth] = move;

                for (int next_ply = 1; next_ply < pv_length[depth+1]  ; next_ply++)
                    pv_table[depth+1, next_ply] = pv_table[depth, next_ply];
            }
        }
        return (bestMove, alpha);
    }
    /*
         public static (string, double) pvs(int depth, double alpha, double beta, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool blackMove, int surface)
    {
        // pv length
        pv_length[depth] = depth;

        // end the recursion
        if (depth == 0)
        {
            return (" ",quiescent(alpha, beta, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, blackMove, 20));
        }

        bool foundPV = false;
        string bestMove = "";
        // all possible moves for the player
        string all_moves = Helper.getLegalMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, blackMove, depth, surface);       

        if (all_moves.Length == 0)
        {
            if (Helper.inCheck(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove) == true) { return ("", -99999-depth); }
            else { return ("", 0); }
        }
        int move_searched = 0;

        for (int i = 0; i < all_moves.Length; i += 4)
        {
            string move = all_moves.Substring(i, 4);
            // updating temporary boards
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

            double result_eval;
            // do a full depth search on first move
            if (move_searched == 0)
            {
                var result = pvs(depth - 1, -alpha - 1, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);
                result_eval = -result.Item2;
            }
            if (foundPV)
            {
                var result = pvs(depth - 1, -alpha - 1, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);
                result_eval = -result.Item2;
                if (result_eval>alpha && result_eval < beta) 
                {
                    result = pvs(depth - 1, -beta, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);
                    result_eval = -result.Item2;
                }
            }
            else
            {
                var result = pvs(depth - 1, -beta, -alpha, WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, EPt, CWKt, CWQt, CBKt, CBQt, !blackMove, surface);  
                result_eval = -result.Item2;
            }

            move_searched++;
            if (result_eval >= beta)
            {
                if (Helper.isQuiteMove(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, blackMove, move) == 0)
                {
                    Helper.killer_moves[1, depth] = Helper.killer_moves[0, depth];
                    Helper.killer_moves[0, depth] = move;
                }
                return (move, beta);
            }
            if (result_eval > alpha)
            {
                alpha = result_eval;
                bestMove = move;
                foundPV = true;
                pv_table[depth, depth] = move;
                pv[depth] = move;

                for (int next_ply = 1; next_ply < pv_length[depth+1]  ; next_ply++)
                    pv_table[depth+1, next_ply] = pv_table[depth, next_ply];
                //pv_length[depth] = pv_length[depth + 1];
            }
        }
        return (bestMove, alpha);
    }
     */
}
