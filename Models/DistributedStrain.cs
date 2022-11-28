using System;

namespace EasyCAD
{
    [Serializable]
    public struct DistributedStrain
    {
        public readonly Rod rod;
        public readonly float qx;
        public int SequenceNumber { get; set; }
        
        public DistributedStrain(Rod myRod, float xDist, int number)
        {
            SequenceNumber = number;
            rod = myRod;
            qx = xDist;
            //rod.distributedStrain = this;
        }

        public override string ToString()
        {
            return $"Нагрузка на стержень {SequenceNumber}: {qx}";
        }
    }
}
