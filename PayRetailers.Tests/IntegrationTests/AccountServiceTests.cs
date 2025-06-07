using FluentAssertions;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Tests.IntegrationTests;

public class AccountServiceTests : TestBase
{
    [Fact]
    public async Task ShouldReturnAccount_WhenExistsInPayBro()
    {
        var dto = await AccountService.GetAccountDetailsAsync("ES7421002763293649356828");

        dto.Should().NotBeNull();
        dto.CustomerAccount.Should().Be("ES7421002763293649356828");
        dto.Provider.Should().Be(Provider.PayBro);
        dto.Transactions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnAccount_WhenExistsInBankvolat()
    {
        var dto = await AccountService.GetAccountDetailsAsync("EU3344303045602386643667");

        dto.Should().NotBeNull();
        dto.CustomerAccount.Should().Be("EU3344303045602386643667");
        dto.Provider.Should().Be(Provider.Bankvolat);
        dto.Transactions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldThrowKeyNotFound_WhenAccountNotExists()
    {
        var act = async () => await AccountService.GetAccountDetailsAsync("NON_EXISTENT_123");

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*No provider found*");
    }
}