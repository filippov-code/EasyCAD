using System;

namespace EasyCAD
{
    [Serializable]
    public class Rod
    {
        public int Index { get; private set; }
        public readonly float L;
        public readonly float A;
        public readonly float E;
        public readonly float O;
        public DistributedStrain distributedStrain;
        public ConcentratedStrain leftConcentratedStrain;

        public Rod(int i, float l, float a, float e, float o)
        {
            Index = i;
            L = l;
            A = a;
            E = e;
            O = o;
        }

        public void ChangeIndex(int i)
        {
            Index = i;
        }

        public override string ToString()
        {
            //return $"Стержень {Index}:  L: {L}, A: {A}, E: {E}, O: {O}";
            return $"Стержень {Index}";
        }


    }
}
