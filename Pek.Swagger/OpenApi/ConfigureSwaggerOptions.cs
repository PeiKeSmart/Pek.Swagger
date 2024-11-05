using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pek.Swagger.OpenApi;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider; // 版本描述提供程序

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider; // 构造函数注入
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions) // 遍历每个版本描述
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description)); // 创建 Swagger 文档
        }

        // 添加令牌认证选项以传递 Bearer 令牌
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header, // 定义位置为头部
            Description = "请输入令牌", // 描述
            Name = "Authorization", // 头部名称
            Type = SecuritySchemeType.Http, // 类型为 HTTP
            BearerFormat = "JWT", // Bearer 格式
            Scheme = "bearer" // 方案
        });

        // 添加安全方案
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, // 类型为安全方案
                            Id = "Bearer" // 引用 ID
                        }
                    },
                    new string[] { } // 允许的范围
                }
            });
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription apiVersionDescription) // 创建版本信息
    {
        var info = new OpenApiInfo
        {
            Title = "API 版本管理", // 标题
            Version = apiVersionDescription.ApiVersion.ToString(), // 版本号
            Description = "API 版本管理的 Swagger 文档。" // 描述
        };

        // 添加已弃用 API 描述
        if (apiVersionDescription.IsDeprecated)
        {
            info.Description += " 此 API 版本已被弃用。"; // 说明
        }

        return info; // 返回信息
    }
}