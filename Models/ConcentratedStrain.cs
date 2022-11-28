using System;

namespace EasyCAD
{
    [Serializable]
    public struct ConcentratedStrain
    {
        public int SequenceNumber { get; set; }
        public float Force { get; set; }


        public ConcentratedStrain(int i, float f)
        {
            SequenceNumber = i;
            Force = f;
        }

        public override string ToString()
        {
            return $"Нагрузка в узле {SequenceNumber}: {Force}";
        }
    }
}
