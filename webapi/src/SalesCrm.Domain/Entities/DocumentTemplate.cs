using SalesCrm.Domain.Common;

namespace SalesCrm.Domain.Entities;

public sealed class DocumentTemplate : EntityBase<Guid>
{
    public DocumentTemplate(Guid id, string templateKey, string name, string documentType)
        : base(id)
    {
        TemplateKey = RequireText(templateKey, nameof(templateKey));
        Name = RequireText(name, nameof(name));
        DocumentType = RequireText(documentType, nameof(documentType));
    }

    public string TemplateKey { get; private set; }

    public string Name { get; private set; }

    public string DocumentType { get; private set; }

    public bool IsActive { get; private set; } = true;

    public void Rename(string name) => Name = RequireText(name, nameof(name));

    public void Deactivate() => IsActive = false;

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
