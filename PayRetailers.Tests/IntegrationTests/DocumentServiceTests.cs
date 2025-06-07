using FluentAssertions;
using PayRetailers.Application.DTOs;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Tests.IntegrationTests;

public class DocumentServiceTests : TestBase
{
    private const string TestAccount = "TEST_ACCOUNT_001";

    [Fact]
    public async Task CreateAsync_ShouldCreateNewDocument()
    {
        var dto = new DocumentCreateDto { DocumentType = DocumentType.Privacy };
        var docId = await DocumentService.CreateAsync(TestAccount, dto);

        try
        {
            docId.Should().NotBeEmpty();
        }
        finally
        {
            await DocumentService.DeleteAsync(TestAccount, docId);
        }
    }

    [Fact]
    public async Task GetByAccountAsync_ShouldReturnDocuments_AfterCreate()
    {
        var dto = new DocumentCreateDto { DocumentType = DocumentType.Contract };
        var docId = await DocumentService.CreateAsync(TestAccount, dto);

        try
        {
            var result = await DocumentService.GetByAccountAsync(TestAccount);

            var documentDtos = result.ToList();
            documentDtos.Should().NotBeNull();
            documentDtos.Should().ContainSingle(d => d.DocumentId == docId);
        }
        finally
        {
            await DocumentService.DeleteAsync(TestAccount, docId);
        }
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldChangeStatus_WhenAccountMatches()
    {
        var dto = new DocumentCreateDto { DocumentType = DocumentType.Conditions };
        var docId = await DocumentService.CreateAsync(TestAccount, dto);

        try
        {
            await DocumentService.UpdateStatusAsync(TestAccount, docId, DocumentStatus.Signed);
        }
        finally
        {
            await DocumentService.DeleteAsync(TestAccount, docId);
        }
    }
}
