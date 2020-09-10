import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { OrderFulfillment } from '../models';
import { OrderStatus } from './order.status';

@Injectable({
    providedIn: 'root',
})
export class OrderFulfillmentService {
    private _connection: HubConnection | null;

    constructor(
        private _orderStatus: OrderStatus,
    ) {}

    public connect(): void {
        if (this._connection && this._connection.state !== HubConnectionState.Disconnected) {
            return;
        }

        this._connection = new HubConnectionBuilder()
            .withUrl('/hub/order')
            .build();

        this._connection.on(OrderFulfillment.Init, (orderId: string) => {
            this._orderStatus.orderId = orderId;
        });

        this._connection.on(OrderFulfillment.Registered, (orderId: string) => {
            this._handleOrderMessage(orderId);
        });

        this._connection.start()
            .then();
    }

    public disconnect(): void {
        this._connection.stop()
            .then();
    }

    private _handleOrderMessage(message: any): void {
        console.log(message);
    }
}
