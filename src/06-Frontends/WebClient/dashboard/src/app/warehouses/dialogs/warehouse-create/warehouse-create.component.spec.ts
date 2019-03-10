import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { PrislisteDetailEditComponent } from './prisliste-detail-edit.component';
import { sharedComponentImports, sharedComponentProviders } from '@app/prislister/testing.shared';
import { PrislisteEditDataModel } from '@app/prislister/models/prisliste-edit-data.model';
import { Prislist } from '@app/prislister/models/prislist.model';
import { IUser } from '@app/shared/models/user.model';
import { ITilganger } from '@app/shared/models/tilganger.model';

describe('PrislisteDetailEditComponent', () => {
    let component: PrislisteDetailEditComponent;
    let fixture: ComponentFixture<PrislisteDetailEditComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [PrislisteDetailEditComponent],
            imports: [sharedComponentImports],
            providers: [sharedComponentProviders],
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(PrislisteDetailEditComponent);
        component = fixture.componentInstance;
        const user = { Tilganger: { ErInnloggetBrukerTsAnsatt: false } as ITilganger } as IUser;
        component.data = new PrislisteEditDataModel('', '', false, user, new Prislist(), undefined);
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
