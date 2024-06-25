namespace AnchorCalc.Infrastructure.Calc;

public class Calc
{
    public Calc(double nValue, double mxValue, double myValue, double toughValue, double hValue,
        double diameterValue, double rbValue, double e1Value, double aValue, double bValue, int triangulationValue,
        double[,] zsxValue, double[,] zsyValue, int multipleXValue, int multipleYValue, int multipleZValue)
    {
        CalcMatrix(nValue, mxValue, myValue, toughValue, hValue,
            diameterValue, rbValue, e1Value, aValue, bValue, triangulationValue,
            zsxValue, zsyValue, multipleXValue, multipleYValue, multipleZValue);
    }

    public double[,] Zbx { get; private set; } = new double[1, 1];

    public double[,] Zby { get; private set; } = new double[1, 1];

    public double[,] Zbz { get; private set; } = new double[1, 1];

    public static double ConcreteDiagram(double X, double e, double eb, double rb)
    {
        if (X >= -e && X < 0)
            return -eb * X;
        if (X >= -0.0035 && X < 0)
            return rb;
        return 0;
    }

    public static double AnchorDiagram(double X, double ea)
    {
        if (X >= 0)
            return ea * X;
        return 0;
    }

    public void CalcMatrix(double nValue, double mxValue, double myValue, double toughValue, double hValue,
        double diameterValue, double rbValue, double e1Value, double aValue, double bValue, int triangulationValue,
        double[,] zsxValue, double[,] zsyValue, int multipleXValue, int multipleYValue, int multipleZValue)
    {
        var tough = toughValue; //жесткость анкера, кН/м
        var h = hValue; //глубина заделки анкера, мм
        var diameter = diameterValue; //диаметр анкера, мм
        var rb = rbValue; //расчетное сопротивление бетона, МПа
        var e1 = e1Value; //параметр диаграммы бетона
        var eBred = rb / e1; //параметр диаграммы бетона
        var a = aValue; //ширина опорной пластины, мм
        var b = bValue; //длина опорной пластины, мм
        var n = nValue; //внешняя продольная сила, кН (+растяжение/-сжатие)
        var mx = mxValue; //изгибающий момент Мх, кН*м
        var my = myValue; //изгибающий момент Му, кН*м
        var triangulation = triangulationValue + 1; //кратность разбивки (численного интегрирования)
        var zsx = zsxValue; //координаты анкеров по оси X, мм
        var zsy = zsyValue; //координаты анкеров по оси У, мм
        var ad = a / triangulationValue; //ширина сетки разбивки бетона, мм
        var bd = b / triangulationValue; //длина сетки разбивки бетона, мм
        var ab = ad * bd; //площадь сетки разбивки бетона, мм2
        var zx = new double[triangulation];
        var zy = new double[triangulation];
        for (var i = 0; i < triangulation; i++)
        {
            zx[i] = ad * i - a * 0.5;
            zy[i] = bd * i - b * 0.5;
        }

        var zbx = new double[triangulation, triangulation];
        var zby = new double[triangulation, triangulation];
        for (var i = 0; i < triangulation; i++)
        for (var j = 0; j < triangulation; j++)
        {
            zbx[i, j] = zx[j];
            zby[i, j] = zy[j];
        }

        zby = Matrix.TransposeMatrix(zby); //массивы координат ячеек сетки бетона
        var aan = Math.PI * diameter * diameter / 4; // площадь сечения анкера номинальная, мм2
        var ea = tough * h / aan * 1000; // приведенный модуль упругости анкера, МПа
        double[,] v = { { mx / 1000 }, { my / 1000 }, { n / 1000 } }; //вектор нагрузок МН (МН*м)
        zsx = Matrix.DivideMatrixByNumber(zsx, 1000);
        zsy = Matrix.DivideMatrixByNumber(zsy, 1000);
        zbx = Matrix.DivideMatrixByNumber(zbx, 1000);
        zby = Matrix.DivideMatrixByNumber(zby, 1000); // перевод мм в м
        aan = aan / 1000 / 1000;
        ab = ab / 1000 / 1000; // перевод мм2 в м2
        var gb = new double[triangulation, triangulation];
        var gs = new double[1, zsx.GetLength(1)];
        for (var i = 0; i < triangulation; i++)
        for (var j = 0; j < triangulation; j++)
            gb[i, j] = 1; // коэффициенты учета = 1,0 для первого шага
        for (var i = 0; i < 1; i++)
        for (var j = 0; j < zsx.GetLength(1); j++)
            gs[i, j] = 1;
        // формирование матрицы жесткости для первого шага
        var d11 = Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zbx, 2), gb),
                          ab * eBred))) +
                  Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zsx, 2), gs),
                          aan * ea)));
        var d22 = Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zby, 2), gb),
                          ab * eBred))) +
                  Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zsy, 2), gs),
                          aan * ea)));
        var d12 = Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(
                          Matrix.MultiplyMatrixByMatrix(Matrix.MultiplyMatrixByMatrix(zbx, zby), gb), ab * eBred))) +
                  Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(
                          Matrix.MultiplyMatrixByMatrix(Matrix.MultiplyMatrixByMatrix(zsx, zsy), gs), aan * ea)));
        var d13 = Matrix.VectorSumOfElements(
                      Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zbx, gb), ab * eBred))) +
                  Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zsx, gs), aan * ea)));
        var d23 = Matrix.VectorSumOfElements(
                      Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zby, gb), ab * eBred))) +
                  Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                      Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zsy, gs), aan * ea)));
        var d33 = Matrix.VectorSumOfElements(
                      Matrix.MatrixSumOfElementsInRows(Matrix.MultiplyMatrixByNumber(gb, ab * eBred))) +
                  Matrix.VectorSumOfElements(
                      Matrix.MatrixSumOfElementsInRows(Matrix.MultiplyMatrixByNumber(gs, aan * ea)));
        double[,] d = { { d11, d12, d13 }, { d12, d22, d23 }, { d13, d23, d33 } };
        var tol = 0.001; // допустимая погрешность итерационного решения
        double[,] err = { { 1, 1, 1 } };
        double errMin = 1;
        var sol = new double[3, 1];
        var ss = new double[2, 2];
        var sb = new double[100, 100];
        while (errMin > tol)
        {
            sol = Matrix.SolutionMatrix(Matrix.InverseMatrix(d),
                v); // решение матричного уравнения, определение кривизны и деформаций\
            var eb = Matrix.FoldMatrixByNumber(
                Matrix.FoldMatrixByMatrix(Matrix.MultiplyMatrixByNumber(zbx, sol[0, 0]),
                    Matrix.MultiplyMatrixByNumber(zby, sol[1, 0])), sol[2, 0]); // распределение деформаций в бетоне
            var es = Matrix.FoldMatrixByNumber(
                Matrix.FoldMatrixByMatrix(Matrix.MultiplyMatrixByNumber(zsx, sol[0, 0]),
                    Matrix.MultiplyMatrixByNumber(zsy, sol[1, 0])), sol[2, 0]); // рапсределение деформаций в анкерах
            var mSize = eb.GetLength(0);
            var nSize = eb.GetLength(1);
            var kSize = es.GetLength(0);
            var pSize = es.GetLength(1);
            sb = new double[mSize, nSize];
            for (var i = 0; i < mSize; i++)
            for (var j = 0; j < nSize; j++)
            {
                sb[i, j] = 0;
                gb[i, j] = 0;
            }

            for (var i = 0; i < mSize; i++) // определение напряжений и коэффициентов учета в бетоне
            for (var j = 0; j < nSize; j++)
            {
                sb[i, j] = ConcreteDiagram(eb[i, j], e1, eBred, rb); // обращение к диаграмме бетона
                gb[i, j] = -sb[i, j] / (eb[i, j] * eBred);
            }

            ss = new double[kSize, pSize];
            for (var i = 0; i < kSize; i++)
            for (var j = 0; j < pSize; j++)
            {
                ss[i, j] = 0;
                gs[i, j] = 0;
            }

            for (var i = 0; i < kSize; i++) // определение напряжений и коэффициентов учета для анкеров
            for (var j = 0; j < pSize; j++)
            {
                ss[i, j] = AnchorDiagram(es[i, j], ea); // обращение к диаграмме анкера
                gs[i, j] = ss[i, j] / (es[i, j] * ea);
            }

            // формирование матрицы жесткости для очередного шага
            d[0, 0] = Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(
                              Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zbx, 2), gb), ab * eBred))) +
                      Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(
                              Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zsx, 2), gs), aan * ea)));
            d[1, 1] = Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(
                              Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zby, 2), gb), ab * eBred))) +
                      Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(
                              Matrix.MultiplyMatrixByMatrix(Matrix.PowMatrixByNumber(zsy, 2), gs), aan * ea)));
            d[0, 1] = Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(
                              Matrix.MultiplyMatrixByMatrix(Matrix.MultiplyMatrixByMatrix(zbx, zby), gb),
                              ab * eBred))) +
                      Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(
                              Matrix.MultiplyMatrixByMatrix(Matrix.MultiplyMatrixByMatrix(zsx, zsy), gs), aan * ea)));
            d[0, 2] = Matrix.VectorSumOfElements(
                          Matrix.MatrixSumOfElementsInRows(
                              Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zbx, gb), ab * eBred))) +
                      Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zsx, gs), aan * ea)));
            d[1, 2] = Matrix.VectorSumOfElements(
                          Matrix.MatrixSumOfElementsInRows(
                              Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zby, gb), ab * eBred))) +
                      Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(
                          Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(zsy, gs), aan * ea)));
            d[2, 2] = Matrix.VectorSumOfElements(
                          Matrix.MatrixSumOfElementsInRows(Matrix.MultiplyMatrixByNumber(gb, ab * eBred))) +
                      Matrix.VectorSumOfElements(
                          Matrix.MatrixSumOfElementsInRows(Matrix.MultiplyMatrixByNumber(gs, aan * ea)));
            d[1, 0] = d[0, 1];
            d[2, 0] = d[0, 2];
            d[2, 1] = d[1, 2];
            var u = Matrix.SolutionMatrix(d,
                sol); // определение усилий при уточненной жесткости на данном шаге(расчет в матричном виде)
            var delta = Matrix.DiffMatrixByMatrix(u, v);
            err = Matrix.TransposeMatrix(
                Matrix.AbsMatrix(Matrix.DivideMatrixByMatrix(delta, v))); //оценка относительной погрешности расчета
            errMin = Matrix.MaxNumberInMatrix(err);
        }

        Zby = Matrix.MultiplyMatrixByNumber(zby, multipleYValue);
        Zbx = Matrix.MultiplyMatrixByNumber(zbx, multipleXValue);
        Zbz = Matrix.MultiplyMatrixByNumber(sb, -multipleZValue);
        var nan = Matrix.MultiplyMatrixByNumber(ss, aan * 1000); // усилия в анкерах, кН            
        var nb = Matrix.VectorSumOfElements(Matrix.MatrixSumOfElementsInRows(Matrix.MultiplyMatrixByNumber(sb, ab))) *
                 1000; // равнодействующая сила в бетоне, кН
        var xR = Matrix.VectorSumOfElements(Matrix.MultiplyVectorByNumber(
            Matrix.MatrixSumOfElementsInRows(Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(sb, zbx), ab)),
            1000)) / nb * 1000; // координаты приложения равнодействующей
        var yR = Matrix.VectorSumOfElements(Matrix.MultiplyVectorByNumber(
            Matrix.MatrixSumOfElementsInRows(Matrix.MultiplyMatrixByNumber(Matrix.MultiplyMatrixByMatrix(sb, zby), ab)),
            1000)) / nb * 1000;
    }
}