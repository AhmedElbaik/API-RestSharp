# Use the .NET SDK image for building and running tests
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the project
RUN dotnet build -c Release -o /app/build

# Run tests and output results to a specific directory
RUN mkdir -p /app/test-results && \
    dotnet test -c Release --logger trx --collect "Code coverage" --results-directory /app/test-results

# Use the .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build output and test results
COPY --from=build /app/build .
COPY --from=build /app/test-results /app/test-results

# Expose the directory where the test results, including extentreport.html, are located
VOLUME ["/app/test-results"]

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "QPROS_APITestingAssignment.dll"]
