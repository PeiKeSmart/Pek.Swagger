using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pek.Swagger;

public class CustomDocumentFilter : IDocumentFilter {
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var path in swaggerDoc.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                // 检查并修改 Content-Type
                var requestBody = operation.Value.RequestBody;
                if (requestBody != null && requestBody.Content.ContainsKey("multipart/form-data"))
                {
                    requestBody.Content.Remove("multipart/form-data");
                    requestBody.Content.Add("multipart/form-data", new OpenApiMediaType());
                }
            }
        }
    }
}
