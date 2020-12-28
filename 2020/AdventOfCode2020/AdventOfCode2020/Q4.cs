using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q4
    {
        public static int Solve()
        {
            var pathToInputs = @"/Users/tomaszszymaniec/Documents/Coding/Classic/AdventOfCode/2020/inputs/q4.txt";
            var lines = Tools.ReadText(pathToInputs);

            var validPassports = 0;
            var currentPassport = "";
            foreach (var line in lines)
            {
                if (line == string.Empty)
                {
                    Console.WriteLine(currentPassport);

                    if (IsValidPassport(currentPassport))
                    {
                        Console.WriteLine("**Valid passport!**\n");
                        validPassports++;
                    }
                    else
                    {
                        Console.WriteLine("**Invalid passport!**\n");
                    }
                    currentPassport = "";
                }
                else
                {
                    currentPassport += $" {line}";
                }
            }

            Console.WriteLine($"Valid passport: {validPassports}");
            return validPassports;
        }

        private static bool IsValidPassport(string passport)
        {
            var elements = passport.Split(" ").Select(e => e.Trim());
            var map = new Dictionary<string, string>();
            
            foreach (var elem in elements)
            {
                if (elem == string.Empty) continue;
                if (!elem.Contains(":")) throw new Exception($"Unexpected elem: {elem}");
                var keyValuePair = elem.Split(":");
                if (keyValuePair.Length != 2) throw new Exception($"Unexpected elem: {elem}");
                map[keyValuePair[0].Trim()] = keyValuePair[1].Trim();
            }

            return
                ValidateBirthYear(map) && ValidateIssueYear(map) && 
                ValidateExpirationYear(map) && ValidateHeight(map) && 
                ValidateHairColor(map) && ValidateEyeColor(map) && ValidatePassportId(map);
        }

        private static bool ValidateBirthYear(IReadOnlyDictionary<string, string> elemsMap)
        {
            if (!elemsMap.ContainsKey("byr")) return false;
            var birthYear = elemsMap["byr"];
            return ValidateNumber(birthYear, 4, 1920, 2002);
        }

        private static bool ValidateIssueYear(IReadOnlyDictionary<string, string> elemsMap)
        {
            if (!elemsMap.ContainsKey("iyr")) return false;
            var issueYear = elemsMap["iyr"];
            return ValidateNumber(issueYear, 4, 2010, 2020);
        }

        private static bool ValidateExpirationYear(IReadOnlyDictionary<string, string> elemsMap)
        {
            if (!elemsMap.ContainsKey("eyr")) return false;
            var expirationYear = elemsMap["eyr"];
            return ValidateNumber(expirationYear, 4, 2020, 2030);
        }

        private static bool ValidateHeight(IReadOnlyDictionary<string, string> elemsMap)
        {
            if (!elemsMap.ContainsKey("hgt")) return false;
            var height = elemsMap["hgt"];
            var units = height.Substring(height.Length - 2, 2);
            var heightValue = height.Substring(0, height.Length - 2);

            return units switch
            {
                "cm" => ValidateNumber(heightValue, 3, 150, 193),
                "in" => ValidateNumber(heightValue, 2, 59, 76),
                _ => false
            };
        }

        private static bool ValidateHairColor(IReadOnlyDictionary<string, string> elemsMap)
        {
            if (!elemsMap.ContainsKey("hcl")) return false;
            var hairColor = elemsMap["hcl"];
            return hairColor.StartsWith("#") && int.TryParse(hairColor.Substring(1, hairColor.Length - 1), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var _);
        }

        private static bool ValidateEyeColor(IReadOnlyDictionary<string, string> elemsMap)
        {
            if (!elemsMap.ContainsKey("ecl")) return false;
            var eyeColor = elemsMap["ecl"];
            var validEyeColors = new [] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"};
            return validEyeColors.Contains(eyeColor);
        }

        private static bool ValidatePassportId(IReadOnlyDictionary<string, string> elemsMap)
        {
            if (!elemsMap.ContainsKey("pid")) return false;
            var passportId = elemsMap["pid"];
            return ValidateNumber(passportId, 9, int.MinValue, int.MaxValue);
        }

        private static bool ValidateNumber(string input, int requiredDigits, int minValue, int maxValue)
        {
            if (int.TryParse(input, out var parsed))
            {
                return input.Length == requiredDigits && parsed >= minValue && parsed <= maxValue;
            }
            return false;
        }
        
    }
}