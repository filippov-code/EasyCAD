using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyCAD
{
    [Serializable]
    public class Construction
    {
        public bool LeftProp { get; set; }
        public bool RightProp { get; set; }
        public ObservableCollection<Rod> Rods { get; set; } = new();
        public ObservableCollection<ConcentratedStrain> ConcentratedStrains { get; set; } = new();
        public Solution? Solution { get; set; } = null;

        public int NodesCount
        {
            get { return Rods.Count + 1; }
        }

        public Construction() { }
        public Construction(List<Rod> rs, ConcentratedStrain rightStrain, bool left, bool right)
        {
            //Rods = new List<Rod>(rs);

            //rightConcentratedStrain = Rods.Count > 0? rightStrain: null;

            //LeftProp = left;
            //RightProp = right;
        }

        public bool SetConcentratedStrain(int num, float val)
        {
            throw new NotImplementedException();
            //if (num < 1 || num > Rods.Count + 1) return false;

            //if (num < Rods.Count + 1)
            //    Rods[num - 1].leftConcentratedStrain = new ConcentratedStrain(num - 1, val);
            //else
            //    rightConcentratedStrain = new ConcentratedStrain(num - 1, val);

            //return true;
        }

        public bool RemoveConcentratedStrain(int num)
        {
            throw new NotImplementedException();
            //if (num < 1 || num > Rods.Count + 1) return false;

            //if (num < Rods.Count + 1)
            //    Rods[num - 1].leftConcentratedStrain = null;
            //else
            //    rightConcentratedStrain = null;

            //return true;
        }

        public ConcentratedStrain GetConcentratedStrainByNumber(int num)
        {
            throw new NotImplementedException();
            //if (num < 1 || num > Rods.Count + 1) return null;

            //if (num < Rods.Count + 1)
            //    return Rods[num - 1].leftConcentratedStrain;
            //else
            //    return rightConcentratedStrain;
        }

        public float GetLength()
        {
            float sum = 0;
            foreach (var rod in Rods) sum += rod.L;
            return sum;
        }

        public float GetNodeLengthByNumber(int num)
        {
            if (num < 1) return -1;
            if (num > Rods.Count + 1) return -1;
            float length = 0;
            for (int i = 0; i < num - 1; i++)
            {
                length += Rods[i].L;
            }
            return length;
        }

        public Rod GetRodByLength(float L)
        {
            if (L == 0) return Rods[0];
            if (L > GetLength()) return Rods.Last();
            float l = 0;
            for (int i = 0; i <= Rods.Count; i++)
            {
                if (L <= l) return Rods[i - 1];
                l += Rods[i].L;
            }
            return null;
        }

        public float GetLengthBeforeRod(float L)
        {
            if (L == 0) return L;
            if (L >= GetLength()) L = GetLength();
            float l = 0;
            for (int i = 0; i <= Rods.Count; i++)
            {
                if (L <= l) return l - Rods[i - 1].L;
                l += Rods[i].L;
            }
            return 0;
        }
    }
}
