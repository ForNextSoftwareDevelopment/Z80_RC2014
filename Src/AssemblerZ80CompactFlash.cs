using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms; 

namespace Z80_RC2014
{
    class AssemblerZ80CompactFlash
    {
        #region Members

        // All opcodes and mnemonics
        public readonly Instructions instructions;

        // Segment types
        public enum SEGMENT
        {
            aseg = 0,
            cseg = 1,
            dseg = 2
        }

        // Segment currently active
        SEGMENT segment;

        // Absolute program segment, Code program segment, Data program segment
        UInt16 aseg;
        UInt16 cseg;
        UInt16 dseg;

        // Total RAM of 65536 bytes (0x0000 - 0xFFFF)
        public byte[] RAM;

        // Linenumber for a given byte of the program (Max 0x10000 = 65535 line numbers)
        public int[] RAMprogramLine;

        // Address Symbol Table
        public Dictionary<string, int> addressSymbolTable;

        // Processed program for running second pass
        public string[] programRun;

        // Program listing
        public string[] programView;

        // Line currently processed
        public int lineNumber;

        // Current location of the program (during firstpass and secondpass)
        public int locationCounter;

        // File characteristics
        public string drive;
        public string name;
        public string type;

        // Indicate we have found a program to write to the drive
        public bool driveProgramFound = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor 
        /// </summary>        
        public AssemblerZ80CompactFlash(string[] program)
        {
            this.programRun = program;
            this.programView = new string[program.Length];

            instructions = new Instructions();
        }

        #endregion

        #region Methods (GetByte(s))

        private byte GetByte(string arg, out string result)
        {
            /// Split arguments
            string[] args = arg.Split(new char[] { ' ', '(', ')', '+', '-', '*', '/' });

            // Sort by size, longest string first to avoid partial replacements
            Array.Sort(args, (x, y) => y.Length.CompareTo(x.Length));

            // Replace all symbols from symbol table
            foreach (string str in args)
            {
                foreach (KeyValuePair<string, int> keyValuePair in addressSymbolTable)
                {
                    if (str.ToLower().Trim() == keyValuePair.Key.ToLower().Trim())
                    {
                        arg = arg.Replace(keyValuePair.Key, keyValuePair.Value.ToString());
                    }
                }
            }

            // Process low order byte of argument
            if (arg.ToLower().Contains("low("))
            {
                int start = arg.IndexOf('(') + 1;
                int end = arg.IndexOf(')', start);

                if (end - start < 2)
                {
                    result = "Illegal argument for LOW(arg)";
                    return (0);
                }

                string argLow = arg.Substring(start, end - start);
                argLow = Convert.ToInt32(argLow).ToString("X4").Substring(2, 2);

                arg = Convert.ToInt32(argLow, 16).ToString() + arg.Substring(end + 1, arg.Length - 1 - end).Trim();
            }

            // Process high order byte of argument
            if (arg.ToLower().Contains("high("))
            {
                int start = arg.IndexOf('(') + 1;
                int end = arg.IndexOf(')', start);

                if (end - start < 2)
                {
                    result = "Illegal argument for HIGH(arg)";
                    return (0);
                }

                string argHigh = arg.Substring(start, end - start);
                argHigh = Convert.ToInt32(argHigh).ToString("X4").Substring(0, 2);

                arg = Convert.ToInt32(argHigh, 16).ToString() + arg.Substring(end + 1, arg.Length - 1 - end).Trim();
            }

            // Replace AND with & as token
            arg = arg.ToLower().Replace("and", "&");

            // Replace OR with | as token
            arg = arg.ToLower().Replace("or", "|");

            // Calculate expression
            byte calc = Calculator.CalculateByte(arg, out string res);

            // result string of the expression ("OK" or error message)
            result = res;

            return (calc);
        }

        private UInt16 Get2Bytes(string arg, out string result)
        {
            /// Split arguments
            string[] args = arg.Split(new char[] { ' ', '(', ')', '+', '-', '*', '/' });

            // Sort by size, longest string first to avoid partial replacements
            Array.Sort(args, (x, y) => y.Length.CompareTo(x.Length));

            // Replace all symbols from symbol table
            foreach (string str in args)
            {
                // Replace $ with location counter -1 (position of opcode)
                if (str.Trim() == "$") arg = arg.Replace("$", (locationCounter - 1).ToString());

                foreach (KeyValuePair<string, int> keyValuePair in addressSymbolTable)
                {
                    if (str.ToLower().Trim() == keyValuePair.Key.ToLower().Trim())
                    {
                        arg = arg.Replace(keyValuePair.Key, keyValuePair.Value.ToString());
                    }
                }
            }

            // Replace AND with & as token
            arg = arg.Replace("AND", "&");

            // Replace OR with | as token
            arg = arg.Replace("OR", "|");

            // Calculate expression
            UInt16 calc = Calculator.Calculate2Bytes(arg, out string res);

            // result string of the expression ("OK" or error message)
            result = res;

            return (calc);
        }

        /// <summary>
        /// Convert integer to hexadecimal string representation
        /// </summary>
        /// <param name="n"></param>
        /// <param name="hi"></param>
        /// <param name="lo"></param>
        private void Get2ByteFromInt(int n, out string lo, out string hi)
        {
            string temp = n.ToString("X4");
            hi = temp.Substring(temp.Length - 4, 2);
            lo = temp.Substring(temp.Length - 2, 2);
        }

        #endregion

        #region Methods (Div)

        /// <summary>
        /// Clear the RAM
        /// </summary>
        public void ClearRam()
        {
            for (int i = 0; i < RAM.Length; i++)
            {
                RAM[i] = 0x00;
            }
        }

        #endregion

        #region Methods (FindInstruction)

