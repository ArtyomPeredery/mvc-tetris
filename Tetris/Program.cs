using Newtonsoft.Json;
using System;
using System.Threading;
using System.Windows.Forms;
using Tetris.Controller;
using Tetris.Model;

namespace Tetris
{
    class Program
    {                               
        [STAThread]
        static void Main(string[] args)
        {            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TetrisController gameController = new TetrisController();
            gameController.StartGame();                               

        }                 
    }
}
