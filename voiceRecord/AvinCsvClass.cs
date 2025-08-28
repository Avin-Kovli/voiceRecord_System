using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace voiceRecord
{
    internal class AvinCsvClass
    {
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "\"\"";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }
        
        public  void AppendToCsvAvin(string filePath, string[] row)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        string field = EscapeCsvField( row[i]) ;
                        writer.Write(field);

                        if (i < row.Length - 1)
                        {
                            writer.Write(",");
                        }
                    }
                    writer.WriteLine();
                }

                //MessageBox.Show($"Row appended to CSV: {filePath}");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error appending row to CSV: {ex.Message}");
            }
        }

        public  List<string[]> ReadFromCsv(string filePath, bool hasHeaders = true)
        {
            var data = new List<string[]>();

            try
            {
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    // Skip headers if present
                    if (hasHeaders && !reader.EndOfStream)
                        reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] fields = ParseCsvLine(line);
                            data.Add(fields);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading CSV: {ex.Message}");
            }

            return data;
        }

        private static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char currentChar = line[i];

                if (currentChar == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped quote
                        currentField.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        // Toggle quote state
                        inQuotes = !inQuotes;
                    }
                }
                else if (currentChar == ',' && !inQuotes)
                {
                    // End of field
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(currentChar);
                }
            }

            // Add the last field
            fields.Add(currentField.ToString());

            return fields.ToArray();
        }

        public void ModifyCsvValueWhenLogIn(string filePath, string newValue, string userName, string passwrod)
        {
            var lines = File.ReadAllLines(filePath).ToList();

            for (int rowIndex = 1; rowIndex < lines.Count; rowIndex++)
            {
                var columns = ParseCsvLine(lines[rowIndex]).ToArray();
                int columnIndex = 0;
                //for (int columnIndex = 0; columnIndex < columns.Length; columnIndex = columnIndex + 3)
                //{
                    if (columns[columnIndex] == userName && columns[columnIndex + 1] == passwrod)
                    {
                        columns[columnIndex + 2] = EscapeCsvField(newValue);
                        lines[rowIndex] = string.Join(",", columns);
                        break;
                    }
                //}
            }
            File.WriteAllLines(filePath, lines);
        }

        public void ModifyUserFileCsv(string filePath, string newValue, string audioPath)
        {
            var lines = File.ReadAllLines(filePath);
            lines[0]=newValue; lines[1]=audioPath;
            File.WriteAllLines(filePath, lines);
        }


    }
}
