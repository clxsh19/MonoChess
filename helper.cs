using System;

public class Helper
{

    static int[] mvv_lva =
    {
        105, 104, 103, 102, 101, 100,
        205, 204, 203, 202, 201, 200,
        305, 304, 303, 302, 301, 300,
        405, 404, 403, 402, 401, 400,
        505, 504, 503, 502, 501, 500,
        605, 604, 603, 602, 601, 600,
          0,   0,   0,   0,   0,   0,
    };

    public static string[,] killer_moves = new string[2,64];
    public static int numberOfTrailingZeros(long i)
    {
        int x, y;
        if (i == 0) return 64;
        int n = 63;
        y = (int)i; if (y != 0) { n = n - 32; x = y; } else x = (int)(i >> 32);
        y = x << 16; if (y != 0) { n = n - 16; x = y; }
        y = x << 8; if (y != 0) { n = n - 8; x = y; }
        y = x << 4; if (y != 0) { n = n - 4; x = y; }
        y = x << 2; if (y != 0) { n = n - 2; x = y; }
        return n - (int)((uint)(x << 1) >> 31);
    }
    public static long reverse(long i)
    {
        // HD, Figure 7-1
        i = (i & 0x5555555555555555L) << 1 | (long)((ulong)i >> 1) & 0x5555555555555555L;
        i = (i & 0x3333333333333333L) << 2 | (long)((ulong)i >> 2) & 0x3333333333333333L;
        i = (i & 0x0f0f0f0f0f0f0f0fL) << 4 | (long)((ulong)i >> 4) & 0x0f0f0f0f0f0f0f0fL;

        return reverseBytes(i);
    }

    public static long reverseBytes(long i)
    {
        i = (i & 0x00ff00ff00ff00ffL) << 8 | (long)((ulong)i >> 8) & 0x00ff00ff00ff00ffL;
        return (i << 48) | ((i & 0xffff0000L) << 16) | (long)((ulong)(i >> 16) & 0xffff0000L) | (long)((ulong)i >> 48);
    }

    public static bool checkValidMove(string selected_move, string all_moves)
    {
        if (all_moves.Length != 0) 
        {
            for (int i = 0; i < all_moves.Length; i += 4)
            {
                string move = all_moves.Substring(i, 4);
                if (selected_move.Equals(move) == true)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool inCheck(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, bool blackMove)
    {
        long check;
        if (blackMove)
        { 
            check = BK & Piece.unsafeForBlack(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK); 
        }
        else
        {
            check = WK & Piece.unsafeForWhite(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK); 
        }

        if (check == 0) { return false; }
        return true;
    }
    public static void enable_pv_scoring(string moves, int depth, int surface)
    {
        Player.follow_pv = false;
        for (int i = 0; i < moves.Length; i += 4)
        {
            string move = moves.Substring(i, 4);
            if (Player.pv_table[surface-1, depth-1] == move)
            {
                Player.score_pv = true;
                Player.follow_pv = true;
                break;
            }
        }
    }

    public static string getLegalMoves(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool blackMove, int depth, int surface)
    {
        string all_moves = "";
        string legal_moves = "";
        if (blackMove) { all_moves = Move.possibleBlackMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ); }
        else { all_moves = Move.possibleWhiteMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ); }

        for (int i = 0; i < all_moves.Length; i += 4)
        {
            string move = all_moves.Substring(i, 4);
            // updating temporary boards
            long WPt = Move.makeMove(WP, move, 'P'), WNt = Move.makeMove(WN, move, 'N'),
                 WBt = Move.makeMove(WB, move, 'B'), WRt = Move.makeMove(WR, move, 'R'),
                 WQt = Move.makeMove(WQ, move, 'Q'), WKt = Move.makeMove(WK, move, 'K'),
                 BPt = Move.makeMove(BP, move, 'p'), BNt = Move.makeMove(BN, move, 'n'),
                 BBt = Move.makeMove(BB, move, 'b'), BRt = Move.makeMove(BR, move, 'r'),
                 BQt = Move.makeMove(BQ, move, 'q'), BKt = Move.makeMove(BK, move, 'k');
            WRt = Move.makeMoveCastle(WRt, WK | BK, move, 'R');
            BRt = Move.makeMoveCastle(BRt, WK | BK, move, 'r');

            if (Helper.inCheck(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, blackMove) == false)
            {
                legal_moves += move;
            }
        }
        if (Player.follow_pv) { Helper.enable_pv_scoring(legal_moves, depth, surface); }
        legal_moves = sort_moves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, legal_moves, depth, surface);

        return legal_moves;
    }

