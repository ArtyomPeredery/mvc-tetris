﻿using System;

namespace Tetris.Model
{
    class PieceProvider
    {
        private Random rnd = new Random();
        private int tempPiece = 1;
        int[] Pieces = new int[255];
        public PieceProvider(int[] pieces)
        {
            Pieces = pieces;
        }

        public Piece ExtractPiece()
        {
            Piece currentPiece = null;

            switch (Pieces[tempPiece])
            {
                case 0:
                    currentPiece = new PieceT();
                    break;
                case 1:
                    currentPiece = new PieceO();
                    break;
                case 2:
                    currentPiece = new PieceI();
                    break;
                case 3:
                    currentPiece = new PieceS();
                    break;
                case 4:
                    currentPiece = new PieceJ();
                    break;
                case 5:
                    currentPiece = new PieceL();
                    break;
                case 6:
                    currentPiece = new PieceZ();
                    break;
            }
            tempPiece++;
            return currentPiece;
        }

    }
}
