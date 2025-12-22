using Lab1;
using Xunit;

namespace Lab1.Tests;

public class PersonTests
{
    [Fact]
    public void FullName_ShouldReturnFirstNameAndLastName()
    {
        var person = new Person
        {
            FirstName = "Дмитрий",
            LastName = "Волков"
        };
        var fullName = person.FullName;
        Assert.Equal("Дмитрий Волков", fullName);
    }

    [Fact]
    public void IsAdult_ShouldReturnTrue_WhenAgeIs18OrMore()
    {
        var person = new Person { Age = 18 };
        var isAdult = person.IsAdult;
        Assert.True(isAdult);
    }

    [Fact]
    public void IsAdult_ShouldReturnFalse_WhenAgeIsLessThan18()
    {
        var person = new Person { Age = 17 };
        var isAdult = person.IsAdult;
        Assert.False(isAdult);
    }

    [Fact]
    public void Email_ShouldSetValue_WhenValidEmail()
    {
        var person = new Person();
        var validEmail = "dim@mail.ru";
        person.Email = validEmail;
        Assert.Equal(validEmail, person.Email);
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenEmailDoesNotContainAtSymbol()
    {
        var person = new Person();
        var exception = Assert.Throws<ArgumentException>(() => person.Email = "invalidemail");
        Assert.Contains("@", exception.Message);
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenEmailIsNullOrWhiteSpace()
    {
        var person = new Person();
        Assert.Throws<ArgumentException>(() => person.Email = "");
        Assert.Throws<ArgumentException>(() => person.Email = "   ");
        Assert.Throws<ArgumentException>(() => person.Email = null!);
    }

    [Fact]
    public void BirthDate_ShouldGetAndSetValue()
    {
        var person = new Person();
        var birthDate = new DateTime(1992, 8, 23);
        person.BirthDate = birthDate;
        Assert.Equal(birthDate, person.BirthDate);
    }

    [Fact]
    public void Password_ShouldNotBeSerialized()
    {
        var person = new Person
        {
            FirstName = "Евгений",
            LastName = "Васильев",
            Password = "vas0501"
        };
        var json = System.Text.Json.JsonSerializer.Serialize(person);
        Assert.DoesNotContain("Password", json);
        Assert.DoesNotContain("vas0501", json);
    }

    [Fact]
    public void Id_ShouldBeSerializedAsPersonId()
    {
        var person = new Person
        {
            Id = "010",
            FirstName = "Олег",
            LastName = "Василевский"
        };
        var json = System.Text.Json.JsonSerializer.Serialize(person);
        Assert.Contains("personId", json);
        Assert.Contains("010", json);
        Assert.DoesNotContain("\"Id\"", json);
    }

    [Fact]
    public void PhoneNumber_ShouldBeSerializedAsPhone()
    {
        var person = new Person
        {
            PhoneNumber = "+7-911-554-34-56",
            FirstName = "Катерина",
            LastName = "Федорова"
        };
        var json = System.Text.Json.JsonSerializer.Serialize(person);
        Assert.Contains("phone", json);
        Assert.Contains("+7-911-554-34-56", json);
        Assert.DoesNotContain("\"PhoneNumber\"", json);
    }
}
