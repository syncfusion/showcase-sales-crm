import { NavLink } from 'react-router-dom';
import { AppIcon, type AppIconName } from '../../shared/components/AppIcon';

const items: Array<{ label: string; to: string; icon: AppIconName }> = [
  { label: 'Dashboard', to: '/', icon: 'dashboard' },
  { label: 'Accounts', to: '/accounts', icon: 'accounts' },
  { label: 'Contacts', to: '/contacts', icon: 'contacts' },
  { label: 'Opportunities', to: '/opportunities', icon: 'opportunities' },
  { label: 'Pipeline Kanban', to: '/pipeline', icon: 'pipeline' },
  { label: 'Forecast', to: '/forecast', icon: 'forecast' },
  { label: 'Activities Calendar', to: '/activities', icon: 'activities' },
  { label: 'Territory Map', to: '/territories', icon: 'territories' },
  { label: 'Documents', to: '/documents', icon: 'documents' },
  { label: 'AI Sales Assistant', to: '/assistant', icon: 'assistant' },
  { label: 'Admin Seed Data', to: '/admin', icon: 'admin' }
];

export function Sidebar({ open, onNavigate }: { open: boolean; onNavigate: () => void }) {
  return (
    <nav className={`sidebar${open ? ' open' : ''}`}>
      <div className="sidebar-title">Sales CRM</div>
      {items.map(({ label, to, icon }) => (
        <div key={to} className="sidebar-item">
          <NavLink
            to={to}
            end
            className={({ isActive }) => (isActive ? 'active' : undefined)}
            onClick={onNavigate}
          >
            <AppIcon name={icon} size={17} />
            <span>{label}</span>
          </NavLink>
        </div>
      ))}
    </nav>
  );
}