    public static string sort_moves(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, string all_moves, int depth, int surface)
    {
        int[] move_scores = new int[all_moves.Length/4];
        string move;
        for (int i = 0; i < all_moves.Length; i += 4)
        {
            move = all_moves.Substring(i, 4);
            move_scores[i / 4] = score_move(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, move, depth, surface);
        }

        for (int current_move = 0; current_move < all_moves.Length; current_move += 4)
        {
            for(int next_move = current_move + 4; next_move < all_moves.Length; next_move += 4)
            {
                if (move_scores[current_move/4] < move_scores[next_move / 4])
                {
                    //swap scores
                    int temp_score = move_scores[current_move / 4];
                    move_scores[current_move / 4] = move_scores[next_move / 4];
                    move_scores[next_move / 4] = temp_score;

                    //swap move
                    string current = all_moves.Substring(current_move, 4);
                    string next = all_moves.Substring(next_move, 4);
                    all_moves = all_moves.Substring(0, current_move) + next + all_moves.Substring(current_move+4, next_move-(current_move + 4)) + current + all_moves.Substring(next_move + 4);
                }
            }
        }

        return all_moves;
    }
    
    //-7313---------
    //-7313-2313--------
    //-7313-2313-5573-------
    //-7323-6766-5573-7667------
    //-7323-6766-5573-7667-1221-----
    //-7323-6766-5573-7667-1221-3423----
    //-7323-6766-5573-7667-1221-3423-2516---
    public static int score_move(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, string move, int depth, int surface)
    {
        int row_index = 36, col_index = 0, score = 0;

        if (Player.score_pv)
        {
            if (Player.pv_table[surface-1, depth-1] == move) { Player.score_pv = false; return 200000; }
        }
        // 1st killer move score
        if (killer_moves[0, depth] == move) { return 9000; }
        // score 2nd killer move
        else if (killer_moves[1, depth] == move) { return 8000; }

        int start = (int)(char.GetNumericValue(move[0]) * 8) + (int)(char.GetNumericValue(move[1]));
        int end = (int)(char.GetNumericValue(move[2]) * 8) + (int)(char.GetNumericValue(move[3]));

        if (((WP >> start) & 1) == 1 | ((BP >> start) & 1) == 1) { col_index = 0; }
        else if (((WN >> start) & 1) == 1 | ((BN >> start) & 1) == 1) { col_index = 1; }
        else if (((WB >> start) & 1) == 1 | ((BB >> start) & 1) == 1) { col_index = 2; }
        else if (((WR >> start) & 1) == 1 | ((BR >> start) & 1) == 1) { col_index = 3; }
        else if (((WQ >> start) & 1) == 1 | ((BQ >> start) & 1) == 1) { col_index = 4; }
        else if (((WK >> start) & 1) == 1 | ((BK >> start) & 1) == 1) { col_index = 5; }

        if (((WP >> end) & 1) == 1 | ((BP >> end) & 1) == 1) { row_index = 0; }
        else if (((WN >> end) & 1) == 1 | ((BN >> end) & 1) == 1) { row_index = 6; }
        else if (((WB >> end) & 1) == 1 | ((BB >> end) & 1) == 1) { row_index = 12; }
        else if (((WR >> end) & 1) == 1 | ((BR >> end) & 1) == 1) { row_index = 18; }
        else if (((WQ >> end) & 1) == 1 | ((BQ >> end) & 1) == 1) { row_index = 24; }
        else if (((WK >> end) & 1) == 1 | ((BK >> end) & 1) == 1) { row_index = 30; }
        score = mvv_lva[col_index + row_index];
        if (score != 0) { score += 10000; };

        return score;
    }

    public static int IsCaptureMove(string move, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, bool blackMove)
    {
        long ENEMY;
        if (blackMove) { ENEMY = (WP | WN | WB | WR | WQ | WK); }
        else { ENEMY = (BP | BN | BB | BR | BQ | BK); }

        if (char.IsDigit(move[3]))
        {
            int index = (int)(char.GetNumericValue(move[2]) * 8) + (int)(char.GetNumericValue(move[3]));
            if (((ENEMY >> index) & 1) == 1) { return 1; }
        }
        return 0;
    }

