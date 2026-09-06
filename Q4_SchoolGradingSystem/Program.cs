using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // ===== a. Student class =====
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    // ===== b, c. Custom exceptions =====
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // ===== d. StudentResultProcessor =====
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();
            int lineNumber = 0;

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue; // skip blank lines

                    var fields = line.Split(',');

                    if (fields.Length < 3)
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: expected 3 fields (ID, Name, Score) but found {fields.Length}.");
                    }

                    string idText = fields[0].Trim();
                    string fullName = fields[1].Trim();
                    string scoreText = fields[2].Trim();

                    if (!int.TryParse(idText, out int id))
                    {
                        throw new MissingFieldException($"Line {lineNumber}: student ID '{idText}' is invalid or missing.");
                    }

                    if (!int.TryParse(scoreText, out int score))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: score '{scoreText}' could not be converted to a number.");
                    }

                    students.Add(new Student(id, fullName, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== School Grading System ===\n");

            string inputFilePath = "students.txt";
            string outputFilePath = "report.txt";

            // Create a sample input file so the program can run out-of-the-box
            EnsureSampleInputFileExists(inputFilePath);

            var processor = new StudentResultProcessor();

            try
            {
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine($"Successfully processed {students.Count} student(s).");
                Console.WriteLine($"Report written to: {Path.GetFullPath(outputFilePath)}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: The input file '{inputFilePath}' was not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Invalid score format: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Missing field: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        // Creates a small sample students.txt file if one doesn't already exist,
        // so the program works immediately without extra setup.
        static void EnsureSampleInputFileExists(string path)
        {
            if (File.Exists(path))
                return;

            var sampleLines = new[]
            {
                "101,Alice Smith,84",
                "102,Kwabena Owusu,72",
                "103,Linda Asante,65",
                "104,John Mensah,45",
                "105,Grace Boateng,91"
            };

            File.WriteAllLines(path, sampleLines);
        }
    }
}
