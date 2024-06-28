namespace AnchorCalc.Infrastructure.Calc;

public class Matrix
{
    public static double[,] TransposeMatrix(double[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[columns, rows];

        for (var c = 0; c < columns; c++)
        for (var r = 0; r < rows; r++)
            result[c, r] = matrix[r, c];
        return result;
    }

    public static double MaxNumberInMatrix(double[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        double result = 0;

        for (var c = 0; c < columns; c++)
        for (var r = 0; r < rows; r++)
            if (Math.Abs(matrix[r, c]) > result)
                result = Math.Abs(matrix[r, c]);
        return result;
    }

    public static double[,] DivideMatrixByNumber(double[,] matrix, double divider)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = matrix[r, c] / divider;
        return result;
    }

    public static double[,] DivideMatrixByMatrix(double[,] matrix, double[,] divider)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = matrix[r, c] / divider[r, c];
        return result;
    }

    public static double[,] FoldMatrixByNumber(double[,] matrix, double folder)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = matrix[r, c] + folder;
        return result;
    }

    public static double[,] FoldMatrixByMatrix(double[,] matrix, double[,] folder)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = matrix[r, c] + folder[r, c];
        return result;
    }

    public static double[,] DiffMatrixByMatrix(double[,] matrix, double[,] differ)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = matrix[r, c] - differ[r, c];
        return result;
    }

    public static double[] MultiplyVectorByNumber(double[] vector, double multiplier)
    {
        var result = new double[vector.GetLength(0)];
        for (var r = 0; r < vector.GetLength(0); r++) result[r] = vector[r] * multiplier;
        return result;
    }

    public static double[,] MultiplyMatrixByNumber(double[,] matrix, double multiplier)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = matrix[r, c] * multiplier;
        return result;
    }

    public static double[,] MultiplyMatrixByMatrix(double[,] matrix, double[,] multiplier)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = matrix[r, c] * multiplier[r, c];
        return result;
    }

    public static double[,] PowMatrixByNumber(double[,] matrix, double power)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = Math.Pow(matrix[r, c], power);
        return result;
    }

    public static double[,] AbsMatrix(double[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[rows, columns];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < columns; c++)
            result[r, c] = Math.Abs(matrix[r, c]);
        return result;
    }

    public static double[,] SolutionMatrix(double[,] leftmatrix, double[,] rightmatrix)
    {
        var rows = leftmatrix.GetLength(0);

        var result = new double[rows, 1];

        for (var r = 0; r < rows; r++)
        {
            double sum = 0;
            for (var c = 0; c < rows; c++) sum +=leftmatrix[r, c] * rightmatrix[c, 0];
            result[r, 0] = sum;
        }

        return result;
    }

    public static double[] MatrixSumOfElementsInRows(double[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        var result = new double[columns];
        double sum;
        for (var c = 0; c < columns; c++)
        {
            sum = 0;
            for (var r = 0; r < rows; r++) sum += matrix[r, c];
            result[c] = sum;
        }

        return result;
    }

    public static double VectorSumOfElements(double[] vector)
    {
        var numbers = vector.GetLength(0);
        double result = 0;
        for (var n = 0; n < numbers; n++) result += vector[n];
        return result;
    }

    public static double[,] InverseMatrix(double[,] matrix)
    {
        var n = matrix.GetLength(0);
        var det = DeterminantOfMatrix(matrix);
        if (det == 0)
            throw new DivideByZeroException("Определитель матрицы равен нулю.");
        var adjugated = AdjugateOfMatrix(matrix);
        var transposed = TransposeMatrix(adjugated);
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            transposed[i, j] = transposed[i, j] / det;
        return transposed;
    }

    public static double DeterminantOfMatrix(double[,] matrix)
    {
        if (matrix == null || matrix.GetLength(0) != matrix.GetLength(1))
            throw new ArgumentException("Matrix must be a square matrix.");
        var n = matrix.GetLength(0);
        if (n == 1)
            return matrix[0, 0]; // Определитель единичной матрицы равен элементу на главной диагонали
        if (n == 2)
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]; // Определитель матрицы 2x2
        if (n == 3)
            return matrix[0, 0] * matrix[1, 1] * matrix[2, 2] + matrix[0, 1] * matrix[1, 2] * matrix[2, 0] +
                   matrix[0, 2] * matrix[1, 0] * matrix[2, 1]
                   - matrix[0, 2] * matrix[1, 1] * matrix[2, 0] - matrix[0, 1] * matrix[1, 0] * matrix[2, 2] -
                   matrix[0, 0] * matrix[1, 2] * matrix[2, 1]; // Определитель матрицы 3x3
        throw new ArgumentException("Rank of matrix must be <=3.");
    }

    private static double DeterminantOfSubmatrix(double[,] matrix, int k, int p)
    {
        var n = matrix.GetLength(0);
        var counti = 0;
        var submatrix = new double[n - 1, n - 1];
        for (var i = 0; i < n; i++)
        {
            var countj = 0;
            if (i != k)
            {
                for (var j = 0; j < n; j++)
                    if (j != p)
                    {
                        submatrix[counti, countj] = matrix[i, j];
                        countj++;
                    }

                counti++;
            }
        }

        return submatrix[0, 0] * submatrix[1, 1] - submatrix[0, 1] * submatrix[1, 0];
    }

    public static double[,] AdjugateOfMatrix(double[,] matrix)
    {
        if (matrix == null || matrix.GetLength(0) != matrix.GetLength(1))
            throw new ArgumentException("Matrix must be a square matrix.");
        var n = matrix.GetLength(0);
        var adjugate = new double[n, n];
        var minor = -1;
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
        {
            // Вычисляем определитель для каждого элемента матрицы дополнений
            minor = -1 * minor;
            adjugate[i, j] = DeterminantOfSubmatrix(matrix, i, j) * minor;
        }

        return adjugate;
    }
}