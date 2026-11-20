namespace SimGame
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.boardView = new SimGame.BoardView();
            this.lblRules = new System.Windows.Forms.Label();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.lblDifficulty = new System.Windows.Forms.Label();
            this.cmbDifficulty = new System.Windows.Forms.ComboBox();
            this.lblFirst = new System.Windows.Forms.Label();
            this.rbHumanFirst = new System.Windows.Forms.RadioButton();
            this.rbComputerFirst = new System.Windows.Forms.RadioButton();
            this.btnNewGame = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpAi = new System.Windows.Forms.GroupBox();
            this.lblAiInfo = new System.Windows.Forms.Label();
            this.grpHistory = new System.Windows.Forms.GroupBox();
            this.lstHistory = new System.Windows.Forms.ListBox();
            this.lblScore = new System.Windows.Forms.Label();
            this.grpSettings.SuspendLayout();
            this.grpAi.SuspendLayout();
            this.grpHistory.SuspendLayout();
            this.SuspendLayout();
            //
            // boardView
            //
            this.boardView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boardView.BackColor = System.Drawing.Color.White;
            this.boardView.Location = new System.Drawing.Point(12, 12);
            this.boardView.Name = "boardView";
            this.boardView.Size = new System.Drawing.Size(600, 594);
            this.boardView.TabIndex = 0;
            //
            // lblRules
            //
            this.lblRules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRules.Location = new System.Drawing.Point(12, 610);
            this.lblRules.Name = "lblRules";
            this.lblRules.Size = new System.Drawing.Size(600, 60);
            this.lblRules.TabIndex = 1;
            this.lblRules.Text = "Правила: гравці по черзі зафарбовують по одному відрізку своїм кольором " +
    "(ви – червоним, комп'ютер – синім). Хто першим утворить трикутник свого кольору " +
    "з вершинами в точках – програє.";
            //
            // grpSettings
            //
            this.grpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSettings.Controls.Add(this.lblDifficulty);
            this.grpSettings.Controls.Add(this.cmbDifficulty);
            this.grpSettings.Controls.Add(this.lblFirst);
            this.grpSettings.Controls.Add(this.rbHumanFirst);
            this.grpSettings.Controls.Add(this.rbComputerFirst);
            this.grpSettings.Controls.Add(this.btnNewGame);
            this.grpSettings.Location = new System.Drawing.Point(628, 12);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(300, 180);
            this.grpSettings.TabIndex = 2;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Налаштування";
            //
            // lblDifficulty
            //
            this.lblDifficulty.AutoSize = true;
            this.lblDifficulty.Location = new System.Drawing.Point(14, 26);
            this.lblDifficulty.Name = "lblDifficulty";
            this.lblDifficulty.Size = new System.Drawing.Size(121, 19);
            this.lblDifficulty.TabIndex = 0;
            this.lblDifficulty.Text = "Рівень складності:";
            //
            // cmbDifficulty
            //
            this.cmbDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDifficulty.FormattingEnabled = true;
            this.cmbDifficulty.Items.AddRange(new object[] {
            "Легкий",
            "Середній",
            "Складний"});
            this.cmbDifficulty.Location = new System.Drawing.Point(14, 48);
            this.cmbDifficulty.Name = "cmbDifficulty";
            this.cmbDifficulty.Size = new System.Drawing.Size(272, 25);
            this.cmbDifficulty.TabIndex = 1;
            this.cmbDifficulty.SelectedIndexChanged += new System.EventHandler(this.cmbDifficulty_SelectedIndexChanged);
            //
            // lblFirst
            //
            this.lblFirst.AutoSize = true;
            this.lblFirst.Location = new System.Drawing.Point(14, 84);
            this.lblFirst.Name = "lblFirst";
            this.lblFirst.Size = new System.Drawing.Size(105, 19);
            this.lblFirst.TabIndex = 2;
            this.lblFirst.Text = "Першим ходить:";
            //
            // rbHumanFirst
            //
            this.rbHumanFirst.AutoSize = true;
            this.rbHumanFirst.Checked = true;
            this.rbHumanFirst.Location = new System.Drawing.Point(14, 106);
            this.rbHumanFirst.Name = "rbHumanFirst";
            this.rbHumanFirst.Size = new System.Drawing.Size(73, 23);
            this.rbHumanFirst.TabIndex = 3;
            this.rbHumanFirst.TabStop = true;
            this.rbHumanFirst.Text = "Гравець";
            this.rbHumanFirst.UseVisualStyleBackColor = true;
            //
            // rbComputerFirst
            //
            this.rbComputerFirst.AutoSize = true;
            this.rbComputerFirst.Location = new System.Drawing.Point(130, 106);
            this.rbComputerFirst.Name = "rbComputerFirst";
            this.rbComputerFirst.Size = new System.Drawing.Size(89, 23);
            this.rbComputerFirst.TabIndex = 4;
            this.rbComputerFirst.Text = "Комп'ютер";
            this.rbComputerFirst.UseVisualStyleBackColor = true;
            //
            // btnNewGame
            //
            this.btnNewGame.Location = new System.Drawing.Point(14, 138);
            this.btnNewGame.Name = "btnNewGame";
            this.btnNewGame.Size = new System.Drawing.Size(272, 32);
            this.btnNewGame.TabIndex = 5;
            this.btnNewGame.Text = "Нова гра";
            this.btnNewGame.UseVisualStyleBackColor = true;
            this.btnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(248)))));
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(628, 200);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblStatus.Size = new System.Drawing.Size(300, 60);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ваш хід";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpAi
            //
            this.grpAi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAi.Controls.Add(this.lblAiInfo);
            this.grpAi.Location = new System.Drawing.Point(628, 268);
            this.grpAi.Name = "grpAi";
            this.grpAi.Size = new System.Drawing.Size(300, 166);
            this.grpAi.TabIndex = 4;
            this.grpAi.TabStop = false;
            this.grpAi.Text = "Останній хід комп'ютера (альфа-бета)";
            //
            // lblAiInfo
            //
            this.lblAiInfo.Location = new System.Drawing.Point(14, 24);
            this.lblAiInfo.Name = "lblAiInfo";
            this.lblAiInfo.Size = new System.Drawing.Size(274, 134);
            this.lblAiInfo.TabIndex = 0;
            this.lblAiInfo.Text = "—";
            //
            // grpHistory
            //
            this.grpHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpHistory.Controls.Add(this.lstHistory);
            this.grpHistory.Location = new System.Drawing.Point(628, 442);
            this.grpHistory.Name = "grpHistory";
            this.grpHistory.Size = new System.Drawing.Size(300, 186);
            this.grpHistory.TabIndex = 5;
            this.grpHistory.TabStop = false;
            this.grpHistory.Text = "Хід гри";
            //
            // lstHistory
            //
            this.lstHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstHistory.FormattingEnabled = true;
            this.lstHistory.IntegralHeight = false;
            this.lstHistory.ItemHeight = 17;
            this.lstHistory.Location = new System.Drawing.Point(14, 24);
            this.lstHistory.Name = "lstHistory";
            this.lstHistory.Size = new System.Drawing.Size(272, 152);
            this.lstHistory.TabIndex = 0;
            //
            // lblScore
            //
            this.lblScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScore.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblScore.Location = new System.Drawing.Point(628, 636);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(300, 28);
            this.lblScore.TabIndex = 6;
            this.lblScore.Text = "Рахунок: гравець 0 : 0 комп'ютер";
            this.lblScore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(940, 676);
            this.Controls.Add(this.boardView);
            this.Controls.Add(this.lblRules);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.grpAi);
            this.Controls.Add(this.grpHistory);
            this.Controls.Add(this.lblScore);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.MinimumSize = new System.Drawing.Size(860, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Гра Sim – альфа-бета відсікання (ЛР №2, варіант 28)";
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpAi.ResumeLayout(false);
            this.grpHistory.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SimGame.BoardView boardView;
        private System.Windows.Forms.Label lblRules;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Label lblDifficulty;
        private System.Windows.Forms.ComboBox cmbDifficulty;
        private System.Windows.Forms.Label lblFirst;
        private System.Windows.Forms.RadioButton rbHumanFirst;
        private System.Windows.Forms.RadioButton rbComputerFirst;
        private System.Windows.Forms.Button btnNewGame;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox grpAi;
        private System.Windows.Forms.Label lblAiInfo;
        private System.Windows.Forms.GroupBox grpHistory;
        private System.Windows.Forms.ListBox lstHistory;
        private System.Windows.Forms.Label lblScore;
    }
}
