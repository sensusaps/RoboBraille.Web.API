using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboBraille.WebApi.Models
{
    public class WordTable
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

        public string[,] Text { get; set; }

        public WordTable(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.Text = new string[rows, columns];
        }

        public WordTable(int rows, int columns, string[,] content)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.Text = content;
        }

        public void AddText(int row, int column, string text)
        {
            Text[row, column] = text;
        }

        public string[,] GetTable()
        {
            return Text;
        }

        public override string ToString()
        {
            string result = null;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result += "[" + i + "," + j + "=" + Text[i, j] + "] ";
                }
                result += Environment.NewLine;
            }
            return result;
        }
    }
}
