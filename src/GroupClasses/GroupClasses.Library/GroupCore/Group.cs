﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupClasses.Library.Datas;
using GroupClasses.Library.Filters;
using GroupClasses.Library.Service;
using GroupClasses.Library.Utils;

namespace GroupClasses.Library.GroupCore
{
    public class Group
    {
        class Class
        {
            public Data[] Datas;

            public decimal[] Weight;

            public Class(Data[] datas)
            {
                Datas = datas;
                Weight = new decimal[datas.Length];
            }

            public void InitWeightValues()
            {
                foreach(var filter in FilterService.Filters)
                {
                    decimal weight = 0;
                    var type = filter.Type;
                    var dataValueId = filter.DataValue.Id;
                    var dataValueType = filter.DataValue.Type;

                    switch(type)
                    {
                        case FilterType.Average:
                            switch (dataValueType)
                            {
                                case DataValueType.Number:
                                    weight = Datas.Select(data => 
                                    Convert.ToDecimal(data.Values[dataValueId]))
                                        .Average();
                                    break;
                            }
                            break;
                        case FilterType.Ratio:
                            switch (dataValueType)
                            {
                                case DataValueType.String:
                                    weight = Datas.Select(data => 
                                    Convert.ToString(data.Values[dataValueId]))
                                        .Count()
                                        / Datas.Length;
                                    break;
                            }
                            break;
                    }

                    Weight[dataValueId] = weight;
                }
            }
        }

        class GroupResult
        {
            public Class[] Classes
            {
                get;
                private set;
            }

            public Dictionary<int, decimal> VarianceResults
            {
                get;
                private set;
            }

            public decimal SumVariance
            {
                get;
                private set;
            }

            public GroupResult(Class[] classes, Dictionary<int, decimal> varianceResults, decimal sumVariance)
            {
                Classes = classes;
                VarianceResults = varianceResults;
                SumVariance = sumVariance;
            }
        }

        public async Task<List<List<Data>>> Grouping(List<Data> datas,
            int groupCount)
        {
            await Task.Yield();

            return null;
        }

        Random random = new Random();

        public Group()
        {
        }

        public async Task<Data[][]> Grouping(Data[] datas,
            int groupCount)
        {
            await Task.Yield();

            int count = 100000;
            decimal minWeight = decimal.MaxValue;
            Dictionary<int, decimal> weightDic = new Dictionary<int, decimal>();

            Class[] classes = initClasses(datas, groupCount);
            Class[] result = new Class[classes.Length];

            while (count > 0)
            {
                classes = Swap(classes, datas.Length);

                var groupResult =  CalculateWeight(classes);

                if (groupResult.SumVariance < minWeight && IsPass(groupResult))
                {
                    minWeight = groupResult.SumVariance;
                    Array.Copy(classes, result, classes.Length);
                }

                count--;
            }

            return result.Select(@class => @class.Datas).ToArray();
        }

        private bool IsPass(GroupResult groupResult)
        {
            foreach (var filter in FilterService.Filters)
            {
                if (groupResult.VarianceResults[filter.DataValue.Id] >= filter.VarianceLimit)
                {
                    return false;
                }
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

            Data temp = data1;
            classes[data1ClassIndex].Datas[index1] = data2;
            classes[data2ClassIndex].Datas[index2] = temp;

            if (data1ClassIndex == data2ClassIndex)
            {
                classes[data1ClassIndex].InitWeightValues();
            }
            else
            {
                classes[data1ClassIndex].InitWeightValues();
                classes[data2ClassIndex].InitWeightValues();
            }

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

        private GroupResult CalculateWeight(Class[] classes)
        {
            decimal sumVariance = 0;
            Dictionary<int, decimal> _weightDic = new Dictionary<int, decimal>();

            for (int index = 0; index < FilterService.Filters.Count; index++)
            {
                var filter = FilterService.Filters[index];

                decimal[] values = new decimal[classes.Length];

                for (int i = 0; i < classes.Length; i++)
                {
                    var value = classes[i].Weight[filter.DataValue.Id];

                    values[i] = value;
                }

                decimal variance = MathUtil.Variance(values) * filter.Weighting;

                _weightDic.Add(filter.DataValue.Id, variance);

                sumVariance += variance;
            }

            return new GroupResult(classes, _weightDic, sumVariance);
        }
    }
}
