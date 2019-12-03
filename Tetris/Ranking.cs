using System;

namespace Tetris.Model
{
    // Сериализуемый класс, используемый для сохранения результатов и имени в файле "HighScores.xml"
    [Serializable()]
    public class Ranking
    {
        public int Score { get; set; }
        public string Name { get; set; }
    }
}
