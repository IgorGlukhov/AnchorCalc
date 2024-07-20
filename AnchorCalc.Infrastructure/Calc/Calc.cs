namespace AnchorCalc.Infrastructure.Calc;

public class Calc
{
    public Calc(double forceValue, double momentXValue, double momentYValue, double sealingDepthValue,
        double diameterValue, double concreteResistanceValue, double localDeformationValue, double basePlateWidthValue,
        double basePlateLengthValue, int triangulationValue,
        double[,] anchorCoordinatesXValue, double[,] anchorCoordinatesYValue, double normativeResistance,
        double crackedNormativeForce, double uncrackedNormativeForce,
        double concreteBaseWidth, double concreteBaseLength, double criticInterAxialDistance, double criticEdgeDistance,
        double minBaseHeight,
        double factBaseHeight, double phiC, double gammaNs, double gammaNp, double gammaNc, double gammaNsp,
        bool isCracked)
    {
        Force = forceValue;
        MomentX = momentXValue;
        MomentY = momentYValue;
        SealingDepth = sealingDepthValue;
        ConcreteResistance = concreteResistanceValue;
        Diameter = diameterValue;
        LocalDeformation = localDeformationValue;
        BasePlateLength = basePlateLengthValue;
        BasePlateWidth = basePlateWidthValue;
        Triangulation = triangulationValue;
        AnchorCoordinatesX = anchorCoordinatesXValue;
        AnchorCoordinatesY = anchorCoordinatesYValue;
        NormativeResistance = normativeResistance;
        CrackedNormativeForce = crackedNormativeForce;
        UncrackedNormativeForce = uncrackedNormativeForce;
        ConcreteBaseWidth = concreteBaseWidth;
        ConcreteBaseLength = concreteBaseLength;
        CriticInterAxialDistance = criticInterAxialDistance;
        CriticEdgeDistance = criticEdgeDistance;
        MinBaseHeight = minBaseHeight;
        FactBaseHeight = factBaseHeight;
        PhiC = phiC;
        GammaNs = gammaNs;
        GammaNp = gammaNp;
        GammaNc = gammaNc;
        GammaNsp = gammaNsp;
        IsCracked = isCracked;

        DeformationModel();
        AnchorValidate();
    }

    public double ConcreteDiagram(double concreteDeformation, double deformationModule)
    {
        if (concreteDeformation >= -LocalDeformation && concreteDeformation < 0)
            return -deformationModule * concreteDeformation;

        if (concreteDeformation >= -0.0035 && concreteDeformation < 0)
            return ConcreteResistance;

        return 0;
    }

    public static double AnchorDiagram(double anchorDeformation, double elasticModule)
    {
        if (anchorDeformation >= 0)
            return elasticModule * anchorDeformation;

        return 0;
    }


