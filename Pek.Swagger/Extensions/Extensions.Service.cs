using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;

using Pek.Swagger.OpenApi;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pek.Swagger.Extensions;

/// <summary>
/// 扩展服务
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 注册API版本
    /// </summary>
    /// <param name="services">服务集合</param>
    public static void AddApiVersionAndSwagger(this IServiceCollection services)
    {
        // https://yoagoa.com/article/api-versioning-and-enable-authorization-in-swagger-ui-net-core/
        // https://mp.weixin.qq.com/s/CAd6ozTlEUprmcp_MrgMxw
        // 添加 API Explorer，以提供可用版本的信息
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0); // 默认 API 版本（v1.0）
            options.AssumeDefaultVersionWhenUnspecified = true; // 未指定时假设默认版本
            options.ReportApiVersions = true; // 在响应头中报告 API 版本

            // 结合多种版本控制方式
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("version"),
                new UrlSegmentApiVersionReader(),   // 使用 URL 段版本（例如，/api/v1/resource）
                new HeaderApiVersionReader("X-API-Version"),
                new MediaTypeApiVersionReader("version")
            );
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; // 版本分组格式
            options.SubstituteApiVersionInUrl = true; // 在 URL 中替换 API 版本
        });

        services.AddSwaggerGen(options =>
        {
            // 使用解决冲突的策略
            options.ResolveConflictingActions(apiDescriptions =>
            {
                return apiDescriptions.First();
            });
        }); // 添加 Swagger 生成器

        // 添加自定义服务
        services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>(); // 注册 Swagger 配置
    }

    /// <summary>
    /// 注册Swagger接口文档
    /// </summary>
    /// <param name="app">应用程序生成器</param>
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger(); // 启用 Swagger
        app.UseSwaggerUI(options =>
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>(); // 获取版本描述提供程序

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant()); // 为每个版本配置 Swagger 端点
            }
        });
        //给没有配置httpmethod的action添加默认操作
        app.AutoHttpMethodIfActionNoBind();

        return app;
    }
}
