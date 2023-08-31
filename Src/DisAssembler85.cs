using System;
using System.Collections.Generic;
using System.Data;

namespace Z80
{
    class DisAssembler85
    {
        #region Members

        // All instructions
        Instructions instructions;

        // Binary file buffer
        private byte[] bytes;

        // Instruction has been decoded already
        private bool[] lineDecoded;

        // All start addresses to be decoded, will be filled along the way after conditional jumps
        private Dictionary<UInt16, bool> addresses;

        // Load address of the code
        private UInt16 loadAddress;

        // Use labels for jump/call adresses
        private bool useLabels;

        // Address Symbol Table
        private Dictionary<UInt16, string> addressSymbolTable = new Dictionary<UInt16, string>();

        // Datatable with addresses and instructions of resulting program 
        private DataTable dtProgram;

        // Resulting (lined) program text
        public string program = "";
        public string linedprogram = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="loadAddress"></param>
        /// <param name="startAddress"></param>
        public DisAssembler85(byte[] bytes, UInt16 loadAddress, UInt16 startAddress, bool useLabels)
        {
            this.bytes = bytes;
            this.loadAddress = loadAddress;
            this.useLabels = useLabels;

            // All instructions
            instructions = new Instructions();

            lineDecoded = new bool[0xFFFF];
            addresses = new Dictionary<ushort, bool>();

            for (int i=0; i<lineDecoded.Length; i++)
            {
                lineDecoded[i] = false;
            }

            // Add first address to start disassemble from
            addresses.Add(startAddress, false);

            // Check if address range falls into hardware interrupts and/or restarts (add those addresses too)
            UInt16 lowestAddress = loadAddress;
            UInt16 highestAddress = Convert.ToUInt16(loadAddress + Convert.ToUInt16(bytes.Length));

            // NMI (Non-Maskable) 
            if ((lowestAddress <= 0x66) && (highestAddress >= 0x00066))
            {
                if (!addresses.ContainsKey(0x0066)) addresses.Add(0x0066, false);
            }

            // RST 0x00 
            if ((lowestAddress <= 0x0000) && (highestAddress >= 0x00000))
            {
                // Must already been added as loadaddress
            }

            // RST 0x08
            if ((lowestAddress <= 0x0008) && (highestAddress >= 0x00008))
            {
                if (!addresses.ContainsKey(0x0008)) addresses.Add(0x0008, false);
            }

            // RST 0x10
            if ((lowestAddress <= 0x0010) && (highestAddress >= 0x00010))
            {
                if (!addresses.ContainsKey(0x0010)) addresses.Add(0x0010, false);
            }

            // RST 0x18
            if ((lowestAddress <= 0x0018) && (highestAddress >= 0x00018))
            {
                if (!addresses.ContainsKey(0x0018)) addresses.Add(0x0018, false);
            }

            // RST 0x20
            if ((lowestAddress <= 0x0020) && (highestAddress >= 0x00020))
            {
                if (!addresses.ContainsKey(0x0020)) addresses.Add(0x0020, false);
            }

            // RST 0x28
            if ((lowestAddress <= 0x0028) && (highestAddress >= 0x00028))
            {
                if (!addresses.ContainsKey(0x0028)) addresses.Add(0x0028, false);
            }

            // RST 0x30
            if ((lowestAddress <= 0x0030) && (highestAddress >= 0x00030))
            {
                if (!addresses.ContainsKey(0x0030)) addresses.Add(0x0030, false);
            }

            // RST 0x38
            if ((lowestAddress <= 0x0038) && (highestAddress >= 0x00038))
            {
                if (!addresses.ContainsKey(0x0038)) addresses.Add(0x0038, false);
            }

            // Init program text
            dtProgram = new DataTable();
            dtProgram.Columns.Add("address", typeof(UInt16));
            dtProgram.Columns.Add("instruction", typeof(String));
            dtProgram.Columns.Add("size", typeof(UInt16));
            dtProgram.Columns.Add("type", typeof(int));

            dtProgram.Rows.Add(null, "org " + loadAddress.ToString("X4"), 0);
        }

