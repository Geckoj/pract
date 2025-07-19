using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

public class StudentAppTests
{
    [Fact]
    public void Test_SerializeStudent()
    {
        var student = new Student
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            BirthDate = new DateTime(2000, 1, 1),
            Grades = new List<Subject>
            {
                new Subject { Name = "Math", Grade = 5 },
                new Subject { Name = "History", Grade = 4 }
            }
        };

        var json = StudentApp.SerializeStudent(student);

        Assert.Contains("Ivan", json);
        Assert.Contains("Ivanov", json);
        Assert.Contains("2000-01-01", json);
        Assert.Contains("Math", json);
        Assert.Contains("History", json);
    }

    [Fact]
    public void Test_DeserializeStudent()
    {
        var json = @"{
            ""FirstName"": ""Ivan"",
            ""LastName"": ""Ivanov"",
            ""BirthDate"": ""2000-01-01"",
            ""Grades"": [
                { ""Name"": ""Math"", ""Grade"": 5 },
                { ""Name"": ""History"", ""Grade"": 4 }
            ]
        }";

        var student = StudentApp.DeserializeStudent(json);

        Assert.NotNull(student);
        Assert.Equal("Ivan", student.FirstName);
        Assert.Equal("Ivanov", student.LastName);
        Assert.Equal(new DateTime(2000, 1, 1), student.BirthDate);
        Assert.Equal(2, student.Grades.Count);
        Assert.Equal("Math", student.Grades[0].Name);
        Assert.Equal(5, student.Grades[0].Grade);
    }

    [Fact]
    public void Test_DeserializeInvalidJson()
    {
        var json = @"{
            ""FirstName"": ""Ivan"",
            ""LastName"": ""Ivanov"",
            ""BirthDate"": ""Invalid date"",
            ""Grades"": [
                { ""Name"": ""Math"", ""Grade"": 5 }
            ]
        }";

        var student = StudentApp.DeserializeStudent(json);

        Assert.Null(student);
    }

    [Fact]
    public void Test_SaveAndLoadFromFile()
    {
        var student = new Student
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            BirthDate = new DateTime(2000, 1, 1),
            Grades = new List<Subject>
            {
                new Subject { Name = "Math", Grade = 5 },
                new Subject { Name = "History", Grade = 4 }
            }
        };

        string filename = "student.json";
        var json = StudentApp.SerializeStudent(student);

        StudentApp.SaveToFile(filename, json);

        var loadedJson = StudentApp.LoadFromFile(filename);

        Assert.Equal(json, loadedJson);
        Assert.True(File.Exists(filename));

        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
    }

    [Fact]
    public void Test_SerializeWithNullValues()
    {
        var student = new Student
        {
            FirstName = null,
            LastName = "Ivanov",
            BirthDate = new DateTime(2000, 1, 1),
            Grades = null
        };

        var json = StudentApp.SerializeStudent(student);

        Assert.DoesNotContain("FirstName", json);
        Assert.Contains("Ivanov", json);
    }
}
