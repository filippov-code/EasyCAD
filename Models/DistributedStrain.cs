using System;

namespace EasyCAD
{
    [Serializable]
    public class DistributedStrain
    {
        public readonly Rod rod;
        public readonly float x;
        
        public DistributedStrain(Rod myRod, float xDist)
        {
            rod = myRod;
            x = xDist;
            rod.distributedStrain = this;
        }

        public override string ToString()
        {
            return $"Нагрузка на стержень {rod.Index}: {x}";
        }
    }
}
