using SalesCrm.Domain.Common;

namespace SalesCrm.Domain.Entities;

public sealed class Product : AuditableEntity<Guid>
{
    public Product(Guid id, string sku, string name, decimal listPrice)
        : base(id)
    {
        Sku = RequireText(sku, nameof(sku));
        Name = RequireText(name, nameof(name));
        ListPrice = listPrice;
    }

    public string Sku { get; private set; }

    public string Name { get; private set; }

    public decimal ListPrice { get; private set; }

    public bool IsActive { get; private set; } = true;

    public void ChangePrice(decimal listPrice) => ListPrice = listPrice;

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