    public static int IsPromotionMove(string move, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, bool blackMove)
    {
        if ( move[3] == 'P'){ return 1; }
        return 0;
    }
    public static string captureMoves(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool blackMove, int depth)
    {
        string all_moves = "";
        string capture_moves = "";
        long ENEMY;
        if (blackMove) 
        { 
            all_moves = Move.possibleBlackMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ);
            ENEMY = (WP | WN | WB | WR | WQ | WK);
        }
        else 
        { 
            all_moves = Move.possibleWhiteMoves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ);
            ENEMY = (BP | BN | BB | BR | BQ | BK);
        }

        for (int i = 0; i < all_moves.Length; i += 4)
        {
            string move = all_moves.Substring(i, 4);
            
            if (char.IsDigit(move[3])) 
            {
                int index = (int)(char.GetNumericValue(move[2]) * 8) + (int)(char.GetNumericValue(move[3]));
                if (((ENEMY >> index) & 1) == 1) 
                {
                    // updating temporary boards
                    long WPt = Move.makeMove(WP, move, 'P'), WNt = Move.makeMove(WN, move, 'N'),
                         WBt = Move.makeMove(WB, move, 'B'), WRt = Move.makeMove(WR, move, 'R'),
                         WQt = Move.makeMove(WQ, move, 'Q'), WKt = Move.makeMove(WK, move, 'K'),
                         BPt = Move.makeMove(BP, move, 'p'), BNt = Move.makeMove(BN, move, 'n'),
                         BBt = Move.makeMove(BB, move, 'b'), BRt = Move.makeMove(BR, move, 'r'),
                         BQt = Move.makeMove(BQ, move, 'q'), BKt = Move.makeMove(BK, move, 'k');
                    WRt = Move.makeMoveCastle(WRt, WK | BK, move, 'R');
                    BRt = Move.makeMoveCastle(BRt, WK | BK, move, 'r');
                    if (Helper.inCheck(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt, blackMove) == false) { capture_moves += move; }
                }
            }

        }
        capture_moves = sort_moves(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, capture_moves, depth, depth);
        return capture_moves;
    }

    public static long isQuiteMove(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, bool blackMove, string move)
    {
        long ENEMY;
        if (blackMove)
        {
            ENEMY = (WP | WN | WB | WR | WQ | WK);
        }
        else
        {
            ENEMY = (BP | BN | BB | BR | BQ | BK);
        }
        if (char.IsDigit(move[3]))
        {
            int index = (int)(char.GetNumericValue(move[2]) * 8) + (int)(char.GetNumericValue(move[3]));
            return (ENEMY >> index) & 1;
        }
        return 0;
    }

    public static string ConvertMove_W(string move, long WP, long BP, long EP)
    {
        long FILE_A = 72340172838076673L, FILE_H = -9187201950435737472L, RANK_5 = 4278190080L, RANK_8 = 255L;
        int Index = (int)char.GetNumericValue(move[2]) * 8 + (int)char.GetNumericValue(move[3]);
        if (((1L << Index) & RANK_8) != 0)
        {
            string promotion_move = move[1] + "" + move[3] + "QP";
            return promotion_move;
        }

        long RIGHT_ENPASSANT = (WP << 1) & BP & RANK_5 & ~FILE_A & EP;
        long LEFT_ENPASSANT = (WP >> 1) & BP & RANK_5 & ~FILE_H & EP;

        if (RIGHT_ENPASSANT != 0 | LEFT_ENPASSANT != 0)
        {
            string enpassant_move = move[1] + "" + move[3] + "WE";
            return enpassant_move;
        }
        return move;
    }

    public static string ConvertMove_B(string move, long BP, long WP, long EP)
    {
        long FILE_A = 72340172838076673L, FILE_H = -9187201950435737472L, RANK_4 = 1095216660480L, RANK_1 = -72057594037927936L;
        int Index = (int)char.GetNumericValue(move[2]) * 8 + (int)char.GetNumericValue(move[3]);
        if (((1L << Index) & RANK_1) != 0)
        {
            string promotion_move = move[1] + "" + move[3] + "qP";
            return promotion_move;
        }

        long RIGHT_ENPASSANT = (BP >> 1) & WP & RANK_4 & ~FILE_H & EP;
        long LEFT_ENPASSANT = (BP << 1) & WP & RANK_4 & ~FILE_A & EP;

        if (RIGHT_ENPASSANT != 0 | LEFT_ENPASSANT != 0)
        {
            string enpassant_move = move[1] + "" + move[3] + "BE";
            return enpassant_move;
        }
        return move;
    }

    public static void drawBitBoard(long Bitboard)
    {
        string[,] board = new string[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++) { board[i, j] = "-"; }
        }
        for (int i = 0; i < 64; i++)
        {
            if (((Bitboard >> i) & 1) == 1) { board[i / 8, i % 8] = "o"; }
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Console.Write(board[i, j]);
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");

    }
}

