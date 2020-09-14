import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { OrderFulfillment } from '../models';
import { OrderStatus } from './order.status';

@Injectable({
    providedIn: 'root',
})
export class OrderFulfillmentService {
    private _connection: HubConnection | null;

    constructor(
        private _orderStatus: OrderStatus,
        private _toastrService: ToastrService,
    ) {}

    public connect(): Promise<void> {
        if (this._connection && this._connection.state !== HubConnectionState.Disconnected) {
            return Promise.resolve();
        }

        this._connection = new HubConnectionBuilder()
            .withUrl('/hub/order')
            .build();

        this._connection.on(OrderFulfillment.Init, (orderId: string) => {
            this._orderStatus.orderId = orderId;
        });

        this._connection.on(OrderFulfillment.Registered, () => {
            this._handleRegisteredMessage();
        });

        this._connection.on(OrderFulfillment.Completed, (orderId: string) => {
            this._handleCompletedMessage(orderId);
        });

        this._connection.on(OrderFulfillment.Failed, (error: string) => {
            this._handleFailedMessage(error);
        });

        this._connection.on(OrderFulfillment.Error, (error: string) => {
            this._handleErrorMessage(error);
        });

        return this._connection.start();
    }

    public initOrder(): void {
        this._connection.send('InitializeOrder').then();
    }

    public cancelOrder(): void {
        this._connection.send('CancelOrder').then();
    }

    public disconnect(): void {
        this._connection.stop()
            .then();
    }

    private _handleRegisteredMessage(): void {
        this._toastrService.success(`Twoje zamówienie zostało zarejestrowane. Za chwilę poinformujemy o czasie dostawy.`);
    }

    private _handleCompletedMessage(deliveryTime: string): void {
        const date = `${new Date(deliveryTime).getDate()}.${new Date(deliveryTime).getMonth() + 1}`;
        const time = `${new Date(deliveryTime).getHours()}:${new Date(deliveryTime).getMinutes()}`;

        this._toastrService.success(`Twoje zamówienie zostało przyjęte! Przewidywany czas dostawy to ${date} o godzinie ${time}.`);
    }

    private _handleFailedMessage(error: string): void {
        this._toastrService.error(`Twoje zamówienie nie mogło zostać przyjęte z powodu blędu: ${error}.`);
    }

    private _handleErrorMessage(error: string): void {
        this._toastrService.error(`Błąd w trakcie przetwarzania zamówienia: ${error}.`);
    }
}
