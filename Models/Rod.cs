using System;

namespace EasyCAD
{
    [Serializable]
    public class Rod
    {
        public int Index { get; private set; }
        public float L { get; set; }
        public float A { get; set; }
        public float E { get; set; }
        public float O { get; set; }

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
