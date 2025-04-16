using System;
using System.Collections.Generic;

namespace SolutionSystemEquations
{
    public class SystemEquations
    {
        public double[,] Coefficients => coefficients;
        private double[,] coefficients;
        public double[,] FreeTerm;
        public readonly double E = 0.01;
        private Dictionary<Method, int> iterations = new Dictionary<Method, int>();
        private Dictionary<Method, double> lastValues = new Dictionary<Method, double>();


        public SystemEquations(double[,] coefficients, double[,] freeTerm)
        {
            this.coefficients = coefficients;
            FreeTerm = freeTerm;

            lastValues[Method.Iteration] = 0;
            lastValues[Method.Zeidel] = 0;
            iterations[Method.Iteration] = 0;
            iterations[Method.Zeidel] = 0;
        }

        public SystemEquations(double[,] coefficients, double[,] freeTerm, double accuracy)
        {
            this.coefficients = coefficients;
            FreeTerm = freeTerm;
            E = accuracy;

            lastValues[Method.Iteration] = 0;
            lastValues[Method.Zeidel] = 0;
            iterations[Method.Iteration] = 0;
            iterations[Method.Zeidel] = 0;
        }

        public double GetLastValueIteration(Method method)
        {
            switch (method)
            {
                case Method.Iteration:
                    return lastValues[Method.Iteration];
                case Method.Zeidel:
                    return lastValues[Method.Zeidel];
                default:
                    return 0;
            }
        }

        public int GetCountIteration(Method method)
        {
            switch (method)
            {
                case Method.Iteration:
                    return iterations[Method.Iteration];
                case Method.Zeidel:
                    return iterations[Method.Zeidel];
                default:
                    return 0;
            }
        }

        public double[,] MatrixMethod()
        {
            double[,] x = Matrix.Multiplication(Matrix.Inverse(Coefficients), FreeTerm);
            return x;
        }

        public double[,] KramerMethod()
        {
            double det = Matrix.Det(Coefficients);
            double[,] x = new double[FreeTerm.GetLength(0), 1];

            for (int k = 0; k < FreeTerm.GetLength(0); k++)
            {
                double[,] mat = new double[Coefficients.GetLength(0), Coefficients.GetLength(1)];
                Array.Copy(Coefficients, mat, Coefficients.Length);

                for (int i = 0; i < mat.GetLength(0); i++)
                {
                    for (int j = 0; j < mat.GetLength(1); j++)
                    {
                        if (k == j) mat[i, j] = FreeTerm[i, 0];
                    }
                }

                x[k, 0] = Matrix.Det(mat) / det;
            }

            return x;
        }

        public double[,] GausMethod()
        {
            double s;
            double[,] x = new double[FreeTerm.GetLength(0), 1];

            for (int k = 0; k < Coefficients.GetLength(0) - 1; k++)
            {
                for (int i = k + 1; i < Coefficients.GetLength(0); i++)
                {
                    for (int j = k + 1; j < Coefficients.GetLength(0); j++)
                    {
                        Coefficients[i, j] = Coefficients[i, j] - Coefficients[k, j] * (Coefficients[i, k] / Coefficients[k, k]);
                    }
                    FreeTerm[i, 0] = FreeTerm[i, 0] - FreeTerm[k, 0] * Coefficients[i, k] / Coefficients[k, k];
                }
            }

            for (int k = Coefficients.GetLength(0) - 1; k >= 0; k--)
            {
                s = 0;
                for (int j = k + 1; j < Coefficients.GetLength(0); j++)
                    s = s + Coefficients[k, j] * x[j, 0];
                x[k, 0] = (FreeTerm[k, 0] - s) / Coefficients[k, k];
            }

            return x;
        }

        public double[,] GausGordanMethod()
        {
            double[,] x = new double[FreeTerm.GetLength(0), 1];

            for (int k = 0; k < Coefficients.GetLength(0) - 1; k++)
            {
                for (int i = k + 1; i < Coefficients.GetLength(0); i++)
                {
                    for (int j = k + 1; j < Coefficients.GetLength(0); j++)
                    {
                        Coefficients[i, j] = Coefficients[i, j] - Coefficients[k, j] * (Coefficients[i, k] / Coefficients[k, k]);
                    }
                    FreeTerm[i, 0] = FreeTerm[i, 0] - FreeTerm[k, 0] * Coefficients[i, k] / Coefficients[k, k];
                }
            }

            for (int i = Coefficients.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = Coefficients.GetLength(0) - 1; j >= 0; j--)
                {
                    if (i > j) Coefficients[i, j] = 0;
                    if (i == j) FreeTerm[i, 0] = FreeTerm[i, 0] / Coefficients[i, i];
                    if (i == j || j > i) Coefficients[i, j] = Coefficients[i, j] / Coefficients[i, i];
                }
            }

            for (int k = Coefficients.GetLength(0) - 1; k > 0; k--)
            {
                for (int i = k - 1; i >= 0; i--)
                {
                    for (int j = i; j >= 0; j--)
                    {
                        Coefficients[i, j] = Coefficients[i, j] - Coefficients[k, j] * (Coefficients[i, k] / Coefficients[k, k]);
                    }
                    FreeTerm[i, 0] = FreeTerm[i, 0] - FreeTerm[k, 0] * Coefficients[i, k] / Coefficients[k, k];
                }
            }

            return FreeTerm;
        }

