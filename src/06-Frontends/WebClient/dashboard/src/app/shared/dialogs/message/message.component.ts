import { Component, OnInit, Input } from '@angular/core';
import { BaseModalComponent } from '../base-modal.component';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { BaseModalDataModel } from '../../models/base-modal-data.model';

@Component({
  selector: 'ts-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent extends BaseModalComponent implements OnInit {

  @Input() data: BaseModalDataModel;

  constructor(protected activeModal: NgbActiveModal) {
    super(activeModal);
  }

  ngOnInit() {
    this.data.title = this.data.title || 'Information';
  }

  confirm() {
    this.activeModal.close(true);
  }
}
