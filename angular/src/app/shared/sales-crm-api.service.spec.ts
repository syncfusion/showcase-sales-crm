import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SalesCrmApiService } from './sales-crm-api.service';

describe('SalesCrmApiService', () => {
  let api: SalesCrmApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(SalesCrmApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('persists an opportunity stage with the current Web API contract', () => {
    api.updateOpportunityStage('90b7a9d8-4152-4ef0-9b80-56b9e40fb450', 'Negotiation').subscribe();

    const request = http.expectOne('/api/opportunities/90b7a9d8-4152-4ef0-9b80-56b9e40fb450/stage');
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual({ stage: 'Negotiation' });
    request.flush(null);
  });

  it('normalizes a bare account array', () => {
    let accountName = '';
    api.getAccounts().subscribe((accounts) => (accountName = accounts[0].accountName));

    http.expectOne('/api/accounts').flush([{ id: '1', name: 'Northwind' }]);
    expect(accountName).toBe('Northwind');
  });
});
