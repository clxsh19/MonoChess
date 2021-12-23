using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono_Chess
{
    public class Game1 : Game
    {
        // bitboards for each peice to draw board from and etc...
        public static long WP = 0L, WN = 0L, WB = 0L, WR = 0L, WQ = 0L, WK = 0L, BP = 0L, BN = 0L, BB = 0L, BR = 0L, BQ = 0L, BK = 0L, SP  = 0L, EP = 0L;
        //true=castle is possible
        public static bool CWK = true, CWQ = true, CBK = true, CBQ = true, whiteMove = true;
        public static int MAX_DEPTH = 7;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        // textures for images
        Texture2D bK, bQ, bN, bR, bB, bP, wK, wQ, wN, wR, wP, wB;
        Texture2D boardTexture, selectedTexture;
        // board imgae position
        Vector2 boardPos;
        // previos mouse state for click
        MouseState previousMouseState;
        // selected peice coordinateds
        public static int selected_sq_index = -1;
        public static List<int> selected_piece_index = new List<int> { -1, -1};
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 640;
            _graphics.ApplyChanges();

            boardPos = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2);
            previousMouseState = Mouse.GetState();

            Board.initiateStandardChess();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            boardTexture = Content.Load<Texture2D>("board");
            selectedTexture = Content.Load<Texture2D>("selected");
            bK = Content.Load<Texture2D>("bK");
            bR = Content.Load<Texture2D>("bR");
            bQ = Content.Load<Texture2D>("bQ");
            bP = Content.Load<Texture2D>("bP");
            bN = Content.Load<Texture2D>("bN");
            bB = Content.Load<Texture2D>("bB");
            wK = Content.Load<Texture2D>("wK");
            wQ = Content.Load<Texture2D>("wQ");
            wR = Content.Load<Texture2D>("wR");
            wN = Content.Load<Texture2D>("wN");
            wB = Content.Load<Texture2D>("wB");
            wP = Content.Load<Texture2D>("wP");
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();


            if (whiteMove)
            {
                if (previousMouseState.LeftButton == ButtonState.Released
            && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    int r = (state.X / 80);
                    int c = ((state.Y / 80) * 8);
                    selected_sq_index = r + c;
                    if (selected_sq_index != -1)
                    {
                        Move.getMove(r, c, selected_sq_index, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, whiteMove);
                    }
                }
                previousMouseState = Mouse.GetState();
            }

            else
            {
                string bestMove = "";

                var watch1 = System.Diagnostics.Stopwatch.StartNew();
                double alpha = double.MinValue;
                double beta = double.MaxValue;
                for (int depth = 1; depth <= MAX_DEPTH; depth++)
                {
                    Player.follow_pv = true;
                    var result = Player.pvs(depth, alpha, beta, WP, WN, WB, WR, WQ, WK, BP, BN, BB, BR, BQ, BK, EP, CWK, CWQ, CBK, CBQ, true, depth);
                    bestMove = result.Item1;
                    Console.WriteLine(result + ", " + depth);

                    if (result.Item2 <= alpha || result.Item2 >= beta)
                    {
                        alpha = double.MinValue;
                        beta = double.MinValue;
                        continue;
                    }
                    alpha = result.Item2 - 50;
                    beta = result.Item2 + 50;
                }
                watch1.Stop(); Console.WriteLine("Time taken: " + watch1.ElapsedMilliseconds / 1000);

                long WPt = Move.makeMove(WP, bestMove, 'P'), WNt = Move.makeMove(WN, bestMove, 'N'),
                         WBt = Move.makeMove(WB, bestMove, 'B'), WRt = Move.makeMove(WR, bestMove, 'R'),
                         WQt = Move.makeMove(WQ, bestMove, 'Q'), WKt = Move.makeMove(WK, bestMove, 'K'),
                         BPt = Move.makeMove(BP, bestMove, 'p'), BNt = Move.makeMove(BN, bestMove, 'n'),
                         BBt = Move.makeMove(BB, bestMove, 'b'), BRt = Move.makeMove(BR, bestMove, 'r'),
                         BQt = Move.makeMove(BQ, bestMove, 'q'), BKt = Move.makeMove(BK, bestMove, 'k'),
                         EPt = Move.makeMoveEP(WP | BP, bestMove);
                WRt = Move.makeMoveCastle(WRt, WK | BK, bestMove, 'R');
                BRt = Move.makeMoveCastle(BRt, WK | BK, bestMove, 'r');
                Game1.WP = WPt; Game1.WN = WNt; Game1.WB = WBt; Game1.WR = WRt; Game1.WQ = WQt; Game1.WK = WKt;
                Game1.BP = BPt; Game1.BN = BNt; Game1.BB = BBt; Game1.BR = BRt; Game1.BQ = BQt; Game1.BK = BKt;
                Game1.EP = EPt;

                {//'regular' move
                    int start = (int)((char.GetNumericValue(bestMove[0]) * 8) + (char.GetNumericValue(bestMove[1])));
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var images_dict = new Dictionary<string, Texture2D> {
                { "n", bN }, {"k", bK}, {"q", bQ}, {"r", bR}, {"b", bB}, {"p", bP},
                { "N", wN }, {"K", wK}, {"Q", wQ}, {"R", wR}, {"B", wB}, {"P", wP},
            };

            _spriteBatch.Begin();

            _spriteBatch.Draw( boardTexture, boardPos,
                               null, Color.White, 0f,
                               new Vector2(boardTexture.Width / 2, boardTexture.Height / 2),
                               Vector2.One, SpriteEffects.None, 0f );
            if (selected_sq_index != -1)
            {
                _spriteBatch.Draw(selectedTexture, new Vector2((selected_sq_index % 8) * 80, (selected_sq_index / 8) * 80),
                 null, Color.White, 0f,
                 new Vector2(0, 0),
                 Vector2.One, SpriteEffects.None, 0f);
                
            }

                for (int i = 0; i < 64; i++)
            {
                int j = -1;
                string peice_name = "";
                
                if (((WP >> i) & 1) == 1) { j = 5; peice_name="P"; }
                else if (((BP >> i) & 1) == 1) { j = 5; peice_name="p"; }
                else if (((WB >> i) & 1) == 1) { j = 3; peice_name = "B"; }
                else if (((BB >> i) & 1) == 1) { j = 3; peice_name = "b"; }
                else if (((WN >> i) & 1) == 1) { j = 4; peice_name = "N"; }
                else if (((BN >> i) & 1) == 1) { j = 4; peice_name = "n"; }
                else if (((WQ >> i) & 1) == 1) { j = 1; peice_name = "Q"; }
                else if (((BQ >> i) & 1) == 1) { j = 1; peice_name = "q"; }
                else if (((WR >> i) & 1) == 1) { j = 2; peice_name = "R"; }
                else if (((BR >> i) & 1) == 1) { j = 2; peice_name = "r"; }
                else if (((WK >> i) & 1) == 1) { j = 0; peice_name = "K"; }
                else if (((BK >> i) & 1) == 1) { j = 0; peice_name = "k"; }

                if (j != -1)
                {
                    _spriteBatch.Draw((Texture2D)images_dict[peice_name], new Vector2((i % 8)*80 + 8, (i / 8)*80 + 8),
                              null, Color.White, 0f,
                              new Vector2(0, 0),
                              1.1f, SpriteEffects.None, 0f);
                }
            }
           
            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
