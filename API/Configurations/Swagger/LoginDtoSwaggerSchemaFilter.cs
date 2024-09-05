using API.Dtos.Identity;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configurations.Swagger
{
    public class LoginDtoSwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(LoginDto))
            {
                schema.Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString(""),
                    ["password"] = new OpenApiString("")
                };
            }
        }
    }
}
