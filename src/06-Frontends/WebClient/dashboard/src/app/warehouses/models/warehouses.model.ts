import { deserialize, deserializeAs } from 'cerialize';
import { BaseModalDataModel } from 'src/app/shared/models/base-modal-data.model';
import { guid } from 'src/app/shared/types/guid.type';

export class Warehouse {
  @deserialize id: guid;
  @deserialize code: string;
  @deserialize name: string;
  @deserialize description: string;
  @deserializeAs(Date) creationDate: Date;
}

export class WarehouseCreateRequest {
  code: string;
  name: string;
  description: string;
}

export class Warehouses {
  @deserializeAs(Warehouse) warehouses: Warehouse[];
}

export class WarehouseCreateDataModel extends BaseModalDataModel {
  constructor(
    public title: string,
    public body: string,
  ) {
    super(body, title);
  }
}