        #endregion

        #region Methods

        public string Parse(UInt16 startAddress = 0xFFFF)
        {
            bool ready;

            // If startaddress has been supplied, use this 
            if (startAddress != 0xFFFF)
            {
                if (!addresses.ContainsKey(startAddress))
                {
                    addresses.Add(startAddress, false);
                }
            }

            do
            {
                ready = true;
                
                // Copy address table for the loop (otherwise we can't add to the original addresses dictionary)
                Dictionary<UInt16, bool> addresses_copy = new Dictionary<UInt16, bool>(addresses);
                
                // Check all adddresses in the dictionary for the program path to decode the instructions in this path
                foreach (KeyValuePair<UInt16, bool> keyValuePair in addresses_copy)
                {
                    UInt16 addressKeyValuePair = keyValuePair.Key;
                    UInt16 address = addressKeyValuePair;

                    // Done this program path already ?
                    bool done = keyValuePair.Value;

                    while (((address - loadAddress) >= 0) && ((address - loadAddress) < bytes.Length) && (!lineDecoded[address]) && !done)
                    {
                        // Set address of the current instruction to be processed
                        UInt16 addressCurrentInstruction = address;

                        // Still some path's to go
                        ready = false;

                        // Get opcode (1 or 2 bytes)
                        int opcode = bytes[(UInt16)(address - loadAddress)];
                        lineDecoded[address] = true;
                        address++;

                        if ((opcode == 0xCB) || (opcode == 0xDD) || (opcode == 0xED) || (opcode == 0xFD))
                        {
                            opcode = opcode * 0x100;
                            opcode += bytes[(UInt16)(address - loadAddress)];
                            lineDecoded[address] = true;
                            address++;
                        }

                        string instruction = "UNKNOWN";
                        int size = 0;
                        TYPE type = TYPE.NONE;

                        if (opcode == 0xDDCB)
                        {
                            // IXbit 
                            string operand = bytes[(UInt16)(address - loadAddress)].ToString("x2") + "h";
                            lineDecoded[address] = true;
                            address++;

                            opcode = bytes[(UInt16)(address - loadAddress)];
                            lineDecoded[address] = true;
                            address++;

                            instruction = DecodeIXbit(opcode, out size, out type);

                            instruction = instruction.Replace("o", operand);
                        } else if (opcode == 0xFDCB)
                        {
                            // IYbit 
                            string operand = bytes[(UInt16)(address - loadAddress)].ToString("x2") + "h";
                            lineDecoded[address] = true;
                            address++;

                            opcode = bytes[(UInt16)(address - loadAddress)];
                            lineDecoded[address] = true;
                            address++;

                            instruction = DecodeIYbit(opcode, out size, out type);

                            instruction = instruction.Replace("o", operand);
                        } else
                        {
                            // (Main, Bit, IX, IY and Misc.)
                            instruction = Decode(opcode, out size, out type);

                            // No operands, only the opcode
                            if ((size == 1) || ((size == 2) && (opcode >= 0x100)))
                            {
                                // Check for end of the path (RETURN, RESTART)
                                if ((type == TYPE.UNCONDITIONALRETURN) ||
                                    (type == TYPE.UNCONDITIONALRESTART) ||
                                    (type == TYPE.HALT))
                                {
                                    done = true;
                                }
                            }

                            // 1 operand
                            if (((size == 2) && (opcode < 0x100)) || (size == 3) && (opcode > 0x100))
                            {
                                if ((address - loadAddress) < bytes.Length)
                                {
                                    byte operand = bytes[(UInt16)(address - loadAddress)];
                                    string operand_hexascii = operand.ToString("x2") + "h";
                                    lineDecoded[address] = true;
                                    address++;

                                    // Replace operand in mnemonic with actual value
                                    string[] instructionSplit = instruction.Split(' ');

                                    string arg = instructionSplit[1].Replace(",n", "," + operand_hexascii).Replace("(n)", "(" + operand_hexascii + ")").Replace(",o", "," + operand_hexascii).Replace("+o", "+" + operand_hexascii); ;
                                    if (arg == "n") arg = operand_hexascii;
                                    if (arg == "o") arg = operand_hexascii;

                                    instruction = instructionSplit[0] + " " + arg;

                                    // Check for 'fork' or change of the path (RELATIVE JUMP)
                                    if ((type == TYPE.CONDITIONALJUMP) || (type == TYPE.UNCONDITIONALJUMP))
                                    {
                                        UInt16 jmpAddress = 0;

                                        if (operand < 0x80)
                                        {
                                            // Positive
                                            jmpAddress = (UInt16)(address - loadAddress + operand);
                                        } else
                                        {
                                            // Negative
                                            jmpAddress = (UInt16)(address - loadAddress - (0x100 - operand));
                                        }

                                        if (useLabels)
                                        {
                                            if (!addressSymbolTable.ContainsKey(jmpAddress))
                                            {
                                                // Add to dictionary and insert
                                                int i = 0;

                                                while (addressSymbolTable.ContainsValue("lbl_jmp" + i.ToString("D4"))) i++;
                                                addressSymbolTable.Add(jmpAddress, "lbl_jmp" + i.ToString("D4"));

                                                instruction = instructionSplit[0] + " " + instructionSplit[1].Replace("o", " lbl_jmp" + i.ToString("D4"));
                                            } else
                                            {
                                                // just insert    
                                                instruction = instructionSplit[0] + " " + instructionSplit[1].Replace("o", addressSymbolTable[jmpAddress]);
                                            }
                                        }

                                        if (type == TYPE.CONDITIONALJUMP)
                                        {
                                            if (!addresses.ContainsKey(jmpAddress)) addresses.Add(jmpAddress, false);
                                        }

                                        if (type == TYPE.UNCONDITIONALJUMP)
                                        {
                                            address = jmpAddress;
                                        }
                                    }
                                }
                            }

                            // 2 operands (no address, so 'ld (ix+o),n' or 'ld (iy+o),n')
                            if ((size == 4) && !instruction.Contains("nn"))
                            {
                                string operand;

                                operand = bytes[(UInt16)(address - loadAddress)].ToString("x2") + "h";
                                instruction = instruction.Replace("o", operand);
                                lineDecoded[address] = true;
                                address++;

                                operand = bytes[(UInt16)(address - loadAddress)].ToString("x2") + "h";
                                instruction = instruction.Replace("n", operand);
                                lineDecoded[address] = true;
                                address++;
                            }

                            // 2 operands (address)
                            if (
                                ((size == 3) && (opcode < 0x100)) ||
                                ((size == 4) && instruction.Contains("nn"))
                               )
                            {
                                byte firstByte = 0x00;
                                byte secondByte = 0x00;

                                string first = "?";
                                string second = "?";

                                if ((address - loadAddress) < bytes.Length)
                                {
                                    firstByte = bytes[(UInt16)(address - loadAddress)];
                                    first = firstByte.ToString("x2");
                                    lineDecoded[address] = true;
                                    address++;

                                    if ((address - loadAddress) < bytes.Length)
                                    {
                                        secondByte = bytes[(UInt16)(address - loadAddress)];
                                        second = secondByte.ToString("x2");
                                        lineDecoded[address] = true;
                                        address++;
                                    }
                                }

                                if (useLabels &&
                                    (
                                     (type == TYPE.CONDITIONALJUMP) ||
                                     (type == TYPE.UNCONDITIONALJUMP) ||
                                     (type == TYPE.CONDITIONALCALL) ||
                                     (type == TYPE.UNCONDITIONALCALL)
                                    )
                                   )
                                {
                                    UInt16 jmpsubAddress = (UInt16)(secondByte * 0x100 + firstByte);

                                    if (!addressSymbolTable.ContainsKey(jmpsubAddress))
                                    {
                                        // Add to dictionary and insert
                                        int i = 0;
                                        if ((type == TYPE.CONDITIONALJUMP) || (type == TYPE.UNCONDITIONALJUMP))
                                        {
                                            while (addressSymbolTable.ContainsValue("lbl_jmp" + i.ToString("D4"))) i++;
                                            addressSymbolTable.Add(jmpsubAddress, "lbl_jmp" + i.ToString("D4"));

                                            string[] instructionSplit = instruction.Split(' ');
                                            instruction = instructionSplit[0] + " " + instructionSplit[1].Replace("nn", " lbl_jmp" + i.ToString("D4"));
                                        }

                                        if ((type == TYPE.CONDITIONALCALL) || (type == TYPE.UNCONDITIONALCALL))
                                        {
                                            while (addressSymbolTable.ContainsValue("lbl_sub" + i.ToString("D4"))) i++;
                                            addressSymbolTable.Add(jmpsubAddress, "lbl_sub" + i.ToString("D4"));

                                            string[] instructionSplit = instruction.Split(' ');
                                            instruction = instructionSplit[0] + " " + instructionSplit[1].Replace("nn", " lbl_sub" + i.ToString("D4"));
                                        }
                                    } else
                                    {
                                        // Just insert
                                        string[] instructionSplit = instruction.Split(' ');
                                        instruction = instructionSplit[0] + " " + instructionSplit[1].Replace("nn", addressSymbolTable[jmpsubAddress]);
                                    }
                                } else
                                {
                                    // Inverted (low byte first, high byte second)
                                    string operand = second + first + "h";
                                    string[] instructionSplit = instruction.Split(' ');
                                    instruction = instructionSplit[0] + " " + instructionSplit[1].Replace("nn", operand);
                                }

                                // Check for 'fork' of the path (JUMP ON PARITY, CALL ON ZERO etc.)
                                if ((type == TYPE.CONDITIONALJUMP) ||
                                    (type == TYPE.CONDITIONALCALL) ||
                                    (type == TYPE.UNCONDITIONALCALL))
                                {
                                    UInt16 newAddress = (UInt16)(secondByte * 0x100 + firstByte);
                                    if (!addresses.ContainsKey(newAddress)) addresses.Add(newAddress, false);
                                }

                                // Check for change of the path (JUMP)
                                if (type == TYPE.UNCONDITIONALJUMP)
                                {
                                    address = (UInt16)(secondByte * 0x100 + firstByte);
                                }
                            }
                        }

                        // Add program line (address + instruction)
                        dtProgram.Rows.Add(addressCurrentInstruction, instruction, size, type);
                    }

                    // Set this key has been done
                    addresses[addressKeyValuePair] = true;
                }
            } while (!ready);

            // Order program lines to address
            dtProgram.DefaultView.Sort = "address";
            dtProgram = dtProgram.DefaultView.ToTable();

            // Copy addressSymbolTable to check for use
            Dictionary<UInt16, string> addressSymbolTableNotUsed = new Dictionary<UInt16, string>(addressSymbolTable);

            program = "";
            linedprogram = "";
            UInt16 lastAddress = loadAddress;
            UInt16 lastSize = 0x0000;
            foreach (DataRow row in dtProgram.Rows)
            {
                // If this instruction is at an address which has a gap with the previous one, then fill with data (DB)
                if (row["address"] != DBNull.Value)
                {
                    UInt16 currentAddress = Convert.ToUInt16(row["address"]);

                    UInt16 currentSize = 0;
                    if (row["size"] != DBNull.Value)
                    {
                        currentSize = Convert.ToUInt16(row["size"]);
                    }

                    // Check for gap, treat this as data
                    int dbSize = currentAddress - lastAddress - lastSize;
                    if (dbSize > 0)
                    {
                        string arg = " ";
                        string argASCII = "    ; ASCII: ";
                        for (int i = lastAddress + lastSize; i < currentAddress; i++)
                        {
                            if (i != lastAddress + lastSize) arg += ", ";
                            byte dataByte = bytes[(UInt16)(i - loadAddress)];
                            arg += dataByte.ToString("X2") + "h";
                            argASCII += (dataByte > 0x20) && (dataByte < 0x80) ? ((char)(bytes[(UInt16)(i - loadAddress)])).ToString() : ".";
                        }

                        if (useLabels) program += "             ";
                        program += "db " + arg + argASCII + "\r\n";

                        linedprogram += (lastAddress + lastSize).ToString("X4") + ": ";
                        if (useLabels) linedprogram += "         ";
                        linedprogram += "db " + arg + argASCII + "\r\n";
                    }

                    if (currentAddress != 0) lastAddress = currentAddress;
                    if (currentSize != 0) lastSize = currentSize;
                }

                // If labels are being used, fill them into the program
                if (useLabels)
                {
                    if (row["address"] != DBNull.Value)
                    {
                        UInt16 currentAddress = Convert.ToUInt16(row["address"]);

                        if (addressSymbolTable.ContainsKey(currentAddress))
                        {
                            program += addressSymbolTable[currentAddress] + ": ";
                            program += row["instruction"] + "\r\n";

                            linedprogram += row["address"] != DBNull.Value ? Convert.ToUInt16(row["address"]).ToString("X4") + ": " : "";
                            linedprogram += addressSymbolTable[currentAddress] + ": ";
                            linedprogram += row["instruction"];

                            // Delete from (copied) table
                            if (addressSymbolTableNotUsed.ContainsKey(currentAddress))
                            {
                                addressSymbolTableNotUsed.Remove(currentAddress);
                            }
                        } else
                        {
                            program += "             ";
                            program += row["instruction"] + "\r\n";

                            linedprogram += row["address"] != DBNull.Value ? Convert.ToUInt16(row["address"]).ToString("X4") + ": " : "";
                            linedprogram += "             ";
                            linedprogram += row["instruction"];
                        }
                    } else
                    {
                        program += row["instruction"] + "\r\n";
                        linedprogram += row["address"] != DBNull.Value ? Convert.ToUInt16(row["address"]).ToString("X4") + ": " : "";
                        linedprogram += row["instruction"];
                    }
                } else
                {
                    program += row["instruction"] + "\r\n";
                    linedprogram += row["address"] != DBNull.Value ? Convert.ToUInt16(row["address"]).ToString("X4") + ": " : "";
                    linedprogram += row["instruction"];
                }

                linedprogram += "\r\n";
            }

            // Add not used address labels as EQU statements
            if (useLabels)
            {
                string labels = "";
                foreach (KeyValuePair<UInt16, string> kvp in addressSymbolTableNotUsed)
                {
                    labels += kvp.Value + " equ " + kvp.Key.ToString("X4") + "H\r\n";
                }

                if (labels != "")
                {
                    program = labels + "\r\n" + program;
                    linedprogram = labels + "\r\n" + linedprogram;
                }
            }

            return (program);
        }

