using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odintsov.Accounts.Web.Models
{
    public partial class Evaluation
    {
        public double? GetPercent()
        {
			// Формула подсчета результата в процентах: (X - Xmin) * 100 / (Xmax - Xmin). 
			// X - это (ReviewedResult + ManagerResult) / 2, Xmin - IndicatorsCount, Xmax - IndicatorsCount * 3
			// Умножаем на 120 т.к. по ТЗ максимальный балл - это 120%.
			return ((ReviewedResult + ManagerResult) / 2 - IndicatorsCount) * 120 / (IndicatorsCount * 2);
        }
    }
}
