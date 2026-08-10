import { AsyncPipe } from '@angular/common';
import { Component, ViewChild, inject, signal } from '@angular/core';
import {
  DocumentEditorContainerAllModule,
  DocumentEditorContainerComponent,
} from '@syncfusion/ej2-angular-documenteditor';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { SalesCrmApiService } from '../../shared/sales-crm-api.service';
import {
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent,
} from '../../shared/state.component';
import { DocumentTemplate } from '../../shared/models';
import { toLoadState } from '../../shared/load-state';

@Component({
  selector: 'app-documents-page',
  imports: [
    AsyncPipe,
    DocumentEditorContainerAllModule,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    PageHeaderComponent,
  ],
  template: `
    <section>
      <app-page-header
        title="Documents"
        subtitle="Manage proposals, templates, and generated artifacts."
        icon="documents"
      />
      @if (state$ | async; as state) {
        @if (state.status === 'loading') {
          <app-loading-state />
        } @else if (state.status === 'error') {
          <app-error-state [message]="state.error" />
        } @else if (state.data.length) {
          @let documents = state.data;
          <div class="content-card">
            <div class="documents-layout">
              <aside class="documents-list">
                <h2>Document Templates</h2>
                @for (doc of documents.slice(0, 12); track doc.id) {
                  <button
                    type="button"
                    [class]="
                      selected(doc, documents).id === doc.id
                        ? 'document-list-item active'
                        : 'document-list-item'
                    "
                    (click)="select(doc)"
                  >
                    <strong>{{ doc.title }}</strong>
                    <span>{{ doc.type }} · {{ doc.account }}</span>
                  </button>
                }
              </aside>

              <div class="document-editor-panel">
                <div class="document-meta">
                  <div>
                    <h2>{{ selected(documents[0], documents).title }}</h2>
                    <p>
                      {{ selected(documents[0], documents).account }} ·
                      {{ selected(documents[0], documents).opportunity }}
                    </p>
                  </div>
                  <span>{{ selected(documents[0], documents).status }}</span>
                </div>
                <ejs-documenteditorcontainer
                  #editor
                  height="560px"
                  [enableToolbar]="true"
                  [restrictEditing]="false"
                  (created)="loadDocument(selected(documents[0], documents))"
                />
              </div>
            </div>
          </div>
        } @else {
          <app-empty-state message="No documents found." />
        }
      }
    </section>
  `,
})
export class DocumentsPageComponent {
  private readonly api = inject(SalesCrmApiService);
  @ViewChild('editor') private editor?: DocumentEditorContainerComponent;
  readonly state$ = toLoadState(this.api.getDocuments());
  private readonly selectedId = signal<string | null>(null);

  selected(fallback: DocumentTemplate, documents: DocumentTemplate[]) {
    return documents.find((doc) => doc.id === (this.selectedId() ?? fallback.id)) ?? fallback;
  }

  select(doc: DocumentTemplate) {
    this.selectedId.set(doc.id);
    this.loadDocument(doc);
  }

  loadDocument(doc: DocumentTemplate) {
    const proposalText = [
      doc.title,
      '',
      `Account: ${doc.account}`,
      `Opportunity: ${doc.opportunity}`,
      `Owner: ${doc.owner}`,
      `Status: ${doc.status}`,
      '',
      'Executive Summary',
      `${doc.account} is evaluating a revenue workflow improvement. This document captures the commercial proposal, next steps, and mutual action plan for the buying team.`,
      '',
      'Recommended Next Steps',
      '1. Confirm decision criteria and security owner.',
      '2. Align implementation milestones with the target close date.',
      '3. Finalize pricing, legal review, and executive approval.',
    ].join('\n');

    setTimeout(() => {
      const documentEditor = this.editor?.documentEditor;
      documentEditor?.openBlank();
      documentEditor?.editor.insertText(proposalText);
    });
  }
}
