import { Injectable } from '@angular/core';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { Observable } from 'rxjs';
import { delay, withLatestFrom, map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class LoadingStatusService {

    loading$: Observable<boolean>;
    loadingDelayed$: Observable<boolean>;

    constructor(
        private loader: LoadingBarService,
    ) {
        this.loading$ = loader.progress$.pipe(
            map(v => {
                return v > 0;
            })
        );

        this.loadingDelayed$ = loader.progress$.pipe(
            delay(500),
            withLatestFrom(loader.progress$),
            map(v => v[1] > 0),
        );

    }

}
