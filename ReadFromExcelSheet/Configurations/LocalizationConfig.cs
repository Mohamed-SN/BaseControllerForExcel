using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace ReadFromExcelSheet.Configurations
{
    public static class LocalizationConfig
    {
        public static void AddLocalizationConfig(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "";
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
            {
                 new CultureInfo("en-US"),
                 new CultureInfo("ar-SA")
            };

                options.DefaultRequestCulture = new RequestCulture("ar-SA");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });


        }
    }
}
