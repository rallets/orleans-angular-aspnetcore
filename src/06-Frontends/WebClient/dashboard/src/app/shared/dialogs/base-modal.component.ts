import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

export class BaseModalComponent {

    protected DELAY_CLOSE_MS = 1000;

    constructor(protected activeModal: NgbActiveModal) {
    }

    public dismiss() {
        this.activeModal.dismiss();
    }
}
