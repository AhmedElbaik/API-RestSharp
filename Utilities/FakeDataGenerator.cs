using Bogus;

namespace QPROS_APITestingAssignment.Utilities;

public static class FakeDataGenerator
{
    public static Faker Faker = new Faker("en");

    public static string GenerateName() => Faker.Name.FullName();
    public static string GenerateJob() => Faker.Name.JobTitle();
    public static string GenerateEmail() => Faker.Internet.Email();

    // New methods for user creation
    public static string GenerateUsername() => Faker.Internet.UserName();
    public static string GeneratePassword() => Faker.Internet.Password();
}
