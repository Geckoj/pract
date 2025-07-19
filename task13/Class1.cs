using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Subject
{
    public string Name { get; set; }
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public List<Subject> Grades { get; set; }
}

public class StudentApp
{
    public static string SerializeStudent(Student student)
    {
        var options = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true,
            Converters = { new DateTimeConverter() }
        };
        return JsonSerializer.Serialize(student, options);
    }
    public static Student DeserializeStudent(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<Student>(json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error during deserialization: {ex.Message}");
            return null;
        }
    }

    public static void SaveToFile(string filename, string json)
    {
        File.WriteAllText(filename, json);
    }
    public static string LoadFromFile(string filename)
    {
        return File.ReadAllText(filename);
    }
}

public class DateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), "yyyy-MM-dd", null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}
