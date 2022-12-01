using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EasyCAD
{
    [Serializable]
    public class Matrix
    {
        List<List<double>> matrix = new List<List<double>>();

        public int RowsCount
        {
            get { return matrix.Count; }
        }

        public int ColumnCount
        {
            get { return matrix[0].Count; }
        }

        public Matrix(int r, int c)
        {
            for (int i = 0; i < r; i++)
            {
                matrix.Add(new List<double>());
                for (int j = 0; j < c; j++)
                    matrix[i].Add(0);
            }
        }

        public Matrix(int v)
        {
            for (int i = 0; i < v; i++)
            {
                matrix.Add(new List<double>());
                for (int j = 0; j < v; j++)
                    matrix[i].Add(0);
            }
        }

        public void AddRows(int firstIndex, int secondIndex, double value = 1d)
        {
            //сложение строк
            for (int i = 0; i < ColumnCount; i++)
            {
                this[firstIndex, i] += this[secondIndex, i] * value;
            }
        }

        public void MultiplyRowNyNumber(int rowIndex, double value)
        {
            //вторая умноженная на number прибавляется к первой
            for (int i = 0; i < ColumnCount; i++)
            {
                this[rowIndex, i] *= value;
            }
        }

        public void RoundElements(int accuracy)
        {
            //округлить элементы
            for (int i = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    this[i, j] = (float)Math.Round(this[i, j], accuracy);
                }
            }
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    s += $"{matrix[i][j]} ";
                }
                s += Environment.NewLine;
            }
            return s;
        }

        public double this[int r, int c]
        {
            get { return matrix[r][c]; }
            set { matrix[r][c] = value; }
        }

        public static Matrix GetExtendedMatrix(Matrix A, Matrix B)
        {
            //получить расширенную матрицу
            Matrix extended = new Matrix(A.RowsCount, A.RowsCount + 1);
            for (int i = 0; i < extended.RowsCount; i++)
            {
                for (int j = 0; j < extended.ColumnCount; j++)
                {
                    if (j == extended.ColumnCount - 1)
                        extended[i, j] = B[i, 0];
                    else
                        extended[i, j] = A[i, j];
                }
            }
            return extended;
        }

        public static Matrix SolveSystemByGaussian(Matrix A, Matrix B)
        {
            //решение системы методом Гаусса
            Matrix extended = GetExtendedMatrix(A, B);

            for (int i = 0; i < extended.RowsCount - 1; i++)
            {
                extended.MultiplyRowNyNumber(i, 1f / extended[i, i]);
                
                for (int j = i + 1; j < extended.RowsCount; j++)
                    extended.AddRows(j, i, -extended[j, i]);
            }

            extended.MultiplyRowNyNumber(extended.RowsCount - 1, 1f / extended[extended.RowsCount - 1, extended.RowsCount - 1]);

            for (int i = extended.RowsCount - 1; i >= 1; i--)
                for (int j = i - 1; j >= 0; j--)
                    extended.AddRows(j, i, -extended[j, i]);

            extended.RoundElements(3);

            Matrix solution = new Matrix(extended.RowsCount, 1);
            for (int i = 0; i < solution.RowsCount; i++)
                solution[i, 0] = extended[i, extended.ColumnCount - 1];

            return solution;
        }

        public double GetMaxInRow(int i)
        {
            return matrix[i].Max();
        }
    }
}
