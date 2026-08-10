import { useEffect, useState } from 'react';

/**
 * Tracks a CSS media query and returns `true` while it matches the current
 * viewport. SSR-safe: returns `false` until the client effect runs.
 *
 * Used by feature pages that render Syncfusion `ChartComponent` /
 * `GridComponent` to pick a taller `height` prop on QHD / 4K displays, so the
 * components do not look short and squashed next to a very wide container.
 */
export function useMediaQuery(query: string): boolean {
  const [matches, setMatches] = useState<boolean>(() => {
    if (typeof window === 'undefined' || !window.matchMedia) return false;
    return window.matchMedia(query).matches;
  });

  useEffect(() => {
    if (typeof window === 'undefined' || !window.matchMedia) return;
    const mql = window.matchMedia(query);
    const onChange = () => setMatches(mql.matches);
    onChange();
    mql.addEventListener('change', onChange);
    return () => mql.removeEventListener('change', onChange);
  }, [query]);

  return matches;
}

/**
 * `true` on QHD and above (>= 2560px CSS pixels). Covers 4K @ 150% Windows
 * scaling and 1440p ultrawide. Useful for bumping Syncfusion chart/grid heights
 * and matching the `@media (min-width: 2560px)` tier in styles.css.
 */
export function useIsXl(): boolean {
  return useMediaQuery('(min-width: 2560px)');
}

/**
 * `true` on native / super-ultrawide 4K (>= 3440px CSS pixels). Use for the
 * largest height tier on Syncfusion components.
 */
export function useIs2Xl(): boolean {
  return useMediaQuery('(min-width: 3440px)');
}

/**
 * Returns a Syncfusion component height string appropriate for the current
 * viewport tier. Charts and grids render with a fixed pixel height prop on
 * small/standard screens and grow progressively on QHD / 4K displays.
 *
 * @param base    Height for standard screens (< 1920px), e.g. `'320px'`.
 * @param xl      Height for QHD / 4K @ 150%  (>= 2560px).
 * @param twoXl   Height for native 4K (>= 3440px). Defaults to `xl` if omitted.
 */
export function useResponsiveHeight(base: string, xl: string, twoXl?: string): string {
  const is2Xl = useIs2Xl();
  const isXl = useIsXl();
  if (is2Xl) return twoXl ?? xl;
  if (isXl) return xl;
  return base;
}
