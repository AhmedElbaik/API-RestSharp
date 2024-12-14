# **Petstore API Testing Project**

This project is designed to test the APIs for the [Swagger Petstore v3](https://petstore3.swagger.io/), which provides a RESTful API for petstore management. The project uses **RestSharp** for API testing and **ExtentReports** for test reporting.

---

## **Project Structure**

```
QPROS_APITestingAssignment
├── DataProviders
│   └── UserDataProvider.cs            # Provides user data for tests
├── Resources
│   └── Schemas
│       └── create-user-schema.json    # JSON schema for user creation
├── Tests
│   ├── BaseTest.cs                    # Test base class for setup/teardown
│   ├── CreateUsersWithListTests.cs    # Tests for creating users
│   └── UserManagementTests.cs         # Tests for user lifecycle and retrieval
├── Utilities
│   ├── FakeDataGenerator.cs           # Generates fake test data
│   ├── InitializationForExtentReports.cs # Configures ExtentReports
│   └── SchemaValidator.cs             # Validates API responses against JSON schemas
├── Dockerfile                         # Docker configuration for building and running tests
├── Readme.md                          # Project documentation
└── .gitattributes
```

---

## **Technologies & Dependencies**

- **RestSharp**: HTTP client library for REST API testing.
- **xUnit**: Unit testing framework.
- **FluentAssertions**: Simplifies test assertions with a fluent API.
- **Bogus**: Generates random test data.
- **Newtonsoft.Json**: JSON serialization and deserialization.
- **Newtonsoft.Json.Schema**: Schema validation for JSON responses.
- **ExtentReports**: Generates detailed and interactive test reports.

---

## **Test Scenarios**

This project tests various API scenarios, categorized into **Positive** and **Negative** test cases:

### **Positive Scenarios**
1. **CreateUsersWithList_ShouldReturnSuccessfulResponse**  
   - Verifies that creating users in bulk returns a successful response.

2. **UserLifecycle_CreateUpdateGetDelete_ShouldSucceed**  
   - Tests the complete user lifecycle:
     1. Create a user.
     2. Retrieve user information.
     3. Update user details.
     4. Login with the updated user.
     5. Logout.
     6. Delete the user.

---

### **Negative Scenarios**
1. **GetUser_InvalidUsername_ShouldReturnMethodNotAllowed**  
   - Validates the API behavior when attempting to retrieve a user with an invalid username.

2. **GetUser_InvalidUsername_ShouldReturnNotFound**  
   - Ensures the API returns a `404 Not Found` status for invalid user retrieval.

---

## **Running the Project with Docker**

The project is containerized using Docker, enabling seamless test execution in any environment.

### **Dockerfile Configuration**

The **Dockerfile** performs the following steps:
1. Builds the project.
2. Executes the tests.
3. Generates the Extent Report as `extentreport.html`.

### **Docker Commands**

1. **Build the Docker Image:**
   ```bash
   docker build -t petstore_restsharp_image .
   ```

2. **Run the Container and Export the Report:**
   ```bash
   docker run --rm -v $(pwd)/test-results:/app/test-results petstore_restsharp_image
   ```

   - `--rm`: Automatically removes the container after execution.
   - `-v`: Mounts a local directory (`test-results`) to the container's `/app/test-results` folder to save the Extent Report.

3. **Check the Report:**
   - Navigate to the `test-results` directory in your local project.
   - Open the `extentreport.html` file to review the test results.

---

## **Extent Reports**

The test execution generates an **interactive HTML report** using ExtentReports. The report includes:
- Test scenario status (Pass/Fail).
- Error details for failed tests.
- Execution logs and timestamps.

The report is saved to:  
`/bin/Debug/net8.0/extentreport.html`

---

## **Future Improvements**

1. Add more scenarios for edge cases.
2. Integrate with CI/CD tools (e.g., Jenkins, GitHub Actions).
3. Add performance and load testing for the APIs.
4. Parameterize test data for broader coverage.

---

## **Conclusion**

This project offers a robust API testing framework for the **Swagger Petstore** APIs. By combining RestSharp, ExtentReports, and Docker, it provides:
- Reliable test execution.
- Clear, detailed reporting.
- Scalability through containerized environments.

---