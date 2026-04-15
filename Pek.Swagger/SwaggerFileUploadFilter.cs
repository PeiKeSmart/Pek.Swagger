using Microsoft.OpenApi;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pek.Swagger;

public class SwaggerFileUploadFilter : IOperationFilter
{
    private static readonly string[] FileParameters = new[] { "ContentType", "ContentDisposition", "Headers", "Length", "Name", "FileName" };
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod;
        if (!(((String.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase) ||
            String.Equals(httpMethod, "PUT", StringComparison.OrdinalIgnoreCase)) && context.ApiDescription.ParameterDescriptions.Any(s => s.Type == typeof(IFormFile) || s.Type == typeof(IFormFileCollection)))))
        {
            return;
        }
        RemoveExistingFileParameters(operation.Parameters ??= new List<IOpenApiParameter>());

        #region 留用
        //IDictionary<string, OpenApiSchema> pro = new Dictionary<string, OpenApiSchema>();
        //foreach (var item in context.ApiDescription.ParameterDescriptions)
        //{
        //	if (item.Type == typeof(IFormFileCollection) || item.Type == typeof(IFormFile))
        //	{
        //		pro.Add($"{item.Name}", new OpenApiSchema()
        //		{
        //			Description = "Select file",
        //			Type = "string",
        //			Format = "binary"
        //		});
        //	}
        //}
        //operation.RequestBody = new OpenApiRequestBody()
        //{
        //	Content =
        //		{
        //			["multipart/form-data"] = new OpenApiMediaType()
        //			{
        //				Schema = new OpenApiSchema()
        //				{
        //					Type = "object",
        //					Properties = pro
        //				}
        //			} ,
        //		}
        //};
        #endregion

        operation.Responses ??= new OpenApiResponses();
        operation.Responses.Clear();
        operation.Responses.Add("200", new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>
        {
            {
                "multipart/form-data", new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Format = "binary"
                    }
                }
            }
        }
        });
    }

    private void RemoveExistingFileParameters(IList<IOpenApiParameter> operationParameters)
    {
        foreach (var parameter in operationParameters.Where(p => p.In == ParameterLocation.Query && FileParameters.Contains(p.Name)).ToList())
        {
            operationParameters.Remove(parameter);
        }
    }
}