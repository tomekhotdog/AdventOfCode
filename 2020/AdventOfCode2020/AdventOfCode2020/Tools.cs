using System;
using System.IO;

namespace AdventOfCode2020
{
    public static class Tools
    {
        private const string PathToInputs = @"/Users/tomaszszymaniec/Documents/Coding/Classic/AdventOfCode/2020/inputs/";
        
        public static string[] ReadText(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"Could not file at path: {path}");
            }
            return File.ReadAllLines(path);
        }

        public static string[] GetInput(int questionNumber, bool testInput = false)
        {
            var extension = testInput ? $"q{questionNumber}_test.txt" : $"q{questionNumber}.txt";
            return ReadText(PathToInputs + extension);
        }
    }
}