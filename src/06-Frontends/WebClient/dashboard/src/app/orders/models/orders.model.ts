import { deserialize, deserializeAs } from 'cerialize';
import { BaseModalDataModel } from 'src/app/shared/models/base-modal-data.model';
import { guid } from 'src/app/shared/types/guid.type';

export class OrderItemProduct {
  @deserialize id: guid;
  @deserialize description: string;
}

export class OrderItem {
  @deserializeAs(OrderItemProduct) product: OrderItemProduct;
  @deserialize quantity: number;
  @deserialize unitPrice: number;
}

export class Order {
  @deserialize id: guid;
  @deserializeAs(Date) date: Date;
  @deserialize name: string;
  @deserialize totalAmount: number;
  @deserializeAs(OrderItem) items: OrderItem[];
}

export class Orders {
  @deserializeAs(Order) orders: Order[];
}

export class OrderCreateRequest {
  // totalAmount: number;
  // name: string;
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
