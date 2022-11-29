using System;

namespace EasyCAD
{
    [Serializable]
    public class Rod
    {
        public float L { get; set; }
        public float A { get; set; }
        public float E { get; set; }
        public float O { get; set; }

        public Rod(float l, float a, float e, float o)
        {
            L = l;
            A = a;
            E = e;
            O = o;
        }

        public override string ToString()
        {
            //return $"Стержень {Index}:  L: {L}, A: {A}, E: {E}, O: {O}";
            return $"Стержень";
        }


    }
}
