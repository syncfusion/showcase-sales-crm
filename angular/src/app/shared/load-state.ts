import { Observable, catchError, map, of, startWith } from 'rxjs';

export type LoadState<T> =
  | { status: 'loading'; data: null; error: null }
  | { status: 'success'; data: T; error: null }
  | { status: 'error'; data: null; error: string };

export function toLoadState<T>(source: Observable<T>): Observable<LoadState<T>> {
  return source.pipe(
    map((data): LoadState<T> => ({ status: 'success', data, error: null })),
    startWith({ status: 'loading', data: null, error: null } as LoadState<T>),
    catchError((error: unknown) =>
      of({ status: 'error', data: null, error: getErrorMessage(error) } as LoadState<T>),
    ),
  );
}

export function getErrorMessage(error: unknown, fallback = 'Unable to load CRM data.') {
  if (error instanceof Error && error.message.trim()) return error.message;
  return fallback;
}
