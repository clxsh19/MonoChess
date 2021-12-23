using System;
using Mono_Chess;
public class Move
{

    static long CENTRE = 103481868288L;
    static long EXTENDED_CENTRE = 66229406269440L;
    static long KING_SIDE = -1085102592571150096L;
    static long QUEEN_SIDE = 1085102592571150095L;
    static long KING_B7 = 460039L;
    static long KNIGHT_C6 = 43234889994L;
    static long KING_SPAN = 460039L;
    static long KNIGHT_SPAN = 43234889994L;
    static long ALLY_PIECES;
    static long OCCUPIED;
    static long ENEMY_PIECES;
    static long EMPTY;
    static long[] CASTLE_ROOKS = { 63L, 56L, 7, 0 };
    static long[] RankMasks8 = new long[]
    {
        0xFFL, 0xFF00L, 0xFF0000L, 0xFF000000L, 0xFF00000000L, 0xFF0000000000L, 0xFF000000000000L, unchecked((long)0xFF00000000000000L)
    };
    //from fileA to FileH 
    static long[] FileMasks8 = new long[]
    {
        0x101010101010101L, 0x202020202020202L, 0x404040404040404L, 0x808080808080808L,
        0x1010101010101010L, 0x2020202020202020L, 0x4040404040404040L, unchecked((long)0x8080808080808080L)
    };

