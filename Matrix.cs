using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SolutionSystemEquations
{
    public enum Method
    {
        Iteration,
        Zeidel
    }
    public class MatrixNotQuadraticException : Exception
    {
        public MatrixNotQuadraticException() : base() { }
        public MatrixNotQuadraticException(string msg) : base(msg) { }
        public MatrixNotQuadraticException(string msg, SystemException inner) : base(msg, inner) { }
        public MatrixNotQuadraticException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
    public class DetIsNullException : Exception
    {
        public DetIsNullException() { }
        public DetIsNullException(string message) : base(message) { }
        public DetIsNullException(string message, Exception inner) : base(message, inner) { }
        public DetIsNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public static class Matrix
    {
        public static double Det(double[,] matrix)
        {
            if (!CheckQuadraticity(matrix)) throw new MatrixNotQuadraticException("Матрица не является квадратичной.");

            double[,] copyArray = new double[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, copyArray, matrix.Length);

            double[,] m = Triangular(copyArray);
            double det = 1;

            for (int i = 0; i < copyArray.GetLength(0); i++)
            {
                det = det * m[i, i];
            }

            return det;
        }

        public static double[,] Multiplication(double[,] matrix, double num) 
        {
            double[,] copyArray = new double[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, copyArray, matrix.Length);

            for (int i = 0; i < copyArray.GetLength(0); i++)
            {
                for (int j = 0; j < copyArray.GetLength(1); j++)
                {
                    copyArray[i, j] = copyArray[i, j] * num;
                }
            }

            return copyArray;
        }

        public static double[,] Multiplication(double[,] a, double[,] b)
        {
            double[,] r = new double[a.GetLength(0), b.GetLength(1)];
            Parallel.For(0, a.GetLength(0), (i) =>
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            });
            return r;
        }

        public static double[,] Sum(double[,] matrix1, double[,] matrix2)
        {
            double[,] sum = new double[matrix1.GetLength(0), matrix1.GetLength(1)];

            for (int i = 0; i < sum.GetLength(0); i++)
            {
                for (int j = 0; j < sum.GetLength(1); j++)
                {
                    sum[i, j] = matrix1[i, j] + matrix2[i, j];
                }
            }

            return sum;
        }

        public static double[,] Dif(double[,] matrix1, double[,] matrix2)
        {
            double[,] dif = new double[matrix1.GetLength(0), matrix1.GetLength(1)];

            for (int i = 0; i < dif.GetLength(0); i++)
            {
                for (int j = 0; j < dif.GetLength(1); j++)
                {
                    dif[i, j] = matrix1[i, j] - matrix2[i, j];
                }
            }

            return dif;
        }

        public static double[,] Dif(double[,] matrix1, double num)
        {
            double[,] dif = new double[matrix1.GetLength(0), matrix1.GetLength(1)];

            for (int i = 0; i < dif.GetLength(0); i++)
            {
                for (int j = 0; j < dif.GetLength(1); j++)
                {
                    dif[i, j] = matrix1[i, j] - num;
                }
            }

            return dif;
        }

        public static double[,] Inverse(double[,] matrix)
        {
            if (!CheckQuadraticity(matrix)) return null;
            double det = Det(matrix);
            if (det == 0) return null;

            double[,] copyArray = new double[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, copyArray, matrix.Length);

            for (int i = 0; i < copyArray.GetLength(0); i++)
            {
                for (int j = 0; j < copyArray.GetLength(1); j++)
                {
                    copyArray[i, j] = GetAlgExtra(matrix, i + 1, j + 1);
                }
            }

            double koef = 1 / det;
            double[,] extraMatrixT = Transporse(copyArray);
            double[,] inverseMatrix = Multiplication(extraMatrixT, koef);

            return inverseMatrix;
        }

        public static double GetAlgExtra(double[,] matrix, int numRow, int numColumn)
        {
            if (!CheckQuadraticity(matrix)) return double.NaN;
            if (numRow > matrix.GetLength(0) || numColumn > matrix.GetLength(1)) return double.NaN;
            if (numRow < 1 || numColumn < 1) return double.NaN;

            double[,] minorMatrix = new double[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];

            int row, col;
            row = col = 0;

            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (i + 1 != numRow && j + 1 != numColumn)
                    {
                        minorMatrix[row, col] = matrix[i, j];
                        col++;
                    }
                }
                col = 0;
                if (i + 1 != numRow) row++;
            }

            double algExtra = Det(minorMatrix) * Math.Pow(-1, numRow + numColumn);

            return algExtra;
        }

        public static double[,] Transporse(double[,] matrix)
        {
            if (!CheckQuadraticity(matrix)) return null;

            double[,] copyArray = new double[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, copyArray, matrix.Length);

            double num;
            for (int i = 0; i < copyArray.GetLength(0); i++)
            {
                for (int j = 0; j < i; j++)
                {
                    num = copyArray[i, j];
                    copyArray[i, j] = copyArray[j, i];
                    copyArray[j, i] = num;
                }
            }

            return copyArray;
        }

        public static double[,] Triangular(double[,] matrix) 
        {
            if (!CheckQuadraticity(matrix)) return null;

            double[,] copyArray = new double[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, copyArray, matrix.Length);

            for (int i = 0; i < copyArray.GetLength(0) - 1; i++)
            {
                for (int j = i + 1; j < copyArray.GetLength(1); j++)
                {
                    double koef;
                    if (copyArray[i, i] == 0)
                    {
                        for (int k = 0; k < copyArray.GetLength(1); k++)
                        {
                            copyArray[i, k] += copyArray[i + 1, k];
                        }
                        koef = copyArray[j, i] / copyArray[i, i];
                    }
                    else koef = copyArray[j, i] / copyArray[i, i];
                    
                    for (int k = i; k < copyArray.GetLength(0); k++)
                        copyArray[j, k] -= copyArray[i, k] * koef;
                }
            }

            return copyArray;
        }

        public static double[,] CreateACopy(double[,] m)
        {
            double[,] lastm = new double[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < lastm.GetLength(0); i++)
            {
                for (int j = 0; j < lastm.GetLength(1); j++)
                {
                    lastm[i, j] = m[i, j];
                }
            }
            return lastm;
        }


        public static double Rate1(double[,] matrix)
        {
            double sum = 0;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                double curSum = 0;
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    curSum += Math.Abs(matrix[i, j]);
                }
                
                if (curSum > sum) sum = curSum;
            }

            return sum;
        }

        public static double Rate2(double[,] matrix)
        {
            double sum = 0;

            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                double curSum = 0;
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    curSum += Math.Abs(matrix[i, j]);
                }

                if (curSum > sum) sum = curSum;
            }

            return sum;
        }

        public static double Rate3(double[,] matrix)
        {
            double sum = 0;

            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    sum += Math.Pow(matrix[i, j], 2);
                }
            }

            double rate = Math.Sqrt(sum);

            return rate;
        }

        public static bool CheckQuadraticity(double[,] matrix) 
        {
            if (matrix.GetLength(0) == matrix.GetLength(1)) return true;
            else return false;
        }

    }
}
