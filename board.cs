using System;
using Mono_Chess;

public class Board
{
    public static void initiateStandardChess()
    {
        long WP = 0L, WN = 0L, WB = 0L, WR = 0L, WQ = 0L, WK = 0L, BP = 0L, BN = 0L, BB = 0L, BR = 0L, BQ = 0L, BK = 0L;

        //string[,] chessBoard = new string[,] {
        //{ "r","-","-","-","-","r","k","-"},
        //{ "-","-","P","P","b","P","p","p"},
        //{ "P","P","b","-","-","-","n","-"},
        //{ "-","-","-","P","P","P","-","P"},
        //{ "-","n","-","-","-","-","-","-"},
        //{ "P","-","P","-","B","-","q","Q"},
        //{ "-","P","-","P","B","P","P","P"},
        //{ "R","-","-","-","-","R","K","-"}
        //};
        //string[,] chessBoard = new string[,]
        //{
        //    { "-","k","b","-","-","-","-","r"},
        //    { "p","-","-","-","-","-","p","-"},
        //    { "-","p","-","p","n","p","r","-"},
        //    { "-","P","p","-","n","N","q","-"},
        //    { "-","-","P","-","P","R","-","p"},
        //    { "P","-","-","P","-","-","-","P"},
        //    { "K","-","-","Q","-","R","P","-"},
        //    { "-","-","-","N","-","B","-","-"}
        //};
        string[,] chessBoard = new string[,] {
            { "r","n","b","q","k","b","n","r"},
            { "p","p","p","p","p","p","p","p"},
            { "-","-","-","-","-","-","-","-"},
            { "-","-","-","-","-","-","-","-"},
            { "-","-","-","-","-","-","-","-"},
            { "-","-","-","-","-","-","-","-"},
            { "P","P","P","P","P","P","P","P"},
            { "R","N","B","Q","K","B","N","R"}
        };
        //string[,] chessBoard = new string[,] {
        //    { "-","-","k","-","-","b","-","r"},
        //    { "p","-","-","-","p","-","-","p"},
        //    { "-","-","-","-","-","p","-","-"},
        //    { "-","-","-","-","-","-","-","-"},
        //    { "-","-","p","-","-","-","-","-"},
        //    { "-","-","-","b","-","-","-","P"},
        //    { "-","-","Q","-","-","P","P","-"},
        //    { "-","n","-","-","-","-","K","-"}
        //};

        /*
4k3/8/8/8/8/8/8/2B1K3 w - - 0 1
c1b2: 34846
c1d2: 32293
c1a3: 15102
c1e3: 45339
c1f4: 41167
c1g5: 19396
c1h6: 25492
e1d1: 28091
e1f1: 32349
e1d2: 25073
e1e2: 42303
e1f2: 43571
Nodes searched: 385022
         */

        //arrayToBitboards(chessBoard, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK);
        string Binary;
        for (int k = 0; k < 64; k++)
        {
            Binary = "0000000000000000000000000000000000000000000000000000000000000000";
            Binary = Binary.Substring(k + 1) + "1" + Binary.Substring(0, k);

            switch (chessBoard[k / 8, k % 8])
            {
                case "P": WP += ConvertStringToBinary(Binary);
                    break;
                case "N": WN += ConvertStringToBinary(Binary);
                    break;
                case "B": WB += ConvertStringToBinary(Binary);
                    break;
                case "R": WR += ConvertStringToBinary(Binary);
                    break;
                case "Q": WQ += ConvertStringToBinary(Binary);
                    break;
                case "K": WK += ConvertStringToBinary(Binary);
                    break;
                case "p": BP += ConvertStringToBinary(Binary);
                    break;
                case "n": BN += ConvertStringToBinary(Binary);
                    break;
                case "b": BB += ConvertStringToBinary(Binary);
                    break;
                case "r": BR += ConvertStringToBinary(Binary);
                    break;
                case "q": BQ += ConvertStringToBinary(Binary);
                    break;
                case "k":  BK += ConvertStringToBinary(Binary);
                    break;
            }
        }
        Game1.WP = WP; Game1.WK = WK; Game1.WR = WR; Game1.WQ = WQ; Game1.WB = WB; Game1.WN = WN;
        Game1.BP = BP; Game1.BK = BK; Game1.BR = BR; Game1.BQ = BQ; Game1.BB = BB; Game1.BN = BN;

    }

    public static long ConvertStringToBinary(string Binary)
    {
        if (Binary[0] == '0')
        {
            return Convert.ToInt64(Binary, 2);
        }
        else
        {
            return Convert.ToInt64("1" + Binary.Substring(2), 2) * 2;
        }
    }

    public static string[,] bitBoards_to_array(long WP, long WN, long WB, long WR, long WQ, long WK, long BP, long BN, long BB, long BR, long BQ, long BK)
    {
        string[,] board = new string[8, 8];
        // putting every square to '-'
        for (int i = 0; i < 8; i++)
        {
            for (int n = 0; n < 8; n++)
            {
                board[i, n] = "-";
            }
        }
        // setting board from bitboards
        for (int i = 0; i < 64; i++)
        {
            if (((WP >> i) & 1) == 1) { board[i / 8, i % 8] = "P"; }
            if (((WN >> i) & 1) == 1) { board[i / 8, i % 8] = "N"; }
            if (((WB >> i) & 1) == 1) { board[i / 8, i % 8] = "B"; }
            if (((WR >> i) & 1) == 1) { board[i / 8, i % 8] = "R"; }
            if (((WQ >> i) & 1) == 1) { board[i / 8, i % 8] = "Q"; }
            if (((WK >> i) & 1) == 1) { board[i / 8, i % 8] = "K"; }
            if (((BP >> i) & 1) == 1) { board[i / 8, i % 8] = "p"; }
            if (((BN >> i) & 1) == 1) { board[i / 8, i % 8] = "n"; }
            if (((BB >> i) & 1) == 1) { board[i / 8, i % 8] = "b"; }
            if (((BR >> i) & 1) == 1) { board[i / 8, i % 8] = "r"; }
            if (((BQ >> i) & 1) == 1) { board[i / 8, i % 8] = "q"; }
            if (((BK >> i) & 1) == 1) { board[i / 8, i % 8] = "k"; }

        }

        return board;
    }

}

