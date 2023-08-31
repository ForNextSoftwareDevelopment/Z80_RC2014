﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80_RC2014
{
    internal static class Calculator
    {
        /// <summary>
        /// Calculates the value of a mathematical expression
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte CalculateByte(string str, out string result)
        {
            try
            {
                Queue<string> queue = CreateRPN(str);

                int res = ParseRPN(queue);

                result = "OK";
                return (byte)res;
            } catch (Exception ex)
            {
                result = ex.Message;
                return 0;
            }
        }

        public static UInt16 Calculate2Bytes(string str, out string result)
        {
            try
            {
                Queue<string> queue = CreateRPN(str);

                int res = ParseRPN(queue);

                result = "OK";
                return (UInt16)res;
            } catch (Exception ex)
            {
                result = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// Generate suffix expression
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Queue<string> CreateRPN(string str)
        {
            // If starting with symbol, insert 0 as start
            if (str.StartsWith("-") || str.StartsWith("+") || str.StartsWith("*") || str.StartsWith("/") || str.StartsWith("&") || str.StartsWith("|"))
            {
                str = "0" + str;
            }

            // Stack of temporary storage + - * / (symbols)
            Stack<char> stack = new Stack<char>();

            // A queue that stores suffix expressions
            Queue<string> queue = new Queue<string>();

            // Convert to lowercase, replace tabs with spaces
            str = str.Replace('\t', ' ').ToLower();

            // Convert hex values to decimal
            while (str.Contains("0x"))
            {
                int start = str.IndexOf("0x");
                int end = str.Length;
                if (end > start + 1) start+=2;
                for (int i = start; i < str.Length; i++)
                {
                    if (str[i] == ' ') { end = i; break; }
                    if (str[i] == '+') { end = i; break; }
                    if (str[i] == '-') { end = i; break; }
                    if (str[i] == '*') { end = i; break; }
                    if (str[i] == '/') { end = i; break; }
                    if (str[i] == '&') { end = i; break; }
                    if (str[i] == '|') { end = i; break; }
                    if (str[i] == ')') { end = i; break; }
                }

                if (end > start)
                {
                    string hex = str.Substring(start, end - start);
                    string dec = Convert.ToInt32(hex, 16).ToString();

                    str = str.Replace("0x" + hex, dec);
                } else
                {
                    Exception ex = new Exception("\r\nError in hexadecimal value in: " + str);
                    throw (ex);
                }
            }

            // Convert hex values to decimal
            while (str.Contains("$"))
            {
                int start = str.IndexOf("$");
                int end = str.Length;
                if (end > start) start++;
                for (int i = start; i < str.Length; i++)
                {
                    if (str[i] == ' ') { end = i; break; }
                    if (str[i] == '+') { end = i; break; }
                    if (str[i] == '-') { end = i; break; }
                    if (str[i] == '*') { end = i; break; }
                    if (str[i] == '/') { end = i; break; }
                    if (str[i] == '&') { end = i; break; }
                    if (str[i] == '|') { end = i; break; }
                    if (str[i] == ')') { end = i; break; }
                }

                if (end > start)
                {
                    string hex = str.Substring(start, end - start);
                    string dec = Convert.ToInt32(hex, 16).ToString();

                    str = str.Replace("$" + hex, dec);
                } else
                {
                    Exception ex = new Exception("\r\nError in hexadecimal value in: " + str);
                    throw (ex);
                }
            }

            while (str.Contains("h"))
            {
                int end = str.IndexOf("h");
                int start = 0;
                if (end > start) end--;
                for (int i = end; i >= 0; i--)
                {
                    if (str[i] == ' ') { start = i + 1; break; }
                    if (str[i] == '+') { start = i + 1; break; }
                    if (str[i] == '-') { start = i + 1; break; }
                    if (str[i] == '*') { start = i + 1; break; }
                    if (str[i] == '/') { start = i + 1; break; }
                    if (str[i] == '&') { start = i + 1; break; }
                    if (str[i] == '|') { start = i + 1; break; }
                    if (str[i] == '(') { start = i + 1; break; }
                }

                if (end > start)
                {
                    string hex = str.Substring(start, end + 1 - start);
                    string dec = Convert.ToInt32(hex, 16).ToString();

                    str = str.Replace(hex + "h", dec);
                } else
                {
                    Exception ex = new Exception("\r\nError in hexadecimal value in: " + str);
                    throw (ex);
                }
            }

            while (str.Contains("b"))
            {
                int end = str.IndexOf("b");
                int start = 0;
                if (end > start) end--;
                for (int i = end; i >= 0; i--)
                {
                    if (str[i] == ' ') { start = i + 1; break; }
                    if (str[i] == '+') { start = i + 1; break; }
                    if (str[i] == '-') { start = i + 1; break; }
                    if (str[i] == '*') { start = i + 1; break; }
                    if (str[i] == '/') { start = i + 1; break; }
                    if (str[i] == '&') { start = i + 1; break; }
                    if (str[i] == '|') { start = i + 1; break; }
                    if (str[i] == '(') { start = i + 1; break; }
                }

                if (end > start)
                {
                    string bin = str.Substring(start, end + 1 - start);
                    string dec = Convert.ToInt32(bin, 2).ToString();

                    str = str.Replace(bin + "b", dec);
                } else
                {
                    Exception ex = new Exception("\r\nError in binary value in: " + str);
                    throw (ex);
                }
            }

            for (int i = 0; i < str.Length;)
            {
                // If it is a space, skip 
                if (str[i] == ' ')
                {
                    i++;
                    continue;
                } else if ((str[i] >= '0') && (str[i] <= '9'))
                {
                    // Current digit
                    int cur = 0;

                    // Check value
                    while (i < str.Length && ((str[i] >= '0') && (str[i] <= '9')))
                    {
                        cur = cur * 10 + str[i] - '0';
                        i++;
                    }

                    queue.Enqueue(cur.ToString());
                } else if (str[i] == ')')
                {
                    // If it is ')', you need to pop up the operation symbol in the stack and add it to the queue of suffix expression
                    // Until '(' in the symbol stack is encountered
                    while (stack.Count != 0 && stack.Peek() != '(')
                    {
                        queue.Enqueue(stack.Pop() + "");
                    }

                    stack.Pop();
                    i++;
                } else
                {
                    // It may be + - * / & | these symbols or left parentheses
                    // At this time, you need to determine the priority of the top element of the symbol stack and the currently traversed character
                    while (stack.Count != 0 && Compare(stack.Peek(), str[i]) < 0)
                    {
                        queue.Enqueue(stack.Pop() + "");
                    }

                    // Check if symbol or integer
                    if (!str[i].Equals('+') && !str[i].Equals('-') && !str[i].Equals('*') && !str[i].Equals('/') && !str[i].Equals('&') && !str[i].Equals('|') && !str[i].Equals('('))
                    {
                        bool result = int.TryParse(str[i].ToString(), out int x);
                        if (!result)
                        {
                            Exception ex = new Exception("\r\nCan't convert operand '" + str + "' to a value");
                            throw (ex);
                        }
                    }

                    stack.Push(str[i]);
                    i++;
                }
            } while (stack.Count != 0)
            {
                queue.Enqueue(stack.Pop() + "");
            }

            return queue;
        }

        /// <summary>
        /// Processing symbol priority
        /// </summary>
        /// <param name="peek"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int Compare(char peek, char c)
        {
            if (peek == '(' || c == '(') return 1;
            if (c == '+' || c == '-') return -1;
            if (c == '&' || c == '|') return -1;
            if (c == '*' && (peek == '*' || peek == '/')) return -1;
            if (c == '/' && (peek == '*' || peek == '/')) return -1;
            return 1;
        }

        /// <summary>
        /// Resolve suffix expression
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private static int ParseRPN(Queue<string> queue)
        {
            // Result stack
            Stack<int> res = new Stack<int>();

            while (queue.Count != 0)
            {
                String t = queue.Dequeue();
                if (t.Equals("+") || t.Equals("-") || t.Equals("*") || t.Equals("/") || t.Equals("&") || t.Equals("|"))
                {
                    int a = res.Pop();
                    int b = res.Pop();
                    int result = Calculate(b, a, t);
                    res.Push(result);
                } else
                {
                    bool result = int.TryParse(t, out int x);
                    if (result)
                    {
                        res.Push(x);
                    } else
                    {
                        Exception ex = new Exception("\r\nCan't convert argument to an integer value");
                        throw (ex);
                    }
                }
            }

            return res.Pop();
        }

        /// <summary>
        /// Basic arithmetic unit
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static int Calculate(int a, int b, String t)
        {
            // Calculate
            if (t.Equals("+"))
            {
                return a + b;
            } else if (t.Equals("-"))
            {
                return a - b;
            } else if (t.Equals("*"))
            {
                return a * b;
            } else if (t.Equals("/"))
            {
                return a / b;
            } else if (t.Equals("&"))
            {
                return a & b;
            } else if (t.Equals("|"))
            {
                return a | b;
            }

            return 0;
        }
    }
}
