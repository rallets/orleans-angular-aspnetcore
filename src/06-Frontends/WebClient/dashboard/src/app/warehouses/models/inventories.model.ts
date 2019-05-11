import { deserialize, deserializeAs } from 'cerialize';
import { Product } from 'src/app/products/models/products.model';

export class ProductStock {
	@deserializeAs(Product) product: Product;

	@deserialize currentStockQuantity: number;
	@deserialize safetyStockQuantity: number;
	@deserialize bookedQuantity: number;
	@deserialize active: boolean;
	@deserialize stateDescription: string;
}

export class Inventory {
	constructor() {
		this.productsStocks = [];
		// console.log('constr', this.productsStocks);
	}

	@deserialize id: string;
	@deserializeAs(Date) creationDate: Date | string;
	@deserialize warehouseCode: string;

	/**ProductStock information for each product */
	@deserializeAs(ProductStock) productsStocks: ProductStock[];
}
