using Compression;
using System;
using System.Collections;
using System.Data.SqlTypes;
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

        private string miiFilter = ".rsd file (*.rsd)|*.rsd|.mae file (*.mae)|*.mae|.mii file (*.mii)|*.mii|*.miigx file (*.miigx)|*.miigx";

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
            ofd.Filter = "All supported files (*.rsd; *.bin; *.bin.lz)|*.rsd;*.bin;*.bin.lz|RSD files (*.rsd)|mii_rsd.bin (*.bin)|*.bin|mii_rsd.bin.lz (*.bin.lz)|*.bin.lz";
            if (ofd.ShowDialog() == DialogResult.OK)
                OpenFile(ofd.FileName);
        }

        private void Clear(bool newFile)
        {
            treeView1.Enabled = newFile;
            saveButton.Enabled = newFile;
            saveAsButton.Enabled = newFile;
            treeView1.Nodes[0].Nodes.Clear();
            miis = new List<byte[]>();
            miiNames = new List<string>();
            compressed = false;
        }

        public void OpenFile(string fileName)
        {
            Clear(false);

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
            if (fileName.ToLower().EndsWith(".rsd") && miis.Count > 1)
            {
                DialogResult dr = MessageBox.Show("WARNING: Saving as a single RSD file, but there is more than 1 Mii in this file (RSD files are meant to be a single Mii file). Proceed?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr != DialogResult.OK)
                    return;
            }

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

        private byte[] GetMiiWithChecksum(byte[] mii)
        {
            byte[] miiBytes = mii.ToArray();
            byte[] checksum = BitConverter.GetBytes(MiiData.CalculateCRC(mii));
            Array.Reverse(checksum);
            miiBytes = miiBytes.Concat(checksum).ToArray();
            return miiBytes;
        }

        private void ExportAllMiis()
        {
            if (miis.Count <= 0)
            {
                MessageBox.Show("There's nothing to export!", "Empty file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ExtensionSelector extensionSelector = new ExtensionSelector();
            if (extensionSelector.ShowDialog() != DialogResult.OK)
                return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            int i = 0;
            foreach (byte[] mii in miis)
            {
                byte[] miiBytes = mii.ToArray();
                if (extensionSelector.selectedExtension == ".rsd")
                    miiBytes = GetMiiWithChecksum(mii);

                File.WriteAllBytes(fbd.SelectedPath + "/" + i + "_" + Validation.ValidateFileName(miiNames[i]) + extensionSelector.selectedExtension, miiBytes);
                i++;
            }

            MessageBox.Show("Exported " + miis.Count + " files into " + fbd.SelectedPath, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportMii(int index)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = miiFilter;
            sfd.FileName = index + "_" + Validation.ValidateFileName(miiNames[index]) + ".rsd";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            byte[] miiBytes = miis[index].ToArray();
            if (sfd.FileName.ToLower().EndsWith(".rsd"))
                miiBytes = GetMiiWithChecksum(miis[index]);

            File.WriteAllBytes(sfd.FileName, miiBytes);
        }

        private void AddOrReplaceMii(int index)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = miiFilter + "|All files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            bool isRsd = false;
            FileInfo fileInfo = new FileInfo(ofd.FileName);
            if (ofd.FileName.ToLower().EndsWith(".rsd"))
            {
                if (fileInfo.Length != 0x4C)
                {
                    MessageBox.Show("Invalid RSD file: File size must be 0x4C bytes long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                isRsd = true;
            }
            else if (fileInfo.Length != 0x4A)
            {
                MessageBox.Show("Invalid Mii file: File size must be 0x4A bytes long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] miiData = File.ReadAllBytes(ofd.FileName);
            if (isRsd)
                miiData = miiData.Take(miiData.Length - 2).ToArray();
            string miiName = MiiData.GetMiiName(miiData);

            if (index == -1)
            {
                miis.Add(miiData);
                miiNames.Add(miiName);
                TreeNode t = new TreeNode("[" + (miis.Count - 1) + "] " + miiName);
                t.ContextMenuStrip = miiContextMenuStrip;
                treeView1.Nodes[0].Nodes.Add(t);
                treeView1.ExpandAll();
                return;
            }

            miis[index] = miiData;
            miiNames[index] = miiName;
            treeView1.Nodes[0].Nodes[index].Text = "[" + index + "] " + miiName;
        }

        private void DeleteMii(int index)
        {
            miis.RemoveAt(index);
            miiNames.RemoveAt(index);
            treeView1.Nodes[0].Nodes[index].Remove();

            for (int i = index; i < miis.Count; i++)
            {
                treeView1.Nodes[0].Nodes[i].Text = "[" + i + "] " + miiNames[i];
            }
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
            AddOrReplaceMii(i);
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!treeView1.Enabled)
                return;

            if (e.KeyCode == Keys.Delete && treeView1.SelectedNode.Parent != null)
            {
                DeleteMii(treeView1.SelectedNode.Index);
                return;
            }
            else if (e.Modifiers != Keys.Control)
                return;

            switch (e.KeyCode)
            {
                case Keys.E:
                    if (treeView1.SelectedNode.Parent == null)
                        ExportAllMiis();
                    else
                        ExportMii(treeView1.SelectedNode.Index);
                    break;

                case Keys.R:
                    if (treeView1.SelectedNode.Parent != null)
                        AddOrReplaceMii(treeView1.SelectedNode.Index);
                    break;

                case Keys.Oemplus:
                    if (treeView1.SelectedNode.Parent == null)
                        AddOrReplaceMii(-1);
                    break;
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
            if (miis.Count <= 0)
            {
                MessageBox.Show("There's nothing to save!", "Empty file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                SaveAs();
                return;
            }

            SaveFile(filePath);
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            if (miis.Count <= 0)
            {
                MessageBox.Show("There's nothing to save!", "Empty file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveAs();
        }

        private void SaveAs()
        {
            if (miis.Count <= 0)
            {
                MessageBox.Show("There's nothing to save!", "Empty file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            if (!compressed && miis.Count == 1)
                sfd.Filter = "RSD Files (*.rsd)|*.rsd|mii_rsd.bin (*.bin)|*.bin|mii_rsd.bin.lz (*.bin.lz)|*.bin.lz";
            else
                sfd.Filter = compressed ? "mii_rsd.bin.lz (*.bin.lz)|*.bin.lz|mii_rsd.bin (*.bin)|*.bin" : "mii_rsd.bin (*.bin)|*.bin|mii_rsd.bin.lz (*.bin.lz)|*.bin.lz";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            compressed = sfd.FileName.EndsWith(".lz");
            SaveFile(sfd.FileName);
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            Clear(true);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteMii(treeView1.SelectedNode.Index);
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            AddOrReplaceMii(-1);
        }
    }
}