import { Moon, Sun } from 'lucide-react';
import { useEffect, useState } from 'react';

type ThemeMode = 'light' | 'dark';

function getInitialTheme(): ThemeMode {
  if (typeof window === 'undefined') return 'light';
  return (window.localStorage.getItem('sales-crm-theme') as ThemeMode | null) ?? 'light';
}

export function ThemeToggle() {
  const [theme, setTheme] = useState<ThemeMode>(getInitialTheme);

  useEffect(() => {
    document.documentElement.dataset.theme = theme;
    document.body.classList.toggle('e-dark-mode', theme === 'dark');
    window.localStorage.setItem('sales-crm-theme', theme);
    // Notify same-tab listeners (charts/maps) that the theme changed so they can re-render.
    window.dispatchEvent(new Event('sales-crm-theme-change'));
  }, [theme]);

  const isDark = theme === 'dark';

  return (
    <button
      type="button"
      className="theme-toggle inline-flex items-center gap-2 rounded-md border px-3 py-2 text-sm font-semibold transition"
      onClick={() => setTheme(isDark ? 'light' : 'dark')}
      aria-label={isDark ? 'Switch to light theme' : 'Switch to dark theme'}
    >
      {isDark ? <Sun aria-hidden="true" size={16} /> : <Moon aria-hidden="true" size={16} />}
      <span>{isDark ? 'Light' : 'Dark'}</span>
    </button>
  );
}
