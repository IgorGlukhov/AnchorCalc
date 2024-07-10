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

    public double[,] Zsy { get; private set; } = new double[1, 1];

    public double[,] Zsx { get; private set; } = new double[1, 1];
    public double[,] Zbz { get; private set; } = new double[1, 1];
    public double[,] Nan { get; private set; } = new double[1, 1];
    public double XR { get; private set; }
    public double YR { get; private set; }
    public double NB { get; private set; }

    public static double ConcreteDiagram(double concreteDeformation, double concreteDiagramParameter,
        double concreteDiagramInverseParameter, double concreteResistance)
    {
        if (concreteDeformation >= -concreteDiagramParameter && concreteDeformation < 0)
            return -concreteDiagramInverseParameter * concreteDeformation;
        if (concreteDeformation >= -0.0035 && concreteDeformation < 0)
            return concreteResistance;
        return 0;
    }

    public static double AnchorDiagram(double anchorDeformation, double elasticModule)
    {
        if (anchorDeformation >= 0)
            return elasticModule * anchorDeformation;
        return 0;
    }

    /// <summary>
    ///     This method contains main business logic of the app
    /// </summary>
    /// <param name="forceValue">Внешняя продольная сила, кН (+растяжение/-сжатие)</param>
    /// <param name="momentXValue">Изгибающий момент Мх, кН*м</param>
    /// <param name="momentYValue">Изгибающий момент Му, кН*м</param>
    /// <param name="toughValue">Жесткость анкера, кН/м</param>
    /// <param name="sealingDepthValue">Глубина заделки анкера, мм</param>
    /// <param name="diameterValue">Диаметр анкера, мм</param>
    /// <param name="concreteResistanceValue">Расчетное сопротивление бетона, МПа</param>
    /// <param name="localDeformationValue">Относительная деформация бетона</param>
    /// <param name="baseWidthValue">Ширина опорной пластины, мм</param>
    /// <param name="baseLengthValue">Длина опорной пластины, мм</param>
    /// <param name="triangulationValue">Кратность разбивки (численного интегрирования)</param>
    /// <param name="anchorCoordinatesXValue">Координаты анкеров по оси X, мм</param>
    /// <param name="anchorCoordinatesYValue">Координаты анкеров по оси У, мм</param>
    /// <param name="multipleXValue">Кратность оси Х</param>
    /// <param name="multipleYValue">Кратность оси Y</param>
    /// <param name="multipleZValue">Кратность оси Z</param>
    public void CalcMatrix(double forceValue, double momentXValue, double momentYValue, double toughValue,
        double sealingDepthValue,
        double diameterValue, double concreteResistanceValue, double localDeformationValue,
        double baseWidthValue, double baseLengthValue, int triangulationValue,
        double[,] anchorCoordinatesXValue, double[,] anchorCoordinatesYValue, int multipleXValue, int multipleYValue,
        int multipleZValue)
    {
        double deformationModule = concreteResistanceValue / localDeformationValue; //модуль приведенной деформации бетона
        int triangulation = triangulationValue + 1; //кратность разбивки (численного интегрирования)
        double baseWidthSegment = baseWidthValue / triangulationValue; //ширина сетки разбивки бетона, мм
        double baseLengthSegment = baseLengthValue / triangulationValue; //длина сетки разбивки бетона, мм
        double areaOfSegment = baseWidthSegment * baseLengthSegment; //площадь сетки разбивки бетона, мм2
        Matrix baseCoordinatesX = new double[triangulation, triangulation];
        Matrix baseCoordinatesY = new double[triangulation, triangulation];
        for (var i = 0; i < triangulation; i++)
        for (var j = 0; j < triangulation; j++)
        {
            baseCoordinatesX[i, j] = baseWidthSegment * j - baseWidthValue * 0.5;
            baseCoordinatesY[i, j] = baseLengthSegment * j - baseLengthValue * 0.5;
        }

        baseCoordinatesY=baseCoordinatesY.Transpose(); //массивы координат ячеек сетки бетона
        double anchorSectionArea = Math.PI * diameterValue * diameterValue / 4; // площадь сечения анкера номинальная, мм2
        double elasticModule =
            toughValue * sealingDepthValue / anchorSectionArea * 1000; // приведенный модуль упругости анкера, МПа
        double[,] forceVector =
            { { momentXValue / 1000 }, { momentYValue / 1000 }, { forceValue / 1000 } }; //вектор нагрузок МН (МН*м)
        Matrix anchorCoordinatesX = anchorCoordinatesXValue;
        Matrix anchorCoordinatesY = anchorCoordinatesYValue;
        anchorCoordinatesX/=1000;
        anchorCoordinatesY /= 1000;
        baseCoordinatesX /= 1000;
        baseCoordinatesY /= 1000; // перевод мм в м
        anchorSectionArea = anchorSectionArea / 1000 / 1000;
        areaOfSegment = areaOfSegment / 1000 / 1000; // перевод мм2 в м2
        Matrix primaryBaseMatrix = new double[triangulation, triangulation];
        Matrix primaryAnchorMatrix = new double[1, anchorCoordinatesX.GetLength(1)];
        for (var i = 0; i < triangulation; i++)
        for (var j = 0; j < triangulation; j++)
            primaryBaseMatrix[i, j] = 1; // коэффициенты учета = 1,0 для первого шага
        for (var i = 0; i < 1; i++)
        for (var j = 0; j < anchorCoordinatesX.GetLength(1); j++)
            primaryAnchorMatrix[i, j] = 1;
        if (anchorCoordinatesX.GetLength(1) > 1)
        {
            // формирование матрицы жесткости для первого шага
            Matrix matrixEquation = new double [3, 3];
            matrixEquation[0, 0] = (baseCoordinatesX * baseCoordinatesX * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                   (anchorCoordinatesX * anchorCoordinatesX * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
            matrixEquation[1, 1] = (baseCoordinatesY * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                   (anchorCoordinatesY * anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
            matrixEquation[0, 1] = (baseCoordinatesX * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                   (anchorCoordinatesX * anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
            matrixEquation[0, 2] = (baseCoordinatesX * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                   (anchorCoordinatesX * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
            matrixEquation[1, 2] = (baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                   (anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
            matrixEquation[2, 2] = (primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                   (primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
            matrixEquation[1, 0] = matrixEquation[0, 1];
            matrixEquation[2, 0] = matrixEquation[0, 2];
            matrixEquation[2, 1] = matrixEquation[1, 2];
            var error = 0.001; // допустимая погрешность итерационного решения
            double actualError = 1;
            Matrix anchorTensionMatrix = new double[1, 1];
            Matrix concreteTensionMatrix = new double[1, 1];
            while (actualError > error)
            {
                Matrix solutionOfEquation = matrixEquation.Inverse().Solution(forceVector); // решение матричного уравнения, определение кривизны и деформаций
                Matrix concreteDeformations = baseCoordinatesX* solutionOfEquation[0, 0]+baseCoordinatesY* solutionOfEquation[1, 0]+ solutionOfEquation[2, 0]; // распределение деформаций в бетоне
                Matrix anchorDeformations = anchorCoordinatesX * solutionOfEquation[0, 0] + anchorCoordinatesY * solutionOfEquation[1, 0] + solutionOfEquation[2, 0]; // рапсределение деформаций в анкерах

                concreteTensionMatrix =
                    new double[concreteDeformations.GetLength(0), concreteDeformations.GetLength(1)];

                for (var i = 0;
                     i < concreteDeformations.GetLength(0);
                     i++) // определение напряжений и коэффициентов учета в бетоне
                for (var j = 0; j < concreteDeformations.GetLength(1); j++)
                {
                    concreteTensionMatrix[i, j] = ConcreteDiagram(concreteDeformations[i, j], localDeformationValue,
                        deformationModule, concreteResistanceValue); // обращение к диаграмме бетона
                    primaryBaseMatrix[i, j] = -concreteTensionMatrix[i, j] / (concreteDeformations[i, j] * deformationModule);
                }

                anchorTensionMatrix = new double[anchorDeformations.GetLength(0), anchorDeformations.GetLength(1)];

                for (var i = 0; i < anchorDeformations.GetLength(0);
                     i++) // определение напряжений и коэффициентов учета для анкеров
                for (var j = 0; j < anchorDeformations.GetLength(1); j++)
                {
                    anchorTensionMatrix[i, j] = AnchorDiagram(anchorDeformations[i, j], elasticModule); // обращение к диаграмме анкера
                    primaryAnchorMatrix[i, j] = anchorTensionMatrix[i, j] / (anchorDeformations[i, j] * elasticModule);
                }

                // формирование матрицы жесткости для очередного шага
               matrixEquation[0, 0] = (baseCoordinatesX * baseCoordinatesX * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                               (anchorCoordinatesX * anchorCoordinatesX * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
               matrixEquation[1, 1] = (baseCoordinatesY * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                      (anchorCoordinatesY * anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
               matrixEquation[0, 1] = (baseCoordinatesX * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                      (anchorCoordinatesX * anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
               matrixEquation[0, 2] = (baseCoordinatesX * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                      (anchorCoordinatesX * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
               matrixEquation[1, 2] = (baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                      (anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
               matrixEquation[2, 2] = (primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                                      (primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
               matrixEquation[1, 0] = matrixEquation[0, 1];
               matrixEquation[2, 0] = matrixEquation[0, 2];
               matrixEquation[2, 1] = matrixEquation[1, 2];
               Matrix updatedSolutionOfEquation = matrixEquation.Solution(solutionOfEquation); // определение усилий при уточненной жесткости на данном шаге(расчет в матричном виде)
               Matrix differenceBetweenSteps = updatedSolutionOfEquation- forceVector;
                actualError = differenceBetweenSteps.Abs().Transpose().Max(); //оценка относительной погрешности расчета
            }

            Zby = baseCoordinatesY * multipleYValue;
            Zbx = baseCoordinatesX * multipleXValue;
            Zbz = concreteTensionMatrix *-multipleZValue;
            Nan = anchorTensionMatrix* anchorSectionArea * 1000; // усилия в анкерах, кН
            Zsx = anchorCoordinatesX;
            Zsy = anchorCoordinatesY;
            NB = (concreteTensionMatrix * areaOfSegment).Sum().Sum() * 1000; // равнодействующая сила в бетоне, кН
            XR = (concreteTensionMatrix * baseCoordinatesX * areaOfSegment).Sum().Sum()*1000 / NB * 1000; // координаты приложения равнодействующей
            YR = (concreteTensionMatrix * baseCoordinatesY * areaOfSegment).Sum().Sum()*1000 / NB * 1000;
        }
        else
        {
            Nan = new[,] { { forceValue } };
            Zby = baseCoordinatesY * multipleYValue;
            Zbx = baseCoordinatesX * multipleXValue;
            Zbz = new Matrix(new double[triangulationValue, triangulationValue])* -multipleZValue;
        }
    }

    public void Validate(double NormativeResistance, double gammaNs, bool isPtr, double Nnptr, double Nnpwihtouttr, double gammaNp,

        double phiC, double gammaNc, double Rbn, double hef, double A, double B, double scrSP, double ccrSP, double gammaNsp, double hmin, double hfact)
    {
        double nanMax = 0;
        for (var i = 0; i < Nan.Length; i++)
            if (nanMax < Nan[0, i])
                nanMax = Nan[0, i];

        //double NormativeResistance = 0; //нормативное значение силы сопротивления анкера
        //при разрушении по стали, принимаемое в зависимости от 
        //типа и марки анкера по техпаспорту
        //double gammaNs = 0; //коэффициент надежности по стали при растяжении
        //, принимаемое в зависимости от типа и марки анкера по техпаспорту

        //****ПЕРВАЯ ПРОВЕР ОЧКА****
        var Nult1 = NormativeResistance / gammaNs;
        if (nanMax < Nult1)
            Console.WriteLine("zaebis");
        //****ПЕРВАЯ ПРОВЕР ОЧКА****

        //var isPtr = true; //наличие трещин
        //double Nnptr = 0; //нормативное значение силы сопротивления анкера
        //сцепления с основанием (по контакту), принимаемое в зависимости от 
        //типа и марки анкера по техпаспорту для основания с трещинами
        //double Nnpwihtouttr = 0; //нормативное значение силы сопротивления анкера
        //сцепления с основанием (по контакту), принимаемое в зависимости от 
        //типа и марки анкера по техпаспорту для основания без трещин
        var gammaBt = 1.5; //Коэффициент надежности по бетону при растяжении
        //double gammaNp = 0; //коэффициент условий работы анкера, принимаемый в зависимости от типа и марки анкера по техпаспорту
        //double phiC = 0; //коэффициент, учитывающий фактическую прочность бетонного основания, принимаемый
        //в зависимости от класса бетона на сжатие и типа и марки анкера по техпаспорту

        //****ВТОРАЯ ПРОВЕР ОЧКА****
        double Nult2 = 0;
        if (isPtr)
            Nult2 = Nnptr * phiC / gammaNp * gammaBt;
        else
            Nult2 = Nnpwihtouttr * phiC / gammaNp * gammaBt;
        if (nanMax < Nult2)
            Console.WriteLine("zaebis");
        //****ВТОРАЯ ПРОВЕР ОЧКА****
        //double gammaNc = 0; //коэффициент условий работы анкера при выкалывании бетонного основания, принимаемый в зависимости от типа и марки анкера по техпаспорту
        double k1 = 0; // коэффициент зависящий от наличия трещин
        if (isPtr)
            k1 = 7.9;
        else
            k1 = 11.3;
        //double Rbn = 0; //нормативное сопротивление бетона сжатию, принимаемое по СП 63.13330 в зависимости от класса бетона на сжатие, МПа*****УЖЕ ЕСТЬ В МАТРИКС КАЛЬКЕ********
        //double hef = 0; //эффективная глубина анкеровки, принимаемая в зависимости от типа и марки анкера по техпаспорту*****УЖЕ ЕСТЬ В МАТРИКС КАЛЬКЕ********
        var Nonc = k1 * Math.Pow(Rbn, 0.5) *
                   Math.Pow(hef,
                       1.5); //значение силы сопротивления, Н, для одиночного анкера, расположенного на значительном удалении от края основания соседнего анкера, при разрушении от выкалывания бетонного основания
        var scrN = 3 * hef; //критическое расстояние между анкерами (межосевое)
        var ccrN = 1.5 * hef; //критическое краевое расстояние
        double
            AcN = 0; //фактическая площадь основания условной призмы выкалывания, с учетом влияния соседних анкеров, а также влияния краевого расположения
        double
            AocN = scrN * scrN; //площадь основания условной призмы выкалывания для одиночного анкера, расположенного на значительном удалении от края основания и соседнего анкера

        //double A = 0; //ширина бетонного основания
        //double B = 0; //длина бетонного основания
        double[] rverh = [];
        double[] rniz = [];
        double[] rlevo = [];
        double[] rpravo = [];
        var coordV = B / 2;
        var coordN = -B / 2;
        var coordL = -A / 2;
        var coordP = A / 2;
        bool[] isCrayCriticV = [];
        bool[] isCrayCriticN = [];
        bool[] isCrayCriticL = [];
        bool[] isCrayCriticP = [];
        bool[] isAxisCriticV = [];
        bool[] isAxisCriticN = [];
        bool[] isAxisCriticL = [];
        bool[] isAxisCriticP = [];
        double cmax = 0; //фактическое краевое расстояние, надо считать в зависимости от координат анкеров и размеров пластины
        var cmin = ccrN;
        double smax = 0; //фактическое межосевое расстояние между анкерами, считается в зависимотси от координат
        double[] criticcount = [];
        for (var i = 0; i < Nan.GetLength(1); i++)
        {
            if (Math.Abs(coordV - Zsy[0, i]) < ccrN)
            {
                rverh[i] = Math.Abs(coordV - Zsy[0, i]);
                isCrayCriticV[i] = true;
                criticcount[i] += 1;
                if (cmin > rverh[i]) cmin = rverh[i];
            }

            if (Math.Abs(coordN - Zsy[0, i]) < ccrN)
            {
                rniz[i] = Math.Abs(coordN - Zsy[0, i]);
                isCrayCriticN[i] = true;
                criticcount[i] += 1;
                if (cmin > rniz[i]) cmin = rniz[i];
            }

            if (Math.Abs(coordL - Zsx[0, i]) < ccrN)
            {
                rlevo[i] = Math.Abs(coordL - Zsx[0, i]);
                isCrayCriticL[i] = true;
                criticcount[i] += 1;
                if (cmin > rlevo[i]) cmin = rlevo[i];
            }

            if (Math.Abs(coordP - Zsx[0, i]) < ccrN)
            {
                rpravo[i] = Math.Abs(coordP - Zsx[0, i]);
                isCrayCriticP[i] = true;
                criticcount[i] += 1;
                if (cmin > rpravo[i]) cmin = rpravo[i];
            }

            if (Nan.GetLength(1) > 1)
                for (var j = 0; j < Nan.GetLength(1); j++)
                    if (Math.Pow(Math.Pow(Zsx[0, i] - Zsx[0, j], 2) + Math.Pow(Zsy[0, i] - Zsy[0, j], 2), 0.5) <
                        scrN)
                    {
                        if (Zsx[0, i] < Zsx[0, j])
                        {
                            rpravo[i] = Math.Abs(Zsx[0, i] - Zsx[0, j]) / 2;
                            isAxisCriticP[i] = true;
                            criticcount[i] += 1;
                        }

                        if (Zsx[0, i] > Zsx[0, j])
                        {
                            rlevo[i] = Math.Abs(Zsx[0, i] - Zsx[0, j]) / 2;
                            isAxisCriticL[i] = true;
                            criticcount[i] += 1;
                        }

                        if (Zsy[0, i] < Zsy[0, j])
                        {
                            rverh[i] = Math.Abs(Zsy[0, i] - Zsy[0, j]) / 2;
                            isAxisCriticV[i] = true;
                            criticcount[i] += 1;
                        }

                        if (Zsy[0, i] > Zsy[0, j])
                        {
                            rniz[i] = Math.Abs(Zsy[0, i] - Zsy[0, j]) / 2;
                            isAxisCriticN[i] = true;
                            criticcount[i] += 1;
                        }
                    }

            if (criticcount[i] > 2)
            {
                if (isCrayCriticV[i] & (rverh[i] > cmax)) cmax = rverh[i];

                if (isCrayCriticN[i] & (rniz[i] > cmax)) cmax = rniz[i];

                if (isCrayCriticL[i] & (rlevo[i] > cmax)) cmax = rlevo[i];

                if (isCrayCriticP[i] & (rpravo[i] > cmax)) cmax = rpravo[i];

                if (isAxisCriticV[i] & (rverh[i] > smax)) smax = rverh[i];

                if (isAxisCriticN[i] & (rniz[i] > smax)) smax = rniz[i];

                if (isAxisCriticL[i] & (rlevo[i] > smax)) smax = rlevo[i];

                if (isAxisCriticP[i] & (rpravo[i] > smax)) smax = rpravo[i];
                if (cmax / 1.5 > smax / 3)
                    hef = cmax / 1.5;
                else
                    hef = smax / 3;
                scrN = 3 * hef;
                ccrN = 1.5 * hef;
            }

            AcN += (rlevo[i] + rpravo[i]) * (rniz[i] + rverh[i]);
        }

        for (var i = 0; i < Nan.GetLength(1); i++)
        {
            if (Math.Abs(coordV - Zsy[0, i]) < ccrN) rverh[i] = Math.Abs(coordV - Zsy[0, i]);
            if (Math.Abs(coordN - Zsy[0, i]) < ccrN) rniz[i] = Math.Abs(coordN - Zsy[0, i]);
            if (Math.Abs(coordL - Zsx[0, i]) < ccrN) rlevo[i] = Math.Abs(coordL - Zsx[0, i]);
            if (Math.Abs(coordP - Zsx[0, i]) < ccrN) rpravo[i] = Math.Abs(coordP - Zsx[0, i]);
            if (Nan.Length > 1)
                for (var j = 0; j < Nan.Length; j++)
                    if (Math.Pow(Math.Pow(Zsx[0, i] - Zsx[0, j], 2) + Math.Pow(Zsy[0, i] - Zsy[0, j], 2), 0.5) <
                        scrN)
                    {
                        if (Zsx[0, i] < Zsx[0, j]) rpravo[i] = Math.Abs(Zsx[0, i] - Zsx[0, j]) / 2;

                        if (Zsx[0, i] > Zsx[0, j]) rlevo[i] = Math.Abs(Zsx[0, i] - Zsx[0, j]) / 2;

                        if (Zsy[0, i] < Zsy[0, j]) rverh[i] = Math.Abs(Zsy[0, i] - Zsy[0, j]) / 2;

                        if (Zsy[0, i] > Zsy[0, j]) rniz[i] = Math.Abs(Zsy[0, i] - Zsy[0, j]) / 2;
                    }

            AcN += (rlevo[i] + rpravo[i]) * (rniz[i] + rverh[i]);
        }

        var PhiSN = 0.7 + 0.3 * cmin / ccrN; //коэффициент влияния установки у края основания
        if (PhiSN > 1) PhiSN = 1;
        var PhireN = 0.5 + hef / 200; //коэффициент влияния установки в защитном слое гутсоармированных конструкций
        if (PhireN > 1) PhireN = 1;
        double PhiecN = 1;
        if (Nan.GetLength(1) > 1)
            PhiecN = 1 / (1 + 2 * XR / scrN) *
                     (1 / (1 + 2 * YR / scrN)); //коэффициент влияния неравномерного загружения анкерной группы
        if (PhiecN > 1) PhiecN = 1;
        var Nult3 = Nonc / (gammaBt * gammaNc) * (AcN / AocN) * PhiSN * PhireN * PhiecN;
        //****ТРЕТЬЯ ПРОВЕР ОЧКА****

        if (Nan.GetLength(1) > 1 && NB < Nult3)
        {
            Console.WriteLine("zaebis");
        }
        else if (Nan.GetLength(1) == 1 && nanMax < Nult3)
        {
            Console.WriteLine("zaebis");
        }
            //****ТРЕТЬЯ ПРОВЕР ОЧКА****

            //double scrSP = 0; //критическое межосевое расстояние по тех паспорту
        //double ccrSP = 0; //критическое краевоерасстояние по тех паспорту
        AocN = scrSP * scrSP;
        PhiSN = 0.7 + 0.3 * cmin / ccrSP;
        if (Nan.Length > 1)
            PhiecN = 1 / (1 + 2 * XR / scrSP) *
                     (1 / (1 + 2 * YR / scrSP)); //коэффициент влияния неравномерного загружения анкерной группы
        if (PhiecN > 1) PhiecN = 1;
        var Nspnc = Nonc / (gammaBt * gammaNc) * (AcN / AocN) * PhiSN * PhireN * PhiecN;
        //double gammaNsp = 0; //коэффициент условий работы анкера при расзрушении отрасклаывания основания при растяжениии, принимаемый в зависимости от типа и марки анкера по тех паспорту
        //double hmin = 0; // минимальная толщина основания по тех паспорту
        //double hfact = 0; //фактическая толщина основания
        var PhihSP = Math.Pow(hfact / hmin, 2 / 3);
        if (PhihSP > Math.Pow(2 * hef / hmin, 2 / 3)) PhihSP = Math.Pow(2 * hef / hmin, 2 / 3);

        var Nult4 = Nspnc * PhihSP / gammaNsp;

        //****ЧЕТВЕРТАЯ ПРОВЕР ОЧКА****
        if (Nan.GetLength(1) > 1 && NB < Nult4)
        {
            Console.WriteLine("zaebis");
        }
        else if (Nan.GetLength(1) == 1 && nanMax < Nult4)
        {
            Console.WriteLine("zaebis");
        }
        //****ЧЕТВЕРТАЯ ПРОВЕР ОЧКА****
    }
}