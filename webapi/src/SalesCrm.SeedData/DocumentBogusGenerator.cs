using SalesCrm.Domain.Entities;

namespace SalesCrm.SeedData;

public sealed class DocumentBogusGenerator
{
    public IReadOnlyList<SalesDocument> Generate(IReadOnlyList<Opportunity> opportunities, IReadOnlyList<DocumentTemplate> templates, IReadOnlyList<User> users, int count)
    {
        if (opportunities.Count == 0)
        {
            throw new InvalidOperationException("Cannot generate sales documents without opportunities.");
        }

        if (count > DeterministicIds.SalesDocuments.Length)
        {
            throw new InvalidOperationException($"Document count {count} exceeds deterministic ID count {DeterministicIds.SalesDocuments.Length}.");
        }

        var items = new List<SalesDocument>(count);

        for (var i = 0; i < count; i++)
        {
            var opportunity = opportunities[i % opportunities.Count];
            var template = templates[i % templates.Count];

            items.Add(new SalesDocument(
                DeterministicIds.SalesDocuments[i],
                opportunity.AccountId,
                opportunity.Id,
                users[(i + 4) % users.Count].Id,
                template.DocumentType,
                $"{template.Name} - Deal {i + 1:000}",
                template.TemplateKey));
        }

        return items;
    }
}
