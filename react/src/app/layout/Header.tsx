import { ThemeToggle } from '../../shared/components/ThemeToggle';
import { AppIcon } from '../../shared/components/AppIcon';

export function Header({ onMenuClick }: { onMenuClick: () => void }) {
  return (
    <div className="topbar">
      <div className="topbar-left">
        <button
          type="button"
          className="sidebar-toggle"
          onClick={onMenuClick}
          aria-label="Toggle navigation"
        >
          <AppIcon name="menu" size={20} />
        </button>
        <strong>Sales CRM Pipeline Intelligence</strong>
      </div>
      <ThemeToggle />
    </div>
  );
}
