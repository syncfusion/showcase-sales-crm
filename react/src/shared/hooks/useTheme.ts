import { useEffect, useState } from 'react';

export type ThemeMode = 'light' | 'dark';

function getInitialTheme(): ThemeMode {
  if (typeof window === 'undefined') return 'light';
  return (window.localStorage.getItem('sales-crm-theme') as ThemeMode | null) ?? 'light';
}

/**
 * Tracks the current Sales CRM theme ('light' | 'dark'). Subscribes to the same
 * `sales-crm-theme` localStorage key the ThemeToggle component writes to, so any
 * Syncfusion component that needs a theme-specific value (charts, maps) stays in
 * sync when the user toggles the header theme button.
 *
 * Also listens for the `storage` event (cross-tab) and a custom `sales-crm-theme-change`
 * event (same-tab, dispatched by ThemeToggle) so the chart re-renders immediately
 * without waiting for a reload.
 */
export function useTheme() {
  const [theme, setTheme] = useState<ThemeMode>(getInitialTheme);

  useEffect(() => {
    const sync = () => setTheme(getInitialTheme());
    window.addEventListener('storage', sync);
    window.addEventListener('sales-crm-theme-change', sync);
    return () => {
      window.removeEventListener('storage', sync);
      window.removeEventListener('sales-crm-theme-change', sync);
    };
  }, []);

  const isDark = theme === 'dark';
  return {
    theme,
    isDark,
    /** Syncfusion EJ2 chart/map `theme` value: Tailwind3 (light) or Tailwind3Dark (dark). */
    syncfusionTheme: isDark ? 'Tailwind3Dark' : 'Tailwind3'
  } as const;
}
