import { deserialize, deserializeAs } from 'cerialize';
import { BaseModalDataModel } from 'src/app/shared/models/base-modal-data.model';
import { guid } from 'src/app/shared/types/guid.type';

export class OrderItemProduct {
	@deserialize id: guid;
	@deserialize code: string;
	@deserialize description: string;
	@deserialize price: number;
}

export class OrderItem {
	@deserializeAs(OrderItemProduct) product: OrderItemProduct;
	@deserialize quantity: number;
}

export class Order {
	@deserialize id: guid;
	@deserialize dispatched: boolean;
	@deserializeAs(Date) date: Date;
	@deserialize name: string;
	@deserialize totalAmount: number;
	@deserializeAs(OrderItem) items: OrderItem[];
}

export class Orders {
	@deserializeAs(Order) orders: Order[];
}

export class OrderCreateRequest {
	name: string;
	items: OrderCreateItemRequest[];
}

export class OrderCreateItemRequest {
	productId: guid;
	quantity: number;
}

export class OrderCreateDataModel extends BaseModalDataModel {
	constructor(
		public title: string,
		public body: string,
	) {
		super(body, title);
	}
}