    public static void getMove(int r, int c, int selected_sq_index, long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ, bool whiteMove)
    {
        string selected_sq = (c / 8) + "" + r;
        long WHITE = (WP | WN | WB | WR | WQ | WK);
        long BLACK = (BP | BN | BB | BR | BQ | BK);
        OCCUPIED = WP | WN | WB | WR | WQ | WK | BP | BN | BB | BR | BQ | BK;
        EMPTY = ~OCCUPIED;
        

        if (Game1.selected_piece_index[0] != -1 && Game1.selected_piece_index[1] != -1)
        {
            // selected piece index
            int index = Game1.selected_piece_index[0]*8 + Game1.selected_piece_index[1];

            // selected piece and selected sq are both white so change the selected piece
            if ( ((WHITE >> selected_sq_index) & 1) == 1 && ((WHITE >> index) & 1) == 1 )
            { 
                Game1.selected_piece_index[0] = (c / 8);
                Game1.selected_piece_index[1] = r;
            }
            else if (((BLACK >> selected_sq_index) & 1) == 1 && ((BLACK >> index) & 1) == 1)
            {
                Game1.selected_piece_index[0] = (c / 8);
                Game1.selected_piece_index[1] = r;
            }
            // selected piece and selected sq are different
            else
            {
                string move = Game1.selected_piece_index[0] + "" + Game1.selected_piece_index[1] + selected_sq; // move to make
                string all_moves = "";

                if (whiteMove)
                {
                    ALLY_PIECES = WP | WN | WB | WR | WQ; //omitted WK to avoid illegal capture
                    ENEMY_PIECES = ~(WP | WN | WB | WR | WQ | WK | BK); //added BK to avoid illegal capture
                    if (((WP >> index) & 1) == 1) 
                    {
                        move = Helper.ConvertMove_W(move, WP, BP, EP);
                        all_moves = Piece.possibleWP_Moves(WP, BP, EP, ENEMY_PIECES, EMPTY, OCCUPIED);
                        //Console.WriteLine(all_moves + " " + all_moves.Length / 4);
                    }
                    else if (((WN >> index) & 1) == 1) { all_moves = Piece.possibleKnight_Moves(OCCUPIED, WN, ENEMY_PIECES); }
                    else if (((WB >> index) & 1) == 1) { all_moves = Piece.possibleBishop_Moves(OCCUPIED, WB, ENEMY_PIECES); }
                    else if (((WR >> index) & 1) == 1) { all_moves = Piece.possibleRook_Moves(OCCUPIED, WR, ENEMY_PIECES); }
                    else if (((WQ >> index) & 1) == 1) { all_moves = Piece.possibleQueen_Moves(OCCUPIED, WQ, ENEMY_PIECES); }
                    else if (((WK >> index) & 1) == 1) 
                    {
                        all_moves = Piece.possibleKing_Moves(OCCUPIED, WK, ENEMY_PIECES) +
                        possibleCW(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, CWK, CWQ);
                    }
                }
                else
                {
                    ALLY_PIECES = BP | BN | BB | BR | BQ;//omitted BK to avoid illegal capture
                    ENEMY_PIECES = ~(BP | BN | BB | BR | BQ | BK | WK);//added WK to avoid illegal capture
                    if (((BP >> index) & 1) == 1) 
                    {
                        move = Helper.ConvertMove_B(move, BP, WP, EP); 
                        all_moves = Piece.possibleBP_Moves(BP, WP, EP, ENEMY_PIECES, EMPTY, OCCUPIED);
                        
                    }
                    else if (((BN >> index) & 1) == 1) { all_moves = Piece.possibleKnight_Moves(OCCUPIED, BN, ENEMY_PIECES); }
                    else if (((BB >> index) & 1) == 1) { all_moves = Piece.possibleBishop_Moves(OCCUPIED, BB, ENEMY_PIECES); }
                    else if (((BR >> index) & 1) == 1) { all_moves = Piece.possibleRook_Moves(OCCUPIED, BR, ENEMY_PIECES); }
                    else if (((BQ >> index) & 1) == 1) { all_moves = Piece.possibleQueen_Moves(OCCUPIED, BQ, ENEMY_PIECES); }
                    else if (((BK >> index) & 1) == 1)
                    {
                        all_moves = Piece.possibleKing_Moves(OCCUPIED, BK, ENEMY_PIECES) +
                        possibleCB(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, CBK, CBQ);
                    }
                }
                if ( Helper.checkValidMove(move, all_moves) ) 
                {
                    long WPt = Move.makeMove(WP, move, 'P'), WNt = Move.makeMove(WN, move, 'N'),
                         WBt = Move.makeMove(WB, move, 'B'), WRt = Move.makeMove(WR, move, 'R'),
                         WQt = Move.makeMove(WQ, move, 'Q'), WKt = Move.makeMove(WK, move, 'K'),
                         BPt = Move.makeMove(BP, move, 'p'), BNt = Move.makeMove(BN, move, 'n'),
                         BBt = Move.makeMove(BB, move, 'b'), BRt = Move.makeMove(BR, move, 'r'),
                         BQt = Move.makeMove(BQ, move, 'q'), BKt = Move.makeMove(BK, move, 'k'),
                         EPt = Move.makeMoveEP(WP | BP, move);
                    WRt = Move.makeMoveCastle(WRt, WK | BK, move, 'R');
                    BRt = Move.makeMoveCastle(BRt, WK | BK, move, 'r');

                    // not in check
                    if (((WKt & Piece.unsafeForWhite(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt)) == 0 && whiteMove) ||
                       ((BKt & Piece.unsafeForBlack(WPt, WNt, WBt, WRt, WQt, WKt, BPt, BNt, BBt, BRt, BQt, BKt)) == 0 && !whiteMove))
                    {
                        Game1.WP = WPt; Game1.WN = WNt; Game1.WB = WBt; Game1.WR = WRt; Game1.WQ = WQt; Game1.WK = WKt;
                        Game1.BP = BPt; Game1.BN = BNt; Game1.BB = BBt; Game1.BR = BRt; Game1.BQ = BQt; Game1.BK = BKt;
                        Game1.EP = EPt;
                        if (char.IsDigit(move[3]))
                        {//'regular' move
                            int start = (int)((char.GetNumericValue(move[0]) * 8) + (char.GetNumericValue(move[1])));
                            if (((1L << start) & WK) != 0) { Game1.CWK = false; Game1.CWQ = false; }
                            if (((1L << start) & BK) != 0) { Game1.CBK = false; Game1.CBQ = false; }
                            if (((1L << start) & WR & (1L << 63)) != 0) { Game1.CWK = false; }
                            if (((1L << start) & WR & (1L << 56)) != 0) { Game1.CWQ = false; }
                            if (((1L << start) & BR & (1L << 7)) != 0) { Game1.CBK = false; }
                            if (((1L << start) & BR & 1L) != 0) { Game1.CBQ = false; }
                        }

                        Game1.selected_piece_index[0] = -1;
                        Game1.selected_piece_index[1] = -1;
                        Game1.whiteMove = !Game1.whiteMove;
                    }  
                }   
            }        
        }

        else
        {   // selected_sq isn't empty        
            if (((EMPTY >> selected_sq_index) & 1) != 1)
            {
                if ((((WHITE >> selected_sq_index)) & 1) == 1 && whiteMove || (((BLACK >> selected_sq_index)) & 1) == 1 && !whiteMove)
                {
                    //Console.WriteLine("selecting selected_piece_index");
                    Game1.selected_piece_index[0] = (c / 8);
                    Game1.selected_piece_index[1] = r;
                }
            }
        }      
    }

