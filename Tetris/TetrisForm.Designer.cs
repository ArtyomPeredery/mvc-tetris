namespace Tetris.View
{
    partial class TetrisForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TetrisForm));
            this.score = new System.Windows.Forms.Label();
            this.diff = new System.Windows.Forms.Label();
            this.buttons = new System.Windows.Forms.Label();
            this.bShowRanking = new System.Windows.Forms.Label();
            this.bPauseResume = new System.Windows.Forms.Label();
            this.Playlbl = new System.Windows.Forms.Label();
            this.TimerLbl = new System.Windows.Forms.Label();
            this.PlayOfflineLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // score
            // 
            this.score.AutoSize = true;
            this.score.BackColor = System.Drawing.Color.Transparent;
            this.score.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.score.ForeColor = System.Drawing.Color.White;
            this.score.Location = new System.Drawing.Point(265, 335);
            this.score.Name = "score";
            this.score.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.score.Size = new System.Drawing.Size(84, 15);
            this.score.TabIndex = 0;
            this.score.Text = "0 :Score: 0";
            this.score.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // diff
            // 
            this.diff.AutoSize = true;
            this.diff.BackColor = System.Drawing.Color.Transparent;
            this.diff.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.diff.ForeColor = System.Drawing.Color.White;
            this.diff.Location = new System.Drawing.Point(265, 320);
            this.diff.Name = "diff";
            this.diff.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.diff.Size = new System.Drawing.Size(63, 15);
            this.diff.TabIndex = 3;
            this.diff.Text = "Level: 1";
            this.diff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttons
            // 
            this.buttons.AutoSize = true;
            this.buttons.BackColor = System.Drawing.Color.Transparent;
            this.buttons.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttons.ForeColor = System.Drawing.Color.White;
            this.buttons.Location = new System.Drawing.Point(265, 262);
            this.buttons.Name = "buttons";
            this.buttons.Size = new System.Drawing.Size(77, 28);
            this.buttons.TabIndex = 4;
            this.buttons.Text = "Buttons:\r\n↑, →, ↓, ←";
            this.buttons.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttons.Click += new System.EventHandler(this.buttons_Click);
            // 
            // bShowRanking
            // 
            this.bShowRanking.AutoSize = true;
            this.bShowRanking.ForeColor = System.Drawing.Color.White;
            this.bShowRanking.Location = new System.Drawing.Point(257, 390);
            this.bShowRanking.Name = "bShowRanking";
            this.bShowRanking.Size = new System.Drawing.Size(91, 13);
            this.bShowRanking.TabIndex = 7;
            this.bShowRanking.Text = "Show high scores";
            // 
            // bPauseResume
            // 
            this.bPauseResume.AutoSize = true;
            this.bPauseResume.ForeColor = System.Drawing.Color.White;
            this.bPauseResume.Location = new System.Drawing.Point(257, 413);
            this.bPauseResume.Name = "bPauseResume";
            this.bPauseResume.Size = new System.Drawing.Size(85, 13);
            this.bPauseResume.TabIndex = 8;
            this.bPauseResume.Text = "Pause - Resume";
            this.bPauseResume.Click += new System.EventHandler(this.bPauseResume_Click);
            // 
            // Playlbl
            // 
            this.Playlbl.AutoSize = true;
            this.Playlbl.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.Playlbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Playlbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Playlbl.Location = new System.Drawing.Point(280, 195);
            this.Playlbl.Name = "Playlbl";
            this.Playlbl.Size = new System.Drawing.Size(36, 18);
            this.Playlbl.TabIndex = 9;
            this.Playlbl.Text = "Play";
            // 
            // TimerLbl
            // 
            this.TimerLbl.AutoSize = true;
            this.TimerLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TimerLbl.ForeColor = System.Drawing.Color.Red;
            this.TimerLbl.Location = new System.Drawing.Point(270, 122);
            this.TimerLbl.Name = "TimerLbl";
            this.TimerLbl.Size = new System.Drawing.Size(0, 73);
            this.TimerLbl.TabIndex = 10;
            // 
            // PlayOfflineLbl
            // 
            this.PlayOfflineLbl.AutoSize = true;
            this.PlayOfflineLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlayOfflineLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.PlayOfflineLbl.Location = new System.Drawing.Point(267, 233);
            this.PlayOfflineLbl.Name = "PlayOfflineLbl";
            this.PlayOfflineLbl.Size = new System.Drawing.Size(75, 16);
            this.PlayOfflineLbl.TabIndex = 11;
            this.PlayOfflineLbl.Text = "Play Offline";
            // 
            // TetrisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(626, 501);
            this.Controls.Add(this.PlayOfflineLbl);
            this.Controls.Add(this.TimerLbl);
            this.Controls.Add(this.Playlbl);
            this.Controls.Add(this.bPauseResume);
            this.Controls.Add(this.bShowRanking);
            this.Controls.Add(this.buttons);
            this.Controls.Add(this.diff);
            this.Controls.Add(this.score);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TetrisForm";
            this.Text = "Tetris";
            this.Load += new System.EventHandler(this.TetrisForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label score;
        private System.Windows.Forms.Label diff;
        private System.Windows.Forms.Label buttons;
        public System.Windows.Forms.Label bShowRanking;
        public System.Windows.Forms.Label bPauseResume;
        public System.Windows.Forms.Label Playlbl;
        public System.Windows.Forms.Label TimerLbl;
        public System.Windows.Forms.Label PlayOfflineLbl;
    }
}