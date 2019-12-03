using System.Drawing;
using System.Windows.Forms;
using Tetris.View;
using Tetris.Model;
using System.IO;
using System.Threading;
using System;

namespace Tetris.Controller
{
    class TetrisController
    {
        // Ссылки на объекты
        public Game game;
        // private WaitingForm waiting;
        private TetrisForm tView;
        private NameForm nameForm;
        private RankingForm rankingForm;
        // Рисование 
        private Graphics g;
        private SolidBrush brush = new SolidBrush(Color.DarkSlateGray);
        private Rectangle sidebar = new Rectangle(250, 0, 125, 500);
        private Rectangle pieceContainer;
        // Изображения 25x25px для просмотра уже исправленных фрагментов
        private Image pieceI_25px = Properties.Resources.immI_25;
        private Image pieceJ_25px = Properties.Resources.immJ_25;
        private Image pieceL_25px = Properties.Resources.immL_25;
        private Image pieceO_25px = Properties.Resources.immO_25;
        private Image pieceS_25px = Properties.Resources.immS_25;
        private Image pieceT_25px = Properties.Resources.immT_25;
        private Image pieceZ_25px = Properties.Resources.immZ_25;
        // Функциональные переменные
        private bool fixedPiece = false;
        private bool onPause = false;
        private int difficulty;
        // Constants
        private const int blockSize = 25;
        UdpListenSend udp;       
        Thread ReceiveThread = null;
        public int ScoreOpponent { get; set; }
        Random rand = new Random();
        bool Start = false;
        public TetrisController()
        {          
            this.tView = new TetrisForm();            
            this.tView.KeyDown += new KeyEventHandler(Tetris_Form_KeyDown);
            this.tView.Paint += new PaintEventHandler(Tetris_Form_Draw);                        
            this.tView.bShowRanking.Click += new EventHandler(Tetris_Form_ShowRanking);
            this.tView.bPauseResume.Click += new EventHandler(Tetris_Form_PauseResume);
            this.tView.Resize += new EventHandler(Tetris_Form_PauseResumeMinimize);
            this.tView.GotFocus += new EventHandler(Tetris_Form_PauseResumeGotLostFocus);
            this.tView.LostFocus += new EventHandler(Tetris_Form_PauseResumeGotLostFocus);   
            this.tView.Playlbl.Click += new EventHandler(PlayLabel);
            this.tView.PlayOfflineLbl.Click += new EventHandler(PlayOffline);
        }

        public void StartGame()
        {
            // Инициализация игрового объекта и начало формы                        

            this.game = new Game();
            udp = new UdpListenSend(game.GridP1, ScoreOpponent, game.pieces);
            for (int i = 0; i < game.pieces.Length; i++)
                game.pieces[i] = rand.Next(0, 7);
            this.tView.t.Tick += new EventHandler(Tetris_Form_Tick);                      
            this.difficulty = this.game.Difficulty;
            this.tView.t.Enabled = false;
           
          
            udp.SendRequest();
            
            ReceiveThread = new Thread( ( ) => { udp.GetIpAddressFromUser();  }   );
            ReceiveThread.Start();

            this.tView.ShowDialog();
            
        }      

        public void CountForStart()
        {
            for (int i = 3; i > 0; i--)
            {
                tView.TimerLbl.Text = i.ToString();
                Thread.Sleep(500);
                tView.Update();
            }
            tView.TimerLbl.Text = "GO";
            Thread.Sleep(500) ;
            tView.Update();
            tView.TimerLbl.Text = "";
            StartTimer();
        }

        public void PlayOffline(object sender, System.EventArgs e)
        {
            ReceiveThread.Abort();
            CountForStart();
            ResumeGame();
        }

        public void StartTimer()
        {
            tView.t.Enabled = true;
        }

        public void PlayLabel(object sender, System.EventArgs e)
        {
            if (udp.Start)
            {
                CountForStart();
                tView.t.Enabled = true;
                ResumeGame();
            }
        }


        private void Tetris_Form_KeyDown(object sender, KeyEventArgs e)
        {
            //while (!Start) { }
            //CountForStart();
            if (!onPause)
            {
                udp.SendGrid(game.GridP1);
                switch (e.KeyData)
                {
                    // Для каждой кнопки выполнить движение и обновить
                    // положение фрагмента, где он находится, и следующий
                    case Keys.Up:
                        game.RotatePiece();
                        tView.Invalidate(new Rectangle(game.CurrentPiece.X * blockSize,
                                                       game.CurrentPiece.Y * blockSize,
                                                       game.CurrentPiece.Sprite.Width,
                                                       game.CurrentPiece.Sprite.Height));
                        tView.Invalidate(pieceContainer);
                        tView.Update();
                        break;
                    case Keys.Down:
                        MoveDown();
                        break;
                    case Keys.Left:
                        game.MoveLeft();
                        tView.Invalidate(pieceContainer);
                        pieceContainer.X -= blockSize;
                        tView.Invalidate(pieceContainer);
                        tView.Update();
                        break;
                    case Keys.Right:
                        game.MoveRight();
                        tView.Invalidate(pieceContainer);
                        pieceContainer.X += blockSize;
                        tView.Invalidate(pieceContainer);
                        tView.Update();
                        break;
                }
            }
        }

