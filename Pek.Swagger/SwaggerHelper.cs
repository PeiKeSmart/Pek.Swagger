using Microsoft.AspNetCore.Mvc.ApiExplorer;

using NewLife;

namespace Pek.Swagger;

public static class SwaggerHelper
{
    /// <summary>
    /// action没有httpmethod attribute的情况下根据action的开头名称给与默认值
    /// </summary>
    /// <param name="app">IApplicationBuilder</param>
    /// <param name="defaultHttpMethod">默认给定的HttpMethod</param>
    public static void AutoHttpMethodIfActionNoBind(this IApplicationBuilder app, String? defaultHttpMethod = null)
    {
        //从容器中获取IApiDescriptionGroupCollectionProvider实例
        var apiDescriptionGroupCollectionProvider = app.ApplicationServices.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
        var apiDescriptionGroupsItems = apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items;
        //遍历ApiDescriptionGroups
        foreach (var apiDescriptionGroup in apiDescriptionGroupsItems)
        {
            foreach (var apiDescription in apiDescriptionGroup.Items)
            {
                if (string.IsNullOrEmpty(apiDescription.HttpMethod))
                {
                    //获取Action名称
                    var actionName = apiDescription.ActionDescriptor.RouteValues["action"];

                    if (actionName.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    //默认给定POST
                    var methodName = defaultHttpMethod ?? "POST";
                    //根据Action开头单词给定HttpMethod默认值
                    if (actionName.StartsWith("get", StringComparison.OrdinalIgnoreCase))
                    {
                        methodName = "GET";
                    }
                    else if (actionName.StartsWith("put", StringComparison.OrdinalIgnoreCase))
                    {
                        methodName = "PUT";
                    }
                    else if (actionName.StartsWith("delete", StringComparison.OrdinalIgnoreCase))
                    {
                        methodName = "DELETE";
                    }
                    apiDescription.HttpMethod = methodName;
                }
            }
        }
    }
}
