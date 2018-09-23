using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PostSermonUploader
{
    public static class Utilities
    {
        public static string ValidateFileName(string fileName)
        {
            Match match = Regex.Match(fileName, @"tbc_(?<Month>...)_(?<Day>\d?\d)_(?<Year>\d\d\d\d).mp3");

            if (!match.Success)
            {
                return "File isn't in the form of tbc_mmm_dd_yyyy.mp3";
            }

            var month = match.Groups["Month"].Value.ToLower();
            if (
                !MonthMapping.Mappings.Any(
                    x =>
                    x.FullName.ToLower().Contains(month)))
            {
                return $"Specified month {month} is not recognized";
            }

            var day = match.Groups["Day"].Value;
            if (!(int.Parse(day) >= 0 && int.Parse(day) <= 31))
            {
                return $"Specified day {day} is not between 0 and 31";
            }

            var year = match.Groups["Year"].Value;
            if (!(int.Parse(year) >= 2009 && int.Parse(year) <= 2100))
            {
                return $"Specified year {year} is not between 2009 and 2100";
            }

            return string.Empty;
        }

        public static DateTime ParseFilename(string fileName)
        {
            Match match = Regex.Match(fileName, @"tbc_(?<Month>...)_(?<Day>\d?\d)_(?<Year>\d\d\d\d).mp3");

            int month =
                MonthMapping.Mappings.
                    First(x => x.FullName.ToLower().Contains(match.Groups["Month"].Value.ToLower())).
                    Number;

            var lReturn = new DateTime(int.Parse(match.Groups["Year"].Value), month,
                                       int.Parse(match.Groups["Day"].Value));

            return lReturn;
        }
    }
}