        /// <summary>
        /// Decode a single instruction and state the number of operand bytes (Main, Bit, IX, IY and Misc.)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string Decode(int code, out int size, out TYPE type)
        {
            string instruction = "UNKNOWN";
            size = 0;
            type = TYPE.NONE;

            // Check opcode to find the instruction in the list (Main)
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80MainInstructions.Length; indexZ80Instructions++)
            {
                // Opcode matches
                if (code == instructions.Z80MainInstructions[indexZ80Instructions].Opcode)
                {
                    instruction = instructions.Z80MainInstructions[indexZ80Instructions].Mnemonic;
                    size = instructions.Z80MainInstructions[indexZ80Instructions].Size;
                    type = instructions.Z80MainInstructions[indexZ80Instructions].Type;

                    return (instruction);
                }
            }

            // Check opcode to find the instruction in the list (Bit)
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80BitInstructions.Length; indexZ80Instructions++)
            {
                // Opcode matches
                if (code == instructions.Z80BitInstructions[indexZ80Instructions].Opcode)
                {
                    instruction = instructions.Z80BitInstructions[indexZ80Instructions].Mnemonic;
                    size = instructions.Z80BitInstructions[indexZ80Instructions].Size;
                    type = instructions.Z80BitInstructions[indexZ80Instructions].Type;

                    return (instruction);
                }
            }

