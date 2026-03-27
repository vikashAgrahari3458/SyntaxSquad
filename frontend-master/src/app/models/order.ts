export interface OrderItem {
	name: string;
	price: number;
	quantity: number;
}

export interface Order {
	id: string;
	items: OrderItem[];
	totalAmount: number;
	deliveryAddress: string;
	orderDate: Date;
}

export interface PlaceOrderPayload {
	items: OrderItem[];
	totalAmount: number;
	deliveryAddress: string;
}
