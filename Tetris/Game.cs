using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Tetris.Controller;

namespace Tetris.Model
{
    class Game
    {
        // Ссылки на объекты
        public int[] pieces = new int[255];
        public TetrisGrid g1 = new TetrisGrid();
        private PieceProvider provider; 
        private Piece currentPiece, nextPiece;
        // Функциональные переменные
        private bool inPlay = false;
        private int score;
        private int difficulty;
        public List<Ranking> scores = new List<Ranking>();
              
        // Конструктор, инициализация игры
        public Game()
        {
            provider = new PieceProvider(pieces);
            InitializeGame();          

        }

        // Способ проверки и/или набора во время игры
        public bool InPlay
        {
            get
            {
                return inPlay;
            }
            set
            {
                inPlay = value;
            }
        }
      
        

        // Инициализация (или реинициализация) игры        
        public void InitializeGame()
        {
            currentPiece = null;
            nextPiece = null;
            g1.EmptyGrid();            
            g1.EmptyGrid2();
            inPlay = true;                                
            Score = 0;
            Difficulty = 4;
            ExtractPieces();
            

            if (!File.Exists("HighScores.xml"))
                CreateRankingFile();
            LoadScores();
        }

        public void CreateRankingFile()
        {
            XmlSerializer writer = new XmlSerializer(scores.GetType(), "Tetris.Model");
            FileStream file = File.Create("HighScores.xml");

            writer.Serialize(file, scores);
            file.Close();
        }