            // Check opcode to find the instruction in the list (IX)
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80IXInstructions.Length; indexZ80Instructions++)
            {
                // Opcode matches
                if (code == instructions.Z80IXInstructions[indexZ80Instructions].Opcode)
                {
                    instruction = instructions.Z80IXInstructions[indexZ80Instructions].Mnemonic;
                    size = instructions.Z80IXInstructions[indexZ80Instructions].Size;
                    type = instructions.Z80IXInstructions[indexZ80Instructions].Type;

                    return (instruction);
                }
            }

            // Check opcode to find the instruction in the list (IY)
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80IYInstructions.Length; indexZ80Instructions++)
            {
                // Opcode matches
                if (code == instructions.Z80IYInstructions[indexZ80Instructions].Opcode)
                {
                    instruction = instructions.Z80IYInstructions[indexZ80Instructions].Mnemonic;
                    size = instructions.Z80IYInstructions[indexZ80Instructions].Size;
                    type = instructions.Z80IYInstructions[indexZ80Instructions].Type;

                    return (instruction);
                }
            }

            // Check opcode to find the instruction in the list (Misc.)
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80MiscInstructions.Length; indexZ80Instructions++)
            {
                // Opcode matches
                if (code == instructions.Z80MiscInstructions[indexZ80Instructions].Opcode)
                {
                    instruction = instructions.Z80MiscInstructions[indexZ80Instructions].Mnemonic;
                    size = instructions.Z80MiscInstructions[indexZ80Instructions].Size;
                    type = instructions.Z80MiscInstructions[indexZ80Instructions].Type;

                    return (instruction);
                }
            }

            return (instruction);
        }

        /// <summary>
        /// Decode a single instruction and state the number of operand bytes (IXbit)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string DecodeIXbit(int code, out int size, out TYPE type)
        {
            string instruction = "UNKNOWN";
            size = 0;
            type = TYPE.NONE;

            // Check opcode to find the instruction in the list (IXbit)
            for (int indexZ80IXBitInstructions = 0; indexZ80IXBitInstructions < instructions.Z80IXBitInstructions.Length; indexZ80IXBitInstructions++)
            {
                // Opcode matches
                if (code == instructions.Z80IXBitInstructions[indexZ80IXBitInstructions].Opcode)
                {
                    instruction = instructions.Z80IXBitInstructions[indexZ80IXBitInstructions].Mnemonic;
                    size = instructions.Z80IXBitInstructions[indexZ80IXBitInstructions].Size;
                    type = instructions.Z80IXBitInstructions[indexZ80IXBitInstructions].Type;

                    return (instruction);
                }
            }

            return (instruction);
        }

        /// <summary>
        /// Decode a single instruction and state the number of operand bytes (IYbit)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string DecodeIYbit(int code, out int size, out TYPE type)
        {
            string instruction = "UNKNOWN";
            size = 0;
            type = TYPE.NONE;

            // Check opcode to find the instruction in the list (IYbit)
            for (int indexZ80IYBitInstructions = 0; indexZ80IYBitInstructions < instructions.Z80IYBitInstructions.Length; indexZ80IYBitInstructions++)
            {
                // Opcode matches
                if (code == instructions.Z80IYBitInstructions[indexZ80IYBitInstructions].Opcode)
                {
                    instruction = instructions.Z80IYBitInstructions[indexZ80IYBitInstructions].Mnemonic;
                    size = instructions.Z80IYBitInstructions[indexZ80IYBitInstructions].Size;
                    type = instructions.Z80IYBitInstructions[indexZ80IYBitInstructions].Type;

                    return (instruction);
                }
            }

            return (instruction);
        }

        #endregion
    }
}
