using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Z80_RC2014
{
    public partial class CompactFlash : UserControl
    {
        #region Members

        // Standard folder for load/save files
        private string folder = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        #endregion

        #region Constructor

        public CompactFlash()
        {
            InitializeComponent();

            // Create empty image with 16 drives and 512 dir entries each
            New(512);
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Show boot sector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBoot_Click(object sender, EventArgs e)
        {
            // Create form for display of results                
            Form bootForm = new Form();
            bootForm.Name = "FormBoot";
            bootForm.Text = "Boot 'Sector'";
            bootForm.Icon = Properties.Resources.Z80;
            bootForm.Size = new Size(680, 600);
            bootForm.MinimumSize = new Size(680, 600);
            bootForm.MaximumSize = new Size(680, 600);
            bootForm.MaximizeBox = false;
            bootForm.MinimizeBox = false;
            bootForm.StartPosition = FormStartPosition.CenterScreen;

            // Create button for closing (dialog)form
            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(584, 530);
            btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOk.Visible = true;
            btnOk.Click += new EventHandler((object o, EventArgs a) =>
            {
                bootForm.Close();
            });

            Font font = new Font(FontFamily.GenericMonospace, 10.25F);

            // Compose 
            DiskImage diskImage = (DiskImage)lbImages.SelectedItem;
            string boot = "";
            string ascii = "";
            for (int address = 0; address < DiskImage.BOOTSIZE; address++)
            {
                if (address % 16 == 0)
                {
                    if (address != 0) boot += "  " + ascii + "\r\n";
                    ascii = "";
                    boot += address.ToString("X").PadLeft(4, '0') + ":   ";
                }

                boot += diskImage.bytes[address].ToString("X").PadLeft(2, '0') + " ";
                if ((diskImage.bytes[address] < 128) && (diskImage.bytes[address] >= 32)) ascii += Convert.ToChar(diskImage.bytes[address]); else ascii += ".";
            }

            // Add controls to form
            TextBox textBox = new TextBox();
            textBox.Multiline = true;
            textBox.WordWrap = false;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.ReadOnly = true;
            textBox.BackColor = Color.LightYellow;
            textBox.Size = new Size(648, 510);
            textBox.Text = boot;
            textBox.Font = font;
            textBox.BorderStyle = BorderStyle.None;
            textBox.Location = new Point(10, 10);
            textBox.Select(0, 0);

            bootForm.Controls.Add(textBox);
            bootForm.Controls.Add(btnOk);

            // Show form
            bootForm.Show();
        }

        /// <summary>
        /// Open image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select Image File";
            fileDialog.InitialDirectory = folder;
            fileDialog.FileName = "";
            fileDialog.Multiselect = false;
            fileDialog.Filter = "CPM Disk Image (Container)|*.img;*.cpm|All Files|*.*";

            if (fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                string fileName = fileDialog.FileName;
                folder = Path.GetDirectoryName(fileDialog.FileName);

                // Read image file into byte array
                byte[] bytes = File.ReadAllBytes(fileName);

                // Check if this is a single diskimage
                if (fileName.ToLower().EndsWith(".cpm"))
                {
                    if (lbImages.Items.Count >= 16)
                    {
                        MessageBox.Show("No more room for images (max = drive 'P')", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Create new diskimage object
                    int max_dir = 512;
                    byte drive = (byte)('A' + lbImages.Items.Count);
                    DiskImage diskImage = new DiskImage(Convert.ToChar(drive).ToString(), bytes, max_dir);

                    // Add the new object (image) to the listbox
                    lbImages.Items.Add(diskImage);
                } else
                {
                    // If not, assume a container of 8MB disk images (2MB image at the end ?)
                    lbImages.Items.Clear();
                    btnBoot.Visible = false;

                    string folderName = "";
                    string[] temp = fileName.Split('\\');
                    for (int i = 0; i < temp.Length - 1; i++) folderName += temp[i] + "\\";

                    int parts = bytes.Length / 0x00800000;
                    if (bytes.Length % 0x00800000 != 0) parts++;

                    for (int part = 0; part < parts; part++)
                    {
                        int bytes_left = bytes.Length - (part * 0x00800000);
                        if (bytes_left > 0x00800000) bytes_left = 0x00800000;

                        byte[] bytes_image = new byte[bytes_left];
                        Array.Copy(bytes, part * 0x00800000, bytes_image, 0, bytes_left);

                        // Create new diskimage object
                        int max_dir = Convert.ToInt32(512);
                        byte drive = (byte)('A' + lbImages.Items.Count);

                        DiskImage diskImage = new DiskImage(Convert.ToChar(drive).ToString(), bytes_image, max_dir);

                        if ((drive == 'P') && (bytes_image.Length > (2 * 1024 * 1024)))
                        {
                            MessageBox.Show("Drive 'P' is larger then 2MB.\r\nThis may not fit a Compact Flash Card of 128MB", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // Add the new object (image) to the listbox
                        lbImages.Items.Add(diskImage);
                    }
                }

                // Show files in this image
                lbImages.SelectedIndex = lbImages.Items.Count - 1;
                ShowImageFiles((DiskImage)lbImages.SelectedItem);
            }
        }

        /// <summary>
        /// Save image(container)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lbImages.Items.Count > 0)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Title = "Save File As";
                fileDialog.InitialDirectory = folder;
                fileDialog.FileName = "";
                fileDialog.Filter = "IMG Disk Image Container|*.img|All Files|*.*";

                if (fileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    string fileNameContainer = fileDialog.FileName;
                    folder = Path.GetDirectoryName(fileDialog.FileName);

                    int size = 0;
                    foreach (DiskImage diskImage in lbImages.Items)
                    {
                        size += diskImage.bytes.Length;
                    }

                    byte[] bytes = new byte[size];
                    int index = 0;
                    foreach (DiskImage diskImage in lbImages.Items)
                    {
                        for (int i = 0; i < diskImage.bytes.Length; i++)
                        {
                            bytes[index++] = diskImage.bytes[i];
                        }
                    }

                    // Save binary file
                    File.WriteAllBytes(fileNameContainer, bytes);

                    MessageBox.Show("File saved as '" + fileNameContainer + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// New drives
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            New(512);
        }

        /// <summary>
        /// Reload file index from memory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReload_Click(object sender, EventArgs e)
        {
            if (lbImages.SelectedItem != null)
            {
                DiskImage diskImage = (DiskImage)lbImages.SelectedItem;
                diskImage.SetFileInfo();
                ShowImageFiles(diskImage);
            }
        }

        /// <summary>
        /// Image file selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show files in this image
            if (lbImages.SelectedItem != null)
            {
                ShowImageFiles((DiskImage)lbImages.SelectedItem);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// New image
        /// </summary>
        /// <param name="max_dir"></param>
        public void New(int max_dir)
        {
            // Delete old drives
            lbImages.Items.Clear();

            // Create 16 drives (A to P)
            for (int index = 0; index < 16; index++)
            {
                // Create new empty image, standard 8MB. Only 2Mb if last one (P)
                char drive = (char)('A' + index);
                byte[] bytes;

                if (drive > 'P')
                {
                    MessageBox.Show("No more room for images (max = drive 'P')", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                } else if (drive == 'P')
                {
                    bytes = new byte[0x00200000];
                } else
                {
                    bytes = new byte[0x00800000];
                }

                DiskImage diskImage = new DiskImage(drive.ToString(), bytes, max_dir);

                // Set dir entries to empty
                for (int i = 0; i < max_dir; i++)
                {
                    if (diskImage.boot)
                    {
                        diskImage.bytes[DiskImage.BOOTSIZE + i * 32] = 0xE5;
                    } else
                    {
                        diskImage.bytes[i * 32] = 0xE5;
                    }
                }

                // Update file info
                diskImage.SetFileInfo();

                // Add the new object (image) to the listbox
                lbImages.Items.Add(diskImage);
            }

            lbImages.SelectedIndex = 0;
        }

        /// <summary>
        /// Show files in this image
        /// </summary>
        /// <param name="diskImage"></param>
        private void ShowImageFiles(DiskImage diskImage)
        {
            dgvFiles.Rows.Clear();
            dgvFiles.Columns.Clear();

            if (diskImage == null) return;

            // Show size
            lblSize.Text = "Size: " + (diskImage.bytes.Length / 0x100000).ToString() + " MB";

            // Show boot (if assigned)
            btnBoot.Visible = false;
            if (diskImage.boot) btnBoot.Visible = true;

            // Create font for header text
            Font font = new Font("Tahoma", 8.25F, FontStyle.Bold);

            // Fill datagridview with info
            dgvFiles.Columns.Add("index", "Index");
            dgvFiles.Columns.Add("number", "User Number");
            dgvFiles.Columns.Add("file", "File");
            dgvFiles.Columns.Add("type", "Type");
            dgvFiles.Columns.Add("extend_high", "Extend Counter High");
            dgvFiles.Columns.Add("extend_low", "Extend Counter Low");
            dgvFiles.Columns.Add("count", "Record Count");
            dgvFiles.Columns["count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvFiles.Columns.Add("size", "Extent Size");
            dgvFiles.Columns["size"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvFiles.Columns.Add("block", "Block Pointers");

            foreach (DataGridViewColumn column in dgvFiles.Columns)
            {
                column.HeaderCell.Style.Font = font;
            }

            for (int i = 0; i < diskImage.max_dir; i++)
            {
                // If user number is not E5 then file entry not empty
                if (diskImage.files[i].user_number != 0xE5)
                {
                    dgvFiles.Rows.Add();
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["index"].Value = i;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["number"].Value = diskImage.files[i].user_number;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["file"].Value = diskImage.files[i].file_name.Trim();
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["type"].Value = diskImage.files[i].file_type.Trim();
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["extend_high"].Value = diskImage.files[i].extend_counter_high;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["extend_low"].Value = diskImage.files[i].extend_counter_low;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["count"].Value = diskImage.files[i].record_count;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["size"].Value = diskImage.files[i].extent_size;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["block"].Value = (diskImage.files[i].block_pointers[ 0] + diskImage.files[i].block_pointers[ 1] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 2] + diskImage.files[i].block_pointers[ 3] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 4] + diskImage.files[i].block_pointers[ 5] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 6] + diskImage.files[i].block_pointers[ 7] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 8] + diskImage.files[i].block_pointers[ 9] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[10] + diskImage.files[i].block_pointers[11] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[12] + diskImage.files[i].block_pointers[13] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[14] + diskImage.files[i].block_pointers[15] * 256).ToString();
                }
            }
        }

        /// <summary>
        /// Insert a file in a diskimage
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="binary"></param>
        /// <param name="fileName"></param>
        public void InsertFile(string volume, byte[] binary, string fileName = "")
        {
            DiskImage diskImage = null;
            fileName = fileName.ToUpper();

            // Search for diskimage from this volume
            foreach (DiskImage image in lbImages.Items)
            {
                if (image.volume.ToUpper() == volume.ToUpper())
                {
                    diskImage = image;
                }
            }

            // Select volume in screen
            lbImages.SelectedItem = diskImage;

            if (diskImage == null)
            {
                MessageBox.Show("Volume '" + volume + "' was not found", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If fileName is empty and on disk A then insert as binary (boot) code
            if (diskImage.boot && (fileName == ""))
            {
                // Insert into boot 'sector'
                diskImage.InsertBoot(binary);
                return;
            }

            // Check if the number of files is lower then max
            if (diskImage.num_files >= diskImage.max_dir)
            {
                MessageBox.Show("Too many files on this disk", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check Folder/File names for duplicates
            bool found = false;
            string[] temp = fileName.Split('.');
            string name = temp[0];
            string type = "";
            if (temp.Length > 1) type = temp[1];
            foreach (DiskImage.FILE file in diskImage.files)
            {
                if (file.user_number != 0xE5)
                {
                    if ((file.file_name.Trim() == name.Trim()) && (file.file_type.Trim() == type.Trim())) found = true;
                }
            }

            if (found)
            {
                MessageBox.Show("Already a file with the same name/type present: " + name + "." + type + "\r\nChanging user number for this file", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Insert
            diskImage.InsertFile(name, type, binary);

            // Update screen
            ShowImageFiles(diskImage);
        }

        /// <summary>
        /// Get data from disk
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public byte GetData(int address)
        {
            byte data = 0x00;
            int drive = address / (8 * 0x100000);
            address = address % (8 * 0x100000);

            if (lbImages.Items.Count > 0)
            {
                DiskImage diskImage = (DiskImage)lbImages.Items[drive];
                if (address < diskImage.bytes.Length)
                {
                    data = diskImage.bytes[address];
                }
            }
            
            return data;
        }

        /// <summary>
        /// Put data on disk
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void PutData(int address, byte data)
        {
            int drive = address / (8 * 0x100000);
            address = address % (8 * 0x100000);

            if (lbImages.Items.Count > 0)
            {
                DiskImage diskImage = (DiskImage)lbImages.Items[drive];
                if (address < diskImage.bytes.Length)
                {
                    diskImage.bytes[address] = data;
                }
            }
        }

        #endregion
    }
}
