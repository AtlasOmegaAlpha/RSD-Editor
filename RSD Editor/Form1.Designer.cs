namespace RSD_Editor
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
            components = new System.ComponentModel.Container();
            TreeNode treeNode1 = new TreeNode("Mii List", 0, 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            listContextMenuStrip = new ContextMenuStrip(components);
            exportAllButton = new ToolStripMenuItem();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openButton = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            saveButton = new ToolStripMenuItem();
            saveAsButton = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitButton = new ToolStripMenuItem();
            menuStrip1 = new MenuStrip();
            miiContextMenuStrip = new ContextMenuStrip(components);
            exportButton = new ToolStripMenuItem();
            replaceButton = new ToolStripMenuItem();
            treeView1 = new TreeView();
            imageList1 = new ImageList(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            newButton = new ToolStripMenuItem();
            importButton = new ToolStripMenuItem();
            deleteButton = new ToolStripMenuItem();
            listContextMenuStrip.SuspendLayout();
            menuStrip1.SuspendLayout();
            miiContextMenuStrip.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listContextMenuStrip
            // 
            listContextMenuStrip.Items.AddRange(new ToolStripItem[] { importButton, exportAllButton });
            listContextMenuStrip.Name = "listContextMenuStrip";
            listContextMenuStrip.Size = new Size(193, 70);
            // 
            // exportAllButton
            // 
            exportAllButton.Image = (Image)resources.GetObject("exportAllButton.Image");
            exportAllButton.Name = "exportAllButton";
            exportAllButton.ShortcutKeys = Keys.Control | Keys.E;
            exportAllButton.Size = new Size(192, 22);
            exportAllButton.Text = "Export All";
            exportAllButton.Click += exportAllButton_Click;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newButton, openButton, toolStripSeparator1, saveButton, saveAsButton, toolStripSeparator2, exitButton });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openButton
            // 
            openButton.Image = (Image)resources.GetObject("openButton.Image");
            openButton.Name = "openButton";
            openButton.ShortcutKeys = Keys.Control | Keys.O;
            openButton.Size = new Size(186, 22);
            openButton.Text = "Open";
            openButton.Click += openButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(183, 6);
            // 
            // saveButton
            // 
            saveButton.Enabled = false;
            saveButton.Image = (Image)resources.GetObject("saveButton.Image");
            saveButton.Name = "saveButton";
            saveButton.ShortcutKeys = Keys.Control | Keys.S;
            saveButton.Size = new Size(186, 22);
            saveButton.Text = "Save";
            saveButton.Click += saveButton_Click;
            // 
            // saveAsButton
            // 
            saveAsButton.Enabled = false;
            saveAsButton.Image = (Image)resources.GetObject("saveAsButton.Image");
            saveAsButton.Name = "saveAsButton";
            saveAsButton.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsButton.Size = new Size(186, 22);
            saveAsButton.Text = "Save As";
            saveAsButton.Click += saveAsButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(183, 6);
            // 
            // exitButton
            // 
            exitButton.Image = (Image)resources.GetObject("exitButton.Image");
            exitButton.Name = "exitButton";
            exitButton.ShortcutKeys = Keys.Control | Keys.Delete;
            exitButton.Size = new Size(186, 22);
            exitButton.Text = "Exit";
            exitButton.Click += exitButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(358, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // miiContextMenuStrip
            // 
            miiContextMenuStrip.Items.AddRange(new ToolStripItem[] { exportButton, replaceButton, deleteButton });
            miiContextMenuStrip.Name = "miiContextMenuStrip";
            miiContextMenuStrip.Size = new Size(157, 70);
            // 
            // exportButton
            // 
            exportButton.Image = (Image)resources.GetObject("exportButton.Image");
            exportButton.Name = "exportButton";
            exportButton.ShortcutKeys = Keys.Control | Keys.E;
            exportButton.Size = new Size(156, 22);
            exportButton.Text = "Export";
            exportButton.Click += exportButton_Click;
            // 
            // replaceButton
            // 
            replaceButton.Image = (Image)resources.GetObject("replaceButton.Image");
            replaceButton.Name = "replaceButton";
            replaceButton.ShortcutKeys = Keys.Control | Keys.R;
            replaceButton.Size = new Size(156, 22);
            replaceButton.Text = "Replace";
            replaceButton.Click += replaceButton_Click;
            // 
            // treeView1
            // 
            treeView1.BackColor = SystemColors.Window;
            treeView1.BorderStyle = BorderStyle.FixedSingle;
            treeView1.Dock = DockStyle.Fill;
            treeView1.Enabled = false;
            treeView1.ImageIndex = 0;
            treeView1.ImageList = imageList1;
            treeView1.Location = new Point(0, 24);
            treeView1.Name = "treeView1";
            treeNode1.ContextMenuStrip = listContextMenuStrip;
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node0";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "Mii List";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode1 });
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new Size(358, 260);
            treeView1.TabIndex = 0;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            treeView1.KeyDown += treeView1_KeyDown;
            treeView1.KeyPress += treeView1_KeyPress;
            treeView1.KeyUp += treeView1_KeyUp;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "mii.png");
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2 });
            contextMenuStrip1.Name = "miiContextMenuStrip";
            contextMenuStrip1.Size = new Size(157, 48);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Image = (Image)resources.GetObject("toolStripMenuItem1.Image");
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.E;
            toolStripMenuItem1.Size = new Size(156, 22);
            toolStripMenuItem1.Text = "Export";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Image = (Image)resources.GetObject("toolStripMenuItem2.Image");
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.ShortcutKeys = Keys.Control | Keys.R;
            toolStripMenuItem2.Size = new Size(156, 22);
            toolStripMenuItem2.Text = "Replace";
            // 
            // newButton
            // 
            newButton.Image = (Image)resources.GetObject("newButton.Image");
            newButton.Name = "newButton";
            newButton.ShortcutKeys = Keys.Control | Keys.N;
            newButton.Size = new Size(186, 22);
            newButton.Text = "New";
            newButton.Click += newButton_Click;
            // 
            // importButton
            // 
            importButton.Image = (Image)resources.GetObject("importButton.Image");
            importButton.Name = "importButton";
            importButton.ShortcutKeys = Keys.Control | Keys.Oemplus;
            importButton.Size = new Size(192, 22);
            importButton.Text = "Import";
            importButton.Click += importButton_Click;
            // 
            // deleteButton
            // 
            deleteButton.Image = (Image)resources.GetObject("deleteButton.Image");
            deleteButton.Name = "deleteButton";
            deleteButton.ShortcutKeys = Keys.Delete;
            deleteButton.Size = new Size(156, 22);
            deleteButton.Text = "Delete";
            deleteButton.Click += deleteButton_Click;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(358, 284);
            Controls.Add(treeView1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(200, 200);
            Name = "Form1";
            Text = "RSD Editor";
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            listContextMenuStrip.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            miiContextMenuStrip.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStripMenuItem fileToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem openButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem saveButton;
        private ToolStripMenuItem saveAsButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitButton;
        private ContextMenuStrip miiContextMenuStrip;
        private ToolStripMenuItem exportButton;
        private ToolStripMenuItem replaceButton;
        private TreeView treeView1;
        private ImageList imageList1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ContextMenuStrip listContextMenuStrip;
        private ToolStripMenuItem exportAllButton;
        private ToolStripMenuItem newButton;
        private ToolStripMenuItem importButton;
        private ToolStripMenuItem deleteButton;
    }
}