    // 0102 a normal move, 10QP white pawn promo move,  27WE white enpassant move
    public static long makeMove(long board, string move, char type)
    {
        // 3rd digit is num so it's a normal move
        if (char.IsDigit(move[3]))
        {// regular move start and end index
            int start = (int)(char.GetNumericValue(move[0]) * 8) + (int)(char.GetNumericValue(move[1]));
            int end = (int)(char.GetNumericValue(move[2]) * 8) + (int)(char.GetNumericValue(move[3]));
            // if correct board then make move
            if ((((long)(ulong)board >> start) & 1) == 1)
            {
                board &= ~(1L << start);// removing
                board |= (1L << end);// adding
            }
            // diffrent board
            else { board &= ~(1L << end); }
            
        }
        else if (move[3] == 'P')
        {//pawn promotion
            int start, end;
            if (char.IsUpper(move[2]))
            {
                start = Helper.numberOfTrailingZeros(FileMasks8[move[0] - '0'] & RankMasks8[1]);
                end = Helper.numberOfTrailingZeros(FileMasks8[move[1] - '0'] & RankMasks8[0]);
            }
            else
            {
                start = Helper.numberOfTrailingZeros(FileMasks8[move[0] - '0'] & RankMasks8[6]);
                end = Helper.numberOfTrailingZeros(FileMasks8[move[1] - '0'] & RankMasks8[7]);
            }
            if (type == move[2]) { board |= (1L << end); }
            else { board &= ~(1L << start); board &= ~(1L << end); }
        }
        else if (move[3] == 'E')
        {//en passant
            int start, end;
            if (move[2] == 'W')
            {
                start = Helper.numberOfTrailingZeros(FileMasks8[move[0] - '0'] & RankMasks8[3]);
                end = Helper.numberOfTrailingZeros(FileMasks8[move[1] - '0'] & RankMasks8[2]);
                board &= ~(FileMasks8[move[1] - '0'] & RankMasks8[3]);

            }
            else
            {
                start = Helper.numberOfTrailingZeros(FileMasks8[move[0] - '0'] & RankMasks8[4]);
                end = Helper.numberOfTrailingZeros(FileMasks8[move[1] - '0'] & RankMasks8[5]);
                board &= ~(FileMasks8[move[1] - '0'] & RankMasks8[4]);
            }
            if ((((long)(ulong)board >> start) & 1) == 1) 
            {        
                board &= ~(1L << start);
                board |= (1L << end);
            }
        }
        else
        {
            Console.WriteLine("ERROR: Invalid move type");
        }
        return board;
    }

    public static long makeMoveCastle(long rookBoard, long kingBoard, string move, char type)
    {
        int start = (int)((char.GetNumericValue(move[0]) * 8) + (char.GetNumericValue(move[1])));
        if ((( (long)((ulong)kingBoard >> start) & 1) == 1) && (("0402".Equals(move)) || ("0406".Equals(move)) || ("7472".Equals(move)) || ("7476".Equals(move))))
        {//'regular' move
            if (type == 'R')
            {
                switch (move)
                {
                    case "7472":
                        rookBoard &= ~(1L << (int)CASTLE_ROOKS[1]); rookBoard |= (1L << (int)(CASTLE_ROOKS[1] + 3));
                        break;
                    case "7476":
                        rookBoard &= ~(1L << (int)CASTLE_ROOKS[0]); rookBoard |= (1L << (int)(CASTLE_ROOKS[0] - 2));
                        break;
                }
            }
            else
            {
                switch (move)
                {
                    case "0402":
                        rookBoard &= ~(1L << (int)CASTLE_ROOKS[3]); rookBoard |= (1L << (int)(CASTLE_ROOKS[3] + 3));
                        break;
                    case "0406":
                        rookBoard &= ~(1L << (int)CASTLE_ROOKS[2]); rookBoard |= (1L << (int)(CASTLE_ROOKS[2] - 2));
                        break;
                }
            }
        }
        return rookBoard;
    }
    public static long makeMoveEP(long board, string move)
    {
        if (char.IsDigit(move[3]))
        {
            int start = (int)((char.GetNumericValue(move[0]) * 8) + (char.GetNumericValue(move[1])));
            if ((Math.Abs(move[0] - move[2]) == 2) & (((long)((ulong)board >> start) & 1) == 1))
            {//pawn double push
                return FileMasks8[move[1] - '0'];
            }
        }
        return 0;
    }