        /// <summary>
        /// Find the instruction(s) that match the opcode + operands in the program line (Main, Bit, IX , IY and Misc.)
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="opcode"></param>
        /// <param name="alternative"></param>
        /// <param name="operands"></param>
        /// <param name="matchOpcode"></param>
        /// <returns></returns>
        private List<Instruction> FindInstruction(Instruction[] instructions, bool alternative, string opcode, string[] operands, out bool matchOpcode)
        {
            matchOpcode = false;

            List<Instruction> found = new List<Instruction>();

            if (opcode == "-") return (found);

            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Length; indexZ80Instructions++)
            {
                string[] splitZ80Instruction = instructions[indexZ80Instructions].Mnemonic.Split(' ');
                if (alternative) splitZ80Instruction = instructions[indexZ80Instructions].AltMnemonic.Split(' ');
                string opcodeZ80Instruction = splitZ80Instruction[0];

                string temp = "";
                for (int i = 1; i < splitZ80Instruction.Length; i++)
                {
                    temp += splitZ80Instruction[i];
                }
                string[] argumentsZ80Instruction = new string[0];
                if (temp != "") argumentsZ80Instruction = temp.Split(',');

                // Opcode matches
                if (opcode == opcodeZ80Instruction)
                {
                    matchOpcode = true;
                    bool matchOperands = true;

                    // Check number of operands
                    if (operands.Length == argumentsZ80Instruction.Length)
                    {
                        // Check operands
                        for (int indexOperands = 0; indexOperands < operands.Length; indexOperands++)
                        {
                            string arg = argumentsZ80Instruction[indexOperands];
                            string opr = operands[indexOperands].ToLower().Trim();

                            // Check if operand is indexed and the instruction is not indexed (not with cp)
                            if ((opcode != "cp") && opr.StartsWith("(") && !arg.StartsWith("("))
                            {
                                matchOperands = false;
                            }

                            // Check if instruction is indexed and the operand is not indexed
                            if (!opr.StartsWith("(") && arg.StartsWith("("))
                            {
                                matchOperands = false;
                            }

                            // Check if the instruction operand is a direct number
                            if (
                                (arg == "0") ||
                                (arg == "1") ||
                                (arg == "2") ||
                                (arg == "3") ||
                                (arg == "4") ||
                                (arg == "5") ||
                                (arg == "6") ||
                                (arg == "7") ||
                                (arg == "8") ||
                                (arg == "9")
                               )
                            {
                                if (GetByte(arg, out string result1) != GetByte(opr, out string result2))
                                {
                                    matchOperands = false;
                                }

                                if ((result1 != "OK") || (result2 != "OK")) matchOperands = false;
                            }

                            // Check if the instruction operand is a direct number
                            if (
                                (arg == "00h") ||
                                (arg == "08h") ||
                                (arg == "10h") ||
                                (arg == "18h") ||
                                (arg == "20h") ||
                                (arg == "28h") ||
                                (arg == "30h") ||
                                (arg == "38h")
                               )
                            {
                                if (GetByte(arg, out string result1) != GetByte(opr, out string result2))
                                {
                                    matchOperands = false;
                                }

                                if ((result1 != "OK") || (result2 != "OK")) matchOperands = false;
                            }

                            // Check if the instruction operand is a number (immediate, extended or indexed)
                            if (
                                (arg == "n") ||
                                (arg == "nn") ||
                                (arg == "o") ||
                                (arg == "(n)") ||
                                (arg == "(nn)") ||
                                (arg == "(o)")
                               )
                            {
                                if (
                                    (opr == "a") ||
                                    (opr == "af") ||
                                    (opr == "b") ||
                                    (opr == "bc") ||
                                    (opr == "(bc)") ||
                                    (opr == "c") ||
                                    (opr == "(c)") ||
                                    (opr == "d") ||
                                    (opr == "de") ||
                                    (opr == "(de)") ||
                                    (opr == "e") ||
                                    (opr == "f") ||
                                    (opr == "h") ||
                                    (opr == "hl") ||
                                    (opr == "(hl)") ||
                                    (opr == "l") ||
                                    (opr == "ix") ||
                                    (opr == "(ix)") ||
                                    (opr == "ixh") ||
                                    (opr == "ixl") ||
                                    (opr == "iy") ||
                                    (opr == "(iy)") ||
                                    (opr == "iyh") ||
                                    (opr == "iyl") ||
                                    (opr == "pc") ||
                                    (opr == "sp") ||
                                    (opr == "z") ||
                                    (opr == "nz") ||
                                    (opr == "c") ||
                                    (opr == "nc") ||
                                    (opr == "p") ||
                                    (opr == "m") ||
                                    (opr == "pe") ||
                                    (opr == "po") ||
                                    (opr == "i") ||
                                    (opr == "r") ||
                                    opr.StartsWith("(ix+") ||
                                    opr.StartsWith("(ix-") ||
                                    opr.StartsWith("(iy+") ||
                                    opr.StartsWith("(iy-") ||
                                    opr.StartsWith("(ix +") ||
                                    opr.StartsWith("(ix -") ||
                                    opr.StartsWith("(iy +") ||
                                    opr.StartsWith("(iy -")
                                   )
                                {
                                    matchOperands = false;
                                }
                            }

                            // Check if the operand of the instruction is a register or condition
                            if (
                                (arg == "a") ||
                                (arg == "af") ||
                                (arg == "b") ||
                                (arg == "bc") ||
                                (arg == "(bc)") ||
                                (arg == "c") ||
                                (arg == "(c)") ||
                                (arg == "d") ||
                                (arg == "de") ||
                                (arg == "(de)") ||
                                (arg == "e") ||
                                (arg == "f") ||
                                (arg == "h") ||
                                (arg == "hl") ||
                                (arg == "(hl)") ||
                                (arg == "l") ||
                                (arg == "ix") ||
                                (arg == "(ix)") ||
                                (arg == "ixh") ||
                                (arg == "ixl") ||
                                (arg == "iy") ||
                                (arg == "(iy)") ||
                                (arg == "iyh") ||
                                (arg == "iyl") ||
                                (arg == "pc") ||
                                (arg == "sp") ||
                                (arg == "z") ||
                                (arg == "nz") ||
                                (arg == "c") ||
                                (arg == "nc") ||
                                (arg == "p") ||
                                (arg == "m") ||
                                (arg == "pe") ||
                                (arg == "po") ||
                                (arg == "i") ||
                                (arg == "r") ||
                                (arg == "(ix+o)") ||
                                (arg == "(iy+o)")
                               )
                            {
                                if (arg == "(ix+o)")
                                {
                                    if (!opr.StartsWith("(ix+") && !opr.StartsWith("(ix +") && !opr.StartsWith("(ix-") && !opr.StartsWith("(ix -"))
                                    {
                                        matchOperands = false;
                                    }
                                } else if (arg == "(iy+o)")
                                {
                                    if (!opr.StartsWith("(iy+") && !opr.StartsWith("(iy +") && !opr.StartsWith("(iy-") && !opr.StartsWith("(iy -"))
                                    {
                                        matchOperands = false;
                                    }
                                } else if (arg != opr)
                                {
                                    matchOperands = false;
                                }
                            }
                        }
                    } else
                    {
                        matchOperands = false;
                    }

                    if (matchOperands)
                    {
                        // Don't add if it is an undocumented instruction that is a copy of a regular instruction
                        if (!((found.Count == 1) && (instructions[indexZ80Instructions].Mnemonic == found[0].Mnemonic)))
                        {
                            found.Add(instructions[indexZ80Instructions]);
                        }
                    }
                }
            }

            return (found);
        }


