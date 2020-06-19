using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GroupClasses.Library.Datas;
using GroupClasses.Library.Utils;

namespace GroupClasses.Library.GroupCore
{
    public class Group
    {
        class Class
        {
            public Data[] Datas;

            public Class(Data[] datas)
            {
                Datas = datas;
            }
        }

        public async Task<List<List<Data>>> Grouping(List<Data> datas,
            int groupCount)
        {
            await Task.Yield();

            return null;
        }

        double minWeight = double.MaxValue;
        List<Class> result = null;
        Dictionary<string, double> weightDic = new Dictionary<string, double>();
        object lockObj = new object();

        Random random = new Random();

        public Group()
        {
        }

        public async Task<Data[][]> Grouping(Data[] datas,
            int groupCount)
        {
            int count = 100000;

            Class[] classes = initClasses(datas, groupCount);

            while (count > 0)
            {
                classes = Swap(classes, datas.Length);

                CalculateWeight(classes);

                count--;
            }

            Task.WaitAll();

            //stopwatch.Stop();

            timer.Stop();

            return result;
        }

        private bool IsPass()
        {
            if (weightDic.Count == WeightConfig.Weights.Length)
            {
                foreach (var weight in WeightConfig.Weights)
                {
                    if (weightDic[weight.Name] >= weight.Limit)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private Class[] Swap(Class[] classes, int datasAllCount)
        {
            int index1 = random.Next(0, datasAllCount);
            int index2 = random.Next(0, datasAllCount);

            if (index1 == index2)
            {
                return classes;
            }

            Data data1 = null;
            Data data2 = null;
            Data temp = null;

            int data1ClassIndex = 0;
            int data2ClassIndex = 0;
            int i = 0;
            while ((data1 is null || data2 is null) && i < classes.Length)
            {
                var _class = classes[i];
                int length = _class.Datas.Length;

                if (data1 is null)
                {
                    if (index1 < length)
                    {
                        data1 = _class.Datas[index1];
                        data1ClassIndex = i;
                    }
                    else
                    {
                        index1 -= length;
                    }
                }

                if (data2 is null)
                {
                    if (index2 < length)
                    {
                        data2 = _class.Datas[index2];
                        data2ClassIndex = i;
                    }
                    else
                    {
                        index2 -= length;
                    }
                }

                i++;
            }

            temp = data1;

            classes[data1ClassIndex].Datas[index1] = data2;
            classes[data2ClassIndex].Datas[index2] = temp;

            //if (student1ClassIndex == student2ClassIndex)
            //{
            //    classes[student1ClassIndex].InitWeightValues();
            //}
            //else
            //{
            //    classes[student1ClassIndex].InitWeightValues();
            //    classes[student2ClassIndex].InitWeightValues();
            //}

            return classes;
        }

        private Class[] initClasses(Data[] datas, int classCount)
        {
            int dataCountOfClass = datas.Length / classCount;
            var result = ArrayUtil.SplitArray(datas, dataCountOfClass);

            return CreateClasses(result);
        }

        private Class[] CreateClasses(Data[][] datasList)
        {
            Class[] classes = new Class[datasList.Length];

            for(int i = 0; i < datasList.Length; i++)
            {
                classes[i] = new Class(datasList[i]);
            }

            return classes;
        }

        private void CalculateWeight(List<IClass> classes)
        {
            double sumVariance = 0;
            Dictionary<string, double> _weightDic = new Dictionary<string, double>();

            for (int index = 0; index < WeightConfig.Weights.Length; index++)
            {
                var weight = WeightConfig.Weights[index];

                double[] values = new double[classes.Count];
                //double max = double.MinValue;
                //double min = double.MaxValue;

                for (int i = 0; i < classes.Count; i++)
                {
                    var value = classes[i].WeightValues[weight.ID];

                    //if(value > max)
                    //{
                    //    max = value;
                    //}

                    //if(value < min)
                    //{
                    //    min = value;
                    //}

                    values[i] = value;
                }

                //if (weight.Type == WeightType.Score && max - min > 1)
                //{
                //    return;
                //}

                double variance = MathUtil.Variance(values);

                _weightDic.Add(weight.Name, variance);

                if (variance > weight.Limit)
                {
                    variance *= weight.Multiple;
                }

                sumVariance += variance;
            }

            lock (lockObj)
            {
                if (sumVariance < minWeight)
                {
                    weightDic = _weightDic;
                    minWeight = sumVariance;
                    result = new List<IClass>();

                    classes.ForEach(_class =>
                    {
                        IStudent[] students = new Student[_class.Students.Length];

                        _class.Students.CopyTo(students, 0);

                        result.Add(new Class(_class.ID, students));
                    });
                }
            }

            //return result;
        }
    }
}