        private void Tetris_Form_PauseResumeGotLostFocus(object sender, System.EventArgs e)
        {

            if (tView.Focused == true)
            {              
                //CountForStart();
                //   ResumeGame();
            }
            
            else if (tView.Focused == false)
                PauseGame();
        }

        // временнОе событие: часть перемещена вниз
        private void Tetris_Form_Tick(object sender, System.EventArgs e)
        {
            MoveDown();
            udp.SendGrid(game.GridP1);
            udp.SendScore(game.Score);           
            
        }       

        private void Tetris_Form_PauseResumeMinimize(object sender, System.EventArgs e)
        {
            if (tView.WindowState == FormWindowState.Minimized)
                PauseGame();
            else if (tView.WindowState == FormWindowState.Normal)
                ResumeGame();
        }

        private void Tetris_Form_ShowRanking(object sender, System.EventArgs e)
        {
            PauseGame();
            rankingForm = new RankingForm(game.scores);
            rankingForm.bEmpty.Click += new System.EventHandler(RankingForm_Empty_Click);
            rankingForm.ShowDialog();
            ResumeGame();
        }

        private void RankingForm_Empty_Click(object sender, System.EventArgs e)
        {
            rankingForm.dataGrid.DataSource = null;
            game.scores.Clear();
            game.CreateRankingFile();
            rankingForm.dataGrid.DataSource = game.scores;
        }

        // приостановить игру
        private void PauseGame()
        {
            onPause = true;
            tView.t.Enabled = false;
          

        }

        // возобновить игру с паузы
        private void ResumeGame()
        {
            onPause = false;
            tView.t.Enabled = true;
        }

        // переключения с паузы и возобновления игры
        private void PauseResumeGame()
        {
            if (tView.t.Enabled == true)
                PauseGame();
            else
                ResumeGame();
        }

        // Метод, который обрабатывает событие click формы на Label bPauseResume
        private void Tetris_Form_PauseResume(object sender, System.EventArgs e)
        {
            PauseResumeGame();
        }

        // Метод, который занимается рисованием фигур в форме
        private void Tetris_Form_Draw(object sender, PaintEventArgs e)
        {
            // Управление событием Paint
            e.Graphics.FillRectangle(brush, sidebar);
            DrawBlocks(g, e);           
            DrawPiece(game.CurrentPiece, g, e);
          //   DrawPiece2(game.CurrentPiece, g, e);
            
           // DrawPiece2(game.CurrentPiece, g, e);
            DrawNextPiece(game.NextPiece, g, e);
        }
        private const int secondgridmove = 375;
        // Метод для рисования блоков, уже зафиксированных в сетке
        private void DrawBlocks(Graphics g, PaintEventArgs e)
        {
            for (int y = 0; y < game.GridP1.Grid1.GetLength(0); y++)
                for (int x = 0; x < game.GridP1.Grid1.GetLength(1); x++)
                {
                    switch (game.GridP1.Grid1[y, x])
                    {
                        case 1:
                            e.Graphics.DrawImage(pieceI_25px,
                                                 x * 25,
                                                 y * 25);
                            break;
                        case 2:
                            e.Graphics.DrawImage(pieceO_25px,
                                                 x * 25,
                                                 y * 25);
                            break;
                        case 3:
                            e.Graphics.DrawImage(pieceL_25px,
                                                 x * 25,
                                                 y * 25);
                            break;
                        case 4:
                            e.Graphics.DrawImage(pieceJ_25px,
                                                 x * 25,
                                                 y * 25);
                            break;
                        case 5:
                            e.Graphics.DrawImage(pieceZ_25px,
                                                 x * 25,
                                                 y * 25);
                            break;
                        case 6:
                            e.Graphics.DrawImage(pieceT_25px,
                                                 x * 25,
                                                 y * 25);
                            break;
                        case 7:
                            e.Graphics.DrawImage(pieceS_25px,
                                                 x * 25,
                                                 y * 25);
                            break;
                    }

                    switch (game.GridP1.Grid2[y, x])
                    {
                        case 1:
                            e.Graphics.DrawImage(pieceI_25px,
                                                 x * 25 + secondgridmove,
                                                 y * 25);

                            
                            break;
                        case 2:
                            e.Graphics.DrawImage(pieceO_25px,
                                                 x * 25 + secondgridmove,
                                                 y * 25);
                            break;
                        case 3:
                            e.Graphics.DrawImage(pieceL_25px,
                                                 x * 25 + secondgridmove,
                                                 y * 25);
                            break;
                        case 4:
                            e.Graphics.DrawImage(pieceJ_25px,
                                                 x * 25 + secondgridmove,
                                                 y * 25);
                            break;
                        case 5:
                            e.Graphics.DrawImage(pieceZ_25px,
                                                 x * 25 + secondgridmove,
                                                 y * 25);
                            break;
                        case 6:
                            e.Graphics.DrawImage(pieceT_25px,
                                                 x * 25 + secondgridmove,
                                                 y * 25);
                            break;
                        case 7:
                            e.Graphics.DrawImage(pieceS_25px,
                                                 x * 25 + secondgridmove,
                                                 y * 25);
                            break;
                    }
                    

                }
        }
        public Thread draw = null;
        // обрабатывает рисунок текущего куска
        private void DrawPiece(Piece p, Graphics g, PaintEventArgs e)
        {                         
            pieceContainer = new Rectangle(p.X * blockSize,
                                           p.Y * blockSize,
                                           p.Sprite.Width,
                                           p.Sprite.Height);
            //e.Graphics.DrawImage(p.Sprite,pieceContainer);
            e.Graphics.DrawImage(p.Sprite, p.X * 25, p.Y * 25);
            
          
        }

