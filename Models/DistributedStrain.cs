using System;

namespace EasyCAD
{
    [Serializable]
    public struct DistributedStrain
    {
        public int SequenceNumber { get; private set; }
        public float Qx { get; private set; }
        
        public DistributedStrain(int number, float qx)
        {
            SequenceNumber = number;
            Qx = qx;
        }

        public override string ToString()
        {
            return $"Нагрузка на стержень {SequenceNumber}: {Qx}";
        }
    }
}
