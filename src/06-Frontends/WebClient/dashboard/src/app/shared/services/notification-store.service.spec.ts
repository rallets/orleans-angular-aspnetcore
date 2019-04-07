import { TestBed, inject } from '@angular/core/testing';

import { NotificationStoreService } from './notification-store.service';
import { NotificationsService, SimpleNotificationsModule } from 'angular2-notifications';

describe('NotificationStoreService', () => {
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [NotificationStoreService, NotificationsService],
			imports: [SimpleNotificationsModule.forRoot({})]
		});
	});

	it('should be created', inject([NotificationStoreService], (service: NotificationStoreService) => {
		expect(service).toBeTruthy();
	}));
});
