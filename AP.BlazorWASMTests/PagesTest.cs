using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;

namespace AP.BlazorWASMTests;

public class PagesTest : TestContext
{
    [Fact]
    public void Index_Shows_Table()
    {
        // Arrange
        var navMan = Services.GetRequiredService<FakeNavigationManager>();
        var hostEnvironment = Services.GetRequiredService<FakeWebAssemblyHostEnvironment>();
        hostEnvironment.SetEnvironmentToDevelopment();


        // Act
        // cut = Component Under Test
        var cut = RenderComponent<BlazorWASM.Pages.Index>();

        // Assert
        cut.Markup.Contains(@"<table>...</table>");
    }

    // TEst af om Oninitialized henter uri
    [Fact]
    public void Index_OnInitialized_Gets_Uri()
    {
        // Arrange
        var navMan = Services.GetRequiredService<FakeNavigationManager>();
        var hostEnvironment = Services.GetRequiredService<FakeWebAssemblyHostEnvironment>();
        hostEnvironment.SetEnvironmentToDevelopment();

        var cut = RenderComponent<BlazorWASM.Pages.Index>();

        // Act


        // Assert
    }


    // Test af StartHubConnection sætter Hubconnection til connected

    // Test af om antallet af rækker i tabellen passer med antallet af indkomne firmaer?
}