using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Playfair
{
    public partial class Form1 : Form
    {
        private char[,] alphabet;
        private int lin, col;

        public Form1()
        {
            InitializeComponent();
            lin = 5;
            col = 5;
            alphabet = new char[lin, col];
            CreateAlphabet("CONTRACTDEDONATIEBILATERALAASOCIETATIIPEACTIUNI");
        }

        private void CreateAlphabet(string key)
        {
            string phrase = new string(key.Distinct().ToArray()); // Remove duplicate letters
            HashSet<char> usedChars = new HashSet<char>(phrase);
            int idx = 0;

            // Fill the alphabet matrix with the phrase first
            for (int i = 0; i < lin; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (idx < phrase.Length)
                    {
                        alphabet[i, j] = phrase[idx];
                        idx++;
                    }
                    else
                    {
                        char c = 'A';
                        while (usedChars.Contains(c) || c == 'J') c++;
                        alphabet[i, j] = c;
                        usedChars.Add(c);
                    }
                }
            }
        }

        private (int, int) FindPosition(char c)
        {
            for (int i = 0; i < lin; i++)
                for (int j = 0; j < col; j++)
                    if (alphabet[i, j] == c) return (i, j);
            return (-1, -1);
        }

        private string PrepareInput(string input)
        {
            input = input.ToUpper().Replace("J", "I");
            string result = "";

            for (int i = 0; i < input.Length; i += 2)
            {
                char a = input[i];
                char b = (i + 1 < input.Length) ? input[i + 1] : 'X';

                if (a == b)
                {
                    result += a.ToString() + "X";
                    i--; // Re-evaluate 'b' on the next iteration
                }
                else
                {
                    result += a.ToString() + b.ToString();
                }
            }

            if (result.Length % 2 != 0) result += "X"; // Add padding if needed
            return result;
        }

        private string Encrypt(string input)
        {
            string preparedText = PrepareInput(input);
            string output = "";

            for (int i = 0; i < preparedText.Length; i += 2)
            {
                char a = preparedText[i];
                char b = preparedText[i + 1];

                var (a1, b1) = FindPosition(a);
                var (a2, b2) = FindPosition(b);

                if (a1 == a2)
                {
                    // Same row: shift right by one
                    output += alphabet[a1, (b1 + 1) % col];
                    output += alphabet[a2, (b2 + 1) % col];
                }
                else if (b1 == b2)
                {
                    // Same column: shift down by one
                    output += alphabet[(a1 + 1) % lin, b1];
                    output += alphabet[(a2 + 1) % lin, b2];
                }
                else
                {
                    // Rectangle rule: swap columns
                    output += alphabet[a1, b2];
                    output += alphabet[a2, b1];
                }
            }
            return output;
        }


        private string Decrypt(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i += 2)
            {
                char a = input[i];
                char b = input[i + 1];

                var (a1, b1) = FindPosition(a);
                var (a2, b2) = FindPosition(b);

                if (a1 == -1 || b1 == -1 || a2 == -1 || b2 == -1)
                {
                    continue;
                }

                if (a1 == a2)
                {
                    // Same row
                    output += alphabet[a1, (b1 - 1 + col) % col];
                    output += alphabet[a2, (b2 - 1 + col) % col];
                }
                else if (b1 == b2)
                {
                    // Same column
                    output += alphabet[(a1 - 1 + lin) % lin, b1];
                    output += alphabet[(a2 - 1 + lin) % lin, b2];
                }
                else
                {
                    // Rectangle rule
                    output += alphabet[a1, b2];
                    output += alphabet[a2, b1];
                }
            }
            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string plainText = textBox1.Text;
            string encryptedText = Encrypt(plainText);
            label1.Text = "Text Criptat: " + encryptedText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string encryptedText = label1.Text.Replace("Text Criptat: ", "");
            string decryptedText = Decrypt(encryptedText);
            label2.Text = "Text Decriptat: " + decryptedText;
        }
    }
}


