import { WarehousesModule } from './warehouses.module';

describe('WarehousesModule', () => {
  let warehousesModule: WarehousesModule;

  beforeEach(() => {
    warehousesModule = new WarehousesModule();
  });

  it('should create an instance', () => {
    expect(warehousesModule).toBeTruthy();
  });
});
