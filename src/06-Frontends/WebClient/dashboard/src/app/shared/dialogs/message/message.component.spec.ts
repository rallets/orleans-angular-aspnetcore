import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageComponent } from './message.component';
import { NgbModalModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { BaseModalDataModel } from '@app/shared/models/base-modal-data.model';

describe('MessageComponent', () => {
    let component: MessageComponent;
    let fixture: ComponentFixture<MessageComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [MessageComponent],
            imports: [NgbModalModule],
            providers: [NgbActiveModal]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(MessageComponent);
        component = fixture.componentInstance;
        component.data = new BaseModalDataModel('body with html <br/>');
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
