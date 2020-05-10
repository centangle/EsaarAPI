using Catalogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityProvider.Helpers
{
    public static class TimePeriodHelper
    {
        //var today = baseDate;
        //var yesterday = baseDate.AddDays(-1);
        //var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
        //var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
        //var lastWeekStart = thisWeekStart.AddDays(-7);
        //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
        //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
        //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
        //var lastMonthStart = thisMonthStart.AddMonths(-1);
        //var lastMonthEnd = thisMonthStart.AddSeconds(-1);
        public static DateTime migrationDate = new DateTime(2020, 3, 1);
        public static DateTime GetStartDate(TimePeriodCatalog? timePeriod, DateTime? startDate)
        {
            var baseDate = DateTime.Now;

            if (timePeriod == null)
            {
                startDate = migrationDate;
            }
            else if (timePeriod.Value == TimePeriodCatalog.Today)
            {
                startDate = baseDate.Date;
            }
            else if (timePeriod.Value == TimePeriodCatalog.Week)
            {
                startDate = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            }
            else if (timePeriod.Value == TimePeriodCatalog.Month)
            {
                startDate = baseDate.AddDays(1 - baseDate.Day);
            }
            else if (timePeriod.Value == TimePeriodCatalog.Custom)
            {
                if (startDate == null)
                    startDate = migrationDate;
            }
            return startDate.Value.ToUniversalTime();
        }
        public static DateTime GetEndDate(TimePeriodCatalog? timePeriod, DateTime? endDate)
        {
            var baseDate = DateTime.Now;
            if (timePeriod == null)
            {
                endDate = migrationDate;
            }
            else if (timePeriod.Value == TimePeriodCatalog.Today)
            {
                endDate = baseDate.Date.AddDays(1).AddTicks(-1);
            }
            else if (timePeriod.Value == TimePeriodCatalog.Week)
            {
                var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                endDate = thisWeekStart.AddDays(7).AddSeconds(-1);
            }
            else if (timePeriod.Value == TimePeriodCatalog.Month)
            {
                var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                endDate = thisMonthStart.AddMonths(1).AddSeconds(-1);
            }
            else if (timePeriod.Value == TimePeriodCatalog.Custom)
            {
                if (endDate == null)
                    endDate = DateTime.Now;
            }
            return endDate.Value.ToUniversalTime();
        }
    }
}
