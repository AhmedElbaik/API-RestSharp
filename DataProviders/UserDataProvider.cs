using QPROS_APITestingAssignment.Utilities;

namespace QPROS_APITestingAssignment.DataProviders;

public class UserDataProvider
{
    public static IEnumerable<object[]> CreateUserData()
    {
        yield return new object[]
        {
            new {
                id = 10,
                username = FakeDataGenerator.GenerateUsername(),
                firstName = FakeDataGenerator.Faker.Name.FirstName(),
                lastName = FakeDataGenerator.Faker.Name.LastName(),
                email = FakeDataGenerator.GenerateEmail(),
                password = FakeDataGenerator.GeneratePassword(),
                phone = FakeDataGenerator.Faker.Phone.PhoneNumber(),
                userStatus = 1
            }
        };
    }

    public static IEnumerable<object[]> CreateMultipleUserData()
    {
        yield return new object[]
        {
            new[]
            {
                new {
                    id = 10,
                    username = FakeDataGenerator.GenerateUsername(),
                    firstName = FakeDataGenerator.Faker.Name.FirstName(),
                    lastName = FakeDataGenerator.Faker.Name.LastName(),
                    email = FakeDataGenerator.GenerateEmail(),
                    password = FakeDataGenerator.GeneratePassword(),
                    phone = FakeDataGenerator.Faker.Phone.PhoneNumber(),
                    userStatus = 1
                },
                new {
                    id = 11,
                    username = FakeDataGenerator.GenerateUsername(),
                    firstName = FakeDataGenerator.Faker.Name.FirstName(),
                    lastName = FakeDataGenerator.Faker.Name.LastName(),
                    email = FakeDataGenerator.GenerateEmail(),
                    password = FakeDataGenerator.GeneratePassword(),
                    phone = FakeDataGenerator.Faker.Phone.PhoneNumber(),
                    userStatus = 1
                }
            }
        };
    }
}