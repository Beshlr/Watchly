using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clsBusinessLayer
{
    public static class clsSettings
    {
        public static string GetTimeDifference(DateTime startDate, DateTime endDate)
        {
            var timeSpan = endDate - startDate;

            if (timeSpan.TotalSeconds < 60)
            {
                return $"{Math.Floor(timeSpan.TotalSeconds)} seconds";
            }
            else if (timeSpan.TotalMinutes < 60)
            {
                return $"{Math.Floor(timeSpan.TotalMinutes)} minutes";
            }
            else if (timeSpan.TotalHours < 24)
            {
                return $"{Math.Floor(timeSpan.TotalHours)} hours";
            }
            else
            {
                return $"{Math.Floor(timeSpan.TotalDays)} days";
            }
        }

    }
}
