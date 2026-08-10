import { useState } from 'react';
import { DocumentEditorContainerComponent, Inject, Toolbar } from '@syncfusion/ej2-react-documenteditor';
import { PageHeader } from '../../shared/components/PageHeader';
import { LoadingState } from '../../shared/components/LoadingState';
import { EmptyState } from '../../shared/components/EmptyState';
import { ErrorState } from '../../shared/components/ErrorState';
import { useAsyncData } from '../../shared/hooks/useAsyncData';
import { useResponsiveHeight } from '../../shared/hooks/useMediaQuery';
import { salesCrmApi } from '../../shared/api/salesCrmApi';
import { AppIcon } from '../../shared/components/AppIcon';

export function DocumentsPage() {
  const { data, loading, error } = useAsyncData(() => salesCrmApi.getDocuments(), []);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  // Taller document editor on QHD / 4K so more proposal content stays visible.
  const editorHeight = useResponsiveHeight('560px', '660px', '720px');

  if (loading) return <LoadingState />;
  if (error) return <ErrorState message={error} />;
  if (!data?.length) return <EmptyState message="No documents found." />;

  const selected = data.find((doc) => doc.id === (selectedId ?? data[0].id)) ?? data[0];
  const proposalText = [
      `${selected.title}`,
      '',
      `Account: ${selected.account}`,
      `Opportunity: ${selected.opportunity}`,
      `Owner: ${selected.owner}`,
      `Status: ${selected.status}`,
      '',
      'Executive Summary',
      `${selected.account} is evaluating a revenue workflow improvement. This document captures the commercial proposal, next steps, and mutual action plan for the buying team.`,
      '',
      'Recommended Next Steps',
      '1. Confirm decision criteria and security owner.',
      '2. Align implementation milestones with the target close date.',
      '3. Finalize pricing, legal review, and executive approval.'
    ].join('\n');

  return (
    <section>
      <PageHeader title="Documents" subtitle="Manage proposals, templates, and generated artifacts." icon={<AppIcon name="documents" />} />
      <div className="content-card">
        <div className="documents-layout">
          <aside className="documents-list">
            <h2 style={{ marginTop: 0 }}>Document Templates</h2>
            {data.slice(0, 12).map((doc) => (
              <button
                key={doc.id}
                className={doc.id === selected.id ? 'document-list-item active' : 'document-list-item'}
                type="button"
                onClick={() => setSelectedId(doc.id)}
              >
                <strong>{doc.title}</strong>
                <span>{doc.type} · {doc.account}</span>
              </button>
            ))}
          </aside>
          <div className="document-editor-panel">
            <div className="document-meta">
              <div>
                <h2>{selected.title}</h2>
                <p>{selected.account} · {selected.opportunity}</p>
              </div>
              <span>{selected.status}</span>
            </div>
            <div className="w-full overflow-hidden">
              <DocumentEditorContainerComponent
                key={selected.id}
                id="sales-document-editor"
                height={editorHeight}
                enableToolbar={true}
                restrictEditing={false}
                created={() => {
                  const editor = (document.getElementById('sales-document-editor') as any)?.ej2_instances?.[0]?.documentEditor;
                  if (editor) {
                    editor.openBlank();
                    editor.editor.insertText(proposalText);
                  }
                }}
              >
                <Inject services={[Toolbar]} />
              </DocumentEditorContainerComponent>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
