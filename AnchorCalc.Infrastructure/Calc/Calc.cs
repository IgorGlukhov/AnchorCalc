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
            Matrix matrixEquation = new double[3, 3];
            IterateEquation(deformationModule, areaOfSegment, anchorSectionArea, elasticModule, baseCoordinatesX, baseCoordinatesY,
                primaryBaseMatrix, anchorCoordinatesX, anchorCoordinatesY, primaryAnchorMatrix, matrixEquation);

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
                IterateEquation(deformationModule, areaOfSegment, anchorSectionArea, elasticModule, baseCoordinatesX, baseCoordinatesY,
                 primaryBaseMatrix, anchorCoordinatesX, anchorCoordinatesY, primaryAnchorMatrix, matrixEquation);

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

    private static void IterateEquation(double deformationModule, double areaOfSegment, double anchorSectionArea, double elasticModule, Matrix baseCoordinatesX, Matrix baseCoordinatesY, Matrix primaryBaseMatrix, Matrix anchorCoordinatesX, Matrix anchorCoordinatesY, Matrix primaryAnchorMatrix, Matrix matrixEquation)
    {
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
    }

    public void AnchorValidate()
    {
        var maxAnchorForce = ((Matrix)AnchorForce).Max();
        var resultantAnchorForce = ((Matrix)AnchorForce).Sum().Sum();
        double forcedAnchorsCount = 0;
        double gravityCenterCoordinatesX = 0;
        double gravityCenterCoordinatesY = 0;
        double resultantAnchorForceCoordinatesX = 0;
        double resultantAnchorForceCoordinatesY = 0;
        for (var i = 0; i < AnchorForce.GetLength(1); i++)
        {
            if (AnchorForce[0, i] > 0)
            {
                gravityCenterCoordinatesX += AnchorCoordinatesX[0, i]*1000;
                gravityCenterCoordinatesY += AnchorCoordinatesX[0, i]*1000;
                forcedAnchorsCount++;
            }
            resultantAnchorForceCoordinatesX += AnchorForce[0, i] * AnchorCoordinatesX[0, i]*1000 / resultantAnchorForce;
            resultantAnchorForceCoordinatesY += AnchorForce[0, i] * AnchorCoordinatesY[0, i]*1000 / resultantAnchorForce;
        }
        gravityCenterCoordinatesX /= forcedAnchorsCount;
        gravityCenterCoordinatesY /= forcedAnchorsCount;
        double eccentricityX = Math.Abs(resultantAnchorForceCoordinatesX - gravityCenterCoordinatesX);
        double eccentricityY = Math.Abs(resultantAnchorForceCoordinatesY - gravityCenterCoordinatesY);

        AnchorValidateSteel = maxAnchorForce / (NormativeResistance / GammaNs) * 100;


        var gammaBt = 1.5; //Коэффициент надежности по бетону при растяжении по СП 63.13330.2018
        var forceUltContact = IsCracked
            ? CrackedNormativeForce * PhiC / (GammaNp * gammaBt)
            : UncrackedNormativeForce * PhiC / (GammaNp * gammaBt);
        AnchorValidateContact = maxAnchorForce / forceUltContact * 100;


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
            phiEcn = 1 / ((1 + 2 * eccentricityX / normativeCriticInterAxialDistance) *
        (1 + 2 * eccentricityY / normativeCriticInterAxialDistance));
        if (phiEcn > 1) phiEcn = 1;
        FactExcavtionAreaCalculation(out double factArea, out double defaultArea, out double phiSn);
        var forceUltExcavation = excavationNormativeResistance / (gammaBt * GammaNc) * (factArea / defaultArea) * phiSn * phiRen * phiEcn;
        AnchorValidateExcavation = resultantAnchorForce / forceUltExcavation * 100;

        if (AnchorForce.Length > 1)
            phiEcn = 1 / ((1 + 2 * eccentricityX / CriticInterAxialDistance) *
        (1 + 2 * eccentricityY / CriticInterAxialDistance));
        //коэффициент влияния неравномерного загружения анкерной группы
        if (phiEcn > 1) phiEcn = 1;
        var splittingNormativeResistance = excavationNormativeResistance / (gammaBt * GammaNc) * (factArea / defaultArea) * phiSn * phiRen * phiEcn;
        var phiHsp = Math.Pow(FactBaseHeight / MinBaseHeight, 0.666);
        if (phiHsp > Math.Pow(2 * SealingDepth / MinBaseHeight, 0.666))
            phiHsp = Math.Pow(2 * SealingDepth / MinBaseHeight, 0.666);
        var forceUltSplitting = splittingNormativeResistance * phiHsp / GammaNsp;
        AnchorValidateSplitting = resultantAnchorForce / forceUltSplitting * 100;
    }

    private void FactExcavtionAreaCalculation(out double factArea, out double defaultArea, out double phiSn)
    {
        var normativeCriticInterAxialDistance = 3 * SealingDepth; //Расчетное критическое межосевое расстояние между анкерами, мм
        var normativeCriticEdgeDistance = 1.5 * SealingDepth; //Расчетное критическое краевое расстояние, мм
        factArea = 0;
        defaultArea = normativeCriticInterAxialDistance * normativeCriticInterAxialDistance;
        double[,] anchorCoordinatesX = (Matrix)AnchorCoordinatesX * 1000;
        double[,] anchorCoordinatesY = (Matrix)AnchorCoordinatesY * 1000;
        int anchorCount = AnchorCoordinatesX.GetLength(1);
        double[] up = new double[anchorCount];
        double[] down = new double[anchorCount];
        double[] left = new double[anchorCount];
        double[] right = new double[anchorCount];
        for (var i = 0; i < anchorCount; i++)
        {
            up[i] = normativeCriticInterAxialDistance / 2;
            down[i] = normativeCriticInterAxialDistance / 2;
            left[i] = normativeCriticInterAxialDistance / 2;
            right[i] = normativeCriticInterAxialDistance / 2;
        }
        double baseUp = ConcreteBaseLength / 2;
        double baseDown = -ConcreteBaseLength / 2;
        double baseLeft = -ConcreteBaseWidth / 2;
        double baseRight = ConcreteBaseWidth / 2;
        double edgeDistanceMin = normativeCriticEdgeDistance;
        for (var i = 0; i < anchorCount; i++)
        {
            if (Math.Abs(baseUp - anchorCoordinatesY
                    [0, i]) < normativeCriticEdgeDistance)
            {
                up[i] = Math.Abs(baseUp - anchorCoordinatesY[0, i]);
                if (edgeDistanceMin > up[i]) edgeDistanceMin = up[i];
            }

            if (Math.Abs(baseDown - anchorCoordinatesY[0, i]) < normativeCriticEdgeDistance)
            {
                down[i] = Math.Abs(baseDown - anchorCoordinatesY[0, i]);
                if (edgeDistanceMin > down[i]) edgeDistanceMin = down[i];
            }

            if (Math.Abs(baseLeft - anchorCoordinatesX[0, i]) < normativeCriticEdgeDistance)
            {
                left[i] = Math.Abs(baseLeft - anchorCoordinatesX[0, i]);
                if (edgeDistanceMin > left[i]) edgeDistanceMin = left[i];
            }

            if (Math.Abs(baseRight - anchorCoordinatesX[0, i]) < normativeCriticEdgeDistance)
            {
                right[i] = Math.Abs(baseRight - anchorCoordinatesX[0, i]);
                if (edgeDistanceMin > right[i]) edgeDistanceMin = right[i];
            }

            for (var j = 0; j < anchorCount; j++)
            {
                if (Math.Pow(Math.Pow(anchorCoordinatesX[0, i] - anchorCoordinatesX[0, j], 2) + Math.Pow(anchorCoordinatesY[0, i] - anchorCoordinatesY[0, j], 2), 0.5) <
                    normativeCriticInterAxialDistance)
                {
                    if (anchorCoordinatesX[0, i] < anchorCoordinatesX[0, j]) right[i] = Math.Abs(anchorCoordinatesX[0, i] - anchorCoordinatesX[0, j]) / 2;
                    if (anchorCoordinatesX[0, i] > anchorCoordinatesX[0, j]) left[i] = Math.Abs(anchorCoordinatesX[0, i] - anchorCoordinatesX[0, j]) / 2;
                    if (anchorCoordinatesY[0, i] < anchorCoordinatesY[0, j]) up[i] = Math.Abs(anchorCoordinatesY[0, i] - anchorCoordinatesY[0, j]) / 2;
                    if (anchorCoordinatesY[0, i] > anchorCoordinatesY[0, j]) down[i] = Math.Abs(anchorCoordinatesY[0, i] - anchorCoordinatesY[0, j]) / 2;
                }
            }
            factArea += (left[i] + right[i]) * (down[i] + up[i]);
        }
        phiSn = 0.7 + 0.3 * edgeDistanceMin / normativeCriticEdgeDistance;
        if (phiSn > 1) phiSn = 1;
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
}