using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace SolutionSystemEquations
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate double[,] SolutionMethod();
        int maxEquations = 5;
        int minEquations = 2;
        int countMethods = 2;
        DataTable dtSolution;
        DataTable dtData;

        public MainWindow()
        {
            InitializeComponent();

            //Создание таблицы ответов и привязка к DataGrid
            dtSolution = new DataTable();
            dtSolution.Columns.Add(new DataColumn("Переменная", typeof(string)));
            dtSolution.Columns.Add(new DataColumn("1-й метод", typeof(double)));
            dtSolution.Columns.Add(new DataColumn("2-й метод", typeof(double)));

            dataGridSolution.ItemsSource = dtSolution.AsDataView();

            //Создание таблицы с дополнительными данными ответа и привязка к DataGrid
            dtData = new DataTable();
            dtData.Columns.Add(new DataColumn("№ Метода", typeof(int)));
            dtData.Columns.Add(new DataColumn("Точность", typeof(string)));
            dtData.Columns.Add(new DataColumn("Количество итераций", typeof(int)));

            dataGridData.ItemsSource = dtData.AsDataView();
        }

        private void buttonAddEquation_Click(object sender, RoutedEventArgs e)
        {
            if (viewSystemEquations.uniformGrid.Rows == maxEquations)
            {
                MessageBox.Show("Достигнуто максимальное количество уравнений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            viewSystemEquations.AddEquation();
        }

        private void buttonDelEquation_Click(object sender, RoutedEventArgs e)
        {
            if (viewSystemEquations.uniformGrid.Rows == minEquations)
            {
                MessageBox.Show("Достигнуто минимальное количество уравнений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            viewSystemEquations.DelEquation();
        }

        private void comboBoxSelectMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void buttonFindSolution_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double[,] matrixA = viewSystemEquations.GetValuesMatrixA();
                double[,] matrixB = viewSystemEquations.GetValuesMatrixB();

                SystemEquations systemEquations;
                if (textBoxAccuracy.Text != string.Empty)
                {
                    double accuracy = Convert.ToDouble(textBoxAccuracy.Text);
                    systemEquations = new SystemEquations(matrixA, matrixB, accuracy);
                }
                else systemEquations = new SystemEquations(matrixA, matrixB);

                SolutionMethod solutionMethod = GetSolutionMethod(comboBoxSelectMethod, systemEquations);
                SolutionMethod solutionMethod2 = GetSolutionMethod(comboBoxSelectMethod2, systemEquations);

                try
                {
                    double[,] solution = solutionMethod();
                    double[,] solution2 = solutionMethod2();

                    dtSolution.Rows.Clear();
                    for (int i = 0; i < solution.Length; i++)
                    {
                        dtSolution.Rows.Add("x" + (i + 1), Math.Round(solution[i, 0], 3), Math.Round(solution2[i, 0], 3));
                    }

                    dtData.Rows.Clear();
                    int[] methods = new int[countMethods];
                    methods[0] = (int)GetMethod(solutionMethod, systemEquations);
                    methods[1] = (int)GetMethod(solutionMethod2, systemEquations);

                    for (int i = 0; i < countMethods; i++)
                    {
                        string strAccuracy = "0";
                        if (methods[i] != -1)
                        {
                            double lastValue = Math.Round(systemEquations.GetLastValueIteration((Method)methods[i]), 6);
                            strAccuracy = lastValue.ToString() + " < " + systemEquations.E.ToString();
                        }
                        dtData.Rows.Add(i + 1, strAccuracy, systemEquations.GetCountIteration((Method)methods[i]));
                    }
                }

                catch (MatrixNotQuadraticException ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                catch (FormatException ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                catch { MessageBox.Show("В процессе решения возникла ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

            }
            catch (FormatException ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch { MessageBox.Show("Возникла ошибка при вводе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private Method GetMethod(SolutionMethod solutionMethod, SystemEquations systemEquations)
        {
            if (systemEquations == null || solutionMethod == null) return (Method)(-1);

            if (solutionMethod == systemEquations.SimpleItterationMethod) return Method.Iteration;
            else if (solutionMethod == systemEquations.Zeidel) return Method.Zeidel;
            else return (Method)(-1);
        }

        private SolutionMethod GetSolutionMethod(ComboBox comboBox, SystemEquations systemEquations)
        {
            if (systemEquations == null || comboBox == null) return null;

            SolutionMethod solutionMethod = null;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    solutionMethod = systemEquations.MatrixMethod;
                    break;
                case 1:
                    solutionMethod = systemEquations.KramerMethod;
                    break;
                case 2:
                    solutionMethod = systemEquations.GausMethod;
                    break;
                case 3:
                    solutionMethod = systemEquations.GausGordanMethod;
                    break;
                case 4:
                    solutionMethod = systemEquations.LUDecopositionMethod;
                    break;
                case 5:
                    solutionMethod = systemEquations.SquareRootMethod;
                    break;
                case 6:
                    solutionMethod = systemEquations.SimpleItterationMethod;
                    break;
                case 7:
                    solutionMethod = systemEquations.Zeidel;
                    break;
                default:
                    solutionMethod = null;
                    break;
            }

            return solutionMethod;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double[,] A = new double[3, 3] {
                { 3, 4, 2 },
                { 2, -4, -3 },
                { 1, 5, 1 }
            };

            double[,] B = new double[3, 1] { { 8 }, { -1 }, { 0 } };

            viewSystemEquations.FillValues(A, B);
        }
    }
}
