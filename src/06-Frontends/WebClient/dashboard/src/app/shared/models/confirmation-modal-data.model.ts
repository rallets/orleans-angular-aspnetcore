import { BaseModalDataModel } from './base-modal-data.model';

export class ConfirmationModalDataModel extends BaseModalDataModel {
	constructor(
		public body: string,
		public title?: string,
	) {
		super(body, title);
	}
}