        /// <summary>
        /// Find the instruction(s) that match the opcode + operands in the program line (in IXBit instructions)
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="operands"></param>
        /// <param name="matchOpcodeIXbit"></param>
        /// <returns></returns>
        private List<Instruction> FindIXbitInstruction(string opcode, string[] operands, out bool matchOpcodeIXbit)
        {
            matchOpcodeIXbit = false;

            List<Instruction> found = new List<Instruction>();

            if (opcode == "-") return (found);

            for (int indexZ80IXBitInstructions = 0; indexZ80IXBitInstructions < instructions.Z80IXBitInstructions.Length; indexZ80IXBitInstructions++)
            {
                string[] splitZ80IXBitInstruction = instructions.Z80IXBitInstructions[indexZ80IXBitInstructions].Mnemonic.Split(' ');
                string opcodeZ80IXBitInstruction = splitZ80IXBitInstruction[0];

                string temp = "";
                for (int i = 1; i < splitZ80IXBitInstruction.Length; i++)
                {
                    temp += splitZ80IXBitInstruction[i];
                }
                string[] argumentsZ80IXBitInstruction = new string[0];
                if (temp != "") argumentsZ80IXBitInstruction = temp.Split(',');

                // Opcode matches
                if (opcode == opcodeZ80IXBitInstruction)
                {
                    matchOpcodeIXbit = true;
                    bool matchOperands = true;

                    // Check number of operands
                    if (operands.Length == argumentsZ80IXBitInstruction.Length)
                    {
                        // Check operands
                        for (int indexOperands = 0; indexOperands < operands.Length; indexOperands++)
                        {
                            if (argumentsZ80IXBitInstruction[indexOperands] == "(ix+o)")
                            {
                                if (!operands[indexOperands].ToLower().Trim().StartsWith("(ix+") && !operands[indexOperands].ToLower().Trim().StartsWith("(ix-"))
                                {
                                    matchOperands = false;
                                }
                            } else if (argumentsZ80IXBitInstruction[indexOperands] != operands[indexOperands].ToLower().Trim())
                            {
                                matchOperands = false;
                            }
                        }
                    }

                    if (matchOperands)
                    {
                        found.Add(instructions.Z80IXBitInstructions[indexZ80IXBitInstructions]);
                    }
                }
            }

            return (found);
        }

        /// <summary>
        /// Find the instruction(s) that match the opcode + operands in the program line (in IXBit instructions)
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="operands"></param>
        /// <param name="matchOpcodeIXbit"></param>
        /// <returns></returns>
        private List<Instruction> FindIYbitInstruction(string opcode, string[] operands, out bool matchOpcodeIYbit)
        {
            matchOpcodeIYbit = false;

            List<Instruction> found = new List<Instruction>();

            if (opcode == "-") return (found);

            for (int indexZ80IYBitInstructions = 0; indexZ80IYBitInstructions < instructions.Z80IYBitInstructions.Length; indexZ80IYBitInstructions++)
            {
                string[] splitZ80IYBitInstruction = instructions.Z80IYBitInstructions[indexZ80IYBitInstructions].Mnemonic.Split(' ');
                string opcodeZ80IYBitInstruction = splitZ80IYBitInstruction[0];

                string temp = "";
                for (int i = 1; i < splitZ80IYBitInstruction.Length; i++)
                {
                    temp += splitZ80IYBitInstruction[i];
                }
                string[] argumentsZ80IYBitInstruction = new string[0];
                if (temp != "") argumentsZ80IYBitInstruction = temp.Split(',');

                // Opcode matches
                if (opcode == opcodeZ80IYBitInstruction)
                {
                    matchOpcodeIYbit = true;
                    bool matchOperands = true;

                    // Check number of operands
                    if (operands.Length == argumentsZ80IYBitInstruction.Length)
                    {
                        // Check operands
                        for (int indexOperands = 0; indexOperands < operands.Length; indexOperands++)
                        {
                            if (argumentsZ80IYBitInstruction[indexOperands] == "(iy+o)")
                            {
                                if (!operands[indexOperands].ToLower().Trim().StartsWith("(iy+") && !operands[indexOperands].ToLower().Trim().StartsWith("(iy-"))
                                {
                                    matchOperands = false;
                                }
                            } else if (argumentsZ80IYBitInstruction[indexOperands] != operands[indexOperands].ToLower().Trim())
                            {
                                matchOperands = false;
                            }
                        }
                    }

                    if (matchOperands)
                    {
                        found.Add(instructions.Z80IYBitInstructions[indexZ80IYBitInstructions]);
                    }
                }
            }

            return (found);
        }

        #endregion

        #region Methods (FirstPass)

