using Compression;
using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using Util.Mii;
using Util.Validation;

namespace RSD_Editor
{
    public partial class Form1 : Form
    {
        private bool compressed;
        private List<byte[]> miis = new List<byte[]>();
        private List<string> miiNames = new List<string>();
        private string filePath = "";

        private string miiFilter = ".mae file (*.mae)|*.mae|.mii file (*.mii)|*.mii|*.miigx file (*.miigx)|*.miigx";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null)
                return;

            OpenFile(((string[])e.Data.GetData(DataFormats.FileDrop, false))[0]);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All supported files (*.bin; *.bin.lz)|*.bin;*.bin.lz|mii_rsd.bin (*.bin)|*.bin|mii_rsd.bin.lz (*.bin.lz)|*.bin.lz";
            if (ofd.ShowDialog() == DialogResult.OK)
                OpenFile(ofd.FileName);
        }

        public void OpenFile(string fileName)
        {
            treeView1.Enabled = false;
            saveButton.Enabled = false;
            saveAsButton.Enabled = false;
            treeView1.Nodes[0].Nodes.Clear();
            miis = new List<byte[]>();
            miiNames = new List<string>();

            EndianReader reader = new EndianReader(File.Open(fileName, FileMode.Open), Endianness.BigEndian);
            if (reader.StreamLength < 4)
                return;

            byte headerMagic = reader.ReadByte();
            reader.Position = 0;
            if (headerMagic == 0x11)
            {
                Stream input = reader.Stream;
                MemoryStream ms = new MemoryStream();
                LZ11.Decompress(input, ms);
                reader.Close();
                reader = new EndianReader(ms, Endianness.BigEndian);
                compressed = true;
            }
            else
                compressed = false;

            reader.Position = 0;
            int nrMiis = (int)(reader.StreamLength / 0x4C);
            for (int i = 0; i < nrMiis; i++)
            {
                byte[] miiData = reader.ReadBytes(0x4A);
                string miiName = MiiData.GetMiiName(miiData);
                miis.Add(miiData);
                miiNames.Add(miiName);
                reader.Position += 2; // checksum

                TreeNode t = new TreeNode("[" + i + "] " + miiName);
                t.ContextMenuStrip = miiContextMenuStrip;
                treeView1.Nodes[0].Nodes.Add(t);
            }

            reader.Close();
            filePath = fileName;

            treeView1.Enabled = true;
            saveButton.Enabled = true;
            saveAsButton.Enabled = true;
            treeView1.ExpandAll();
        }

        private void SaveFile(string fileName)
        {
            EndianWriter writer = new EndianWriter(new MemoryStream(), Endianness.BigEndian);
            for (int i = 0; i < miis.Count; i++)
            {
                writer.WriteBytes(miis[i]);
                writer.WriteUInt16(MiiData.CalculateCRC(miis[i]));
            }

            if (compressed)
            {
                writer.Position = 0;
                Stream input = writer.Stream;
                MemoryStream ms = new MemoryStream();
                LZ11.Compress(input, ms);
                writer.Close();
                File.WriteAllBytes(fileName, ms.ToArray());
            }
            else
                File.WriteAllBytes(fileName, ((MemoryStream)writer.Stream).ToArray());
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                treeView1.SelectedNode = e.Node;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ExportAllMiis()
        {
            ExtensionSelector extensionSelector = new ExtensionSelector();
            if (extensionSelector.ShowDialog() != DialogResult.OK)
                return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            int i = 0;
            foreach (byte[] mii in miis)
            {
                File.WriteAllBytes(fbd.SelectedPath + "/" + i + "_" + Validation.ValidateFileName(miiNames[i]) + extensionSelector.selectedExtension, mii);
                i++;
            }

            MessageBox.Show("Exported " + miis.Count + " files into " + fbd.SelectedPath, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportMii(int index)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = miiFilter;
            sfd.FileName = index + "_" + Validation.ValidateFileName(miiNames[index]) + ".mae";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllBytes(sfd.FileName, miis[index]);
        }

        private void ReplaceMii(int index)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = miiFilter + "|All files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            FileInfo fileInfo = new FileInfo(ofd.FileName);
            if (fileInfo.Length != 0x4A)
            {
                MessageBox.Show("Invalid Mii file: File size must be 0x4A bytes long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] miiData = File.ReadAllBytes(ofd.FileName);
            string miiName = MiiData.GetMiiName(miiData);

            miis[index] = miiData;
            miiNames[index] = miiName;
            treeView1.Nodes[0].Nodes[index].Text = "[" + index + "] " + miiName;
        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {
            ExportAllMiis();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Parent == null)
                return;

            int i = treeView1.SelectedNode.Index;
            ExportMii(i);
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Parent == null)
                return;

            int i = treeView1.SelectedNode.Index;
            ReplaceMii(i);
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!treeView1.Enabled)
                return;

            if (e.Modifiers != Keys.Control)
                return;

            if (e.KeyCode == Keys.E)
            {
                if (treeView1.SelectedNode.Parent == null)
                    ExportAllMiis();
                else
                    ExportMii(treeView1.SelectedNode.Index);

            }
            else if (e.KeyCode == Keys.R)
            {
                if (treeView1.SelectedNode.Parent != null)
                    ReplaceMii(treeView1.SelectedNode.Index);
            }
        }

        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void treeView1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFile(filePath);
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = compressed ? "mii_rsd.bin.lz (*.bin.lz)|*.bin.lz|mii_rsd.bin (*.bin)|*.bin" : "mii_rsd.bin (*.bin)|*.bin|mii_rsd.bin.lz (*.bin.lz)|*.bin.lz";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            compressed = sfd.FileName.EndsWith(".lz");
            SaveFile(sfd.FileName);
        }
    }
}