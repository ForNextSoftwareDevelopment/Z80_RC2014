using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Z80_RC2014
{
    public class DiskImage
    {
        #region Define

        public const int SECTOR_LENGTH  = 512;
        public const int TRACKS         = 64;
        public const int SECTOR_TRACKS  = 256;
        public const int BLOCKSIZE      = 4096;
        public const int RECORDSIZE     = 128;
        public const int BOOTSIZE       = 4 * 4096;

        public struct FILE
        {
            public FILE(byte user_number, string file_name, string file_type, byte extend_counter_low, byte extend_counter_high, byte record_count, byte[] block_pointers)
            {
                this.user_number = user_number;
                this.file_name = file_name;
                this.file_type = file_type;
                this.extend_counter_low = extend_counter_low;
                this.extend_counter_high = extend_counter_high;
                this.record_count = record_count;
                this.block_pointers = block_pointers;
                this.extent_size = 0;

                // Calculate extent size
                if (record_count >= 0x80)
                {
                    extent_size += 0x8000;
                } else
                {
                    if (block_pointers != null)
                    {
                        // Check number of blocks
                        int num_blocks = 0;

                        for (int i = 0; i < block_pointers.Length; i += 2)
                        {
                            if ((block_pointers[i] != 0) || (block_pointers[i + 1] != 0))
                            {
                                num_blocks++;
                            }
                        }

                        // Add size    
                        if (num_blocks <= 4)
                        {
                            // If less or equal then 4 blocks used
                            extent_size += record_count * RECORDSIZE;
                        } else
                        {
                            // If more then 4 blocks used 
                            extent_size += 0x4000;
                            extent_size += record_count * RECORDSIZE;
                        }
                    }
                }
            }

            public byte user_number;
            public string file_name;
            public string file_type;
            public byte extend_counter_low;
            public byte extend_counter_high;
            public byte record_count;
            public int extent_size;
            public byte[] block_pointers;

            public override string ToString() => $"{file_name.Trim()}.{file_type.Trim()}";
        }

        #endregion

        #region Members

        public string volume;
        public int num_files;
        public int max_dir;
        public bool boot;
        public int boot_index;

        public byte[] bytes;

        public FILE[] files;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bytes"></param>
        public DiskImage(string volume, byte[] bytes, int max_dir)
        {
            this.volume = volume;
            this.bytes = bytes;
            this.max_dir = max_dir;
            this.num_files = 0;
            this.boot = false;
            this.boot_index = 0;

            if (volume.ToUpper() == "A")
            {
                boot = true;
            }

            // Create file entries
            files = new FILE[max_dir];

            // Set info for files from the data 
            SetFileInfo();

            // Do some checks to see if the image file is not corrupt
            byte result = 0;
            for (int i = 0; i < num_files; i++)
            {
                if (files[i].file_name == null) result = (byte)(result | 0b00000001);
                if ((files[i].file_name != null) && (Encoding.UTF8.GetByteCount(files[i].file_name) != files[i].file_name.Length)) result = (byte)(result | 0b00000010);
            }

            if ((byte)(result & 0b00000001) == 0b00000001) MessageBox.Show("Some file names are not entered, use with caution !", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if ((byte)(result & 0b00000010) == 0b00000010) MessageBox.Show("Some file names are not ASCII, use with caution !", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return volume;
        }

        /// <summary>
        /// Set file info from data bytes
        /// </summary>
        /// <returns></returns>
        public void SetFileInfo()
        {
            try
            {
                // If this is a boot drive the first 0x4000 bytes are for the system files
                int offset = 0;
                if (boot) offset = BOOTSIZE;
                num_files = 0;

                for (int i = 0; i < max_dir; i++)
                {
                    // Reset file
                    files[i].file_name = null;
                    files[i].file_type = null;
                    files[i].extend_counter_low = 0;
                    files[i].extend_counter_high = 0;
                    files[i].record_count = 0;
                    files[i].block_pointers = new byte[16];
                    files[i].extent_size = 0;

                    // get user number
                    files[i].user_number = Convert.ToByte(bytes[offset + (i * 32)]);

                    // If user number is not E5 then file ok
                    if (files[i].user_number != 0xE5)
                    {
                        num_files++;
                        
                        // Get file name (ignore read-only bit)
                        for (int j = 0; j < 8; j++)
                        {
                            files[i].file_name += Convert.ToChar(bytes[offset + 1 + (i * 32) + j] & 0x7F);
                        }

                        // Get file type (ignore system bit)
                        for (int j = 0; j < 3; j++)
                        {
                            files[i].file_type += Convert.ToChar(bytes[offset + 9 + (i * 32) + j] & 0x7F);
                        }

                        // Get file extension low
                        files[i].extend_counter_low = bytes[offset + 12 + (i * 32)];

                        // Get file extension high
                        files[i].extend_counter_high = bytes[offset + 14 + (i * 32)];

                        // Get number of records (each 128 bytes)
                        files[i].record_count = bytes[offset + 15 + (i * 32)];

                        // Get block pointers
                        files[i].block_pointers = new byte[16];
                        for (int j=0; j<16; j++)
                        {
                            files[i].block_pointers[j] = bytes[offset + 16 + j + (i * 32)];
                        }

                        // Calculate size of this extent
                        if (files[i].record_count >= 0x80)
                        {
                            files[i].extent_size += 0x8000;
                        } else
                        {
                            // Check number of blocks
                            int num_blocks = 0;
                            for (int j = 0; j < files[i].block_pointers.Length; j += 2)
                            {
                                if ((files[i].block_pointers[j] != 0) || (files[i].block_pointers[j + 1] != 0))
                                {
                                    num_blocks++;
                                }
                            }

                            // Add size    
                            if (num_blocks <= 4)
                            {
                                // If less or equal then 4 blocks used
                                files[i].extent_size += files[i].record_count * RECORDSIZE;
                            } else
                            {
                                // If more then 4 blocks used 
                                files[i].extent_size += 0x4000;
                                files[i].extent_size += files[i].record_count * RECORDSIZE;
                            }
                        }
                    }
                }
            } catch (Exception exception)
            {
                MessageBox.Show("Can't get file info: " + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Get binary data from a file
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte[] GetFileData(int index)
        {
            try
            {
                // If this is a boot drive the first 0x4000 bytes are for the system files
                int offset = 0;
                if (boot) offset = BOOTSIZE;

                byte[] data = new byte[0];

                FILE file = files[index];

                // Check for more file entries and copy the files to a 'found' list
                int file_size = 0;

                List<FILE> files_found = new List<FILE>();

                foreach (FILE f in files)
                {
                    if ((f.file_name == file.file_name) && (f.file_type == file.file_type) && (f.user_number == file.user_number))
                    {
                        files_found.Add(f);
                        file_size += f.extent_size;
                    }
                }

                // Data for this file
                int data_ptr = 0;
                data = new byte[file_size];

                // Start with file with extend counter 0 and increase the counter to search for the next file in line
                int extend_counter = 0;
                do
                {
                    foreach (FILE file_found in files_found)
                    {
                        // Check if this file is the extend_counter  
                        if ((file_found.extend_counter_high * 32 + file_found.extend_counter_low) == extend_counter)
                        {
                            for (int block_pointer_index = 0; block_pointer_index < 16; block_pointer_index += 2)
                            {
                                // Check if block pointers are not both 0
                                if ((file_found.block_pointers[block_pointer_index] != 0) || (file_found.block_pointers[block_pointer_index + 1] != 0))
                                {
                                    // Copy bytes 
                                    int image_ptr = offset;
                                    image_ptr += file_found.block_pointers[block_pointer_index] * BLOCKSIZE;
                                    image_ptr += file_found.block_pointers[block_pointer_index + 1] * BLOCKSIZE * 256;

                                    int bytes_copied = 0;

                                    do
                                    {
                                        data[data_ptr++] = bytes[image_ptr++];
                                        bytes_copied++;
                                    } while ((bytes_copied < BLOCKSIZE) && (data_ptr < data.Length));
                                }
                            }
                        }
                    }

                    extend_counter++;

                } while (extend_counter < (256 * 32));
    
                return (data);
            } catch (Exception exception)
            {
                MessageBox.Show("Can't get file data: " + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Delete file from image
        /// </summary>
        /// <param name="index"></param>
        public void DeleteFile(int index)
        {
            try
            {
                FILE file = files[index];

                for (int i=0; i < files.Length; i++)
                {
                    if ((files[i].file_name == file.file_name) && (files[i].file_type == file.file_type) && (files[i].user_number == file.user_number))
                    {
                        // Create a new file entry (user number 0xE5)
                        files[i] = new FILE(0xE5, null, null, 0, 0, 0, null);
                    }
                }
            } catch (Exception exception)
            {
                MessageBox.Show("Can't delete file: " + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Insert file into image 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void InsertFile(string name, string type, byte[] data)
        {
            try
            {
                // If this is a boot drive the first 0x4000 bytes are for the system files
                int offset = 0;
                if (boot) offset = BOOTSIZE;

                // Create a list of free blocks to write to
                List<int> freeBlocks = new List<int>();

                // Check first block available (depends on max_dir, before that block are file entries) 
                int blocks_needed_for_dir = max_dir * 32 / BLOCKSIZE;
                if (max_dir * 32 % BLOCKSIZE != 0) blocks_needed_for_dir++;
                int start_block = blocks_needed_for_dir;

                // Check free blocks 
                bool enough = false;

                for (int block = start_block; !enough && (block <= (bytes.Length - blocks_needed_for_dir * 4096 - offset)); block++)
                {
                    bool free = true;
                    foreach (FILE file in files)
                    {
                        for (int block_pointer_index = 0; block_pointer_index < 16; block_pointer_index += 2)
                        {
                            if ((file.user_number != 0xE5) && (file.block_pointers[block_pointer_index] + file.block_pointers[block_pointer_index + 1] * 256 == block)) free = false;
                        }
                    }

                    if (free) freeBlocks.Add(block);
                    if (freeBlocks.Count > data.Length / BLOCKSIZE) enough = true;
                }

                // No room, so skip this file
                if (!enough)
                {
                    MessageBox.Show("Not enough room to add: " + name + "." + type, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if file allready exists, then use other user number
                int user_number = 0;    
                foreach (FILE file in files)
                {
                    if (file.user_number != 0xE5)
                    {

                        if ((file.user_number >= user_number) && (file.file_name.Trim() == name.Trim()) && (file.file_type.Trim() == type.Trim())) user_number = file.user_number + 1;
                        if (user_number > 0xFF)
                        {
                            MessageBox.Show("No more user numbers to add: " + name + "." + type, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                int data_written = 0;
                int freeBlocks_index = 0;
                byte extend_counter_low = 0;
                byte extend_counter_high = 0;
                while (data_written < data.Length)
                {
                    byte record_count = 0;

                    // Create new set of 8 block pointers (each 2 bytes)
                    byte[] block_pointers = new byte[16];

                    // Fill block pointers from the free list found before
                    int block_pointers_used = 0;
                    for (int block_pointer_index = 0; block_pointer_index < 16; block_pointer_index += 2)
                    {
                        if (freeBlocks_index < freeBlocks.Count)
                        {
                            block_pointers[block_pointer_index] = (byte)(freeBlocks[freeBlocks_index] % 256);
                            block_pointers[block_pointer_index + 1] = (byte)(freeBlocks[freeBlocks_index] / 256);

                            // Update extend counter if more then 4 blocks filled
                            if (block_pointer_index == 8) extend_counter_low++;
                            if (extend_counter_low > 32)
                            {
                                extend_counter_high++;
                                extend_counter_low = 0;
                            }

                            // Copy a block of data from the file to the image bytes
                            for (int i = 0; (i < BLOCKSIZE) && (data_written < data.Length); i++)
                            {
                                bytes[offset + freeBlocks[freeBlocks_index] * BLOCKSIZE + i] = data[data_written++];
                            }

                            // Next free block
                            freeBlocks_index++;
                            block_pointers_used++;
                        } else
                        {
                            block_pointers[block_pointer_index    ] = 0x00;
                            block_pointers[block_pointer_index + 1] = 0x00;
                        }
                    }

                    // Check for full extent
                    if (data_written < data.Length)
                    {
                        // Record count = max
                        record_count = 0x80;
                    } else
                    {
                        if (block_pointers_used <= 4)
                        {
                            record_count = (byte)(data_written / RECORDSIZE);
                            if (data_written % RECORDSIZE != 0) record_count++;
                        } else
                        {
                            record_count = (byte)((data_written - 0x4000) / RECORDSIZE);
                            if ((data_written - 0x4000) % RECORDSIZE != 0) record_count++;
                        }
                    }

                    // Create new file entry
                    FILE file = new FILE((byte)user_number, name, type, extend_counter_low, extend_counter_high, record_count, block_pointers);

                    // Fill empty slot and write file entry into image memory 
                    bool inserted = false;
                    for (int i = 0; !inserted && (i < files.Length); i++)
                    {
                        if (files[i].user_number == 0xE5)
                        {
                            // Fill free file entry with new file(extent)
                            files[i] = file;
                            inserted = true;

                            // Fill user number
                            bytes[offset + (i * 32)] = file.user_number;

                            // Fill file name (ignore read-only bit)
                            for (int j = 0; j < 8; j++)
                            {
                                char c = ' ';
                                if (j < file.file_name.Length) c = file.file_name[j];
                                bytes[offset + 1 + (i * 32) + j] = (byte)c;
                            }

                            // Fill file type (ignore system bit)
                            for (int j = 0; j < 3; j++)
                            {
                                char c = ' ';
                                if (j < file.file_type.Length) c = file.file_type[j];
                                bytes[offset + 9 + (i * 32) + j] = (byte)c;
                            }

                            // Fill file extension low
                            bytes[offset + 12 + (i * 32)] = file.extend_counter_low;

                            // Fill file extension high
                            bytes[offset + 14 + (i * 32)] = file.extend_counter_high;

                            // Fillt number of records (each 128 bytes)
                            bytes[offset + 15 + (i * 32)] = file.record_count;

                            // Fill block pointers
                            for (int j = 0; j < 16; j++)
                            {
                                bytes[offset + 16 + j + (i * 32)] = file.block_pointers[j];
                            }
                        }
                    }

                    if (!inserted)
                    {
                        MessageBox.Show("No more file extents available to add: " + name + "." + type, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Another extent coming up
                    if (data_written < data.Length)
                    {
                        // Update extent counter 
                        extend_counter_low++;
                        if (extend_counter_low > 32)
                        {
                            extend_counter_high++;
                            extend_counter_low = 0;
                        }

                        // Record count = max
                        record_count = 0x80;
                    }

                }
            } catch (Exception exception)
            {
                MessageBox.Show("Can't insert file: " + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Insert a binary file in the boot 'sector' of this image (always A drive)
        /// </summary>
        /// <param name="binary"></param>
        public void InsertBoot(byte[] binary)
        {
            if (binary.Length > (BOOTSIZE - boot_index))
            {
                MessageBox.Show("File to big to fit in the boot space\r\nSize: " + binary.Length + "\r\nFree: " + (DiskImage.BOOTSIZE - boot_index).ToString(), "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < binary.Length; i++)
            {
                bytes[boot_index++] = binary[i];
            }
        }

        #endregion
    }
}