        /// <summary>
        /// First pass through the code, remove labels, check etc.
        /// </summary>
        /// <returns></returns>
        public string FirstPass()
        {
            // Segment currently active
            segment = SEGMENT.aseg;

            // Absolute program segment, Code program segment, Data program segment
            aseg = 0x0000;
            cseg = 0x0000;
            dseg = 0x0000;

            // Total RAM of 65536 bytes (0x0000 - 0xFFFF)
            RAM = new byte[0x10000];

            // Linenumber for a given byte of the program (Max 0x10000 = 65535 line numbers)
            RAMprogramLine = new int[0x10000];

            for (int i = 0; i < RAMprogramLine.Length; i++)
            {
                RAMprogramLine[i] = -1;
            }

            ClearRam();

            // Address Symbol Table
            addressSymbolTable = new Dictionary<string, int>();

            // File characteristics
            drive = null;
            name = null;
            type = null;

            // locationCounter is a temporary variable to traverse program for first pass, all programs start at 0x0100
            locationCounter = 0x0100;

            // Opcode in the line
            string opcode;

            // Operand(s) for the opcode 
            string[] operands;              

            char[] delimiters = new[] { ',' };

            // Process all lines
            for (lineNumber = 0; lineNumber < programRun.Length; lineNumber++)       
            {
                // Copy line of code to process and clear original line to rebuild
                string line = programRun[lineNumber];
                programRun[lineNumber] = "";
                programView[lineNumber] = "";

                opcode = null;
                operands = null;
                int InstructionStart = locationCounter;

                try
                {
                    // Replace all tabs with spaces
                    line = line.Replace('\t', ' ');

                    // Remove leading or trailing spaces
                    line = line.Trim();
                    if (line == "") continue;

                    // if a comment is found, remove
                    int start_of_comment_pos = line.IndexOf(';');
                    if (start_of_comment_pos != -1)            
                    {
                        // Check if really a comment (; could be in a string or char array)
                        int num_quotes = 0;
                        for (int i = 0; i < start_of_comment_pos; i++)
                        {
                            if ((line[i] == '\'') || (line[i] == '\"')) num_quotes++;
                        }

                        if ((num_quotes % 2) == 0)
                        {
                            line = line.Remove(line.IndexOf(';')).Trim();
                            if (line == "") continue;
                        }
                    }

                    // Replace single characters (in between single quotes) with HEX value
                    bool found;
                    do
                    {
                        found = false;
                        int startQuote = line.IndexOf('\'');
                        int endQuote = 0;
                        if (startQuote < line.Length - 2) endQuote = line.IndexOf('\'', startQuote + 1);
                        if ((startQuote != -1) && (endQuote == startQuote + 2))
                        {
                            found = true;
                            char ch = line[startQuote + 1];
                            line = line.Replace("'" + ch + "'", ((int)ch).ToString("X2") + "H");
                        }
                    } while (found);
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:Quotes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                // Check for equ directive 
                int equ_pos = line.ToLower().IndexOf("equ");
                if (equ_pos > 0)
                {
                    string label = line.Substring(0, equ_pos-1).Trim().TrimEnd(':');

                    if (addressSymbolTable.ContainsKey(label))
                    {
                        return ("Label already used at line " + (lineNumber + 1));
                    }

                    if (equ_pos < line.Length - 4)
                    {
                        string val = line.Substring(equ_pos + 3).Trim();

                        // Replace $ with location counter
                        if (val.Trim() == "$") val = val.Replace("$", locationCounter.ToString());

                        int calc = Get2Bytes(val, out string result);
                        if (result != "OK")
                        {
                            return ("Invalid operand for EQU (" + result + ") at line " + (lineNumber + 1));
                        }

                        // ADD the label/value
                        if (label != "")
                        {
                            addressSymbolTable.Add(label, calc);
                        } else
                        {
                            return ("Syntax: [LABEL] equ [VALUE] at line " + (lineNumber + 1));
                        }

                        // Next line
                        continue;
                    } else
                    {
                        return ("Syntax: [LABEL] equ [VALUE] at line " + (lineNumber + 1));
                    }
                }

                // Check for/get a label
                if (line.IndexOf(':') != -1)
                {
                    try
                    {
                        int end_of_label_pos = line.IndexOf(':');

                        // Check if really a LABEL (: could be in a string or char array)
                        int num_quotes = 0;
                        for (int i = 0; i < end_of_label_pos; i++)
                        {
                            if ((line[i] == '\'') || (line[i] == '\"')) num_quotes++;
                        }

                        if ((num_quotes % 2) == 0)
                        {
                            string label = line.Substring(0, end_of_label_pos).Trim();

                            // Check for spaces in label
                            if (label.Contains(" "))
                            {
                                return ("label '" + label + "' contains spaces at line " + (lineNumber + 1));
                            }

                            if (addressSymbolTable.ContainsKey(label))
                            {
                                return ("Label already used at line " + (lineNumber + 1));
                            }

                            if (line.Length > end_of_label_pos + 1)
                            {
                                line = line.Substring(end_of_label_pos + 1, line.Length - end_of_label_pos - 1).Trim();

                                // ADD the label/value
                                addressSymbolTable.Add(label, locationCounter);
                            } else
                            {
                                line = "";

                                // ADD the label/value
                                addressSymbolTable.Add(label, locationCounter);

                                // Next line
                                continue;
                            }
                        }
                    } catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "FirstPass:Label", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                    }
                }

                try
                {
                    // Check for opcode (directive) db, replace chars/strings with hex values
                    if (line.ToLower().StartsWith("db") || line.ToLower().StartsWith(".db") || line.ToLower().StartsWith("defb") || line.ToLower().StartsWith(".text"))
                    {
                        string lineDB = line;

                        // Create new line, copy opcode
                        opcode = lineDB.Substring(0, line.IndexOf(" "));
                        line = opcode;
                        for (int i = line.Length; i < 6; i++)
                        {
                            line += " ";
                        }

                        // Traverse the line from left to right
                        int index = opcode.Length;
                        bool stringProcessing = false;
                        while (index < lineDB.Length)
                        {
                            if ((lineDB[index] == '\"') || (lineDB[index] == '\''))
                            {
                                // Char or string found
                                stringProcessing = true;
                                char endChar = lineDB[index];

                                if (index < lineDB.Length - 1) index++;
                                char processChar = lineDB[index];

                                // Replace until end of string found
                                while ((index < lineDB.Length) && (processChar != endChar))
                                {
                                    processChar = lineDB[index];
                                    if ((processChar != endChar))
                                    {
                                        line += ((int)processChar).ToString("X2") + "H";
                                        line += ", ";
                                    }
                                    index++;
                                }

                                stringProcessing = false;
                            } else if ((lineDB[index] == ',') || (lineDB[index] == ' ') || (lineDB[index] == '\t') || (lineDB[index] == '\r') || (lineDB[index] == '\n'))
                            {
                                // Skip these chars
                                index++;
                            } else
                            {
                                // Just copy up to next comma or end of line
                                while ((index < lineDB.Length) && (lineDB[index] != ','))
                                {
                                    line += lineDB[index];
                                    index++;
                                }

                                line += ", ";
                            }
                        }

                        // Remove last comma and space
                        if (line.Length > 2)
                        {
                            line = line.Substring(0, line.Length - 2);
                        } else
                        {
                            return (opcode + " directive has an error at line " + (lineNumber + 1));
                        }

                        // Give warning if an unclosed string has been found
                        if (stringProcessing)
                        {
                            return (opcode + " directive has an unclosed string at line " + (lineNumber + 1));
                        }
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:db", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                // Get the opcode and operands
                try
                {
                    int end_of_opcode_pos = line.IndexOf(' ');
                    if ((end_of_opcode_pos == -1) && (line.Length != 0)) end_of_opcode_pos = line.Length;

                    if (end_of_opcode_pos <= 0)
                    {
                        // Next line
                        continue;
                    }

                    opcode = line.Substring(0, end_of_opcode_pos).ToLower().Trim();

                    // Split the line and store the strings formed in an array
                    operands = line.Substring(end_of_opcode_pos).Trim().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    // Rebuild the line
                    line = opcode;
                    while (line.Length < 6) line += " ";
                    for (int i=0; i<operands.Length; i++)
                    {
                        operands[i] = operands[i].Trim();
                        if (i != 0) line += ",";
                        line += operands[i];
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:Opcode", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                try
                {
                    // Check for opcode (directive) aseg
                    if (opcode.Equals("aseg"))
                    {
                        // Set current segment
                        segment = SEGMENT.aseg;

                        // Set locationcounter
                        locationCounter = aseg;

                        // Copy to program for second pass
                        programRun[lineNumber] = opcode;

                        // Copy to programView for examining
                        programView[lineNumber] = opcode;

                        // Next line
                        continue;
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:aseg", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                try
                {
                    // Check for opcode (directive) cseg
                    if (opcode.Equals("cseg"))
                    {
                        // Set current segment
                        segment = SEGMENT.cseg;

                        // Set locationcounter
                        locationCounter = cseg;

                        // Copy to program for second pass
                        programRun[lineNumber] = opcode;

                        // Copy to programView for examining
                        programView[lineNumber] = opcode;

                        // Next line
                        continue;
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:cseg", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                try
                {
                    // Check for opcode (directive) dseg
                    if (opcode.Equals("dseg"))
                    {
                        // Set current segment
                        segment = SEGMENT.dseg;

                        // Set locationcounter
                        locationCounter = dseg;

                        // Copy to program for second pass
                        programRun[lineNumber] = opcode;

                        // Copy to programView for examining
                        programView[lineNumber] = opcode;

                        // Next line
                        continue;
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:dseg", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                try
                {
                    // Check for opcode (directive) org
                    if (opcode.Equals("org") || opcode.Equals(".org"))
                    {
                        // Line must have arguments after the opcode
                        if ((operands.Length == 0) || (operands.Length > 2))
                        {
                            return ("Org directive must have 1 or 2 arguments (Address or Disk and FileName) at line " + (lineNumber + 1));
                        }

                        // Boot code
                        if (operands.Length == 1)
                        {
                            // If valid address then store in locationCounter
                            int calc = Get2Bytes(operands[0], out string result);
                            if (result == "OK")
                            {
                                locationCounter = calc;
                            } else
                            {
                                return ("Invalid operand for " + opcode + "(" + result + ") at line " + (lineNumber + 1));
                            }

                            // Next line
                            continue;
                        }

                        // Program on virtual disk
                        if (operands.Length == 2)
                        {
                            if (driveProgramFound)
                            {
                                return ("Second drive program found at line " + (lineNumber + 1));
                            }

                            // Check drive
                            if ((operands[0].Trim().ToUpper() != "A") &&
                                (operands[0].Trim().ToUpper() != "B") &&
                                (operands[0].Trim().ToUpper() != "C") &&
                                (operands[0].Trim().ToUpper() != "D") &&
                                (operands[0].Trim().ToUpper() != "E") &&
                                (operands[0].Trim().ToUpper() != "F") &&
                                (operands[0].Trim().ToUpper() != "G") &&
                                (operands[0].Trim().ToUpper() != "H") &&
                                (operands[0].Trim().ToUpper() != "I") &&
                                (operands[0].Trim().ToUpper() != "J") &&
                                (operands[0].Trim().ToUpper() != "K") &&
                                (operands[0].Trim().ToUpper() != "L") &&
                                (operands[0].Trim().ToUpper() != "M") &&
                                (operands[0].Trim().ToUpper() != "N") &&
                                (operands[0].Trim().ToUpper() != "O") &&
                                (operands[0].Trim().ToUpper() != "P"))
                            {
                                return ("Org directive must have a disk name as first operand at line " + (lineNumber + 1));
                            }

                            // Check filename
                            string[] name_type = operands[1].Trim().Split('.');
                            if (name_type.Length != 2)
                            {
                                return ("Org directive must have a name/type (e.g name.typ) as second operand at line " + (lineNumber + 1));
                            }

                            if (name_type[1].Length != 3)
                            {
                                return ("Org directive must have a name/type with type as 3 letters (e.g name.typ) as second operand at line " + (lineNumber + 1));
                            }

                            driveProgramFound = true;

                            drive = operands[0].Trim();
                            name = name_type[0].Trim();
                            type = name_type[1].Trim();

                            // Copy to program for second pass
                            programRun[lineNumber] = opcode + " " + operands[0] + operands[1];

                            // Copy to programView for examining
                            programView[lineNumber] = opcode + " " + operands[0];

                            // All programs start at 0x0100
                            locationCounter = 0x0100;

                            // Next line
                            continue;
                        }
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:org", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                // Count the operand(s) for db, dw, ds
                try
                {
                    if (opcode.Equals("db") || opcode.Equals(".db") || opcode.Equals("defb") || opcode.Equals(".text"))
                    {
                        if (operands.Length == 0)
                        {
                            return (opcode + " directive has too few operands at line " + (lineNumber + 1));
                        }

                        // Loop for traversing after db
                        for (int pos = 0; pos < operands.Length; pos++)
                        {
                            // get to next location by skipping location for byte
                            locationCounter++;
                        }

                        // Copy to program for second pass
                        programRun[lineNumber] = line;

                        // Copy to programView for examining
                        programView[lineNumber] = InstructionStart.ToString("X4") + ": " + line;

                        // Next line
                        continue;
                    }

                    if (opcode.Equals("dw") || opcode.Equals(".dw") || opcode.Equals("defw"))
                    {
                        if (operands.Length == 0)
                        {
                            return (opcode + " directive has too few operands at line " + (lineNumber + 1));
                        }

                        for (int pos = 0; pos < operands.Length; pos++)
                        {
                            // Get to next location by skipping location for 2 bytes
                            locationCounter += 2;
                        }

                        // Copy to program for second pass
                        programRun[lineNumber] = line;

                        // Copy to programView for examining
                        programView[lineNumber] = InstructionStart.ToString("X4") + ": " + line;

                        // Next line
                        continue;
                    }

                    if (opcode.Equals("ds") || opcode.Equals(".ds") || opcode.Equals("defs"))
                    {
                        if (operands.Length == 0)
                        {
                            return (opcode + " directive has too few operands at line " + (lineNumber + 1));
                        }

                        // If valid address then store in locationCounter
                        int calc = Get2Bytes(operands[0], out string result);
                        if (result == "OK")
                        {
                            locationCounter += calc;
                        } else
                        {
                            return ("Invalid operand for " + opcode + "(" + result + ") at line " + (lineNumber + 1));
                        }

                        // Copy to program for second pass
                        programRun[lineNumber] = line;

                        // Copy to programView for examining
                        programView[lineNumber] = InstructionStart.ToString("X4") + ": " + line;

                        // Next line
                        continue;
                    }

                    // End of program
                    if ((opcode == "end") || (opcode == ".end"))
                    {
                        programRun[lineNumber] = line;
                        programView[lineNumber] = line;
                        return ("OK");
                    }

                    // List of matching instructions
                    List<Instruction> found;

                    // Check opcode/operands for Z80 instructions (Main)
                    bool matchOpcodeMain = false;
                    found = FindInstruction(instructions.Z80MainInstructions, false, opcode, operands, out matchOpcodeMain);

                    // Check opcode/operands for Z80 instructions (Bit)
                    bool matchOpcodeBit = false;
                    if (found.Count == 0)
                    {
                        found = FindInstruction(instructions.Z80BitInstructions, false, opcode, operands, out matchOpcodeBit);
                    }

                    // Check opcode/operands for Z80 instructions (IX)
                    bool matchOpcodeIX = false;
                    if (found.Count == 0)
                    {
                        found = FindInstruction(instructions.Z80IXInstructions, false, opcode, operands, out matchOpcodeIX);
                    }

                    // Check opcode/operands for Z80 instructions (IY)
                    bool matchOpcodeIY = false;
                    if (found.Count == 0)
                    {
                        found = FindInstruction(instructions.Z80IYInstructions, false, opcode, operands, out matchOpcodeIY);
                    }

                    // Check opcode/operands for Z80 instructions (Misc.)
                    bool matchOpcodeMisc = false;
                    if (found.Count == 0)
                    {
                        found = FindInstruction(instructions.Z80MiscInstructions, false, opcode, operands, out matchOpcodeMisc);
                    }

                    // Check opcode/operands for Z80 instructions (IXbit)
                    bool matchOpcodeIXbit = false;
                    if (found.Count == 0)
                    {
                        found = FindIXbitInstruction(opcode, operands, out matchOpcodeIXbit);
                    }

                    // Check opcode/operands for Z80 instructions (IYbit)
                    bool matchOpcodeIYbit = false;
                    if (found.Count == 0)
                    {
                        found = FindIYbitInstruction(opcode, operands, out matchOpcodeIYbit);
                    }

                    string args = "";
                    foreach (string arg in operands)
                    {
                        args += arg + ", ";
                    }
                    args = args.Trim();
                    args = args.TrimEnd(',');

                    if (found.Count > 1)
                    {
                        // Just for debugging, should not happen
                        string message = "Multiple solutions for '" + opcode + " " + args + "':\r\n\r\n";
                        foreach(Instruction instruction in found)
                        {
                            message += instruction.Mnemonic + "\r\n";
                        }
                        message += "\r\nAt line " + (lineNumber + 1);

                        return (message);
                    }

                    // No match, try alternative opcodes
                    bool matchOpcodeAlternative = false;
                    if (found.Count == 0)
                    {
                        // Check opcode/operands for Z80 instructions (Main)
                        found = FindInstruction(instructions.Z80MainInstructions, true, opcode, operands, out matchOpcodeAlternative);

                        // Check opcode/operands for Z80 instructions (Bit)
                        if (found.Count == 0)
                        {
                            found = FindInstruction(instructions.Z80BitInstructions, true, opcode, operands, out matchOpcodeAlternative);
                        }

                        // Check opcode/operands for Z80 instructions (IX)
                        if (found.Count == 0)
                        {
                            found = FindInstruction(instructions.Z80IXInstructions, true, opcode, operands, out matchOpcodeAlternative);
                        }

                        // Check opcode/operands for Z80 instructions (IY)
                        if (found.Count == 0)
                        {
                            found = FindInstruction(instructions.Z80IYInstructions, true, opcode, operands, out matchOpcodeAlternative);
                        }

                        // Check opcode/operands for Z80 instructions (Misc.)
                        if (found.Count == 0)
                        {
                            found = FindInstruction(instructions.Z80MiscInstructions, true, opcode, operands, out matchOpcodeAlternative);
                        }
                    }

                    // No match
                    if (found.Count == 0)
                    {
                        string message = "";

                        if (matchOpcodeMain || matchOpcodeBit || matchOpcodeIX || matchOpcodeIY || matchOpcodeMisc || matchOpcodeIXbit || matchOpcodeIYbit || matchOpcodeAlternative)
                        {
                            if (matchOpcodeMain)        message += "Opcode '" + opcode + "' found in Main instructions.\r\n";
                            if (matchOpcodeBit)         message += "Opcode '" + opcode + "' found in Bit instructions.\r\n";
                            if (matchOpcodeIX)          message += "Opcode '" + opcode + "' found in IX instructions.\r\n";
                            if (matchOpcodeIY)          message += "Opcode '" + opcode + "' found in IY instructions.\r\n";
                            if (matchOpcodeMisc)        message += "Opcode '" + opcode + "' found in Misc instructions.\r\n";
                            if (matchOpcodeIXbit)       message += "Opcode '" + opcode + "' found in BitIX instructions.\r\n";
                            if (matchOpcodeIYbit)       message += "Opcode '" + opcode + "' found in BitIY instructions.\r\n";
                            if (matchOpcodeAlternative) message += "Opcode '" + opcode + "' found in Alternative instructions.\r\n";

                            return (message + "\r\nError in arguments: '" + args + "'\r\n\r\nAt line " + (lineNumber + 1));
                        } 

                        return ("Unknown opcode '" + opcode + "' at line " + (lineNumber + 1));
                    }

                    // Update locationcounter
                    locationCounter += found[0].Size;

                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "FirstPass:OPCODE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                // Update current segment
                if (segment == SEGMENT.aseg) aseg = (UInt16)locationCounter;
                if (segment == SEGMENT.cseg) cseg = (UInt16)locationCounter;
                if (segment == SEGMENT.dseg) dseg = (UInt16)locationCounter;

                //  Copy the edited program (without labels and equ) to new array of strings
                //  The new program array of strings will be used in second pass
                programRun[lineNumber] = line;

                // Copy to programView for examining
                programView[lineNumber] = InstructionStart.ToString("X4") + ": " + line;
            }

            return ("OK");
        }

        #endregion

        #region Methods (SecondPass)

        /// <summary>
        /// Second pass through the code, convert instructions etc.
        /// </summary>
        /// <returns></returns>
        public string SecondPass(int ln)
        {
            // Using locationCounter to traverse the location of RAM during second pass
            locationCounter = 0x0100; 

            // Opcode in the line
            string opcode;

            // Operand(s) for the opcode 
            string[] operands;

            // Split operands by these delimeter(s)
            char[] delimiters = new[] {','};

            // Reset segments
            aseg = 0;
            cseg = 0;
            dseg = 0;

            // Temporary variables
            byte calcByte;
            UInt16 calcShort;
            int k;
            string str;

            for (lineNumber = ln; lineNumber < programRun.Length; lineNumber ++)
            {
                int locationCounterInstructionStart = locationCounter;

                try
                {
                    // Empty line
                    if ((programRun[lineNumber] == null) || (programRun[lineNumber] == ""))
                    {
                        // If line is empty, there is no need to check
                        continue;
                    }

                    int end_of_opcode_pos = programRun[lineNumber].IndexOf(' ');
                    if ((end_of_opcode_pos == -1) && (programRun[lineNumber].Length != 0)) end_of_opcode_pos = programRun[lineNumber].Length;

                    if (end_of_opcode_pos <= 0)
                    {
                        // Next line
                        continue;
                    }

                    opcode = programRun[lineNumber].Substring(0, end_of_opcode_pos).Trim();

                    // Split the line and store the strings formed in array
                    operands = programRun[lineNumber].Substring(end_of_opcode_pos).Trim().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    // Remove spaces and tabs from operands
                    for (int j=0; j<operands.Length; j++)
                    {
                        operands[j] = operands[j].Trim();
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "SECONDPASS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }

                try
                {
                    // Check instruction
                    switch (opcode)
                    {
                        case "aseg":                                                                                    // aseg

                            segment = SEGMENT.aseg;
                            locationCounter = aseg;
                            break;

                        case "cseg":                                                                                    // cseg

                            segment = SEGMENT.cseg;
                            locationCounter = cseg;
                            break;

                        case "dseg":                                                                                    // dseg

                            segment = SEGMENT.dseg;
                            locationCounter = dseg;
                            break;

                        case "org":                                                                                     // org
                        case ".org":                                                                                     
                            break;

                        case "end":                                                                                     // end
                        case ".end":

                            locationCounter = 0x0100;
                            return ("OK");

                        case "db":                                                                                      // db
                        case "defb":
                        case ".db":
                        case ".text":

                            for (k = 0; k < operands.Length; k++)
                            {
                                // Extract all db operands
                                calcByte = GetByte(operands[k], out string resultdb);
                                if (resultdb == "OK")
                                {
                                    RAMprogramLine[locationCounter] = lineNumber;
                                    RAM[locationCounter++] = calcByte;
                                } else
                                {
                                    return ("Invalid operand for " + opcode + ": " + resultdb + " at line " + (lineNumber + 1));
                                }
                            }
                            break;

                        case "dw":                                                                                      // dw
                        case "defw":
                        case ".dw":

                            for (k = 0; k < operands.Length; k++)
                            {
                                // Extract all dw operands
                                calcShort = Get2Bytes(operands[k], out string resultdefw);
                                if (resultdefw == "OK")
                                {
                                    str = calcShort.ToString("X4");
                                    RAMprogramLine[locationCounter] = lineNumber;
                                    RAM[locationCounter++] = Convert.ToByte(str.Substring(2, 2), 16);
                                    RAMprogramLine[locationCounter] = lineNumber;
                                    RAM[locationCounter++] = Convert.ToByte(str.Substring(0, 2), 16);
                                } else
                                {
                                    return ("Invalid operand for " + opcode + ": " + resultdefw + " at line " + (lineNumber + 1));
                                }
                            }
                            break;

                        case "ds":                                                                                      // ds
                        case "defs":
                        case ".ds":

                            calcShort = Get2Bytes(operands[0], out string resultds);
                            if (resultds == "OK")
                            {
                                while (calcShort != 0)
                                {
                                    // We don't have to initialize operands for ds, just reserve space for them
                                    RAMprogramLine[locationCounter] = lineNumber;
                                    locationCounter++;
                                    calcShort--;
                                }
                            } else
                            {
                                return ("Invalid operand for " + opcode + ": " + resultds + " at line " + (lineNumber + 1));
                            }
                            break;

                        default:

                            // List of matching instructions
                            List<Instruction> found;

                            // Check opcode/operands for Z80 instructions (Main)
                            bool matchOpcodeMain = false;
                            found = FindInstruction(instructions.Z80MainInstructions, false, opcode, operands, out matchOpcodeMain);

                            // Check opcode/operands for Z80 instructions (Bit)
                            bool matchOpcodeBit = false;
                            if (found.Count == 0)
                            {
                                found = FindInstruction(instructions.Z80BitInstructions, false, opcode, operands, out matchOpcodeBit);
                            }

                            // Check opcode/operands for Z80 instructions (IX)
                            bool matchOpcodeIX = false;
                            if (found.Count == 0)
                            {
                                found = FindInstruction(instructions.Z80IXInstructions, false, opcode, operands, out matchOpcodeIX);
                            }

                            // Check opcode/operands for Z80 instructions (IY)
                            bool matchOpcodeIY = false;
                            if (found.Count == 0)
                            {
                                found = FindInstruction(instructions.Z80IYInstructions, false, opcode, operands, out matchOpcodeIY);
                            }

                            // Check opcode/operands for Z80 instructions (Misc.)
                            bool matchOpcodeMisc = false;
                            if (found.Count == 0)
                            {
                                found = FindInstruction(instructions.Z80MiscInstructions, false, opcode, operands, out matchOpcodeMisc);
                            }

                            // Check opcode/operands for Z80 instructions (IXbit)
                            bool matchOpcodeIXbit = false;
                            bool foundIXbit = false;
                            if (found.Count == 0)
                            {
                                found = FindIXbitInstruction(opcode, operands, out matchOpcodeIXbit);
                                if (found.Count == 1) foundIXbit = true;
                            }

                            // Check opcode/operands for Z80 instructions (IYbit)
                            bool matchOpcodeIYbit = false;
                            bool foundIYbit = false;
                            if (found.Count == 0)
                            {
                                found = FindIYbitInstruction(opcode, operands, out matchOpcodeIYbit);
                                if (found.Count == 1) foundIYbit = true;
                            }

                            string args = "";
                            foreach (string arg in operands)
                            {
                                args += arg + ", ";
                            }
                            args = args.Trim();
                            args = args.TrimEnd(',');

                            // Just a double check (already checked in firstpass)
                            if (found.Count > 1)
                            {
                                // Just for debugging, should not happen
                                string message = "Multiple solutions for '" + opcode + " " + args + "':\r\n\r\n";
                                foreach (Instruction find in found)
                                {
                                    message += find.Mnemonic + "\r\n";
                                }
                                message += "\r\nAt line " + (lineNumber + 1);

                                return (message);
                            }

                            // No match, try alternative opcodes
                            bool matchOpcodeAlternative = false;
                            if (found.Count == 0)
                            {
                                // Check opcode/operands for Z80 instructions (Main)
                                found = FindInstruction(instructions.Z80MainInstructions, true, opcode, operands, out matchOpcodeAlternative);

                                // Check opcode/operands for Z80 instructions (Bit)
                                if (found.Count == 0)
                                {
                                    found = FindInstruction(instructions.Z80BitInstructions, true, opcode, operands, out matchOpcodeAlternative);
                                }

                                // Check opcode/operands for Z80 instructions (IX)
                                if (found.Count == 0)
                                {
                                    found = FindInstruction(instructions.Z80IXInstructions, true, opcode, operands, out matchOpcodeAlternative);
                                }

                                // Check opcode/operands for Z80 instructions (IY)
                                if (found.Count == 0)
                                {
                                    found = FindInstruction(instructions.Z80IYInstructions, true, opcode, operands, out matchOpcodeAlternative);
                                }

                                // Check opcode/operands for Z80 instructions (Misc.)
                                if (found.Count == 0)
                                {
                                    found = FindInstruction(instructions.Z80MiscInstructions, true, opcode, operands, out matchOpcodeAlternative);
                                }
                            }

                            // Just a double check (already checked in firstpass)
                            if (found.Count == 0)
                            {
                                string message = "";

                                if (matchOpcodeMain || matchOpcodeBit || matchOpcodeIX || matchOpcodeIY || matchOpcodeMisc || matchOpcodeIXbit || matchOpcodeIYbit || matchOpcodeAlternative)
                                {
                                    if (matchOpcodeMain)        message += "Opcode '" + opcode + "' found in Main instructions.\r\n";
                                    if (matchOpcodeBit)         message += "Opcode '" + opcode + "' found in Bit instructions.\r\n";
                                    if (matchOpcodeIX)          message += "Opcode '" + opcode + "' found in IX instructions.\r\n";
                                    if (matchOpcodeIY)          message += "Opcode '" + opcode + "' found in IY instructions.\r\n";
                                    if (matchOpcodeMisc)        message += "Opcode '" + opcode + "' found in Misc instructions.\r\n";
                                    if (matchOpcodeIXbit)       message += "Opcode '" + opcode + "' found in BitIX instructions.\r\n";
                                    if (matchOpcodeIYbit)       message += "Opcode '" + opcode + "' found in BitIY instructions.\r\n";
                                    if (matchOpcodeAlternative) message += "Opcode '" + opcode + "' found in Alternative instructions.\r\n";

                                    return (message + "\r\nError in arguments: '" + args + "'\r\n\r\nAt line " + (lineNumber + 1));
                                }

                                return ("Unknown opcode '" + opcode + "' at line " + (lineNumber + 1));
                            }

                            // Instruction to handle
                            Instruction instruction = found[0];

                            RAMprogramLine[locationCounter] = lineNumber;

                            if (foundIXbit)
                            {
                                // IXBit: 2 opcodes, then operand, then third opcode  
                                RAM[locationCounter++] = 0xDD;
                                RAM[locationCounter++] = 0xCB;
                            } else
                            if (foundIYbit)
                            {
                                // IYBit: 2 opcodes, then operand, then third opcode  
                                RAM[locationCounter++] = 0xFD;
                                RAM[locationCounter++] = 0xCB;
                            } else
                            {
                                // Regular instruction: 1 or 2 opcodes, then operand 
                                if (instruction.Opcode <= 0xFF)
                                {
                                    RAM[locationCounter++] = Convert.ToByte(instruction.Opcode);
                                } else
                                {
                                    RAM[locationCounter++] = Convert.ToByte(instruction.Opcode / 0x100);
                                    RAM[locationCounter++] = Convert.ToByte(instruction.Opcode % 0x100);
                                }
                            }

                            string[] splitZ80Instruction = instruction.Mnemonic.Split(' ');
                            if (matchOpcodeAlternative) splitZ80Instruction = instruction.AltMnemonic.Split(' ');

                            string opcodeZ80Instruction = splitZ80Instruction[0];

                            string temp = "";
                            for (int i = 1; i < splitZ80Instruction.Length; i++)
                            {
                                temp += splitZ80Instruction[i];
                            }
                            string[] argumentsZ80Instruction = new string[0];
                            if (temp != "") argumentsZ80Instruction = temp.Split(',');

                            // Just a double check
                            if (argumentsZ80Instruction.Length != operands.Length)
                            {
                                return ("Arguments mismatch for opcode '" + opcode + "'\r\nAt line " + (lineNumber + 1));
                            }

                            for (int i=0; i<argumentsZ80Instruction.Length; i++)
                            {
                                if ((argumentsZ80Instruction[i] != operands[i]) || (operands[i] == "n") || (operands[i] == "nn"))
                                {
                                    switch (argumentsZ80Instruction[i])
                                    {
                                        case "n":
                                        case "(n)":
                                            calcByte = GetByte(operands[i], out string nResult);
                                            if (nResult == "OK")
                                            {
                                                if ((instruction.Opcode == 0xDDCB) || (instruction.Opcode == 0xFDCB))
                                                {
                                                    // Adjust the n byte for these instructions
                                                    calcByte = (byte)(calcByte << 3); 
                                                    calcByte = (byte)((calcByte & 0b00111000) | 0b01000110); 
                                                }
                                                
                                                RAMprogramLine[locationCounter] = lineNumber;
                                                RAM[locationCounter++] = calcByte;
                                            } else
                                            {
                                                return ("Invalid operand for " + opcode + ":\r\n" + nResult + "\r\nAt line " + (lineNumber + 1));
                                            }
                                            break;

                                        case "nn":
                                        case "(nn)":
                                            calcShort = Get2Bytes(operands[i], out string nnResult);
                                            if (nnResult == "OK")
                                            { 
                                                str = calcShort.ToString("X4");
                                                RAMprogramLine[locationCounter] = lineNumber;
                                                RAM[locationCounter++] = Convert.ToByte(str.Substring(2, 2), 16);
                                                RAMprogramLine[locationCounter] = lineNumber;
                                                RAM[locationCounter++] = Convert.ToByte(str.Substring(0, 2), 16);
                                            } else
                                            {
                                                return ("Invalid operand for " + opcode + ":\r\n" + nnResult + "\r\nAt line " + (lineNumber + 1));
                                            }
                                            break;

                                        case "o":
                                            if ((opcode == "jr") || (opcode == "djnz"))
                                            {
                                                // Relative jump, so calulate offset    
                                                calcShort = Get2Bytes(operands[i], out string oResult);
                                                if (oResult == "OK")
                                                {
                                                    // Check if operand is a label or direct value
                                                    bool direct = true;
                                                    if (operands[i].Contains("$")) direct = false;
                                                    foreach (KeyValuePair<string, int> keyValuePair in addressSymbolTable)
                                                    {
                                                        if (operands[i].ToLower().Trim() == keyValuePair.Key.ToLower().Trim()) direct = false;
                                                    }

                                                    int offset;
                                                    if (direct)
                                                    {
                                                        offset = calcShort < 0x80 ? calcShort : calcShort - 256;
                                                    } else
                                                    {
                                                        offset = calcShort - locationCounter - 1;
                                                    }

                                                    if (offset > 127) return ("Offset to large for " + opcode + ":\r\nOffset = " + offset + " (max 127)\r\nAt line " + (lineNumber + 1));
                                                    if (offset < -128) return ("Offset to small for " + opcode + ":\r\nOffset = " + offset + " (min -128)\r\nAt line " + (lineNumber + 1));
                                                    RAMprogramLine[locationCounter] = lineNumber;
                                                    RAM[locationCounter++] = (byte)(offset);
                                                } else
                                                {
                                                    return ("Invalid operand for " + opcode + ":\r\n" + oResult + "\r\nAt line " + (lineNumber + 1));
                                                }
                                            } else
                                            {
                                                calcByte = GetByte(operands[i], out string oResult);
                                                if (oResult == "OK")
                                                {
                                                    RAMprogramLine[locationCounter] = lineNumber;
                                                    RAM[locationCounter++] = calcByte;
                                                } else
                                                {
                                                    return ("Invalid operand for " + opcode + ":\r\n" + oResult + "\r\nAt line " + (lineNumber + 1));
                                                }
                                            }
                                            break;

                                        case "(ix+o)":
                                            string arg_ix = "";
                                            operands[i] = operands[i].Trim(new char[] { '(', ')' });
                                            if (operands[i].Length > 2)
                                            {
                                                arg_ix = operands[i].Substring(2);
                                            }
                                            
                                            calcByte = GetByte(arg_ix, out string ixoResult);
                                            if (ixoResult == "OK")
                                            {
                                                RAMprogramLine[locationCounter] = lineNumber;
                                                RAM[locationCounter++] = calcByte;
                                            } else
                                            {
                                                return ("Invalid operand for " + opcode + ":\r\n" + ixoResult + "\r\nAt line " + (lineNumber + 1));
                                            }
                                            break;

                                        case "(iy+o)":
                                            string arg_iy = "";
                                            operands[i] = operands[i].Trim(new char[] { '(', ')' });
                                            if (operands[i].Length > 2)
                                            {
                                                arg_iy = operands[i].Substring(2);
                                            }

                                            calcByte = GetByte(arg_iy, out string iyoResult);
                                            if (iyoResult == "OK")
                                            {
                                                RAMprogramLine[locationCounter] = lineNumber;
                                                RAM[locationCounter++] = calcByte;
                                            } else
                                            {
                                                return ("Invalid operand for " + opcode + ":\r\n" + iyoResult + "\r\nAt line " + (lineNumber + 1));
                                            }
                                            break;
                                    }
                                }
                            }

                            // IXBit and IYBit: third opcode byte 
                            if (foundIXbit || foundIYbit)
                            {
                                RAM[locationCounter++] = Convert.ToByte(instruction.Opcode);
                            }

                            break;
                    }            
                } catch (Exception exception)
                {
                    if (locationCounter > 0xFFFF)
                    {
                        return ("MEMORY OVERRUN AT LINE " + (lineNumber + 1));
                    }

                    MessageBox.Show(exception.Message, "SECONDPASS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ("EXCEPTION ERROR AT LINE " + (lineNumber + 1));
                }
            }

            return ("OK");
        }

        #endregion
    }
}

 