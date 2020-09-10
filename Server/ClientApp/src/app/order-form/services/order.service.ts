import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Order } from '../models';
import { OrderStatus } from './order.status';

@Injectable()
export class OrderService {
    constructor(
        private _httpClient: HttpClient,
        private _orderStatus: OrderStatus,
    ) { }

    public add(order: Order): Observable<void> {
        return this._httpClient.post<void>('/api/orders', {
            id: this._orderStatus.orderId,
            ...order,
        });
    }
}
