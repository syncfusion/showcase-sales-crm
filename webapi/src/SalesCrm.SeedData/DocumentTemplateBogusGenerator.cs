using SalesCrm.Domain.Entities;

namespace SalesCrm.SeedData;

public sealed class DocumentTemplateBogusGenerator
{
    public IReadOnlyList<DocumentTemplate> Generate(int count)
    {
        var templates = new[]
        {
            ("enterprise-proposal", "Enterprise Subscription Proposal", "Proposal"),
            ("services-sow", "Implementation Services SOW", "Statement of Work"),
            ("renewal-quote", "Renewal Quote Packet", "Quote"),
            ("mutual-action-plan", "Mutual Action Plan", "Account Plan"),
            ("security-review", "Security Review Addendum", "Addendum"),
            ("executive-brief", "Executive Business Review", "Brief"),
            ("expansion-plan", "Expansion Opportunity Plan", "Account Plan"),
            ("legal-redline", "Contract Redline Summary", "Contract")
        };

        var documents = new List<DocumentTemplate>(count);
        for (var i = 0; i < count; i++)
        {
            var template = templates[i % templates.Length];
            documents.Add(new DocumentTemplate(
                DeterministicIds.DocumentTemplates[i],
                template.Item1,
                template.Item2,
                template.Item3));
        }

        return documents;
    }
}
