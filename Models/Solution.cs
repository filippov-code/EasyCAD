using System;
using System.Linq;
using System.Text;

namespace EasyCAD
{
    [Serializable]
    public class Solution
    {
        public readonly Construction Construction;
        public Matrix AMatrix { get; private set; }
        public Matrix BMatrix { get; private set; }
        public Matrix ABMatrix { get; private set; }
        public Matrix DeltaMatrix { get; private set; }
        public Matrix NxMatrix { get; private set; }
        public Matrix OxMatrix { get; private set; }
        public Matrix UxMatrix { get; private set; }
        public int Accuracy { get; set; } = 3;

        public Solution(Construction con)
        {
            Construction = con;
        }

        public void Calculate()
        {
            if (Construction == null) return;

            int nodesCount = Construction.Rods.Count + 1;

            AMatrix = new Matrix(nodesCount);

            for (int i = 0; i < nodesCount - 1; i++)
            {
                double value = Construction.Rods[i].E * Construction.Rods[i].A / Construction.Rods[i].L;
                AMatrix[i, i] += value;
                AMatrix[i, i + 1] += -value;
                AMatrix[i + 1, i] += -value;
                AMatrix[i + 1, i + 1] += value;
            }

            BMatrix = new Matrix(nodesCount, 1);

            for (int i = 0; i < nodesCount; i++)
            {
                if (i > 0)
                {
                    DistributedStrain? distributedStrain = Construction.DistributedStrains.FirstOrDefault(x => x.SequenceNumber == i);
                    if (distributedStrain != null)
                    {
                        float K = distributedStrain.Value.Qx * Construction.Rods[i - 1].L / 2;
                        BMatrix[i, 0] += K;
                    }
                }

                if (i < nodesCount - 1)
                {
                    DistributedStrain? distributedStrain = Construction.DistributedStrains.FirstOrDefault(x => x.SequenceNumber == i + 1);
                    if (distributedStrain != null)
                    {
                        double K = distributedStrain.Value.Qx * Construction.Rods[i].L / 2;
                        BMatrix[i, 0] += K;
                    }
                }

                ConcentratedStrain? concentratedStrain = Construction.ConcentratedStrains.FirstOrDefault(x => x.SequenceNumber == i + 1);
                if (concentratedStrain != null)
                {
                    float K = concentratedStrain.Value.Force;
                    BMatrix[i, 0] += K;
                }
            }

            if (Construction.LeftProp)
            {
                for (int i = 0; i < nodesCount; i++)
                {
                    if (i == 0) AMatrix[0, i] = 1f;
                    else AMatrix[0, i] = 0f;
                }
                BMatrix[0, 0] = 0;
            }
            if (Construction.RightProp)
            {
                for (int i = 0; i < nodesCount; i++)
                {
                    if (i == nodesCount - 1) AMatrix[nodesCount - 1, i] = 1f;
                    else AMatrix[nodesCount - 1, i] = 0f;
                }
                BMatrix[nodesCount - 1, 0] = 0;
            }

            ABMatrix = Matrix.GetExtendedMatrix(AMatrix, BMatrix);

            DeltaMatrix = Matrix.SolveSystemByGaussian(AMatrix, BMatrix);

            NxMatrix = new Matrix(Construction.Rods.Count, 2);
            for (int i = 0; i < Construction.Rods.Count; i++)
            {
                Rod rod = Construction.Rods[i];
                NxMatrix[i, 0] = GetNxSolution(rod, 0);
                NxMatrix[i, 1] = GetNxSolution(rod, rod.L);
            }

            OxMatrix = new Matrix(Construction.Rods.Count, 2);
            for (int i = 0; i < Construction.Rods.Count; i++)
            {
                OxMatrix[i, 0] = NxMatrix[i, 0] / Construction.Rods[i].A;
                OxMatrix[i, 1] = NxMatrix[i, 1] / Construction.Rods[i].A;
            }

            UxMatrix = new Matrix(Construction.Rods.Count, 2);
            for (int i = 0; i < Construction.Rods.Count; i++)
            {
                Rod rod = Construction.Rods[i];
                UxMatrix[i, 0] = GetUxSolution(rod, 0);
                UxMatrix[i, 1] = GetUxSolution(rod, rod.L);
            }
        }

        public System.Drawing.PointF GetDeltasByRod(Rod rod)
        {
            int i = Construction.Rods.IndexOf(rod);
            return new System.Drawing.PointF((float)Math.Round(DeltaMatrix[i, 0], Accuracy), (float)Math.Round(DeltaMatrix[i + 1, 0], Accuracy));
        }

        public double GetNxSolution(Rod rod, double lengthX)
        {
            System.Drawing.PointF delta01 = GetDeltasByRod(rod);
            DistributedStrain? strain = Construction.GetDistributedStrainByRod(rod);
            double qx = strain == null ? 0 : strain.Value.Qx;
            double first = (rod.E * rod.A / rod.L) * (delta01.Y - delta01.X);
            double second = (qx * rod.L / 2) * (1 - (2 * lengthX) / rod.L);
            return Math.Round(first + second, Accuracy);
        }

        public double GetUxSolution(Rod rod, double lengthX)
        {
            System.Drawing.PointF delta01 = GetDeltasByRod(rod);
            DistributedStrain? strain = Construction.GetDistributedStrainByRod(rod);
            double qx = strain == null ? 0 : strain.Value.Qx;
            double first = (lengthX / rod.L) * (delta01.Y - delta01.X);
            double second = ((qx * rod.L * rod.L * lengthX) / (2 * rod.E * rod.A * rod.L)) * (1 - lengthX / rod.L);
            return Math.Round(delta01.X + first + second, Accuracy);
        }

        public double[,] GetValuesTable(int accuracyStep)
        {
            float length = Construction.Length;
            //float progress = 0;
            double progressStep = (float)Math.Pow(0.1d, accuracyStep);
            int steps = (int)(length / progressStep) + 1;
            double[,] table = new double[steps, 5];

            int i = 0;
            for (double progress = 0; progress < length + progressStep; progress += progressStep, i++)
            {
                progress = (float)Math.Round(progress, accuracyStep);

                Rod rod = Construction.GetRodByLength(progress);
                double lOnRod = progress - Construction.GetLengthBeforeRod(progress);

                double nx = GetNxSolution(rod, lOnRod);
                table[i, 0] = progress;
                table[i, 1] = nx;
                table[i, 2] = nx / rod.A;
                table[i, 3] = rod.O;
                table[i, 4] = GetUxSolution(rod, lOnRod);
            }

            return table;
        }
    }
}
