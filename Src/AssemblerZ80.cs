using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Windows.Forms; 

namespace Z80_RC2014
{
    class AssemblerZ80
    {
        #region Members

        // All opcodes and mnemonics
        public readonly Instructions instructions;

        // Operator types at arithmetic operations
        public enum OPERATOR
        {
            NONE = 0,
            ADD  = 1,  // Add
            ADC  = 2,  // Add with carry
            SUB  = 3,  // Subtract
            SBC  = 4,  // Subtract with carry
            AND  = 5,  // Logical AND
            OR   = 6,  // Logical OR
            XOR  = 7,  // Logical Exclusive OR
            RLC  = 8,  // Rotate left with carry
            RRC  = 9,  // Rotate right with carry
            RL   = 10, // Rotate left through carry 
            RR   = 11, // Rotate right through carry 
            SLA  = 12, // Arithmetic shift left  
            SRA  = 13, // Arithmetic shift right
            SLL  = 14, // Logical shift left
            SRL  = 15  // Logical shift right
        }

        // Interrupt modes
        public enum IM
        {
            im0 = 0,
            im1 = 1,
            im2 = 2
        }

        // Current interrupt mode
        public IM im = IM.im0;

        // Segment types
        public enum SEGMENT
        {
            aseg = 0,
            cseg = 1,
            dseg = 2
        }

        // Segment currently active
        SEGMENT segment = SEGMENT.aseg;

        // Absolute program segment, Code program segment, Data program segment
        UInt16 aseg = 0x0000;
        UInt16 cseg = 0x0000;
        UInt16 dseg = 0x0000;

        // Total RAM of 65536 bytes (0x0000 - 0xFFFF)
        public byte[] RAM = new byte[0x10000];

        // Total 256 PORTS (0x00 - 0xFF)
        public byte[] PORT = new byte[0x0100];

        // Linenumber for a given byte of the program (Max 0x10000 = 65535 line numbers)
        public int[] RAMprogramLine = new int[0x10000];

        // Address Symbol Table
        public Dictionary<string, int> addressSymbolTable = new Dictionary<string, int>();

        // Processed program for running second pass
        public string[] programRun;

        // Program listing
        public string[] programView;

        // Current instruction to be processed
        private byte byteInstruction = 00;

        // Start location of the program
        public int startLocation;
        bool startLocationSet; 

        // Current location of the program (during firstpass and secondpass)
        public int locationCounter;

        // Register values
        public byte registerA = 0x00;
        public byte registerB = 0x00;
        public byte registerC = 0x00;
        public byte registerD = 0x00;
        public byte registerE = 0x00;
        public byte registerH = 0x00;
        public byte registerL = 0x00;
        public byte registerI = 0x00;
        public byte registerR = 0x00;

        public byte registerAalt = 0x00;
        public byte registerBalt = 0x00;
        public byte registerCalt = 0x00;
        public byte registerDalt = 0x00;
        public byte registerEalt = 0x00;
        public byte registerHalt = 0x00;
        public byte registerLalt = 0x00;

        public UInt16 registerIX = 0x0000;
        public UInt16 registerIY = 0x0000;

        public UInt16 registerPC = 0x0000;
        public UInt16 registerSP = 0x0000;

        // Flag values
        public bool flagC = false;
        public bool flagN = false;
        public bool flagPV = false;
        public bool flagH = false;
        public bool flagZ = false;
        public bool flagS = false;

        public bool flagCalt = false;
        public bool flagNalt = false;
        public bool flagPValt = false;
        public bool flagHalt = false;
        public bool flagZalt = false;
        public bool flagSalt = false;

        // Interrupt Enable value
        public bool intrIE = false;

        // Update output terminal
        public bool UpdateDisplay = false;

        // Update compact flash port
        public bool UpdateCompactFlashRead = false;
        public bool UpdateCompactFlashWrite = false;

        // Counter to indicate the byte in the sector to read/write
        public int cfIndex = 0;

        // Boolean to detect the vector (pointer) of  the SIO interrupt routine
        public bool register2Selected = false;

        // Address of the SIO interrupt handler (if detected)
        public int IO_INTERRUPT_HANDLER_VECTOR = -1;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor 
        /// </summary>        
        public AssemblerZ80(string[] program)
        {
            this.programRun = program;
            this.programView = new string[program.Length];

            instructions = new Instructions();

            startLocation = 0x0000;
            startLocationSet = false;
            registerSP = 0x0000;

            for (int i = 0; i < RAMprogramLine.Length; i++)
            {
                RAMprogramLine[i] = -1;
            }

            ClearRam();
            ClearPorts();
        }

        #endregion

        #region Methods (Calculations)

        /// <summary>
        /// Calculate and adjust the flags on screen
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="type"></param>
        private byte Calculate(byte arg1, byte arg2, OPERATOR type)
        {
            int i, count;
            byte b1, b2, b3;
            byte carry = (byte)(flagC ? 1 : 0);
            byte result = (byte)0x00;

            switch (type)
            {
                case OPERATOR.ADD:
                    result = (byte)(arg1 + arg2);

                    // Add/Subtract flag
                    flagN = false;

                    // Carry flag
                    if (arg1 + arg2 > 0xFF)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    // Half Carry flag
                    b1 = (byte)(arg1 & 0x0F);  // Masking upper 4 bits
                    b2 = (byte)(arg2 & 0x0F);  // Masking upper 4 bits

                    if (b1 + b2 > 0x0F)
                    {
                        flagH = true;
                    } else
                    {
                        flagH = false;
                    }

                    // Overflow flag
                    flagPV = false;
                    if ((arg1 >= 0x80) && (arg2 >= 0x80) && (result < 0x80)) flagPV = true;
                    if ((arg1 < 0x80) && (arg2 < 0x80) && (result >= 0x80)) flagPV = true;

                    // Zero flag
                    if (result == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x80)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    break;

                case OPERATOR.ADC:
                    result = (byte)(arg1 + arg2 + carry);

                    // Add/Subtract flag
                    flagN = false;

                    // Carry flag
                    if (arg1 + arg2 + carry > 0xFF)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    // Half Carry flag
                    b1 = (byte)(arg1 & 0x0F);  // Masking upper 4 bits
                    b2 = (byte)(arg2 & 0x0F);  // Masking upper 4 bits

                    if (b1 + b2 + carry > 0x0F)
                    {
                        flagH = true;
                    } else
                    {
                        flagH = false;
                    }

                    // Overflow flag
                    flagPV = false;
                    if ((arg1 >= 0x80) && (arg2 >= 0x80) && (result < 0x80)) flagPV = true;
                    if ((arg1 < 0x80) && (arg2 < 0x80) && (result >= 0x80)) flagPV = true;

                    // Zero flag
                    if (result == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x80)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    break;

                case OPERATOR.SUB:
                    result = (byte)(arg1 - arg2);

                    // Add/Subtract flag
                    flagN = true;

                    // Carry flag
                    if (arg1 - arg2 < 0x00)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    // Half Carry flag
                    b1 = (byte)(arg1 & 0x0F);  // Masking upper 4 bits
                    b2 = (byte)(arg2 & 0x0F);  // Masking upper 4 bits
                    flagH = (((b1 - b2) & 0x10) == 0x10);

                    // Overflow flag
                    flagPV = false;
                    if ((arg1 >= 0x80) && (arg2 < 0x80) && (result < 0x80)) flagPV = true;
                    if ((arg1 < 0x80) && (arg2 >= 0x80) && (result >= 0x80)) flagPV = true;

                    // Zero flag
                    if (result == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x80)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    break;

                case OPERATOR.SBC:
                    result = (byte)(arg1 - arg2 - carry);

                    // Add/Subtract flag
                    flagN = true;

                    // Carry flag
                    if (arg1 - arg2 - carry < 0x00)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    // Half Carry flag
                    b1 = (byte)( arg1 & 0x0F);  // Masking upper 4 bits
                    b2 = (byte)( arg2 & 0x0F);  // Masking upper 4 bits
                    b3 = (byte)(carry & 0x0F);  // Masking upper 4 bits
                    flagH = (((b1 - b2 - b3) & 0x10) == 0x10);

                    // Overflow flag
                    flagPV = false;
                    if ((arg1 >= 0x80) && (arg2 < 0x80) && (result < 0x80)) flagPV = true;
                    if ((arg1 < 0x80) && (arg2 >= 0x80) && (result >= 0x80)) flagPV = true;

                    // Zero flag
                    if (result == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x80)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    break;

                case OPERATOR.AND:
                    result = (byte)(arg1 & arg2);

                    flagC = false;
                    flagN = false;
                    flagH = true;

                    // Zero flag
                    if (result == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x80)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    // Parity flag
                    string strResultAND = Convert.ToString(Convert.ToInt32(result.ToString("X2"), 16), 2).PadLeft(8, '0');
                    count = 0;
                    for (i = 0; i < 8; i++)
                    {
                        if (strResultAND[i] == '1') count++;
                    }

                    if (count % 2 == 0)
                    {
                        flagPV = true;
                    } else
                    {
                        flagPV = false;
                    }

                    break;

                case OPERATOR.OR:
                    result = (byte)(arg1 | arg2);

                    flagC = false;
                    flagN = false;
                    flagH = false;

                    // Zero flag
                    if (result == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x80)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    // Parity flag
                    string strResultOR = Convert.ToString(Convert.ToInt32(result.ToString("X2"), 16), 2).PadLeft(8, '0');
                    count = 0;
                    for (i = 0; i < 8; i++)
                    {
                        if (strResultOR[i] == '1') count++;
                    }

                    if (count % 2 == 0)
                    {
                        flagPV = true;
                    } else
                    {
                        flagPV = false;
                    }

                    break;

                case OPERATOR.XOR:
                    result = (byte)(arg1 ^ arg2);

                    flagC = false;
                    flagN = false;
                    flagH = false;

                    // Zero flag
                    if (result == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x80)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    // Parity flag
                    string strResultXOR = Convert.ToString(Convert.ToInt32(result.ToString("X2"), 16), 2).PadLeft(8, '0');
                    count = 0;
                    for (i = 0; i < 8; i++)
                    {
                        if (strResultXOR[i] == '1') count++;
                    }

                    if (count % 2 == 0)
                    {
                        flagPV = true;
                    } else
                    {
                        flagPV = false;
                    }

                    break;

                default:
                    throw new Exception("Unknown operator '" + Enum.GetName(typeof(OPERATOR), type) + "' in calculation.");
            }

            return (result);
        }

        /// <summary>
        /// Calculate and adjust the flags on screen
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="type"></param>
        private UInt16 Calculate(UInt16 arg1, UInt16 arg2, OPERATOR type)
        {
            UInt16 b1, b2, b3;
            byte carry = (byte)(flagC ? 1 : 0);
            UInt16 result = (UInt16)0x0000;

            switch (type)
            {
                case OPERATOR.ADD:
                    result = (UInt16)(arg1 + arg2);

                    // Add/Subtract flag
                    flagN = false;

                    // Carry flag
                    if (arg1 + arg2 > 0xFFFF)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    // Half Carry flag
                    b1 = (UInt16)(arg1 & 0xFF);  // Masking upper 8 bits
                    b2 = (UInt16)(arg2 & 0xFF);  // Masking upper 8 bits

                    if (b1 + b2 > 0xFF)
                    {
                        flagH = true;
                    } else
                    {
                        flagH = false;
                    }

                    break;

                case OPERATOR.ADC:
                    result = (UInt16)(arg1 + arg2 + carry);

                    // Add/Subtract flag
                    flagN = false;

                    // Carry flag
                    if (arg1 + arg2 + carry > 0xFFFF)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    // Half Carry flag
                    b1 = (UInt16)(arg1 & 0xFF);  // Masking upper 8 bits
                    b2 = (UInt16)(arg2 & 0xFF);  // Masking upper 8 bits

                    if (b1 + b2 + carry > 0xFF)
                    {
                        flagH = true;
                    } else
                    {
                        flagH = false;
                    }

                    // Overflow flag
                    flagPV = false;
                    if ((arg1 >= 0x8000) && (arg2 >= 0x8000) && (result < 0x8000)) flagPV = true;
                    if ((arg1 < 0x8000) && (arg2 < 0x8000) && (result >= 0x8000)) flagPV = true;

                    // Zero flag
                    if (result == 0x0000)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x8000)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    break;

                case OPERATOR.SBC:
                    result = (UInt16)(arg1 - arg2 - carry);

                    // Add/Subtract flag
                    flagN = true;

                    // Carry flag
                    if (arg1 - arg2 - carry < 0x0000)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    // Half Carry flag
                    b1 = (byte)( arg1 & 0xFF);  // Masking upper 8 bits
                    b2 = (byte)( arg2 & 0xFF);  // Masking upper 8 bits
                    b3 = (byte)(carry & 0xFF);  // Masking upper 8 bits
                    flagH = (((b1 - b2 - b3) & 0x100) == 0x100);

                    // Overflow flag
                    flagPV = false;
                    if ((arg1 >= 0x8000) && (arg2 < 0x8000) && (result < 0x8000)) flagPV = true;
                    if ((arg1 < 0x8000) && (arg2 >= 0x8000) && (result >= 0x8000)) flagPV = true;

                    // Zero flag
                    if (result == 0x0000)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    // Sign flag
                    if (result >= 0x8000)
                    {
                        flagS = true;
                    } else
                    {
                        flagS = false;
                    }

                    break;

                default:
                    throw new Exception("Unknown operator '" + Enum.GetName(typeof(OPERATOR), type) + "' in calculation.");
            }

            return (result);
        }

        #endregion

        #region Methods (Rotation/Shift)

