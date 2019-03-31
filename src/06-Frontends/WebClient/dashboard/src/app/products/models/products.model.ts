import { deserialize, deserializeAs } from 'cerialize';
import { BaseModalDataModel } from 'src/app/shared/models/base-modal-data.model';
import { guid } from 'src/app/shared/types/guid.type';

export class Product {
  @deserialize id: guid;
  @deserializeAs(Date) creationDate: Date;
  @deserialize code: string;
  @deserialize name: string;
  @deserialize description: string;
  @deserialize price: number;
}

export class ProductCreateRequest {
  code: string;
  name: string;
  description: string;
  price: number;
}

export class Products {
  @deserializeAs(Product) products: Product[];
}

export class ProductCreateDataModel extends BaseModalDataModel {
  constructor(
    public title: string,
    public body: string,
  ) {
    super(body, title);
  }
}