    public void DeformationModel()
    {
        var triangulation = Triangulation + 1; //кратность разбивки (численного интегрирования)
        var deformationModule = ConcreteResistance / LocalDeformation; //модуль приведенной деформации бетона
        var baseWidthSegment = BasePlateWidth / Triangulation; //ширина сетки разбивки бетона, мм
        var baseLengthSegment = BasePlateLength / Triangulation; //длина сетки разбивки бетона, мм
        var areaOfSegment = baseWidthSegment * baseLengthSegment / 1000 / 1000; //площадь сетки разбивки бетона, мм2
        var anchorSectionArea =
            Math.PI * Diameter * Diameter / 4 / 1000 / 1000; // площадь сечения анкера номинальная, мм2
        double anchorTough = 100;
        var elasticModule =
            anchorTough * SealingDepth / anchorSectionArea / 1000; // приведенный модуль упругости анкера, МПа
        double[,] forceVector =
            { { MomentX / 1000 }, { MomentY / 1000 }, { Force / 1000 } }; //вектор нагрузок МН (МН*м)

        Matrix baseCoordinatesX = new double[triangulation, triangulation];
        Matrix baseCoordinatesY = new double[triangulation, triangulation];
        Matrix primaryBaseMatrix = new double[triangulation, triangulation];

        for (var i = 0; i < triangulation; i++)
        for (var j = 0; j < triangulation; j++)
        {
            baseCoordinatesX[i, j] = (baseWidthSegment * j - BasePlateWidth * 0.5) / 1000;
            baseCoordinatesY[j, i] = (baseLengthSegment * j - BasePlateLength * 0.5) / 1000;
            primaryBaseMatrix[i, j] = 1; // коэффициенты учета = 1,0 для первого шага
        }

        Matrix anchorCoordinatesX = AnchorCoordinatesX;
        Matrix anchorCoordinatesY = AnchorCoordinatesY;
        Matrix primaryAnchorMatrix = new double[1, anchorCoordinatesX.GetLength(1)];

        for (var i = 0; i < 1; i++)
        for (var j = 0; j < anchorCoordinatesX.GetLength(1); j++)
            primaryAnchorMatrix[i, j] = 1;

        if (anchorCoordinatesX.GetLength(1) > 1)
        {
            // формирование матрицы жесткости для первого шага
            Matrix matrixEquation = new double [3, 3];
            matrixEquation[0, 0] =
                (baseCoordinatesX * baseCoordinatesX * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                (anchorCoordinatesX * anchorCoordinatesX * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();

            matrixEquation[1, 1] =
                (baseCoordinatesY * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                (anchorCoordinatesY * anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();

            matrixEquation[0, 1] =
                (baseCoordinatesX * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
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
                var solutionOfEquation =
                    matrixEquation.Inverse()
                        .Solution(forceVector); // решение матричного уравнения, определение кривизны и деформаций
                var concreteDeformations = baseCoordinatesX * solutionOfEquation[0, 0] +
                                           baseCoordinatesY * solutionOfEquation[1, 0] +
                                           solutionOfEquation[2, 0]; // распределение деформаций в бетоне
                var anchorDeformations = anchorCoordinatesX * solutionOfEquation[0, 0] +
                                         anchorCoordinatesY * solutionOfEquation[1, 0] +
                                         solutionOfEquation[2, 0]; // рапсределение деформаций в анкерах

                concreteTensionMatrix =
                    new double[concreteDeformations.GetLength(0), concreteDeformations.GetLength(1)];

                for (var i = 0;
                     i < concreteDeformations.GetLength(0);
                     i++) // определение напряжений и коэффициентов учета в бетоне
                for (var j = 0; j < concreteDeformations.GetLength(1); j++)
                {
                    concreteTensionMatrix[i, j] =
                        ConcreteDiagram(concreteDeformations[i, j], deformationModule); // обращение к диаграмме бетона
                    primaryBaseMatrix[i, j] =
                        -concreteTensionMatrix[i, j] / (concreteDeformations[i, j] * deformationModule);
                }

                anchorTensionMatrix = new double[anchorDeformations.GetLength(0), anchorDeformations.GetLength(1)];

                for (var i = 0;
                     i < anchorDeformations.GetLength(0);
                     i++) // определение напряжений и коэффициентов учета для анкеров
                for (var j = 0; j < anchorDeformations.GetLength(1); j++)
                {
                    anchorTensionMatrix[i, j] =
                        AnchorDiagram(anchorDeformations[i, j], elasticModule); // обращение к диаграмме анкера
                    primaryAnchorMatrix[i, j] = anchorTensionMatrix[i, j] / (anchorDeformations[i, j] * elasticModule);
                }

                // формирование матрицы жесткости для очередного шага
                matrixEquation[0, 0] =
                    (baseCoordinatesX * baseCoordinatesX * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                    (anchorCoordinatesX * anchorCoordinatesX * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();

                matrixEquation[1, 1] =
                    (baseCoordinatesY * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
                    (anchorCoordinatesY * anchorCoordinatesY * primaryAnchorMatrix * (anchorSectionArea * elasticModule)).Sum().Sum();
                
                matrixEquation[0, 1] =
                    (baseCoordinatesX * baseCoordinatesY * primaryBaseMatrix * (areaOfSegment * deformationModule)).Sum().Sum() +
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

                var updatedSolutionOfEquation =
                    matrixEquation.Solution(
                        solutionOfEquation); // определение усилий при уточненной жесткости на данном шаге(расчет в матричном виде)
                actualError =
                    (updatedSolutionOfEquation - forceVector).Abs().Transpose()
                    .Max(); //оценка относительной погрешности расчета
            }

            BasePlateCoordinatesY = baseCoordinatesY * 1000;
            BasePlateCoordinatesX = baseCoordinatesX * 1000;
            ConcreteTensionValues = concreteTensionMatrix * -10;
            AnchorForce = anchorTensionMatrix * anchorSectionArea * 1000;
            AnchorCoordinatesX = anchorCoordinatesX;
            AnchorCoordinatesY = anchorCoordinatesY;
            ResultantForce = (concreteTensionMatrix * areaOfSegment).Sum().Sum() * 1000;
            ResultantCoordinatesX = (concreteTensionMatrix * baseCoordinatesX * areaOfSegment).Sum().Sum() * 1000 *
                1000 / ResultantForce;
            ResultantCoordinatesY = (concreteTensionMatrix * baseCoordinatesY * areaOfSegment).Sum().Sum() * 1000 *
                1000 / ResultantForce;
        }
        else
        {
            AnchorForce = new[,] { { Force } };
            BasePlateCoordinatesY = baseCoordinatesY * 1000;
            BasePlateCoordinatesX = baseCoordinatesX * 1000;
            ConcreteTensionValues = new Matrix(new double[Triangulation, Triangulation]) * -10;
        }
    }

    public void AnchorValidate()
    {
        var maxForce = ((Matrix)AnchorForce).Max();
        var sumForce = ((Matrix)AnchorForce).Sum().Sum();
        AnchorValidateSteel = maxForce / (NormativeResistance / GammaNs) * 100;


        var gammaBt = 1.5; //Коэффициент надежности по бетону при растяжении по СП 63.13330.2018
        var forceUltContact = IsCracked
            ? CrackedNormativeForce * PhiC / GammaNp * gammaBt
            : UncrackedNormativeForce * PhiC / GammaNp * gammaBt;
        AnchorValidateContact = maxForce / forceUltContact * 100;


        var k1 = IsCracked ? 7.9 : 11.3; // Коэффициент зависящий от наличия трещин
        var excavationNormativeResistance = k1 * Math.Pow(ConcreteResistance, 0.5) * Math.Pow(SealingDepth, 1.5) / 1000;
        //Значение силы сопротивления, кН, для одиночного анкера, расположенного на значительном удалении от края основания соседнего анкера,
        //при разрушении от выкалывания бетонного основания
        var normativeCriticInterAxialDistance =
            3 * SealingDepth; //Расчетное критическое межосевое расстояние между анкерами, мм
        var phiRen =
            0.5 + SealingDepth / 200; //Коэффициент влияния установки в защитном слое гутсоармированных конструкций
        if (phiRen > 1) phiRen = 1;
        double phiEcn = 1; //Коэффициент влияния неравномерного загружения анкерной группы

        if (AnchorForce.GetLength(1) > 1)
            phiEcn = 1; // ((1 + 2 * Math.Abs(ResultantCoordinatesX) / normativeCriticInterAxialDistance) * 
        //(1 + 2 * Math.Abs(ResultantCoordinatesY) / normativeCriticInterAxialDistance)); 
        if (phiEcn > 1) phiEcn = 1;
        var forceUltExcavation = excavationNormativeResistance / (gammaBt * GammaNc) * phiRen * phiEcn;
        AnchorValidateExcavation = AnchorForce.GetLength(1) switch
        {
            > 1 => maxForce / forceUltExcavation * 100,
            _ => maxForce / forceUltExcavation * 100
        };


        if (AnchorForce.Length > 1)
            phiEcn = 1; // ((1 + 2 * Math.Abs(ResultantCoordinatesX) / CriticInterAxialDistance) *
        //(1 + 2 * Math.Abs(ResultantCoordinatesY) / CriticInterAxialDistance)); 
        //коэффициент влияния неравномерного загружения анкерной группы
        if (phiEcn > 1) phiEcn = 1;
        var splittingNormativeResistance = excavationNormativeResistance / (gammaBt * GammaNc) * phiRen * phiEcn;
        var phiHsp = Math.Pow(FactBaseHeight / MinBaseHeight, 0D);
        if (phiHsp > Math.Pow(2 * SealingDepth / MinBaseHeight, 0D))
            phiHsp = Math.Pow(2 * SealingDepth / MinBaseHeight, 0D);
        var forceUltSplitting = splittingNormativeResistance * phiHsp / GammaNsp;
        AnchorValidateSplitting = AnchorForce.GetLength(1) switch
        {
            > 1 => maxForce / forceUltSplitting * 100,
            _ => maxForce / forceUltSplitting * 100
        };
    }

    #region DeformationModelProperties

    public double[,] BasePlateCoordinatesX { get; private set; } =
        new double[1, 1]; // Координаты Х сетки напряжений, мм

    public double[,] BasePlateCoordinatesY { get; private set; } =
        new double[1, 1]; // Координаты Y сетки напряжений, мм

    public double[,] ConcreteTensionValues { get; private set; } = new double[1, 1]; // Напряжения в бетоне, МПа

    public double[,] AnchorForce { get; private set; } = new double[1, 1]; // Усилия в анкерах, кН

    public double ResultantCoordinatesX { get; private set; } // Координаты приложения равнодействующей, мм

    public double ResultantCoordinatesY { get; private set; } // Координаты приложения равнодействующей, мм

    public double ResultantForce { get; private set; } // Равнодействующая сила в бетоне, кН

    public double[,] AnchorCoordinatesX { get; private set; } //Координаты анкеров по оси X, мм

    public double[,] AnchorCoordinatesY { get; private set; } //Координаты анкеров по оси Y, мм

    public double Force { get; } //Внешняя продольная сила, кН (+растяжение/-сжатие)

    public double MomentX { get; } //Изгибающий момент Мх, кН*м

    public double MomentY { get; } //Изгибающий момент Му, кН*м

    public double
        SealingDepth
    {
        get;
        set;
    } //Эффективная глубина анкеровки, принимаемая в зависимости от типа и марки анкера по техпаспорту, мм

    public double ConcreteResistance { get; } //Расчетное сопротивление бетона, МПа

    public double LocalDeformation { get; } //Относительная деформация бетона

    public double BasePlateWidth { get; } //Ширина опорной пластины, мм

    public double BasePlateLength { get; } //Длина опорной пластины, мм

    public int Triangulation { get; } //Кратность разбивки (численного интегрирования)

    public double Diameter { get; } //Диаметр анкера, мм

    #endregion

    #region AnchorValidateProperties

    /*Нормативное значение силы сопротивления анкера
    при разрушении по стали, принимаемое в зависимости от
    типа и марки анкера по техпаспорту, кН*/
    public double NormativeResistance { get; }

    /*Нормативное значение силы сопротивления анкера
    сцепления с основанием (по контакту), принимаемое в зависимости от
    типа и марки анкера по техпаспорту для основания с трещинами, кН*/
    public double CrackedNormativeForce { get; }

    /*Нормативное значение силы сопротивления анкера
    сцепления с основанием (по контакту), принимаемое в зависимости от
    типа и марки анкера по техпаспорту для основания без трещин, кН*/
    public double UncrackedNormativeForce { get; }

    /*Ширина бетонного основания, мм*/
    public double ConcreteBaseWidth { get; }

    /*Длина бетонного основания, мм*/
    public double ConcreteBaseLength { get; }

    /*Критическое межосевое расстояние, принимаемое в зависимости от
    типа и марки анкера по техпаспорту*/
    public double CriticInterAxialDistance { get; }

    /*Критическое краевое расстояние, принимаемое в зависимости от
    типа и марки анкера по техпаспорту*/
    public double CriticEdgeDistance { get; }

    /*Минимальная высота бетонного основания, принимаемое в зависимости от
    типа и марки анкера по техпаспорту, мм*/
    public double MinBaseHeight { get; }

    /*Фактическая высота бетонного основания, мм*/
    public double FactBaseHeight { get; }

    /*Коэффициент, учитывающий фактическую прочность бетонного основания,
    принимаемый в зависимости от типа и марки анкера по техпаспорту*/
    public double PhiC { get; }

    /*Коэффициент надежности по стали при растяжении,
    принимаемый в зависимости от типа и марки анкера по техпаспорту*/
    public double GammaNs { get; }

    /*Коэффициент условий работы анкера при разрыве контакта с бетонным основанием,
    принимаемый в зависимости от типа и марки анкера по техпаспорту*/
    public double GammaNp { get; }

    /*Коэффициент условий работы анкера при выкалывании бетонного основания,
    принимаемый в зависимости от типа и марки анкера по техпаспорту*/
    public double GammaNc { get; }

    /*Коэффициент условий работы анкера при раскалывании бетонного основания,
    принимаемый в зависимости от типа и марки анкера по техпаспорту*/
    public double GammaNsp { get; }

    /*Наличие трещин*/
    public bool IsCracked { get; }

    /*Проверка на разрушение по стали анкера*/
    public double AnchorValidateSteel { get; set; }

    /*Проверка на разрушение по контакту с основанием*/
    public double AnchorValidateContact { get; set; }

    /*Проверка на разрушение от выкалывания бетонного основания*/
    public double AnchorValidateExcavation { get; set; }

    /*Проверка на разрушение от раскалывания бетонного основания*/
    public double AnchorValidateSplitting { get; set; }

    #endregion


    #region future features

    /*double factArea = 0; //фактическая площадь основания условной призмы выкалывания, с учетом влияния соседних анкеров, а также влияния краевого расположения
        double defaultArea = normativeCriticInterAxialDistance * normativeCriticInterAxialDistance; //площадь основания условной призмы выкалывания для одиночного анкера, расположенного на значительном удалении от края основания и соседнего анкера
        double[] up = [];
        double[] down = [];
        double[] left = [];
        double[] right = [];
        var baseUp = ConcreteBaseLength / 2;
        var baseDown = -ConcreteBaseLength / 2;
        var baseLeft = -ConcreteBaseWidth / 2;
        var baseRight = ConcreteBaseWidth / 2;
        bool[] isEdgeCriticUp = [];
        bool[] isEdgeCriticDown = [];
        bool[] isEdgeCriticLeft = [];
        bool[] isEdgeCriticRight = [];
        bool[] isAxisCriticUp = [];
        bool[] isAxisCriticDown = [];
        bool[] isAxisCriticLeft = [];
        bool[] isAxisCriticRight = [];
        double edgeDistanceMax = 0; //фактическое краевое расстояние, надо считать в зависимости от координат анкеров и размеров пластины
        var edgeDistanceMin = normativeCriticEdgeDistance;
        double axisDistanceMax = 0; //фактическое межосевое расстояние между анкерами, считается в зависимотси от координат
        double[] criticCount = [];
        for (var i = 0; i < AnchorForce.GetLength(1); i++)
        {
            if (Math.Abs(baseUp - AnchorCoordinatesY
                    [0, i]) < normativeCriticEdgeDistance)
            {
                up[i] = Math.Abs(baseUp - AnchorCoordinatesY[0, i]);
                isEdgeCriticUp[i] = true;
                criticCount[i] += 1;
                if (edgeDistanceMin > up[i]) edgeDistanceMin = up[i];
            }

            if (Math.Abs(baseDown - AnchorCoordinatesY[0, i]) < normativeCriticEdgeDistance)
            {
                down[i] = Math.Abs(baseDown - AnchorCoordinatesY[0, i]);
                isEdgeCriticDown[i] = true;
                criticCount[i] += 1;
                if (edgeDistanceMin > down[i]) edgeDistanceMin = down[i];
            }

            if (Math.Abs(baseLeft - AnchorCoordinatesX[0, i]) < normativeCriticEdgeDistance)
            {
                left[i] = Math.Abs(baseLeft - AnchorCoordinatesX[0, i]);
                isEdgeCriticLeft[i] = true;
                criticCount[i] += 1;
                if (edgeDistanceMin > left[i]) edgeDistanceMin = left[i];
            }

            if (Math.Abs(baseRight - AnchorCoordinatesX[0, i]) < normativeCriticEdgeDistance)
            {
                right[i] = Math.Abs(baseRight - AnchorCoordinatesX[0, i]);
                isEdgeCriticRight[i] = true;
                criticCount[i] += 1;
                if (edgeDistanceMin > right[i]) edgeDistanceMin = right[i];
            }

            if (AnchorForce.GetLength(1) > 1)
                for (var j = 0; j < AnchorForce.GetLength(1); j++)
                    if (Math.Pow(Math.Pow(AnchorCoordinatesX[0, i] - AnchorCoordinatesX[0, j], 2) + Math.Pow(AnchorCoordinatesY[0, i] - AnchorCoordinatesY[0, j], 2), 0.5) <
                        normativeCriticInterAxialDistance)
                    {
                        if (AnchorCoordinatesX[0, i] < AnchorCoordinatesX[0, j])
                        {
                            right[i] = Math.Abs(AnchorCoordinatesX[0, i] - AnchorCoordinatesX[0, j]) / 2;
                            isAxisCriticRight[i] = true;
                            criticCount[i] += 1;
                        }

                        if (AnchorCoordinatesX[0, i] > AnchorCoordinatesX[0, j])
                        {
                            left[i] = Math.Abs(AnchorCoordinatesX[0, i] - AnchorCoordinatesX[0, j]) / 2;
                            isAxisCriticLeft[i] = true;
                            criticCount[i] += 1;
                        }

                        if (AnchorCoordinatesY[0, i] < AnchorCoordinatesY[0, j])
                        {
                            up[i] = Math.Abs(AnchorCoordinatesY[0, i] - AnchorCoordinatesY[0, j]) / 2;
                            isAxisCriticUp[i] = true;
                            criticCount[i] += 1;
                        }

                        if (AnchorCoordinatesY[0, i] > AnchorCoordinatesY[0, j])
                        {
                            down[i] = Math.Abs(AnchorCoordinatesY[0, i] - AnchorCoordinatesY[0, j]) / 2;
                            isAxisCriticDown[i] = true;
                            criticCount[i] += 1;
                        }
                    }

            if (criticCount[i] > 2)
            {
                if (isEdgeCriticUp[i] & (up[i] > edgeDistanceMax)) edgeDistanceMax = up[i];

                if (isEdgeCriticDown[i] & (down[i] > edgeDistanceMax)) edgeDistanceMax = down[i];

                if (isEdgeCriticLeft[i] & (left[i] > edgeDistanceMax)) edgeDistanceMax = left[i];

                if (isEdgeCriticRight[i] & (right[i] > edgeDistanceMax)) edgeDistanceMax = right[i];

                if (isAxisCriticUp[i] & (up[i] > axisDistanceMax)) axisDistanceMax = up[i];

                if (isAxisCriticDown[i] & (down[i] > axisDistanceMax)) axisDistanceMax = down[i];

                if (isAxisCriticLeft[i] & (left[i] > axisDistanceMax)) axisDistanceMax = left[i];

                if (isAxisCriticRight[i] & (right[i] > axisDistanceMax)) axisDistanceMax = right[i];
                if (edgeDistanceMax / 1.5 > axisDistanceMax / 3)
                    SealingDepth = edgeDistanceMax / 1.5;
                else
                    SealingDepth = axisDistanceMax / 3;
                normativeCriticInterAxialDistance = 3 * SealingDepth;
                normativeCriticEdgeDistance = 1.5 * SealingDepth;
            }

            factArea += (left[i] + right[i]) * (down[i] + up[i]);
        }

        for (var i = 0; i < AnchorForce.GetLength(1); i++)
        {
            if (Math.Abs(baseUp - AnchorCoordinatesY[0, i]) < normativeCriticEdgeDistance) up[i] = Math.Abs(baseUp - AnchorCoordinatesY[0, i]);
            if (Math.Abs(baseDown - AnchorCoordinatesY[0, i]) < normativeCriticEdgeDistance) down[i] = Math.Abs(baseDown - AnchorCoordinatesY[0, i]);
            if (Math.Abs(baseLeft - AnchorCoordinatesX[0, i]) < normativeCriticEdgeDistance) left[i] = Math.Abs(baseLeft - AnchorCoordinatesX[0, i]);
            if (Math.Abs(baseRight - AnchorCoordinatesX[0, i]) < normativeCriticEdgeDistance) right[i] = Math.Abs(baseRight - AnchorCoordinatesX[0, i]);
            if (AnchorForce.Length > 1)
                for (var j = 0; j < AnchorForce.Length; j++)
                    if (Math.Pow(Math.Pow(AnchorCoordinatesX[0, i] - AnchorCoordinatesX[0, j], 2) + Math.Pow(AnchorCoordinatesY[0, i] - AnchorCoordinatesY[0, j], 2), 0.5) <
                        normativeCriticInterAxialDistance)
                    {
                        if (AnchorCoordinatesX[0, i] < AnchorCoordinatesX[0, j]) right[i] = Math.Abs(AnchorCoordinatesX[0, i] - AnchorCoordinatesX[0, j]) / 2;

                        if (AnchorCoordinatesX[0, i] > AnchorCoordinatesX[0, j]) left[i] = Math.Abs(AnchorCoordinatesX[0, i] - AnchorCoordinatesX[0, j]) / 2;

                        if (AnchorCoordinatesY[0, i] < AnchorCoordinatesY[0, j]) up[i] = Math.Abs(AnchorCoordinatesY[0, i] - AnchorCoordinatesY[0, j]) / 2;

                        if (AnchorCoordinatesY[0, i] > AnchorCoordinatesY[0, j]) down[i] = Math.Abs(AnchorCoordinatesY[0, i] - AnchorCoordinatesY[0, j]) / 2;
                    }

            factArea += (left[i] + right[i]) * (down[i] + up[i]);
        }
        var phiSn = 0.7 + 0.3 * edgeDistanceMin / normativeCriticEdgeDistance; //коэффициент влияния установки у края основания
        if (phiSn > 1) phiSn = 1;
        Я пытался сделать автоматическое определение наличия критических краевых и осевых расстояний в соответствии с СП и подсчет их влияния на несущую способность,
        но на данный момент этот функционал не нужен.
        Формула для 3й проверки, если раскомментировать этот блок будет такой:
        var nUlt3 = excavationNormativeResistance / (gammaBt * GammaNc) * (factArea / defaultArea) * phiSn * phiRen * phiEcn;*/

    #endregion
}