using System;

namespace EasyCAD
{
    [Serializable]
    public class Solution
    {
        public readonly Construction construction;
        public Matrix Amatrix;
        public Matrix Bmatrix;
        public Matrix extendedMatrix;
        public Matrix deltaMatrix;
        public Matrix Nsolutions;
        public Matrix osolutions;
        public Matrix Usolutions;

        public Solution(Construction con)
        {
            construction = con;
        }

        public void Calculate()
        {
            if (construction == null) return;

            int nodesCount = construction.Rods.Count + 1;

            Amatrix = new Matrix(nodesCount);

            for (int i = 0; i < nodesCount - 1; i++)
            {
                float value = construction.Rods[i].E * construction.Rods[i].A / construction.Rods[i].L;
                Amatrix[i, i] += value;
                Amatrix[i, i + 1] += -value;
                Amatrix[i + 1, i] += -value;
                Amatrix[i + 1, i + 1] += value;
            }

            Bmatrix = new Matrix(nodesCount, 1);

            for (int i = 0; i < nodesCount; i++)
            {
                if (i > 0)
                {
                    DistributedStrain distForce = construction.Rods[i - 1].distributedStrain;
                    if (distForce != null)
                    {
                        float K = distForce.x * construction.Rods[i - 1].L / 2;
                        Bmatrix[i, 0] += K;
                    }
                }

                if (i < nodesCount - 1)
                {
                    DistributedStrain distForce = construction.Rods[i].distributedStrain;
                    if (distForce != null)
                    {
                        float K = distForce.x * construction.Rods[i].L / 2;
                        Bmatrix[i, 0] += K;
                    }
                }

                ConcentratedStrain concForce = construction.GetConcentratedStrainByNumber(i + 1);
                if (concForce != null)
                {
                    float K = concForce.force;
                    Bmatrix[i, 0] += K;
                }
            }

            if (construction.LeftProp)
            {
                for (int i = 0; i < nodesCount; i++)
                {
                    if (i == 0) Amatrix[0, i] = 1f;
                    else Amatrix[0, i] = 0f;
                }
                Bmatrix[0, 0] = 0;
            }
            if (construction.RightProp)
            {
                for (int i = 0; i < nodesCount; i++)
                {
                    if (i == nodesCount - 1) Amatrix[nodesCount - 1, i] = 1f;
                    else Amatrix[nodesCount - 1, i] = 0f;
                }
                Bmatrix[nodesCount - 1, 0] = 0;
            }

            extendedMatrix = Matrix.GetExtendedMatrix(Amatrix, Bmatrix);

            deltaMatrix = Matrix.SolveSystemByGaussian(Amatrix, Bmatrix);

            Nsolutions = new Matrix(construction.Rods.Count, 3);
            for (int i = 0; i < construction.Rods.Count; i++)
            {
                Nsolutions[i, 0] = i + 1;
                Rod rod = construction.Rods[i];
                Nsolutions[i, 1] = GetNxSolution(rod, 0);
                Nsolutions[i, 2] = GetNxSolution(rod, rod.L);
            }

            osolutions = new Matrix(construction.Rods.Count, 3);
            for (int i = 0; i < construction.Rods.Count; i++)
            {
                osolutions[i, 0] = i + 1;
                osolutions[i, 1] = Nsolutions[i, 1] / construction.Rods[i].A;
                osolutions[i, 2] = Nsolutions[i, 2] / construction.Rods[i].A;
            }

            Usolutions = new Matrix(construction.Rods.Count, 3);
            for (int i = 0; i < construction.Rods.Count; i++)
            {
                Usolutions[i, 0] = i + 1;
                Rod rod = construction.Rods[i];
                Usolutions[i, 1] = GetUxSolution(rod, 0);
                Usolutions[i, 2] = GetUxSolution(rod, rod.L);
            }
        }

        public System.Drawing.PointF GetDeltasByRod(Rod rod)
        {
            int i = construction.Rods.IndexOf(rod);
            return new System.Drawing.PointF(deltaMatrix[i, 0], deltaMatrix[i + 1, 0]);
        }

        public float GetNxSolution(Rod rod, float lengthX)
        {
            System.Drawing.PointF delta01 = GetDeltasByRod(rod);
            float qx = rod.distributedStrain == null ? 0 : rod.distributedStrain.x;
            float first = (rod.E * rod.A / rod.L) * (delta01.Y - delta01.X);
            float second = (qx * rod.L / 2) * (1 - (2 * lengthX) / rod.L);
            return first + second;
        }

        public float GetUxSolution(Rod rod, float lengthX)
        {
            System.Drawing.PointF delta01 = GetDeltasByRod(rod);
            float qx = rod.distributedStrain == null ? 0 : rod.distributedStrain.x;
            float first = (lengthX / rod.L) * (delta01.Y - delta01.X);
            float second = ((qx * rod.L * rod.L * lengthX) / (2 * rod.E * rod.A * rod.L)) * (1 - lengthX / rod.L);
            return delta01.X + first + second;
        }

        public float[,] GetValuesTable(int accuracy)
        {
            float length = construction.GetLength();
            //float progress = 0;
            float progressStep = (float)Math.Pow(0.1d, accuracy);
            int steps = (int)(length / progressStep) + 1;
            float[,] table = new float[steps, 5];

            int i = 0;
            for (float progress = 0; progress < length + progressStep; progress += progressStep, i++)
            {
                progress = (float)Math.Round(progress, accuracy);

                Rod rod = construction.GetRodByLength(progress);
                float lOnRod = progress - construction.GetLengthBeforeRod(progress);

                float nx = GetNxSolution(rod, lOnRod);
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
