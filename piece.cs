using System;

public class Piece
{
    static long FILE_A = 72340172838076673L;
    static long FILE_H = -9187201950435737472L;
    static long FILE_AB = 217020518514230019L;
    static long FILE_GH = -4557430888798830400L;
    static long RANK_1 = -72057594037927936L;
    static long RANK_4 = 1095216660480L;
    static long RANK_5 = 4278190080L;
    static long RANK_8 = 255L;
    static long KING_SPAN = 460039L;
    static long KNIGHT_SPAN = 43234889994L;
    static long[] CASTLE_ROOKS = { 63L, 56L, 7, 0 };
    //from rank1 to rank8
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

    //from top left to bottom right
    static long[] DiagonalMasks8 =
    {
    0x1L, 0x102L, 0x10204L, 0x1020408L, 0x102040810L, 0x10204081020L, 0x1020408102040L,
    0x102040810204080L, 0x204081020408000L, 0x408102040800000L, 0x810204080000000L,
    0x1020408000000000L, 0x2040800000000000L, 0x4080000000000000L, unchecked((long)0x8000000000000000L)
    };

    static long[] AntiDiagonalMasks8 =/*from top right to bottom left*/
    {
    0x80L, 0x8040L, 0x804020L, 0x80402010L, 0x8040201008L, 0x804020100804L, 0x80402010080402L,
    unchecked((long)0x8040201008040201L), 0x4020100804020100L, 0x2010080402010000L, 0x1008040201000000L,
    0x804020100000000L, 0x402010000000000L, 0x201000000000000L, 0x100000000000000L
    };

