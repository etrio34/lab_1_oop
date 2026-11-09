using Microsoft.Maui.Controls;
using System;
using System.IO;

namespace Lab1Final
{
    public partial class MainPage : ContentPage
    {
        private int _rowCount = 10;
        private int _colCount = 5;
        private bool _showFormulas = true;
        private GridModel _model = new GridModel();

        private Entry[,] _uiCells = null!;

        public MainPage()
        {
            InitializeComponent();
            BuildGrid();
        }

        private void BuildGrid()
        {
            SpreadsheetGrid.Children.Clear();
            SpreadsheetGrid.RowDefinitions.Clear();
            SpreadsheetGrid.ColumnDefinitions.Clear();
            _uiCells = new Entry[_rowCount, _colCount];

            SpreadsheetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            for (int c = 0; c < _colCount; c++)
                SpreadsheetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 100 });

            SpreadsheetGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int r = 0; r < _rowCount; r++)
                SpreadsheetGrid.RowDefinitions.Add(new RowDefinition { Height = 40 });

            for (int c = 0; c < _colCount; c++)
            {
                var label = new Label { Text = GetColumnName(c), FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
                SpreadsheetGrid.Add(label, c + 1, 0);
            }

            for (int r = 0; r < _rowCount; r++)
            {
                var rowLabel = new Label { Text = (r + 1).ToString(), FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                SpreadsheetGrid.Add(rowLabel, 0, r + 1);

                for (int c = 0; c < _colCount; c++)
                {
                    string cellName = $"{GetColumnName(c)}{r + 1}";
                    var entry = new Entry
                    {
                        Text = _model.GetCellExpression(cellName),
                        BackgroundColor = Colors.White,
                        TextColor = Colors.Black
                    };

                    entry.Completed += (s, e) =>
                    {
                        _model.SetCell(cellName, entry.Text);
                        RefreshGridValues();
                    };

                    entry.Unfocused += (s, e) =>
                    {
                        _model.SetCell(cellName, entry.Text);
                        RefreshGridValues();
                    };

                    _uiCells[r, c] = entry;
                    SpreadsheetGrid.Add(entry, c + 1, r + 1);
                }
            }
            RefreshGridValues();
        }

        private void RefreshGridValues()
        {
            _model.ClearCache();
            for (int r = 0; r < _rowCount; r++)
            {
                for (int c = 0; c < _colCount; c++)
                {
                    string cellName = $"{GetColumnName(c)}{r + 1}";
                    var entry = _uiCells[r, c];

                    string expr = _model.GetCellExpression(cellName);

                    if (_showFormulas)
                    {
                        entry.Text = expr;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(expr))
                        {
                            entry.Text = string.Empty;
                        }
                        else
                        {
                            try
                            {
                                double value = _model.GetCellValue(cellName);
                                entry.Text = value.ToString();
                            }
                            catch
                            {
                                entry.Text = "#ERROR";
                            }
                        }
                    }
                }
            }
        }

        private string GetColumnName(int index)
        {
            int dividend = index + 1;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }
            return columnName;
        }

        private void OnToggleViewClicked(object sender, EventArgs e)
        {
            _showFormulas = !_showFormulas;
            BtnToggleView.Text = _showFormulas ? "Режим: Вирази" : "Режим: Значення";
            RefreshGridValues();
        }

        private void OnAddRowClicked(object sender, EventArgs e)
        {
            _rowCount++;
            BuildGrid();
        }

        private void OnAddColClicked(object sender, EventArgs e)
        {
            _colCount++;
            BuildGrid();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "spreadsheet.json");
            _model.SaveToFile(path);
            await DisplayAlert("Збережено", $"Таблицю успішно збережено у файл:\n{path}", "OK");
        }

        private async void OnLoadClicked(object sender, EventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "spreadsheet.json");
            if (File.Exists(path))
            {
                _model.LoadFromFile(path);
                BuildGrid();
                await DisplayAlert("Завантажено", "Таблицю успішно завантажено.", "OK");
            }
            else
            {
                await DisplayAlert("Помилка", "Файл збереження не знайдено.", "OK");
            }
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Допомога",
                "Табличний процесор (Вар. 39).\n" +
                "Автор: Шостак Денис, К-25\n\n" +
                "Підтримувані функції:\n" +
                "- Бінарні та унарні операції: +, -, *, /\n" +
                "- Відношення: =, <, >\n" +
                "- Функції: mmax(x,y), mmin(x,y)\n" +
                "- Логіка: not\n" +
                "Вираз починається зі знаку '='", "Зрозуміло");
        }
    }
}
