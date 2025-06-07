using FluentAssertions;
using Moq;
using PayRetailers.Application.DTOs;
using PayRetailers.Application.Services;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Repositories;

namespace PayRetailers.Tests.UnitTests.Application;

public class DocumentServiceTests
{
    private readonly Mock<IDocumentRepository> _repoMock = new();

    private DocumentService CreateService() => new(_repoMock.Object);

    [Fact]
    public async Task GetByAccountAsync_ShouldReturnMappedDTOs()
    {
        // Arrange
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Account = "ACC123",
            Type = DocumentType.Contract
        };
        document.SetStatus(DocumentStatus.Signed);

        var documents = new List<Document> { document };

        _repoMock.Setup(r => r.GetByAccountAsync("ACC123")).ReturnsAsync(documents);

        var service = CreateService();

        // Act
        var result = await service.GetByAccountAsync("ACC123");

        // Assert
        var documentDtos = result.ToList();
        documentDtos.Should().HaveCount(1);
        documentDtos.First().DocumentId.Should().Be(document.Id);
        documentDtos.First().DocumentType.Should().Be(DocumentType.Contract);
        documentDtos.First().Status.Should().Be(DocumentStatus.Signed);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDocumentWithNewStatus()
    {
        // Arrange
        var dto = new DocumentCreateDto { DocumentType = DocumentType.Privacy };
        Document? createdDoc = null;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Document>()))
                 .Callback<Document>(d => createdDoc = d)
                 .ReturnsAsync(Guid.NewGuid());

        var service = CreateService();

        // Act
        var result = await service.CreateAsync("ACC001", dto);

        // Assert
        result.Should().NotBeEmpty();
        createdDoc.Should().NotBeNull();
        createdDoc!.Account.Should().Be("ACC001");
        createdDoc.Type.Should().Be(DocumentType.Privacy);
        createdDoc.Status.Should().Be(DocumentStatus.New);
        createdDoc.LastChange.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenDocumentIsValid()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var doc = new Document
        {
            Id = docId,
            Account = "ACC111",
            Type = DocumentType.Conditions
        };
        doc.SetStatus(DocumentStatus.New);

        _repoMock.Setup(r => r.GetAsync(docId)).ReturnsAsync(doc);

        var service = CreateService();

        // Act
        await service.UpdateStatusAsync("ACC111", docId, DocumentStatus.Signed);

        // Assert
        doc.Status.Should().Be(DocumentStatus.Signed);
        _repoMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenDocumentNotFound()
    {
        _repoMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Document?)null);
        var service = CreateService();

        var act = async () => await service.UpdateStatusAsync("ACC1", Guid.NewGuid(), DocumentStatus.Signed);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Document not found");
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenAccountMismatch()
    {
        var docId = Guid.NewGuid();
        var doc = new Document
        {
            Id = docId,
            Account = "ACC999",
            Type = DocumentType.Contract
        };

        _repoMock.Setup(r => r.GetAsync(docId)).ReturnsAsync(doc);

        var service = CreateService();

        var act = async () => await service.UpdateStatusAsync("OTHER_ACC", docId, DocumentStatus.Signed);

        await act.Should().ThrowAsync<Exception>().WithMessage("Document does not belong to this account");
    }
}
