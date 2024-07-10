using System.Collections;

namespace AnchorCalc.Infrastructure.Calc;

public class Matrix(double[,] matrix) : IEnumerable
{
    private readonly double[,] _matrix = matrix;
    private int Rows => _matrix.GetLength(0);
    private int Columns => _matrix.GetLength(1);

    public double this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < 0 || rowIndex >= Rows)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            if (columnIndex < 0 || columnIndex >= Columns)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            return _matrix[rowIndex, columnIndex];
        }
        set
        {
            if (rowIndex < 0 || rowIndex >= Rows)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            if (columnIndex < 0 || columnIndex >= Columns)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            _matrix[rowIndex, columnIndex] = value;
        }
    }

    public IEnumerator GetEnumerator()
    {
        return _matrix.GetEnumerator();
    }

    public static implicit operator Matrix(double[,] matrix)
    {
        return new Matrix(matrix);
    }

    public static implicit operator double[,](Matrix matrix)
    {
        return matrix._matrix;
    }

    public static Matrix operator /(Matrix matrix, Matrix divider)
    {
        var result = new double[matrix.Rows, matrix.Columns];

        for (var r = 0; r < matrix.Rows; r++)
        for (var c = 0; c < matrix.Columns; c++)
            result[r, c] = matrix._matrix[r, c] / divider._matrix[r, c];
        return result;
    }

    public static Matrix operator /(Matrix matrix, double divider)
    {
        var result = new double[matrix.Rows, matrix.Columns];

        for (var r = 0; r < matrix.Rows; r++)
        for (var c = 0; c < matrix.Columns; c++)
            result[r, c] = matrix._matrix[r, c] / divider;
        return result;
    }

    public static Matrix operator +(Matrix matrix, double folder)
    {
        var result = new double[matrix.Rows, matrix.Columns];

        for (var r = 0; r < matrix.Rows; r++)
        for (var c = 0; c < matrix.Columns; c++)
            result[r, c] = matrix._matrix[r, c] + folder;
        return result;
    }

    public static Matrix operator +(Matrix matrix, Matrix folder)
    {
        var result = new double[matrix.Rows, matrix.Columns];

        for (var r = 0; r < matrix.Rows; r++)
        for (var c = 0; c < matrix.Columns; c++)
            result[r, c] = matrix._matrix[r, c] + folder._matrix[r, c];
        return result;
    }

    public static Matrix operator -(Matrix matrix, Matrix differ)
    {
        var result = new double[matrix.Rows, matrix.Columns];

        for (var r = 0; r < matrix.Rows; r++)
        for (var c = 0; c < matrix.Columns; c++)
            result[r, c] = matrix._matrix[r, c] - differ._matrix[r, c];
        return result;
    }


    public static Matrix operator *(Matrix matrix, Matrix multiplier)
    {
        var result = new double[matrix.Rows, matrix.Columns];

        for (var r = 0; r < matrix.Rows; r++)
        for (var c = 0; c < matrix.Columns; c++)
            result[r, c] = matrix._matrix[r, c] * multiplier._matrix[r, c];
        return result;
    }

    public static Matrix operator *(Matrix matrix, double multiplier)
    {
        var result = new double[matrix.Rows, matrix.Columns];

        for (var r = 0; r < matrix.Rows; r++)
        for (var c = 0; c < matrix.Columns; c++)
            result[r, c] = matrix._matrix[r, c] * multiplier;
        return result;
    }

    public Matrix Transpose()
    {
        var result = new double[Columns, Rows];

        for (var c = 0; c < Columns; c++)
        for (var r = 0; r < Rows; r++)
            result[c, r] = _matrix[r, c];

        return result;
    }

    public double Max()
    {
        double result = 0;
        for (var r = 0; r < Rows; r++)
        for (var c = 0; c < Columns; c++)
            if (Math.Abs(_matrix[r, c]) > result)
                result = Math.Abs(_matrix[r, c]);

        return result;
    }

    public Matrix Abs()
    {
        var result = new double[Rows, Columns];

        for (var r = 0; r < Rows; r++)
        for (var c = 0; c < Columns; c++)
            result[r, c] = Math.Abs(_matrix[r, c]);

        return result;
    }

    public Matrix Solution(double[,] rightMatrix)
    {
        var result = new double[Rows, 1];

        for (var r = 0; r < Rows; r++)
        for (var c = 0; c < Columns; c++)
            result[r, 0] += _matrix[r, c] * rightMatrix[c, 0];

        return result;
    }

    public double[] Sum()
    {
        var result = new double[Rows];
        for (var r = 0; r < Rows; r++)
        for (var c = 0; c < Columns; c++)
            result[r] += _matrix[r, c];

        return result;
    }


    public Matrix Inverse()
    {
        var determinant = Determinant();
        if (determinant == 0)
            throw new ArgumentException("Определитель матрицы равен нулю.");
        var result = Addition().Transpose();
        for (var r = 0; r < Rows; r++)
        for (var c = 0; c < Columns; c++)
            result._matrix[r, c] /= determinant;
        return result;
    }

    public double Determinant()
    {
        if (_matrix == null || Rows != Columns)
            throw new ArgumentException("Matrix must be a square matrix.");
        return Rows switch
        {
            1 => _matrix[0, 0],
            2 => _matrix[0, 0] * _matrix[1, 1] - _matrix[0, 1] * _matrix[1, 0],
            3 => _matrix[0, 0] * _matrix[1, 1] * _matrix[2, 2] + _matrix[0, 1] * _matrix[1, 2] * _matrix[2, 0] +
                 _matrix[0, 2] * _matrix[1, 0] * _matrix[2, 1] - _matrix[0, 2] * _matrix[1, 1] * _matrix[2, 0] -
                 _matrix[0, 1] * _matrix[1, 0] * _matrix[2, 2] - _matrix[0, 0] * _matrix[1, 2] * _matrix[2, 1],
            _ => throw new ArgumentException("Rank of matrix must be <= 3.")
        };
    }

    private double SubDeterminant(int currentRows, int currentColumns)
    {
        var subRows = 0;
        var subMatrix = new double[Rows - 1, Columns - 1];
        for (var r = 0; r < Rows; r++)
        {
            var subColumns = 0;
            if (r != currentRows)
            {
                for (var c = 0; c < Columns; c++)
                    if (c != currentColumns)
                    {
                        subMatrix[subRows, subColumns] = _matrix[r, c];
                        subColumns++;
                    }

                subRows++;
            }
        }

        return subMatrix[0, 0] * subMatrix[1, 1] - subMatrix[0, 1] * subMatrix[1, 0];
    }

    public Matrix Addition()
    {
        var addition = new double[Rows, Columns];
        var minor = -1;
        for (var r = 0; r < Rows; r++)
        for (var c = 0; c < Columns; c++)
        {
            // Вычисляем определитель для каждого элемента матрицы дополнений
            minor = -minor;
            addition[r, c] = SubDeterminant(r, c) * minor;
        }

        return addition;
    }

    public int GetLength(int dimension)
    {
        return _matrix.GetLength(dimension);
    }
}