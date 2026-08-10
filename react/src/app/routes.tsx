import { Navigate, Route, Routes } from 'react-router-dom';
import { DashboardPage } from '../features/dashboard/DashboardPage';
import { AccountsPage } from '../features/accounts/AccountsPage';
import { AccountDetailsPage } from '../features/accounts/AccountDetailsPage';
import { ContactsPage } from '../features/contacts/ContactsPage';
import { OpportunitiesPage } from '../features/opportunities/OpportunitiesPage';
import { PipelineKanbanPage } from '../features/pipeline/PipelineKanbanPage';
import { ForecastPage } from '../features/forecast/ForecastPage';
import { ActivitiesCalendarPage } from '../features/activities/ActivitiesCalendarPage';
import { TerritoryMapPage } from '../features/territories/TerritoryMapPage';
import { DocumentsPage } from '../features/documents/DocumentsPage';
import { AiAssistantPage } from '../features/ai-assistant/AiAssistantPage';
import { AdminSeedDataPage } from '../features/admin/AdminSeedDataPage';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<DashboardPage />} />
      <Route path="/accounts" element={<AccountsPage />} />
      <Route path="/accounts/:accountId" element={<AccountDetailsPage />} />
      <Route path="/contacts" element={<ContactsPage />} />
      <Route path="/opportunities" element={<OpportunitiesPage />} />
      <Route path="/pipeline" element={<PipelineKanbanPage />} />
      <Route path="/forecast" element={<ForecastPage />} />
      <Route path="/activities" element={<ActivitiesCalendarPage />} />
      <Route path="/territories" element={<TerritoryMapPage />} />
      <Route path="/documents" element={<DocumentsPage />} />
      <Route path="/assistant" element={<AiAssistantPage />} />
      <Route path="/admin" element={<AdminSeedDataPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
