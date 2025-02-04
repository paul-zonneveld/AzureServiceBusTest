namespace MessagePortal
{
    partial class Portal
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
            label4 = new Label();
            label3 = new Label();
            ProcessedMessagesPanel = new FlowLayoutPanel();
            txtMessageCount = new TextBox();
            label2 = new Label();
            startManual = new Button();
            InProcessPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 66);
            label4.Name = "label4";
            label4.Size = new Size(163, 20);
            label4.TabIndex = 14;
            label4.Text = "Current message count:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 453);
            label3.Name = "label3";
            label3.Size = new Size(143, 20);
            label3.TabIndex = 13;
            label3.Text = "Processed messages";
            // 
            // ProcessedMessagesPanel
            // 
            ProcessedMessagesPanel.BorderStyle = BorderStyle.FixedSingle;
            ProcessedMessagesPanel.Location = new Point(12, 476);
            ProcessedMessagesPanel.Name = "ProcessedMessagesPanel";
            ProcessedMessagesPanel.Size = new Size(1500, 300);
            ProcessedMessagesPanel.TabIndex = 11;
            // 
            // txtMessageCount
            // 
            txtMessageCount.Location = new Point(181, 63);
            txtMessageCount.Name = "txtMessageCount";
            txtMessageCount.Size = new Size(125, 27);
            txtMessageCount.TabIndex = 15;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 115);
            label2.Name = "label2";
            label2.Size = new Size(143, 20);
            label2.TabIndex = 12;
            label2.Text = "Messages in process";
            // 
            // startManual
            // 
            startManual.Location = new Point(12, 12);
            startManual.Name = "startManual";
            startManual.Size = new Size(94, 29);
            startManual.TabIndex = 10;
            startManual.Text = "Start";
            startManual.UseVisualStyleBackColor = true;
            startManual.Click += startManual_Click;
            // 
            // InProcessPanel
            // 
            InProcessPanel.BorderStyle = BorderStyle.FixedSingle;
            InProcessPanel.Location = new Point(12, 138);
            InProcessPanel.Name = "InProcessPanel";
            InProcessPanel.Size = new Size(1500, 300);
            InProcessPanel.TabIndex = 9;
            // 
            // Portal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1630, 855);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(ProcessedMessagesPanel);
            Controls.Add(txtMessageCount);
            Controls.Add(label2);
            Controls.Add(startManual);
            Controls.Add(InProcessPanel);
            Name = "Portal";
            Text = "Portal";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label4;
        private Label label3;
        private FlowLayoutPanel ProcessedMessagesPanel;
        private TextBox txtMessageCount;
        private Label label2;
        private Button startManual;
        private FlowLayoutPanel InProcessPanel;
    }
}