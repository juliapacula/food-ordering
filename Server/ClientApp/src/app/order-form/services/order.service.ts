import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Order } from '../models';

@Injectable()
export class OrderService {
    constructor(
        private _httpClient: HttpClient,
    ) { }

    public add(order: Order): Observable<void> {
        return this._httpClient.post<void>('/api/orders', order);
    }
}