        private void LoadScores()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(scores.GetType(), "Tetris.Model");
                object obj;
                using (StreamReader reader = new StreamReader("HighScores.xml"))
                {
                    obj = serializer.Deserialize(reader.BaseStream);
                    reader.Close();
                }
                scores = (List<Ranking>)obj;
            }
            catch(InvalidOperationException)
            {
                CreateRankingFile();
                LoadScores();
            }            
        }

        public void SaveScore(string name, int points)
        {
            var score = new Ranking()
            {
                Score = points,
                Name = name
            };

            scores.Add(score);

            try
            {
                XmlSerializer writer = new XmlSerializer(scores.GetType(), "Tetris.Model");

                FileStream file = File.Create("HighScores.xml");

                writer.Serialize(file, scores);
                file.Close();
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.Forms.MessageBox.Show("Unable to write in 'HighScores.xml' file.\nIt's probably read-only");
            }
            catch (IOException)
            {
                System.Windows.Forms.MessageBox.Show("Unable to save 'HighScores.xml' file.\nProbably the disk is full");
            }
            
        }

        // получение созданной фигурки
        public Piece CurrentPiece
        {
            get
            {
                return currentPiece;
            }
        }

        // получение следующей фигурки
        public Piece NextPiece
        {
            get
            {
                return nextPiece;
            }
        }

        // завершение игры
        private void EndGame()
        {
            InPlay = false;
        }

        // создание фигурок
        public void ExtractPieces()
        {
            // если это первый старт, обе части должны быть созданы
            if (currentPiece == null && nextPiece == null)
            {
                    currentPiece = provider.ExtractPiece();
                    nextPiece = provider.ExtractPiece();
                if (currentPiece == nextPiece)
                {
                    nextPiece = provider.ExtractPiece();
                }
            }
            // если это не первый запуск, он назначает текущую фигурку следующей
          
            else
            {
                if  (nextPiece != currentPiece)
                {
                    currentPiece = nextPiece;
                }
                else
                {
                    currentPiece = provider.ExtractPiece();
                }
                currentPiece = nextPiece;
                nextPiece = provider.ExtractPiece();
            }
        }

        // возвращает количество строк текущей фигурки
        private int RowsCurrentPiece
        {
            get
            {
                return currentPiece.Pattern.GetLength(0);
            }
        }

        // возвращает количество столбцов текущей фигурки
        private int ColumnsCurrentPiece
        {
            get
            {
                return currentPiece.Pattern.GetLength(1);
            }
        }

        // можно ли выполнить движение
        private bool CanMove(int x, int y)
        {
            bool canMove = true;

            // может ли кусок существовать в заданных координатах
            for (int i = y, row = 0; i < (y + RowsCurrentPiece); i++, row++)
                for (int j = x, column = 0; j < (x + ColumnsCurrentPiece); j++, column++)
                    if ((g1.Grid1[i, j] != 0) && (currentPiece.Pattern[row, column] != 0))
                        canMove = false;

            return canMove;
        }

        // «закрепить» текущий кусок на сетке
        private void FixPiece()
        {
            for (int i = currentPiece.Y, rowPiece = 0; i < (currentPiece.Y + RowsCurrentPiece); i++, rowPiece++)
                for (int j = currentPiece.X, columnPiece = 0; (j < (currentPiece.X + ColumnsCurrentPiece)); j++, columnPiece++)
                    if (currentPiece.Pattern[rowPiece, columnPiece] != 0)
                        g1.Grid1[i, j] = currentPiece.Pattern[rowPiece, columnPiece];
        }

        // Метод проверки возможности вращения
        private bool CanRotate()
        {
            bool canRotate = true;

            // получение временной матрицы вращения, следующей за текущей
            int[,] rotationTemp = currentPiece.NextRotation(currentPiece.Rotation);
            // обновление х и у вращения
            int xNext = currentPiece.XNextRotation(), 
                yNext = currentPiece.YNextRotation();
            // Проверяет, выходит ли следующее вращение из сетки
            if ((yNext + rotationTemp.GetLength(0)) > g1.Grid1.GetLength(0))
                canRotate = false;
            if ((xNext + rotationTemp.GetLength(1)) > g1.Grid1.GetLength(1))
                canRotate = false;
            if (xNext < 0)
                canRotate = false;

            int i, j;
            if (canRotate)
                for (i = yNext; (i < (yNext + rotationTemp.GetLength(0))); i++)
                    for (j = xNext; (j < (xNext + rotationTemp.GetLength(1))); j++)
                        if (g1.Grid1[i, j] != 0)
                            canRotate = false;

            return canRotate;
        }

        // Метод перемещения фигуры влево, вызываемый контроллером
        public void MoveLeft()
        {
            // проверка что текущий кусок не находится в левом краю
            if (currentPiece.X != 0)
            {
                // Проверяет, может ли текущий фрагмент двигаться, передав метод CanMove ();
                // х, у изменен
                if (CanMove(currentPiece.X - 1, currentPiece.Y))
                {
                    // Движение возможно, Х уменьшилось
                    currentPiece.X--;
                }
            }
        }

        // Метод перемещения фигуры вправо, вызываемый контроллером
        public void MoveRight()
        {
            // проверка что текущий кусок не находится на правом краю
            if ((currentPiece.X + ColumnsCurrentPiece) < g1.Grid1.GetLength(1))
            {
                // Проверяет, может ли текущий фрагмент перемещаться с помощью метода CanMove (), 
                // передавая ему увеличенную координату x, y в неизмененном виде
                 
                if (CanMove(currentPiece.X + 1, currentPiece.Y))
                {
                    // можно передвинуть, Х увеличивается
                    currentPiece.X++;
                }
            }
        }

        // Метод перемещения фигуры вниз, вызываемый контроллером
        public bool MoveDown()
        {
            bool fixedPiece = false;

            // проверка на то, идет ли игра
            if ((g1.Grid1[0, 4] != 0) || (g1.Grid1[0, 3] != 0))
                EndGame();
            else
            {
                // текущий кусок не находится на нижнем торце поля
                if (((currentPiece.Y + RowsCurrentPiece) < g1.Grid1.GetLength(0)) && 
                    (CanMove(currentPiece.X, currentPiece.Y + 1)))
                    currentPiece.Y++;
                else
                {
                    // Кусок больше не может двигаться, его необходимо «закрепить» на базовой сетке и убедиться,
                    // что линия не сформирована полностью, и в этом случае удалить его.
                    FixPiece();
                    fixedPiece = true;
                    CheckCompletedLine();
                }
            }

            return fixedPiece;
        }

        // Метод поворота текущего фрагмента, вызываемый контроллером
        public bool RotatePiece()
        {
            bool val = false;

            if (CanRotate())
            {
                currentPiece.Rotate();
                val = true;
            }

            return val;
        }

        private bool CheckCompletedLine()
        {
            bool completedLine = true;

            for (int i = 0; i < g1.Grid1.GetLength(0); i++)
            {
                completedLine = true;

                // Проверяет, есть ли хотя бы один элемент равный 0, если таковой имеется,
                // строка не завершена, и будет пропущено следующее условие. Перейти к следующей строке (если есть)
                for (int j = 0; (j < g1.Grid1.GetLength(1)) && (completedLine == true); j++)
                    if (g1.Grid1[i, j] == 0)
                        completedLine = false;
                if (completedLine)
                {
                    // возвращение завершенной линии (i) и всех элементов выше
                    // строки копируются на неё. То же самое делается, пока не доберется до линии 0, где
                    // она установлена в 0, потому что все вниз на одну строку
                    for (int k = i; k > 0; k--)
                        for (int j = 0; j < g1.Grid1.GetLength(1); j++)
                            g1.Grid1[k, j] = g1.Grid1[k - 1, j];
                    
                    for (int j = 0; j < g1.Grid1.GetLength(1); j++)
                        g1.Grid1[0, j] = 0;

                    // за каждую строку счет + 10
                    Score += 10;

                    // Каждые 100 баллов(10 линий пройдено) сложность увеличивается
                    if (Score % 100 == 0)
                        Difficulty++;
                }
                
            }

            return completedLine;
        }

        public int Difficulty
        {
            get
            {
                return difficulty;
            }
            private set
            {
                difficulty = value;
            }
        }

        public int Score
        {
            get
            {
                return score;
            }
            private set
            {
                score = value;
            }
        }

        public TetrisGrid GridP1
        {
            get
            {
                return g1;
            }
        }

       
    }
}
