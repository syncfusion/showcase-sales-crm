import { lastValueFrom, of, throwError, toArray } from 'rxjs';
import { toLoadState } from './load-state';

describe('toLoadState', () => {
  it('emits loading before successful data', async () => {
    const states = await lastValueFrom(toLoadState(of(['account'])).pipe(toArray()));
    expect(states).toEqual([
      { status: 'loading', data: null, error: null },
      { status: 'success', data: ['account'], error: null },
    ]);
  });

  it('preserves an API error instead of reporting an empty data set', async () => {
    const states = await lastValueFrom(
      toLoadState(throwError(() => new Error('CRM unavailable'))).pipe(toArray()),
    );
    expect(states.at(-1)).toEqual({ status: 'error', data: null, error: 'CRM unavailable' });
  });
});
