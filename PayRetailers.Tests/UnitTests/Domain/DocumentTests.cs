using FluentAssertions;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Tests.UnitTests.Domain;

public class DocumentTests
{
    [Fact]
    public void SetStatus_ShouldUpdateStatusAndLastChange()
    {
        // Arrange
        var doc = new Document
        {
            Account = "ACC001",
            Type = DocumentType.Privacy
        };

        var before = DateTimeOffset.UtcNow;

        // Act
        var result = doc.SetStatus(DocumentStatus.Signed);

        // Assert
        doc.Status.Should().Be(DocumentStatus.Signed);
        result.Should().Be(DocumentStatus.Signed);
        doc.LastChange.Should().BeAfter(before).And.BeBefore(DateTimeOffset.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Document_ShouldGenerateIdOnInit()
    {
        // Act
        var doc = new Document
        {
            Account = "ACC002",
            Type = DocumentType.Conditions
        };

        // Assert
        doc.Id.Should().NotBeEmpty();
    }
}