using API.Dtos.Identity;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configurations.Swagger
{
    public class RegisterDtoSwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(RegisterDto))
            {
                schema.Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString(""),
                    ["fullName"] = new OpenApiString(""),
                    ["password"] = new OpenApiString("")
                };
            }
        }
    }
}