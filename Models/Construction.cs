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
        public ObservableCollection<Rod> Rods { get; } = new();
        public ObservableCollection<DistributedStrain> DistributedStrains { get; } = new();
        public ObservableCollection<ConcentratedStrain> ConcentratedStrains { get; } = new();
        //public Solution? Solution { get; set; } = null;
        public Action ConstructionChanged;

        public int NodesCount
        {
            get { return Rods.Count + 1; }
        }

        public Construction() 
        {
            ConstructionChanged += RemoveUnnecessaryConcentratedStrains;
            ConstructionChanged += RemoveUnnecessaryDistributedStrains;
        }

        public void AddRod(Rod rod)
        {
            Rods.Add(rod);
            ConstructionChanged?.Invoke();
        }

        public void RemoveRod(Rod rod)
        {
            Rods.Remove(rod);
            ConstructionChanged?.Invoke();
        }

        public void SetDistributedStrain(int number, float qx)
        {
            if (number < 1) return;
            if (number > Rods.Count) return;
            DistributedStrain? existingStrain = DistributedStrains.FirstOrDefault(x => x.SequenceNumber == number);
            if (existingStrain != null)
            {
                DistributedStrains.Remove(existingStrain.Value);
            }
            DistributedStrains.Add(new DistributedStrain(null, qx, number));
        }

        public void RemoveDistributedStrain(DistributedStrain strain)
        {
            DistributedStrains.Remove(strain);
        }

        private void RemoveUnnecessaryDistributedStrains()
        {
            if (Rods.Count == 0)
            {
                DistributedStrains.Clear();
                return;
            }

            List<DistributedStrain> strainsToRemove = new();
            foreach (var strain in DistributedStrains)
            {
                if (strain.SequenceNumber > Rods.Count)
                {
                    strainsToRemove.Add(strain);
                }
            }

            foreach (var strain in strainsToRemove)
            {
                DistributedStrains.Remove(strain);
            }
        }

        public void SetConcentratedStrain(int number, float F)
        {
            if (number == 0) return;
            if (number > Rods.Count + 1) return;
            ConcentratedStrain? existingStrain = ConcentratedStrains.FirstOrDefault(x => x.SequenceNumber == number);
            if (existingStrain != null)
            {
                ConcentratedStrains.Remove(existingStrain.Value);
            }
            ConcentratedStrains.Add(new ConcentratedStrain(number, F));
        }

        public void RemoveConcentratedStrain(ConcentratedStrain strain)
        {
            ConcentratedStrains.Remove(strain);
            //ConstructionChanged?.Invoke();
        }

        private void RemoveUnnecessaryConcentratedStrains()
        {
            if (Rods.Count == 0)
            {
                ConcentratedStrains.Clear();
                return;
            }

            List<ConcentratedStrain> strainsToRemove = new();
            foreach (var strain in ConcentratedStrains)
            {
                if (strain.SequenceNumber > Rods.Count + 1)
                {
                    strainsToRemove.Add(strain);
                }
            }

            foreach (var strain in strainsToRemove)
            {
                ConcentratedStrains.Remove(strain);
            }
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

        public float GetLengthUpToNode(int nodeNumber)
        {
            if (nodeNumber == 1) return 0;
            if (nodeNumber == Rods.Count + 1) return GetLength();
            float length = 0;
            for (int i = 1; i < nodeNumber; i++)
            {
                length += Rods[i - 1].L;
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
