using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AP.BlazorWASMTests;

public class PagesTest : TestContext
{
    private readonly FakeWebAssemblyHostEnvironment _hostEnvironment;
    private readonly IRenderedComponent<BlazorWASM.Pages.Index> _cut;

    public PagesTest()
    {
        // Arrange
        _hostEnvironment = Services.GetRequiredService<FakeWebAssemblyHostEnvironment>();
        _hostEnvironment.SetEnvironmentToDevelopment();

        // Act
        // cut = Component Under Test
        _cut = RenderComponent<BlazorWASM.Pages.Index>();
    }

    // Er der en tabel på siden
    [Fact]
    public void Index_Shows_Table()
    {
        // Arrange
        
        // Act
        var tableTags = _cut.Find("table");
        var theadTags = _cut.Find("thead");

        // Assert
        Assert.NotNull(theadTags);
        tableTags.MarkupMatches("<table class=\"table\" diff:ignoreChildren></table>");
        theadTags.MarkupMatches("<thead diff:ignoreChildren></thead>");
    }

    // Er H1 inkluderet i markup og viser den en bestemt text?
    [Fact]
    public void Index_Contains_H1_tag()
    {
        // Arrange

        // Act
        var h1Tag = _cut.Find("h1");

        // Assert
        Assert.NotNull(h1Tag);
        h1Tag.MarkupMatches("<h1>Number of messages recieved: 0</h1>");
    }

    // Er H3 teksten er rigtig?
    [Fact]
    public void App_Is_In_Environment()
    {
        // Arrange
        var environment = _hostEnvironment.IsProduction() ? "Production" : "Development";

        // Act
        var h3Tags = _cut.FindAll("h3");

        // Assert
        Assert.NotNull(h3Tags[1]);
        h3Tags[1].MarkupMatches($"<h3>App is in {environment}</h3>");
    }

    // Er StartHubConnection er connecting fra starten
    [Fact]
    public void HubConnection_Is_Connecting()
    {
        // Arrange

        // Act
        var h2Tag = _cut.Find("h2");

        // Assert
        Assert.NotNull(h2Tag);
        h2Tag.MarkupMatches($"<h2>Connecting</2>");
    }

    // Er StartHubConnection connected efterventetid?
    // Denne test kan fejle hvis Azure Web App Service ikke når at vågne på de 15 sekunder der er sat af!
    [Fact]
    public void HubConnection_Is_Connected()
    {
        // Arrange

        // Act
        _cut.WaitForState(() => _cut.Find("h2").TextContent == "Connected", TimeSpan.FromSeconds(15)); // Grunden til det lange timespan er at min web app instance skal vågne på Azure, da den måske er i dvale..
        var h2Tag = _cut.Find("h2");

        // Assert
        Assert.NotNull(h2Tag);
        h2Tag.MarkupMatches($"<h2>Connected</2>");
    }
}