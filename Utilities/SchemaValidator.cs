using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Xunit.Abstractions;

namespace QPROS_APITestingAssignment.Utilities;

public class SchemaValidator
{
    private readonly Dictionary<string, JSchema> _schemas;
    private readonly ITestOutputHelper _output;

    public SchemaValidator(ITestOutputHelper output)
    {
        _output = output;
        _schemas = new Dictionary<string, JSchema>();
        LoadSchemas();
    }

    private void LoadSchemas()
    {
        var schemaFiles = new Dictionary<string, string>
        {
            { "CreateUser", "create-user-schema.json" }
        };

        foreach (var schemaFile in schemaFiles)
        {
            var schemaPath = Path.Combine("Resources", "Schemas", schemaFile.Value);
            var schemaJson = File.ReadAllText(schemaPath);
            _schemas[schemaFile.Key] = JSchema.Parse(schemaJson);
        }
    }

    public bool ValidateJson(string json, string schemaType)
    {
        if (!_schemas.TryGetValue(schemaType, out var schema))
        {
            throw new ArgumentException($"Schema type '{schemaType}' not found");
        }

        var jsonObject = JToken.Parse(json);
        bool isValid = jsonObject.IsValid(schema, out IList<string> errorMessages);

        if (!isValid)
        {
            _output.WriteLine("Schema validation errors:");
            foreach (var error in errorMessages)
            {
                _output.WriteLine($"- {error}");
            }
        }

        return isValid;
    }
}

