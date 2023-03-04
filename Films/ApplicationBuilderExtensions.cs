using Films.Core.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace Films;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(applicationBuilder =>
        {
            applicationBuilder.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature is null) return;

                // TODO: Log the exception here
            });
        });
    }
}