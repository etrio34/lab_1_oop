using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Lab1Final
{
    public class GridModel
    {
        public Dictionary<string, string> Cells { get; set; } = new Dictionary<string, string>();

        private Dictionary<string, double> _evaluatedCache = new Dictionary<string, double>();
        private HashSet<string> _currentlyEvaluating = new HashSet<string>();

        public void SetCell(string name, string expression)
        {
            Cells[name.ToUpper()] = expression;
        }

        public string GetCellExpression(string name)
        {
            name = name.ToUpper();
            return Cells.ContainsKey(name) ? Cells[name] : string.Empty;
        }

        public double GetCellValue(string name)
        {
            name = name.ToUpper();

            if (_evaluatedCache.ContainsKey(name))
                return _evaluatedCache[name];

            if (!Cells.ContainsKey(name) || string.IsNullOrWhiteSpace(Cells[name]))
                return 0.0;

            if (_currentlyEvaluating.Contains(name))
                throw new Exception("Циклічне посилання!");

            _currentlyEvaluating.Add(name);

            try
            {
                string expr = Cells[name];
                double result = 0.0;

                if (expr.StartsWith("="))
                    result = Calculator.Evaluate(expr.Substring(1), GetCellValue);
                else
                    double.TryParse(expr, out result);

                _evaluatedCache[name] = result;
                return result;
            }
            finally
            {
                _currentlyEvaluating.Remove(name);
            }
        }

        public void ClearCache()
        {
            _evaluatedCache.Clear();
        }

        public void SaveToFile(string filePath)
        {
            string json = JsonSerializer.Serialize(Cells, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Cells = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
        }
    }
}