using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace KickStat.Routing;

public static class SlugifyRoutingExtensions
{
    /// <summary>
    /// Добавить поддержку Url-ов в формате "get-all" вместо "GetAll"
    /// </summary>
    public static IServiceCollection AddSlugifyLowercaseRouting(this IServiceCollection services) =>
        services.AddRouting(option =>
        {
            option.ConstraintMap[SlugifyParameterTransformer.SLUGIFY] = typeof(SlugifyParameterTransformer);
            option.LowercaseUrls = true;
            //option.LowercaseQueryStrings = true;        // не надо пока, фиг знает, как себя идентификаторы поведут
        });

    /// <summary>
    /// Добавить поддержку Url-ов в формате "get-all" вместо "GetAll"
    /// </summary>
    public static IServiceCollection AddSlugifyRouting(this IServiceCollection services) =>
        services.AddRouting(option => { option.ConstraintMap[SlugifyParameterTransformer.SLUGIFY] = typeof(SlugifyParameterTransformer); });


    /// <summary>
    /// Добавить маршрут для Controllers для slugify Url-ов в формате "get-all" вместо "GetAll"
    /// Добавляется как: {controller:slugify=Home}/{action:slugify=Index}/{id?}
    /// </summary>
    public static ControllerActionEndpointConventionBuilder MapDefaultSlugifyControllerRoute(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapControllerRoute(
            "defaultControllerSlugify",
            "{controller:slugify=Home}/{action:slugify=Index}/{id?}");


    /// <summary>
    /// Добавить маршрут для Controllers и Area для slugify Url-ов в формате "get-all" вместо "GetAll".
    /// Рекомендуется использовать до определения маршрутов с контроллерами.
    /// Добавляется как: {area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}
    /// </summary>
    public static ControllerActionEndpointConventionBuilder MapDefaultSlugifyAreaControllerRoute(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapControllerRoute(
            "defaultAreaSlugify",
            "{area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}");
}