
namespace ApplicationForTransfer_GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lstNodes = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendFIleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstNodes
            // 
            this.lstNodes.ContextMenuStrip = this.contextMenuStrip1;
            this.lstNodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstNodes.FormattingEnabled = true;
            this.lstNodes.Location = new System.Drawing.Point(0, 0);
            this.lstNodes.Name = "lstNodes";
            this.lstNodes.Size = new System.Drawing.Size(734, 518);
            this.lstNodes.TabIndex = 0;
            this.lstNodes.SelectedIndexChanged += new System.EventHandler(this.mmuSendFile_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendFIleToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(122, 26);
            // 
            // sendFIleToolStripMenuItem
            // 
            this.sendFIleToolStripMenuItem.Name = "sendFIleToolStripMenuItem";
            this.sendFIleToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.sendFIleToolStripMenuItem.Text = "Send FIle";
            this.sendFIleToolStripMenuItem.Click += new System.EventHandler(this.mmuSendFile_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(734, 518);
            this.Controls.Add(this.lstNodes);
            this.Name = "MainForm";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstNodes;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendFIleToolStripMenuItem;
    }
}

