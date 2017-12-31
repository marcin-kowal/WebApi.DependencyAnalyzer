using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApi.DependencyAnalyzer.Engine.Tests.Integration
{
    public class TestClass
    {
        private const string DashboardSettingsApiRoute = "api/v1/module/bid/{0}/user/{1}/dashboard/settings";
        private const string SwaggerApiRoute = "api/v1/swagger";

        private readonly string HierarchyApiRoute = string.Format(CultureInfo.InvariantCulture,
            "{0}/{1}/versions/{2}/dashboard/settings/hierarchy/",
            "api/v1/module-management/bids",
            7,
            9);

        [Display(Description = "api/v1/module/bid/{0}/user/{1}/personal/settings")]
        private int Method()
        {
            string hierarchyItemsApiRoute = "api/v1/module-management/bids/"
                + "{0}/{1}/versions/{2}/dashboard/settings/hierarchy/"
                + "items";

            return hierarchyItemsApiRoute.Length;
        }
    }
}