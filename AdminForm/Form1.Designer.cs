namespace AdminForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            userIdTextBox = new TextBox();
            LoadScreenshots = new Button();
            LoadKeyPresses = new Button();
            label1 = new Label();
            keyPressListBox = new ListBox();
            screenshotPictureBox = new PictureBox();
            PreviousScreenshot = new Button();
            NextScreenshot = new Button();
            Next10Screenshots = new Button();
            Previous100Screenshots = new Button();
            Previous10Screenshots = new Button();
            Next100Screenshots = new Button();
            ((System.ComponentModel.ISupportInitialize)screenshotPictureBox).BeginInit();
            SuspendLayout();
            // 
            // userIdTextBox
            // 
            userIdTextBox.Location = new Point(88, 40);
            userIdTextBox.Name = "userIdTextBox";
            userIdTextBox.Size = new Size(138, 23);
            userIdTextBox.TabIndex = 0;
            // 
            // LoadScreenshots
            // 
            LoadScreenshots.Location = new Point(28, 101);
            LoadScreenshots.Name = "LoadScreenshots";
            LoadScreenshots.Size = new Size(124, 23);
            LoadScreenshots.TabIndex = 1;
            LoadScreenshots.Text = "Load Screenshots";
            LoadScreenshots.UseVisualStyleBackColor = true;
            LoadScreenshots.Click += LoadScreenshots_Click;
            // 
            // LoadKeyPresses
            // 
            LoadKeyPresses.Location = new Point(188, 101);
            LoadKeyPresses.Name = "LoadKeyPresses";
            LoadKeyPresses.Size = new Size(122, 23);
            LoadKeyPresses.TabIndex = 2;
            LoadKeyPresses.Text = "Load Key Presses";
            LoadKeyPresses.UseVisualStyleBackColor = true;
            LoadKeyPresses.Click += LoadKeyPresses_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(120, 7);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 3;
            label1.Text = "Enter User ID";
            // 
            // keyPressListBox
            // 
            keyPressListBox.FormattingEnabled = true;
            keyPressListBox.ItemHeight = 15;
            keyPressListBox.Location = new Point(593, 7);
            keyPressListBox.Name = "keyPressListBox";
            keyPressListBox.Size = new Size(879, 469);
            keyPressListBox.TabIndex = 4;
            // 
            // screenshotPictureBox
            // 
            screenshotPictureBox.Location = new Point(28, 167);
            screenshotPictureBox.Name = "screenshotPictureBox";
            screenshotPictureBox.Size = new Size(537, 419);
            screenshotPictureBox.TabIndex = 5;
            screenshotPictureBox.TabStop = false;
            // 
            // PreviousScreenshot
            // 
            PreviousScreenshot.Location = new Point(291, 592);
            PreviousScreenshot.Name = "PreviousScreenshot";
            PreviousScreenshot.Size = new Size(126, 23);
            PreviousScreenshot.TabIndex = 6;
            PreviousScreenshot.Text = "Previous Screenshot";
            PreviousScreenshot.UseVisualStyleBackColor = true;
            PreviousScreenshot.Click += PreviousScreenshot_Click;
            // 
            // NextScreenshot
            // 
            NextScreenshot.Location = new Point(175, 592);
            NextScreenshot.Name = "NextScreenshot";
            NextScreenshot.Size = new Size(110, 23);
            NextScreenshot.TabIndex = 8;
            NextScreenshot.Text = "Next Screenshot";
            NextScreenshot.UseVisualStyleBackColor = true;
            NextScreenshot.Click += NextScreenshot_Click;
            // 
            // Next10Screenshots
            // 
            Next10Screenshots.Location = new Point(423, 592);
            Next10Screenshots.Name = "Next10Screenshots";
            Next10Screenshots.Size = new Size(75, 23);
            Next10Screenshots.TabIndex = 9;
            Next10Screenshots.Text = "10 ->";
            Next10Screenshots.UseVisualStyleBackColor = true;
            Next10Screenshots.Click += Next10Screenshots_Click;
            // 
            // Previous100Screenshots
            // 
            Previous100Screenshots.Location = new Point(16, 592);
            Previous100Screenshots.Name = "Previous100Screenshots";
            Previous100Screenshots.Size = new Size(75, 23);
            Previous100Screenshots.TabIndex = 10;
            Previous100Screenshots.Text = "<- 100";
            Previous100Screenshots.UseVisualStyleBackColor = true;
            Previous100Screenshots.Click += Previous100Screenshots_Click;
            // 
            // Previous10Screenshots
            // 
            Previous10Screenshots.Location = new Point(97, 592);
            Previous10Screenshots.Name = "Previous10Screenshots";
            Previous10Screenshots.Size = new Size(75, 23);
            Previous10Screenshots.TabIndex = 11;
            Previous10Screenshots.Text = "<- 10";
            Previous10Screenshots.UseVisualStyleBackColor = true;
            Previous10Screenshots.Click += Previous10Screenshots_Click;
            // 
            // Next100Screenshots
            // 
            Next100Screenshots.Location = new Point(504, 592);
            Next100Screenshots.Name = "Next100Screenshots";
            Next100Screenshots.Size = new Size(75, 23);
            Next100Screenshots.TabIndex = 12;
            Next100Screenshots.Text = "100 ->";
            Next100Screenshots.UseVisualStyleBackColor = true;
            Next100Screenshots.Click += Next100Screenshots_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1484, 701);
            Controls.Add(Next100Screenshots);
            Controls.Add(Previous10Screenshots);
            Controls.Add(Previous100Screenshots);
            Controls.Add(Next10Screenshots);
            Controls.Add(NextScreenshot);
            Controls.Add(PreviousScreenshot);
            Controls.Add(screenshotPictureBox);
            Controls.Add(keyPressListBox);
            Controls.Add(label1);
            Controls.Add(LoadKeyPresses);
            Controls.Add(LoadScreenshots);
            Controls.Add(userIdTextBox);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)screenshotPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox userIdTextBox;
        private Button LoadScreenshots;
        private Button LoadKeyPresses;
        private Label label1;
        private ListBox keyPressListBox;
        private PictureBox screenshotPictureBox;
        private Button PreviousScreenshot;
        private Button NextScreenshot;
        private Button Next10Screenshots;
        private Button Previous100Screenshots;
        private Button Previous10Screenshots;
        private Button Next100Screenshots;
    }
}