        public double[,] LUDecopositionMethod()
        {
            double[,] lu = Coefficients;


            for (int k = 0; k <= Coefficients.GetLength(0); k++)
            {
                int j, i;
                j = k;
                double sum;
                for (i = k; i < lu.GetLength(0); i++)
                {
                    sum = 0;
                    for (int s = 0; s <= j - 1; s++)
                    {
                        sum += lu[i, s] * lu[s, j];
                    }

                    lu[i, j] = lu[i, j] - sum;
                }

                i = k;
                for (j = k + 1; j < lu.GetLength(1); j++)
                {
                    sum = 0;
                    for (int s = 0; s <= i - 1; s++)
                    {
                        sum += lu[i, s] * lu[s, j];
                    }

                    lu[i, j] = 1 / lu[i, i] * (lu[i, j] - sum);
                }
            }

            double[,] l = new double[lu.GetLength(0), lu.GetLength(1)];
            double[,] u = new double[lu.GetLength(0), lu.GetLength(1)];

            for(int i = 0; i < l.GetLength(0); i++)
            {
                for (int j = 0; j < l.GetLength(1); j++)
                {
                    if (i >= j) l[i, j] = lu[i, j];
                }
            }

            for (int i = 0; i < u.GetLength(0); i++)
            {
                for (int j = 0; j < u.GetLength(1); j++)
                {
                    if (i < j) u[i, j] = lu[i, j];
                    if (i == j) u[i, j] = 1;
                }
            }

            double[,] y = Matrix.Multiplication(Matrix.Inverse(l), FreeTerm);
            double[,] x = Matrix.Multiplication(Matrix.Inverse(u), y);

            return x;
        }

        public double[,] SquareRootMethod()
        {
            double[,] uMatrix = new double[Coefficients.GetLength(0), Coefficients.GetLength(1)];

            for (int i = 0; i < Coefficients.GetLength(0); i++)
            {
                for (int j = 0; j < Coefficients.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        double sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += Math.Pow(uMatrix[k, i], 2);
                        }
                        uMatrix[i, j] = Math.Sqrt(Coefficients[i, j] - sum);
                    }
                    else if (i < j)
                    {
                        double sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += uMatrix[k, i] * uMatrix[k, j];
                        }

                        uMatrix[i, j] = (Coefficients[i, j] - sum) / uMatrix[i, i];
                    }
                    else if (i > j) uMatrix[i, j] = 0;
                }
            }

            double[,] y = Matrix.Multiplication(Matrix.Inverse(Matrix.Transporse(uMatrix)), FreeTerm);
            double[,] x = Matrix.Multiplication(Matrix.Inverse(uMatrix), y);

            return x;
        }

        public double[,] SimpleItterationMethod()
        {
            if (Matrix.Det(Coefficients) == 0) return null;

            double[,] e = new double[Coefficients.GetLength(0), Coefficients.GetLength(1)];
            for (int i = 0; i < e.GetLength(0); i++)
            {
                for (int j = 0; j < e.GetLength(1); j++)
                {
                    e[i, j] = E;
                }
            }

            double[,] d = Matrix.Dif(Matrix.Inverse(Coefficients), e);
            double[,] a = Matrix.Multiplication(e, Coefficients);
            double[,] b = Matrix.Multiplication(d, FreeTerm);

            Dictionary<int, double[,]> results = new Dictionary<int, double[,]>();
            results.Add(0, b);

            double norm = 0;
            double[,] solution;
            int k = 0;
            do
            {
                k++;
                solution = Matrix.Sum(Matrix.Multiplication(a, results[k - 1]), b);
                results.Add(k, solution);
                norm = Math.Abs(Matrix.Rate1(Matrix.Dif(results[k], results[k - 1])));
            } while ( norm > E);

            iterations[Method.Iteration] = k;
            lastValues[Method.Iteration] = norm;

            return solution;
        }

        public double[,] Zeidel()
        {
            if (Matrix.Det(coefficients) == 0) throw new DetIsNullException("Детерминант равен нулю.");

            double[,] e = new double[Coefficients.GetLength(0), Coefficients.GetLength(1)];
            for (int i = 0; i < e.GetLength(0); i++)
            {
                for (int j = 0; j < e.GetLength(1); j++)
                {
                    e[i, j] = E;
                }
            }

            double[,] d = Matrix.Dif(Matrix.Inverse(Coefficients), e);
            double[,] a = Matrix.Multiplication(e, Coefficients);
            double[,] b = Matrix.Multiplication(d, FreeTerm);

            Dictionary<int, double[,]> results = new Dictionary<int, double[,]>();
            results.Add(0, b);

            double[,] solution = null;
            double norm;
            int k = 0;
            do
            {
                double[,] previousX = results[k];//Значения X на предыдущей итерации
                double[,] currentX = Matrix.CreateACopy(previousX);//Переменная для хранения X на текущей итерации

                for (int i = 0; i < coefficients.GetLength(0); i++)
                {
                    double[,] current = Matrix.CreateACopy(currentX);
                    double[,] result = Matrix.Sum(Matrix.Multiplication(a, current), b);//Нахождение значения X

                    currentX[i, 0] = result[i, 0];//Добавление нового найденного значения
                }
                k++;
                results.Add(k, currentX);//Добавление нового X в таблицу результатов
                norm = Math.Abs(Matrix.Rate1(Matrix.Dif(currentX, previousX)));
            }
            while (norm > E);

            iterations[Method.Zeidel] = k;
            lastValues[Method.Zeidel] = norm;
            return results[results.Count - 1];
        }
    }
}

