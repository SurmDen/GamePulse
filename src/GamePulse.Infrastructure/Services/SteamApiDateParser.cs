using GamePulse.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.Services
{
    public class SteamApiDateParser : IDateParser
    {
        public DateTime ParseDateFromString(string date)
        {
            //23 Oct, 2025
            //November 2025
            if (string.IsNullOrWhiteSpace(date))
                throw new ArgumentException("Date string cannot be empty");

            date = date.Trim();

            if (date.Contains(","))
            {
                var parts = date.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    int day = int.Parse(parts[0]);
                    string monthStr = parts[1];
                    int year = int.Parse(parts[2]);

                    return new DateTime(year, ParseMonth(monthStr), day);
                }
            }
            else
            {
                var parts = date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    string monthStr = parts[0];
                    int year = int.Parse(parts[1]);

                    int month = ParseMonth(monthStr);

                    return new DateTime(year, month, DateTime.DaysInMonth(year, month));
                }
            }

            throw new FormatException($"Invalid date format: {date}");
        }

        private int ParseMonth(string monthStr)
        {
            return monthStr.ToLower() switch
            {
                "jan" or "january" => 1,
                "feb" or "february" => 2,
                "mar" or "march" => 3,
                "apr" or "april" => 4,
                "may" => 5,
                "jun" or "june" => 6,
                "jul" or "july" => 7,
                "aug" or "august" => 8,
                "sep" or "september" => 9,
                "oct" or "october" => 10,
                "nov" or "november" => 11,
                "dec" or "december" => 12,
                _ => throw new FormatException($"Invalid month: {monthStr}")
            };
        }
    }
}
