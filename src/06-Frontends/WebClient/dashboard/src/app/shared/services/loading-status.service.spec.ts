import { TestBed, inject } from '@angular/core/testing';

import { LoadingStatusService } from './loading-status.service';
import { LoadingBarService, LoadingBarModule } from '@ngx-loading-bar/core';

describe('LoadingStatusService', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [LoadingStatusService, LoadingBarService],
            imports: [LoadingBarModule]
        });
    });

    it('should be created', inject([LoadingStatusService], (service: LoadingStatusService) => {
        expect(service).toBeTruthy();
    }));
});