        private void DrawPiece2(Piece p, Graphics g, PaintEventArgs e)
        {
            switch (p.Name)
            {
                case 'I':
                    e.Graphics.DrawImage(pieceI_25px,
                                         p.X * 25 + secondgridmove,
                                         p.Y * 25);


                    break;
                case 'O':
                    e.Graphics.DrawImage(pieceO_25px,
                                         p.X * 25 + secondgridmove,
                                         p.Y * 25);
                    break;
                case 'L':
                    e.Graphics.DrawImage(pieceL_25px,
                                          p.X * 25 + secondgridmove,
                                         p.Y * 25);
                    break;
                case 'J':
                    e.Graphics.DrawImage(pieceJ_25px,
                                          p.X * 25 + secondgridmove,
                                         p.Y * 25);
                    break;
                case 'Z':
                    e.Graphics.DrawImage(pieceZ_25px,
                                         p.X * 25 + secondgridmove,
                                         p.Y * 25);
                    break;
                case 'T':
                    e.Graphics.DrawImage(pieceT_25px,
                                          p.X * 25 + secondgridmove,
                                         p.Y * 25);
                    break;
                case 'S':
                    e.Graphics.DrawImage(pieceS_25px,
                                         p.X * 25 + secondgridmove,
                                         p.Y * 25);
                    break;
            }

            e.Graphics.DrawImage(p.Sprite, p.X * 25 + 625, p.Y * 25);
            
            // e.Graphics.DrawImage(p.Sprite, p.X * 25+ secondgridmove, p.Y * 25);
        }    








        // обрабатывает рисование следующего куска на боковой панели 
        private void DrawNextPiece(Piece p, Graphics g, PaintEventArgs e)
        {
            e.Graphics.DrawImage(p.Sprite,
                                 p.Name=='I'? 290:280,
                                 50);
        }

        // Метод, который работает с интервальным таймером в форме
        private void DecreaseTimerInterval()
        {
            // Интервал таймера не может быть <200
            // Шаги уменьшения 50
            if (tView.t.Interval >= 200)
                tView.t.Interval -= 50;
        }

        // сдвинуть текущий кусок вниз
        private void MoveDown()
        {
            // действует ли игра еще

           

                if (game.InPlay)
                {
                    // деталь сдвинута ли вниз
                    if (!fixedPiece)
                    {
                        fixedPiece = game.MoveDown();
                        if (fixedPiece)
                        {
                            tView.Invalidate();
                            // Если часть была исправлена, 
                            // извлечь новые части и повторно инициализировать фиксированную переменную
                            game.ExtractPieces();
                            fixedPiece = false;

                            // Обновить метку счета  и проверить
                            // если сложность увеличилась по сравнению с текущей
                            // если увеличено, изменитб интервальный таймер и обновить метку
                            // до текущего уровня
                            tView.Score = game.Score.ToString() + ": Score : " + udp.Score;
                            
                            if (difficulty < game.Difficulty)
                            {
                                DecreaseTimerInterval();
                                difficulty = game.Difficulty;
                                tView.Diff = "Level: " + difficulty.ToString();
                            }
                        }
                    }
                }
                else
                {
                    //  заканчивается игра:
                    // отключаем объект таймера и показываем MessageBox со счетом
                    // Нажав кнопку "ОК", игра начинается снова и переменные будут инициализированы заново
                    this.tView.t.Enabled = false;
                if (game.Score == udp.Score)
                    MessageBox.Show("Ничья довольно редкое событие в этой игре, но высмогли. Набрано очков: " + game.Score);
                else
                if (game.Score > udp.Score)
                    MessageBox.Show("Победа за вами! набрано очков: " + game.Score);
                else
                MessageBox.Show("Увы, вы проиграли:( , счёт: " + game.Score);
                    InsertScore();
                    game.InitializeGame();
                    this.tView.Initialize();
                    this.tView.Invalidate();

                }
            
            tView.Invalidate(pieceContainer);
            pieceContainer.Y += blockSize;
            tView.Invalidate(pieceContainer);
            tView.Update();
          //!!!!!!  tcp.SendGrid(user, game.g1);
        }

        // Метод для отображения NameForm
        private void InsertScore()
        {
            nameForm = new NameForm();
            this.nameForm.ShowDialog();
            game.SaveScore(nameForm.Name, game.Score);
        }
    }
}
