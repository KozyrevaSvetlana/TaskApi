using TaskApi.Validation;

namespace TaskApi.Tests.Unit;

public class TitleValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Empty_titles_are_rejected(string? title)
    {
        Assert.False(TitleValidator.TryNormalize(title, out _, out var error));
        Assert.NotEmpty(error);
    }

    [Fact]
    public void Too_long_title_is_rejected_and_boundary_is_accepted()
    {
        Assert.False(TitleValidator.TryNormalize(new string('x', 201), out _, out _));
        Assert.True(TitleValidator.TryNormalize(new string('x', 200), out var normalized, out _));
        Assert.Equal(200, normalized.Length);
    }

    [Fact]
    public void Title_is_trimmed()
    {
        Assert.True(TitleValidator.TryNormalize("  Buy milk  ", out var normalized, out _));
        Assert.Equal("Buy milk", normalized);
    }
}
