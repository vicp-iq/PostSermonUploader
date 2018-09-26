using System.Collections.Generic;

namespace PostSermonUploader.Helpers
{
    public class MonthMapping
    {
        public static List<MonthMapping> Mappings = new List<MonthMapping>
                                                         {
                                                             new MonthMapping
                                                                 {
                                                                     Number = 1,
                                                                     LocalName = "01 - January",
                                                                     ServerName = "01_jan",
                                                                     FullName = "January",
                                                                     Shorthand = "jan"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 2,
                                                                     LocalName = "02 - February",
                                                                     ServerName = "02_feb",
                                                                     FullName = "February",
                                                                     Shorthand = "feb"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 3,
                                                                     LocalName = "03 - March",
                                                                     ServerName = "03_mar",
                                                                     FullName = "March",
                                                                     Shorthand = "mar"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 4,
                                                                     LocalName = "04 - April",
                                                                     ServerName = "04_apr",
                                                                     FullName = "April",
                                                                     Shorthand = "apr"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 5,
                                                                     LocalName = "05 - May",
                                                                     ServerName = "05_may",
                                                                     FullName = "May",
                                                                     Shorthand = "may"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 6,
                                                                     LocalName = "06 - June",
                                                                     ServerName = "06_jun",
                                                                     FullName = "June",
                                                                     Shorthand = "jun"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 7,
                                                                     LocalName = "07 - July",
                                                                     ServerName = "07_jul",
                                                                     FullName = "July",
                                                                     Shorthand = "jul"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 8,
                                                                     LocalName = "08 - August",
                                                                     ServerName = "08_aug",
                                                                     FullName = "August",
                                                                     Shorthand = "aug"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 9,
                                                                     LocalName = "09 - September",
                                                                     ServerName = "09_sep",
                                                                     FullName = "September",
                                                                     Shorthand = "sep"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 10,
                                                                     LocalName = "10 - October",
                                                                     ServerName = "10_oct",
                                                                     FullName = "October",
                                                                     Shorthand = "oct"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 11,
                                                                     LocalName = "11 - November",
                                                                     ServerName = "11_nov",
                                                                     FullName = "November",
                                                                     Shorthand = "nov"
                                                                 },
                                                             new MonthMapping
                                                                 {
                                                                     Number = 12,
                                                                     LocalName = "12 - December",
                                                                     ServerName = "12_dec",
                                                                     FullName = "December",
                                                                     Shorthand = "dec"
                                                                 }
                                                         };

        public string Shorthand { get; set; }
        public int Number { get; set; }
        public string LocalName { get; set; }
        public string ServerName { get; set; }
        public string FullName { get; set; }
    }
}