    public static string possibleWP_Moves(long WP, long BP, long EP, long ENEMY_PIECES, long EMPTY, long OCCUPIED)
    {
        string list = "";

        //capture right
        long PAWN_RIGHT_CAPTURE = (WP >> 7) & ENEMY_PIECES & OCCUPIED & ~RANK_8 & ~FILE_A;
        //capture left
        long PAWN_LEFT_CAPTURE = (WP >> 9) & ENEMY_PIECES & OCCUPIED & ~RANK_8 & ~FILE_H;
        // move 1 forward
        long PAWN_MOVE_FORWARD = (WP >> 8) & EMPTY & ~RANK_8;
        // move 2 forward
        long PAWN_MOVE_DOUBLE_FORWARD = (WP >> 16) & EMPTY & (EMPTY >> 8) & RANK_4;
        // pawn promotion by right capture
        long PAWN_PROMOTION_RIGHT = (WP >> 7) & ENEMY_PIECES & OCCUPIED & RANK_8 & ~FILE_A;
        // pawn promotion by left capture
        long PAWN_PROMOTION_LEFT = (WP >> 9) & ENEMY_PIECES & OCCUPIED & RANK_8 & ~FILE_H;
        // pawn promotion by 1 move forward
        long PAWN_PROMOTION_FORWARD = (WP >> 8) & EMPTY & RANK_8;
        // enpassant right
        long RIGHT_ENPASSANT = (WP << 1) & BP & RANK_5 & ~FILE_A & EP;
        // enpassant left
        long LEFT_ENPASSANT = (WP >> 1) & BP & RANK_5 & ~FILE_H & EP;

        // adding moves to list
        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_RIGHT_CAPTURE >> i) & 1) == 1) { list += "" + (i / 8 + 1) + (i % 8 - 1) + (i / 8) + (i % 8); }
            if (((PAWN_LEFT_CAPTURE >> i) & 1) == 1) { list += "" + (i / 8 + 1) + (i % 8 + 1) + (i / 8) + (i % 8); }
            if (((PAWN_MOVE_FORWARD >> i) & 1) == 1) { list += "" + (i / 8 + 1) + (i % 8) + (i / 8) + (i % 8); }
            if (((PAWN_MOVE_DOUBLE_FORWARD >> i) & 1) == 1) { list += "" + (i / 8 + 2) + (i % 8) + (i / 8) + (i % 8); }
            if (((PAWN_PROMOTION_RIGHT >> i) & 1) == 1) { list += "" + (i % 8 - 1) + (i % 8) + "QP" + (i % 8 - 1) + (i % 8) + "RP" + (i % 8 - 1) + (i % 8) + "BP" + (i % 8 - 1) + (i % 8) + "NP"; ; }
            if (((PAWN_PROMOTION_LEFT >> i) & 1) == 1) { list += "" + (i % 8 + 1) + (i % 8) + "QP" + (i % 8 + 1) + (i % 8) + "RP" + (i % 8 + 1) + (i % 8) + "BP" + (i % 8 + 1) + (i % 8) + "NP"; }
            if (((PAWN_PROMOTION_FORWARD >> i) & 1) == 1) { list += "" + (i % 8) + (i % 8) + "QP" + (i % 8) + (i % 8) + "RP" + (i % 8) + (i % 8) + "BP" + (i % 8) + (i % 8) + "NP"; }
            if (((RIGHT_ENPASSANT >> i) & 1) == 1) { list += "" + (i % 8 - 1) + (i % 8) + "WE"; }
            if (((LEFT_ENPASSANT >> i) & 1) == 1) { list += "" + (i % 8 + 1) + (i % 8) + "WE"; }
        }
        return list;
    }

    public static string possibleBP_Moves(long BP, long WP, long EP, long ENEMY_PIECES, long EMPTY, long OCCUPIED)
    {
        string list = "";

        //capture right
        long PAWN_RIGHT_CAPTURE = (BP << 7) & ENEMY_PIECES & OCCUPIED & ~RANK_1 & ~FILE_H;
        //capture left
        long PAWN_LEFT_CAPTURE = (BP << 9) & ENEMY_PIECES & OCCUPIED & ~RANK_1 & ~FILE_A;
        // move 1 forward
        long PAWN_MOVE_FORWARD = (BP << 8) & EMPTY & ~RANK_1;
        // move 2 forward
        long PAWN_MOVE_DOUBLE_FORWARD = (BP << 16) & EMPTY & (EMPTY << 8) & RANK_5;
        // pawn promotion by right capture
        long PAWN_PROMOTION_RIGHT = (BP << 7) & ENEMY_PIECES & OCCUPIED & RANK_1 & ~FILE_H;
        // pawn promotion by left capture
        long PAWN_PROMOTION_LEFT = (BP << 9) & ENEMY_PIECES & OCCUPIED & RANK_1 & ~FILE_A;
        // pawn promotion by 1 move forward
        long PAWN_PROMOTION_FORWARD = (BP << 8) & EMPTY & RANK_1;
        // enpassant right
        long RIGHT_ENPASSANT = (BP >> 1) & WP & RANK_4 & ~FILE_H & EP;
        // enpassant left
        long LEFT_ENPASSANT = (BP << 1) & WP & RANK_4 & ~FILE_A & EP;

        for (int i = 0; i < 64; i++)
        {
            if (((PAWN_RIGHT_CAPTURE >> i) & 1) == 1){ list += "" + (i / 8 - 1) + (i % 8 + 1) + (i / 8) + (i % 8); }
            if (((PAWN_LEFT_CAPTURE >> i) & 1) == 1) { list += "" + (i / 8 - 1) + (i % 8 - 1) + (i / 8) + (i % 8); }
            if (((PAWN_MOVE_FORWARD >> i) & 1) == 1) { list += "" + (i / 8 - 1) + (i % 8) + (i / 8) + (i % 8); }
            if (((PAWN_MOVE_DOUBLE_FORWARD >> i) & 1) == 1) { list += "" + (i / 8 - 2) + (i % 8) + (i / 8) + (i % 8); }
            if (((PAWN_PROMOTION_RIGHT >> i) & 1) == 1) { list += "" + (i % 8 + 1) + (i % 8) + "qP" + (i % 8 + 1) + (i % 8) + "rP" + (i % 8 + 1) + (i % 8) + "bP" + (i % 8 + 1) + (i % 8) + "nP"; ; }
            if (((PAWN_PROMOTION_LEFT >> i) & 1) == 1) { list += "" + (i % 8 - 1) + (i % 8) + "qP" + (i % 8 - 1) + (i % 8) + "rP" + (i % 8 - 1) + (i % 8) + "bP" + (i % 8 - 1) + (i % 8) + "nP"; }
            if (((PAWN_PROMOTION_FORWARD >> i) & 1) == 1) { list += "" + (i % 8) + (i % 8) + "qP" + (i % 8) + (i % 8) + "rP" + (i % 8) + (i % 8) + "bP" + (i % 8) + (i % 8) + "nP"; }
            if (((RIGHT_ENPASSANT >> i) & 1) == 1) { list += "" + (i % 8 + 1) + (i % 8) + "BE"; }
            if (((LEFT_ENPASSANT >> i) & 1) == 1) { list += "" + (i % 8 - 1) + (i % 8) + "BE"; }
        }
        return list;
    }

    static long HandV_Moves(int s, long OCCUPIED)
    {
        long binaryS = 1L << s;
        long possibilitiesHorizontal = (OCCUPIED - 2 * binaryS) ^ Helper.reverse(Helper.reverse(OCCUPIED) - 2 * Helper.reverse(binaryS));
        long possibilitiesVertical = ((OCCUPIED & FileMasks8[s % 8]) - (2 * binaryS)) ^ Helper.reverse(Helper.reverse(OCCUPIED & FileMasks8[s % 8]) - (2 * Helper.reverse(binaryS)));
        return (possibilitiesHorizontal & RankMasks8[s / 8]) | (possibilitiesVertical & FileMasks8[s % 8]);
    }

    static long DAndAntiD_Moves(int s, long OCCUPIED)
    {
        long binaryS = 1L << s;
        long DIAGONAL_MOVES = ((OCCUPIED & DiagonalMasks8[(s / 8) + (s % 8)]) - (2 * binaryS)) ^ Helper.reverse(Helper.reverse(OCCUPIED & DiagonalMasks8[(s / 8) + (s % 8)]) - (2 * Helper.reverse(binaryS)));
        long ANTI_DIAGONAL = ((OCCUPIED & AntiDiagonalMasks8[(s / 8) + 7 - (s % 8)]) - (2 * binaryS)) ^ Helper.reverse(Helper.reverse(OCCUPIED & AntiDiagonalMasks8[(s / 8) + 7 - (s % 8)]) - (2 * Helper.reverse(binaryS)));
        return (DIAGONAL_MOVES & DiagonalMasks8[(s / 8) + (s % 8)]) | (ANTI_DIAGONAL & AntiDiagonalMasks8[(s / 8) + 7 - (s % 8)]);
    }

    public static string possibleBishop_Moves(long OCCUPIED, long B, long ENEMY_PIECES)
    {
        String list = "";
        long i = B & ~(B - 1);
        long BishopMoves;
        // going thorugh each bishop
        while (i != 0)
        {
            int iLocation = Helper.numberOfTrailingZeros(i);
            BishopMoves = DAndAntiD_Moves(iLocation, OCCUPIED) & ENEMY_PIECES;
            
            long j = BishopMoves &~(BishopMoves - 1);
            // going through each move for the bishop
            while (j != 0)
            {
                int index = Helper.numberOfTrailingZeros(j);
                list += "" + (iLocation / 8) + (iLocation % 8) + (index / 8) + (index % 8);
                BishopMoves &= ~j;
                j = BishopMoves & ~(BishopMoves - 1);
            }
            B &= ~i;
            i = B & ~(B - 1);
        }
        return list;
    }

    public static string possibleRook_Moves(long OCCUPIED, long R, long ENEMY_PIECES)
    {
        string list = "";
        long i = R & ~(R - 1);
        long RookMoves;
        // going thorugh each rook
        while (i != 0)
        {
            int iLocation = Helper.numberOfTrailingZeros(i);
            RookMoves = HandV_Moves(iLocation, OCCUPIED) & ENEMY_PIECES;
            long j = RookMoves & ~(RookMoves - 1);
            // going through each move for the rook
            while (j != 0)
            {
                int index = Helper.numberOfTrailingZeros(j);
                list += "" + (iLocation / 8) + (iLocation % 8) + (index / 8) + (index % 8);
                RookMoves &= ~j;
                j = RookMoves & ~(RookMoves - 1);
            }
            R &= ~i;
            i = R & ~(R - 1);
        }
        return list;
    }

    public static string possibleQueen_Moves(long OCCUPIED, long Q, long ENEMY_PIECES)
    {
        string list = "";
        long i = Q & ~(Q - 1);
        long QUEEN_MOVES;
        while (i != 0)
        {
            // index for piece
            int iLocation = Helper.numberOfTrailingZeros(i);
            QUEEN_MOVES = (HandV_Moves(iLocation, OCCUPIED) | DAndAntiD_Moves(iLocation, OCCUPIED)) & ENEMY_PIECES;
            // going through queen moves
            long j = QUEEN_MOVES & ~(QUEEN_MOVES - 1);
            while (j != 0)
            {
                int index = Helper.numberOfTrailingZeros(j);
                list += "" + (iLocation / 8) + (iLocation % 8) + (index / 8) + (index % 8);
                // removing this move and going to next one
                QUEEN_MOVES &= ~j;
                j = QUEEN_MOVES & ~(QUEEN_MOVES - 1);
            }
            Q &= ~i;
            i = Q & ~(Q - 1);
        }
        return list;
    }

    public static string possibleKnight_Moves(long OCCUPIED, long N, long ENEMY_PIECES)
    {
        string list = "";
        long i = N & ~(N - 1);
        long knightMoves;
        while (i != 0)
        {
            int iLocation = Helper.numberOfTrailingZeros(i);
            if (iLocation > 18)
            {
                knightMoves = KNIGHT_SPAN << (iLocation - 18);
            }
            else
            {
                knightMoves = KNIGHT_SPAN >> (18 - iLocation);
            }
            if (iLocation % 8 < 4)
            {
                knightMoves &= ~FILE_GH & ENEMY_PIECES;
            }
            else
            {
                knightMoves &= ~FILE_AB & ENEMY_PIECES;
            }
            long j = knightMoves & ~(knightMoves - 1);

            while (j != 0)
            {
                int index = Helper.numberOfTrailingZeros(j);
                list += "" + (iLocation / 8) + (iLocation % 8) + (index / 8) + (index % 8);
                knightMoves &= ~j;
                j = knightMoves & ~(knightMoves - 1);
            }
            N &= ~i;
            i = N & ~(N - 1);
        }
        return list;
    }

    public static string possibleKing_Moves(long OCCUPIED, long K, long ENEMY_PIECES)
    {
        string list = "";
        long KingMoves;
        int iLocation = Helper.numberOfTrailingZeros(K);
        if (iLocation > 9)
        {
            KingMoves = KING_SPAN << (iLocation - 9);
        }
        else
        {
            KingMoves = KING_SPAN >> (9 - iLocation);
        }
        if (iLocation % 8 < 4)
        {
            KingMoves &= ~FILE_GH & ENEMY_PIECES;
        }
        else
        {
            KingMoves &= ~FILE_AB & ENEMY_PIECES;
        }

        long j = KingMoves & ~(KingMoves - 1);
        while (j != 0)
        {
            int index = Helper.numberOfTrailingZeros(j);
            list += "" + (iLocation / 8) + (iLocation % 8) + (index / 8) + (index % 8);
            KingMoves &= ~j;
            j = KingMoves & ~(KingMoves - 1);
        }
        return list;
    }

    public static long unsafeForWhite(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK)
    {
        long unsafeWhite;
        long OCCUPIED = WP | WN | WB | WR | WQ | WK | BP | BN | BB | BR | BQ | BK;
        //pawn
        unsafeWhite = ((BP << 7) & ~FILE_H);//pawn capture right
        unsafeWhite |= ((BP << 9) & ~FILE_A);//pawn capture left
        long possibility;
        //knight
        long i = BN & ~(BN - 1);
        while (i != 0)
        {
            int iLocationN = Helper.numberOfTrailingZeros(i);
            if (iLocationN > 18)
            {
                possibility = KNIGHT_SPAN << (iLocationN - 18);
            }
            else
            {
                possibility = KNIGHT_SPAN >> (18 - iLocationN);
            }
            if (iLocationN % 8 < 4)
            {
                possibility &= ~FILE_GH;
            }
            else
            {
                possibility &= ~FILE_AB;
            }
            unsafeWhite |= possibility;
            BN &= ~i;
            i = BN & ~(BN - 1);
        }
        //bishop/queen
        long QB = BQ | BB;
        i = QB & ~(QB - 1);
        while (i != 0)
        {
            int iLocationD = Helper.numberOfTrailingZeros(i);
            possibility = DAndAntiD_Moves(iLocationD, OCCUPIED);
            unsafeWhite |= possibility;
            QB &= ~i;
            i = QB & ~(QB - 1);
        }
        //rook/queen
        long QR = BQ | BR;
        i = QR & ~(QR - 1);
        while (i != 0)
        {
            int iLocationS = Helper.numberOfTrailingZeros(i);
            possibility = HandV_Moves(iLocationS, OCCUPIED);
            unsafeWhite |= possibility;
            QR &= ~i;
            i = QR & ~(QR - 1);
        }
        //king
        int iLocation = Helper.numberOfTrailingZeros(BK);
        if (iLocation > 9)
        {
            possibility = KING_SPAN << (iLocation - 9);
        }
        else
        {
            possibility = KING_SPAN >> (9 - iLocation);
        }
        if (iLocation % 8 < 4)
        {
            possibility &= ~FILE_GH;
        }
        else
        {
            possibility &= ~FILE_AB;
        }
        unsafeWhite |= possibility;
        return unsafeWhite;
    }

    public static long unsafeForBlack(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK)
    {
        long unsafeBlack;
        long OCCUPIED = WP | WN | WB | WR | WQ | WK | BP | BN | BB | BR | BQ | BK;
        //pawn
        unsafeBlack = ((long)((ulong)WP >> 7) & ~FILE_A);//pawn capture right
        unsafeBlack |= ((long)((ulong)WP >> 9) & ~FILE_H);//pawn capture left
        long possibility;
        //knight
        long i = WN & ~(WN - 1);
        while (i != 0)
        {
            int iLocationN = Helper.numberOfTrailingZeros(i);
            if (iLocationN > 18)
            {
                possibility = KNIGHT_SPAN << (iLocationN - 18);
            }
            else
            {
                possibility = KNIGHT_SPAN >> (18 - iLocationN);
            }
            if (iLocationN % 8 < 4)
            {
                possibility &= ~FILE_GH;
            }
            else
            {
                possibility &= ~FILE_AB;
            }
            unsafeBlack |= possibility;
            WN &= ~i;
            i = WN & ~(WN - 1);
        }
        //bishop/queen
        long QB = WQ | WB;
        i = QB & ~(QB - 1);
        while (i != 0)
        {
            int iLocationD = Helper.numberOfTrailingZeros(i);
            possibility = DAndAntiD_Moves(iLocationD, OCCUPIED);
            unsafeBlack |= possibility;
            QB &= ~i;
            i = QB & ~(QB - 1);
        }
        //rook/queen
        long QR = WQ | WR;
        i = QR & ~(QR - 1);
        while (i != 0)
        {
            int iLocationS = Helper.numberOfTrailingZeros(i);
            possibility = HandV_Moves(iLocationS, OCCUPIED);
            unsafeBlack |= possibility;
            QR &= ~i;
            i = QR & ~(QR - 1);
        }
        //king
        int iLocationK = Helper.numberOfTrailingZeros(WK);
        if (iLocationK > 9)
        {
            possibility = KING_SPAN << (iLocationK - 9);
        }
        else
        {
            possibility = KING_SPAN >> (9 - iLocationK);
        }
        if (iLocationK % 8 < 4)
        {
            possibility &= ~FILE_GH;
        }
        else
        {
            possibility &= ~FILE_AB;
        }
        unsafeBlack |= possibility;
        return unsafeBlack;
    }

}
