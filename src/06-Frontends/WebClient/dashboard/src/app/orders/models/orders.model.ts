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

export class OrderEvent {
	@deserialize name: string;
	@deserializeAs(Date) date: Date;
	@deserialize description: string;
}

export class OrderEvents {
	@deserializeAs(OrderEvent) events: OrderEvent[];
}

export class Order {
	@deserialize id: guid;
	@deserialize dispatched: boolean;
	@deserializeAs(Date) date: Date;
	@deserialize name: string;
	@deserialize totalAmount: number;
	@deserializeAs(OrderItem) items: OrderItem[];

	@deserializeAs(OrderEvent) events: OrderEvent[];
}

export class Orders {
	@deserializeAs(Order) orders: Order[];
}

export class OrdersStats {
	@deserialize all: number;
	@deserialize notDispatched: number;
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