        /// <summary>
        /// Rotate or Shift bitwise and adjust the flags on screen
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="type"></param>
        private byte RotateShift(byte arg, OPERATOR type)
        {
            int i, count;
            bool lsb, msb;
            byte result = (byte)0x00;

            switch (type)
            {
                case OPERATOR.RLC:
                    msb = (arg & 0b10000000) == 0b10000000;
                    result = (byte)(arg << 1);
                    if (msb) result = (byte)(result | 0b00000001);

                    // Carry flag
                    if (msb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                case OPERATOR.RRC:
                    lsb = (arg & 0b00000001) == 0b00000001;
                    result = (byte)(arg >> 1);
                    if (lsb) result = (byte)(result | 0b10000000);

                    // Carry flag
                    if (lsb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                case OPERATOR.RL:
                    msb = (arg & 0b10000000) == 0b10000000;
                    result = (byte)(arg << 1);
                    if (flagC) result = (byte)(result | 0b00000001);

                    // Carry flag
                    if (msb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                case OPERATOR.RR:
                    lsb = (arg & 0b00000001) == 0b00000001;
                    result = (byte)(arg >> 1);
                    if (flagC) result = (byte)(result | 0b10000000);

                    // Carry flag
                    if (lsb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                case OPERATOR.SLA:
                    msb = (arg & 0b10000000) == 0b10000000;
                    result = (byte)(arg << 1);

                    // Carry flag
                    if (msb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                case OPERATOR.SRA:
                    msb = (arg & 0b10000000) == 0b10000000;
                    lsb = (arg & 0b00000001) == 0b00000001;
                    result = (byte)(arg >> 1);
                    if (msb) result = (byte)(result | 0b10000000);

                    // Carry flag
                    if (lsb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                case OPERATOR.SLL:
                    msb = (arg & 0b10000000) == 0b10000000;
                    result = (byte)(arg << 1);
                    result = (byte)(result | 0b00000001);

                    // Carry flag
                    if (msb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                case OPERATOR.SRL:
                    lsb = (arg & 0b00000001) == 0b00000001;
                    result = (byte)(arg >> 1);

                    // Carry flag
                    if (lsb)
                    {
                        flagC = true;
                    } else
                    {
                        flagC = false;
                    }

                    break;

                default:
                    throw new Exception("Unknown operator '" + Enum.GetName(typeof(OPERATOR), type) + "' in rotation.");
            }

            // Add/Subtract flag
            flagN = false;

            // Half Carry flag
            flagH = false;

            // Parity flag
            string strResultAND = Convert.ToString(Convert.ToInt32(result.ToString("X2"), 16), 2).PadLeft(8, '0');
            count = 0;
            for (i = 0; i < 8; i++)
            {
                if (strResultAND[i] == '1') count++;
            }

            if (count % 2 == 0)
            {
                flagPV = true;
            } else
            {
                flagPV = false;
            }

            // Zero flag
            if (result == 0x00)
            {
                flagZ = true;
            } else
            {
                flagZ = false;
            }

            // Sign flag
            if (result >= 0x80)
            {
                flagS = true;
            } else
            {
                flagS = false;
            }

            return (result);
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
                // Replace $ with location counter -1 (position of opcode)
                if (str.Trim() == "$") arg = arg.Replace("$", (locationCounter - 1).ToString());

                foreach (KeyValuePair<string, int> keyValuePair in addressSymbolTable)
                {
                    if (str.ToLower().Trim() == keyValuePair.Key.ToLower().Trim())
                    {
                        arg = arg.Replace(str, keyValuePair.Value.ToString());
                    }
                }
            }

            // Process low order byte of argument
            if (arg.ToLower().Contains("low("))
            {
                int start = arg.IndexOf('(') + 1;
                int end = arg.IndexOf(')', start);

                string argLow = arg.Substring(start, end - start);
                bool succes = Int32.TryParse(argLow, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int output);
                if (!succes)
                {
                    result = "Illegal argument for LOW(arg)";
                    return (0);
                }

                argLow = Convert.ToInt32(argLow).ToString("X4").Substring(2, 2);
                arg = Convert.ToInt32(argLow, 16).ToString() + arg.Substring(end + 1, arg.Length - 1 - end).Trim();
            }

            // Process high order byte of argument
            if (arg.ToLower().Contains("high("))
            {
                int start = arg.IndexOf('(') + 1;
                int end = arg.IndexOf(')', start);

                string argHigh = arg.Substring(start, end - start);
                bool succes = Int32.TryParse(argHigh, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int output);
                if (!succes)
                {
                    result = "Illegal argument for HIGH(arg)";
                    return (0);
                }

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
                        arg = arg.Replace(str, keyValuePair.Value.ToString());
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

        #region Methods (RegisterValue)

        /// <summary>
        /// Get the current content of a register
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool GetRegisterValue(byte arg, ref byte val)
        {
            switch (arg & 0b00001111)
            {
                case 0b0000:
                    val = registerB;
                    break;
                case 0b0001:
                    val = registerC;
                    break;
                case 0b0010:
                    val = registerD;
                    break;
                case 0b0011:
                    val = registerE;
                    break;
                case 0b0100:
                    val = registerH;
                    break;
                case 0b0101:
                    val = registerL;
                    break;
                case 0b0110:
                    val = RAM[registerH * 0x0100 + registerL];
                    break;
                case 0b0111:
                    val = registerA;
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Set the current content of a register
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool SetRegisterValue(byte arg, byte val)
        {
            switch (arg & 0b00001111)
            {
                case 0b0000:
                    registerB = val;
                    break;
                case 0b0001:
                    registerC = val;
                    break;
                case 0b0010:
                    registerD = val;
                    break;
                case 0b0011:
                    registerE = val;
                    break;
                case 0b0100:
                    registerH = val;
                    break;
                case 0b0101:
                    registerL = val;
                    break;
                case 0b0110:
                    RAM[registerH * 0x0100 + registerL] = val;
                    break;
                case 0b0111:
                    registerA = val;
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Get the current content of a register (IX/IY instructions)
        /// </summary>
        /// <param name="register"></param>
        /// <param name="offset"></param>
        /// <param name="arg"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool GetRegisterValue(ref UInt16 register, byte offset, byte arg, ref byte val)
        {
            switch (arg & 0b00001111)
            {
                case 0b0000:
                    val = registerB;
                    break;
                case 0b0001:
                    val = registerC;
                    break;
                case 0b0010:
                    val = registerD;
                    break;
                case 0b0011:
                    val = registerE;
                    break;
                case 0b0100:
                    val = (byte)(register >> 8);
                    break;
                case 0b0101:
                    val = (byte)(register & 0xFF);
                    break;
                case 0b0110:
                    val = RAM[register + offset];
                    break;
                case 0b0111:
                    val = registerA;
                    break;
                default:
                    return false;
            }

            return true;
        }

        #endregion

        #region Methods (Div)

        /// <summary>
        /// Set (certain) flags according to the value
        /// </summary>
        /// <param name="val"></param>
        private void SetFlags(byte val)
        {
            int i, count;

            // Add/Subtract flag
            flagN = false;

            // Half Carry flag
            flagH = false;

            // Parity flag
            string strResultAND = Convert.ToString(Convert.ToInt32(val.ToString("X2"), 16), 2).PadLeft(8, '0');
            count = 0;
            for (i = 0; i < 8; i++)
            {
                if (strResultAND[i] == '1') count++;
            }

            if (count % 2 == 0)
            {
                flagPV = true;
            } else
            {
                flagPV = false;
            }

            // Zero flag
            if (val == 0x00)
            {
                flagZ = true;
            } else
            {
                flagZ = false;
            }

            // Sign flag
            if (val >= 0x80)
            {
                flagS = true;
            } else
            {
                flagS = false;
            }
        }

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

        /// <summary>
        /// Clear the Ports
        /// </summary>
        public void ClearPorts()
        {
            for (int i = 0; i < PORT.Length; i++)
            {
                PORT[i] = 0x00;
            }
        }

        /// <summary>
        /// Check for any action to perform on writing to ports 
        /// </summary>
        /// <param name="port"></param>
        private void CheckOutPorts(byte port)
        {
            // Output to port A data or port B data
            if (port == 0x81) UpdateDisplay = true;
            if (port == 0x83) UpdateDisplay = true;
            // Set SIO control port status 'buffer empty' and 'no char ready'
            if (port == 0x80) PORT[0x80] |= 0x04;
            if (port == 0x82) PORT[0x82] |= 0x04;
            // Set Compact Flash card status 'ready' (clear 'busy' bit)
            if (port == 0x17) PORT[0x17] &= 0x7F;
            // Indicate write to CF card
            if (port == 0x10) UpdateCompactFlashWrite = true;
            // Reset index for (new) sector to read/write
            if ((port == 0x17) && ((registerA == 0x20) || (registerA == 0x30))) cfIndex = 0;
            // Set SIO/2 interrupt vector    
            if ((port == 0x82) && register2Selected) IO_INTERRUPT_HANDLER_VECTOR = registerA;
            if ((port == 0x82) && (registerA == 0x02)) register2Selected = true;
            if ((port == 0x82) && (registerA != 0x02)) register2Selected = false;
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
                            string opr = operands[indexOperands].Trim();

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

                            // Operand to lowercase for easy checking next items
                            opr = opr.ToLower();

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
            // locationCounter is a temporary variable to traverse program for first pass
            locationCounter = 0;

            // Opcode in the line
            string opcode;

            // Operand(s) for the opcode 
            string[] operands;              

            char[] delimiters = new[] { ',' };

            // Process all lines
            for (int lineNumber = 0; lineNumber < programRun.Length; lineNumber++)       
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
                        if ((label != null) && (label != ""))
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

                            // Check for empty labels
                            if ((label == null) || (label == ""))
                            {
                                return ("Empty label at line " + (lineNumber + 1));
                            }

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
                        // Line must have an argument after the opcode
                        if (operands.Length == 0)
                        {
                            return ("org directive must have an argument following at line " + (lineNumber + 1));
                        }

                        // If valid address then store in locationCounter
                        int calc = Get2Bytes(operands[0], out string result);
                        if (result == "OK")
                        {
                            locationCounter = calc;

                            // Set startlocation if not set already
                            if (!startLocationSet)
                            {
                                startLocation = locationCounter;
                                startLocationSet = true;
                            }
                        } else
                        {
                            return ("Invalid operand for " + opcode + "(" + result + ") at line " + (lineNumber + 1));
                        }

                        // Copy to program for second pass
                        programRun[lineNumber] = opcode + " " + operands[0];

                        // Copy to programView for examining
                        programView[lineNumber] = opcode + " " + operands[0];

                        // Next line
                        continue;
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
                        return "OK";
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
        public string SecondPass()
        {
            // Using locationCounter to traverse the location of RAM during second pass
            locationCounter = 0; 

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

            for (int lineNumber = 0; lineNumber < programRun.Length; lineNumber ++)
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

                            if (operands.Length == 0)
                            {
                                // Must have an operand
                                return ("Missing operand for " + opcode + " at line " + (lineNumber + 1));
                            } else
                            {
                                // If valid address then store in locationCounter
                                calcShort = Get2Bytes(operands[0], out string resultOrg);
                                if (resultOrg == "OK")
                                {
                                    locationCounter = calcShort;
                                } else
                                {
                                    return ("Invalid operand for " + opcode + ": " + resultOrg + " at line " + (lineNumber + 1));
                                }
                            }
                            break;

                        case "end":                                                                                     // end
                        case ".end":                                                                                     

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
                                    if (RAMprogramLine[locationCounter] != -1)
                                    {
                                        return ("Allready code at 0x" + locationCounter.ToString("X4") + " (from line " + (RAMprogramLine[locationCounter] +1).ToString() + ") for " + opcode + " at line " + (lineNumber + 1));
                                    }

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
                                    if (RAMprogramLine[locationCounter] != -1)
                                    {
                                        return ("Allready code at 0x" + locationCounter.ToString("X4") + " (from line " + (RAMprogramLine[locationCounter] + 1).ToString() + ") for " + opcode + " at line " + (lineNumber + 1));
                                    }

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
                                    if (RAMprogramLine[locationCounter] != -1)
                                    {
                                        return ("Allready code at 0x" + locationCounter.ToString("X4") + " (from line " + (RAMprogramLine[locationCounter] + 1).ToString() + ") for " + opcode + " at line " + (lineNumber + 1));
                                    }

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

                            if (RAMprogramLine[locationCounter] != -1)
                            {
                                return ("Allready code at 0x" + locationCounter.ToString("X4") + " (from line " + (RAMprogramLine[locationCounter] + 1).ToString() + ") for " + opcode + " at line " + (lineNumber + 1));
                            }

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
                                    if (RAMprogramLine[locationCounter] != -1)
                                    {
                                        return ("Allready code at 0x" + locationCounter.ToString("X4") + " (from line " + (RAMprogramLine[locationCounter] + 1).ToString() + ") for " + opcode + " at line " + (lineNumber + 1));
                                    }

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

                    // Show ascii if db
                    if (((opcode == "db") || (opcode == ".db") || (opcode == ".text")) && (operands.Length > 0))
                    {
                        string strAscii = " ('";

                        for (int i = 0; i < operands.Length; i++)
                        {
                            calcByte = GetByte(operands[i], out string resultdb);
                            if (resultdb == "OK")
                            {
                                if ((calcByte >= 32) && (calcByte < 127))
                                {
                                    strAscii += Convert.ToChar(calcByte);
                                } else
                                {
                                    strAscii += '.';
                                }
                            }
                        }

                        programView[lineNumber] +=  strAscii + "')";
                    }

                    // Show ascii if ld
                    if ((opcode == "ld") && (operands.Length > 1))
                    {
                        calcByte = GetByte(operands[1], out string resultdb);
                        if (resultdb == "OK")
                        {
                            if ((calcByte >= 32) && (calcByte < 127))
                            {
                                programView[lineNumber] += " ('" + Convert.ToChar(calcByte) + "')";
                            }
                        }
                    }

                    // Show ascii if cp
                    if ((opcode == "cp") && (operands.Length > 0))
                    {
                        calcByte = GetByte(operands[0], out string resultdb);
                        if (resultdb == "OK")
                        {
                            if ((calcByte >= 32) && (calcByte < 127))
                            {
                                programView[lineNumber] += " ('" + Convert.ToChar(calcByte) + "')";
                            }
                        }
                    }

                    // Update current segment
                    if (segment == SEGMENT.aseg) aseg = (UInt16)locationCounter;
                    if (segment == SEGMENT.cseg) cseg = (UInt16)locationCounter;
                    if (segment == SEGMENT.dseg) dseg = (UInt16)locationCounter;

                    if ((opcode != "org") && (opcode != ".org") && (opcode != "aseg") && (opcode != "cseg") && (opcode != "dseg") && (opcode != "db") && (opcode != ".db") && (opcode != ".text"))
                     {
                        while (programView[lineNumber].Length < 46)
                        {
                            programView[lineNumber] += " ";
                        }

                        for (int i = locationCounterInstructionStart; i < locationCounter; i++)
                        {
                            programView[lineNumber] += " " + RAM[i].ToString("X2");
                        }
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

        #region Methods (RunInstruction)

        /// <summary>
        /// Run program from memory address
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="nextAddress"></param>
        /// <returns></returns>
        public string RunInstruction(UInt16 startAddress, ref UInt16 nextAddress)
        { 
            int num;
            bool result;
            byte val = 0x00;
            registerPC = startAddress;
            string lo, hi;

            byteInstruction = RAM[registerPC];

            try
            {
                if (byteInstruction == 0xCE)                                                                                // adc a,n 
                {
                    registerPC++;
                    registerA = Calculate(registerA, RAM[registerPC], OPERATOR.ADC);
                    registerPC++;
                } else if ((byteInstruction >= 0x88) && (byteInstruction <= 0x8F))                                          // adc a,r
                {
                    num = byteInstruction - 0x88;
                    result = GetRegisterValue((byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, (byte)(val), OPERATOR.ADC);
                    registerPC++;
                } else if (byteInstruction == 0xC6)                                                                         // add a,n
                {
                    registerPC++;
                    registerA = Calculate(registerA, RAM[registerPC], OPERATOR.ADD);
                    registerPC++;
                } else if ((byteInstruction >= 0x80) && (byteInstruction <= 0x87))                                          // add a,r
                {
                    num = byteInstruction - 0x80;
                    result = GetRegisterValue((byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.ADD);
                    registerPC++;
                } else if (byteInstruction == 0x09)                                                                         // add hl,bc
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerB + registerC);
                    UInt16 value2 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADD);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x19)                                                                         // add hl,de
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerD + registerE);
                    UInt16 value2 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADD);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x29)                                                                         // add hl,hl
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADD);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x39)                                                                         // add hl,sp
                {
                    UInt16 value1 = registerSP;
                    UInt16 value2 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADD);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0xE6)                                                                         // and n
                {
                    registerPC++;
                    registerA = Calculate(registerA, RAM[registerPC], OPERATOR.AND);
                    registerPC++;
                } else if ((byteInstruction >= 0xA0) && (byteInstruction <= 0xA7))                                          // and r
                {
                    num = byteInstruction - 0xA0;
                    result = GetRegisterValue((byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.AND);
                    registerPC++;
                } else if (byteInstruction == 0xDC)                                                                         // call c,nn    
                {
                    if (flagC)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0xFC)                                                                         // call m,nn
                {
                    if (flagS)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0xD4)                                                                         // call nc,nn 
                {
                    if (flagC)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xCD)                                                                         // call nn
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = address;
                } else if (byteInstruction == 0xC4)                                                                         // call nz,nn
                {
                    if (flagZ)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xF4)                                                                         // call p,nn
                {
                    if (flagS)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xEC)                                                                         // call pe,nn
                {
                    if (flagPV)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0xE4)                                                                         // call po,nn
                {
                    if (flagPV)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xCC)                                                                         // call z,nn
                {
                    if (flagZ)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC++;
                        Get2ByteFromInt(registerPC, out lo, out hi);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(hi, 16);
                        registerSP--;
                        RAM[registerSP] = Convert.ToByte(lo, 16);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0x3F)                                                                         // ccf
                {
                    flagH = flagC;
                    flagC = !flagC;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0xFE)                                                                         // cp n  
                {
                    registerPC++;
                    Calculate(registerA, RAM[registerPC], OPERATOR.SUB);
                    registerPC++;
                } else if ((byteInstruction >= 0xB8) && (byteInstruction <= 0xBF))                                          // cp r  
                {
                    num = byteInstruction - 0xB8;
                    byte compareValue = 0x00;
                    result = GetRegisterValue((byte)num, ref compareValue);
                    if (!result) return ("Can't get the register value");
                    Calculate(registerA, compareValue, OPERATOR.SUB);
                    registerPC++;
                } else if (byteInstruction == 0x2F)                                                                         // cpl
                {
                    registerA = (byte)(0xFF - registerA);
                    flagN = true;
                    flagH = true;
                    registerPC++;
                } else if (byteInstruction == 0x27)                                                                         // daa 
                {
                    byte low = (byte)(registerA & 0x0F);
                    byte high = (byte)(registerA & 0xF0);
                    bool flagCold = flagC;
                    bool flagHold = flagH;

                    byte diff = 0x00;

                    if ((high <= 0x90) && (low <= 0x09) && flagHold && !flagCold)
                    {
                        diff = 0x06;
                    }

                    if ((high <= 0x80) && (low >= 0x0A) && !flagCold)
                    {
                        diff = 0x06;
                    }

                    if ((high >= 0xA0) && (low <= 0x09) && !flagHold && !flagCold)
                    {
                        diff = 0x60;
                    }

                    if ((low <= 0x09) && !flagHold && flagCold)
                    {
                        diff = 0x60;
                    }

                    if ((low <= 0x09) && flagHold && flagCold)
                    {
                        diff = 0x66;
                    }

                    if ((low >= 0x0A) && flagCold)
                    {
                        diff = 0x66;
                    }

                    if ((high >= 0x90) && (low >= 0x0A) && !flagCold)
                    {
                        diff = 0x66;
                    }

                    if ((high >= 0xA0) && (low <= 0x09) && flagHold && !flagCold)
                    {
                        diff = 0x66;
                    }

                    if (flagN == false)
                    {
                        registerA = Calculate(registerA, diff, OPERATOR.ADD);
                    } else
                    {
                        registerA = Calculate(registerA, diff, OPERATOR.SUB);
                    }

                    // Carry
                    flagC = false;
                    if ((high >= 0x90) && (low > 0x09)) flagC = true;
                    if ((high > 0x90) || flagCold) flagC = true;

                    // Sign
                    flagS = (registerA >= 0x80) ? true : false;

                    // Half Carry
                    if (!flagN && (low <= 0x09)) flagH = false;
                    if (!flagN && (low > 0x09)) flagH = true;
                    if (flagN && !flagHold) flagH = false;
                    if (flagN && flagHold && (low >= 0x06)) flagH = false;
                    if (flagN && flagHold && (low < 0x06)) flagH = true;

                    // Parity
                    string strResult = Convert.ToString(Convert.ToInt32(registerA.ToString("X2"), 16), 2).PadLeft(8, '0');
                    int count = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (strResult[i] == '1') count++;
                    }

                    if (count % 2 == 0)
                    {
                        flagPV = true;
                    } else
                    {
                        flagPV = false;
                    }

                    // Zero
                    if (registerA == 0x00)
                    {
                        flagZ = true;
                    } else
                    {
                        flagZ = false;
                    }

                    registerPC++;
                } else if (byteInstruction == 0x3D)                                                                         // dec a
                {
                    bool save_flag = flagC;
                    registerA = Calculate(registerA, 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x05)                                                                         // dec b
                {
                    bool save_flag = flagC;
                    registerB = Calculate(registerB, 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x0D)                                                                         // dec c
                {
                    bool save_flag = flagC;
                    registerC = Calculate(registerC, 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x15)                                                                         // dec d
                {
                    bool save_flag = flagC;
                    registerD = Calculate(registerD, 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x1D)                                                                         // dec e
                {
                    bool save_flag = flagC;
                    registerE = Calculate(registerE, 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x25)                                                                         // dec h
                {
                    bool save_flag = flagC;
                    registerH = Calculate(registerH, 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x2D)                                                                         // dec l
                {
                    bool save_flag = flagC;
                    registerL = Calculate(registerL, 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x35)                                                                         // dec (hl)
                {
                    bool save_flag = flagC;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = Calculate(RAM[address], 0x01, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x0B)                                                                         // dec bc
                {
                    int value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x1B)                                                                         // dec de
                {
                    int value = 0x0100 * registerD + registerE;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerD = (byte)Convert.ToInt32(hi, 16);
                    registerE = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x2B)                                                                         // dec hl
                {
                    int value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x3B)                                                                         // dec sp
                {
                    registerSP -= 0x01;
                    registerPC++;
                } else if (byteInstruction == 0xF3)                                                                         // di
                {
                    intrIE = false; 
                    registerPC++;
                } else if (byteInstruction == 0x10)                                                                         // djnz o 
                {
                    registerB -= 0x01;
                    if (registerB == 0)
                    {
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        registerPC++;
                        byte offset = RAM[registerPC];
                        registerPC++;
                        UInt16 address = registerPC;
                        if (offset < 0x80) address += offset;
                        if (offset >= 0x80) address -= (UInt16)(0x100 - offset);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xFB)                                                                         // ei
                {
                    intrIE = true;
                    registerPC++;
                } else if (byteInstruction == 0x08)                                                                         // ex af,af'
                {
                    byte temp;
                    temp = registerA;
                    registerA = registerAalt;
                    registerAalt = temp;
                    temp = 0x00;
                    if (flagS) temp += 0x80;
                    if (flagZ) temp += 0x40;
                    if (flagH) temp += 0x10;
                    if (flagPV) temp += 0x04;
                    if (flagC) temp += 0x01;
                    flagS = flagSalt;
                    flagZ = flagZalt;
                    flagH = flagHalt;
                    flagPV = flagPValt;
                    flagC = flagCalt;
                    if ((temp & 0x01) != 0) flagCalt = true; else flagCalt = false;
                    if ((temp & 0x04) != 0) flagPValt = true; else flagPValt = false;
                    if ((temp & 0x10) != 0) flagHalt = true; else flagHalt = false;
                    if ((temp & 0x40) != 0) flagZalt = true; else flagZalt = false;
                    if ((temp & 0x80) != 0) flagSalt = true; else flagSalt = false;
                    registerPC++;
                } else if (byteInstruction == 0xEB)                                                                         // ex de,hl
                {
                    byte temp;
                    temp = registerD;
                    registerD = registerH;
                    registerH = temp;
                    temp = registerL;
                    registerL = registerE;
                    registerE = temp;
                    registerPC++;
                } else if (byteInstruction == 0xE3)                                                                         // ex (sp),hl
                {
                    byte b1, b2;
                    b1 = registerL;
                    b2 = registerH;
                    registerL = RAM[registerSP];
                    RAM[registerSP] = b1;
                    registerH = RAM[registerSP + 1];
                    RAM[registerSP + 1] = b2;
                    registerPC++;
                } else if (byteInstruction == 0xD9)                                                                         // exx
                {
                    byte temp;
                    temp = registerB;
                    registerB = registerBalt;
                    registerBalt = temp;
                    temp = registerC;
                    registerC = registerCalt;
                    registerCalt = temp;
                    temp = registerD;
                    registerD = registerDalt;
                    registerDalt = temp;
                    temp = registerE;
                    registerE = registerEalt;
                    registerEalt = temp;
                    temp = registerH;
                    registerH = registerHalt;
                    registerHalt = temp;
                    temp = registerL;
                    registerL = registerLalt;
                    registerLalt = temp;
                    registerPC++;
                } else if (byteInstruction == 0x76)                                                                         // halt
                {
                    return ("System Halted");
                } else if (byteInstruction == 0xdb)                                                                         // in a,(n)
                {
                    registerPC++;
                    if (RAM[registerPC] == 0x10)
                    {
                        // Update the Port and registerA from the compact flash card data
                        UpdateCompactFlashRead = true;
                    } else
                    {
                        registerA = PORT[RAM[registerPC]];
                    }
                    registerPC++;
                } else if (byteInstruction == 0x3C)                                                                         // inc a
                {
                    bool save_flag = flagC;
                    registerA = Calculate(registerA, 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x04)                                                                         // inc b
                {
                    bool save_flag = flagC;
                    registerB = Calculate(registerB, 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x0C)                                                                         // inc c
                {
                    bool save_flag = flagC;
                    registerC = Calculate(registerC, 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x14)                                                                         // inc d
                {
                    bool save_flag = flagC;
                    registerD = Calculate(registerD, 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x1C)                                                                         // inc e
                {
                    bool save_flag = flagC;
                    registerE = Calculate(registerE, 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x24)                                                                         // inc h
                {
                    bool save_flag = flagC;
                    registerH = Calculate(registerH, 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x2C)                                                                         // inc l
                {
                    bool save_flag = flagC;
                    registerL = Calculate(registerL, 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x34)                                                                         // inc (hl)
                {
                    bool save_flag = flagC;
                    UInt16 address = 0;
                    address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = Calculate(RAM[address], 0x01, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x03)                                                                         // inc bc
                {
                    int value = 0x0100 * registerB + registerC;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x13)                                                                         // inc de
                {
                    int value = 0x0100 * registerD + registerE;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerD = (byte)Convert.ToInt32(hi, 16);
                    registerE = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x23)                                                                         // inc hl
                {
                    int value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x33)                                                                         // inc sp
                {
                    registerSP += 0x01;
                    registerPC++;
                } else if (byteInstruction == 0xDA)                                                                         // jp c,nn
                {
                    if (flagC)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0xE9)                                                                         // jp (hl)
                {
                    registerPC = (UInt16)(registerH * 0x0100 + registerL);
                } else if (byteInstruction == 0xFA)                                                                         // jp m,nn
                {
                    if (flagS)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0xC3)                                                                         // jp nn
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerPC = address;
                } else if (byteInstruction == 0xD2)                                                                         // jp nc,nn
                {
                    if (flagC)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xC2)                                                                         // jp nz,nn
                {
                    if (flagZ)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xF2)                                                                         // jp p,nn
                {
                    if (flagS)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xEA)                                                                         // jp pe,nn
                {
                    if (flagPV)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0xE2)                                                                         // jp po,nn
                {
                    if (flagPV)
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xCA)                                                                         // jp z,nn
                {
                    if (flagZ)
                    {
                        UInt16 address = 0;
                        registerPC++;
                        address += RAM[registerPC];
                        registerPC++;
                        address += (UInt16)(0x0100 * RAM[registerPC]);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0x38)                                                                         // jr c,o
                {
                    if (flagC)
                    {
                        registerPC++;
                        byte offset = RAM[registerPC];
                        registerPC++;
                        UInt16 address = registerPC;
                        if (offset < 0x80) address += offset;
                        if (offset >= 0x80) address -= (UInt16)(0x100 - offset);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0x30)                                                                         // jr nc,o
                {
                    if (flagC)
                    {
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        registerPC++;
                        byte offset = RAM[registerPC];
                        registerPC++;
                        UInt16 address = registerPC;
                        if (offset < 0x80) address += offset;
                        if (offset >= 0x80) address -= (UInt16)(0x100 - offset);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0x20)                                                                         // jr nz,o
                {
                    if (flagZ)
                    {
                        registerPC++;
                        registerPC++;
                    } else
                    {
                        registerPC++;
                        byte offset = RAM[registerPC];
                        registerPC++;
                        UInt16 address = registerPC;
                        if (offset < 0x80) address += offset;
                        if (offset >= 0x80) address -= (UInt16)(0x100 - offset);
                        registerPC = address;
                    }
                } else if (byteInstruction == 0x18)                                                                         // jr o
                {
                    registerPC++;
                    byte offset = RAM[registerPC];
                    registerPC++;
                    UInt16 address = registerPC;
                    if (offset < 0x80) address += offset;
                    if (offset >= 0x80) address -= (UInt16)(0x100 - offset);
                    registerPC = address;
                } else if (byteInstruction == 0x28)                                                                         // jr z,o
                {
                    if (flagZ)
                    {
                        registerPC++;
                        byte offset = RAM[registerPC];
                        registerPC++;
                        UInt16 address = registerPC;
                        if (offset < 0x80) address += offset;
                        if (offset >= 0x80) address -= (UInt16)(0x100 - offset);
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                        registerPC++;
                    }
                } else if (byteInstruction == 0x3A)                                                                         // ld a,(nn)
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerA = RAM[address];
                    registerPC++;
                } else if (byteInstruction == 0x0A)                                                                         // ld a,(bc)
                {
                    UInt16 address = 0;
                    address = (UInt16)(registerB * 0x0100 + registerC);
                    registerA = RAM[address];
                    registerPC++;
                } else if (byteInstruction == 0x1A)                                                                         // ld a,(de)
                {
                    UInt16 address = 0;
                    address = (UInt16)(registerD * 0x0100 + registerE);
                    registerA = RAM[address];
                    registerPC++;
                } else if (byteInstruction == 0x01)                                                                         // ld bc,nn
                {
                    registerPC++;
                    registerC = RAM[registerPC];
                    registerPC++;
                    registerB = RAM[registerPC];
                    registerPC++;
                } else if (byteInstruction == 0x11)                                                                         // ld de,nn
                {
                    registerPC++;
                    registerE = RAM[registerPC];
                    registerPC++;
                    registerD = RAM[registerPC];
                    registerPC++;
                } else if (byteInstruction == 0x21)                                                                         // ld hl,nn
                {
                    registerPC++;
                    registerL = RAM[registerPC];
                    registerPC++;
                    registerH = RAM[registerPC];
                    registerPC++;
                } else if (byteInstruction == 0x2A)                                                                         // ld hl,(nn)
                {
                    UInt16 address = 0;
                    registerPC++;
                    address = RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerPC++;
                    registerL = RAM[address];
                    address++;
                    registerH = RAM[address];
                } else if (byteInstruction == 0x02)                                                                         // ld (bc),a
                {
                    UInt16 address;
                    address = registerC;
                    address += (UInt16)(0x0100 * registerB);
                    RAM[address] = registerA;
                    registerPC++;
                } else if (byteInstruction == 0x12)                                                                         // ld (de),a
                {
                    UInt16 address;
                    address = registerE;
                    address = (UInt16)(address + (0x0100 * registerD));
                    RAM[address] = registerA;
                    registerPC++;
                } else if ((byteInstruction == 0x70) ||                                                                     // ld (hl),r
                           (byteInstruction == 0x71) ||
                           (byteInstruction == 0x72) ||
                           (byteInstruction == 0x73) ||
                           (byteInstruction == 0x74) ||
                           (byteInstruction == 0x75) ||
                           (byteInstruction == 0x77))
                {
                    num = byteInstruction - 0x40;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = val;
                    registerPC++;
                } else if (byteInstruction == 0x32)                                                                         // ld (nn),a
                {
                    UInt16 address = 0;
                    registerPC++;
                    address = RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    RAM[address] = registerA;
                    registerPC++;
                } else if (byteInstruction == 0x22)                                                                         // ld (nn),hl
                {
                    UInt16 address = 0;
                    registerPC++;
                    address = RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerPC++;
                    RAM[address] = registerL;
                    address++;
                    RAM[address] = registerH;
                } else if ((byteInstruction == 0x46) ||                                                                     // ld r,(hl)
                           (byteInstruction == 0x4E) ||
                           (byteInstruction == 0x56) ||
                           (byteInstruction == 0x5E) ||
                           (byteInstruction == 0x66) ||
                           (byteInstruction == 0x6E) ||
                           (byteInstruction == 0x7E))
                {
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    num = byteInstruction - 0x40;
                    result = SetRegisterValue((byte)((num >> 3) & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x06) ||                                                                     // ld r,n
                           (byteInstruction == 0x0E) ||
                           (byteInstruction == 0x16) ||
                           (byteInstruction == 0x1E) ||
                           (byteInstruction == 0x26) ||
                           (byteInstruction == 0x2E) ||
                           (byteInstruction == 0x36) ||
                           (byteInstruction == 0x3E))

                {
                    registerPC++;
                    val = RAM[registerPC];
                    num = byteInstruction;
                    result = SetRegisterValue((byte)((num >> 3) & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x40) ||                                                                     // ld r,r
                           (byteInstruction == 0x41) ||
                           (byteInstruction == 0x42) ||
                           (byteInstruction == 0x43) ||
                           (byteInstruction == 0x44) ||
                           (byteInstruction == 0x45) ||
                           (byteInstruction == 0x47) ||
                           (byteInstruction == 0x48) ||
                           (byteInstruction == 0x49) ||
                           (byteInstruction == 0x4A) ||
                           (byteInstruction == 0x4B) ||
                           (byteInstruction == 0x4C) ||
                           (byteInstruction == 0x4D) ||
                           (byteInstruction == 0x4F) ||
                           (byteInstruction == 0x50) ||
                           (byteInstruction == 0x51) ||
                           (byteInstruction == 0x52) ||
                           (byteInstruction == 0x53) ||
                           (byteInstruction == 0x54) ||
                           (byteInstruction == 0x55) ||
                           (byteInstruction == 0x57) ||
                           (byteInstruction == 0x58) ||
                           (byteInstruction == 0x59) ||
                           (byteInstruction == 0x5A) ||
                           (byteInstruction == 0x5B) ||
                           (byteInstruction == 0x5C) ||
                           (byteInstruction == 0x5D) ||
                           (byteInstruction == 0x5F) ||
                           (byteInstruction == 0x60) ||
                           (byteInstruction == 0x61) ||
                           (byteInstruction == 0x62) ||
                           (byteInstruction == 0x63) ||
                           (byteInstruction == 0x64) ||
                           (byteInstruction == 0x65) ||
                           (byteInstruction == 0x67) ||
                           (byteInstruction == 0x68) ||
                           (byteInstruction == 0x69) ||
                           (byteInstruction == 0x6A) ||
                           (byteInstruction == 0x6B) ||
                           (byteInstruction == 0x6C) ||
                           (byteInstruction == 0x6D) ||
                           (byteInstruction == 0x6F) ||
                           (byteInstruction == 0x78) ||
                           (byteInstruction == 0x79) ||
                           (byteInstruction == 0x7A) ||
                           (byteInstruction == 0x7B) ||
                           (byteInstruction == 0x7C) ||
                           (byteInstruction == 0x7D) ||
                           (byteInstruction == 0x7F))
                { 
                    num = byteInstruction - 0x40;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = SetRegisterValue((byte)((num >> 3) & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if (byteInstruction == 0xF9)                                                                         // ld sp,hl
                {
                    registerSP = registerL;
                    registerSP = (UInt16)(registerSP + (0x0100 * registerH));
                    registerPC++;
                } else if (byteInstruction == 0x31)                                                                         // ld sp,nn
                {
                    byte b1, b2;
                    registerPC++;
                    b1 = RAM[registerPC];
                    registerPC++;
                    b2 = RAM[registerPC];
                    registerPC++;
                    registerSP = (UInt16)(b1 + (0x0100 * b2));
                } else if (byteInstruction == 0x00)                                                                         // nop
                {
                    registerPC++;
                } else if ((byteInstruction >= 0xB0) && (byteInstruction <= 0xB7))                                          // or r
                {
                    num = byteInstruction - 0xB0;
                    result = GetRegisterValue((byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.OR);
                    registerPC++;
                } else if (byteInstruction == 0xF6)                                                                         // or n
                {
                    registerPC++;
                    registerA = Calculate(registerA, RAM[registerPC], OPERATOR.OR);
                    registerPC++;
                } else if (byteInstruction == 0xD3)                                                                         // out (n),a 
                {
                    registerPC++;
                    PORT[RAM[registerPC]] = registerA;
                    CheckOutPorts(RAM[registerPC]);
                    registerPC++;
                } else if (byteInstruction == 0xF1)                                                                         // pop af
                {
                    byte flags, b;
                    flags = RAM[registerSP];
                    registerSP++;
                    registerA = RAM[registerSP];
                    registerSP++;
                    b = (byte)(flags & 0x01);
                    if (b != 0) flagC = true; else flagC = false;
                    b = (byte)(flags & 0x02);
                    if (b != 0) flagN = true; else flagN = false;
                    b = (byte)(flags & 0x04);
                    if (b != 0) flagPV = true; else flagPV = false;
                    b = (byte)(flags & 0x10);
                    if (b != 0) flagH = true; else flagH = false;
                    b = (byte)(flags & 0x40);
                    if (b != 0) flagZ = true; else flagZ = false;
                    b = (byte)(flags & 0x80);
                    if (b != 0) flagS = true; else flagS = false;
                    registerPC++;
                } else if (byteInstruction == 0xC1)                                                                         // pop bc
                {
                    registerC = RAM[registerSP];
                    registerSP++;
                    registerB = RAM[registerSP];
                    registerSP++;
                    registerPC++;
                } else if (byteInstruction == 0xD1)                                                                         // pop de
                {
                    registerE = RAM[registerSP];
                    registerSP++;
                    registerD = RAM[registerSP];
                    registerSP++;
                    registerPC++;
                } else if (byteInstruction == 0xE1)                                                                         // pop hl
                {
                    registerL = RAM[registerSP];
                    registerSP++;
                    registerH = RAM[registerSP];
                    registerSP++;
                    registerPC++;
                } else if (byteInstruction == 0xF5)                                                                         // push af 
                {
                    byte aflag = 00;
                    if (flagS) aflag += 0x80;
                    if (flagZ) aflag += 0x40;
                    if (flagH) aflag += 0x10;
                    if (flagPV) aflag += 0x04;
                    if (flagN) aflag += 0x02;
                    if (flagC) aflag += 0x01;
                    registerSP--;
                    RAM[registerSP] = registerA;
                    registerSP--;
                    RAM[registerSP] = aflag;
                    registerPC++;
                } else if (byteInstruction == 0xC5)                                                                         // push bc
                {
                    registerSP--;
                    RAM[registerSP] = registerB;
                    registerSP--;
                    RAM[registerSP] = registerC;
                    registerPC++;
                } else if (byteInstruction == 0xD5)                                                                         // push de
                {
                    registerSP--;
                    RAM[registerSP] = registerD;
                    registerSP--;
                    RAM[registerSP] = registerE;
                    registerPC++;
                } else if (byteInstruction == 0xE5)                                                                         // push hl
                {
                    registerSP--;
                    RAM[registerSP] = registerH;
                    registerSP--;
                    RAM[registerSP] = registerL;
                    registerPC++;
                } else if (byteInstruction == 0xC9)                                                                         // ret
                {
                    UInt16 address;
                    address = RAM[registerSP];
                    registerSP++;
                    address += (UInt16)(RAM[registerSP] * 0x0100);
                    registerSP++;
                    registerPC = address;
                } else if (byteInstruction == 0xD8)                                                                         // ret c
                {
                    if (flagC)
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                    }
                } else if (byteInstruction == 0xF8)                                                                         // ret m
                {
                    if (flagS)
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                    }
                } else if (byteInstruction == 0xD0)                                                                         // ret nc
                {
                    if (flagC)
                    {
                        registerPC++;
                    } else
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xC0)                                                                         // ret nz
                {
                    if (flagZ)
                    {
                        registerPC++;
                    } else
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xF0)                                                                         // ret p
                {
                    if (flagS)
                    {
                        registerPC++;
                    } else
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xE8)                                                                         // ret pe
                {
                    if (flagPV)
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                    }
                } else if (byteInstruction == 0xE0)                                                                         // ret po
                {
                    if (flagPV)
                    {
                        registerPC++;
                    } else
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    }
                } else if (byteInstruction == 0xC8)                                                                         // ret z
                {
                    if (flagZ)
                    {
                        UInt16 address;
                        address = RAM[registerSP];
                        registerSP++;
                        address += (UInt16)(RAM[registerSP] * 0x0100);
                        registerSP++;
                        registerPC = address;
                    } else
                    {
                        registerPC++;
                    }
                } else if (byteInstruction == 0x17)                                                                         // rla 
                {
                    byte ac = registerA;
                    bool saveC = flagC;
                    flagC = (registerA & 0x80) != 0 ? true : false;
                    ac = (byte)(ac << 1);
                    if (saveC) ac = (byte)(ac | 0x01);
                    registerA = ac;
                    flagN = false;
                    flagH = false;
                    registerPC++;
                } else if (byteInstruction == 0x07)                                                                         // rlca
                {
                    flagC = (registerA & 0x80) != 0 ? true : false;
                    registerA = (byte)(registerA << 1);
                    if (flagC) registerA = (byte)(registerA | 0x01);
                    flagN = false;
                    flagH = false;
                    registerPC++;
                } else if (byteInstruction == 0x1F)                                                                         // rra   
                {
                    byte ac = registerA;
                    bool saveC = flagC;
                    flagC = (registerA & 0x01) != 0 ? true : false;
                    ac = (byte)(ac >> 1);
                    if (saveC) ac = (byte)(ac | 0x80);
                    registerA = ac;
                    flagN = false;
                    flagH = false;
                    registerPC++;
                } else if (byteInstruction == 0x0F)                                                                         // rrca
                {
                    flagC = (registerA & 0x01) != 0 ? true : false;
                    registerA = (byte)(registerA >> 1);
                    if (flagC) registerA = (byte)(registerA | 0x80);
                    flagN = false;
                    flagH = false;
                    registerPC++;
                } else if (byteInstruction == 0xC7)                                                                         // rst 00h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0000;
                } else if (byteInstruction == 0xCF)                                                                         // rst 08h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0008;
                } else if (byteInstruction == 0xD7)                                                                         // rst 10h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0010;
                } else if (byteInstruction == 0xDF)                                                                         // rst 18h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0018;
                } else if (byteInstruction == 0xE7)                                                                         // rst 20h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0020;
                } else if (byteInstruction == 0xEF)                                                                         // rst 28h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0028;
                } else if (byteInstruction == 0xF7)                                                                         // rst 30h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0030;
                } else if (byteInstruction == 0xFF)                                                                         // rst 38h
                {
                    registerPC++;
                    Get2ByteFromInt(registerPC, out lo, out hi);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(hi, 16);
                    registerSP--;
                    RAM[registerSP] = Convert.ToByte(lo, 16);
                    registerPC = 0x0038;
                } else if ((byteInstruction >= 0x98) && (byteInstruction <= 0x9F))                                          // sbc a,r
                {
                    num = byteInstruction - 0x98;
                    result = GetRegisterValue((byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, (byte)(val), OPERATOR.SBC);
                    registerPC++;
                } else if (byteInstruction == 0xDE)                                                                         // sbc a,n
                {
                    registerPC++;
                    registerA = Calculate(registerA, (byte)(RAM[registerPC]), OPERATOR.SBC);
                    registerPC++;
                } else if (byteInstruction == 0x37)                                                                         // scf
                {
                    flagH = false;
                    flagC = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction >= 0x90) && (byteInstruction <= 0x97))                                          // sub r
                {
                    num = byteInstruction - 0x90;
                    result = GetRegisterValue((byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.SUB);
                    registerPC++;
                } else if (byteInstruction == 0xD6)                                                                         // sub n
                {
                    registerPC++;
                    registerA = Calculate(registerA, RAM[registerPC], OPERATOR.SUB);
                    registerPC++;
                } else if ((byteInstruction >= 0xA8) && (byteInstruction <= 0xAF))                                          // xor r
                {
                    num = byteInstruction - 0xA8;
                    result = GetRegisterValue((byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.XOR);
                    registerPC++;
                } else if (byteInstruction == 0xEE)                                                                         // xor n
                {
                    registerPC++;
                    registerA = Calculate(registerA, RAM[registerPC], OPERATOR.XOR);
                    registerPC++;
                } else if (byteInstruction == 0xDD)                                                                         // to IX
                {
                    registerPC++;
                    string message = RunInstructionIXIY(ref registerIX);
                    if (message != "") return (message);
                } else if (byteInstruction == 0xFD)                                                                         // to IY
                {
                    registerPC++;
                    string message = RunInstructionIXIY(ref registerIY);
                    if (message != "") return (message);
                } else if (byteInstruction == 0xED)                                                                         // to Misc
                {
                    registerPC++;
                    string message = RunInstructionMisc();
                    if (message != "") return (message);
                } else if (byteInstruction == 0xCB)                                                                         // to Bit
                {
                    registerPC++;
                    string message = RunInstructionBit();
                    if (message != "") return (message);
                } else
                {
                    return ("Unknown instruction '" + byteInstruction.ToString("X2") + "'");
                }
            } catch (Exception exception)
            {
                return ("Exception at memory location: " + registerPC.ToString("X") + ":\r\n" + exception.Message);
            }

            nextAddress = registerPC;

            return "";
        }

        #endregion

        #region Methods (RunInstruction IX/IY)

        /// <summary>
        /// Run IX/IY instruction (at programcounter)
        /// </summary>
        /// <returns></returns>
        public string RunInstructionIXIY(ref UInt16 registerIndex)
        {
            int num;
            bool result;
            byte val = 0x00;

            byteInstruction = RAM[registerPC];

            try
            {
                if ((byteInstruction >= 0x88) && (byteInstruction <= 0x8F))                                                 // adc a,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x8E)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x88;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, (byte)(val), OPERATOR.ADC);
                    registerPC++;
                } else if ((byteInstruction >= 0x80) && (byteInstruction <= 0x87))                                          // add a,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x86)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x80;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.ADD);
                    registerPC++;
                } else if (byteInstruction == 0x09)                                                                         // add ix,bc 
                {
                    UInt16 value1 = registerIndex;
                    UInt16 value2 = (UInt16)(0x0100 * registerB + registerC);
                    registerIndex = Calculate(value1, value2, OPERATOR.ADD);
                    registerPC++;
                } else if (byteInstruction == 0x19)                                                                         // add ix,de 
                {
                    UInt16 value1 = registerIndex;
                    UInt16 value2 = (UInt16)(0x0100 * registerD + registerE);
                    registerIndex = Calculate(value1, value2, OPERATOR.ADD);
                    registerPC++;
                } else if (byteInstruction == 0x29)                                                                         // add ix,ix 
                {
                    UInt16 value1 = registerIndex;
                    UInt16 value2 = registerIndex;
                    registerIndex = Calculate(value1, value2, OPERATOR.ADD);
                    registerPC++;
                } else if (byteInstruction == 0x39)                                                                         // add ix,sp 
                {
                    UInt16 value1 = registerIndex;
                    UInt16 value2 = registerSP;
                    registerIndex = Calculate(value1, value2, OPERATOR.ADD);
                    registerPC++;
                } else if ((byteInstruction >= 0xA0) && (byteInstruction <= 0xA7))                                          // and r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0xA6)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0xA0;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)num, ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.AND);
                    registerPC++;
                } else if ((byteInstruction >= 0xB8) && (byteInstruction <= 0xBF))                                          // cp r  
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0xBE)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0xB8;
                    byte compareValue = 0x00;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)num, ref compareValue);
                    if (!result) return ("Can't get the register value");
                    Calculate(registerA, compareValue, OPERATOR.SUB);
                    registerPC++;
                } else if (byteInstruction == 0x2B)                                                                         // dec ix
                {
                    registerIndex--;
                    registerPC++;
                } else if (byteInstruction == 0x25)                                                                         // dec ixh
                {
                    bool save_flag = flagC;
                    byte value1 = (byte)(registerIndex >> 8);
                    byte value2 = 1;
                    val = Calculate(value1, value2, OPERATOR.SUB);
                    registerIndex = (UInt16)((registerIndex & 0x00FF) + (val << 8));
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x2D)                                                                         // dec ixl
                {
                    bool save_flag = flagC;
                    byte value1 = (byte)(registerIndex);
                    byte value2 = 1;
                    val = Calculate(value1, value2, OPERATOR.SUB);
                    registerIndex = (UInt16)((registerIndex & 0xFF00) + val);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x35)                                                                         // dec (ix+o)
                {
                    bool save_flag = flagC;
                    registerPC++;
                    byte offset = RAM[registerPC];
                    UInt16 address = (UInt16)(registerIndex + (offset < 0x80 ? offset : offset - 0x100));
                    byte value1 = RAM[address];
                    byte value2 = 1;
                    RAM[address] = Calculate(value1, value2, OPERATOR.SUB);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0xE3)                                                                         // ex (sp),ix
                {
                    byte b1, b2;
                    b1 = (byte)registerIndex;
                    b2 = (byte)(registerIndex >> 8);
                    registerIndex = RAM[registerSP];
                    RAM[registerSP] = b1;
                    registerIndex += (UInt16)(RAM[registerSP + 1] << 8);
                    RAM[registerSP + 1] = b2;
                    registerPC++;
                } else if (byteInstruction == 0x23)                                                                         // inc ix
                {
                    registerIndex++;
                    registerPC++;
                } else if (byteInstruction == 0x24)                                                                         // inc ixh
                {
                    bool save_flag = flagC;
                    byte value1 = (byte)(registerIndex >> 8);
                    byte value2 = 1;
                    val = Calculate(value1, value2, OPERATOR.ADD);
                    registerIndex = (UInt16)((registerIndex & 0x00FF) + (val << 8));
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x2C)                                                                         // inc ixl
                {
                    bool save_flag = flagC;
                    byte value1 = (byte)(registerIndex);
                    byte value2 = 1;
                    val = Calculate(value1, value2, OPERATOR.ADD);
                    registerIndex = (UInt16)((registerIndex & 0xFF00) + val);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0x34)                                                                         // inc (ix+o)
                {
                    bool save_flag = flagC;
                    registerPC++;
                    byte offset = RAM[registerPC];
                    UInt16 address = (UInt16)(registerIndex + (offset < 0x80 ? offset : offset - 0x100));
                    byte value1 = RAM[address];
                    byte value2 = 1;
                    RAM[address] = Calculate(value1, value2, OPERATOR.ADD);
                    flagC = save_flag;
                    registerPC++;
                } else if (byteInstruction == 0xE9)                                                                         // jp (ix)
                {
                    registerPC = registerIndex;
                } else if ((byteInstruction >= 0x78) && (byteInstruction <= 0x7F))                                          // ld a,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x7E)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x78;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = val;
                    registerPC++;
                } else if ((byteInstruction >= 0x40) && (byteInstruction <= 0x47))                                          // ld b,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x46)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x40;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerB = val;
                    registerPC++;
                } else if ((byteInstruction >= 0x48) && (byteInstruction <= 0x4F))                                          // ld c,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x4E)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x48;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerC = val;
                    registerPC++;
                } else if ((byteInstruction >= 0x50) && (byteInstruction <= 0x57))                                          // ld d,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x56)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x50;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerD = val;
                    registerPC++;
                } else if ((byteInstruction >= 0x58) && (byteInstruction <= 0x5F))                                          // ld e,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x5E)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x58;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerE = val;
                    registerPC++;
                } else if (byteInstruction == 0x66)                                                                         // ld h,(ix+o)
                {
                    registerPC++;
                    byte offset = RAM[registerPC];
                    registerH = RAM[registerIndex + offset];
                    registerPC++;
                } else if (byteInstruction == 0x21)                                                                         // ld ix,nn
                {
                    byte b1, b2;
                    registerPC++;
                    b1 = RAM[registerPC];
                    registerPC++;
                    b2 = RAM[registerPC];
                    registerPC++;
                    registerIndex = (UInt16)(b1 + (0x0100 * b2));
                } else if (byteInstruction == 0x2A)                                                                         // ld ix,(nn)
                {
                    UInt16 address = 0;
                    registerPC++;
                    address = RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerPC++;
                    byte b1, b2;
                    b1 = RAM[address];
                    address++;
                    b2 = RAM[address];
                    registerIndex = (UInt16)(b1 + (0x0100 * b2));
                } else if (byteInstruction == 0x26)                                                                         // ld ixh,n
                {
                    registerPC++;
                    byte n = RAM[registerPC];
                    registerIndex = (UInt16)((registerIndex & 0x00FF) + (n * 0x100));
                    registerPC++;
                } else if (byteInstruction == 0x2E)                                                                         // ld ixl,n
                {
                    registerPC++;
                    byte n = RAM[registerPC];
                    registerIndex = (UInt16)((registerIndex & 0xFF00) + n);
                    registerPC++;
                } else if ((byteInstruction == 0x60) ||                                                                     // ld ixh,r
                           (byteInstruction == 0x61) ||
                           (byteInstruction == 0x62) ||
                           (byteInstruction == 0x63) ||
                           (byteInstruction == 0x64) ||
                           (byteInstruction == 0x65) ||
                           (byteInstruction == 0x67))
                {
                    num = byteInstruction - 0x60;
                    result = GetRegisterValue(ref registerIndex, 0x00, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerIndex = (UInt16)((registerIndex & 0x00FF) | (val << 8));
                    registerPC++;
                } else if ((byteInstruction == 0x68) ||                                                                     // ld ixl,r
                           (byteInstruction == 0x69) ||
                           (byteInstruction == 0x6A) ||
                           (byteInstruction == 0x6B) ||
                           (byteInstruction == 0x6C) ||
                           (byteInstruction == 0x6D) ||
                           (byteInstruction == 0x6F))
                {
                    num = byteInstruction - 0x68;
                    result = GetRegisterValue(ref registerIndex, 0x00, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerIndex = (UInt16)((registerIndex & 0xFF00) | val);
                    registerPC++;
                } else if (byteInstruction == 0x6E)                                                                         // ld l,(ix+o)
                {
                    registerPC++;
                    byte offset = RAM[registerPC];
                    registerL = RAM[registerIndex + offset];
                    registerPC++;
                } else if (byteInstruction == 0x36)                                                                         // ld (ix+o),n
                {
                    registerPC++;
                    byte offset = RAM[registerPC];
                    UInt16 address = (UInt16)(registerIndex + (offset < 0x80 ? offset : offset - 0x100));
                    registerPC++;
                    RAM[address] = RAM[registerPC];
                    registerPC++;
                } else if ((byteInstruction == 0x70) ||                                                                     // ld (ix+o),r
                           (byteInstruction == 0x71) ||
                           (byteInstruction == 0x72) ||
                           (byteInstruction == 0x73) ||
                           (byteInstruction == 0x74) ||
                           (byteInstruction == 0x75) ||
                           (byteInstruction == 0x77))
                {
                    registerPC++;
                    byte offset = RAM[registerPC];
                    UInt16 address = (UInt16)(registerIndex + (offset < 0x80 ? offset : offset - 0x100));
                    num = byteInstruction - 0x40;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    RAM[address] = val;
                    registerPC++;
                } else if (byteInstruction == 0x22)                                                                         // ld (nn),ix
                {
                    UInt16 address = 0;
                    registerPC++;
                    address = RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerPC++;
                    RAM[address] = (byte)(registerIndex);
                    address++;
                    RAM[address] = (byte)(registerIndex >> 8);
                } else if (byteInstruction == 0xF9)                                                                         // ld sp,ix
                {
                    registerSP = registerIndex;
                    registerPC++;
                } else if ((byteInstruction >= 0xB0) && (byteInstruction <= 0xB7))                                          // or r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0xB6)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0xB0;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.OR);
                    registerPC++;
                } else if (byteInstruction == 0xE1)                                                                         // pop ix
                {
                    registerIndex = RAM[registerSP]; 
                    registerSP++;
                    registerIndex += (UInt16)(0x0100 * RAM[registerSP]);
                    registerSP++;
                    registerPC++;
                } else if (byteInstruction == 0xE5)                                                                         // push ix
                {
                    registerSP--;
                    RAM[registerSP] = (byte)(registerIndex >> 8);
                    registerSP--;
                    RAM[registerSP] = (byte)(registerIndex & 0x00FF);
                    registerPC++;
                } else if ((byteInstruction >= 0x98) && (byteInstruction <= 0x9F))                                          // sbc a,r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x9E)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x98;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, (byte)(val), OPERATOR.SBC);
                    registerPC++;
                } else if ((byteInstruction >= 0x90) && (byteInstruction <= 0x97))                                          // sub r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0x96)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0x90;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.SUB);
                    registerPC++;
                } else if ((byteInstruction >= 0xA8) && (byteInstruction <= 0xAF))                                          // xor r
                {
                    byte offset = 0x00;
                    if (byteInstruction == 0xAE)
                    {
                        registerPC++;
                        offset = RAM[registerPC];
                    }
                    num = byteInstruction - 0xA8;
                    result = GetRegisterValue(ref registerIndex, offset, (byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    registerA = Calculate(registerA, val, OPERATOR.XOR);
                    registerPC++;
                } else if (byteInstruction == 0xCB)                                                                         // to IX/IY-Bit
                {
                    registerPC++;
                    string message = RunInstructionIXIYBit(ref registerIndex);
                    if (message != "") return (message);
                } else
                {
                    if (registerIndex == registerIX) return ("Unknown IX instruction 'DD" + byteInstruction.ToString("X2") + "'");
                    if (registerIndex == registerIY) return ("Unknown IX instruction 'FD" + byteInstruction.ToString("X2") + "'");
                }
            } catch (Exception exception)
            {
                return ("Exception at memory location: " + registerPC.ToString("X") + ":\r\n" + exception.Message);
            }

            return "";
        }

        #endregion

        #region Methods (RunInstruction IX/IY Bit)

        /// <summary>
        /// Run IX/IY-Bit instruction (at programcounter)
        /// </summary>
        /// <returns></returns>
        public string RunInstructionIXIYBit(ref UInt16 registerIndex)
        {
            int num;
            bool result;
            byte offset = RAM[registerPC];
            registerPC++;
            byteInstruction = RAM[registerPC];
            UInt16 address = (UInt16)(registerIndex + (offset < 0x80 ? offset : offset - 0x100));

            try
            {
                if (byteInstruction == 0x46)                                                                                // bit 0,(ix+o)
                {
                    result = (RAM[address] & 0b00000001) == 0b00000001;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0x4E)                                                                         // bit 1,(ix+o)
                {
                    result = (RAM[address] & 0b00000010) == 0b00000010;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0x56)                                                                         // bit 2,(ix+o)
                {
                    result = (RAM[address] & 0b00000100) == 0b00000100;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0x5E)                                                                         // bit 3,(ix+o)
                {
                    result = (RAM[address] & 0b00001000) == 0b00001000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0x66)                                                                         // bit 4,(ix+o)
                {
                    result = (RAM[address] & 0b00010000) == 0b00010000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0x6E)                                                                         // bit 5,(ix+o)
                {
                    result = (RAM[address] & 0b00100000) == 0b00100000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0x76)                                                                         // bit 6,(ix+o)
                {
                    result = (RAM[address] & 0b01000000) == 0b01000000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0x7E)                                                                         // bit 7,(ix+o)
                {
                    result = (RAM[address] & 0b10000000) == 0b10000000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x80) ||                                                                     // res 0,(ix+o)
                           (byteInstruction == 0x81) ||
                           (byteInstruction == 0x82) ||
                           (byteInstruction == 0x83) ||
                           (byteInstruction == 0x84) ||
                           (byteInstruction == 0x85) ||
                           (byteInstruction == 0x86) ||
                           (byteInstruction == 0x87))
                {
                    RAM[address] = (byte)(RAM[address] & 0b11111110);
                    if (byteInstruction != 0x86)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x80) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x88) ||                                                                     // res 1,(ix+o)
                           (byteInstruction == 0x89) ||
                           (byteInstruction == 0x8A) ||
                           (byteInstruction == 0x8B) ||
                           (byteInstruction == 0x8C) ||
                           (byteInstruction == 0x8D) ||
                           (byteInstruction == 0x8E) ||
                           (byteInstruction == 0x8F))
                {
                    RAM[address] = (byte)(RAM[address] & 0b11111101);
                    if (byteInstruction != 0x8E)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x88) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x90) ||                                                                     // res 2,(ix+o)
                           (byteInstruction == 0x91) ||
                           (byteInstruction == 0x92) ||
                           (byteInstruction == 0x93) ||
                           (byteInstruction == 0x94) ||
                           (byteInstruction == 0x95) ||
                           (byteInstruction == 0x96) ||
                           (byteInstruction == 0x97))
                {
                    RAM[address] = (byte)(RAM[address] & 0b11111011);
                    if (byteInstruction != 0x96)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x90) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x98) ||                                                                     // res 3,(ix+o)
                           (byteInstruction == 0x99) ||
                           (byteInstruction == 0x9A) ||
                           (byteInstruction == 0x9B) ||
                           (byteInstruction == 0x9C) ||
                           (byteInstruction == 0x9D) ||
                           (byteInstruction == 0x9E) ||
                           (byteInstruction == 0x9F))
                {
                    RAM[address] = (byte)(RAM[address] & 0b11110111);
                    if (byteInstruction != 0x9E)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x98) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xA0) ||                                                                     // res 4,(ix+o)
                           (byteInstruction == 0xA1) ||
                           (byteInstruction == 0xA2) ||
                           (byteInstruction == 0xA3) ||
                           (byteInstruction == 0xA4) ||
                           (byteInstruction == 0xA5) ||
                           (byteInstruction == 0xA6) ||
                           (byteInstruction == 0xA7))
                {
                    RAM[address] = (byte)(RAM[address] & 0b11101111);
                    if (byteInstruction != 0xA6)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xA0) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xA8) ||                                                                     // res 5,(ix+o)
                           (byteInstruction == 0xA9) ||
                           (byteInstruction == 0xAA) ||
                           (byteInstruction == 0xAB) ||
                           (byteInstruction == 0xAC) ||
                           (byteInstruction == 0xAD) ||
                           (byteInstruction == 0xAE) ||
                           (byteInstruction == 0xAF))
                {
                    RAM[address] = (byte)(RAM[address] & 0b11011111);
                    if (byteInstruction != 0xAE)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xA8) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xB0) ||                                                                     // res 6,(ix+o)
                           (byteInstruction == 0xB1) ||
                           (byteInstruction == 0xB2) ||
                           (byteInstruction == 0xB3) ||
                           (byteInstruction == 0xB4) ||
                           (byteInstruction == 0xB5) ||
                           (byteInstruction == 0xB6) ||
                           (byteInstruction == 0xB7))
                {
                    RAM[address] = (byte)(RAM[address] & 0b10111111);
                    if (byteInstruction != 0xB6)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xB0) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xB8) ||                                                                     // res 7,(ix+o)
                           (byteInstruction == 0xB9) ||
                           (byteInstruction == 0xBA) ||
                           (byteInstruction == 0xBB) ||
                           (byteInstruction == 0xBC) ||
                           (byteInstruction == 0xBD) ||
                           (byteInstruction == 0xBE) ||
                           (byteInstruction == 0xBF))
                {
                    RAM[address] = (byte)(RAM[address] & 0b01111111);
                    if (byteInstruction != 0xBE)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xB8) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x10) ||                                                                     // rl (ix+o)
                           (byteInstruction == 0x11) ||
                           (byteInstruction == 0x12) ||
                           (byteInstruction == 0x13) ||
                           (byteInstruction == 0x14) ||
                           (byteInstruction == 0x15) ||
                           (byteInstruction == 0x16) ||
                           (byteInstruction == 0x17))
                {
                    RAM[address] = RotateShift(RAM[address], OPERATOR.RL);
                    if (byteInstruction != 0x16)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x10) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x00) ||                                                                     // rlc (ix+o)
                           (byteInstruction == 0x01) ||
                           (byteInstruction == 0x02) ||
                           (byteInstruction == 0x03) ||
                           (byteInstruction == 0x04) ||
                           (byteInstruction == 0x05) ||
                           (byteInstruction == 0x06) ||
                           (byteInstruction == 0x07))
                { 
                    RAM[address] = RotateShift(RAM[address], OPERATOR.RLC);
                    if (byteInstruction != 0x06)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)(num & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value"); 
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x18) ||                                                                      // rr (ix+o)
                           (byteInstruction == 0x19) ||
                           (byteInstruction == 0x1A) ||
                           (byteInstruction == 0x1B) ||
                           (byteInstruction == 0x1C) ||
                           (byteInstruction == 0x1D) ||
                           (byteInstruction == 0x1E) ||
                           (byteInstruction == 0x1F))
                {
                    RAM[address] = RotateShift(RAM[address], OPERATOR.RR);
                    if (byteInstruction != 0x1E)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x18) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x08) ||                                                                     // rrc (ix+o)
                           (byteInstruction == 0x09) ||
                           (byteInstruction == 0x0A) ||
                           (byteInstruction == 0x0B) ||
                           (byteInstruction == 0x0C) ||
                           (byteInstruction == 0x0D) ||
                           (byteInstruction == 0x0E) ||
                           (byteInstruction == 0x0F))
                {
                    RAM[address] = RotateShift(RAM[address], OPERATOR.RRC);
                    if (byteInstruction != 0x0E)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x08) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x20) ||                                                                     // sla (ix+o)
                           (byteInstruction == 0x21) ||
                           (byteInstruction == 0x22) ||
                           (byteInstruction == 0x23) ||
                           (byteInstruction == 0x24) ||
                           (byteInstruction == 0x25) ||
                           (byteInstruction == 0x26) ||
                           (byteInstruction == 0x27))
                {
                    RAM[address] = RotateShift(RAM[address], OPERATOR.SLA);
                    if (byteInstruction != 0x26)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x20) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x30) ||                                                                     // sll (ix+o)
                           (byteInstruction == 0x31) ||
                           (byteInstruction == 0x32) ||
                           (byteInstruction == 0x33) ||
                           (byteInstruction == 0x34) ||
                           (byteInstruction == 0x35) ||
                           (byteInstruction == 0x36) ||
                           (byteInstruction == 0x37))
                {
                    RAM[address] = RotateShift(RAM[address], OPERATOR.SLL);
                    if (byteInstruction != 0x36)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x30) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x28) ||                                                                     // sra (ix+o)
                           (byteInstruction == 0x29) ||
                           (byteInstruction == 0x2A) ||
                           (byteInstruction == 0x2B) ||
                           (byteInstruction == 0x2C) ||
                           (byteInstruction == 0x2D) ||
                           (byteInstruction == 0x2E) ||
                           (byteInstruction == 0x2F))
                {
                    RAM[address] = RotateShift(RAM[address], OPERATOR.SRA);
                    if (byteInstruction != 0x2E)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x28) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x38) ||                                                                     // srl (ix+o)
                           (byteInstruction == 0x39) ||
                           (byteInstruction == 0x3A) ||
                           (byteInstruction == 0x3B) ||
                           (byteInstruction == 0x3C) ||
                           (byteInstruction == 0x3D) ||
                           (byteInstruction == 0x3E) ||
                           (byteInstruction == 0x3F))
                {
                    RAM[address] = RotateShift(RAM[address], OPERATOR.SRL);
                    if (byteInstruction != 0x3E)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0x38) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xC0) ||                                                                     // set 0,(ix+o)
                           (byteInstruction == 0xC1) ||
                           (byteInstruction == 0xC2) ||
                           (byteInstruction == 0xC3) ||
                           (byteInstruction == 0xC4) ||
                           (byteInstruction == 0xC5) ||
                           (byteInstruction == 0xC6) ||
                           (byteInstruction == 0xC7))
                {
                    RAM[address] = (byte)(RAM[address] | 0b00000001);
                    if (byteInstruction != 0xC6)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xC0) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xC8) ||                                                                     // set 1,(ix+o)
                           (byteInstruction == 0xC9) ||
                           (byteInstruction == 0xCA) ||
                           (byteInstruction == 0xCB) ||
                           (byteInstruction == 0xCC) ||
                           (byteInstruction == 0xCD) ||
                           (byteInstruction == 0xCE) ||
                           (byteInstruction == 0xCF))
                {
                    RAM[address] = (byte)(RAM[address] | 0b00000010);
                    if (byteInstruction != 0xCE)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xC8) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xD0) ||                                                                     // set 2,(ix+o)
                           (byteInstruction == 0xD1) ||
                           (byteInstruction == 0xD2) ||
                           (byteInstruction == 0xD3) ||
                           (byteInstruction == 0xD4) ||
                           (byteInstruction == 0xD5) ||
                           (byteInstruction == 0xD6) ||
                           (byteInstruction == 0xD7))
                {
                    RAM[address] = (byte)(RAM[address] | 0b00000100);
                    if (byteInstruction != 0xD6)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xD0) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xD8) ||                                                                     // set 3,(ix+o)
                           (byteInstruction == 0xD9) ||
                           (byteInstruction == 0xDA) ||
                           (byteInstruction == 0xDB) ||
                           (byteInstruction == 0xDC) ||
                           (byteInstruction == 0xDD) ||
                           (byteInstruction == 0xDE) ||
                           (byteInstruction == 0xDF))
                {
                    RAM[address] = (byte)(RAM[address] | 0b00001000);
                    if (byteInstruction != 0xDE)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xD8) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xE0) ||                                                                     // set 4,(ix+o)
                           (byteInstruction == 0xE1) ||
                           (byteInstruction == 0xE2) ||
                           (byteInstruction == 0xE3) ||
                           (byteInstruction == 0xE4) ||
                           (byteInstruction == 0xE5) ||
                           (byteInstruction == 0xE6) ||
                           (byteInstruction == 0xE7))
                {
                    RAM[address] = (byte)(RAM[address] | 0b00010000);
                    if (byteInstruction != 0xE6)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xE0) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xE8) ||                                                                     // set 5,(ix+o)
                           (byteInstruction == 0xE9) ||
                           (byteInstruction == 0xEA) ||
                           (byteInstruction == 0xEB) ||
                           (byteInstruction == 0xEC) ||
                           (byteInstruction == 0xED) ||
                           (byteInstruction == 0xEE) ||
                           (byteInstruction == 0xEF))
                {
                    RAM[address] = (byte)(RAM[address] | 0b00100000);
                    if (byteInstruction != 0xEE)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xE8) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xF0) ||                                                                     // set 6,(ix+o)
                           (byteInstruction == 0xF1) ||
                           (byteInstruction == 0xF2) ||
                           (byteInstruction == 0xF3) ||
                           (byteInstruction == 0xF4) ||
                           (byteInstruction == 0xF5) ||
                           (byteInstruction == 0xF6) ||
                           (byteInstruction == 0xF7))
                {
                    RAM[address] = (byte)(RAM[address] | 0b01000000);
                    if (byteInstruction != 0xF6)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xF0) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else if ((byteInstruction == 0xF8) ||                                                                     // set 7,(ix+o)
                           (byteInstruction == 0xF9) ||
                           (byteInstruction == 0xFA) ||
                           (byteInstruction == 0xFB) ||
                           (byteInstruction == 0xFC) ||
                           (byteInstruction == 0xFD) ||
                           (byteInstruction == 0xFE) ||
                           (byteInstruction == 0xFF))
                {
                    RAM[address] = (byte)(RAM[address] | 0b10000000);
                    if (byteInstruction != 0xFE)
                    {
                        num = byteInstruction;
                        result = SetRegisterValue((byte)((num - 0xF8) & 0x07), RAM[address]);
                        if (!result) return ("Can't set the register value");
                    }
                    registerPC++;
                } else
                {
                    if (registerIndex == registerIX) return ("Unknown IX-Bit instruction 'DDCB" + byteInstruction.ToString("X2") + "'");
                    if (registerIndex == registerIY) return ("Unknown IY-Bit instruction 'FDCB" + byteInstruction.ToString("X2") + "'");
                }
            } catch (Exception exception)
            {
                return ("Exception at memory location: " + registerPC.ToString("X") + ":\r\n" + exception.Message);
            }

            return "";
        }

        #endregion

        #region Methods (RunInstructionMisc)

        /// <summary>
        /// Run Miscellaneous instruction (at programcounter)
        /// </summary>
        /// <returns></returns>
        public string RunInstructionMisc()
        {
            int num;
            bool result;
            byte val = 0x00;
            string lo, hi;
            byteInstruction = RAM[registerPC];

            try
            {
                if ((byteInstruction == 0x40) ||                                                                            // in r,(c)
                    (byteInstruction == 0x48) ||
                    (byteInstruction == 0x50) ||
                    (byteInstruction == 0x58) ||
                    (byteInstruction == 0x60) ||
                    (byteInstruction == 0x68) ||
                    (byteInstruction == 0x78))
                {
                    num = byteInstruction - 0x40;
                    if (registerC == 0x10)
                    {
                        // Update the Port and register from the compact flash card data
                        UpdateCompactFlashRead = true;
                    } else
                    {
                        SetRegisterValue((byte)((num >> 3) & 0x07), PORT[registerC]);
                        SetFlags(PORT[registerC]);
                    }
                    registerPC++;
                } else if ((byteInstruction == 0x41) ||                                                                    // out (c),r
                           (byteInstruction == 0x49) ||
                           (byteInstruction == 0x51) ||
                           (byteInstruction == 0x59) ||
                           (byteInstruction == 0x61) ||
                           (byteInstruction == 0x69) ||
                           (byteInstruction == 0x79))
                {
                    num = byteInstruction - 0x41;
                    result = GetRegisterValue((byte)((num >> 3) & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    PORT[registerC] = val;
                    CheckOutPorts(registerC);
                    registerPC++;
                } else if (byteInstruction == 0x42)                                                                         // sbc hl,bc
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(0x0100 * registerB + registerC);
                    UInt16 value = Calculate(value1, value2, OPERATOR.SBC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x52)                                                                         // sbc hl,de
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(0x0100 * registerD + registerE);
                    UInt16 value = Calculate(value1, value2, OPERATOR.SBC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x62)                                                                         // sbc hl,hl
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value = Calculate(value1, value2, OPERATOR.SBC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x72)                                                                         // sbc hl,sp
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(registerSP);
                    UInt16 value = Calculate(value1, value2, OPERATOR.SBC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x4A)                                                                         // adc hl,bc
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(0x0100 * registerB + registerC);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x5A)                                                                         // adc hl,de
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(0x0100 * registerD + registerE);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x6A)                                                                         // adc hl,hl
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x7A)                                                                         // adc hl,sp
                {
                    UInt16 value1 = (UInt16)(0x0100 * registerH + registerL);
                    UInt16 value2 = (UInt16)(registerSP);
                    UInt16 value = Calculate(value1, value2, OPERATOR.ADC);
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    registerPC++;
                } else if (byteInstruction == 0x43)                                                                         // ld (nn),bc
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    RAM[address  ] = registerC;
                    RAM[address+1] = registerB;
                    registerPC++;
                } else if (byteInstruction == 0x53)                                                                         // ld (nn),de
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    RAM[address    ] = registerE;
                    RAM[address + 1] = registerD;
                    registerPC++;
                } else if (byteInstruction == 0x63)                                                                         // ld (nn),hl
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    RAM[address    ] = registerL;
                    RAM[address + 1] = registerH;
                    registerPC++;
                } else if (byteInstruction == 0x73)                                                                         // ld (nn),sp
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    RAM[address    ] = (byte)(registerSP & 0x00FF);
                    RAM[address + 1] = (byte)(registerSP >> 8);
                    registerPC++;
                } else if (byteInstruction == 0x4B)                                                                         // ld bc,(nn)
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerC = RAM[address];
                    registerB = RAM[address + 1];
                    registerPC++;
                } else if (byteInstruction == 0x5B)                                                                         // ld de,(nn)
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerE = RAM[address];
                    registerD = RAM[address + 1];
                    registerPC++;
                } else if (byteInstruction == 0x6B)                                                                         // ld hl,(nn)
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerL = RAM[address];
                    registerH = RAM[address + 1];
                    registerPC++;
                } else if (byteInstruction == 0x7B)                                                                         // ld sp,(nn)
                {
                    UInt16 address = 0;
                    registerPC++;
                    address += RAM[registerPC];
                    registerPC++;
                    address += (UInt16)(0x0100 * RAM[registerPC]);
                    registerSP = RAM[address];
                    registerSP += (UInt16)(RAM[address + 1] * 0x100);
                    registerPC++;
                } else if (byteInstruction == 0x70)                                                                         // in (c)
                {
                    if (registerC == 0x10)
                    {
                        // Update the Port and registerA from the compact flash card data
                        UpdateCompactFlashRead = true;
                    } else
                    {
                        SetFlags(PORT[registerC]);
                    }
                    registerPC++;
                } else if (byteInstruction == 0x44)                                                                         // neg
                {
                    registerA = Calculate(0, registerA, OPERATOR.SUB);
                    registerPC++;
                } else if (byteInstruction == 0x45)                                                                         // retn
                {
                    UInt16 address;
                    address = RAM[registerSP];
                    registerSP++;
                    address += (UInt16)(RAM[registerSP] * 0x0100);
                    registerSP++;
                    registerPC = address;
                } else if (byteInstruction == 0x4D)                                                                         // reti
                {
                    UInt16 address;
                    address = RAM[registerSP];
                    registerSP++;
                    address += (UInt16)(RAM[registerSP] * 0x0100);
                    registerSP++;
                    registerPC = address;
                } else if (byteInstruction == 0x46)                                                                         // im 0
                {
                    im = IM.im0;
                    registerPC++;
                } else if (byteInstruction == 0x56)                                                                         // im 1
                {
                    im = IM.im1;
                    registerPC++;
                } else if (byteInstruction == 0x5E)                                                                         // im 2
                {
                    im = IM.im2;
                    registerPC++;
                } else if (byteInstruction == 0x47)                                                                         // ld i,a
                {
                    registerI = registerA;
                    IO_INTERRUPT_HANDLER_VECTOR = (IO_INTERRUPT_HANDLER_VECTOR & 0x00FF) + (0x100 * registerI);
                    registerPC++;
                } else if (byteInstruction == 0x57)                                                                         // ld a,i
                {
                    registerA = registerI;
                    registerPC++;
                } else if (byteInstruction == 0x67)                                                                         // rrd
                {
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = (byte)(registerA & 0b00001111);
                    registerA = (byte)((registerA & 0b11110000) | (RAM[address] & 0b00001111));
                    RAM[address] = (byte)((RAM[address] >> 4) | (val << 4));
                    SetFlags(registerA);
                    registerPC++;
                } else if (byteInstruction == 0x4F)                                                                         // ld r,a
                {
                    registerR = registerA;
                    registerPC++;
                } else if (byteInstruction == 0x5F)                                                                         // ld a,r
                {
                    registerA = registerR;
                    registerPC++;
                } else if (byteInstruction == 0x6F)                                                                         // rld
                {
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = (byte)(registerA & 0b00001111);
                    registerA = (byte)((registerA & 0b11110000) | (RAM[address] >> 4));
                    RAM[address] = (byte)((RAM[address] << 4) | val);
                    SetFlags(registerA);
                    registerPC++;
                } else if (byteInstruction == 0xA0)                                                                         // ldi
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    address = (UInt16)(0x0100 * registerD + registerE);
                    RAM[address] = val;
                    value = 0x0100 * registerD + registerE;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerD = (byte)Convert.ToInt32(hi, 16);
                    registerE = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    if ((registerB == 0x00) && (registerC == 0x00)) flagPV = false; else flagPV = true;
                    flagH = false;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0xB0)                                                                         // ldir
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    address = (UInt16)(0x0100 * registerD + registerE);
                    RAM[address] = val;
                    value = 0x0100 * registerD + registerE;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerD = (byte)Convert.ToInt32(hi, 16);
                    registerE = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    flagH = false;
                    flagN = false;
                    if ((registerB == 0x00) && (registerC == 0x00))
                    {
                        flagPV = false;
                        registerPC++;
                    } else
                    {
                        flagPV = true;
                        registerPC--;
                    }
                } else if (byteInstruction == 0xA1)                                                                         // cpi
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    bool flagCold = flagC;
                    Calculate(registerA, val, OPERATOR.SUB);
                    flagC = flagCold;
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    if ((registerB == 0x00) && (registerC == 0x00)) flagPV = false; else flagPV = true;
                    registerPC++;
                } else if (byteInstruction == 0xB1)                                                                         // cpir
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    bool flagCold = flagC;
                    Calculate(registerA, val, OPERATOR.SUB);
                    flagC = flagCold;
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    if ((registerB == 0x00) && (registerC == 0x00))
                    {
                        flagPV = false;
                        registerPC++;
                    } else
                    {
                        flagPV = true;
                        if (!flagZ)
                        {
                            registerPC--;
                        } else
                        {
                            registerPC++;
                        }
                    }
                } else if (byteInstruction == 0xA2)                                                                         // ini
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = PORT[registerC];
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0) flagZ = true; else flagZ = false;
                    registerPC++;
                } else if (byteInstruction == 0xB2)                                                                         // inir
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = PORT[registerC];
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0)
                    {
                        flagZ = true;
                        registerPC++;
                    } else
                    {
                        flagZ = false;
                        registerPC--;
                    }
                } else if (byteInstruction == 0xA3)                                                                         // outi
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    PORT[registerC] = RAM[address];
                    CheckOutPorts(registerC);
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0) flagZ = true; else flagZ = false;
                    registerPC++;
                } else if (byteInstruction == 0xB3)                                                                         // otir
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    PORT[registerC] = RAM[address];
                    value = 0x0100 * registerH + registerL;
                    value += 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0)
                    {
                        flagZ = true;
                        registerPC++;
                    } else
                    {
                        flagZ = false;
                        registerPC--;
                    }
                } else if (byteInstruction == 0xA8)                                                                         // ldd
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    address = (UInt16)(0x0100 * registerD + registerE);
                    RAM[address] = val;
                    value = 0x0100 * registerD + registerE;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerD = (byte)Convert.ToInt32(hi, 16);
                    registerE = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    if ((registerB == 0x00) && (registerC == 0x00)) flagPV = false; else flagPV = true;
                    flagH = false;
                    flagN = false;
                    registerPC++;
                } else if (byteInstruction == 0xB8)                                                                         // lddr
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    address = (UInt16)(0x0100 * registerD + registerE);
                    RAM[address] = val;
                    value = 0x0100 * registerD + registerE;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerD = (byte)Convert.ToInt32(hi, 16);
                    registerE = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    flagH = false;
                    flagN = false;
                    if ((registerB == 0x00) && (registerC == 0x00))
                    {
                        flagPV = false;
                        registerPC++;
                    } else
                    {
                        flagPV = true;
                        registerPC--;
                    }
                } else if (byteInstruction == 0xA9)                                                                         // cpd
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    bool flagCold = flagC;
                    Calculate(registerA, val, OPERATOR.SUB);
                    flagC = flagCold;
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    if ((registerB == 0x00) && (registerC == 0x00)) flagPV = false; else flagPV = true;
                    registerPC++;
                } else if (byteInstruction == 0xB9)                                                                         // cpdr
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    val = RAM[address];
                    bool flagCold = flagC;
                    Calculate(registerA, val, OPERATOR.SUB);
                    flagC = flagCold;
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    value = 0x0100 * registerB + registerC;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerB = (byte)Convert.ToInt32(hi, 16);
                    registerC = (byte)Convert.ToInt32(lo, 16);
                    if ((registerB == 0x00) && (registerC == 0x00))
                    {
                        flagPV = false;
                        registerPC++;
                    } else
                    {
                        flagPV = true;
                        if (!flagZ)
                        {
                            registerPC--;
                        } else
                        {
                            registerPC++;
                        }
                    }
                } else if (byteInstruction == 0xAA)                                                                         // ind
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = PORT[registerC];
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0) flagZ = true; else flagZ = false;
                    registerPC++;
                } else if (byteInstruction == 0xBA)                                                                         // indr
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = PORT[registerC];
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0)
                    {
                        flagZ = true;
                        registerPC++;
                    } else
                    {
                        flagZ = false;
                        registerPC--;
                    }
                } else if (byteInstruction == 0xAB)                                                                         // outd
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    PORT[registerC] = RAM[address];
                    CheckOutPorts(registerC);
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0) flagZ = true; else flagZ = false;
                    registerPC++;
                } else if (byteInstruction == 0xBB)                                                                         // otdr
                {
                    int value;
                    UInt16 address = (UInt16)(0x0100 * registerH + registerL);
                    RAM[address] = PORT[registerC];
                    value = 0x0100 * registerH + registerL;
                    value -= 0x01;
                    Get2ByteFromInt(value, out lo, out hi);
                    registerH = (byte)Convert.ToInt32(hi, 16);
                    registerL = (byte)Convert.ToInt32(lo, 16);
                    flagN = true;
                    registerB--;
                    if (registerB == 0)
                    {
                        flagZ = true;
                        registerPC++;
                    } else
                    {
                        flagZ = false;
                        registerPC--;
                    }
                } else
                {
                    return ("Unknown Miscellaneous instruction 'ED" + byteInstruction.ToString("X2") + "'");
                }
            } catch (Exception exception)
            {
                return ("Exception at memory location: " + registerPC.ToString("X") + ":\r\n" + exception.Message);
            }

            return "";
        }

        #endregion

        #region Methods (RunInstructionBit)

        /// <summary>
        /// Run Bit instruction (at programcounter)
        /// </summary>
        /// <returns></returns>
        public string RunInstructionBit()
        {
            int num;
            bool result;
            byte val = 0x00;
            byteInstruction = RAM[registerPC];

            try
            {
                if ((byteInstruction == 0x00) ||                                                                            // rlc r
                    (byteInstruction == 0x01) ||
                    (byteInstruction == 0x02) ||
                    (byteInstruction == 0x03) ||
                    (byteInstruction == 0x04) ||
                    (byteInstruction == 0x05) ||
                    (byteInstruction == 0x06) ||
                    (byteInstruction == 0x07))
                {
                    num = byteInstruction;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.RLC);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x08) ||                                                                     // rrc r
                           (byteInstruction == 0x09) ||
                           (byteInstruction == 0x0A) ||
                           (byteInstruction == 0x0B) ||
                           (byteInstruction == 0x0C) ||
                           (byteInstruction == 0x0D) ||
                           (byteInstruction == 0x0E) ||
                           (byteInstruction == 0x0F))
                {
                    num = byteInstruction - 0x08;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.RRC);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x10) ||                                                                     // rl r
                           (byteInstruction == 0x11) ||
                           (byteInstruction == 0x12) ||
                           (byteInstruction == 0x13) ||
                           (byteInstruction == 0x14) ||
                           (byteInstruction == 0x15) ||
                           (byteInstruction == 0x16) ||
                           (byteInstruction == 0x17))
                {
                    num = byteInstruction - 0x10;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.RL);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x18) ||                                                                     // rr r
                           (byteInstruction == 0x19) ||
                           (byteInstruction == 0x1A) ||
                           (byteInstruction == 0x1B) ||
                           (byteInstruction == 0x1C) ||
                           (byteInstruction == 0x1D) ||
                           (byteInstruction == 0x1E) ||
                           (byteInstruction == 0x1F))
                {
                    num = byteInstruction - 0x18;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.RR);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x20) ||                                                                     // sla r
                           (byteInstruction == 0x21) ||
                           (byteInstruction == 0x22) ||
                           (byteInstruction == 0x23) ||
                           (byteInstruction == 0x24) ||
                           (byteInstruction == 0x25) ||
                           (byteInstruction == 0x26) ||
                           (byteInstruction == 0x27))
                {
                    num = byteInstruction - 0x20;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.SLA);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x28) ||                                                                     // sra r
                           (byteInstruction == 0x29) ||
                           (byteInstruction == 0x2A) ||
                           (byteInstruction == 0x2B) ||
                           (byteInstruction == 0x2C) ||
                           (byteInstruction == 0x2D) ||
                           (byteInstruction == 0x2E) ||
                           (byteInstruction == 0x2F))
                {
                    num = byteInstruction - 0x28;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.SRA);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x30) ||                                                                     // sll r
                           (byteInstruction == 0x31) ||
                           (byteInstruction == 0x32) ||
                           (byteInstruction == 0x33) ||
                           (byteInstruction == 0x34) ||
                           (byteInstruction == 0x35) ||
                           (byteInstruction == 0x36) ||
                           (byteInstruction == 0x37))
                {
                    num = byteInstruction - 0x20;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.SLL);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x38) ||                                                                     // srl r
                           (byteInstruction == 0x39) ||
                           (byteInstruction == 0x3A) ||
                           (byteInstruction == 0x3B) ||
                           (byteInstruction == 0x3C) ||
                           (byteInstruction == 0x3D) ||
                           (byteInstruction == 0x3E) ||
                           (byteInstruction == 0x3F))
                {
                    num = byteInstruction - 0x28;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = RotateShift(val, OPERATOR.SRL);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x40) ||                                                                     // bit 0, r
                           (byteInstruction == 0x41) ||
                           (byteInstruction == 0x42) ||
                           (byteInstruction == 0x43) ||
                           (byteInstruction == 0x44) ||
                           (byteInstruction == 0x45) ||
                           (byteInstruction == 0x46) ||
                           (byteInstruction == 0x47))
                {
                    num = byteInstruction - 0x40;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b00000001) == 0b00000001;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x48) ||                                                                     // bit 1, r
                           (byteInstruction == 0x49) ||
                           (byteInstruction == 0x4A) ||
                           (byteInstruction == 0x4B) ||
                           (byteInstruction == 0x4C) ||
                           (byteInstruction == 0x4D) ||
                           (byteInstruction == 0x4E) ||
                           (byteInstruction == 0x4F))
                {
                    num = byteInstruction - 0x48;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b00000010) == 0b00000010;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x50) ||                                                                     // bit 2, r
                           (byteInstruction == 0x51) ||
                           (byteInstruction == 0x52) ||
                           (byteInstruction == 0x53) ||
                           (byteInstruction == 0x54) ||
                           (byteInstruction == 0x55) ||
                           (byteInstruction == 0x56) ||
                           (byteInstruction == 0x57))
                {
                    num = byteInstruction - 0x50;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b00000100) == 0b00000100;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x58) ||                                                                     // bit 3, r
                           (byteInstruction == 0x59) ||
                           (byteInstruction == 0x5A) ||
                           (byteInstruction == 0x5B) ||
                           (byteInstruction == 0x5C) ||
                           (byteInstruction == 0x5D) ||
                           (byteInstruction == 0x5E) ||
                           (byteInstruction == 0x5F))
                {
                    num = byteInstruction - 0x58;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b00001000) == 0b00001000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x60) ||                                                                     // bit 4, r
                           (byteInstruction == 0x61) ||
                           (byteInstruction == 0x62) ||
                           (byteInstruction == 0x63) ||
                           (byteInstruction == 0x64) ||
                           (byteInstruction == 0x65) ||
                           (byteInstruction == 0x66) ||
                           (byteInstruction == 0x67))
                {
                    num = byteInstruction - 0x60;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b00010000) == 0b00010000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x68) ||                                                                     // bit 5, r
                           (byteInstruction == 0x69) ||
                           (byteInstruction == 0x6A) ||
                           (byteInstruction == 0x6B) ||
                           (byteInstruction == 0x6C) ||
                           (byteInstruction == 0x6D) ||
                           (byteInstruction == 0x6E) ||
                           (byteInstruction == 0x6F))
                {
                    num = byteInstruction - 0x68;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b00100000) == 0b00100000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x70) ||                                                                     // bit 6, r
                           (byteInstruction == 0x71) ||
                           (byteInstruction == 0x72) ||
                           (byteInstruction == 0x73) ||
                           (byteInstruction == 0x74) ||
                           (byteInstruction == 0x75) ||
                           (byteInstruction == 0x76) ||
                           (byteInstruction == 0x77))
                {
                    num = byteInstruction - 0x70;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b01000000) == 0b01000000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x78) ||                                                                     // bit 7, r
                           (byteInstruction == 0x79) ||
                           (byteInstruction == 0x7A) ||
                           (byteInstruction == 0x7B) ||
                           (byteInstruction == 0x7C) ||
                           (byteInstruction == 0x7D) ||
                           (byteInstruction == 0x7E) ||
                           (byteInstruction == 0x7F))
                {
                    num = byteInstruction - 0x78;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    result = (val & 0b10000000) == 0b10000000;
                    if (result) flagZ = false; else flagZ = true;
                    flagH = true;
                    flagN = false;
                    registerPC++;
                } else if ((byteInstruction == 0x80) ||                                                                     // res 0, r
                           (byteInstruction == 0x81) ||
                           (byteInstruction == 0x82) ||
                           (byteInstruction == 0x83) ||
                           (byteInstruction == 0x84) ||
                           (byteInstruction == 0x85) ||
                           (byteInstruction == 0x86) ||
                           (byteInstruction == 0x87))
                {
                    num = byteInstruction - 0x80;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b11111110);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x88) ||                                                                     // res 1, r
                           (byteInstruction == 0x89) ||
                           (byteInstruction == 0x8A) ||
                           (byteInstruction == 0x8B) ||
                           (byteInstruction == 0x8C) ||
                           (byteInstruction == 0x8D) ||
                           (byteInstruction == 0x8E) ||
                           (byteInstruction == 0x8F))
                {
                    num = byteInstruction - 0x88;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b11111101);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x90) ||                                                                     // res 2, r
                           (byteInstruction == 0x91) ||
                           (byteInstruction == 0x92) ||
                           (byteInstruction == 0x93) ||
                           (byteInstruction == 0x94) ||
                           (byteInstruction == 0x95) ||
                           (byteInstruction == 0x96) ||
                           (byteInstruction == 0x97))
                {
                    num = byteInstruction - 0x90;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b11111011);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0x98) ||                                                                     // res 3, r
                           (byteInstruction == 0x99) ||
                           (byteInstruction == 0x9A) ||
                           (byteInstruction == 0x9B) ||
                           (byteInstruction == 0x9C) ||
                           (byteInstruction == 0x9D) ||
                           (byteInstruction == 0x9E) ||
                           (byteInstruction == 0x9F))
                {
                    num = byteInstruction - 0x98;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b11110111);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xA0) ||                                                                     // res 4, r
                           (byteInstruction == 0xA1) ||
                           (byteInstruction == 0xA2) ||
                           (byteInstruction == 0xA3) ||
                           (byteInstruction == 0xA4) ||
                           (byteInstruction == 0xA5) ||
                           (byteInstruction == 0xA6) ||
                           (byteInstruction == 0xA7))
                {
                    num = byteInstruction - 0xA0;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b11101111);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xA8) ||                                                                     // res 5, r
                           (byteInstruction == 0xA9) ||
                           (byteInstruction == 0xAA) ||
                           (byteInstruction == 0xAB) ||
                           (byteInstruction == 0xAC) ||
                           (byteInstruction == 0xAD) ||
                           (byteInstruction == 0xAE) ||
                           (byteInstruction == 0xAF))
                {
                    num = byteInstruction - 0xA8;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b11011111);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xB0) ||                                                                     // res 6, r
                           (byteInstruction == 0xB1) ||
                           (byteInstruction == 0xB2) ||
                           (byteInstruction == 0xB3) ||
                           (byteInstruction == 0xB4) ||
                           (byteInstruction == 0xB5) ||
                           (byteInstruction == 0xB6) ||
                           (byteInstruction == 0xB7))
                {
                    num = byteInstruction - 0xB0;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b10111111);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xB8) ||                                                                     // res 7, r
                           (byteInstruction == 0xB9) ||
                           (byteInstruction == 0xBA) ||
                           (byteInstruction == 0xBB) ||
                           (byteInstruction == 0xBC) ||
                           (byteInstruction == 0xBD) ||
                           (byteInstruction == 0xBE) ||
                           (byteInstruction == 0xBF))
                {
                    num = byteInstruction - 0xB8;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val & 0b01111111);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xC0) ||                                                                     // set 0, r
                           (byteInstruction == 0xC1) ||
                           (byteInstruction == 0xC2) ||
                           (byteInstruction == 0xC3) ||
                           (byteInstruction == 0xC4) ||
                           (byteInstruction == 0xC5) ||
                           (byteInstruction == 0xC6) ||
                           (byteInstruction == 0xC7))
                {
                    num = byteInstruction - 0xC0;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b00000001);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xC8) ||                                                                     // set 1, r
                           (byteInstruction == 0xC9) ||
                           (byteInstruction == 0xCA) ||
                           (byteInstruction == 0xCB) ||
                           (byteInstruction == 0xCC) ||
                           (byteInstruction == 0xCD) ||
                           (byteInstruction == 0xCE) ||
                           (byteInstruction == 0xCF))
                {
                    num = byteInstruction - 0xC8;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b00000010);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xD0) ||                                                                     // set 2, r
                           (byteInstruction == 0xD1) ||
                           (byteInstruction == 0xD2) ||
                           (byteInstruction == 0xD3) ||
                           (byteInstruction == 0xD4) ||
                           (byteInstruction == 0xD5) ||
                           (byteInstruction == 0xD6) ||
                           (byteInstruction == 0xD7))
                {
                    num = byteInstruction - 0xD0;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b00000100);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xD8) ||                                                                     // set 3, r
                           (byteInstruction == 0xD9) ||
                           (byteInstruction == 0xDA) ||
                           (byteInstruction == 0xDB) ||
                           (byteInstruction == 0xDC) ||
                           (byteInstruction == 0xDD) ||
                           (byteInstruction == 0xDE) ||
                           (byteInstruction == 0xDF))
                {
                    num = byteInstruction - 0xD8;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b00001000);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xE0) ||                                                                     // set 4, r
                           (byteInstruction == 0xE1) ||
                           (byteInstruction == 0xE2) ||
                           (byteInstruction == 0xE3) ||
                           (byteInstruction == 0xE4) ||
                           (byteInstruction == 0xE5) ||
                           (byteInstruction == 0xE6) ||
                           (byteInstruction == 0xE7))
                {
                    num = byteInstruction - 0xE0;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b00010000);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xE8) ||                                                                     // set 5, r
                           (byteInstruction == 0xE9) ||
                           (byteInstruction == 0xEA) ||
                           (byteInstruction == 0xEB) ||
                           (byteInstruction == 0xEC) ||
                           (byteInstruction == 0xED) ||
                           (byteInstruction == 0xEE) ||
                           (byteInstruction == 0xEF))
                {
                    num = byteInstruction - 0xE8;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b00100000);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xF0) ||                                                                     // set 6, r
                           (byteInstruction == 0xF1) ||
                           (byteInstruction == 0xF2) ||
                           (byteInstruction == 0xF3) ||
                           (byteInstruction == 0xF4) ||
                           (byteInstruction == 0xF5) ||
                           (byteInstruction == 0xF6) ||
                           (byteInstruction == 0xF7))
                {
                    num = byteInstruction - 0xF0;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b01000000);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else if ((byteInstruction == 0xF8) ||                                                                     // set 7, r
                           (byteInstruction == 0xF9) ||
                           (byteInstruction == 0xFA) ||
                           (byteInstruction == 0xFB) ||
                           (byteInstruction == 0xFC) ||
                           (byteInstruction == 0xFD) ||
                           (byteInstruction == 0xFE) ||
                           (byteInstruction == 0xFF))
                {
                    num = byteInstruction - 0xF8;
                    result = GetRegisterValue((byte)(num & 0x07), ref val);
                    if (!result) return ("Can't get the register value");
                    val = (byte)(val | 0b10000000);
                    result = SetRegisterValue((byte)(num & 0x07), val);
                    if (!result) return ("Can't set the register value");
                    registerPC++;
                } else
                {
                    return ("Unknown Bit instruction '" + byteInstruction.ToString("X2") + "'");
                }
            } catch (Exception exception)
            {
                return ("Exception at memory location: " + registerPC.ToString("X") + ":\r\n" + exception.Message);
            }

            return "";
        }

        #endregion

        #region Methods (DisAssemble)

        /// <summary>
        /// Decode a single instruction and state the number of operand bytes (Main, Bit, IX, IY and Misc.)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string Decode(int code, out int size, out TYPE type)
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

                    return (instruction.ToUpper());
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

                    return (instruction.ToUpper());
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

                    return (instruction.ToUpper());
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

                    return (instruction.ToUpper());
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

                    return (instruction.ToUpper());
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
        public string DecodeIXbit(int code, out int size, out TYPE type)
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

                    return (instruction.ToUpper());
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
        public string DecodeIYbit(int code, out int size, out TYPE type)
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

                    return (instruction.ToUpper());
                }
            }

            return (instruction);
        }

        #endregion
    }
}

 