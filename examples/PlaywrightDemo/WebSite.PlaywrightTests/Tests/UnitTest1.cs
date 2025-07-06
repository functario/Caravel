using Microsoft.Playwright;

namespace WebSite.PlaywrightTests.Tests;

[Collection(nameof(PlaywrightCollection))]
public class UnitTest1
{
    private readonly PlaywrightFixture _playwrightFixture;
    private readonly IPage _page;

    public UnitTest1(PlaywrightFixture playwrightFixture)
    {
        _playwrightFixture = playwrightFixture;
        _page = _playwrightFixture.Page;
    }

    [Fact]
    public async Task Test1()
    {
        var filePath = Path.GetFullPath("WebSite.html");
        var fileUrl = new Uri(filePath).AbsoluteUri;
        await _page.GotoAsync(fileUrl);
    }
}
