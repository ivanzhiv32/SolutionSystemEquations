using System;
using System.Windows;
using System.Windows.Controls;

namespace SolutionSystemEquations
{
    /// <summary>
    /// Логика взаимодействия для ViewSystemEquations.xaml
    /// </summary>
    public partial class ViewSystemEquations : UserControl
    {
        public ViewSystemEquations()
        {
            InitializeComponent();
            FillNameVariables();
        }

        private void FillNameVariables()
        {
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                Cell cell = uniformGrid.Children[i] as Cell;
                if (cell != null) cell.textBlockVar.Text = " x" + Grid.GetColumn(cell) + " +";
                else
                {
                    cell = uniformGrid.Children[i - 1] as Cell;
                    if (cell != null) cell.textBlockVar.Text = " x" + Grid.GetColumn(cell) + " =";
                }
            }
        }

        public void FillValues(double[,] matrixA, double[,] matrixB)
        {
            //Добавление уравнений
            if (uniformGrid.Rows != matrixA.GetLength(0))
            {
                int count = matrixA.GetLength(0) - uniformGrid.Rows;
                for (int k = 0; k < count; k++)
                {
                    AddEquation();
                }
            }

            //Заполнение значений
            int i, j;
            i = j = 0;
            foreach (UIElement ui in uniformGrid.Children)
            {
                Cell cell = ui as Cell;
                TextBox text = ui as TextBox;
                if (cell != null)
                {
                    cell.textBoxCoef.Text = matrixA[i, j].ToString();
                    j++;
                }
                else
                {
                    if (text != null) text.Text = matrixB[i, 0].ToString();
                    j = 0;
                    i++;
                }
            }
        }

        public double[,] GetValuesMatrixA()
        {
            double[,] matrix = new double[uniformGrid.Rows, uniformGrid.Rows];

            int i, j;
            i = j = 0;

            foreach (UIElement ui in uniformGrid.Children)
            {
                Cell cell = ui as Cell;
                if (cell != null)
                {
                    matrix[i, j] = Convert.ToDouble(cell.textBoxCoef.Text.ToString());
                    j++;
                }
                else
                {
                    j = 0;
                    i++;
                }
            }

            return matrix;
        }

        public double[,] GetValuesMatrixB()
        {
            double[,] matrix = new double[uniformGrid.Rows, 1];

            int i = 0;
            foreach (UIElement ui in uniformGrid.Children)
            {
                TextBox text = ui as TextBox;
                if (text != null)
                {
                    matrix[i, 0] = Convert.ToDouble(text.Text.ToString());
                    i++;
                }
            }

            return matrix;
        }

        public void AddEquation()
        {
            Cell cellSize = uniformGrid.Children[0] as Cell;
            double width, height;
            width = height = 0;
            if (cellSize != null)
            {
                width = cellSize.ActualWidth;
                height = cellSize.ActualHeight;
            }

            uniformGrid.Rows += 1;
            uniformGrid.Columns += 1;
            Width += width;
            Height += height;

            uniformGrid.Children.Clear();
            for (int i = 1; i <= uniformGrid.Rows; i++)
            {
                for (int j = 1; j <= uniformGrid.Columns; j++)
                {
                    //Изменить условия для добавления
                    if (j == uniformGrid.Columns)
                    {
                        TextBox textBox = new TextBox();
                        uniformGrid.Children.Add(textBox);
                        Grid.SetColumn(textBox, j);
                        Grid.SetRow(textBox, i);
                    }
                    else
                    {
                        Cell cell = new Cell();
                        uniformGrid.Children.Add(cell);
                        Grid.SetColumn(cell, j);
                        Grid.SetRow(cell, i);
                    }
                }
            }

            FillNameVariables();
        }

        public void DelEquation()
        {
            Cell cell = uniformGrid.Children[0] as Cell;
            double width, height;
            width = height = 0;
            if (cell != null)
            {
                width = cell.ActualWidth;
                height = cell.ActualHeight;
            }

            uniformGrid.Rows -= 1;
            uniformGrid.Columns -= 1;
            Width -= width;
            Height -= height;

            uniformGrid.Children.Clear();
            for (int i = 1; i <= uniformGrid.Rows; i++)
            {
                for (int j = 1; j <= uniformGrid.Columns; j++)
                {
                    //Изменить условия для добавления
                    if (j == uniformGrid.Columns)
                    {
                        TextBox textBox = new TextBox();
                        uniformGrid.Children.Add(textBox);
                        Grid.SetColumn(textBox, j);
                        Grid.SetRow(textBox, i);
                    }
                    else
                    {
                        Cell c = new Cell();
                        uniformGrid.Children.Add(c);
                        Grid.SetColumn(c, j);
                        Grid.SetRow(c, i);
                    }
                }
            }

            FillNameVariables();
        }
    }
}
