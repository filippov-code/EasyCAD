using System;

namespace EasyCAD
{
    [Serializable]
    public class ConcentratedStrain
    {
        public readonly int index;
        public readonly float force;


        public ConcentratedStrain(int i, float f)
        {
            index = i;
            force = f;
        }

        public override string ToString()
        {
            return $"Нагрузка в узле {index + 1}: {force}";
        }
    }
}