    public static string possibleWhiteMoves(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ)
    {
        string list = "";

        ALLY_PIECES = WP | WN | WB | WR | WQ;//omitted WK to avoid illegal capture
        ENEMY_PIECES = ~(WP | WN | WB | WR | WQ | WK | BK);//added BK to avoid illegal capture
        OCCUPIED = WP | WN | WB | WR | WQ | WK | BP | BN | BB | BR | BQ | BK;
        EMPTY = ~OCCUPIED;
        list = Piece.possibleWP_Moves(WP, BP, EP, ENEMY_PIECES, EMPTY, OCCUPIED) +
        Piece.possibleKnight_Moves(OCCUPIED, WN, ENEMY_PIECES) +
        Piece.possibleBishop_Moves(OCCUPIED, WB, ENEMY_PIECES) +
        Piece.possibleQueen_Moves(OCCUPIED, WQ, ENEMY_PIECES) +
        Piece.possibleRook_Moves(OCCUPIED, WR, ENEMY_PIECES) + 
        Piece.possibleKing_Moves(OCCUPIED, WK, ENEMY_PIECES) +
        possibleCW(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, CWK, CWQ);

        return list;
    }

    public static string possibleBlackMoves(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, long EP, bool CWK, bool CWQ, bool CBK, bool CBQ)
    {
        string list = "";
        ALLY_PIECES = BP | BN | BB | BR | BQ;//omitted BK to avoid illegal capture
        ENEMY_PIECES = ~(BP | BN | BB | BR | BQ | BK | WK);//added WK to avoid illegal capture
        OCCUPIED = WP | WN | WB | WR | WQ | WK | BP | BN | BB | BR | BQ | BK;
        EMPTY = ~OCCUPIED;
        list = Piece.possibleBP_Moves(BP, WP, EP, ENEMY_PIECES, EMPTY, OCCUPIED) +
        Piece.possibleKnight_Moves(OCCUPIED, BN, ENEMY_PIECES) +
        Piece.possibleBishop_Moves(OCCUPIED, BB, ENEMY_PIECES) +
        Piece.possibleQueen_Moves(OCCUPIED, BQ, ENEMY_PIECES) +
        Piece.possibleRook_Moves(OCCUPIED, BR, ENEMY_PIECES) +
        Piece.possibleKing_Moves(OCCUPIED, BK, ENEMY_PIECES) +
        possibleCB(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, CBK, CBQ);

        return list;
    }

    public static string possibleCW(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, bool CWK, bool CWQ)
    {
        string list = "";
        long UNSAFE = Piece.unsafeForWhite(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK);
        if ((UNSAFE & WK) == 0)
        {
            if (CWK && (((1L <<(int) CASTLE_ROOKS[0]) & WR) != 0))
            {
                if (((OCCUPIED | UNSAFE) & ((1L << 61) | (1L << 62))) == 0)
                {
                    list += "7476";
                }
            }
            if (CWQ && (((1L << (int)CASTLE_ROOKS[1]) & WR) != 0))
            {
                if (((OCCUPIED | (UNSAFE & ~(1L << 57))) & ((1L << 57) | (1L << 58) | (1L << 59))) == 0)
                {
                    list += "7472";
                }
            }
        }
        return list;
    }
    public static string possibleCB(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK, bool CBK, bool CBQ)
    {
        string list = "";
        long UNSAFE = Piece.unsafeForBlack(WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK);
        if ((UNSAFE & BK) == 0)
        {
            if (CBK && (((1L << (int)CASTLE_ROOKS[2]) & BR) != 0))
            {
                if (((OCCUPIED | UNSAFE) & ((1L << 5) | (1L << 6))) == 0)
                {
                    list += "0406";
                }
            }
            if (CBQ && (((1L << (int)CASTLE_ROOKS[3]) & BR) != 0))
            {
                if (((OCCUPIED | (UNSAFE & ~(1L << 1))) & ((1L << 1) | (1L << 2) | (1L << 3))) == 0)
                {
                    list += "0402";
                }
            }
        }
        return list;
    }



}

    
